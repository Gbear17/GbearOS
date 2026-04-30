// Copyright (c) 2026 Garrett Wyrick.
//
// [GbearOS Component]
// Name: Program.cs (PB1_Core)
// Purpose: PB1 entry point — load config, run inventory/refinery/ice/power subsystems, wrap/broadcast DTO telemetry over IGC.
// PB Association: PB1
// Dependencies: Config, ConfigParser, InventoryScanner, RefineryManager, IceManager, PowerManager, DTO types, IGCChannels, Serializer, SenderEnvelope
// Key Methods: Main (PBLimiter + tick; entry surface — internal macro tick helpers are private)

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // ---------------------------------------------------------
        // Fields
        // ---------------------------------------------------------

        // Config
        private Config _config;
        private ConfigParser _configParser;

        // Subsystems
        private InventoryScanner _inventoryScanner;
        private RefineryManager _refineryManager;
        private IceManager _iceManager;
        private PowerManager _powerManager;

        // IGC
        private IMyBroadcastListener _pb2Listener;

        // Cached DTOs
        private InventorySummaryDTO _inventorySummaryDto = new InventorySummaryDTO();
        private RefineryStatusDTO _refineryStatusDto = new RefineryStatusDTO();
        private IceStatusDTO _iceStatusDto = new IceStatusDTO();
        private PowerStatusDTO _powerStatusDto = new PowerStatusDTO();
        private WarningDTO _warningDto = new WarningDTO();
        private InventoryDynamicDTO _inventoryDynamicDto = new InventoryDynamicDTO();

        private readonly StringBuilder _statusLcdBuilder = new StringBuilder(768);

        private List<IMyTerminalBlock> _globalBlocks = new List<IMyTerminalBlock>();
        private readonly List<IMyCargoContainer> _cachedCargo = new List<IMyCargoContainer>();
        private readonly List<IMyRefinery> _cachedRefineries = new List<IMyRefinery>();
        private readonly List<IMyGasTank> _cachedGasTanks = new List<IMyGasTank>();
        private readonly List<IMyGasGenerator> _cachedGasGenerators = new List<IMyGasGenerator>();
        private readonly List<IMyBatteryBlock> _cachedBatteries = new List<IMyBatteryBlock>();
        private bool _cachedAssemblerStalled;
        private int _tickCounter = 0;

        private const int DbgMatrixHoldUpdate10Ticks = 30; // ~5s at Update10 (10 ticks per update)
        private int _dbgMatrixHoldTicksRemaining = 0;
        private string _dbgMatrixHoldText = null;

        /// <summary>Torch Advanced PB Limiter: skip ticks until this UTC time after graceful shutdown.</summary>
        private DateTime _pblTime = DateTime.MinValue;

        /// <summary>Last graceful-shutdown reason (Torch / diagnostics).</summary>
        private string _pblReason = string.Empty;

        private const float Cfw = 95f;
        private readonly List<MyProductionItem> _assemblerQueueScratch = new List<MyProductionItem>();

        // ---------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            // Initialize config parser
            _configParser = new ConfigParser(Me);

            // Load, clean, validate, and write defaults
            _config = _configParser.LoadConfig();

            if (string.IsNullOrEmpty(_config.NetworkSharedKey))
            {
                const string w = "GbearOS WARNING: [Network] SharedKey is empty — IGC DTOs use SenderEnvelope with no shared secret (unauthenticated). Set SharedKey on PB1 and PB2 CustomData.";
                Echo(w);
            }

            // Wire InventoryScanner static context
            InventoryScanner.Api = this;
            InventoryScanner.AppConfig = _config;

            // Initialize subsystems
            _inventoryScanner = new InventoryScanner();
            _refineryManager = new RefineryManager();
            _iceManager = new IceManager();
            _powerManager = new PowerManager();

            // Initialize IGC
            _pb2Listener = IGC.RegisterBroadcastListener(IGCChannels.PB2ToPB1);
            _pb2Listener.SetMessageCallback("PB2_MSG");
        }

        public void Save()
        {
        }

        // ---------------------------------------------------------
        // Main Entry Point
        // ---------------------------------------------------------
        public void Main(string argument, UpdateType updateSource)
        {
            if (PBLimiter(argument))
            {
                return;
            }

            try
            {
                if (_dbgMatrixHoldTicksRemaining > 0 && !string.IsNullOrEmpty(_dbgMatrixHoldText))
                {
                    Echo(_dbgMatrixHoldText);
                    if ((updateSource & UpdateType.Update10) != 0)
                    {
                        _dbgMatrixHoldTicksRemaining--;
                    }
                    return;
                }

                if (!string.IsNullOrEmpty(argument) && string.Equals(argument, "DBG_MATRIX", StringComparison.OrdinalIgnoreCase))
                {
                    EmitModdedBlockDebugMatrix();
                    return;
                }

                if ((updateSource & UpdateType.Update10) != 0)
                {
                    U();
                    string status = BuildStatusDashboardText();
                    Echo(status);
                    IGC.SendBroadcastMessage(IGCChannels.SYS_STATUS, status);
                }

                if ((updateSource & UpdateType.IGC) != 0)
                {
                    ProcessIGCMessages();
                }
            }
            catch (Exception e)
            {
                if (IsGracefulShutDown(e))
                {
                    _pblTime = DateTime.UtcNow.AddSeconds(60);
                    _pblReason = e.Message ?? string.Empty;
                    return;
                }

                string err = "PB1 ERROR:\n" + e.ToString();
                Echo(err);
                IGC.SendBroadcastMessage(IGCChannels.SYS_STATUS, err);
            }
        }

        private void EmitModdedBlockDebugMatrix()
        {
            if (_config == null || !_config.EnableDebug)
            {
                Echo("DBG_MATRIX: blocked (set [Debug] EnableDebug=true).");
                return;
            }

            _globalBlocks.Clear();
            GridTerminalSystem.GetBlocks(_globalBlocks);

            const int rowCap = 120;
            int rows = 0;

            var sb = new StringBuilder(4096);
            sb.AppendLine("=== DBG_MATRIX (PB1) ===");
            sb.Append("PBID: ");
            sb.AppendLine(_config.PBID ?? "CMD-????");
            sb.Append("Blocks: ");
            sb.AppendLine(_globalBlocks.Count.ToString());
            sb.AppendLine("Cols: name | inv? | invN | Cargo Ref Asm Prod Pwr Reac Bat GasGen GasTank Solar | TypeId/SubtypeId");
            sb.AppendLine();

            for (int i = 0; i < _globalBlocks.Count && rows < rowCap; i++)
            {
                var b = _globalBlocks[i];
                if (b == null || !b.IsSameConstructAs(Me))
                {
                    continue;
                }

                var owner = b as IMyInventoryOwner;
                int invN = owner != null ? owner.InventoryCount : 0;
                bool hasInv = invN > 0;

                bool isCargo = b as IMyCargoContainer != null;
                bool isRef = b as IMyRefinery != null;
                bool isAsm = b as IMyAssembler != null;
                bool isProd = b as IMyProductionBlock != null;
                bool isPwr = b as IMyPowerProducer != null;
                bool isReac = b as IMyReactor != null;
                bool isBat = b as IMyBatteryBlock != null;
                bool isGasGen = b as IMyGasGenerator != null;
                bool isGasTank = b as IMyGasTank != null;
                bool isSolar = b as IMySolarPanel != null;

                // Keep output focused: include blocks that are likely relevant OR have inventory.
                if (!hasInv && !(isCargo || isRef || isAsm || isProd || isPwr || isReac || isBat || isGasGen || isGasTank || isSolar))
                {
                    continue;
                }

                string typeId = "";
                string subtypeId = "";
                try
                {
                    var def = b.BlockDefinition;
                    typeId = def.TypeIdString ?? "";
                    subtypeId = def.SubtypeId ?? "";
                }
                catch
                {
                    // Some modded blocks or API edges may throw; debug should degrade safely.
                }

                sb.Append(FormattingUtils.SanitizeIngressWireText(b.CustomName ?? "(null)"));
                sb.Append(" | ");
                sb.Append(hasInv ? "Y" : "N");
                sb.Append(" | ");
                sb.Append(invN);
                sb.Append(" | ");
                sb.Append(isCargo ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isRef ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isAsm ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isProd ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isPwr ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isReac ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isBat ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isGasGen ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isGasTank ? "Y" : "N");
                sb.Append(" ");
                sb.Append(isSolar ? "Y" : "N");
                sb.Append(" | ");
                sb.Append(typeId);
                sb.Append("/");
                sb.AppendLine(subtypeId);

                rows++;
            }

            sb.AppendLine();
            sb.Append("Rows: ");
            sb.Append(rows);
            sb.Append(" (cap ");
            sb.Append(rowCap);
            sb.AppendLine(")");

            _dbgMatrixHoldText = sb.ToString();
            _dbgMatrixHoldTicksRemaining = DbgMatrixHoldUpdate10Ticks;
            Echo(_dbgMatrixHoldText);
        }

        /// <summary>Torch PB Limiter hook — true skips this tick (cooldown active).</summary>
        private bool PBLimiter(string argument)
        {
            if (argument == null)
            {
                // Scheduled ticks use null argument; reserved for Torch/limiter extensions.
            }

            return DateTime.UtcNow < _pblTime;
        }

        private static bool IsGracefulShutDown(Exception e)
        {
            for (Exception x = e; x != null; x = x.InnerException)
            {
                string n = x.GetType().Name;
                if (n.IndexOf("GracefulShutDown", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        // ---------------------------------------------------------
        // PB tick body (runs on UpdateFrequency / UpdateType 10)
        // ---------------------------------------------------------
        private void U()
        {
            if (_tickCounter % 5 == 0 && _inventoryScanner.InvPassIdle())
            {
                _globalBlocks.Clear();
                GridTerminalSystem.GetBlocks(_globalBlocks);
                InventoryScanner.FillConstructBlockCaches(
                    _globalBlocks,
                    _cachedCargo,
                    _cachedRefineries,
                    _cachedGasTanks,
                    _cachedGasGenerators,
                    _cachedBatteries);
            }

            int phaseThisTick = _tickCounter % 5;
            bool phaseDone = false;
            switch (phaseThisTick)
            {
                case 0:
                    phaseDone = _inventoryScanner.InventoryPassStep(
                        _globalBlocks,
                        _cachedCargo,
                        _cachedRefineries,
                        ref _inventorySummaryDto,
                        ref _inventoryDynamicDto);
                    if (phaseDone)
                    {
                        _configParser.TryPersistModdedIngotTargets(_config, _inventoryScanner.LastScanModdedOres);
                    }

                    break;
                case 1:
                    phaseDone = _refineryManager.RefineryPassStep(_globalBlocks, _cachedRefineries, ref _refineryStatusDto);
                    if (phaseDone)
                    {
                        _cachedAssemblerStalled = ComputeAssemblerStalled(_globalBlocks);
                    }

                    break;
                case 2:
                    phaseDone = _iceManager.IcePassStep(
                        _globalBlocks,
                        _cachedCargo,
                        _cachedGasGenerators,
                        _cachedGasTanks,
                        ref _iceStatusDto);
                    break;
                case 3:
                    phaseDone = _powerManager.PowerPassStep(_globalBlocks, _cachedBatteries, ref _powerStatusDto);
                    break;
                case 4:
                    phaseDone = true;
                    break;
            }

            if (phaseDone)
            {
                _tickCounter++;
            }

            // Warnings + IGC telemetry must not wait for phase 4: if inventory/refinery passes span many ticks,
            // _tickCounter never reached 4 and DTOs were never broadcast (SYS_STATUS still sent every tick).
            PopulateWarningBooleans();
            ComputeWarningState(_warningDto);

            if (_config.EnableNetwork && !string.IsNullOrEmpty(_config.NetworkSharedKey))
            {
                SendDto(IGCChannels.PB1ToPB2_InventorySummary, _inventorySummaryDto);
                SendDto(IGCChannels.PB1ToPB2_RefineryStatus, _refineryStatusDto);
                SendDto(IGCChannels.PB1ToPB2_IceStatus, _iceStatusDto);
                SendDto(IGCChannels.PB1ToPB2_PowerStatus, _powerStatusDto);
                SendDto(IGCChannels.PB1ToPB2_InventoryDynamic, _inventoryDynamicDto);
            }

            SendDto(IGCChannels.PB1_WARNINGS, _warningDto);

        }

        private string BuildStatusDashboardText()
        {
            _statusLcdBuilder.Clear();
            _statusLcdBuilder.Append("=== ");
            _statusLcdBuilder.Append(_config.PBID);
            _statusLcdBuilder.AppendLine(" ORCHESTRATOR ===");
            _statusLcdBuilder.Append("Instructions: ");
            _statusLcdBuilder.Append(Runtime.CurrentInstructionCount);
            _statusLcdBuilder.Append(" / ");
            _statusLcdBuilder.Append(Runtime.MaxInstructionCount);
            _statusLcdBuilder.AppendLine();
            _statusLcdBuilder.Append("Last Run: ");
            _statusLcdBuilder.Append(Runtime.LastRunTimeMs.ToString("F4"));
            _statusLcdBuilder.AppendLine(" ms");
            _statusLcdBuilder.Append("CARGO: ");
            if (_inventorySummaryDto.cargoPercent >= Cfw)
            {
                _statusLcdBuilder.AppendLine("FULL");
            }
            else
            {
                _statusLcdBuilder.AppendLine("Nominal");
            }

            _statusLcdBuilder.Append("POWER: ");
            if (_powerStatusDto.lowPower)
            {
                _statusLcdBuilder.AppendLine("LOW CHARGE");
            }
            else
            {
                _statusLcdBuilder.AppendLine("Nominal");
            }

            _statusLcdBuilder.Append(_refineryManager.GetCascadeDiagnostic(
                _config != null && _config.EnableRefineryBalancing));
            return _statusLcdBuilder.ToString();
        }

        // ---------------------------------------------------------
        // IGC Handling
        // ---------------------------------------------------------
        private void ProcessIGCMessages()
        {
            while (_pb2Listener.HasPendingMessage)
            {
                var msg = _pb2Listener.AcceptMessage();

                // PB2 → PB1 messages (if any) handled here
                // Currently unused, but reserved for future expansion
            }
        }


        // ---------------------------------------------------------
        // DTO Sending
        // ---------------------------------------------------------
        private void SendDto<T>(string channel, T dto)
        {
            if (string.IsNullOrEmpty(_config.NetworkSharedKey))
            {
                Echo("NET BLOCKED: SharedKey missing.");
                return;
            }

            string serialized = Serializer.Serialize(dto);
            string wrapped = SenderEnvelope.Wrap(_config.PBID, serialized, _config.NetworkSharedKey);
            IGC.SendBroadcastMessage(channel, wrapped);
        }

        private void PopulateWarningBooleans()
        {
            WarningDTO w = _warningDto;
            w.lowPower = _powerStatusDto.lowPower;
            w.lowIce = _iceStatusDto.lowIce;
            w.cargoFull = _inventorySummaryDto.cargoMax > 0.0001f
                          && _inventorySummaryDto.cargoPercent >= Cfw;
            string[] refineryNames = _refineryStatusDto.refineryNames;
            w.noRefineries = refineryNames == null || refineryNames.Length == 0;
            w.refineryStalled = ComputeRefineryStalled(_refineryStatusDto);
            w.assemblerStalled = _cachedAssemblerStalled;
        }

        private static bool ComputeRefineryStalled(RefineryStatusDTO rs)
        {
            bool[] hasOre = rs.hasOre;
            bool[] isWorking = rs.isWorking;
            if (hasOre == null || isWorking == null)
            {
                return false;
            }
            int n = hasOre.Length < isWorking.Length ? hasOre.Length : isWorking.Length;
            for (int i = 0; i < n; i++)
            {
                if (hasOre[i] && !isWorking[i])
                {
                    return true;
                }
            }
            return false;
        }

        private bool ComputeAssemblerStalled(List<IMyTerminalBlock> blocks)
        {
            if (blocks == null)
            {
                return false;
            }
            string manualTag = _config.ManualTag;
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyTerminalBlock b = blocks[i];
                var asm = b as IMyAssembler;
                if (asm == null || !asm.IsSameConstructAs(Me))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(manualTag))
                {
                    string name = asm.CustomName;
                    if (BlockUtils.HasTag(name, manualTag))
                    {
                        continue;
                    }
                }
                var prod = asm as IMyProductionBlock;
                if (prod == null)
                {
                    continue;
                }
                _assemblerQueueScratch.Clear();
                prod.GetQueue(_assemblerQueueScratch);
                if (_assemblerQueueScratch.Count > 0 && !prod.IsProducing)
                {
                    return true;
                }
            }
            return false;
        }

        private void ComputeWarningState(WarningDTO dto)
        {
            dto.isNominal = false;
            if (dto.lowPower)
            {
                dto.activeCode = 0;
                dto.activeMessage = "LOW POWER";
                return;
            }
            if (dto.cargoFull)
            {
                dto.activeCode = 1;
                dto.activeMessage = "CARGO FULL";
                return;
            }
            if (dto.lowIce)
            {
                dto.activeCode = 2;
                dto.activeMessage = "LOW ICE";
                return;
            }
            if (dto.refineryStalled)
            {
                dto.activeCode = 3;
                dto.activeMessage = "REFINERY STALLED";
                return;
            }
            if (dto.assemblerStalled)
            {
                dto.activeCode = 4;
                dto.activeMessage = "ASSEMBLER STALLED";
                return;
            }
            if (dto.noRefineries)
            {
                dto.activeCode = 5;
                dto.activeMessage = "NO REFINERIES";
                return;
            }
            dto.activeCode = -1;
            dto.activeMessage = "NOMINAL";
            dto.isNominal = true;
        }


    }
}
