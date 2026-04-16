// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: power_manager.cs
// Purpose: Aggregates battery/reactor/engine/solar state and applies optional automation using Config thresholds.
// PB Association: PB1
// Dependencies: InventoryScanner, Config, PowerStatusDTO
// Key Methods: PowerPassStep

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class PowerManager
    {
        private const double FractionEpsilon = 1e-9;

        private readonly List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
        private readonly List<IMyReactor> _reactors = new List<IMyReactor>();
        private readonly List<IMyPowerProducer> _engines = new List<IMyPowerProducer>();
        private readonly List<IMyPowerProducer> _solars = new List<IMyPowerProducer>();

        private double _totalStoredPower;
        private double _totalMaxStoredPower;
        private double _batteryInput;
        private double _batteryOutput;
        private double _solarOutput;
        private double _reactorOutput;
        private double _engineOutput;
        private double _batteryMaxInput;
        private double _batteryMaxOutput;
        private double _reactorMaxOutput;
        private double _engineMaxOutput;

        private const int IcCap = 38000;
        private int _lastProcessedIndex;
        private byte _ifp = 4;
        private bool _pwRfBatterySeedDone;

        public bool PowerPassStep(List<IMyTerminalBlock> globalBlocks, List<IMyBatteryBlock> cachedBatteries, ref PowerStatusDTO dto)
        {
            if (!EnsureContext())
            {
                _ifp = 4;
                PwClr(ref dto);
                return true;
            }

            if (_ifp == 4)
            {
                _batteries.Clear();
                _reactors.Clear();
                _engines.Clear();
                _solars.Clear();
                _pwRfBatterySeedDone = false;
                _ifp = 0;
                _lastProcessedIndex = 0;
            }

            if (_ifp == 0)
            {
                if (!PwRf(globalBlocks, cachedBatteries))
                {
                    return false;
                }

                ScanPowerTotals();
                ApplyBatteryLogic();
                ApplySolarLogic();
                var built = BuildPowerStatusDTO();
                PwCp(dto, built);
                _ifp = 4;
                return true;
            }

            return true;
        }

        private static void PwClr(ref PowerStatusDTO d)
        {
            d.batteryStored = 0f;
            d.batteryMax = 0f;
            d.batteryInput = 0f;
            d.batteryOutput = 0f;
            d.batteryMaxInput = 0f;
            d.batteryMaxOutput = 0f;
            d.reactorOutput = 0f;
            d.engineOutput = 0f;
            d.reactorMaxOutput = 0f;
            d.engineMaxOutput = 0f;
            d.batteryCount = 0;
            d.reactorCount = 0;
            d.engineCount = 0;
            d.lowPower = false;
        }

        private static void PwCp(PowerStatusDTO dst, PowerStatusDTO src)
        {
            dst.batteryStored = src.batteryStored;
            dst.batteryMax = src.batteryMax;
            dst.batteryInput = src.batteryInput;
            dst.batteryOutput = src.batteryOutput;
            dst.batteryMaxInput = src.batteryMaxInput;
            dst.batteryMaxOutput = src.batteryMaxOutput;
            dst.reactorOutput = src.reactorOutput;
            dst.engineOutput = src.engineOutput;
            dst.reactorMaxOutput = src.reactorMaxOutput;
            dst.engineMaxOutput = src.engineMaxOutput;
            dst.batteryCount = src.batteryCount;
            dst.reactorCount = src.reactorCount;
            dst.engineCount = src.engineCount;
            dst.lowPower = src.lowPower;
        }

        private bool PwRf(List<IMyTerminalBlock> globalBlocks, List<IMyBatteryBlock> cachedBatteries)
        {
            if (globalBlocks == null)
            {
                return true;
            }

            var me = InventoryScanner.Api.Me;

            if (!_pwRfBatterySeedDone && cachedBatteries != null)
            {
                for (int bi = 0; bi < cachedBatteries.Count; bi++)
                {
                    var bat = cachedBatteries[bi];
                    if (bat != null && bat.IsSameConstructAs(me) && !IsManual(bat))
                    {
                        _batteries.Add(bat);
                    }
                }

                _pwRfBatterySeedDone = true;
            }

            for (int i = _lastProcessedIndex; i < globalBlocks.Count; i++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = i;
                    return false;
                }

                var block = globalBlocks[i];
                if (!block.IsSameConstructAs(me) || IsManual(block))
                {
                    continue;
                }

                if (block is IMyBatteryBlock)
                {
                    continue;
                }

                var reactor = block as IMyReactor;
                if (reactor != null)
                {
                    _reactors.Add(reactor);
                    continue;
                }

                var producer = block as IMyPowerProducer;
                if (producer != null)
                {
                    if (IsHydrogenEngine(producer))
                    {
                        _engines.Add(producer);
                    }
                    else if (IsSolarPanel(producer))
                    {
                        _solars.Add(producer);
                    }
                }
            }

            return true;
        }

        private static bool IsSolarPanel(IMyPowerProducer b)
        {
            var tb = b as IMyTerminalBlock;
            if (tb == null)
            {
                return false;
            }

            string def = tb.BlockDefinition.ToString();
            return def.IndexOf("SolarPanel", StrIX.C) >= 0;
        }

        private static bool IsHydrogenEngine(IMyPowerProducer b)
        {
            var tb = b as IMyTerminalBlock;
            if (tb == null)
            {
                return false;
            }

            string def = tb.BlockDefinition.ToString();
            return def.IndexOf("HydrogenEngine", StrIX.C) >= 0;
        }

        private void ScanPowerTotals()
        {
            _totalStoredPower = 0;
            _totalMaxStoredPower = 0;
            _batteryInput = 0;
            _batteryOutput = 0;
            _solarOutput = 0;
            _reactorOutput = 0;
            _engineOutput = 0;
            _batteryMaxInput = 0;
            _batteryMaxOutput = 0;
            _reactorMaxOutput = 0;
            _engineMaxOutput = 0;

            for (int i = 0; i < _batteries.Count; i++)
            {
                var bat = _batteries[i];
                _totalStoredPower += bat.CurrentStoredPower;
                _totalMaxStoredPower += bat.MaxStoredPower;
                _batteryInput += bat.CurrentInput;
                _batteryOutput += bat.CurrentOutput;
                _batteryMaxInput += bat.MaxInput;
                _batteryMaxOutput += bat.MaxOutput;
            }

            for (int i = 0; i < _solars.Count; i++)
            {
                _solarOutput += _solars[i].CurrentOutput;
            }

            for (int i = 0; i < _reactors.Count; i++)
            {
                _reactorOutput += _reactors[i].CurrentOutput;
                _reactorMaxOutput += _reactors[i].MaxOutput;
            }

            for (int i = 0; i < _engines.Count; i++)
            {
                _engineOutput += _engines[i].CurrentOutput;
                _engineMaxOutput += _engines[i].MaxOutput;
            }
        }

        private void ApplyBatteryLogic()
        {
            if (_batteries.Count == 0)
            {
                return;
            }

            var cfg = InventoryScanner.AppConfig;
            double chargeFracTarget = 0.25;
            double dischargeFracTarget = 0.80;
            if (cfg != null)
            {
                chargeFracTarget = cfg.BatteryChargeTarget;
                dischargeFracTarget = cfg.BatteryDischargeTarget;
            }

            double frac = 0;
            if (_totalMaxStoredPower > FractionEpsilon)
            {
                frac = _totalStoredPower / _totalMaxStoredPower;
            }

            bool wantRecharge = frac < chargeFracTarget;
            bool wantAuto = frac > dischargeFracTarget;

            for (int i = 0; i < _batteries.Count; i++)
            {
                var bat = _batteries[i];
                if (wantRecharge)
                {
                    bat.ChargeMode = ChargeMode.Recharge;
                }
                else if (wantAuto)
                {
                    bat.ChargeMode = ChargeMode.Auto;
                }
            }
        }

        private void ApplySolarLogic()
        {
            var cfg = InventoryScanner.AppConfig;
            if (cfg == null || !cfg.EnablePowerAutomation)
            {
                return;
            }

            bool solarLow = _solarOutput < cfg.SolarMinimumOutput;
            bool solarGood = _solarOutput >= cfg.SolarMinimumOutput;

            if (solarLow)
            {
                for (int i = 0; i < _reactors.Count; i++)
                {
                    _reactors[i].Enabled = true;
                }

                for (int i = 0; i < _engines.Count; i++)
                {
                    _engines[i].Enabled = true;
                }
            }
            else if (solarGood)
            {
                for (int i = 0; i < _reactors.Count; i++)
                {
                    _reactors[i].Enabled = false;
                }

                for (int i = 0; i < _engines.Count; i++)
                {
                    _engines[i].Enabled = false;
                }
            }
        }

        private PowerStatusDTO BuildPowerStatusDTO()
        {
            var dto = new PowerStatusDTO();
            dto.batteryStored = (float)_totalStoredPower;
            dto.batteryMax = (float)_totalMaxStoredPower;
            dto.batteryInput = (float)_batteryInput;
            dto.batteryOutput = (float)_batteryOutput;
            dto.batteryMaxInput = (float)_batteryMaxInput;
            dto.batteryMaxOutput = (float)_batteryMaxOutput;
            dto.reactorOutput = (float)_reactorOutput;
            dto.engineOutput = (float)_engineOutput;
            dto.reactorMaxOutput = (float)_reactorMaxOutput;
            dto.engineMaxOutput = (float)_engineMaxOutput;
            dto.batteryCount = _batteries.Count;
            dto.reactorCount = _reactors.Count;
            dto.engineCount = _engines.Count;

            double chargeFrac = 0;
            if (_totalMaxStoredPower > FractionEpsilon)
            {
                chargeFrac = _totalStoredPower / _totalMaxStoredPower;
            }

            double lowFrac = 0.25;
            if (InventoryScanner.AppConfig != null)
            {
                lowFrac = InventoryScanner.AppConfig.BatteryChargeTarget;
            }

            dto.lowPower = _totalMaxStoredPower > FractionEpsilon && chargeFrac < lowFrac;

            return dto;
        }

        private bool EnsureContext()
        {
            return InventoryScanner.Api != null
                && InventoryScanner.Api.Me != null
                && InventoryScanner.Api.GridTerminalSystem != null;
        }

        private bool IsManual(IMyTerminalBlock block)
        {
            if (block == null)
            {
                return false;
            }

            var cfg = InventoryScanner.AppConfig;
            string tag = cfg != null && !string.IsNullOrEmpty(cfg.ManualTag) ? cfg.ManualTag : "[Manual]";
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            string name = block.CustomName;
            return BlockUtils.HasTag(name, tag);
        }
    }
}
