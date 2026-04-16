// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: warning_formatter.cs
// Purpose: Formats WarningDTO boolean flags into short labeled lines for warning strips on LCDs.
// PB Association: PB2
// Dependencies: WarningDTO, FormattingUtils
// Key Methods: FormatWarnings

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class WarningFormatter
    {
        // Build a formatted warning list for LCDs
        public string FormatWarnings(WarningDTO warnings)
        {
            if (warnings == null)
                return "(no data)\n";

            if (!warnings.lowIce && !warnings.lowPower && !warnings.cargoFull
                && !warnings.noRefineries && !warnings.refineryStalled && !warnings.assemblerStalled)
                return "";

            var sb = new StringBuilder();
            bool needNewline = false;

            if (warnings.lowIce)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("Low Ice", warnings.lowIce));
                needNewline = true;
            }
            if (warnings.lowPower)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("Low Power", warnings.lowPower));
                needNewline = true;
            }
            if (warnings.cargoFull)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("Cargo Full", warnings.cargoFull));
                needNewline = true;
            }
            if (warnings.noRefineries)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("No Refineries", warnings.noRefineries));
                needNewline = true;
            }
            if (warnings.refineryStalled)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("Refinery Stalled", warnings.refineryStalled));
                needNewline = true;
            }
            if (warnings.assemblerStalled)
            {
                if (needNewline)
                    sb.Append('\n');
                sb.Append(FormattingUtils.FormatWarning("Assembler Stalled", warnings.assemblerStalled));
            }

            return sb.ToString();
        }
    }
}
