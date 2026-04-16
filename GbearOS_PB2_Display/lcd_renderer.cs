// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: lcd_renderer.cs
// Purpose: Discovers text panels, renders tagged single-module LCDs and the virtual [GbearOS] sprite dashboard from DTOs.
// PB Association: PB2
// Dependencies: WarningFormatter, InventorySummaryDTO, RefineryStatusDTO, IceStatusDTO, PowerStatusDTO, InventoryDynamicDTO, WarningDTO, FormattingUtils, MathUtils
// Key Methods: Init, RenderAll

using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public partial class LCDRenderer
    {
        private const string TagGbear = "[GbearOS]";

        private const string DefaultManualTag = "[Manual]";

        private const string _fW = "White";
        private const string _fM = "Monospace";
        private const string _tS = "SquareSimple";

        private static readonly VRageMath.Color _cW = VRageMath.Color.White;
        private static readonly VRageMath.Color _cR = new VRageMath.Color(255, 0, 0, 255);
        private static readonly VRageMath.Color _cG = new VRageMath.Color(0, 255, 0, 255);
        private static readonly VRageMath.Color _cGy = new VRageMath.Color(128, 128, 128, 255);
        private static readonly VRageMath.Color _backdropFill = new VRageMath.Color(0, 0, 0, 255);
        private static readonly VRageMath.Color _barTrackColor = new VRageMath.Color(38, 42, 48, 255);

        private enum CommandType
        {
            Unknown,
            Head,
            Inv,
            Ref,
            Pwr,
            Ice,
            Warn,
            Status,
            Col,
        }

        private struct DisplayCommand
        {
            public CommandType TypeEnum;
            public string Arg;
            public string UnknownKind;
        }

        private struct PanelEntry
        {
            public IMyTextPanel Panel;
            public List<DisplayCommand> Commands;
            public float ScrollPos;
            public float TargetScrollPos;
            public int ScrollPauseTicks;
            public bool NeedsRedraw;
            public float CalculatedHeaderHeight;
            public float CalculatedTotalScrollable;
        }

        private IMyGridTerminalSystem _gts;
        private IMyProgrammableBlock _me;
        private WarningFormatter _warningFormatter;

        private readonly List<IMyTextPanel> _tmpPanels = new List<IMyTextPanel>(64);
        private readonly List<PanelEntry> _panels = new List<PanelEntry>(64);
        private readonly List<PanelEntry> _oldPanelsScratch = new List<PanelEntry>(64);
        private readonly List<MySprite> _spriteBatch = new List<MySprite>(320);
        private readonly LayoutManager _measureLayout = new LayoutManager();
        private readonly LayoutManager _drawLayout = new LayoutManager();
        private readonly List<string> _warnLinesScratch = new List<string>(8);
        private readonly Dictionary<string, float> _oreByNameScratch = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, float> _ingotByNameScratch = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private readonly List<int> _compIndicesScratch = new List<int>(128);
        private readonly List<string> _mergeKeysScratch = new List<string>(128);
        private readonly List<string> _sysStatusScratch = new List<string>(16);
        private readonly List<string> _lastSysStatusSnapshot = new List<string>(16);
        private IGCParser _igcParser;
        private bool _dirtySysStatus;
        private int _refreshCountdown = 0;

        private float _clipTop = -1f;
        private float _clipBot = 9999f;

        private InventorySummaryDTO _lastInv;
        private RefineryStatusDTO _lastRef;
        private IceStatusDTO _lastIce;
        private PowerStatusDTO _lastPwr;
        private InventoryDynamicDTO _lastDyn;
        private WarningDTO _lastWarn;

        private bool _dirtyInv;
        private bool _dirtyRef;
        private bool _dirtyIce;
        private bool _dirtyPwr;
        private bool _dirtyDyn;
        private bool _dirtyWarn;

        private Dictionary<string, IDisplayComponent> _displayModuleByTag;

        /// <summary>Must run before <see cref="IMyTextSurface.DrawFrame"/> — TEXT_AND_IMAGE mode will not show script sprites.</summary>
        private static void EnsureScriptDrawSurface(IMyTextPanel p)
        {
            if (p == null) return;
            p.ContentType = ContentType.SCRIPT;
            p.Script = "";
            p.ScriptBackgroundColor = VRageMath.Color.Black;
        }

        /// <summary>Opaque full-surface backdrop so content is not lost to default/clear alpha.</summary>
        private static void AddFrameBackdrop(MySpriteDrawFrame frame, VRageMath.Vector2 size, VRageMath.Vector2 center)
        {
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = _tS,
                Position = center,
                Size = size,
                Color = _backdropFill,
                Alignment = TextAlignment.CENTER,
                RotationOrScale = 0f,
            });
        }

        private void _aT(string t, float x, float y, float s, VRageMath.Color c, string f, TextAlignment a)
        {
            if (_clipTop >= 0f && (y < _clipTop || y > _clipBot))
                return;
            _spriteBatch.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = t,
                Position = new VRageMath.Vector2(x, y),
                Color = c,
                FontId = f,
                Alignment = a,
                RotationOrScale = s,
            });
        }

        private void _aS(string d, float x, float y, float w, float h, VRageMath.Color c)
        {
            if (_clipTop >= 0f && (y - (h * 0.5f) < _clipTop || y + (h * 0.5f) > _clipBot))
                return;
            _spriteBatch.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = d,
                Position = new VRageMath.Vector2(x, y),
                Size = new VRageMath.Vector2(w, h),
                Color = c,
                Alignment = TextAlignment.CENTER,
                RotationOrScale = 0f,
            });
        }

        /// <summary>Full-width track sprite plus fill when ratio is greater than zero (zero fill still shows the empty track).</summary>
        private void _drawBarSprites(float centerX, float y, float bW, float bH, float minFillWidth, float ratio, VRageMath.Color fillColor)
        {
            _aS(_tS, centerX, y, bW, bH, _barTrackColor);
            float r = MathUtils.Clamp(ratio, 0f, 1f);
            if (r <= 1e-5f)
                return;
            float fw = MathUtils.Clamp(Math.Max(minFillWidth, r * bW), minFillWidth, bW);
            float cx = centerX - bW * 0.5f + fw * 0.5f;
            _aS(_tS, cx, y, fw, bH, fillColor);
        }

        private void _dBarScreen(MySpriteDrawFrame frame, string title, VRageMath.Vector2 size, VRageMath.Vector2 center,
            string[] labels, float[] ratios, string[] values, VRageMath.Color barFill, bool endPass)
        {
            if (endPass)
                _spriteBatch.Clear();
            _aT(title, center.X, size.Y * 0.045f, 0.55f, _cGy, _fW, TextAlignment.CENTER);
            float rowS = size.Y * 0.17f;
            float y0 = size.Y * 0.18f;
            float bH = size.Y * 0.055f;
            float ep = size.X * 0.02f;
            float bW = size.X - 2f * ep;
            float mBW = bH * 0.35f;
            int n = labels.Length;
            for (int i = 0; i < n; i++)
            {
                float y = y0 + i * rowS;
                _drawBarSprites(center.X, y, bW, bH, mBW, ratios[i], barFill);
                _aT(labels[i] + " " + values[i], center.X, y + bH * 0.55f, 0.62f, new VRageMath.Color(230, 230, 230, 255), _fW, TextAlignment.CENTER);
            }
            if (endPass)
                FlushSprites(frame);
        }

        private float _dBarAt(float yTop, VRageMath.Vector2 size, VRageMath.Vector2 center,
            string[] labels, float[] ratios, string[] values, VRageMath.Color barFill, bool draw)
        {
            float rowS = size.Y * 0.11f;
            float bH = size.Y * 0.045f;
            float ep = size.X * 0.02f;
            float bW = size.X - 2f * ep;
            float mBW = bH * 0.35f;
            int n = labels.Length;
            float y0 = yTop + size.Y * 0.035f;
            for (int i = 0; i < n; i++)
            {
                float y = y0 + i * rowS;
                if (draw)
                    _drawBarSprites(center.X, y, bW, bH, mBW, ratios[i], barFill);
                if (draw)
                    _aT(labels[i] + " " + values[i], center.X, y + bH * 0.55f, 0.55f, new VRageMath.Color(230, 230, 230, 255), _fW, TextAlignment.CENTER);
            }
            return size.Y * 0.035f + n * rowS + size.Y * 0.02f;
        }

        public void Init(IMyGridTerminalSystem gts, IMyProgrammableBlock me, WarningFormatter warningFormatter, IGCParser igcParser)
        {
            _gts = gts;
            _me = me;
            _warningFormatter = warningFormatter;
            _igcParser = igcParser;
            _refreshCountdown = 0;

            _displayModuleByTag = new Dictionary<string, IDisplayComponent>(StringComparer.OrdinalIgnoreCase);
            _displayModuleByTag["INV"] = new InventoryDisplayModule(this);
            _displayModuleByTag["PWR"] = new PowerDisplayModule(this);
            _displayModuleByTag["ICE"] = new IceDisplayModule(this);
            _displayModuleByTag["REF"] = new RefineryDisplayModule(this);
            _displayModuleByTag["WARN"] = new WarningDisplayModule(this);
            _displayModuleByTag["STATUS"] = new StatusDisplayModule(this);
        }

        private IDisplayComponent TryGetVirtualModule(string commandType)
        {
            if (_displayModuleByTag == null || commandType == null)
                return null;
            IDisplayComponent comp;
            return _displayModuleByTag.TryGetValue(commandType, out comp) ? comp : null;
        }

        private static CommandType ParseCommandTypeString(string typeToken)
        {
            if (string.IsNullOrEmpty(typeToken))
                return CommandType.Unknown;
            if (string.Equals(typeToken, "HEAD", StrIX.C))
                return CommandType.Head;
            if (string.Equals(typeToken, "INV", StrIX.C))
                return CommandType.Inv;
            if (string.Equals(typeToken, "REF", StrIX.C))
                return CommandType.Ref;
            if (string.Equals(typeToken, "PWR", StrIX.C))
                return CommandType.Pwr;
            if (string.Equals(typeToken, "ICE", StrIX.C))
                return CommandType.Ice;
            if (string.Equals(typeToken, "WARN", StrIX.C))
                return CommandType.Warn;
            if (string.Equals(typeToken, "STATUS", StrIX.C))
                return CommandType.Status;
            if (string.Equals(typeToken, LayoutManager.CmdCol, StrIX.C))
                return CommandType.Col;
            return CommandType.Unknown;
        }

        private static string ModuleKeyForCommand(ref DisplayCommand cmd)
        {
            if (cmd.TypeEnum == CommandType.Unknown)
                return cmd.UnknownKind;
            switch (cmd.TypeEnum)
            {
                case CommandType.Inv:
                    return "INV";
                case CommandType.Ref:
                    return "REF";
                case CommandType.Pwr:
                    return "PWR";
                case CommandType.Ice:
                    return "ICE";
                case CommandType.Warn:
                    return "WARN";
                case CommandType.Status:
                    return "STATUS";
                default:
                    return null;
            }
        }

        private static string SubheaderLabelForCommand(CommandType t, string unknownKind)
        {
            switch (t)
            {
                case CommandType.Inv:
                    return "INVENTORY";
                case CommandType.Ref:
                    return "REFINERY STATUS";
                case CommandType.Ice:
                    return "ICE STATUS";
                case CommandType.Pwr:
                    return "POWER GRID STATUS";
                case CommandType.Warn:
                    return "WARNING STATUS";
                case CommandType.Status:
                    return "SYSTEM STATUS";
                case CommandType.Unknown:
                    return unknownKind != null ? unknownKind : "";
                default:
                    return "";
            }
        }

        public void RenderNoSignal(double secondsOffline)
        {
            if (_gts == null || _me == null)
                return;

            RefreshPanelsIfNeeded();

            string offlineLine = "Offline for: " + secondsOffline.ToString("F0") + "s";

            int count = _panels.Count;
            for (int i = 0; i < count; i++)
            {
                IMyTextPanel panel = _panels[i].Panel;
                if (panel == null)
                    continue;

                EnsureScriptDrawSurface(panel);

                VRageMath.Vector2 size;
                VRageMath.Vector2 center;
                GetSurfaceLayout(panel, out size, out center);

                using (var frame = panel.DrawFrame())
                {
                    AddFrameBackdrop(frame, size, center);
                    _spriteBatch.Clear();
                    _aT("NO SIGNAL", center.X, size.Y * 0.10f, 1.35f, _cR, _fW, TextAlignment.CENTER);
                    _aT("WAITING FOR TELEMETRY...", center.X, size.Y * 0.20f, 0.72f, _cW, _fW, TextAlignment.CENTER);
                    _aT(offlineLine, center.X, size.Y * 0.28f, 0.62f, _cGy, _fW, TextAlignment.CENTER);
                    FlushSprites(frame);
                }
            }
        }

        public void RenderAll(
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings,
            bool isFullTick)
        {
            if (_gts == null || _me == null)
                return;

            RefreshPanelsIfNeeded();

            if (isFullTick)
            {
                _dirtyInv = DtoChanged(_lastInv, inventory);
                _dirtyRef = DtoChanged(_lastRef, refineryStatus);
                _dirtyIce = DtoChanged(_lastIce, ice);
                _dirtyPwr = DtoChanged(_lastPwr, power);
                _dirtyDyn = DtoChanged(_lastDyn, dynamicItems);
                _dirtyWarn = DtoChanged(_lastWarn, warnings);
                _dirtySysStatus = SysStatusSnapshotChanged();

                _lastInv = inventory;
                _lastRef = refineryStatus;
                _lastIce = ice;
                _lastPwr = power;
                _lastDyn = dynamicItems;
                _lastWarn = warnings;
            }

            UpdateVirtualScrollsAndSyncFields(inventory, refineryStatus, ice, power, dynamicItems, warnings);
            TryRenderVirtualPanels(inventory, refineryStatus, ice, power, dynamicItems, warnings);
        }

        private static bool DtoChanged<T>(T a, T b)
        {
            if (a == null && b == null) return false;
            if (a == null || b == null) return true;
            return !a.Equals(b);
        }

        private bool CommandsDirty(List<DisplayCommand> cmds)
        {
            if (cmds == null || cmds.Count == 0)
                return false;

            bool anyDirty = _dirtyInv || _dirtyDyn || _dirtyRef || _dirtyIce || _dirtyPwr || _dirtyWarn || _dirtySysStatus;
            bool invDirty = _dirtyInv || _dirtyDyn;

            int n = cmds.Count;
            for (int i = 0; i < n; i++)
            {
                switch (cmds[i].TypeEnum)
                {
                    case CommandType.Inv:
                        if (invDirty) return true;
                        break;
                    case CommandType.Ref:
                        if (_dirtyRef) return true;
                        break;
                    case CommandType.Ice:
                        if (_dirtyIce) return true;
                        break;
                    case CommandType.Pwr:
                        if (_dirtyPwr) return true;
                        break;
                    case CommandType.Warn:
                        if (_dirtyWarn) return true;
                        break;
                    case CommandType.Status:
                        if (_dirtySysStatus) return true;
                        break;
                    case CommandType.Unknown:
                        if (anyDirty) return true;
                        break;
                }
            }
            return false;
        }

        private void FlushSprites(MySpriteDrawFrame frame)
        {
            int n = _spriteBatch.Count;
            for (int si = 0; si < n; si++)
                frame.Add(_spriteBatch[si]);
            _spriteBatch.Clear();
        }

        private void TryRenderVirtualPanels(
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings)
        {
            int count = _panels.Count;
            for (int i = 0; i < count; i++)
            {
                var e = _panels[i];
                if (e.Commands == null || e.Commands.Count == 0)
                    continue;
                if (!CommandsDirty(e.Commands) && !e.NeedsRedraw)
                    continue;
                RenderToPanelVirtual(ref e, inventory, refineryStatus, ice, power, dynamicItems, warnings);
                e.NeedsRedraw = false;
                _panels[i] = e;
            }
        }

        private void UpdateVirtualScrollsAndSyncFields(
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings)
        {
            int count = _panels.Count;
            for (int i = 0; i < count; i++)
            {
                var e = _panels[i];
                if (e.Commands == null)
                    continue;

                VRageMath.Vector2 sz, ct;
                GetSurfaceLayout(e.Panel, out sz, out ct);
                float yCut = sz.Y * 0.95703125f;
                float headH, totalContent;
                MeasureVirtualContent(
                    _measureLayout,
                    e.Commands,
                    sz,
                    inventory,
                    refineryStatus,
                    ice,
                    power,
                    dynamicItems,
                    warnings,
                    out headH,
                    out totalContent);
                e.CalculatedHeaderHeight = headH;
                e.CalculatedTotalScrollable = totalContent;
                float viewportH = yCut - headH;

                if (totalContent > viewportH)
                {
                    float maxScroll = totalContent - viewportH;
                    float scrollStep = viewportH * 0.90f;

                    if (e.TargetScrollPos > e.ScrollPos)
                    {
                        float step = scrollStep / 12f;
                        e.ScrollPos += step;
                        if (e.ScrollPos >= e.TargetScrollPos)
                            e.ScrollPos = e.TargetScrollPos;
                        e.NeedsRedraw = true;
                    }
                    else if (e.TargetScrollPos < e.ScrollPos)
                    {
                        float dist = e.ScrollPos - e.TargetScrollPos;
                        float rewindStep = dist * 0.15f;
                        if (rewindStep < 20f)
                            rewindStep = 20f;
                        e.ScrollPos -= rewindStep;
                        if (e.ScrollPos <= e.TargetScrollPos)
                            e.ScrollPos = e.TargetScrollPos;
                        e.NeedsRedraw = true;
                    }
                    else
                    {
                        e.ScrollPauseTicks++;
                        if (e.ScrollPauseTicks >= 30)
                        {
                            e.ScrollPauseTicks = 0;
                            if (e.ScrollPos >= maxScroll - 5f)
                            {
                                e.TargetScrollPos = 0f;
                            }
                            else
                            {
                                e.TargetScrollPos = e.ScrollPos + scrollStep;
                                if (e.TargetScrollPos > maxScroll)
                                    e.TargetScrollPos = maxScroll;
                            }
                            e.NeedsRedraw = true;
                        }
                    }
                }
                else
                {
                    e.ScrollPos = 0f;
                    e.TargetScrollPos = 0f;
                    e.ScrollPauseTicks = 0;
                }

                _panels[i] = e;
            }
        }

        private void RefreshPanelsIfNeeded()
        {
            if (_refreshCountdown > 0)
            {
                _refreshCountdown--;
                return;
            }
            _refreshCountdown = 100;

            _tmpPanels.Clear();
            _gts.GetBlocksOfType(_tmpPanels, PanelFilter);

            _oldPanelsScratch.Clear();
            for (int pi = 0; pi < _panels.Count; pi++)
                _oldPanelsScratch.Add(_panels[pi]);

            _panels.Clear();

            int n = _tmpPanels.Count;
            for (int i = 0; i < n; i++)
            {
                var p = _tmpPanels[i];
                if (p == null)
                    continue;

                string name = p.CustomName;
                if (BlockUtils.HasTag(name, DefaultManualTag))
                    continue;

                PanelEntry e;
                e.Panel = p;
                e.ScrollPos = 0f;
                e.TargetScrollPos = 0f;
                e.ScrollPauseTicks = 0;
                e.NeedsRedraw = false;
                e.CalculatedHeaderHeight = 0f;
                e.CalculatedTotalScrollable = 0f;
                for (int oldIdx = 0; oldIdx < _oldPanelsScratch.Count; oldIdx++)
                {
                    if (_oldPanelsScratch[oldIdx].Panel == p)
                    {
                        e.ScrollPos = _oldPanelsScratch[oldIdx].ScrollPos;
                        e.TargetScrollPos = _oldPanelsScratch[oldIdx].TargetScrollPos;
                        e.ScrollPauseTicks = _oldPanelsScratch[oldIdx].ScrollPauseTicks;
                        break;
                    }
                }

                if (!BlockUtils.HasTag(name, TagGbear))
                    continue;

                var cmds = new List<DisplayCommand>(8);
                ParseCustomDataCommands(p.CustomData, cmds);
                if (cmds.Count == 0)
                    continue;
                e.Commands = cmds;

                _panels.Add(e);
            }
        }

        private void ParseCustomDataCommands(string customData, List<DisplayCommand> outList)
        {
            outList.Clear();
            bool emptyCd = string.IsNullOrWhiteSpace(customData);
            if (emptyCd)
            {
                outList.Add(new DisplayCommand { TypeEnum = CommandType.Inv, Arg = "", UnknownKind = null });
                return;
            }

            int i0 = 0;
            int len = customData.Length;
            while (i0 < len)
            {
                int nl = customData.IndexOf('\n', i0);
                string line = nl < 0 ? customData.Substring(i0) : customData.Substring(i0, nl - i0);
                i0 = nl < 0 ? len : nl + 1;

                int lb = line.IndexOf('[');
                int rb = line.IndexOf(']');
                if (lb < 0 || rb <= lb)
                    continue;

                string inner = line.Substring(lb + 1, rb - lb - 1).Trim();
                if (inner.Length == 0)
                    continue;

                DisplayCommand dc;
                int c = inner.IndexOf(':');
                string typeTok;
                if (c < 0)
                {
                    typeTok = inner.Trim();
                    dc.Arg = "";
                }
                else
                {
                    typeTok = inner.Substring(0, c).Trim();
                    dc.Arg = inner.Substring(c + 1).Trim();
                }

                if (typeTok.Length == 0)
                    continue;

                dc.TypeEnum = ParseCommandTypeString(typeTok);
                if (dc.TypeEnum == CommandType.Unknown)
                    dc.UnknownKind = typeTok;
                else
                    dc.UnknownKind = null;
                outList.Add(dc);
            }
        }

        private bool PanelFilter(IMyTextPanel p)
        {
            if (p == null)
                return false;
            if (!p.IsSameConstructAs(_me))
                return false;
            return true;
        }

        private static void GetSurfaceLayout(IMyTextPanel panel, out VRageMath.Vector2 size, out VRageMath.Vector2 center)
        {
            var surf = panel as IMyTextSurface;
            VRageMath.Vector2 tex = surf != null ? surf.TextureSize : default(VRageMath.Vector2);
            VRageMath.Vector2 vis = surf != null ? surf.SurfaceSize : default(VRageMath.Vector2);
            size = (tex.X >= 8f && tex.Y >= 8f) ? tex
                : ((vis.X >= 8f && vis.Y >= 8f) ? vis : new VRageMath.Vector2(512f, 512f));
            center = size * 0.5f;
        }

        private float SubheaderHeight(VRageMath.Vector2 size)
        {
            return size.Y * 0.045f;
        }

        private float DrawSubheader(float yScreen, VRageMath.Vector2 size, float centerX, string label, bool draw)
        {
            float h = SubheaderHeight(size);
            if (draw)
                _aT("--- " + label + " ---", centerX, yScreen, 0.55f, _cGy, _fW, TextAlignment.CENTER);
            return h;
        }

        private void MeasureVirtualContent(
            LayoutManager layout,
            List<DisplayCommand> cmds,
            VRageMath.Vector2 size,
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings,
            out float headerHeight,
            out float totalScrollableHeight)
        {
            headerHeight = size.Y * 0.02f;
            layout.Reset(size.X, size.Y);
            int nc = cmds.Count;
            for (int i = 0; i < nc; i++)
            {
                var c = cmds[i];
                switch (c.TypeEnum)
                {
                    case CommandType.Head:
                        headerHeight += size.Y * 0.07f;
                        continue;
                    case CommandType.Col:
                        layout.ApplyCol(c.Arg);
                        continue;
                }

                bool isFilt = (c.TypeEnum == CommandType.Inv || c.TypeEnum == CommandType.Status)
                    && !string.IsNullOrEmpty(c.Arg);
                float subH = isFilt ? 0f : SubheaderHeight(size);
                float modH = ModuleHeight(c, layout, size, inventory, refineryStatus, ice, power, dynamicItems, warnings);
                layout.RegisterContentHeight(subH + modH);
            }
            layout.FinalizeScrollExtent();
            totalScrollableHeight = layout.PeakScrollY;
        }

        private float ModuleHeight(
            DisplayCommand cmd,
            LayoutManager layout,
            VRageMath.Vector2 size,
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings)
        {
            if (cmd.TypeEnum == CommandType.Col)
                return 0f;
            string modKey = ModuleKeyForCommand(ref cmd);
            IDisplayComponent comp = TryGetVirtualModule(modKey);
            if (comp != null)
                return comp.Measure(this, size, layout.CurrentBounds, cmd.Arg, inventory, dynamicItems, refineryStatus, ice, power, warnings);
            return size.Y * 0.04f;
        }

        private void RenderToPanelVirtual(
            ref PanelEntry entry,
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings)
        {
            IMyTextPanel panel = entry.Panel;
            if (panel == null)
                return;

            EnsureScriptDrawSurface(panel);

            VRageMath.Vector2 size;
            VRageMath.Vector2 center;
            GetSurfaceLayout(panel, out size, out center);
            float yCutoff = size.Y * 0.95703125f;

            float headerHeight = entry.CalculatedHeaderHeight;
            float totalScrollable = entry.CalculatedTotalScrollable;
            float viewportHeight = yCutoff - headerHeight;
            float viewportPad = size.Y * 0.02f;
            float drawStart = headerHeight + viewportPad - entry.ScrollPos;

            using (var frame = panel.DrawFrame())
            {
                AddFrameBackdrop(frame, size, center);
                _spriteBatch.Clear();

                float hy = size.Y * 0.025f;
                int nc = entry.Commands.Count;
                for (int i = 0; i < nc; i++)
                {
                    var c = entry.Commands[i];
                    if (c.TypeEnum != CommandType.Head)
                        continue;
                    string ht = string.IsNullOrEmpty(c.Arg) ? " " : c.Arg;
                    _aT(ht, center.X, hy, 0.88f, _cW, _fW, TextAlignment.CENTER);
                    hy += size.Y * 0.07f;
                }

                if (totalScrollable > viewportHeight)
                {
                    float maxS = totalScrollable - viewportHeight;
                    float stepS = viewportHeight * 0.90f;
                    int totalPages = (int)Math.Ceiling(maxS / stepS) + 1;
                    int curPage;
                    if (entry.ScrollPos >= maxS - 5f)
                        curPage = totalPages;
                    else
                        curPage = (int)(entry.ScrollPos / stepS) + 1;
                    _aT("PAGE " + curPage + "/" + totalPages, size.X * 0.97f, size.Y * 0.025f, 0.5f, new VRageMath.Color(180, 180, 180, 255), _fW, TextAlignment.RIGHT);
                }

                _clipTop = headerHeight + viewportPad;
                _clipBot = yCutoff;

                _drawLayout.Reset(size.X, size.Y);
                for (int i = 0; i < nc; i++)
                {
                    var c = entry.Commands[i];
                    switch (c.TypeEnum)
                    {
                        case CommandType.Head:
                            continue;
                        case CommandType.Col:
                            _drawLayout.ApplyCol(c.Arg);
                            continue;
                    }

                    bool isFilt = (c.TypeEnum == CommandType.Inv || c.TypeEnum == CommandType.Status)
                        && !string.IsNullOrEmpty(c.Arg);
                    float subH = isFilt ? 0f : SubheaderHeight(size);
                    float modH = ModuleHeight(c, _drawLayout, size, inventory, refineryStatus, ice, power, dynamicItems, warnings);
                    float blockTop = drawStart + _drawLayout.NextContentY;
                    float blockBot = blockTop + subH + modH;

                    bool fullyOut = blockBot <= headerHeight || blockTop >= yCutoff;
                    if (!fullyOut)
                    {
                        if (!isFilt)
                            DrawSubheader(blockTop, size, _drawLayout.ActiveCenterX, SubheaderLabelForCommand(c.TypeEnum, c.UnknownKind), true);
                        float y0 = blockTop + subH;
                        DrawCommandModule(c, _drawLayout, frame, inventory, refineryStatus, ice, power, dynamicItems, warnings, size, y0, headerHeight, yCutoff);
                    }

                    _drawLayout.RegisterContentHeight(subH + modH);
                }
                _drawLayout.FinalizeScrollExtent();

                _clipTop = -1f;

                FlushSprites(frame);
            }
        }

        private void DrawCommandModule(
            DisplayCommand cmd,
            LayoutManager layout,
            MySpriteDrawFrame frame,
            InventorySummaryDTO inventory,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            InventoryDynamicDTO dynamicItems,
            WarningDTO warnings,
            VRageMath.Vector2 size,
            float yTop,
            float headerHeight,
            float yCutoff)
        {
            string modKey = ModuleKeyForCommand(ref cmd);
            IDisplayComponent comp = TryGetVirtualModule(modKey);
            if (comp != null)
            {
                comp.Draw(this, frame, size, layout.CurrentBounds, cmd.Arg, yTop, headerHeight, yCutoff, inventory, dynamicItems, refineryStatus, ice, power, warnings);
            }
        }

        private bool SysStatusSnapshotChanged()
        {
            if (_igcParser == null)
                return false;
            _igcParser.GetSystemStatuses(_sysStatusScratch);
            bool diff = _sysStatusScratch.Count != _lastSysStatusSnapshot.Count;
            if (!diff)
            {
                for (int i = 0; i < _sysStatusScratch.Count; i++)
                {
                    string a = _sysStatusScratch[i] ?? "";
                    string b = i < _lastSysStatusSnapshot.Count ? (_lastSysStatusSnapshot[i] ?? "") : "";
                    if (!string.Equals(a, b, StrIX.C))
                    {
                        diff = true;
                        break;
                    }
                }
            }
            if (!diff)
                return false;
            _lastSysStatusSnapshot.Clear();
            for (int i = 0; i < _sysStatusScratch.Count; i++)
                _lastSysStatusSnapshot.Add(_sysStatusScratch[i] ?? "");
            return true;
        }

        private const float StatusTextScaleFloor = 0.45f;
        private const int StatusHangingIndentChars = 2;
        private const string StatusContinuationIndent = "  ";

        /// <summary>Approximate monospace chars per line from column width and effective sprite text scale (after <see cref="StatusEffectiveTextScale"/> floor).</summary>
        internal static int StatusMaxCharsForWidth(float viewWidth, float textScale)
        {
            // Conservative: shrink usable width so we wrap well before physical bezel clipping.
            // NOTE: STATUS lines are particularly sensitive to right-side bezel crop on some LCD models.
            float usable = viewWidth * 0.80f; // virtual 20% safety margin (extreme)
            if (usable < 8f)
                usable = Math.Max(1f, viewWidth * 0.5f);

            // Assume characters are slightly wider than the nominal monospace estimate to trigger earlier wraps.
            // (Make wrapping more aggressive instead of flirting with the bezel.)
            float unit = 19.5f * textScale; // aggressive monospace width estimate
            if (unit <= 0.0001f)
                return 4;
            int n = (int)(usable / unit);
            return n < 1 ? 1 : n;
        }

        /// <summary>Narrow columns scale down, but never below <see cref="StatusTextScaleFloor"/> (legibility).</summary>
        internal static float StatusEffectiveTextScale(float baseScale, float columnWidth)
        {
            float w = columnWidth > 2f ? columnWidth : 400f;
            float refW = 520f;
            float scaled = baseScale * Math.Min(1f, w / refW);
            if (scaled < StatusTextScaleFloor)
                scaled = StatusTextScaleFloor;
            if (scaled > baseScale)
                scaled = baseScale;
            return scaled;
        }

        internal static float StatusLineHeight(float panelHeightY, float effectiveScale, float baseScale)
        {
            float ratio = baseScale > 1e-4f ? effectiveScale / baseScale : 1f;
            ratio = Math.Max(0.88f, ratio);
            return panelHeightY * (0.028f + 0.012f * ratio);
        }

        internal static int StatusContinuationBudget(int maxCharsFirst)
        {
            int n = maxCharsFirst - StatusHangingIndentChars;
            return n < 4 ? Math.Max(1, maxCharsFirst - 1) : n;
        }

        internal static int CountWrappedWords(string text, int maxCharsFirst, int maxCharsCont)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            int i = 0;
            int lines = 0;
            bool firstLine = true;

            while (i < text.Length)
            {
                // Skip leading spaces.
                while (i < text.Length && text[i] == ' ')
                    i++;
                if (i >= text.Length)
                    break;

                int budget = firstLine ? maxCharsFirst : maxCharsCont;
                firstLine = false;

                int lineLen = 0;
                while (i < text.Length)
                {
                    while (i < text.Length && text[i] == ' ')
                        i++;
                    if (i >= text.Length)
                        break;

                    int wordStart = i;
                    while (i < text.Length && text[i] != ' ')
                        i++;
                    int wordLen = i - wordStart;
                    if (wordLen <= 0)
                        continue;

                    int extra = lineLen == 0 ? wordLen : (1 + wordLen);
                    if (lineLen + extra <= budget)
                    {
                        lineLen += extra;
                        continue;
                    }

                    // Word doesn't fit.
                    if (lineLen == 0)
                    {
                        // Extremely long single token: hard-break so we make progress.
                        int step = budget < 1 ? 1 : budget;
                        int wordEnd = i;
                        int pos = wordStart;
                        while (pos < wordEnd)
                        {
                            int take = Math.Min(step, wordEnd - pos);
                            pos += take;
                            lines++;
                            firstLine = false;
                        }
                    }
                    else
                    {
                        // Start a new line with this word (do not consume it here).
                        i = wordStart;
                        lines++;
                        firstLine = false;
                    }
                    goto NextLine;
                }

                lines++;
                firstLine = false;
            NextLine:
                ;
            }

            return lines;
        }

        internal int DrawWrappedWords(
            string text,
            float xLeft,
            float yStart,
            float lineH,
            float scale,
            VRageMath.Color color,
            string font,
            TextAlignment align,
            int maxCharsFirst,
            int maxCharsCont,
            bool indentContinuations)
        {
            if (string.IsNullOrEmpty(text))
            {
                _aT(" ", xLeft, yStart, scale, color, font, align);
                return 1;
            }

            int i = 0;
            int lines = 0;
            bool firstLine = true;
            float y = yStart;

            while (i < text.Length)
            {
                while (i < text.Length && text[i] == ' ')
                    i++;
                if (i >= text.Length)
                    break;

                int budget = firstLine ? maxCharsFirst : maxCharsCont;

                int lineStart = i;
                int lineEnd = i;
                int lineLen = 0;

                while (i < text.Length)
                {
                    while (i < text.Length && text[i] == ' ')
                        i++;
                    if (i >= text.Length)
                        break;

                    int wordStart = i;
                    while (i < text.Length && text[i] != ' ')
                        i++;
                    int wordEnd = i;
                    int wordLen = wordEnd - wordStart;
                    if (wordLen <= 0)
                        continue;

                    int extra = lineLen == 0 ? wordLen : (1 + wordLen);
                    if (lineLen + extra <= budget)
                    {
                        lineLen += extra;
                        lineEnd = wordEnd;
                        continue;
                    }

                    // Word doesn't fit.
                    if (lineLen == 0)
                    {
                        // Hard-break long token.
                        int take = budget < 1 ? 1 : budget;
                        lineEnd = wordStart + take;
                        i = lineEnd;
                    }
                    else
                    {
                        i = wordStart; // rewind so next loop starts on the word.
                    }
                    break;
                }

                string piece = text.Substring(lineStart, Math.Max(0, lineEnd - lineStart)).TrimEnd();
                if (!firstLine && indentContinuations && piece.Length > 0)
                    piece = StatusContinuationIndent + piece;
                if (piece.Length == 0)
                    piece = " ";

                _aT(piece, xLeft, y, scale, color, font, align);
                y += lineH;
                lines++;
                firstLine = false;
            }

            if (lines == 0)
            {
                _aT(" ", xLeft, yStart, scale, color, font, align);
                return 1;
            }

            return lines;
        }

        internal float MeasureStatusContent(VRageMath.Vector2 panelSize, VRageMath.RectangleF bounds, string filter, float textScale)
        {
            if (_igcParser == null)
                return panelSize.Y * 0.06f;
            _igcParser.GetSystemStatuses(_sysStatusScratch);
            float effectiveScale = StatusEffectiveTextScale(textScale, bounds.Width);
            float lineH = StatusLineHeight(panelSize.Y, effectiveScale, textScale);
            int maxCharsFirst = StatusMaxCharsForWidth(bounds.Width, effectiveScale);
            int maxCharsCont = StatusContinuationBudget(maxCharsFirst);
            int lines = 0;
            for (int bi = 0; bi < _sysStatusScratch.Count; bi++)
            {
                string b = _sysStatusScratch[bi];
                if (string.IsNullOrEmpty(b))
                    continue;
                if (!string.IsNullOrEmpty(filter) && b.IndexOf(filter, StrIX.C) < 0)
                    continue;
                if (lines > 0)
                    lines++;
                int i0 = 0;
                while (i0 <= b.Length)
                {
                    int nl = b.IndexOf('\n', i0);
                    string seg = nl < 0 ? b.Substring(i0) : b.Substring(i0, nl - i0);
                    if (seg.Length == 0)
                        lines++;
                    else
                        lines += CountWrappedWords(seg, maxCharsFirst, maxCharsCont);
                    if (nl < 0)
                        break;
                    i0 = nl + 1;
                }
            }
            if (lines == 0)
                lines = 1;
            return lines * lineH + panelSize.Y * 0.02f;
        }

        internal void DrawStatusContent(
            VRageMath.Vector2 panelSize,
            VRageMath.RectangleF viewport,
            string filter,
            float yTop,
            float clipTop,
            float clipBottom,
            float textScale)
        {
            if (_igcParser == null)
                return;
            float totalH = MeasureStatusContent(panelSize, viewport, filter, textScale);
            if (yTop + totalH <= clipTop || yTop >= clipBottom)
                return;

            _igcParser.GetSystemStatuses(_sysStatusScratch);
            float effectiveScale = StatusEffectiveTextScale(textScale, viewport.Width);
            float lineH = StatusLineHeight(panelSize.Y, effectiveScale, textScale);
            int maxCharsFirst = StatusMaxCharsForWidth(viewport.Width, effectiveScale);
            int maxCharsCont = StatusContinuationBudget(maxCharsFirst);
            float xLeft = viewport.X + viewport.Width * 0.04f;
            float y = yTop;
            bool any = false;
            for (int bi = 0; bi < _sysStatusScratch.Count; bi++)
            {
                string b = _sysStatusScratch[bi];
                if (string.IsNullOrEmpty(b))
                    continue;
                if (!string.IsNullOrEmpty(filter) && b.IndexOf(filter, StrIX.C) < 0)
                    continue;
                any = true;
                if (y > yTop + 0.5f)
                    y += lineH;
                int i0 = 0;
                while (i0 <= b.Length)
                {
                    int nl = b.IndexOf('\n', i0);
                    string seg = nl < 0 ? b.Substring(i0) : b.Substring(i0, nl - i0);
                    if (seg.Length == 0)
                    {
                        if (y + lineH > clipTop && y < clipBottom)
                            _aT(" ", xLeft, y, effectiveScale, _cGy, _fM, TextAlignment.LEFT);
                        y += lineH;
                    }
                    else
                    {
                        if (y + lineH > clipTop && y < clipBottom)
                        {
                            int used = DrawWrappedWords(seg, xLeft, y, lineH, effectiveScale, _cW, _fM, TextAlignment.LEFT, maxCharsFirst, maxCharsCont, true);
                            y += used * lineH;
                        }
                        else
                        {
                            int used = CountWrappedWords(seg, maxCharsFirst, maxCharsCont);
                            y += used * lineH;
                        }
                    }

                    if (nl < 0)
                        break;
                    i0 = nl + 1;
                }
            }
            if (!any && y + lineH > clipTop && y < clipBottom)
                _aT("(no matching status)", xLeft, y, effectiveScale, _cGy, _fM, TextAlignment.LEFT);
        }

        /// <summary>Shared ore/ingot merge + component index lists for virtual <c>INV</c> modules.</summary>
        internal static void BuildInventoryDisplayListsFromDynamic(
            string filterArg,
            InventoryDynamicDTO dynDto,
            Dictionary<string, float> oreByName,
            Dictionary<string, float> ingotByName,
            List<int> compIndices,
            List<string> mergeKeys)
        {
            if (dynDto.itemNames == null || dynDto.itemAmounts == null || dynDto.itemTypes == null)
            {
                oreByName.Clear();
                ingotByName.Clear();
                compIndices.Clear();
                mergeKeys.Clear();
                return;
            }

            bool noFilter = string.IsNullOrEmpty(filterArg);
            bool keywordOi = string.Equals(filterArg, "OresIngots", StrIX.C);
            bool keywordComp = string.Equals(filterArg, "Components", StrIX.C);

            oreByName.Clear();
            ingotByName.Clear();
            compIndices.Clear();
            mergeKeys.Clear();

            int nDyn = dynDto.itemNames.Length;
            for (int i = 0; i < nDyn; i++)
            {
                if (dynDto.itemAmounts == null || dynDto.itemTypes == null || dynDto.itemAmounts[i] <= 0.001f)
                    continue;

                string type = dynDto.itemTypes[i] ?? "";
                string rawName = dynDto.itemNames[i] ?? "";
                if (type == "Ore")
                {
                    float prev;
                    oreByName[rawName] = oreByName.TryGetValue(rawName, out prev) ? prev + dynDto.itemAmounts[i] : dynDto.itemAmounts[i];
                }
                else if (type == "Ingot")
                {
                    float prev;
                    ingotByName[rawName] = ingotByName.TryGetValue(rawName, out prev) ? prev + dynDto.itemAmounts[i] : dynDto.itemAmounts[i];
                }
                else
                {
                    compIndices.Add(i);
                }
            }

            if (!keywordComp)
            {
                if (noFilter || keywordOi)
                {
                    foreach (var k in oreByName.Keys)
                        mergeKeys.Add(k);
                    foreach (var k in ingotByName.Keys)
                    {
                        if (!oreByName.ContainsKey(k))
                            mergeKeys.Add(k);
                    }
                }
                else
                {
                    foreach (var k in oreByName.Keys)
                    {
                        if (string.Equals(k, filterArg, StrIX.C))
                            mergeKeys.Add(k);
                    }

                    foreach (var k in ingotByName.Keys)
                    {
                        if (oreByName.ContainsKey(k))
                            continue;
                        if (string.Equals(k, filterArg, StrIX.C))
                            mergeKeys.Add(k);
                    }
                }

                mergeKeys.Sort(StringComparer.OrdinalIgnoreCase);
            }

            compIndices.Sort((a, b) => string.Compare(dynDto.itemNames[a] ?? "", dynDto.itemNames[b] ?? "", StrIX.C));

            if (keywordOi)
                compIndices.Clear();
            else if (!noFilter && !keywordComp)
            {
                for (int j = compIndices.Count - 1; j >= 0; j--)
                {
                    int idx = compIndices[j];
                    string nm = dynDto.itemNames[idx] ?? "";
                    if (!string.Equals(nm, filterArg, StrIX.C))
                        compIndices.RemoveAt(j);
                }
            }
        }
    }
}
