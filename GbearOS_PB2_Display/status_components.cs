// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: status_components.cs
// Purpose: Virtual-dashboard PWR / ICE / REF / WARN modules (IDisplayComponent, viewport-aware).
// PB Association: PB2
// Dependencies: LCDRenderer (partial), IDisplayComponent, FormattingUtils, MathUtils
// Key Methods: — (PowerDisplayModule, IceDisplayModule, RefineryDisplayModule, WarningDisplayModule)

using System;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public partial class LCDRenderer
    {
        private sealed class PowerDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;

            public PowerDisplayModule(LCDRenderer host)
            {
                _h = host;
            }

            private static int PowerFilterLineCount(string filter, PowerStatusDTO power)
            {
                if (power == null)
                    return 0;
                if (string.IsNullOrEmpty(filter))
                    return 3;
                int n = 0;
                string bat = "Batteries x" + power.batteryCount;
                string rea = "Reactors x" + power.reactorCount;
                string eng = "Engines x" + power.engineCount;
                if (bat.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                if (rea.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                if (eng.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                return n;
            }

            public float Measure(
                LCDRenderer host,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filter,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (power == null)
                    return panelSize.Y * 0.04f;
                int count = PowerFilterLineCount(filter, power);
                return panelSize.Y * 0.035f + count * (panelSize.Y * 0.11f) + panelSize.Y * 0.02f;
            }

            public void Draw(
                LCDRenderer host,
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filter,
                float yTop,
                float clipTop,
                float clipBottom,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (power == null)
                    return;
                int count = PowerFilterLineCount(filter, power);
                float h = panelSize.Y * 0.035f + count * (panelSize.Y * 0.11f) + panelSize.Y * 0.02f;
                if (yTop + h <= clipTop || yTop >= clipBottom)
                    return;
                if (count == 0)
                    return;

                float vw = viewport.Width;
                float vx = viewport.X;
                float centerX = vx + vw * 0.5f;
                var columnSize = new VRageMath.Vector2(vw, panelSize.Y);

                float safeBatOut = power.batteryMaxOutput > 1e-6f ? power.batteryMaxOutput : 1f;
                float rBat = MathUtils.Clamp(power.batteryOutput / safeBatOut, 0f, 1f);
                float safeReactOut = power.reactorMaxOutput > 1e-6f ? power.reactorMaxOutput : 1f;
                float rRe = MathUtils.Clamp(power.reactorOutput / safeReactOut, 0f, 1f);
                float safeEngOut = power.engineMaxOutput > 1e-6f ? power.engineMaxOutput : 1f;
                float rEn = MathUtils.Clamp(power.engineOutput / safeEngOut, 0f, 1f);

                string batLb = "Batteries x" + power.batteryCount;
                string reaLb = "Reactors x" + power.reactorCount;
                string engLb = "Engines x" + power.engineCount;

                var lb = new string[count];
                var rt = new float[count];
                var vl = new string[count];
                int idx = 0;
                if (string.IsNullOrEmpty(filter) || batLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = batLb;
                    rt[idx] = rBat;
                    vl[idx] = "OUT:" + power.batteryOutput.ToString("0.0") + " IN:" + power.batteryInput.ToString("0.0");
                    idx++;
                }
                if (string.IsNullOrEmpty(filter) || reaLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = reaLb;
                    rt[idx] = rRe;
                    vl[idx] = "OUT:" + power.reactorOutput.ToString("0.0");
                    idx++;
                }
                if (string.IsNullOrEmpty(filter) || engLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = engLb;
                    rt[idx] = rEn;
                    vl[idx] = "OUT:" + power.engineOutput.ToString("0.0");
                    idx++;
                }

                _h._dBarAt(yTop, columnSize, new VRageMath.Vector2(centerX, 0f), lb, rt, vl, new VRageMath.Color(255, 0, 0, 200), true);
            }
        }

        private sealed class IceDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;

            public IceDisplayModule(LCDRenderer host)
            {
                _h = host;
            }

            private static int IceFilterLineCount(string filter, IceStatusDTO ice)
            {
                if (ice == null)
                    return 0;
                if (string.IsNullOrEmpty(filter))
                    return 4;
                int n = 0;
                string tot = "Total";
                string gen = "Generators x" + ice.generatorCount;
                string irr = "Irrigation x" + ice.irrigationCount;
                string car = "Cargo";
                if (tot.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                if (gen.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                if (irr.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                if (car.IndexOf(filter, StrIX.C) >= 0)
                    n++;
                return n;
            }

            public float Measure(
                LCDRenderer host,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filter,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (ice == null)
                    return panelSize.Y * 0.04f;
                int count = IceFilterLineCount(filter, ice);
                return panelSize.Y * 0.035f + count * (panelSize.Y * 0.11f) + panelSize.Y * 0.02f;
            }

            public void Draw(
                LCDRenderer host,
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filter,
                float yTop,
                float clipTop,
                float clipBottom,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (ice == null)
                    return;
                int count = IceFilterLineCount(filter, ice);
                float h = panelSize.Y * 0.035f + count * (panelSize.Y * 0.11f) + panelSize.Y * 0.02f;
                if (yTop + h <= clipTop || yTop >= clipBottom)
                    return;
                if (count == 0)
                    return;

                float vw = viewport.Width;
                float vx = viewport.X;
                float centerX = vx + vw * 0.5f;
                var columnSize = new VRageMath.Vector2(vw, panelSize.Y);

                string totLb = "Total";
                string genLb = "Generators x" + ice.generatorCount;
                string irrLb = "Irrigation x" + ice.irrigationCount;
                string carLb = "Cargo";

                var lb = new string[count];
                var rt = new float[count];
                var vl = new string[count];
                int idx = 0;
                if (string.IsNullOrEmpty(filter) || totLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = totLb;
                    rt[idx] = ice.pctTotal;
                    vl[idx] = FormattingUtils.FormatLargeNumber(ice.totalIce);
                    idx++;
                }
                if (string.IsNullOrEmpty(filter) || genLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = genLb;
                    rt[idx] = ice.pctGen;
                    vl[idx] = FormattingUtils.FormatLargeNumber(ice.generatorIce);
                    idx++;
                }
                if (string.IsNullOrEmpty(filter) || irrLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = irrLb;
                    rt[idx] = ice.pctIrr;
                    vl[idx] = FormattingUtils.FormatLargeNumber(ice.irrigationIce);
                    idx++;
                }
                if (string.IsNullOrEmpty(filter) || carLb.IndexOf(filter, StrIX.C) >= 0)
                {
                    lb[idx] = carLb;
                    rt[idx] = ice.pctCargo;
                    vl[idx] = FormattingUtils.FormatLargeNumber(ice.cargoIce);
                    idx++;
                }

                _h._dBarAt(yTop, columnSize, new VRageMath.Vector2(centerX, 0f), lb, rt, vl, new VRageMath.Color(165, 220, 255, 200), true);
            }
        }

        private sealed class RefineryDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;

            public RefineryDisplayModule(LCDRenderer host)
            {
                _h = host;
            }

            public float Measure(
                LCDRenderer host,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filter,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (refineryStatus == null || refineryStatus.refineryNames == null)
                    return panelSize.Y * 0.04f;
                float rowH = panelSize.Y * 0.072f;
                if (string.IsNullOrEmpty(filter))
                {
                    int total = refineryStatus.refineryNames.Length;
                    int rows = total > 0 ? (total + 1) / 2 : 1;
                    return panelSize.Y * 0.180f + rows * rowH + panelSize.Y * 0.02f;
                }
                if (string.Equals(filter, "Priority", StrIX.C))
                    return panelSize.Y * 0.180f;
                int matchCount = 0;
                int n = refineryStatus.refineryNames.Length;
                for (int i = 0; i < n; i++)
                {
                    string nm = refineryStatus.refineryNames[i] ?? "";
                    if (nm.IndexOf(filter, StrIX.C) >= 0)
                        matchCount++;
                }
                int rowsName = matchCount > 0 ? (matchCount + 1) / 2 : 0;
                return panelSize.Y * 0.08f + rowsName * rowH + panelSize.Y * 0.02f;
            }

            public void Draw(
                LCDRenderer host,
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filter,
                float yTop,
                float clipTop,
                float clipBottom,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO dto,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (dto == null || dto.refineryNames == null)
                    return;

                float rowH = panelSize.Y * 0.072f;
                float mh;
                if (string.IsNullOrEmpty(filter))
                {
                    int totalNames = dto.refineryNames.Length;
                    int rows = totalNames > 0 ? (totalNames + 1) / 2 : 1;
                    mh = panelSize.Y * 0.180f + rows * rowH + panelSize.Y * 0.02f;
                }
                else if (string.Equals(filter, "Priority", StrIX.C))
                    mh = panelSize.Y * 0.180f;
                else
                {
                    int mc = 0;
                    for (int j = 0; j < dto.refineryNames.Length; j++)
                    {
                        if ((dto.refineryNames[j] ?? "").IndexOf(filter, StrIX.C) >= 0)
                            mc++;
                    }
                    int rowsName = mc > 0 ? (mc + 1) / 2 : 0;
                    mh = panelSize.Y * 0.08f + rowsName * rowH + panelSize.Y * 0.02f;
                }
                if (yTop + mh <= clipTop || yTop >= clipBottom)
                    return;

                float vw = viewport.Width;
                float vx = viewport.X;
                float centerX = vx + vw * 0.5f;
                float cellW = vw * 0.5f;

                const float refOreScale = 0.52f;
                const float refNameScale = 0.58f;
                float iconSz = panelSize.Y * 0.038f;

                if (string.Equals(filter, "Priority", StrIX.C))
                {
                    string prio1 = dto.priorityLine1;
                    string prio2 = dto.priorityLine2;
                    if (string.IsNullOrEmpty(prio1))
                    {
                        prio1 = "1. Fe  2. Co  3. Ni";
                        prio2 = null;
                    }
                    _h._aT(prio1, centerX, yTop + panelSize.Y * 0.025f, 0.72f, LCDRenderer._cW, LCDRenderer._fW, TextAlignment.CENTER);
                    if (!string.IsNullOrEmpty(prio2))
                        _h._aT(prio2, centerX, yTop + panelSize.Y * 0.075f, 0.72f, LCDRenderer._cW, LCDRenderer._fW, TextAlignment.CENTER);
                    return;
                }

                if (string.IsNullOrEmpty(filter))
                {
                    string prio1 = dto.priorityLine1;
                    string prio2 = dto.priorityLine2;
                    if (string.IsNullOrEmpty(prio1))
                    {
                        prio1 = "1. Fe  2. Co  3. Ni";
                        prio2 = null;
                    }
                    _h._aT(prio1, centerX, yTop + panelSize.Y * 0.025f, 0.72f, LCDRenderer._cW, LCDRenderer._fW, TextAlignment.CENTER);
                    if (!string.IsNullOrEmpty(prio2))
                        _h._aT(prio2, centerX, yTop + panelSize.Y * 0.075f, 0.72f, LCDRenderer._cW, LCDRenderer._fW, TextAlignment.CENTER);
                }

                float gridTop = string.IsNullOrEmpty(filter) ? yTop + panelSize.Y * 0.180f : yTop + panelSize.Y * 0.08f;
                int total = dto.refineryNames.Length;
                int shown = 0;
                for (int i = 0; i < total; i++)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        string refNm = dto.refineryNames[i] ?? "";
                        if (refNm.IndexOf(filter, StrIX.C) < 0)
                            continue;
                    }

                    int col = shown % 2;
                    int row = shown / 2;
                    shown++;
                    float bx = vx + col * cellW;
                    float iy = gridTop + row * rowH;
                    float ny = iy - panelSize.Y * 0.018f;
                    float ix = bx + cellW * 0.065f;

                    string refName = dto.refineryNames[i] ?? "Unknown Refinery";
                    bool isWorking = (dto.isWorking != null && i < dto.isWorking.Length) ? dto.isWorking[i] : false;
                    bool hasOre = (dto.hasOre != null && i < dto.hasOre.Length) ? dto.hasOre[i] : false;
                    string oreSubtype = (dto.currentOre != null && i < dto.currentOre.Length) ? dto.currentOre[i] : "";

                    VRageMath.Color statusColor = LCDRenderer._cGy;
                    if (isWorking) statusColor = LCDRenderer._cG;
                    else if (hasOre) statusColor = LCDRenderer._cR;

                    string oreAbbr = hasOre && !string.IsNullOrEmpty(oreSubtype)
                        ? FormattingUtils.OreSubtypeAbbrev(oreSubtype)
                        : "-";

                    _h._aT(oreAbbr, bx + cellW * 0.24f, ny, refOreScale, new VRageMath.Color(220, 220, 220, 255), LCDRenderer._fM, TextAlignment.CENTER);
                    _h._aT(refName, bx + cellW * 0.36f, ny, refNameScale, LCDRenderer._cW, LCDRenderer._fW, TextAlignment.LEFT);
                    _h._aS("Circle", ix, iy, iconSz, iconSz, statusColor);
                }
            }
        }

        private sealed class WarningDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;

            public WarningDisplayModule(LCDRenderer host)
            {
                _h = host;
            }

            public float Measure(
                LCDRenderer host,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filter,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                if (warnings == null || warnings.isNominal)
                    return panelSize.Y * 0.22f;

                int lines = 0;
                if (warnings.lowPower) lines++;
                if (warnings.cargoFull) lines++;
                if (warnings.lowIce) lines++;
                if (warnings.refineryStalled) lines++;
                if (warnings.assemblerStalled) lines++;
                if (warnings.noRefineries) lines++;
                if (lines == 0) lines = 1;
                return lines * (panelSize.Y * 0.065f) + panelSize.Y * 0.02f;
            }

            public void Draw(
                LCDRenderer host,
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filter,
                float yTop,
                float clipTop,
                float clipBottom,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                int lines = 0;
                if (warnings != null && !warnings.isNominal)
                {
                    if (warnings.lowPower) lines++;
                    if (warnings.cargoFull) lines++;
                    if (warnings.lowIce) lines++;
                    if (warnings.refineryStalled) lines++;
                    if (warnings.assemblerStalled) lines++;
                    if (warnings.noRefineries) lines++;
                    if (lines == 0) lines = 1;
                }
                float mh = warnings == null || warnings.isNominal
                    ? panelSize.Y * 0.22f
                    : lines * (panelSize.Y * 0.065f) + panelSize.Y * 0.02f;
                if (yTop + mh <= clipTop || yTop >= clipBottom)
                    return;

                if (warnings == null)
                    return;

                float vw = viewport.Width;
                float vx = viewport.X;
                float centerX = vx + vw * 0.5f;

                float textScale = Math.Min(1f, vw / 350f);

                if (warnings.isNominal)
                {
                    _h._aT("ALL SYSTEMS NOMINAL", centerX, yTop + panelSize.Y * 0.13f, 1.0f * textScale, LCDRenderer._cG, LCDRenderer._fW, TextAlignment.CENTER);
                    return;
                }

                _h._warnLinesScratch.Clear();
                if (warnings.lowPower) _h._warnLinesScratch.Add("LOW POWER");
                if (warnings.cargoFull) _h._warnLinesScratch.Add("CARGO FULL");
                if (warnings.lowIce) _h._warnLinesScratch.Add("LOW ICE");
                if (warnings.refineryStalled) _h._warnLinesScratch.Add("REFINERY STALLED");
                if (warnings.assemblerStalled) _h._warnLinesScratch.Add("ASSEMBLER STALLED");
                if (warnings.noRefineries) _h._warnLinesScratch.Add("NO REFINERIES");

                float y = yTop + panelSize.Y * 0.02f;
                float lineStep = panelSize.Y * 0.065f;
                for (int i = 0; i < _h._warnLinesScratch.Count; i++)
                {
                    string w = _h._warnLinesScratch[i];
                    _h._aT(w, centerX, y, 0.92f * textScale, LCDRenderer._cR, LCDRenderer._fW, TextAlignment.CENTER);
                    y += lineStep;
                }
            }
        }

        private sealed class StatusDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;
            /// <summary>Base sprite text scale; <see cref="LCDRenderer.MeasureStatusContent"/> / <see cref="LCDRenderer.DrawStatusContent"/> apply column-width scaling and wrap using <see cref="LayoutManager.CurrentBounds"/>.</summary>
            private const float TextScale = 0.52f;

            public StatusDisplayModule(LCDRenderer host)
            {
                _h = host;
            }

            public float Measure(
                LCDRenderer host,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filter,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                return _h.MeasureStatusContent(panelSize, bounds, filter ?? "", TextScale);
            }

            public void Draw(
                LCDRenderer host,
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filter,
                float yTop,
                float clipTop,
                float clipBottom,
                InventorySummaryDTO inventory,
                InventoryDynamicDTO dynamicItems,
                RefineryStatusDTO refineryStatus,
                IceStatusDTO ice,
                PowerStatusDTO power,
                WarningDTO warnings)
            {
                float mh = _h.MeasureStatusContent(panelSize, viewport, filter ?? "", TextScale);
                if (yTop + mh <= clipTop || yTop >= clipBottom)
                    return;
                _h.DrawStatusContent(panelSize, viewport, filter ?? "", yTop, clipTop, clipBottom, TextScale);
            }
        }
    }
}
