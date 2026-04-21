// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: inventory_scanner.cs
// Purpose: Scans tagged inventories and assemblers/refineries; builds aggregated inventory and dynamic item DTOs for PB1.
// PB Association: PB1
// Dependencies: Config, InventorySummaryDTO, InventoryDynamicDTO, MyGridProgram (static Api context)
// Key Methods: InventoryPassStep, IsModded, FillConstructBlockCaches

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
    public class InventoryScanner
    {
        /// <summary>Assign from Program: grid API (Me, GridTerminalSystem). Required for scanning.</summary>
        public static MyGridProgram Api;

        /// <summary>Assign from Program: loaded config (ManualTag, etc.). If null, ManualTag defaults to [Manual].</summary>
        public static Config AppConfig;

        private const double AssemblerIngotOverflowRatio = 0.9;

        private const string OreStorageTag = "[Ore]";
        private const string IngotStorageTag = "[Ingot]";
        private const string ComponentStorageTag = "[Component]";
        private const string AmmoStorageTag = "[Ammo]";
        private const string ToolStorageTag = "[Tool]";
        private const string BottleStorageTag = "[Bottle]";
        private const string GeneralCargoTag = "[Cargo]";

        private static readonly HashSet<string> VanillaOreSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Iron", "Nickel", "Cobalt", "Silicon", "Magnesium", "Silver", "Gold", "Platinum", "Uranium", "Stone", "Ice"
        };

        public static bool IsVanillaOreSubtype(string subtypeId)
        {
            return !string.IsNullOrEmpty(subtypeId) && VanillaOreSubtypes.Contains(subtypeId);
        }

        /// <summary>Lightweight mod heuristics for PB1 (replaces shared item_type_helpers in this build).</summary>
        public static bool IsModded(MyItemType t)
        {
            string sub = t.SubtypeId;
            if (string.IsNullOrEmpty(sub))
            {
                return false;
            }

            string tid = t.TypeId.ToString();
            if (IsOreType(t))
            {
                return !VanillaOreSubtypes.Contains(sub);
            }

            if (IsIngotItem(t))
            {
                return !VanillaIngotSubtypes.Contains(sub);
            }

            if (IsComponentItem(t))
            {
                return !VanillaComponentSubtypes.Contains(sub);
            }

            if (IsAmmoItem(t))
            {
                return !VanillaAmmoSubtypes.Contains(sub);
            }

            if (IsToolItem(t))
            {
                return !VanillaToolSubtypes.Contains(sub);
            }

            if (IsBottleItem(t))
            {
                return !VanillaBottleSubtypes.Contains(sub);
            }

            if (sub.IndexOf("Mk", StringComparison.OrdinalIgnoreCase) >= 0
                || sub.IndexOf("Z-", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        private static readonly HashSet<string> VanillaIngotSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Iron", "Nickel", "Cobalt", "Silicon", "Magnesium", "Silver", "Gold", "Platinum", "Uranium"
        };

        private static readonly HashSet<string> VanillaComponentSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SteelPlate", "InteriorPlate", "ConstructionComponent", "SmallTube", "LargeTube", "Motor", "Computer",
            "MetalGrid", "Display", "BulletproofGlass", "MedicalComponents", "PowerCell", "RadioCommunicationComponent",
            "ReactorComponents", "ThrustModule", "GravityGeneratorComponents", "Superconductor", "Girder",
            "DetectorComponents", "Explosives", "SolarCell", "TargetingComputerComponent", "PistonMechanism",
            "RotorPart", "ArmorPanel", "WelderComponent", "HandDrillComponent", "HydrogenEngineComponent"
        };

        private static readonly HashSet<string> VanillaAmmoSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NATO_25x184mm", "NATO_5p56x45mm", "Missile200mm", "LargeCaliberAmmo", "MediumCaliberAmmo", "AutocannonAmmo",
            "RapidFireAutomaticGunAmmo", "Rocket200mm", "FlareGunMagazine", "AutomaticRifleGun_Mag_20rd", "ElitePistolMagazine",
            "FullAutoPistolMagazine", "PistolMagazine", "SemiAutoPistolMagazine", "AdvancedPistolMagazine", "MilestonePistolMagazine",
            "AutomaticRifleGun_Mag_40rd", "RapidFireAutomaticGun_Mag_150rd", "RapidFireAutomaticGun_Mag_560rd",
            "ArtilleryShell200mm", "ArtilleryShell100mm", "AssaultCraftAmmoMassDriver", "Cannon750mmAmmo", "AssaultCannonAmmo120mm"
        };

        private static readonly HashSet<string> VanillaToolSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "WelderItem", "Welder2Item", "Welder3Item", "Welder4Item", "AngleGrinderItem", "AngleGrinder2Item",
            "AngleGrinder3Item", "AngleGrinder4Item", "HandDrillItem", "HandDrill2Item", "HandDrill3Item", "HandDrill4Item"
        };

        private static readonly HashSet<string> VanillaBottleSubtypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "OxygenBottle", "HydrogenBottle"
        };

        private readonly List<IMyAssembler> _assemblersScratch = new List<IMyAssembler>();
        private readonly List<IMyRefinery> _refineriesScratch = new List<IMyRefinery>();
        private readonly List<IMyInventory> _oreDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _ingotDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _componentDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _ammoDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _toolDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _bottleDestScratch = new List<IMyInventory>();
        private readonly List<IMyInventory> _generalDestScratch = new List<IMyInventory>();

        private readonly List<IMyTerminalBlock> _cachedInventoryOwners = new List<IMyTerminalBlock>();
        private readonly List<IMyCargoContainer> _cachedCargoContainers = new List<IMyCargoContainer>();
        private readonly HashSet<string> _moddedOreDedupe = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _moddedOreAccum = new List<string>();

        private const int IcCap = 38000;
        private int _lastProcessedIndex;
        private byte _ifp = 4;
        private byte _exPart;
        private InventorySummaryDTO _wipSum = new InventorySummaryDTO();
        private readonly List<MyInventoryItem> _invItemScratch = new List<MyInventoryItem>();
        private readonly StringBuilder _invTel = new StringBuilder(96);

        public List<string> LastScanModdedOres { get { return _moddedOreAccum; } }

        public bool InvPassIdle()
        {
            return _ifp == 4;
        }

        /// <summary>
        /// Runs only when PB1 reloads the grid list (<c>GetBlocks</c>). Classifies cargo, refineries, gas blocks,
        /// and batteries once per refresh (interface casts, no per-tick full-grid work in Ice/Power passes).
        /// </summary>
        public static void FillConstructBlockCaches(
            List<IMyTerminalBlock> globalBlocks,
            List<IMyCargoContainer> cargoOut,
            List<IMyRefinery> refineryOut,
            List<IMyGasTank> gasTankOut,
            List<IMyGasGenerator> gasGeneratorOut,
            List<IMyBatteryBlock> batteryOut)
        {
            cargoOut.Clear();
            refineryOut.Clear();
            gasTankOut.Clear();
            gasGeneratorOut.Clear();
            batteryOut.Clear();
            if (globalBlocks == null || Api == null || Api.Me == null)
            {
                return;
            }

            for (int i = 0; i < globalBlocks.Count; i++)
            {
                var block = globalBlocks[i];
                if (block == null || !block.IsSameConstructAs(Api.Me))
                {
                    continue;
                }

                if (IsManualBlock(block))
                {
                    continue;
                }

                var battery = block as IMyBatteryBlock;
                if (battery != null)
                {
                    batteryOut.Add(battery);
                    continue;
                }

                var gasTank = block as IMyGasTank;
                if (gasTank != null)
                {
                    gasTankOut.Add(gasTank);
                    continue;
                }

                var gasGen = block as IMyGasGenerator;
                if (gasGen != null)
                {
                    gasGeneratorOut.Add(gasGen);
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null || owner.InventoryCount <= 0)
                {
                    continue;
                }

                var cargo = block as IMyCargoContainer;
                if (cargo != null)
                {
                    cargoOut.Add(cargo);
                    continue;
                }

                var refinery = block as IMyRefinery;
                if (refinery != null)
                {
                    refineryOut.Add(refinery);
                }
            }
        }

        private readonly Dictionary<string, DynAccum> _dynamicAccumScratch = new Dictionary<string, DynAccum>(StringComparer.Ordinal);

        private struct DynAccum
        {
            public MyFixedPoint Amount;
            public string TypeLabel;
            public string CleanName;
        }

        public bool InventoryPassStep(
            List<IMyTerminalBlock> globalBlocks,
            List<IMyCargoContainer> cachedCargo,
            List<IMyRefinery> cachedRefineries,
            ref InventorySummaryDTO sumDto,
            ref InventoryDynamicDTO dynDto)
        {
            if (!EnsureContext())
            {
                _ifp = 4;
                ClrSum(ref sumDto);
                ClrDyn(ref dynDto);
                return true;
            }

            if (_ifp == 4)
            {
                _moddedOreAccum.Clear();
                _moddedOreDedupe.Clear();
                InvRfBegin(cachedCargo, cachedRefineries);
                _ifp = 0;
                _lastProcessedIndex = 0;
            }

            if (_ifp == 0)
            {
                if (!InvRfGlobal(globalBlocks))
                {
                    return false;
                }

                _ifp = 1;
                _exPart = 0;
                _lastProcessedIndex = 0;
                return false;
            }

            if (_ifp == 1)
            {
                if (!InvExtractSlice())
                {
                    return false;
                }

                InvWipCargoPrep();
                _ifp = 2;
                _lastProcessedIndex = 0;
                return false;
            }

            if (_ifp == 2)
            {
                if (!InvSumOwners())
                {
                    return false;
                }

                _ifp = 3;
                _lastProcessedIndex = 0;
                _dynamicAccumScratch.Clear();
                return false;
            }

            if (_ifp == 3)
            {
                if (!InvDynOwners())
                {
                    return false;
                }

                CpSum(sumDto);
                FillDyn(dynDto);
                _ifp = 4;
                return true;
            }

            _ifp = 4;
            return true;
        }

        private static void ClrSum(ref InventorySummaryDTO d)
        {
            d = new InventorySummaryDTO();
        }

        private static void ClrDyn(ref InventoryDynamicDTO d)
        {
            d.itemNames = new string[0];
            d.itemAmounts = new float[0];
            d.itemTypes = new string[0];
        }

        private void InvRfBegin(List<IMyCargoContainer> cachedCargo, List<IMyRefinery> cachedRefineries)
        {
            _cachedInventoryOwners.Clear();
            _cachedCargoContainers.Clear();
            _assemblersScratch.Clear();
            _refineriesScratch.Clear();
            _oreDestScratch.Clear();
            _ingotDestScratch.Clear();
            _componentDestScratch.Clear();
            _ammoDestScratch.Clear();
            _toolDestScratch.Clear();
            _bottleDestScratch.Clear();
            _generalDestScratch.Clear();

            if (cachedCargo != null)
            {
                for (int i = 0; i < cachedCargo.Count; i++)
                {
                    var cargo = cachedCargo[i];
                    if (cargo == null)
                    {
                        continue;
                    }

                    _cachedInventoryOwners.Add(cargo);
                    _cachedCargoContainers.Add(cargo);
                    AddStorageDestinations(cargo);
                }
            }

            if (cachedRefineries != null)
            {
                for (int i = 0; i < cachedRefineries.Count; i++)
                {
                    var refinery = cachedRefineries[i];
                    if (refinery == null)
                    {
                        continue;
                    }

                    _cachedInventoryOwners.Add(refinery);
                    _refineriesScratch.Add(refinery);
                }
            }
        }

        private bool InvRfGlobal(List<IMyTerminalBlock> globalBlocks)
        {
            if (globalBlocks == null)
            {
                return true;
            }

            for (int i = _lastProcessedIndex; i < globalBlocks.Count; i++)
            {
                if (Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = i;
                    return false;
                }

                var block = globalBlocks[i];
                if (!IsOnConstructWithInventory(block) || IsManual(block))
                {
                    continue;
                }

                if (block as IMyCargoContainer != null || block as IMyRefinery != null)
                {
                    continue;
                }

                _cachedInventoryOwners.Add(block);

                var asm = block as IMyAssembler;
                if (asm != null)
                {
                    _assemblersScratch.Add(asm);
                }
            }

            return true;
        }

        /// <summary>
        /// Refinery ore input is inventory index 0; ingot output is index 1. Prefer <c>GetInventory(1)</c> when
        /// <c>InventoryCount</c> is at least 2, then fall back to <see cref="IMyProductionBlock.OutputInventory"/>.
        /// </summary>
        private static IMyInventory GetRefineryOutputInventory(IMyRefinery refinery)
        {
            if (refinery == null)
            {
                return null;
            }

            if (refinery.InventoryCount >= 2)
            {
                var byIndex = refinery.GetInventory(1);
                if (byIndex != null)
                {
                    return byIndex;
                }
            }

            return refinery.OutputInventory;
        }

        private bool InvExtractSlice()
        {
            const int storageMoveBudget = 5;
            int moved = 0;
            if (_exPart == 0)
            {
                for (int ci = _lastProcessedIndex; ci < _cachedCargoContainers.Count; ci++)
                {
                    if (Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _lastProcessedIndex = ci;
                        return false;
                    }

                    var cargo = _cachedCargoContainers[ci];
                    if (cargo == null || IsManual(cargo))
                    {
                        continue;
                    }

                    string nm = cargo.CustomName ?? string.Empty;
                    if (!BlockUtils.HasTag(nm, ComponentStorageTag))
                    {
                        continue;
                    }

                    for (int invIdx = 0; invIdx < cargo.InventoryCount; invIdx++)
                    {
                        var inv = cargo.GetInventory(invIdx);
                        if (inv == null)
                        {
                            continue;
                        }

                        _invItemScratch.Clear();
                        inv.GetItems(_invItemScratch);
                        for (int idx = _invItemScratch.Count - 1; idx >= 0 && moved < storageMoveBudget; idx--)
                        {
                            if (Api.Runtime.CurrentInstructionCount > IcCap)
                            {
                                _lastProcessedIndex = ci;
                                return false;
                            }

                            if (!IsComponentItem(_invItemScratch[idx].Type))
                            {
                                continue;
                            }

                            if (TryMoveItemToPrimaryCargo(inv, idx))
                            {
                                moved++;
                            }
                        }
                    }
                }

                EmitInvTel(0, moved);
                _exPart = 1;
                _lastProcessedIndex = 0;
                return false;
            }

            if (_exPart == 1)
            {
                for (int ci = _lastProcessedIndex; ci < _cachedCargoContainers.Count; ci++)
                {
                    if (Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _lastProcessedIndex = ci;
                        return false;
                    }

                    var cargo = _cachedCargoContainers[ci];
                    if (cargo == null || IsManual(cargo))
                    {
                        continue;
                    }

                    string nm = cargo.CustomName ?? string.Empty;
                    if (!BlockUtils.HasTag(nm, IngotStorageTag))
                    {
                        continue;
                    }

                    for (int invIdx = 0; invIdx < cargo.InventoryCount; invIdx++)
                    {
                        var inv = cargo.GetInventory(invIdx);
                        if (inv == null)
                        {
                            continue;
                        }

                        _invItemScratch.Clear();
                        inv.GetItems(_invItemScratch);
                        for (int idx = _invItemScratch.Count - 1; idx >= 0 && moved < storageMoveBudget; idx--)
                        {
                            if (Api.Runtime.CurrentInstructionCount > IcCap)
                            {
                                _lastProcessedIndex = ci;
                                return false;
                            }

                            if (!IsIngotItem(_invItemScratch[idx].Type))
                            {
                                continue;
                            }

                            if (TryMoveItemToPrimaryCargo(inv, idx))
                            {
                                moved++;
                            }
                        }
                    }
                }

                EmitInvTel(1, moved);
                _exPart = 2;
                _lastProcessedIndex = 0;
                return false;
            }

            if (_exPart == 2)
            {
                for (int a = _lastProcessedIndex; a < _assemblersScratch.Count; a++)
                {
                    if (Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _lastProcessedIndex = a;
                        return false;
                    }

                    var asm = _assemblersScratch[a];
                    var output = asm.OutputInventory;
                    if (output == null)
                    {
                        continue;
                    }

                    _invItemScratch.Clear();
                    output.GetItems(_invItemScratch);
                    for (int idx = _invItemScratch.Count - 1; idx >= 0 && moved < storageMoveBudget; idx--)
                    {
                        if (Api.Runtime.CurrentInstructionCount > IcCap)
                        {
                            _lastProcessedIndex = a;
                            return false;
                        }

                        if (!IsComponentItem(_invItemScratch[idx].Type))
                        {
                            continue;
                        }

                        if (MoveItemToStorageAt(output, idx))
                        {
                            moved++;
                        }
                    }
                }

                EmitInvTel(2, moved);
                _exPart = 3;
                _lastProcessedIndex = 0;
                return false;
            }

            if (_exPart == 3)
            {
                const int refineryExtractBudget = 20;
                for (int r = _lastProcessedIndex; r < _refineriesScratch.Count; r++)
                {
                    if (Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _lastProcessedIndex = r;
                        return false;
                    }

                    var refinery = _refineriesScratch[r];
                    var output = GetRefineryOutputInventory(refinery);
                    if (output == null)
                    {
                        continue;
                    }

                    IMyInventory refineryInput = refinery.InventoryCount > 0 ? refinery.GetInventory(0) : null;

                    _invItemScratch.Clear();
                    output.GetItems(_invItemScratch);
                    for (int idx = _invItemScratch.Count - 1; idx >= 0 && moved < refineryExtractBudget; idx--)
                    {
                        if (Api.Runtime.CurrentInstructionCount > IcCap)
                        {
                            _lastProcessedIndex = r;
                            return false;
                        }

                        if (MoveItemToStorageAt(output, idx, refineryInput))
                        {
                            moved++;
                        }
                    }
                }

                EmitInvTel(3, moved);
                _exPart = 4;
                _lastProcessedIndex = 0;
                return false;
            }

            for (int a = _lastProcessedIndex; a < _assemblersScratch.Count; a++)
            {
                if (Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = a;
                    return false;
                }

                var asm = _assemblersScratch[a];
                var input = asm.InputInventory;
                if (input == null || input.MaxVolume.RawValue <= 0)
                {
                    continue;
                }

                while (moved < storageMoveBudget)
                {
                    double ratio = (double)input.CurrentVolume.RawValue / (double)input.MaxVolume.RawValue;
                    if (ratio < AssemblerIngotOverflowRatio)
                    {
                        break;
                    }

                    if (Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _lastProcessedIndex = a;
                        return false;
                    }

                    _invItemScratch.Clear();
                    input.GetItems(_invItemScratch);
                    bool did = false;
                    for (int idx = _invItemScratch.Count - 1; idx >= 0; idx--)
                    {
                        if (!IsIngotItem(_invItemScratch[idx].Type))
                        {
                            continue;
                        }

                        if (TryMoveItemToPrimaryCargo(input, idx))
                        {
                            moved++;
                            did = true;
                            break;
                        }
                    }

                    if (!did)
                    {
                        break;
                    }

                    ratio = (double)input.CurrentVolume.RawValue / (double)input.MaxVolume.RawValue;
                    if (ratio < AssemblerIngotOverflowRatio)
                    {
                        break;
                    }
                }

                double finalR = input.MaxVolume.RawValue > 0
                    ? (double)input.CurrentVolume.RawValue / (double)input.MaxVolume.RawValue
                    : 0;
                if (finalR >= AssemblerIngotOverflowRatio && moved >= storageMoveBudget)
                {
                    _lastProcessedIndex = a;
                    return false;
                }
            }

            EmitInvTel(4, moved);
            return true;
        }

        private void InvWipCargoPrep()
        {
            _wipSum = new InventorySummaryDTO();
            float cargoUsed = 0f;
            float cargoMax = 0f;
            for (int c = 0; c < _cachedCargoContainers.Count; c++)
            {
                var cargo = _cachedCargoContainers[c];
                if (IsManual(cargo))
                {
                    continue;
                }

                for (int invIdx = 0; invIdx < cargo.InventoryCount; invIdx++)
                {
                    var inv = cargo.GetInventory(invIdx);
                    cargoUsed += (float)inv.CurrentVolume;
                    cargoMax += (float)inv.MaxVolume;
                }
            }

            _wipSum.cargoUsed = cargoUsed;
            _wipSum.cargoMax = cargoMax;
            _wipSum.cargoPercent = cargoMax > 0.0001f ? (cargoUsed / cargoMax) * 100f : 0f;
        }

        private bool InvSumOwners()
        {
            for (int b = _lastProcessedIndex; b < _cachedInventoryOwners.Count; b++)
            {
                if (Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = b;
                    return false;
                }

                var block = _cachedInventoryOwners[b];
                if (IsManual(block))
                {
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                for (int invIdx = 0; invIdx < owner.InventoryCount; invIdx++)
                {
                    var inv = owner.GetInventory(invIdx);
                    AccumulateInventoryIntoSummary(inv, ref _wipSum);
                }
            }

            return true;
        }

        private bool InvDynOwners()
        {
            for (int b = _lastProcessedIndex; b < _cachedInventoryOwners.Count; b++)
            {
                if (Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = b;
                    return false;
                }

                var block = _cachedInventoryOwners[b];
                if (IsManual(block))
                {
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                for (int invIdx = 0; invIdx < owner.InventoryCount; invIdx++)
                {
                    var inv = owner.GetInventory(invIdx);
                    var items = new List<MyInventoryItem>();
                    inv.GetItems(items);
                    for (int i = 0; i < items.Count; i++)
                    {
                        var it = items[i];
                        if (!ShouldIncludeInDynamic(it.Type))
                        {
                            continue;
                        }

                        string key = it.Type.ToString();
                        string typeLabel = ClassifyLabel(it.Type);

                        DynAccum acc;
                        if (_dynamicAccumScratch.TryGetValue(key, out acc))
                        {
                            acc.Amount += it.Amount;
                            _dynamicAccumScratch[key] = acc;
                        }
                        else
                        {
                            _dynamicAccumScratch[key] = new DynAccum
                            {
                                Amount = it.Amount,
                                TypeLabel = typeLabel,
                                CleanName = it.Type.SubtypeId
                            };
                        }
                    }
                }
            }

            return true;
        }

        private void CpSum(InventorySummaryDTO d)
        {
            d.ironOre = _wipSum.ironOre;
            d.nickelOre = _wipSum.nickelOre;
            d.cobaltOre = _wipSum.cobaltOre;
            d.siliconOre = _wipSum.siliconOre;
            d.magnesiumOre = _wipSum.magnesiumOre;
            d.silverOre = _wipSum.silverOre;
            d.goldOre = _wipSum.goldOre;
            d.platinumOre = _wipSum.platinumOre;
            d.uraniumOre = _wipSum.uraniumOre;
            d.stoneOre = _wipSum.stoneOre;
            d.iceOre = _wipSum.iceOre;
            d.ironIngot = _wipSum.ironIngot;
            d.nickelIngot = _wipSum.nickelIngot;
            d.cobaltIngot = _wipSum.cobaltIngot;
            d.siliconIngot = _wipSum.siliconIngot;
            d.magnesiumPowder = _wipSum.magnesiumPowder;
            d.silverIngot = _wipSum.silverIngot;
            d.goldIngot = _wipSum.goldIngot;
            d.platinumIngot = _wipSum.platinumIngot;
            d.uraniumIngot = _wipSum.uraniumIngot;
            d.componentsTotal = _wipSum.componentsTotal;
            d.ammoTotal = _wipSum.ammoTotal;
            d.toolsTotal = _wipSum.toolsTotal;
            d.bottlesTotal = _wipSum.bottlesTotal;
            d.cargoUsed = _wipSum.cargoUsed;
            d.cargoMax = _wipSum.cargoMax;
            d.cargoPercent = _wipSum.cargoPercent;
        }

        private void FillDyn(InventoryDynamicDTO dto)
        {
            int n = _dynamicAccumScratch.Count;
            if (n == 0)
            {
                dto.itemNames = new string[0];
                dto.itemAmounts = new float[0];
                dto.itemTypes = new string[0];
                return;
            }

            var names = new string[n];
            var amounts = new float[n];
            var types = new string[n];
            int j = 0;
            foreach (var kv in _dynamicAccumScratch)
            {
                var acc = kv.Value;
                names[j] = FormattingUtils.SanitizeIngressWireText(acc.CleanName);
                amounts[j] = (float)acc.Amount;
                types[j] = FormattingUtils.SanitizeIngressWireText(acc.TypeLabel);
                j++;
            }

            dto.itemNames = names;
            dto.itemAmounts = amounts;
            dto.itemTypes = types;
        }

        private void EmitInvTel(byte phase, int moved)
        {
            if (moved <= 0 || AppConfig == null || !AppConfig.EnableDebug || Api == null)
            {
                return;
            }

            _invTel.Clear();
            _invTel.Append("INV ex=");
            _invTel.Append(phase);
            _invTel.Append(" n=");
            _invTel.Append(moved);
            Api.Echo(_invTel.ToString());
        }

        private bool TryMoveItemToPrimaryCargo(IMyInventory source, int sourceItemIndex)
        {
            _invItemScratch.Clear();
            source.GetItems(_invItemScratch);
            if (sourceItemIndex < 0 || sourceItemIndex >= _invItemScratch.Count)
            {
                return false;
            }

            for (int d = 0; d < _generalDestScratch.Count; d++)
            {
                var dst = _generalDestScratch[d];
                if (dst == null || dst == source)
                {
                    continue;
                }

                if (TryTransferAt(source, sourceItemIndex, dst))
                {
                    return true;
                }
            }

            return false;
        }

        private bool MoveItemToStorageAt(IMyInventory source, int sourceItemIndex, IMyInventory forbidDestination = null)
        {
            _invItemScratch.Clear();
            source.GetItems(_invItemScratch);
            if (sourceItemIndex < 0 || sourceItemIndex >= _invItemScratch.Count)
            {
                return false;
            }

            MyItemType itemType = _invItemScratch[sourceItemIndex].Type;
            StorageKind kind = GetStorageKind(itemType);
            List<IMyInventory> destList;
            switch (kind)
            {
                case StorageKind.Ore:
                    destList = _oreDestScratch;
                    break;
                case StorageKind.Ingot:
                    destList = _ingotDestScratch;
                    break;
                case StorageKind.Component:
                    destList = _componentDestScratch;
                    break;
                case StorageKind.Ammo:
                    destList = _ammoDestScratch;
                    break;
                case StorageKind.Tool:
                    destList = _toolDestScratch;
                    break;
                case StorageKind.Bottle:
                    destList = _bottleDestScratch;
                    break;
                default:
                    destList = _generalDestScratch;
                    break;
            }

            for (int d = 0; d < destList.Count; d++)
            {
                var dst = destList[d];
                if (dst == null || dst == source || dst == forbidDestination)
                {
                    continue;
                }

                if (TryTransferAt(source, sourceItemIndex, dst))
                {
                    return true;
                }
            }

            if (kind == StorageKind.General)
            {
                return false;
            }

            for (int d = 0; d < _generalDestScratch.Count; d++)
            {
                var dst = _generalDestScratch[d];
                if (dst == null || dst == source || dst == forbidDestination)
                {
                    continue;
                }

                if (TryTransferAt(source, sourceItemIndex, dst))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryTransferAt(IMyInventory source, int sourceItemIndex, IMyInventory dst)
        {
            if (!source.IsConnectedTo(dst))
            {
                return false;
            }

            return source.TransferItemTo(dst, sourceItemIndex, null, true, null);
        }

        private void AccumulateInventoryIntoSummary(IMyInventory inv, ref InventorySummaryDTO dto)
        {
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var t = it.Type;
                string sub = t.SubtypeId;
                float amt = (float)it.Amount;

                if (IsOreType(t))
                {
                    if (VanillaOreSubtypes.Contains(sub))
                    {
                        AssignVanillaOre(ref dto, sub, amt);
                    }
                    else if (_moddedOreDedupe.Add(sub))
                    {
                        _moddedOreAccum.Add(sub);
                    }

                    continue;
                }

                if (IsIngotItem(t))
                {
                    if (VanillaIngotSubtypes.Contains(sub))
                    {
                        AssignVanillaIngot(ref dto, sub, amt);
                    }

                    continue;
                }

                if (IsComponentItem(t))
                {
                    dto.componentsTotal += amt;
                    continue;
                }

                if (IsAmmoItem(t))
                {
                    dto.ammoTotal += amt;
                    continue;
                }

                if (IsToolItem(t))
                {
                    dto.toolsTotal += amt;
                    continue;
                }

                if (IsBottleItem(t))
                {
                    dto.bottlesTotal += amt;
                }
            }
        }

        private static void AssignVanillaOre(ref InventorySummaryDTO dto, string sub, float amt)
        {
            switch (sub.ToUpperInvariant())
            {
                case "IRON": dto.ironOre += amt; break;
                case "NICKEL": dto.nickelOre += amt; break;
                case "COBALT": dto.cobaltOre += amt; break;
                case "SILICON": dto.siliconOre += amt; break;
                case "MAGNESIUM": dto.magnesiumOre += amt; break;
                case "SILVER": dto.silverOre += amt; break;
                case "GOLD": dto.goldOre += amt; break;
                case "PLATINUM": dto.platinumOre += amt; break;
                case "URANIUM": dto.uraniumOre += amt; break;
                case "STONE": dto.stoneOre += amt; break;
                case "ICE": dto.iceOre += amt; break;
            }
        }

        private static void AssignVanillaIngot(ref InventorySummaryDTO dto, string sub, float amt)
        {
            switch (sub.ToUpperInvariant())
            {
                case "IRON": dto.ironIngot += amt; break;
                case "NICKEL": dto.nickelIngot += amt; break;
                case "COBALT": dto.cobaltIngot += amt; break;
                case "SILICON": dto.siliconIngot += amt; break;
                case "MAGNESIUM": dto.magnesiumPowder += amt; break;
                case "SILVER": dto.silverIngot += amt; break;
                case "GOLD": dto.goldIngot += amt; break;
                case "PLATINUM": dto.platinumIngot += amt; break;
                case "URANIUM": dto.uraniumIngot += amt; break;
            }
        }

        private bool ShouldIncludeInDynamic(MyItemType t)
        {
            return true; // Option B: Everything goes into the dynamic arrays
        }

        private static string ClassifyLabel(MyItemType t)
        {
            if (IsOreType(t))
            {
                return "Ore";
            }

            if (IsIngotItem(t))
            {
                return "Ingot";
            }

            if (IsComponentItem(t))
            {
                return "Component";
            }

            if (IsAmmoItem(t))
            {
                return "Ammo";
            }

            if (IsToolItem(t))
            {
                return "Tool";
            }

            if (IsBottleItem(t))
            {
                return "Bottle";
            }

            return "Other";
        }

        private enum StorageKind
        {
            Ore, Ingot, Component, Ammo, Tool, Bottle, General
        }

        private static StorageKind GetStorageKind(MyItemType t)
        {
            if (IsOreType(t))
            {
                return StorageKind.Ore;
            }

            if (IsIngotItem(t))
            {
                return StorageKind.Ingot;
            }

            if (IsComponentItem(t))
            {
                return StorageKind.Component;
            }

            if (IsAmmoItem(t))
            {
                return StorageKind.Ammo;
            }

            if (IsToolItem(t))
            {
                return StorageKind.Tool;
            }

            if (IsBottleItem(t))
            {
                return StorageKind.Bottle;
            }

            return StorageKind.General;
        }

        private static bool IsOreType(MyItemType t)
        {
            string id = t.TypeId.ToString();
            return id.IndexOf("Ore", StringComparison.Ordinal) >= 0;
        }

        private static bool IsIngotItem(MyItemType t)
        {
            return t.TypeId.ToString().IndexOf("Ingot", StringComparison.Ordinal) >= 0;
        }

        private static bool IsComponentItem(MyItemType t)
        {
            return t.TypeId.ToString().IndexOf("Component", StringComparison.Ordinal) >= 0;
        }

        private static bool IsAmmoItem(MyItemType t)
        {
            string id = t.TypeId.ToString();
            return id.IndexOf("Ammo", StringComparison.Ordinal) >= 0 || id.IndexOf("Magazine", StringComparison.Ordinal) >= 0;
        }

        private static bool IsToolItem(MyItemType t)
        {
            string id = t.TypeId.ToString();
            if (id.IndexOf("PhysicalGun", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            if (id.IndexOf("Welder", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            if (id.IndexOf("Drill", StringComparison.Ordinal) >= 0 && id.IndexOf("Component", StringComparison.Ordinal) < 0)
            {
                return true;
            }

            if (id.IndexOf("Grinder", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }

        private static bool IsBottleItem(MyItemType t)
        {
            string id = t.TypeId.ToString();
            return id.IndexOf("OxygenContainer", StringComparison.Ordinal) >= 0
                   || id.IndexOf("GasContainer", StringComparison.Ordinal) >= 0;
        }

        private bool EnsureContext()
        {
            return Api != null && Api.Me != null && Api.GridTerminalSystem != null;
        }

        private static bool IsManualBlock(IMyTerminalBlock block)
        {
            if (block == null)
            {
                return false;
            }

            string tag = AppConfig != null && !string.IsNullOrEmpty(AppConfig.ManualTag) ? AppConfig.ManualTag : "[Manual]";
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            string name = block.CustomName;
            return BlockUtils.HasTag(name, tag);
        }

        private bool IsManual(IMyTerminalBlock block)
        {
            return IsManualBlock(block);
        }

        private bool IsOnConstructWithInventory(IMyTerminalBlock block)
        {
            if (block == null || !block.IsSameConstructAs(Api.Me))
            {
                return false;
            }

            var owner = block as IMyInventoryOwner;
            return owner != null && owner.InventoryCount > 0;
        }

        private void AddStorageDestinations(IMyCargoContainer cargo)
        {
            string name = cargo.CustomName ?? string.Empty;
            bool taggedOre = BlockUtils.HasTag(name, OreStorageTag);
            bool taggedIngot = BlockUtils.HasTag(name, IngotStorageTag);
            bool taggedComp = BlockUtils.HasTag(name, ComponentStorageTag);
            bool taggedAmmo = BlockUtils.HasTag(name, AmmoStorageTag);
            bool taggedTool = BlockUtils.HasTag(name, ToolStorageTag);
            bool taggedBottle = BlockUtils.HasTag(name, BottleStorageTag);
            bool taggedGeneral = BlockUtils.HasTag(name, GeneralCargoTag);
            bool anySpecialTag = taggedOre || taggedIngot || taggedComp || taggedAmmo || taggedTool || taggedBottle;

            for (int invIdx = 0; invIdx < cargo.InventoryCount; invIdx++)
            {
                var inv = cargo.GetInventory(invIdx);
                if (taggedOre)
                {
                    _oreDestScratch.Add(inv);
                }

                if (taggedIngot)
                {
                    _ingotDestScratch.Add(inv);
                }

                if (taggedComp)
                {
                    _componentDestScratch.Add(inv);
                }

                if (taggedAmmo)
                {
                    _ammoDestScratch.Add(inv);
                }

                if (taggedTool)
                {
                    _toolDestScratch.Add(inv);
                }

                if (taggedBottle)
                {
                    _bottleDestScratch.Add(inv);
                }

                if (taggedGeneral || !anySpecialTag)
                {
                    _generalDestScratch.Add(inv);
                }
            }
        }
    }
}
