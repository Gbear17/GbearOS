// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: SenderEnvelope.cs
// Purpose: Stateless outer frame — Base64 inner payload, monotonic UTC ticks, and FNV-style MAC for weak authentication.
// PB Association: Shared
// Dependencies: None (BCL only)
// Key Methods: Wrap, TryParse, EvictStaleReplayState

using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    /// <summary>
    /// Stateless MAC envelope: PBID|Timestamp|PayloadB64|MAC (NOC Phase 1, v1.4).
    /// Payload is UTF-8 Base64 so inner IGC strings may contain '|' without breaking the four-field split.
    /// </summary>
    public static class SenderEnvelope
    {
        private const uint FnvOffsetBasis = 2166136261u;
        private const uint FnvPrime = 16777619u;


        /// <summary>
        /// PB2-only: wall-clock silence after the last accepted envelope for a sender before its replay baseline is forgotten (Phase 3 / decision 3A).
        /// </summary>
        public const long ReplaySilenceExpiryTicks = 90L * TimeSpan.TicksPerSecond;

        private static long s_lastEmitTicks;

        /// <summary>
        /// 32-bit FNV-1a over the string (UTF-16 code units as LE byte pairs). Do not use string.GetHashCode().
        /// </summary>
        public static uint Fnv1aHash(string data)
        {
            return Fnv1aFold(FnvOffsetBasis, data);
        }

        /// <summary>
        /// Builds PBID|Timestamp|Base64(UTF8(payload))|MAC. MAC is over pbId+timestamp+payload+sharedKey (logical payload, not Base64).
        /// Timestamps are forced strictly increasing per process to avoid replay drops on back-to-back sends in one tick.
        /// </summary>
        public static string Wrap(string pbId, string payload, string sharedKey)
        {
            long ticks = DateTime.UtcNow.Ticks;
            if (ticks <= s_lastEmitTicks)
            {
                ticks = s_lastEmitTicks + 1;
            }

            s_lastEmitTicks = ticks;

            string p = payload ?? "";
            string timestamp = ticks.ToString();
            string material = (pbId ?? "") + timestamp + p + (sharedKey ?? "");
            uint h = Fnv1aHash(material);
            string mac = h.ToString("X8");
            string b64 = p.Length == 0 ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(p));
            return (pbId ?? "") + "|" + timestamp + "|" + b64 + "|" + mac;
        }

        /// <summary>
        /// Parses rawMessage with Split('|', 4), decodes payload from Base64, verifies replay window and MAC.
        /// When <paramref name="lastAcceptedWallUtcTicks"/> is non-null and <paramref name="replaySilenceExpiryTicks"/> is positive,
        /// a sender’s last accepted envelope timestamp older than <paramref name="replaySilenceExpiryTicks"/> clears the replay baseline for that sender (PB1 restart/recompile).
        /// </summary>
        public static bool TryParse(
            string rawMessage,
            string sharedKey,
            Dictionary<string, long> lastSeenTimestamps,
            Dictionary<string, long> lastAcceptedWallUtcTicks,
            long utcNowTicks,
            long replaySilenceExpiryTicks,
            out string pbId,
            out string payload)
        {
            pbId = null;
            payload = null;

            if (rawMessage == null || lastSeenTimestamps == null)
            {
                return false;
            }

            string[] parts = rawMessage.Split(new[] { '|' }, 4);
            if (parts.Length != 4)
            {
                return false;
            }

            string sid = parts[0];
            string ts = parts[1];
            string plB64 = parts[2];
            string macIn = parts[3];

            if (sid == null || ts == null || plB64 == null || macIn == null)
            {
                return false;
            }

            string pl;
            if (plB64.Length == 0)
            {
                pl = "";
            }
            else
            {
                byte[] raw;
                try
                {
                    raw = Convert.FromBase64String(plB64);
                }
                catch
                {
                    return false;
                }

                pl = Encoding.UTF8.GetString(raw);
            }

            long incomingTicks;
            if (!long.TryParse(ts, out incomingTicks))
            {
                return false;
            }

            long lastSeen = 0;
            bool silenceExpiryEnabled = lastAcceptedWallUtcTicks != null && replaySilenceExpiryTicks > 0;
            long prev;
            if (lastSeenTimestamps.TryGetValue(sid, out prev))
            {
                if (silenceExpiryEnabled)
                {
                    long wall;
                    if (lastAcceptedWallUtcTicks.TryGetValue(sid, out wall))
                    {
                        if (utcNowTicks - wall > replaySilenceExpiryTicks)
                        {
                            lastSeenTimestamps.Remove(sid);
                            lastAcceptedWallUtcTicks.Remove(sid);
                            lastSeen = 0;
                        }
                        else
                        {
                            lastSeen = prev;
                        }
                    }
                    else
                    {
                        lastSeenTimestamps.Remove(sid);
                        lastSeen = 0;
                    }
                }
                else
                {
                    lastSeen = prev;
                }
            }

            if (incomingTicks <= lastSeen)
            {
                return false;
            }

            string sk = sharedKey ?? "";
            uint h = FnvOffsetBasis;
            h = Fnv1aFold(h, sid);
            h = Fnv1aFold(h, ts);
            h = Fnv1aFold(h, pl);
            h = Fnv1aFold(h, sk);
            string macExpected = h.ToString("X8");
            if (!string.Equals(macIn, macExpected, StringComparison.Ordinal))
            {
                return false;
            }

            lastSeenTimestamps[sid] = incomingTicks;
            if (silenceExpiryEnabled)
            {
                lastAcceptedWallUtcTicks[sid] = utcNowTicks;
            }

            pbId = sid;
            payload = pl;
            return true;
        }

        /// <summary>
        /// Removes replay entries whose last accepted wall time is older than <paramref name="replaySilenceExpiryTicks"/> relative to <paramref name="utcNowTicks"/>.
        /// Uses <paramref name="scratch"/> as temporary storage (cleared); must be non-null when expiry is enabled.
        /// </summary>
        public static void EvictStaleReplayState(
            Dictionary<string, long> lastSeenTimestamps,
            Dictionary<string, long> lastAcceptedWallUtcTicks,
            long utcNowTicks,
            long replaySilenceExpiryTicks,
            List<string> scratch)
        {
            if (lastSeenTimestamps == null || lastAcceptedWallUtcTicks == null || scratch == null)
            {
                return;
            }

            if (replaySilenceExpiryTicks <= 0)
            {
                return;
            }

            scratch.Clear();
            foreach (KeyValuePair<string, long> kv in lastAcceptedWallUtcTicks)
            {
                if (utcNowTicks - kv.Value > replaySilenceExpiryTicks)
                {
                    scratch.Add(kv.Key);
                }
            }

            for (int i = 0; i < scratch.Count; i++)
            {
                string k = scratch[i];
                lastSeenTimestamps.Remove(k);
                lastAcceptedWallUtcTicks.Remove(k);
            }

            scratch.Clear();
            foreach (string k in lastSeenTimestamps.Keys)
            {
                if (!lastAcceptedWallUtcTicks.ContainsKey(k))
                {
                    scratch.Add(k);
                }
            }

            for (int i = 0; i < scratch.Count; i++)
            {
                lastSeenTimestamps.Remove(scratch[i]);
            }
        }

        private static uint Fnv1aFold(uint hash, string s)
        {
            if (s == null || s.Length == 0)
            {
                return hash;
            }

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                hash ^= (byte)(c & 0xFF);
                hash *= FnvPrime;
                hash ^= (byte)((c >> 8) & 0xFF);
                hash *= FnvPrime;
            }

            return hash;
        }
    }
}
