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

        private readonly IMyBroadcastListener[] _listeners = new IMyBroadcastListener[8];
        private int _listenerCount;

        private readonly Dictionary<string, long> _networkLastSeen = new Dictionary<string, long>();
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
        }

        private void RegisterListener(string channelTag)
        {
            IMyBroadcastListener listener = _me.IGC.RegisterBroadcastListener(channelTag);
            listener.SetMessageCallback("PB1_MSG");
            _listeners[_listenerCount] = listener;
            _listenerCount++;
        }

        public void ProcessMessages()
        {
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
                        Route(msg, text);
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

        private void Route(MyIGCMessage msg, string text)
        {
            if (string.IsNullOrEmpty(_sharedKey))
            {
                return;
            }

            string tag = msg.Tag;

            if (tag == IGCChannels.SYS_STATUS)
            {
                long src = msg.Source;
                _systemStatusTicks[src] = System.DateTime.UtcNow.Ticks;
                _systemStatuses[src] = text ?? string.Empty;
                return;
            }

            string innerPayload;
            string envelopePbId;
            if (!SenderEnvelope.TryParse(text, _sharedKey, _networkLastSeen, out envelopePbId, out innerPayload))
            {
                _droppedEnvelopeCount++;
                return;
            }

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
