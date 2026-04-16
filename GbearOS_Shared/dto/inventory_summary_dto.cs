// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: inventory_summary_dto.cs
// Purpose: Wire DTO for vanilla ore/ingot buckets, component/ammo/tool/bottle totals, and cargo fill ratio.
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
    public class InventorySummaryDTO
    {
        // Ores
        public float ironOre;
        public float nickelOre;
        public float cobaltOre;
        public float siliconOre;
        public float magnesiumOre;
        public float silverOre;
        public float goldOre;
        public float platinumOre;
        public float uraniumOre;
        public float stoneOre;
        public float iceOre;

        // Ingots
        public float ironIngot;
        public float nickelIngot;
        public float cobaltIngot;
        public float siliconIngot;
        public float magnesiumPowder;
        public float silverIngot;
        public float goldIngot;
        public float platinumIngot;
        public float uraniumIngot;

        // Components
        public float componentsTotal;

        // Ammo
        public float ammoTotal;

        // Tools
        public float toolsTotal;

        // Bottles (O2 + H2)
        public float bottlesTotal;

        // Cargo utilization
        public float cargoUsed;
        public float cargoMax;
        public float cargoPercent;
    }
}
