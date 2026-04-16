// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: formatting_utils.cs
// Purpose: Human-readable formatting for floats, percents, large counts, warning lines, and ore abbreviations.
// PB Association: Shared
// Dependencies: None
// Key Methods: FormatLargeNumber, FormatPercent

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class FormattingUtils
    {
        private static readonly StringBuilder _sb = new StringBuilder(48);

        // Format a float with no trailing zeros
        public static string FormatFloat(float value)
        {
            if (float.IsNaN(value))
                return "NaN";
            if (float.IsInfinity(value))
                return value > 0f ? "Infinity" : "-Infinity";

            return value.ToString("0.######");
        }

        // Format percentage points (0–100), e.g. 23.5f -> "24%". PB1 cargoPercent is already (used/max)*100.
        public static string FormatPercent(float value)
        {
            if (float.IsNaN(value))
                return "NaN%";
            if (float.IsInfinity(value))
                return value > 0f ? "Infinity%" : "-Infinity%";

            int p = (int)Math.Round((double)value);
            _sb.Clear();
            _sb.Append(p.ToString());
            _sb.Append('%');
            return _sb.ToString();
        }

        // Format a large number with suffixes (k, M, B)
        public static string FormatLargeNumber(float value)
        {
            if (float.IsNaN(value))
                return "NaN";
            if (float.IsInfinity(value))
                return value > 0f ? "Infinity" : "-Infinity";

            bool negative = value < 0f;
            double v = negative ? -(double)value : (double)value;

            string suffix = "";
            double divisor = 1.0;
            if (v >= 1e9)
            {
                suffix = "B";
                divisor = 1e9;
            }
            else if (v >= 1e6)
            {
                suffix = "M";
                divisor = 1e6;
            }
            else if (v >= 1e3)
            {
                suffix = "k";
                divisor = 1e3;
            }

            _sb.Clear();
            if (negative)
                _sb.Append('-');

            if (suffix.Length > 0)
            {
                double scaled = v / divisor;
                scaled = Math.Round(scaled * 10.0) / 10.0;
                _sb.Append(scaled.ToString("0.0"));
                _sb.Append(suffix);
            }
            else
            {
                float fv = negative ? -(float)v : (float)v;
                _sb.Append(fv.ToString("0.######"));
            }

            return _sb.ToString();
        }

        // Format a warning line for PB2 LCDs
        public static string FormatWarning(string label, bool active)
        {
            if (!active)
                return string.Empty;

            _sb.Clear();
            _sb.Append("! ");
            if (label != null)
                _sb.Append(label);
            _sb.Append(" !");
            return _sb.ToString();
        }

        /// <summary>Two-character style labels for ores/ingots on narrow LCD headers (matches REF row ore column).</summary>
        public static string OreSubtypeAbbrev(string sub)
        {
            if (string.IsNullOrEmpty(sub))
            {
                return "-";
            }

            if (string.Equals(sub, "Iron", StrIX.C))
            {
                return "Fe";
            }

            if (string.Equals(sub, "Nickel", StrIX.C))
            {
                return "Ni";
            }

            if (string.Equals(sub, "Cobalt", StrIX.C))
            {
                return "Co";
            }

            if (string.Equals(sub, "Silicon", StrIX.C))
            {
                return "Si";
            }

            if (string.Equals(sub, "Silver", StrIX.C))
            {
                return "Ag";
            }

            if (string.Equals(sub, "Gold", StrIX.C))
            {
                return "Au";
            }

            if (string.Equals(sub, "Magnesium", StrIX.C))
            {
                return "Mg";
            }

            if (string.Equals(sub, "Platinum", StrIX.C))
            {
                return "Pt";
            }

            if (string.Equals(sub, "Uranium", StrIX.C))
            {
                return "U";
            }

            if (string.Equals(sub, "Stone", StrIX.C))
            {
                return "St";
            }

            if (string.Equals(sub, "Ice", StrIX.C))
            {
                return "Ic";
            }

            if (sub.Length <= 2)
            {
                return sub.ToUpperInvariant();
            }

            return sub.Substring(0, 2).ToUpperInvariant();
        }
    }
}
