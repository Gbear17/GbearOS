// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: refinery_status_dto.cs
// Purpose: Wire DTO carrying per-refinery ore, output, working state, and priority summary lines for PB2.
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
    public class RefineryStatusDTO
    {
        public string[] refineryNames;

        public string[] currentOre;     // Ore in input slot 0 (queue front), e.g. "Iron"; empty if slot 0 is not ore
        public float[] oreAmounts;      // Total ore amount in refinery input (all stacks)

        public string[] outputIngot;    // e.g. "IronIngot"
        public float[] outputAmounts;   // output inventory amounts

        public bool[] isWorking;
        public bool[] hasOre;

        /// <summary>REF LCD priority row 1 (first half, e.g. 1. Fe  2. Co …).</summary>
        public string priorityLine1;
        /// <summary>REF LCD priority row 2 (second half of ranked ores).</summary>
        public string priorityLine2;
    }
}
