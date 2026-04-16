// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: message_types.cs
// Purpose: Maps inner IGC payload type identifiers to DTO class names for routing deserialization.
// PB Association: Shared
// Dependencies: None
// Key Methods: — (IGCMessageTypes string constants)

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class IGCMessageTypes
    {
        public const string InventorySummary = "InventorySummaryDTO";
        public const string RefineryStatus = "RefineryStatusDTO";
        public const string PowerStatus = "PowerStatusDTO";
        public const string IceStatus = "IceStatusDTO";
        public const string Warning = "WarningDTO";
        public const string InventoryDynamic = "InventoryDynamicDTO";
    }
}
