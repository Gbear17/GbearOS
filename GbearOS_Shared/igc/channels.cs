// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: channels.cs
// Purpose: Canonical string tags for IGC broadcast channels between PB1, PB2, and system status paths.
// PB Association: Shared
// Dependencies: None
// Key Methods: — (IGCChannels string constants)

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class IGCChannels
    {
        public const string SYS_STATUS = "SYS_STATUS";
        public const string PB1_WARNINGS = "PB1_WARNINGS";
        public const string PB2_ACK = "PB2_ACK";
        public const string PB1ToPB2_InventorySummary = "PB1ToPB2_InventorySummary";
        public const string PB1ToPB2_RefineryStatus = "PB1ToPB2_RefineryStatus";
        public const string PB1ToPB2_IceStatus = "PB1ToPB2_IceStatus";
        public const string PB1ToPB2_PowerStatus = "PB1ToPB2_PowerStatus";
        public const string PB1ToPB2_InventoryDynamic = "PB1ToPB2_InventoryDynamic";
        public const string PB2ToPB1 = "PB2ToPB1";
    }
}
