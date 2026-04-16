// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: inventory_component.cs
// Purpose: Virtual-dashboard inventory module (IDisplayComponent) with column-aware layout.
// PB Association: PB2
// Dependencies: LCDRenderer (partial), IDisplayComponent, FormattingUtils, MathUtils
// Key Methods: — (InventoryDisplayModule)

using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public partial class LCDRenderer
    {
        private sealed class InventoryDisplayModule : IDisplayComponent
        {
            private readonly LCDRenderer _h;

            public InventoryDisplayModule(LCDRenderer host)
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
                return MeasureInner(panelSize, bounds, filter, inventory, dynamicItems);
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
                DrawInner(frame, panelSize, viewport, filter, yTop, clipTop, clipBottom, inventory, dynamicItems);
            }

            private void BuildDisplayLists(string filterArg, InventoryDynamicDTO dynDto)
            {
                BuildInventoryDisplayListsFromDynamic(
                    filterArg,
                    dynDto,
                    _h._oreByNameScratch,
                    _h._ingotByNameScratch,
                    _h._compIndicesScratch,
                    _h._mergeKeysScratch);
            }

            private const float InvVirtualTextScale = 0.55f;

            /// <summary>Narrow <c>[COL]</c> viewports: stack ORES/INGOTS then COMPONENTS with STATUS-style min text scale.</summary>
            private static bool InvStackOreComponentsVertically(VRageMath.Vector2 panelSize, VRageMath.RectangleF bounds)
            {
                if (panelSize.X < 24f)
                    return false;
                return bounds.Width < panelSize.X * 0.72f;
            }

            private float MeasureInner(
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF bounds,
                string filterArg,
                InventorySummaryDTO sumDto,
                InventoryDynamicDTO dynDto)
            {
                if (sumDto == null || dynDto == null)
                    return panelSize.Y * 0.12f;

                float rowHeight = panelSize.Y * 0.03515625f;
                bool stacked = InvStackOreComponentsVertically(panelSize, bounds);
                float effectiveScale = stacked ? StatusEffectiveTextScale(InvVirtualTextScale, bounds.Width) : InvVirtualTextScale;
                float listRowH = stacked ? StatusLineHeight(panelSize.Y, effectiveScale, InvVirtualTextScale) : rowHeight;
                int maxCharsFirst = StatusMaxCharsForWidth(bounds.Width, effectiveScale);
                if (maxCharsFirst < 8) maxCharsFirst = 8;
                int maxCharsCont = StatusContinuationBudget(maxCharsFirst);

                float barBlock = panelSize.Y * 0.035f + panelSize.Y * 0.11f + panelSize.Y * 0.02f;
                float padBottom = panelSize.Y * 0.02f;
                bool invFiltered = !string.IsNullOrEmpty(filterArg);

                float hTop = invFiltered ? 0f : (barBlock + panelSize.Y * 0.015f);

                BuildDisplayLists(filterArg, dynDto);

                int nComp = _h._compIndicesScratch.Count;
                int mergeCount = _h._mergeKeysScratch.Count;

                if (stacked)
                {
                    int linesOi = 0;
                    for (int m = 0; m < mergeCount; m++)
                    {
                        string mat = _h._mergeKeysScratch[m];
                        float oreV = 0f;
                        float ingV = 0f;
                        _h._oreByNameScratch.TryGetValue(mat, out oreV);
                        _h._ingotByNameScratch.TryGetValue(mat, out ingV);

                        string textOi;
                        if (string.Equals(mat, "Ice", StrIX.C))
                        {
                            float iceTotal = oreV + ingV;
                            textOi = FormattingUtils.FormatLargeNumber(iceTotal) + " " + mat;
                        }
                        else
                        {
                            string oreS = FormattingUtils.FormatLargeNumber(oreV);
                            string ingS = FormattingUtils.FormatLargeNumber(ingV);
                            textOi = oreS + "/" + ingS + " " + mat;
                        }
                        linesOi += CountWrappedWords(textOi, maxCharsFirst, maxCharsCont);
                    }

                    int linesComp = 0;
                    for (int c = 0; c < nComp; c++)
                    {
                        int idx = _h._compIndicesScratch[c];
                        string name = dynDto.itemNames[idx] ?? "";
                        string amt = FormattingUtils.FormatLargeNumber(dynDto.itemAmounts[idx]);
                        string textLine = amt.PadLeft(6) + " " + name;
                        linesComp += CountWrappedWords(textLine, maxCharsFirst, maxCharsCont);
                    }

                    bool any = mergeCount > 0 || nComp > 0;
                    if (!any)
                        return hTop + listRowH + padBottom;

                    int hdrLines;
                    if (invFiltered)
                    {
                        if (mergeCount > 0 && nComp > 0) hdrLines = 2;
                        else hdrLines = 1;
                    }
                    else
                        hdrLines = 2;

                    return hTop + hdrLines * listRowH + (linesOi + linesComp) * listRowH + padBottom;
                }

                // Wide view: keep OI and COMPONENTS side-by-side, but wrap within each column.
                float vw = bounds.Width;
                float vx = bounds.X;
                float xOreIngotCol = vx + vw * 0.01953125f;
                float xComponentsCol = vx + vw * 0.52f;
                float leftColW = Math.Max(24f, xComponentsCol - xOreIngotCol - 2f);
                float rightColW = Math.Max(24f, (vx + vw) - xComponentsCol - 2f);
                float fullListW = Math.Max(40f, vw - vw * 0.04f);

                int maxCharsLeft = invFiltered && nComp == 0
                    ? StatusMaxCharsForWidth(fullListW, InvVirtualTextScale)
                    : StatusMaxCharsForWidth(leftColW, InvVirtualTextScale);
                int maxCharsRight = invFiltered && mergeCount == 0
                    ? StatusMaxCharsForWidth(fullListW, InvVirtualTextScale)
                    : StatusMaxCharsForWidth(rightColW, InvVirtualTextScale);
                if (maxCharsLeft < 8) maxCharsLeft = 8;
                if (maxCharsRight < 8) maxCharsRight = 8;
                int maxContLeft = StatusContinuationBudget(maxCharsLeft);
                int maxContRight = StatusContinuationBudget(maxCharsRight);

                int maxItems = Math.Max(mergeCount, nComp);
                if (maxItems == 0)
                    return hTop + rowHeight + padBottom;

                int headerLinesWide = invFiltered ? ((mergeCount > 0 || nComp > 0) ? 1 : 0) : 1;
                int rowLines = 0;
                for (int r = 0; r < maxItems; r++)
                {
                    int leftLines = 0;
                    int rightLines = 0;
                    if (r < mergeCount)
                    {
                        string mat = _h._mergeKeysScratch[r];
                        float oreV = 0f;
                        float ingV = 0f;
                        _h._oreByNameScratch.TryGetValue(mat, out oreV);
                        _h._ingotByNameScratch.TryGetValue(mat, out ingV);
                        string textOi;
                        if (string.Equals(mat, "Ice", StrIX.C))
                        {
                            float iceTotal = oreV + ingV;
                            textOi = FormattingUtils.FormatLargeNumber(iceTotal) + " " + mat;
                        }
                        else
                        {
                            string oreS = FormattingUtils.FormatLargeNumber(oreV);
                            string ingS = FormattingUtils.FormatLargeNumber(ingV);
                            textOi = oreS + "/" + ingS + " " + mat;
                        }
                        leftLines = CountWrappedWords(textOi, maxCharsLeft, maxContLeft);
                    }

                    if (r < nComp)
                    {
                        int idx = _h._compIndicesScratch[r];
                        string name = dynDto.itemNames[idx] ?? "";
                        string amt = FormattingUtils.FormatLargeNumber(dynDto.itemAmounts[idx]);
                        string textLine = amt.PadLeft(6) + " " + name;
                        rightLines = CountWrappedWords(textLine, maxCharsRight, maxContRight);
                    }

                    int used = Math.Max(1, Math.Max(leftLines, rightLines));
                    rowLines += used;
                }

                return hTop + headerLinesWide * rowHeight + rowLines * rowHeight + padBottom;
            }

            private void DrawInner(
                MySpriteDrawFrame frame,
                VRageMath.Vector2 panelSize,
                VRageMath.RectangleF viewport,
                string filterArg,
                float yTop,
                float headerHeight,
                float yCutoff,
                InventorySummaryDTO sumDto,
                InventoryDynamicDTO dynDto)
            {
                if (sumDto == null || dynDto == null)
                    return;
                if (dynDto.itemNames == null || dynDto.itemAmounts == null || dynDto.itemTypes == null)
                    return;

                float vw = viewport.Width;
                float vx = viewport.X;
                float totalH = MeasureInner(panelSize, viewport, filterArg, sumDto, dynDto);
                if (yTop + totalH <= headerHeight || yTop >= yCutoff)
                    return;

                bool stacked = InvStackOreComponentsVertically(panelSize, viewport);
                float rowHeight = panelSize.Y * 0.03515625f;
                float listRowH = rowHeight;
                float invScale = InvVirtualTextScale;
                if (stacked)
                {
                    invScale = StatusEffectiveTextScale(InvVirtualTextScale, vw);
                    listRowH = StatusLineHeight(panelSize.Y, invScale, InvVirtualTextScale);
                }

                bool invFiltered = !string.IsNullOrEmpty(filterArg);
                BuildDisplayLists(filterArg, dynDto);

                int mergeCount = _h._mergeKeysScratch.Count;
                int compCount = _h._compIndicesScratch.Count;
                float rowCutVirt = yCutoff + (panelSize.Y * 0.01f);

                if (stacked)
                {
                    float margin = Math.Max(2f, vw * 0.02f);
                    float xText = vx + margin;
                    int maxChars = StatusMaxCharsForWidth(vw, invScale);
                    if (maxChars < 8) maxChars = 8;
                    int maxCont = StatusContinuationBudget(maxChars);

                    float cursorY = yTop;
                    if (!invFiltered)
                    {
                        float ratio = sumDto.cargoMax > 0.0001f ? MathUtils.Clamp(sumDto.cargoUsed / sumDto.cargoMax, 0f, 1f) : 0f;
                        string pct = FormattingUtils.FormatPercent(sumDto.cargoPercent);
                        string used = FormattingUtils.FormatLargeNumber(sumDto.cargoUsed);
                        string max = FormattingUtils.FormatLargeNumber(sumDto.cargoMax);
                        var lb = new[] { "Cargo" };
                        var rt = new[] { ratio };
                        var vl = new[] { used + " / " + max + " L " + pct };
                        var iceFill = new VRageMath.Color(0, 0, 255, 200);
                        float colCx = vx + vw * 0.5f;
                        float usedBar = _h._dBarAt(
                            yTop,
                            new VRageMath.Vector2(vw, panelSize.Y),
                            new VRageMath.Vector2(colCx, 0f),
                            lb,
                            rt,
                            vl,
                            iceFill,
                            true);
                        cursorY = yTop + usedBar + panelSize.Y * 0.015f;
                        _h._aT("ORES/INGOTS", xText, cursorY, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        cursorY += listRowH;
                        for (int m = 0; m < mergeCount; m++)
                        {
                            string mat = _h._mergeKeysScratch[m];
                            float oreV = 0f;
                            float ingV = 0f;
                            _h._oreByNameScratch.TryGetValue(mat, out oreV);
                            _h._ingotByNameScratch.TryGetValue(mat, out ingV);

                            string textOi;
                            if (string.Equals(mat, "Ice", StrIX.C))
                            {
                                float iceTotal = oreV + ingV;
                                textOi = FormattingUtils.FormatLargeNumber(iceTotal) + " " + mat;
                            }
                            else
                            {
                                string oreS = FormattingUtils.FormatLargeNumber(oreV);
                                string ingS = FormattingUtils.FormatLargeNumber(ingV);
                                textOi = oreS + "/" + ingS + " " + mat;
                            }

                            if (cursorY + listRowH > headerHeight && cursorY < rowCutVirt)
                            {
                                int usedLinesOi = _h.DrawWrappedWords(textOi, xText, cursorY, listRowH, invScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxChars, maxCont, true);
                                cursorY += usedLinesOi * listRowH;
                            }
                            else
                            {
                                int usedLinesOi = CountWrappedWords(textOi, maxChars, maxCont);
                                cursorY += usedLinesOi * listRowH;
                            }
                        }
                        _h._aT("COMPONENTS", xText, cursorY, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        cursorY += listRowH;
                        for (int c = 0; c < compCount; c++)
                        {
                            int idx = _h._compIndicesScratch[c];
                            string name = dynDto.itemNames[idx] ?? "";
                            string amt = FormattingUtils.FormatLargeNumber(dynDto.itemAmounts[idx]);
                            string textLine = amt.PadLeft(6) + " " + name;

                            if (cursorY + listRowH > headerHeight && cursorY < rowCutVirt)
                            {
                                int usedLinesComp = _h.DrawWrappedWords(textLine, xText, cursorY, listRowH, invScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxChars, maxCont, true);
                                cursorY += usedLinesComp * listRowH;
                            }
                            else
                            {
                                int usedLinesComp = CountWrappedWords(textLine, maxChars, maxCont);
                                cursorY += usedLinesComp * listRowH;
                            }
                        }
                    }
                    else
                    {
                        float hdrY = yTop;
                        if (mergeCount > 0 && compCount > 0)
                        {
                            _h._aT("ORES/INGOTS", xText, hdrY, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                            _h._aT("COMPONENTS", xText, hdrY + listRowH, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                            cursorY = hdrY + listRowH * 2f;
                        }
                        else if (mergeCount > 0)
                        {
                            _h._aT("ORES/INGOTS", xText, hdrY, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                            cursorY = hdrY + listRowH;
                        }
                        else if (compCount > 0)
                        {
                            _h._aT("COMPONENTS", xText, hdrY, invScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                            cursorY = hdrY + listRowH;
                        }
                        else
                            cursorY = yTop;

                        for (int m = 0; m < mergeCount; m++)
                        {
                            string mat = _h._mergeKeysScratch[m];
                            float oreV = 0f;
                            float ingV = 0f;
                            _h._oreByNameScratch.TryGetValue(mat, out oreV);
                            _h._ingotByNameScratch.TryGetValue(mat, out ingV);

                            string textOi;
                            if (string.Equals(mat, "Ice", StrIX.C))
                            {
                                float iceTotal = oreV + ingV;
                                textOi = FormattingUtils.FormatLargeNumber(iceTotal) + " " + mat;
                            }
                            else
                            {
                                string oreS = FormattingUtils.FormatLargeNumber(oreV);
                                string ingS = FormattingUtils.FormatLargeNumber(ingV);
                                textOi = oreS + "/" + ingS + " " + mat;
                            }

                            if (cursorY + listRowH > headerHeight && cursorY < rowCutVirt)
                            {
                                int used = _h.DrawWrappedWords(textOi, xText, cursorY, listRowH, invScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxChars, maxCont, true);
                                cursorY += used * listRowH;
                            }
                            else
                            {
                                int used = CountWrappedWords(textOi, maxChars, maxCont);
                                cursorY += used * listRowH;
                            }
                        }
                        for (int c = 0; c < compCount; c++)
                        {
                            int idx = _h._compIndicesScratch[c];
                            string name = dynDto.itemNames[idx] ?? "";
                            string amt = FormattingUtils.FormatLargeNumber(dynDto.itemAmounts[idx]);
                            string textLine = amt.PadLeft(6) + " " + name;

                            if (cursorY + listRowH > headerHeight && cursorY < rowCutVirt)
                            {
                                int used = _h.DrawWrappedWords(textLine, xText, cursorY, listRowH, invScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxChars, maxCont, true);
                                cursorY += used * listRowH;
                            }
                            else
                            {
                                int used = CountWrappedWords(textLine, maxChars, maxCont);
                                cursorY += used * listRowH;
                            }
                        }
                    }

                    return;
                }

                float colOiY;

                if (!invFiltered)
                {
                    float ratio = sumDto.cargoMax > 0.0001f ? MathUtils.Clamp(sumDto.cargoUsed / sumDto.cargoMax, 0f, 1f) : 0f;
                    string pct = FormattingUtils.FormatPercent(sumDto.cargoPercent);
                    string used = FormattingUtils.FormatLargeNumber(sumDto.cargoUsed);
                    string max = FormattingUtils.FormatLargeNumber(sumDto.cargoMax);
                    var lb = new[] { "Cargo" };
                    var rt = new[] { ratio };
                    var vl = new[] { used + " / " + max + " L " + pct };
                    var iceFill = new VRageMath.Color(0, 0, 255, 200);
                    float colCx = vx + vw * 0.5f;
                    float usedBar = _h._dBarAt(
                        yTop,
                        new VRageMath.Vector2(vw, panelSize.Y),
                        new VRageMath.Vector2(colCx, 0f),
                        lb,
                        rt,
                        vl,
                        iceFill,
                        true);
                    float startY = yTop + usedBar + panelSize.Y * 0.015f;
                    float xOreIngot = vx + vw * 0.01953125f;
                    float xComponents = vx + vw * 0.52f;
                    _h._aT("ORES/INGOTS", xOreIngot, startY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                    _h._aT("COMPONENTS", xComponents, startY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                    colOiY = startY + rowHeight;
                }
                else
                {
                    float hdrY = yTop;
                    if (mergeCount > 0 && compCount > 0)
                    {
                        _h._aT("ORES/INGOTS", vx + vw * 0.01953125f, hdrY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        _h._aT("COMPONENTS", vx + vw * 0.52f, hdrY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        colOiY = hdrY + rowHeight;
                    }
                    else if (mergeCount > 0)
                    {
                        _h._aT("ORES/INGOTS", vx + vw * 0.01953125f, hdrY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        colOiY = hdrY + rowHeight;
                    }
                    else if (compCount > 0)
                    {
                        _h._aT("COMPONENTS", vx + vw * 0.01953125f, hdrY, InvVirtualTextScale, LCDRenderer._cGy, LCDRenderer._fW, TextAlignment.LEFT);
                        colOiY = hdrY + rowHeight;
                    }
                    else
                        colOiY = yTop;
                }

                float xOreIngotCol = vx + vw * 0.01953125f;
                float xComponentsCol = vx + vw * 0.52f;
                float leftColW = Math.Max(24f, xComponentsCol - xOreIngotCol - 2f);
                float rightColW = Math.Max(24f, (vx + vw) - xComponentsCol - 2f);
                float fullListW = Math.Max(40f, vw - vw * 0.04f);
                int maxCharsLeft = invFiltered && compCount == 0
                    ? StatusMaxCharsForWidth(fullListW, InvVirtualTextScale)
                    : StatusMaxCharsForWidth(leftColW, InvVirtualTextScale);
                int maxCharsRight = invFiltered && mergeCount == 0
                    ? StatusMaxCharsForWidth(fullListW, InvVirtualTextScale)
                    : StatusMaxCharsForWidth(rightColW, InvVirtualTextScale);
                if (maxCharsLeft < 8) maxCharsLeft = 8;
                if (maxCharsRight < 8) maxCharsRight = 8;
                int maxContLeft = StatusContinuationBudget(maxCharsLeft);
                int maxContRight = StatusContinuationBudget(maxCharsRight);

                float listTopY = colOiY;
                int maxItems = Math.Max(mergeCount, compCount);
                float yRow = listTopY;
                for (int r = 0; r < maxItems; r++)
                {
                    string leftText = null;
                    string rightText = null;
                    int leftLines = 0;
                    int rightLines = 0;

                    if (r < mergeCount)
                    {
                        string mat = _h._mergeKeysScratch[r];
                        float oreV = 0f;
                        float ingV = 0f;
                        _h._oreByNameScratch.TryGetValue(mat, out oreV);
                        _h._ingotByNameScratch.TryGetValue(mat, out ingV);
                        if (string.Equals(mat, "Ice", StrIX.C))
                        {
                            float iceTotal = oreV + ingV;
                            leftText = FormattingUtils.FormatLargeNumber(iceTotal) + " " + mat;
                        }
                        else
                        {
                            string oreS = FormattingUtils.FormatLargeNumber(oreV);
                            string ingS = FormattingUtils.FormatLargeNumber(ingV);
                            leftText = oreS + "/" + ingS + " " + mat;
                        }
                        leftLines = CountWrappedWords(leftText, maxCharsLeft, maxContLeft);
                    }

                    if (r < compCount)
                    {
                        int idx = _h._compIndicesScratch[r];
                        string name = dynDto.itemNames[idx] ?? "";
                        string amt = FormattingUtils.FormatLargeNumber(dynDto.itemAmounts[idx]);
                        rightText = amt.PadLeft(6) + " " + name;
                        rightLines = CountWrappedWords(rightText, maxCharsRight, maxContRight);
                    }

                    int usedLines = Math.Max(1, Math.Max(leftLines, rightLines));

                    if (yRow + rowHeight > headerHeight && yRow < rowCutVirt)
                    {
                        if (leftText != null)
                        {
                            float xLeft = invFiltered && compCount == 0 ? vx + vw * 0.01953125f : xOreIngotCol;
                            _h.DrawWrappedWords(leftText, xLeft, yRow, rowHeight, InvVirtualTextScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxCharsLeft, maxContLeft, true);
                        }
                        if (rightText != null)
                        {
                            float xRight = invFiltered && mergeCount == 0 ? vx + vw * 0.01953125f : xComponentsCol;
                            _h.DrawWrappedWords(rightText, xRight, yRow, rowHeight, InvVirtualTextScale, LCDRenderer._cW, LCDRenderer._fM, TextAlignment.LEFT, maxCharsRight, maxContRight, true);
                        }
                    }

                    yRow += usedLines * rowHeight;
                }
            }
        }
    }
}
