// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: refinery_manager.cs
// Purpose: Refinery discovery, ore priority/balancing, pull/cascade logic, and RefineryStatusDTO construction.
// PB Association: PB1
// Dependencies: Config, InventoryScanner, RefineryStatusDTO
// Key Methods: RefineryPassStep, GetCascadeDiagnostic

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public sealed class RefineryManager
    {
        private const double Alpha = 0.05;
        private const double Floor = 1000;
        private const double Cap = 100000;
        private const double StoneThresh = 50000;
        private const double UnknownYield = 0.3;
        private const double IronTgtDef = 125000;

        private readonly List<IMyRefinery> _refineries = new List<IMyRefinery>();
        private readonly HashSet<IMyInventory> _refineryNoPullInv = new HashSet<IMyInventory>();
        private List<IMyTerminalBlock> _globalBlocks;
        private readonly List<string> _oreTypesWork = new List<string>();
        private readonly Dictionary<string, double> _ingotTotalsScratch = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private readonly List<MyInventoryItem> _balItemScratch = new List<MyInventoryItem>();
        private readonly Dictionary<string, int> _priorityRanksScratch = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly List<KeyValuePair<string, int>> _priorityEntriesScratch = new List<KeyValuePair<string, int>>(32);
        private static readonly int[] _topOrePullChunks = { 2000, 1000, 500, 250, 100, 50 };
        private string _lastTopPriorityOre = string.Empty;

        private const int IcCap = 38000;
        private int _lastProcessedIndex;
        private byte _rfp = 255;
        private int _balOreT;
        private Dictionary<string, int> _priSnap;

        private static int _ti(int i, int a, int b)
        {
            if (b <= 0)
            {
                return 0;
            }

            if (a < 4)
            {
                return 0;
            }

            int t;
            if (a < 8)
            {
                int n0 = (2 * a + 2) / 3;
                t = i < n0 ? 0 : 1;
            }
            else
            {
                int n0 = (a + 1) / 2;
                int rem = a - n0;
                int n1 = (rem + 1) / 2;
                if (i < n0)
                {
                    t = 0;
                }
                else if (i < n0 + n1)
                {
                    t = 1;
                }
                else
                {
                    t = 2;
                }
            }

            if (t >= b)
            {
                t = b - 1;
            }

            return t;
        }

        private static double _gy(string s)
        {
            if (s == null)
            {
                return UnknownYield;
            }

            switch (s.ToLowerInvariant())
            {
                case "iron":
                case "silicon":
                    return 0.7;
                case "nickel":
                    return 0.4;
                case "cobalt":
                    return 0.3;
                case "magnesium":
                    return 0.007;
                case "silver":
                    return 0.1;
                case "gold":
                    return 0.01;
                case "platinum":
                    return 0.005;
                case "uranium":
                    return 0.01;
                default:
                    return UnknownYield;
            }
        }

        private static double _dm(string oreSub, Config cfg)
        {
            if (string.Equals(oreSub, "Stone", StrIX.C))
            {
                return StoneThresh;
            }

            double T;
            string prod = IngotProductSubtypeForOre(oreSub);
            if (prod == null || !TryResolveIngotTarget(cfg, prod, out T) || T <= 0)
            {
                if (!TryResolveIngotTarget(cfg, "Iron", out T) || T <= 0)
                {
                    T = IronTgtDef;
                }
            }

            double y = _gy(oreSub);
            if (y <= 0)
            {
                y = UnknownYield;
            }

            double th = Alpha * (T / y);
            if (th < Floor)
            {
                th = Floor;
            }

            if (th > Cap)
            {
                th = Cap;
            }

            return th;
        }

        public bool RefineryPassStep(List<IMyTerminalBlock> globalBlocks, List<IMyRefinery> cachedRefineries, ref RefineryStatusDTO dto)
        {
            if (!EnsureContext())
            {
                _rfp = 255;
                RfClr(dto);
                return true;
            }

            if (_rfp == 255)
            {
                _globalBlocks = globalBlocks;
                RefreshRefineryCache(cachedRefineries);
                Config cfg0 = InventoryScanner.AppConfig;
                SetRefConvPolicy(cfg0 != null && cfg0.EnableRefineryBalancing);
                _oreTypesWork.Clear();
                _lastProcessedIndex = 0;
                _rfp = 1;
                return false;
            }

            if (_rfp == 1)
            {
                if (!RfColOre())
                {
                    return false;
                }

                _ingotTotalsScratch.Clear();
                _lastProcessedIndex = 0;
                _rfp = 2;
                return false;
            }

            if (_rfp == 2)
            {
                if (!RfColIng())
                {
                    return false;
                }

                SortOreTypesByTargetFillRatio();
                _priSnap = BuildComputedPriorityDisplayRanks();
                Config cfg = InventoryScanner.AppConfig;
                bool bal = cfg != null && cfg.EnableRefineryBalancing;
                if (bal)
                {
                    _balOreT = 0;
                    _lastProcessedIndex = 0;
                    _rfp = 4;
                }
                else
                {
                    _rfp = 6;
                }

                return false;
            }

            if (_rfp == 4)
            {
                if (!RfBalance())
                {
                    return false;
                }

                _lastProcessedIndex = 0;
                _rfp = 5;
                return false;
            }

            if (_rfp == 5)
            {
                if (!RfDistrib())
                {
                    return false;
                }

                _rfp = 6;
                return false;
            }

            if (_rfp == 6)
            {
                var built = BuildRefineryStatusDTO(_priSnap);
                RfPub(dto, built);
                _rfp = 255;
                return true;
            }

            return true;
        }

        private static void RfClr(RefineryStatusDTO d)
        {
            d.refineryNames = new string[0];
            d.currentOre = new string[0];
            d.oreAmounts = new float[0];
            d.outputIngot = new string[0];
            d.outputAmounts = new float[0];
            d.isWorking = new bool[0];
            d.hasOre = new bool[0];
            d.priorityLine1 = null;
            d.priorityLine2 = null;
        }

        private static void RfPub(RefineryStatusDTO dst, RefineryStatusDTO src)
        {
            dst.refineryNames = src.refineryNames;
            dst.currentOre = src.currentOre;
            dst.oreAmounts = src.oreAmounts;
            dst.outputIngot = src.outputIngot;
            dst.outputAmounts = src.outputAmounts;
            dst.isWorking = src.isWorking;
            dst.hasOre = src.hasOre;
            dst.priorityLine1 = src.priorityLine1;
            dst.priorityLine2 = src.priorityLine2;
        }

        private bool RfColOre()
        {
            if (_globalBlocks == null)
            {
                return true;
            }

            for (int bi = _lastProcessedIndex; bi < _globalBlocks.Count; bi++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = bi;
                    return false;
                }

                var block = _globalBlocks[bi];
                if (IsManual(block))
                {
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                for (int ii = 0; ii < owner.InventoryCount; ii++)
                {
                    var inv = owner.GetInventory(ii);
                    if (inv == null)
                    {
                        continue;
                    }

                    var items = new List<MyInventoryItem>();
                    inv.GetItems(items);
                    for (int j = 0; j < items.Count; j++)
                    {
                        var t = items[j].Type;
                        if (!IsOreType(t))
                        {
                            continue;
                        }

                        string sub = t.SubtypeId;
                        if (string.Equals(sub, "Ice", StrIX.C))
                        {
                            continue;
                        }

                        bool found = false;
                        for (int k = 0; k < _oreTypesWork.Count; k++)
                        {
                            if (string.Equals(_oreTypesWork[k], sub, StrIX.C))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            _oreTypesWork.Add(sub);
                        }
                    }
                }
            }

            return true;
        }

        private bool RfColIng()
        {
            if (_globalBlocks == null)
            {
                return true;
            }

            for (int bi = _lastProcessedIndex; bi < _globalBlocks.Count; bi++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = bi;
                    return false;
                }

                var block = _globalBlocks[bi];
                if (IsManual(block))
                {
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                for (int ii = 0; ii < owner.InventoryCount; ii++)
                {
                    var inv = owner.GetInventory(ii);
                    if (inv == null)
                    {
                        continue;
                    }

                    var items = new List<MyInventoryItem>();
                    inv.GetItems(items);
                    for (int j = 0; j < items.Count; j++)
                    {
                        var it = items[j];
                        if (!IsIngotType(it.Type))
                        {
                            continue;
                        }

                        string sub = it.Type.SubtypeId;
                        double amt = (double)it.Amount;
                        double cur;
                        if (!_ingotTotalsScratch.TryGetValue(sub, out cur))
                        {
                            cur = 0;
                        }

                        _ingotTotalsScratch[sub] = cur + amt;
                    }
                }
            }

            return true;
        }

        private bool RfBalance()
        {
            if (_oreTypesWork.Count == 0)
            {
                return true;
            }

            int transfers = 0;
            const int maxTransfersPerTick = 20;
            for (int t = _balOreT; t < _oreTypesWork.Count && transfers < maxTransfersPerTick; t++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _balOreT = t;
                    return false;
                }

                string oreSubtype = _oreTypesWork[t];
                while (transfers < maxTransfersPerTick && TryTransferOneStackOfSubtype(oreSubtype))
                {
                    if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                    {
                        _balOreT = t;
                        return false;
                    }

                    transfers++;
                }
            }

            return true;
        }

        private bool RfDistrib()
        {
            if (_oreTypesWork.Count == 0)
            {
                return true;
            }

            int a = _refineries.Count;
            int b = _oreTypesWork.Count;
            for (int i = _lastProcessedIndex; i < a; i++)
            {
                if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                {
                    _lastProcessedIndex = i;
                    return false;
                }

                var r = _refineries[i];
                var recvInv = r.InputInventory;
                if (recvInv == null)
                {
                    continue;
                }

                string p0 = _oreTypesWork[_ti(i, a, b)];

                if (RefineryInputOreSubtypeTotal(r, p0) <= 50f && InputFreeVolume(r).RawValue > 0)
                {
                    IMyInventory srcInv;
                    int srcIdx;
                    if (FindLargestOreSubtypeStack(p0, recvInv, out srcInv, out srcIdx))
                    {
                        for (int k = 0; k < _topOrePullChunks.Length; k++)
                        {
                            if (InventoryScanner.Api.Runtime.CurrentInstructionCount > IcCap)
                            {
                                _lastProcessedIndex = i;
                                return false;
                            }

                            srcInv.GetItems(_balItemScratch);
                            if (srcIdx >= _balItemScratch.Count)
                            {
                                break;
                            }

                            MyItemType moveType = _balItemScratch[srcIdx].Type;
                            if (!srcInv.CanTransferItemTo(recvInv, moveType))
                            {
                                break;
                            }

                            MyFixedPoint amt = _balItemScratch[srcIdx].Amount;
                            if (amt <= (MyFixedPoint)0)
                            {
                                break;
                            }

                            MyFixedPoint moveAmt = (MyFixedPoint)_topOrePullChunks[k];
                            if (amt < moveAmt)
                            {
                                moveAmt = amt;
                            }

                            if (moveAmt <= (MyFixedPoint)0)
                            {
                                continue;
                            }

                            if (srcInv.TransferItemTo(recvInv, srcIdx, null, true, moveAmt))
                            {
                                break;
                            }
                        }
                    }
                }

                _balItemScratch.Clear();
                recvInv.GetItems(_balItemScratch);
                int bestIdx = -1;
                MyFixedPoint bestAmt = (MyFixedPoint)0;
                for (int j = 0; j < _balItemScratch.Count; j++)
                {
                    var it = _balItemScratch[j];
                    if (!IsOreType(it.Type))
                    {
                        continue;
                    }

                    if (!string.Equals(it.Type.SubtypeId, p0, StrIX.C))
                    {
                        continue;
                    }

                    if (it.Amount > bestAmt)
                    {
                        bestAmt = it.Amount;
                        bestIdx = j;
                    }
                }

                if (bestIdx > 0)
                {
                    recvInv.TransferItemTo(recvInv, bestIdx, 0, true, null);
                }
            }

            return true;
        }

        public string GetCascadeDiagnostic(bool isBalancingEnabled)
        {
            if (!isBalancingEnabled || _refineries.Count == 0 || _oreTypesWork.Count == 0)
            {
                return "--- REFINERY CASCADE ---\n"
                       + "Refinery Balancing: OFF / STANDBY\n"
                       + "(No scripted chunk pull; conveyors route ore when balancing is OFF.)";
            }

            int a = _refineries.Count;
            int b = _oreTypesWork.Count;
            int s0 = 0;
            int s1 = 0;
            int s2 = 0;
            for (int i = 0; i < a; i++)
            {
                int t = _ti(i, a, b);
                if (t == 0)
                {
                    s0++;
                }
                else if (t == 1)
                {
                    s1++;
                }
                else
                {
                    s2++;
                }
            }

            var sb = new StringBuilder(320);
            sb.AppendLine("--- REFINERY CASCADE ---");
            sb.Append("Total Refineries: ");
            sb.AppendLine(a.ToString());
            sb.AppendLine("(Live chunk pull uses the top 3 ranked ores in order; deeper ranks are balanced separately.)");

            int rows = b < 3 ? b : 3;
            for (int slot = 0; slot < rows; slot++)
            {
                int n = slot == 0 ? s0 : (slot == 1 ? s1 : s2);
                sb.Append('[');
                sb.Append('P');
                sb.Append((char)('1' + slot));
                sb.Append("] ");
                sb.Append(_oreTypesWork[slot]);
                sb.Append(": ");
                sb.Append(n);
                sb.AppendLine(" assigned");
            }

            return sb.ToString();
        }

        private static bool EnsureContext()
        {
            return InventoryScanner.Api != null
                   && InventoryScanner.Api.Me != null
                   && InventoryScanner.Api.GridTerminalSystem != null;
        }

        private string ManualTagValue()
        {
            Config ac = InventoryScanner.AppConfig;
            return ac != null && !string.IsNullOrEmpty(ac.ManualTag) ? ac.ManualTag : "[Manual]";
        }

        private bool IsManual(IMyTerminalBlock block)
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

        private static bool IsOnConstructWithInventory(IMyTerminalBlock block)
        {
            if (block == null || !block.IsSameConstructAs(InventoryScanner.Api.Me))
            {
                return false;
            }

            var owner = block as IMyInventoryOwner;
            return owner != null && owner.InventoryCount > 0;
        }

        private void RefreshRefineryCache(List<IMyRefinery> cachedRefineries)
        {
            _refineries.Clear();
            _refineryNoPullInv.Clear();

            if (cachedRefineries == null)
            {
                return;
            }

            for (int i = 0; i < cachedRefineries.Count; i++)
            {
                var refinery = cachedRefineries[i];
                if (refinery == null)
                {
                    continue;
                }

                // Exclude only refinery outputs from ore pull sources (ingots / byproducts).
                // Inputs must stay eligible so ore can be moved between refineries for balancing.
                var outputInv = refinery.OutputInventory;
                if (outputInv != null)
                {
                    _refineryNoPullInv.Add(outputInv);
                }

                _refineries.Add(refinery);
            }
        }

        private void SetRefConvPolicy(bool scriptBalancesOreInputs)
        {
            for (int i = 0; i < _refineries.Count; i++)
            {
                var prod = _refineries[i] as IMyProductionBlock;
                if (prod == null)
                {
                    continue;
                }

                prod.UseConveyorSystem = !scriptBalancesOreInputs;
            }
        }

        private static bool IsOreType(MyItemType t)
        {
            string id = t.TypeId.ToString();
            return id.IndexOf("Ore", StringComparison.Ordinal) >= 0;
        }

        private static bool IsIngotType(MyItemType t)
        {
            return t.TypeId.ToString().IndexOf("Ingot", StringComparison.Ordinal) >= 0;
        }

        private static MyFixedPoint InputFreeVolume(IMyRefinery r)
        {
            var inv = r.InputInventory;
            if (inv == null || inv.MaxVolume.RawValue <= 0)
            {
                return (MyFixedPoint)0;
            }

            return inv.MaxVolume - inv.CurrentVolume;
        }

        private float RefineryInputOreSubtypeTotal(IMyRefinery r, string oreSubtype)
        {
            var inv = r.InputInventory;
            if (inv == null)
            {
                return 0f;
            }

            float sum = 0f;
            _balItemScratch.Clear();
            inv.GetItems(_balItemScratch);
            for (int j = 0; j < _balItemScratch.Count; j++)
            {
                var it = _balItemScratch[j];
                if (IsOreType(it.Type) && string.Equals(it.Type.SubtypeId, oreSubtype, StrIX.C))
                {
                    sum += (float)it.Amount;
                }
            }

            return sum;
        }

        private bool TryTransferStackToRefineryInput(
            IMyInventory srcInv,
            int srcIdx,
            IMyInventory dstInv)
        {
            if (srcInv == null || dstInv == null || srcInv == dstInv || srcIdx < 0)
            {
                return false;
            }

            srcInv.GetItems(_balItemScratch);
            if (srcIdx >= _balItemScratch.Count)
            {
                return false;
            }

            MyItemType moveType = _balItemScratch[srcIdx].Type;
            if (!srcInv.CanTransferItemTo(dstInv, moveType))
            {
                return false;
            }

            if (srcInv.TransferItemTo(dstInv, srcIdx, null, true, null))
            {
                return true;
            }

            srcInv.GetItems(_balItemScratch);
            if (srcIdx >= _balItemScratch.Count)
            {
                return false;
            }

            MyFixedPoint amt = _balItemScratch[srcIdx].Amount;
            if (amt <= (MyFixedPoint)0)
            {
                return false;
            }

            for (int p = 1; p <= 8; p++)
            {
                double chunk = (double)amt / (1 << p);
                if (chunk < 0.01)
                {
                    break;
                }

                MyFixedPoint tryAmt = (MyFixedPoint)chunk;
                if (tryAmt <= (MyFixedPoint)0)
                {
                    continue;
                }

                if (srcInv.TransferItemTo(dstInv, srcIdx, null, true, tryAmt))
                {
                    return true;
                }
            }

            return false;
        }

        private bool FindLargestOreSubtypeStack(
            string oreSubtype,
            IMyInventory excludeDst,
            out IMyInventory srcInv,
            out int srcIdx)
        {
            srcInv = null;
            srcIdx = -1;
            MyFixedPoint bestAmt = (MyFixedPoint)0;
            MyFixedPoint minD = (MyFixedPoint)_dm(oreSubtype, InventoryScanner.AppConfig);

            if (_globalBlocks == null)
            {
                return false;
            }

            for (int bi = 0; bi < _globalBlocks.Count; bi++)
            {
                var block = _globalBlocks[bi];
                if (IsManual(block))
                {
                    continue;
                }

                var owner = block as IMyInventoryOwner;
                if (owner == null)
                {
                    continue;
                }

                var rf = block as IMyRefinery;

                for (int ii = 0; ii < owner.InventoryCount; ii++)
                {
                    var inv = owner.GetInventory(ii);
                    if (inv == null || inv == excludeDst || _refineryNoPullInv.Contains(inv))
                    {
                        continue;
                    }

                    _balItemScratch.Clear();
                    inv.GetItems(_balItemScratch);
                    for (int j = 0; j < _balItemScratch.Count; j++)
                    {
                        var it = _balItemScratch[j];
                        if (!IsOreType(it.Type))
                        {
                            continue;
                        }

                        if (!string.Equals(it.Type.SubtypeId, oreSubtype, StrIX.C))
                        {
                            continue;
                        }

                        if (rf != null && it.Amount <= minD)
                        {
                            continue;
                        }

                        if (it.Amount > bestAmt)
                        {
                            bestAmt = it.Amount;
                            srcInv = inv;
                            srcIdx = j;
                        }
                    }
                }
            }

            return srcInv != null && srcIdx >= 0 && bestAmt > (MyFixedPoint)0;
        }

        private bool TryTransferOneStackOfSubtype(string oreSubtype, int forceRecvIdx = -1)
        {
            int n = _refineries.Count;
            if (n == 0)
            {
                return false;
            }

            const float balanceGap = 0.5f;
            int recvIdx;
            float recvMinAmt;

            if (forceRecvIdx >= 0 && forceRecvIdx < n)
            {
                recvIdx = forceRecvIdx;
                if (InputFreeVolume(_refineries[recvIdx]).RawValue <= 0)
                {
                    return false;
                }

                recvMinAmt = RefineryInputOreSubtypeTotal(_refineries[recvIdx], oreSubtype);
            }
            else
            {
                recvIdx = -1;
                recvMinAmt = float.PositiveInfinity;
                long recvMaxFreeRaw = -1;
                for (int i = 0; i < n; i++)
                {
                    MyFixedPoint free = InputFreeVolume(_refineries[i]);
                    if (free.RawValue <= 0)
                    {
                        continue;
                    }

                    float amt = RefineryInputOreSubtypeTotal(_refineries[i], oreSubtype);
                    long fr = free.RawValue;
                    if (amt < recvMinAmt - 1e-4f || (Math.Abs(amt - recvMinAmt) < 1e-4f && fr > recvMaxFreeRaw))
                    {
                        recvMinAmt = amt;
                        recvMaxFreeRaw = fr;
                        recvIdx = i;
                    }
                }

                if (recvIdx < 0)
                {
                    return false;
                }
            }

            IMyInventory recvInv = _refineries[recvIdx].InputInventory;
            if (recvInv == null)
            {
                return false;
            }

            int donorIdx = -1;
            float donorMaxAmt = -1f;
            for (int i = 0; i < n; i++)
            {
                if (i == recvIdx)
                {
                    continue;
                }

                float amt = RefineryInputOreSubtypeTotal(_refineries[i], oreSubtype);
                if (amt > donorMaxAmt)
                {
                    donorMaxAmt = amt;
                    donorIdx = i;
                }
            }

            if (donorIdx >= 0 && donorMaxAmt > recvMinAmt + balanceGap)
            {
                var donorInv = _refineries[donorIdx].InputInventory;
                if (donorInv != null && donorInv != recvInv)
                {
                    _balItemScratch.Clear();
                    donorInv.GetItems(_balItemScratch);
                    int bestJ = -1;
                    MyFixedPoint bestStack = (MyFixedPoint)0;
                    for (int j = 0; j < _balItemScratch.Count; j++)
                    {
                        var it = _balItemScratch[j];
                        if (!IsOreType(it.Type) || !string.Equals(it.Type.SubtypeId, oreSubtype, StrIX.C))
                        {
                            continue;
                        }

                        if (it.Amount > bestStack)
                        {
                            bestStack = it.Amount;
                            bestJ = j;
                        }
                    }

                    if (bestJ >= 0
                        && InputFreeVolume(_refineries[recvIdx]).RawValue > 0
                        && TryTransferStackToRefineryInput(donorInv, bestJ, recvInv))
                    {
                        return true;
                    }
                }
            }

            IMyInventory srcInv;
            int srcIdx;
            if (!FindLargestOreSubtypeStack(oreSubtype, recvInv, out srcInv, out srcIdx))
            {
                return false;
            }

            return TryTransferStackToRefineryInput(srcInv, srcIdx, recvInv);
        }

        private static string IngotProductSubtypeForOre(string oreSub)
        {
            if (string.IsNullOrEmpty(oreSub))
            {
                return null;
            }

            if (string.Equals(oreSub, "Ice", StrIX.C))
            {
                return null;
            }

            return oreSub;
        }

        private static bool TryResolveIngotTarget(Config cfg, string ingotSubtype, out double target)
        {
            target = 0;
            if (cfg == null || cfg.IngotTargets == null || string.IsNullOrEmpty(ingotSubtype))
            {
                return false;
            }

            if (cfg.IngotTargets.TryGetValue(ingotSubtype, out target))
            {
                return true;
            }

            string prefixed = "Ingot/" + ingotSubtype;
            return cfg.IngotTargets.TryGetValue(prefixed, out target);
        }

        private static readonly string[] StoneRefineryIngotProducts = { "Iron", "Nickel", "Silicon", "Gravel" };

        private double StoneOreCompositeTargetFillRatio(Config cfg)
        {
            double worst = double.PositiveInfinity;
            int considered = 0;
            for (int i = 0; i < StoneRefineryIngotProducts.Length; i++)
            {
                string product = StoneRefineryIngotProducts[i];
                double target;
                if (!TryResolveIngotTarget(cfg, product, out target))
                {
                    continue;
                }

                considered++;
                if (target <= 0)
                {
                    if (1.0 < worst)
                    {
                        worst = 1.0;
                    }

                    continue;
                }

                double stock;
                if (!_ingotTotalsScratch.TryGetValue(product, out stock))
                {
                    stock = 0;
                }

                double r = stock / target;
                if (r < worst)
                {
                    worst = r;
                }
            }

            if (considered == 0)
            {
                return double.PositiveInfinity;
            }

            return worst;
        }

        private double IngotStockToTargetRatio(string ingotSubtype, Config cfg)
        {
            double target;
            if (!TryResolveIngotTarget(cfg, ingotSubtype, out target))
            {
                return double.PositiveInfinity;
            }

            if (target <= 0)
            {
                return 1.0;
            }

            double stock;
            if (!_ingotTotalsScratch.TryGetValue(ingotSubtype, out stock))
            {
                stock = 0;
            }

            return stock / target;
        }

        private double OreRefineryStockToTargetRatio(string oreSubtype, Config cfg)
        {
            if (string.Equals(oreSubtype, "Stone", StrIX.C))
            {
                return StoneOreCompositeTargetFillRatio(cfg);
            }

            string product = IngotProductSubtypeForOre(oreSubtype);
            if (product == null)
            {
                return double.PositiveInfinity;
            }

            return IngotStockToTargetRatio(product, cfg);
        }

        private void SortOreTypesByTargetFillRatio()
        {
            Config cfg = InventoryScanner.AppConfig;
            double hysteresis = cfg != null ? cfg.RefineryHysteresis : 0.05;
            if (hysteresis < 0)
            {
                hysteresis = 0;
            }

            int c = _oreTypesWork.Count;
            for (int a = 0; a < c - 1; a++)
            {
                for (int b = a + 1; b < c; b++)
                {
                    string sa = _oreTypesWork[a];
                    string sb = _oreTypesWork[b];
                    double ra = OreRefineryStockToTargetRatio(sa, cfg);
                    double rb = OreRefineryStockToTargetRatio(sb, cfg);
                    if (hysteresis > 0 && !string.IsNullOrEmpty(_lastTopPriorityOre))
                    {
                        if (string.Equals(sa, _lastTopPriorityOre, StrIX.C))
                        {
                            ra -= hysteresis;
                        }

                        if (string.Equals(sb, _lastTopPriorityOre, StrIX.C))
                        {
                            rb -= hysteresis;
                        }
                    }

                    if (ra > rb || (ra == rb && string.CompareOrdinal(sa, sb) > 0))
                    {
                        _oreTypesWork[a] = sb;
                        _oreTypesWork[b] = sa;
                    }
                }
            }

            if (c > 0)
            {
                _lastTopPriorityOre = _oreTypesWork[0];
            }
            else
            {
                _lastTopPriorityOre = string.Empty;
            }
        }

        private Dictionary<string, int> BuildComputedPriorityDisplayRanks()
        {
            _priorityRanksScratch.Clear();
            for (int i = 0; i < _oreTypesWork.Count; i++)
            {
                _priorityRanksScratch[_oreTypesWork[i]] = i + 1;
            }

            return _priorityRanksScratch;
        }

        private static string FrontInputSlotOreSubtype(IMyRefinery r, out float totalOreSum)
        {
            totalOreSum = 0f;
            var inv = r.InputInventory;
            if (inv == null)
            {
                return string.Empty;
            }

            var items = new List<MyInventoryItem>();
            inv.GetItems(items);

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                if (IsOreType(it.Type))
                {
                    totalOreSum += (float)it.Amount;
                }
            }

            if (items.Count == 0)
            {
                return string.Empty;
            }

            var front = items[0];
            if (!IsOreType(front.Type))
            {
                return string.Empty;
            }

            return front.Type.SubtypeId;
        }

        private static string DominantIngotSubtypeInOutput(IMyRefinery r, out float ingotSum)
        {
            ingotSum = 0f;
            var inv = r.OutputInventory;
            if (inv == null)
            {
                return string.Empty;
            }

            string bestSub = string.Empty;
            float bestAmt = 0f;

            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                if (!IsIngotType(it.Type))
                {
                    continue;
                }

                float a = (float)it.Amount;
                ingotSum += a;
                if (a > bestAmt)
                {
                    bestAmt = a;
                    bestSub = it.Type.SubtypeId;
                }
            }

            return bestSub;
        }

        private static bool IsRefineryWorking(IMyRefinery r)
        {
            var prod = r as IMyProductionBlock;
            return prod != null && prod.IsProducing;
        }

        private RefineryStatusDTO BuildRefineryStatusDTO(Dictionary<string, int> priorityRanks)
        {
            int n = _refineries.Count;
            var dto = new RefineryStatusDTO();
            if (n == 0)
            {
                dto.refineryNames = new string[0];
                dto.currentOre = new string[0];
                dto.oreAmounts = new float[0];
                dto.outputIngot = new string[0];
                dto.outputAmounts = new float[0];
                dto.isWorking = new bool[0];
                dto.hasOre = new bool[0];
                AssignPriorityDisplayLines(dto, priorityRanks);
                return dto;
            }

            var names = new string[n];
            var curOre = new string[n];
            var oreAmts = new float[n];
            var outIng = new string[n];
            var outAmts = new float[n];
            var working = new bool[n];
            var hasOre = new bool[n];

            for (int i = 0; i < n; i++)
            {
                var rf = _refineries[i];
                names[i] = SanitizeIngressWireText(rf.CustomName);

                float oSum;
                string oSub = FrontInputSlotOreSubtype(rf, out oSum);
                curOre[i] = oSub;
                oreAmts[i] = oSum;
                hasOre[i] = oSum > 0.0001f;

                float iSum;
                string iSub = DominantIngotSubtypeInOutput(rf, out iSum);
                outIng[i] = iSum > 0.0001f ? iSub : string.Empty;
                outAmts[i] = iSum;

                working[i] = IsRefineryWorking(rf);
            }

            dto.refineryNames = names;
            dto.currentOre = curOre;
            dto.oreAmounts = oreAmts;
            dto.outputIngot = outIng;
            dto.outputAmounts = outAmts;
            dto.isWorking = working;
            dto.hasOre = hasOre;
            AssignPriorityDisplayLines(dto, priorityRanks);
            return dto;
        }

        /// <summary>
        /// Sanitizes player-controlled text so it cannot corrupt the semicolon-delimited DTO wire format.
        /// Applied once at ingress (scan layer) before any DTO population/serialization/MAC signing.
        /// </summary>
        private static string SanitizeIngressWireText(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return string.Empty;
            }

            int n = raw.Length;
            int firstBad = -1;
            for (int i = 0; i < n; i++)
            {
                char c = raw[i];
                if (c == ';' || c == '|' || c == '\\' || c == '\r' || c == '\n')
                {
                    firstBad = i;
                    break;
                }
            }

            if (firstBad < 0)
            {
                return raw;
            }

            char[] buf = new char[n];
            for (int i = 0; i < firstBad; i++)
            {
                buf[i] = raw[i];
            }

            for (int i = firstBad; i < n; i++)
            {
                char c = raw[i];
                buf[i] = (c == ';' || c == '|' || c == '\\' || c == '\r' || c == '\n') ? ' ' : c;
            }

            return new string(buf);
        }

        private void AssignPriorityDisplayLines(RefineryStatusDTO dto, Dictionary<string, int> priorityRanks)
        {
            dto.priorityLine1 = null;
            dto.priorityLine2 = null;
            if (priorityRanks == null || priorityRanks.Count == 0)
            {
                return;
            }

            _priorityEntriesScratch.Clear();
            foreach (var kv in priorityRanks)
            {
                if (string.Equals(kv.Key, "Ice", StrIX.C))
                {
                    continue;
                }

                _priorityEntriesScratch.Add(kv);
            }

            _priorityEntriesScratch.Sort((a, b) =>
            {
                int c = a.Value.CompareTo(b.Value);
                if (c != 0)
                {
                    return c;
                }

                return string.CompareOrdinal(a.Key, b.Key);
            });

            if (_priorityEntriesScratch.Count == 0)
            {
                return;
            }

            int n = _priorityEntriesScratch.Count;
            int mid = (n + 1) / 2;
            var sb = new StringBuilder();
            for (int i = 0; i < mid; i++)
            {
                if (i > 0)
                {
                    sb.Append("  ");
                }

                sb.Append(i + 1);
                sb.Append(". ");
                sb.Append(FormattingUtils.OreSubtypeAbbrev(_priorityEntriesScratch[i].Key));
            }

            dto.priorityLine1 = sb.ToString();
            sb.Clear();
            for (int i = mid; i < n; i++)
            {
                if (i > mid)
                {
                    sb.Append("  ");
                }

                sb.Append(i + 1);
                sb.Append(". ");
                sb.Append(FormattingUtils.OreSubtypeAbbrev(_priorityEntriesScratch[i].Key));
            }

            dto.priorityLine2 = sb.Length > 0 ? sb.ToString() : string.Empty;
        }
    }
}
