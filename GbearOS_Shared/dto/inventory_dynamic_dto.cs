// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: inventory_dynamic_dto.cs
// Purpose: Wire DTO for modded or non-vanilla item rows (name, amount, kind) for dynamic inventory lists.
// PB Association: Shared
// Dependencies: None (plain DTO)
// Key Methods: — (DTO fields only)

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class InventoryDynamicDTO
    {
        public string[] itemNames;
        public float[] itemAmounts;
        public string[] itemTypes; // "Ore", "Ingot", "Component", etc.
    }
}
