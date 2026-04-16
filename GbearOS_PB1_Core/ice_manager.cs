// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: ice_manager.cs
// Purpose: Measures and redistributes ice across gas generators, irrigation-tagged blocks, and cargo against Config targets.
// PB Association: PB1
// Dependencies: InventoryScanner, Config, IceStatusDTO
// Key Methods: IcePassStep

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
    public class IceManager
    {
        private static readonly MyItemType IceType = MyItemType.MakeOre("Ice");

        private readonly List<IMyGasGenerator> _generators = new List<IMyGasGenerator>();
        private readonly List<IMyTerminalBlock> _irrigationBlocks = new List<IMyTerminalBlock>();
        private readonly List<IMyCargoContainer> _cargoContainers = new List<IMyCargoContainer>();
        private double _scanGeneratorIce;
        private double _scanIrrigationIce;
        private double _scanCargoIce;

        private const int IcCap = 38000;
        private int _lastProcessedIndex;
        private byte _ifp = 4;
        private bool _iceRfCacheSeedDone;

        public bool IcePassStep(
            List<IMyTerminalBlock> globalBlocks,
            List<IMyCargoContainer> cachedCargo,
            List<IMyGasGenerator> cachedGasGenerators,
            List<IMyGasTank> cachedGasTanks,
            ref IceStatusDTO dto)
        {
            if (!EnsureApi())
            {
                _ifp = 4;
                IceClr(ref dto);
                return true;
            }

            if (_ifp == 4)
            {
                _generators.Clear();
                _irrigationBlocks.Clear();
                _cargoContainers.Clear();
                _iceRfCacheSeedDone = false;
                _ifp = 0;
                _lastProcessedIndex = 0;
            }

            if (_ifp == 0)
            {
                if (!IceRf(globalBlocks, cachedCargo, cachedGasGenerators))
                {
                    return false;
                }

                _ifp = 1;
                return false;
            }

            if (_ifp == 1)
            {
                IceCp(dto, BuildIceStatusDTO());
                _ifp = 2;
                return false;
            }

            DistributeIceIfNeeded();
            _ifp = 4;
            return true;
        }

        private static void IceClr(ref IceStatusDTO d)
        {
            d.totalIce = 0f;
            d.generatorIce = 0f;
            d.irrigationIce = 0f;
            d.cargoIce = 0f;
            d.pctTotal = 0f;
            d.pctGen = 0f;
            d.pctIrr = 0f;
            d.pctCargo = 0f;
            d.generatorCount = 0;
            d.irrigationCount = 0;
            d.lowIce = false;
        }

        private static void IceCp(IceStatusDTO dst, IceStatusDTO src)
        {
            dst.totalIce = src.totalIce;
            dst.generatorIce = src.generatorIce;
            dst.irrigationIce = src.irrigationIce;
            dst.cargoIce = src.cargoIce;
            dst.pctTotal = src.pctTotal;
            dst.pctGen = src.pctGen;
            dst.pctIrr = src.pctIrr;
            dst.pctCargo = src.pctCargo;
            dst.generatorCount = src.generatorCount;
            dst.irrigationCount = src.irrigationCount;
            dst.lowIce = src.lowIce;
        }

        private bool IceRf(
            List<IMyTerminalBlock> globalBlocks,
            List<IMyCargoContainer> cachedCargo,
            List<IMyGasGenerator> cachedGasGenerators)
        {
            if (globalBlocks == null)
            {
                return true;
            }

            if (!_iceRfCacheSeedDone)
            {
                if (cachedCargo != null)
                {
                    for (int cc = 0; cc < cachedCargo.Count; cc++)
                    {
                        var c = cachedCargo[cc];
                        if (c != null)
                        {
                            _cargoContainers.Add(c);
                        }
                    }
                }

                if (cachedGasGenerators != null)
                {
                    for (int gg = 0; gg < cachedGasGenerators.Count; gg++)
                    {
                        var g = cachedGasGenerators[gg];
                        if (g != null)
                        {
                            _generators.Add(g);
                        }
                    }
                }

                _iceRfCacheSeedDone = true;
            }

            for (int i = _lastProcessedIndex; i < globalBlocks.Count; i++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = i;
                    return false;
                }

                var b = globalBlocks[i];
                if (!OnConstructIceRelevantBlock(b))
                {
                    continue;
                }

                if (b is IMyGasGenerator)
                {
                    continue;
                }

                if (b is IMyCargoContainer)
                {
                    continue;
                }

                if (IsIrrigationTaggedBlock(b))
                {
                    _irrigationBlocks.Add(b);
                }
            }

            return true;
        }

        private bool OnConstructIceRelevantBlock(IMyTerminalBlock b)
        {
            if (!EnsureApi() || b == null)
            {
                return false;
            }

            if (!b.IsSameConstructAs(InventoryScanner.Api.Me))
            {
                return false;
            }

            if (IsManual(b))
            {
                return false;
            }

            if (b is IMyGasGenerator)
            {
                return true;
            }

            if (b is IMyCargoContainer)
            {
                return true;
            }

            return IsIrrigationTaggedBlock(b);
        }

        private void ScanIceTotals()
        {
            _scanGeneratorIce = SumIceInGenerators();
            _scanIrrigationIce = SumIceInIrrigationBlocks();
            _scanCargoIce = SumIceInCargoContainers();
        }

        private void DistributeIceIfNeeded()
        {
            var cfg = InventoryScanner.AppConfig;
            if (cfg == null)
            {
                return;
            }

            double minCargo = cfg.MinimumCargoIce;
            if (minCargo < 0)
            {
                minCargo = 0;
            }

            const double Epsilon = 0.001;
            for (int pass = 0; pass < 3; pass++)
            {
                double totalCargo = SumIceInCargoContainers();
                double spare = totalCargo - minCargo;
                if (spare <= Epsilon)
                {
                    break;
                }

                if (!TryMoveIceOneStep(cfg, spare))
                {
                    break;
                }
            }
        }

        private bool TryMoveIceOneStep(Config cfg, double spareBudget)
        {
            const double Epsilon = 0.001;
            IMyInventoryOwner bestConsumer = null;
            double bestDeficit = Epsilon;

            for (int g = 0; g < _generators.Count; g++)
            {
                var gen = _generators[g];
                var genOwner = gen as IMyInventoryOwner;
                if (genOwner == null)
                {
                    continue;
                }

                bool isLarge = gen.CubeGrid.GridSizeEnum == MyCubeSize.Large;
                double target = isLarge ? cfg.GeneratorLargeTargetIce : cfg.GeneratorSmallTargetIce;

                double have = SumIceInOwner(genOwner);
                double need = target - have;
                if (need <= Epsilon)
                {
                    continue;
                }

                if (need > bestDeficit)
                {
                    bestDeficit = need;
                    bestConsumer = genOwner;
                }
            }

            for (int r = 0; r < _irrigationBlocks.Count; r++)
            {
                var block = _irrigationBlocks[r];
                bool large = block.CubeGrid.GridSizeEnum == MyCubeSize.Large;
                double target = large ? cfg.IrrigationLargeTargetIce : cfg.IrrigationSmallTargetIce;
                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                double have = SumIceInOwner(owner);
                double need = target - have;
                if (need <= Epsilon)
                {
                    continue;
                }

                if (need > bestDeficit)
                {
                    bestDeficit = need;
                    bestConsumer = owner;
                }
            }

            if (bestConsumer == null)
            {
                return false;
            }

            double moveCap = spareBudget;
            if (bestDeficit < moveCap)
            {
                moveCap = bestDeficit;
            }

            for (int di = 0; di < bestConsumer.InventoryCount; di++)
            {
                var bestDst = bestConsumer.GetInventory(di);
                if (bestDst == null)
                {
                    continue;
                }

                for (int c = 0; c < _cargoContainers.Count; c++)
                {
                    var cargo = _cargoContainers[c];
                    for (int invIdx = 0; invIdx < cargo.InventoryCount; invIdx++)
                    {
                        var src = cargo.GetInventory(invIdx);
                        if (src == null || src == bestDst)
                        {
                            continue;
                        }

                        if (!src.IsConnectedTo(bestDst))
                        {
                            continue;
                        }

                        double chunkCap = moveCap;
                        if (!TryTransferIceFromTo(src, bestDst, ref chunkCap))
                        {
                            continue;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryTransferIceFromTo(IMyInventory src, IMyInventory dst, ref double moveCap)
        {
            if (moveCap <= 0.0001 || src == null || dst == null)
            {
                return false;
            }

            if (!src.IsConnectedTo(dst))
            {
                return false;
            }

            var items = new List<MyInventoryItem>();
            src.GetItems(items);
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var it = items[i];
                if (!IsIceItem(it.Type))
                {
                    continue;
                }

                double stackAmt = (double)it.Amount;
                double chunk = stackAmt;
                if (chunk > moveCap)
                {
                    chunk = moveCap;
                }

                if (chunk <= 0.0001)
                {
                    continue;
                }

                MyFixedPoint amt = (MyFixedPoint)chunk;
                if (amt <= (MyFixedPoint)0)
                {
                    continue;
                }

                if (!src.CanTransferItemTo(dst, IceType))
                {
                    continue;
                }

                if (src.TransferItemTo(dst, i, null, true, amt))
                {
                    moveCap -= chunk;
                    return true;
                }

                if (stackAmt <= moveCap + 0.0001 && src.TransferItemTo(dst, i, null, true, null))
                {
                    moveCap -= stackAmt;
                    return true;
                }
            }

            return false;
        }

        private IceStatusDTO BuildIceStatusDTO()
        {
            ScanIceTotals();

            double gen = _scanGeneratorIce;
            double irr = _scanIrrigationIce;
            double cargo = _scanCargoIce;
            double total = gen + irr + cargo;

            var cfg = InventoryScanner.AppConfig;
            bool low = cfg != null && cargo < cfg.CargoReserveIce;

            int generatorCount = _generators.Count;
            int irrigationCount = _irrigationBlocks.Count;

            double generatorCapacity = SumGeneratorTargetCapacity(cfg);
            double irrigationCapacity = SumIrrigationTargetCapacity(cfg);
            double cargoCapacity = SumCargoIceCapacity();

            float totalF = (float)total;
            float cargoF = (float)cargo;
            float totalCapacity = (float)(generatorCapacity + irrigationCapacity + cargoCapacity);
            float pctTotal;
            if (totalCapacity > 0f)
            {
                pctTotal = MathUtils.Clamp(totalF / totalCapacity, 0f, 1f);
            }
            else
            {
                pctTotal = 0f;
            }

            float pctGen = generatorCapacity > 0 ? Clamp01(gen / generatorCapacity) : 0f;
            float pctIrr = irrigationCapacity > 0 ? Clamp01(irr / irrigationCapacity) : 0f;
            float pctCargo = cargoCapacity > 0 ? Clamp01(cargo / cargoCapacity) : 0f;

            return new IceStatusDTO
            {
                totalIce = totalF,
                generatorIce = (float)gen,
                irrigationIce = (float)irr,
                cargoIce = cargoF,
                pctTotal = pctTotal,
                pctGen = pctGen,
                pctIrr = pctIrr,
                pctCargo = pctCargo,
                generatorCount = generatorCount,
                irrigationCount = irrigationCount,
                lowIce = low
            };
        }

        private static float Clamp01(double x)
        {
            if (x <= 0)
            {
                return 0f;
            }

            if (x >= 1)
            {
                return 1f;
            }

            return (float)x;
        }

        private double SumGeneratorTargetCapacity(Config cfg)
        {
            if (cfg == null)
            {
                return 0;
            }

            double sum = 0;
            for (int i = 0; i < _generators.Count; i++)
            {
                bool large = _generators[i].CubeGrid.GridSizeEnum == MyCubeSize.Large;
                sum += large ? cfg.GeneratorLargeTargetIce : cfg.GeneratorSmallTargetIce;
            }

            return sum;
        }

        private double SumIrrigationTargetCapacity(Config cfg)
        {
            if (cfg == null)
            {
                return 0;
            }

            double sum = 0;
            for (int i = 0; i < _irrigationBlocks.Count; i++)
            {
                bool large = _irrigationBlocks[i].CubeGrid.GridSizeEnum == MyCubeSize.Large;
                sum += large ? cfg.IrrigationLargeTargetIce : cfg.IrrigationSmallTargetIce;
            }

            return sum;
        }

        private double SumCargoIceCapacity()
        {
            double sum = 0;
            for (int c = 0; c < _cargoContainers.Count; c++)
            {
                var owner = _cargoContainers[c] as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                for (int i = 0; i < owner.InventoryCount; i++)
                {
                    var inv = owner.GetInventory(i);
                    if (inv == null)
                    {
                        continue;
                    }

                    sum += (double)inv.GetItemAmount(IceType);
                    sum += MaxAdditionalIceThatFits(inv);
                }
            }

            return sum;
        }

        private static double MaxAdditionalIceThatFits(IMyInventory inv)
        {
            if (inv == null || inv.MaxVolume.RawValue <= 0)
            {
                return 0;
            }

            const double hiCap = 2e9;
            double lo = 0;
            double hi = hiCap;
            for (int iter = 0; iter < 40; iter++)
            {
                double mid = (lo + hi) * 0.5;
                if (mid <= 0)
                {
                    hi = mid;
                    continue;
                }

                MyFixedPoint amt = (MyFixedPoint)mid;
                if (inv.CanItemsBeAdded(amt, IceType))
                {
                    lo = mid;
                }
                else
                {
                    hi = mid;
                }
            }

            return lo;
        }

        private static bool EnsureApi()
        {
            return InventoryScanner.Api != null
                   && InventoryScanner.Api.Me != null
                   && InventoryScanner.Api.GridTerminalSystem != null;
        }

        private static string ManualTagValue()
        {
            return InventoryScanner.AppConfig != null && !string.IsNullOrEmpty(InventoryScanner.AppConfig.ManualTag)
                ? InventoryScanner.AppConfig.ManualTag
                : "[Manual]";
        }

        private static string IrrigationTagValue()
        {
            return InventoryScanner.AppConfig != null && !string.IsNullOrEmpty(InventoryScanner.AppConfig.IrrigationTag)
                ? InventoryScanner.AppConfig.IrrigationTag
                : "[Irrigator]";
        }

        private static bool IsManual(IMyTerminalBlock block)
        {
            if (block == null)
            {
                return false;
            }

            string tag = ManualTagValue();
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            string name = block.CustomName;
            return BlockUtils.HasTag(name, tag);
        }

        private static bool IsIrrigationTaggedBlock(IMyTerminalBlock b)
        {
            var owner = b as IMyInventoryOwner;
            if (owner == null || owner.InventoryCount <= 0)
            {
                return false;
            }

            string tag = IrrigationTagValue();
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            string name = b.CustomName;
            return BlockUtils.HasTag(name, tag);
        }

        private static bool IsIceItem(MyItemType t)
        {
            if (t.SubtypeId != "Ice")
            {
                return false;
            }

            string id = t.TypeId.ToString();
            return id.IndexOf("Ore", StringComparison.Ordinal) >= 0;
        }

        private double SumIceInGenerators()
        {
            double sum = 0;
            for (int i = 0; i < _generators.Count; i++)
            {
                var owner = _generators[i] as IMyInventoryOwner;
                if (owner != null)
                {
                    sum += SumIceInOwner(owner);
                }
            }

            return sum;
        }

        private double SumIceInIrrigationBlocks()
        {
            double sum = 0;
            for (int i = 0; i < _irrigationBlocks.Count; i++)
            {
                var owner = _irrigationBlocks[i] as IMyInventoryOwner;
                if (owner != null)
                {
                    sum += SumIceInOwner(owner);
                }
            }

            return sum;
        }

        private double SumIceInCargoContainers()
        {
            double sum = 0;
            for (int c = 0; c < _cargoContainers.Count; c++)
            {
                var cargo = _cargoContainers[c];
                var owner = cargo as IMyInventoryOwner;
                if (owner != null)
                {
                    sum += SumIceInOwner(owner);
                }
            }

            return sum;
        }

        private static double SumIceInOwner(IMyInventoryOwner owner)
        {
            if (owner == null)
            {
                return 0;
            }

            double sum = 0;
            for (int i = 0; i < owner.InventoryCount; i++)
            {
                var inv = owner.GetInventory(i);
                if (inv != null)
                {
                    sum += (double)inv.GetItemAmount(IceType);
                }
            }

            return sum;
        }
    }
}
