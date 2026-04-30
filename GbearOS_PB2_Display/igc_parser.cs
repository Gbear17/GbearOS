// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: igc_parser.cs
// Purpose: Registers IGC listeners, unwraps SenderEnvelope, deserializes inner DTO strings, exposes last PB1 telemetry to the renderer.
// PB Association: PB2
// Dependencies: IGCChannels, Serializer, SenderEnvelope, DTO types, MyGridProgram
// Key Methods: Init, ProcessMessages

using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class IGCParser
    {
        private MyGridProgram _me;
        private string _sharedKey = "";

        // #region agent log
        private const int AgentHoldUpdate10Ticks = 30; // ~5s at Update10 cadence (10 ticks per update)
        private int _agentHoldTicksRemaining = 0;
        private readonly List<string> _agentRecent = new List<string>(8);
        private string _agentHoldText = null;

        public bool AgentHoldActive { get { return _agentHoldTicksRemaining > 0 && !string.IsNullOrEmpty(_agentHoldText); } }

        public string AgentGetHoldTextAndTick()
        {
            if (!AgentHoldActive)
                return null;
            _agentHoldTicksRemaining--;
            return _agentHoldText;
        }

        private void AgentLog(string runId, string hypothesisId, string location, string message, string dataJson)
        {
            try
            {
                // Space Engineers prohibits file I/O; use Echo as the runtime evidence channel.
                string line = "{\"sessionId\":\"3eb514\",\"runId\":\"" + (runId ?? "") + "\",\"hypothesisId\":\"" + (hypothesisId ?? "") +
                              "\",\"location\":\"" + (location ?? "") + "\",\"message\":\"" + (message ?? "") +
                              "\",\"data\":" + (dataJson ?? "{}") + ",\"timestamp\":" + System.DateTime.UtcNow.Ticks + "}";

                if (_agentRecent.Count >= 8)
                    _agentRecent.RemoveAt(0);
                _agentRecent.Add("AGENTLOG " + line);

                var sb = new System.Text.StringBuilder(1536);
                sb.AppendLine("=== AGENTLOG (hold ~5s) ===");
                for (int i = 0; i < _agentRecent.Count; i++)
                    sb.AppendLine(_agentRecent[i]);
                _agentHoldText = sb.ToString();
                _agentHoldTicksRemaining = AgentHoldUpdate10Ticks;
            }
            catch
            {
            }
        }
        private static string AgentJsonEsc(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }
        // #endregion

        private readonly IMyBroadcastListener[] _listeners = new IMyBroadcastListener[8];
        private int _listenerCount;

        private readonly Dictionary<string, long> _networkLastSeen = new Dictionary<string, long>();
        private readonly Dictionary<string, long> _networkLastAcceptedWallUtc = new Dictionary<string, long>();
        private readonly List<string> _replayEvictScratch = new List<string>();
        private int _droppedEnvelopeCount;

        private InventorySummaryDTO _inventorySummary = new InventorySummaryDTO();
        private RefineryStatusDTO _refineryStatus = new RefineryStatusDTO();
        private IceStatusDTO _iceStatus = new IceStatusDTO();
        private PowerStatusDTO _powerStatus = new PowerStatusDTO();
        private InventoryDynamicDTO _inventoryDynamic = new InventoryDynamicDTO();
        private WarningDTO _warnings = new WarningDTO();

        private readonly Dictionary<long, string> _systemStatuses = new Dictionary<long, string>();
        private readonly Dictionary<long, long> _systemStatusTicks = new Dictionary<long, long>();
        private readonly List<long> _staleStatusSourceScratch = new List<long>();

        public InventorySummaryDTO InventorySummary { get { return _inventorySummary; } }
        public RefineryStatusDTO RefineryStatus { get { return _refineryStatus; } }
        public IceStatusDTO IceStatus { get { return _iceStatus; } }
        public PowerStatusDTO PowerStatus { get { return _powerStatus; } }
        public InventoryDynamicDTO InventoryDynamic { get { return _inventoryDynamic; } }
        public WarningDTO Warnings { get { return _warnings; } }

        /// <summary>UTC ticks when any PB1 DTO or <c>PB1_WARNINGS</c> was last received; 0 if never.</summary>
        public long LastPb1DataTicks { get; private set; }

        /// <summary>DTO / warnings messages dropped after failing SenderEnvelope (MAC or replay).</summary>
        public int DroppedEnvelopeCount { get { return _droppedEnvelopeCount; } }

        public bool EnableNetwork { get; private set; }

        /// <summary>Self-healing display PB identity: <c>PREFIX-XXXX</c> suffix bound to this programmable block's EntityId.</summary>
        public string PBID { get; private set; } = "";

        /// <summary>Latest PB2 internal performance snapshot; injected into <see cref="GetSystemStatuses"/> for <c>[STATUS:…]</c> filters (include <see cref="PBID"/> substring).</summary>
        public string SelfStatus { get; set; }

        public void Init(MyGridProgram me)
        {
            _me = me;
            _listenerCount = 0;
            LoadNetworkSharedKeyFromCustomData();

            RegisterListener(IGCChannels.PB1ToPB2_InventorySummary);
            RegisterListener(IGCChannels.PB1ToPB2_RefineryStatus);
            RegisterListener(IGCChannels.PB1ToPB2_IceStatus);
            RegisterListener(IGCChannels.PB1ToPB2_PowerStatus);
            RegisterListener(IGCChannels.PB1ToPB2_InventoryDynamic);

            RegisterListener(IGCChannels.SYS_STATUS);
            RegisterListener(IGCChannels.PB1_WARNINGS);

            // #region agent log
            AgentLog(
                "pre-fix",
                "H2",
                "GbearOS_PB2_Display/igc_parser.cs:Init",
                "PB2 init completed (key cached once)",
                "{\"pb2EntityId\":" + _me.Me.EntityId + ",\"pb2Pbid\":\"" + AgentJsonEsc(PBID) + "\",\"enableNetwork\":" + (EnableNetwork ? "true" : "false") +
                ",\"sharedKeyLen\":" + (_sharedKey == null ? 0 : _sharedKey.Length) + "}");
            // #endregion
        }

        private static string ExtractPbidPrefix(string raw, string defaultIfExtractEmpty)
        {
            if (string.IsNullOrEmpty(raw))
                return defaultIfExtractEmpty;
            int dash = raw.IndexOf('-');
            string first = dash < 0 ? raw : raw.Substring(0, dash);
            char[] buf = new char[3];
            int len = 0;
            for (int i = 0; i < first.Length && len < 3; i++)
            {
                char ch = first[i];
                if (char.IsLetterOrDigit(ch))
                {
                    buf[len] = char.ToUpperInvariant(ch);
                    len++;
                }
            }

            if (len == 0)
                return defaultIfExtractEmpty;
            return new string(buf, 0, len);
        }

        private string ComposeBoundPbid(string rawRead, string defaultPrefix)
        {
            string suffix = _me.Me.EntityId.ToString("X");
            suffix = suffix.Substring(Math.Max(0, suffix.Length - 4));
            string prefix = ExtractPbidPrefix(rawRead, defaultPrefix);
            return prefix + "-" + suffix;
        }

        private void LoadNetworkSharedKeyFromCustomData()
        {
            IMyProgrammableBlock pb = _me.Me;
            var ini = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
            VRage.Game.ModAPI.Ingame.Utilities.MyIniParseResult r;
            if (!ini.TryParse(pb.CustomData ?? "", out r))
            {
                ini.Clear();
            }

            string key = ini.Get("Network", "SharedKey").ToString("");
            bool enableNetwork = ini.Get("Network", "EnableNetwork").ToBoolean(true);
            string rawPbid = ini.Get("Network", "PBID").ToString("");
            if (rawPbid != null)
                rawPbid = rawPbid.Trim();
            this.PBID = ComposeBoundPbid(rawPbid ?? "", "DIS");

            if (ini.ContainsKey("Network", "SenderId"))
                ini.Delete("Network", "SenderId");

            ini.Set("Network", "EnableNetwork", enableNetwork);
            ini.SetComment("Network", "EnableNetwork", "See docs/configuration.md — set false for offline mode (no envelope parse).");
            ini.Set("Network", "PBID", this.PBID);
            ini.SetComment("Network", "PBID", "Format: ABC-XXXX. You may change the 3-letter prefix. The 4-character suffix is locked to this block's ID and will auto-reset if changed.");
            ini.Set("Network", "SharedKey", key);
            ini.SetComment("Network", "SharedKey", "Must match PB1 SharedKey.");
            pb.CustomData = ini.ToString();

            EnableNetwork = enableNetwork;
            _sharedKey = key == null ? "" : key.Trim();

            // #region agent log
            AgentLog(
                "pre-fix",
                "H2",
                "GbearOS_PB2_Display/igc_parser.cs:LoadNetworkSharedKeyFromCustomData",
                "PB2 loaded Network settings from CustomData",
                "{\"pb2EntityId\":" + pb.EntityId + ",\"pb2Pbid\":\"" + AgentJsonEsc(PBID) + "\",\"enableNetwork\":" + (EnableNetwork ? "true" : "false") +
                ",\"sharedKeyLen\":" + (_sharedKey == null ? 0 : _sharedKey.Length) + "}");
            // #endregion
        }

        private void RegisterListener(string channelTag)
        {
            IMyBroadcastListener listener = _me.IGC.RegisterBroadcastListener(channelTag);
            listener.SetMessageCallback("PB1_MSG");
            _listeners[_listenerCount] = listener;
            _listenerCount++;

            // #region agent log
            AgentLog(
                "pre-fix",
                "H1",
                "GbearOS_PB2_Display/igc_parser.cs:RegisterListener",
                "PB2 registered broadcast listener",
                "{\"channelTag\":\"" + AgentJsonEsc(channelTag) + "\",\"listenerCount\":" + _listenerCount + "}");
            // #endregion
        }

        public void ProcessMessages()
        {
            long utcNow = System.DateTime.UtcNow.Ticks;
            SenderEnvelope.EvictStaleReplayState(
                _networkLastSeen,
                _networkLastAcceptedWallUtc,
                utcNow,
                SenderEnvelope.ReplaySilenceExpiryTicks,
                _replayEvictScratch);

            for (int i = 0; i < _listenerCount; i++)
            {
                IMyBroadcastListener listener = _listeners[i];
                while (listener.HasPendingMessage)
                {
                    MyIGCMessage msg = listener.AcceptMessage();
                    object data = msg.Data;
                    string text = data as string;
                    if (text == null)
                        continue;

                    try
                    {
                        Route(msg, text, utcNow);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void GetSystemStatuses(List<string> outList)
        {
            outList.Clear();
            long cutoff = System.DateTime.UtcNow.Ticks - 30L * System.TimeSpan.TicksPerSecond;
            _staleStatusSourceScratch.Clear();
            foreach (KeyValuePair<long, string> kv in _systemStatuses)
            {
                long tick;
                if (!_systemStatusTicks.TryGetValue(kv.Key, out tick) || tick < cutoff)
                    _staleStatusSourceScratch.Add(kv.Key);
            }
            for (int i = 0; i < _staleStatusSourceScratch.Count; i++)
            {
                long k = _staleStatusSourceScratch[i];
                _systemStatuses.Remove(k);
                _systemStatusTicks.Remove(k);
            }
            foreach (KeyValuePair<long, string> kv in _systemStatuses)
                outList.Add(kv.Value);

            if (!string.IsNullOrEmpty(SelfStatus))
                outList.Add(SelfStatus);
        }

        private void Route(MyIGCMessage msg, string text, long utcNowTicks)
        {
            if (string.IsNullOrEmpty(_sharedKey))
            {
                // #region agent log
                AgentLog(
                    "pre-fix",
                    "H2",
                    "GbearOS_PB2_Display/igc_parser.cs:Route",
                    "Route ignored message because shared key is empty",
                    "{\"tag\":\"" + AgentJsonEsc(msg.Tag) + "\",\"src\":" + msg.Source + ",\"sharedKeyLen\":0}");
                // #endregion
                return;
            }

            string tag = msg.Tag;

            if (tag == IGCChannels.SYS_STATUS)
            {
                long src = msg.Source;
                _systemStatusTicks[src] = utcNowTicks;
                _systemStatuses[src] = text ?? string.Empty;

                // #region agent log
                string preview = text ?? "";
                if (preview.Length > 64) preview = preview.Substring(0, 64);
                AgentLog(
                    "pre-fix",
                    "H1",
                    "GbearOS_PB2_Display/igc_parser.cs:Route(SYS_STATUS)",
                    "Stored SYS_STATUS by msg.Source (multiple PB1s accumulate)",
                    "{\"src\":" + src + ",\"tag\":\"SYS_STATUS\",\"textPreview\":\"" + AgentJsonEsc(preview) + "\",\"registryCount\":" + _systemStatuses.Count + "}");
                // #endregion
                return;
            }

            string innerPayload;
            string envelopePbId;
            if (!SenderEnvelope.TryParse(
                    text,
                    _sharedKey,
                    _networkLastSeen,
                    _networkLastAcceptedWallUtc,
                    utcNowTicks,
                    SenderEnvelope.ReplaySilenceExpiryTicks,
                    out envelopePbId,
                    out innerPayload))
            {
                _droppedEnvelopeCount++;

                // #region agent log
                AgentLog(
                    "pre-fix",
                    "H3",
                    "GbearOS_PB2_Display/igc_parser.cs:Route(TryParse)",
                    "Dropped envelope (MAC/replay/parse failure)",
                    "{\"tag\":\"" + AgentJsonEsc(tag) + "\",\"src\":" + msg.Source + ",\"droppedEnvelopeCount\":" + _droppedEnvelopeCount + "}");
                // #endregion
                return;
            }

            // #region agent log
            AgentLog(
                "pre-fix",
                "H1",
                "GbearOS_PB2_Display/igc_parser.cs:Route(accepted)",
                "Accepted envelope",
                "{\"tag\":\"" + AgentJsonEsc(tag) + "\",\"src\":" + msg.Source + ",\"envelopePbId\":\"" + AgentJsonEsc(envelopePbId) + "\",\"innerLen\":" + (innerPayload == null ? 0 : innerPayload.Length) + "}");
            // #endregion

            if (tag == IGCChannels.PB1ToPB2_InventorySummary)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                InventorySummaryDTO dto = Serializer.Deserialize<InventorySummaryDTO>(innerPayload);
                if (dto != null)
                    _inventorySummary = dto;
                return;
            }
            if (tag == IGCChannels.PB1ToPB2_RefineryStatus)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                RefineryStatusDTO dto = Serializer.Deserialize<RefineryStatusDTO>(innerPayload);
                if (dto != null)
                    _refineryStatus = dto;
                return;
            }
            if (tag == IGCChannels.PB1ToPB2_IceStatus)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                IceStatusDTO dto = Serializer.Deserialize<IceStatusDTO>(innerPayload);
                if (dto != null)
                    _iceStatus = dto;
                return;
            }
            if (tag == IGCChannels.PB1ToPB2_PowerStatus)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                PowerStatusDTO dto = Serializer.Deserialize<PowerStatusDTO>(innerPayload);
                if (dto != null)
                    _powerStatus = dto;
                return;
            }
            if (tag == IGCChannels.PB1ToPB2_InventoryDynamic)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                InventoryDynamicDTO dto = Serializer.Deserialize<InventoryDynamicDTO>(innerPayload);
                if (dto != null)
                    _inventoryDynamic = dto;
                return;
            }
            if (tag == IGCChannels.PB1_WARNINGS)
            {
                LastPb1DataTicks = System.DateTime.UtcNow.Ticks;
                WarningDTO dto = Serializer.Deserialize<WarningDTO>(innerPayload);
                if (dto != null)
                    _warnings = dto;
                return;
            }
        }
    }
}
