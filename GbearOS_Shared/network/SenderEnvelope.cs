// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: SenderEnvelope.cs
// Purpose: Stateless outer frame — Base64 inner payload, monotonic UTC ticks, and FNV-style MAC for weak authentication.
// PB Association: Shared
// Dependencies: None (BCL only)
// Key Methods: Wrap, TryParse

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
        /// </summary>
        public static bool TryParse(
            string rawMessage,
            string sharedKey,
            Dictionary<string, long> lastSeenTimestamps,
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
            long prev;
            if (lastSeenTimestamps.TryGetValue(sid, out prev))
            {
                lastSeen = prev;
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
            pbId = sid;
            payload = pl;
            return true;
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
