// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: ice_status_dto.cs
// Purpose: Wire DTO for ice totals by bucket (generator, irrigation, cargo), percents, counts, and low-ice flag.
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
    public class IceStatusDTO
    {
        public float totalIce;
        public float generatorIce;
        public float irrigationIce;
        public float cargoIce;

        public float pctTotal;
        public float pctGen;
        public float pctIrr;
        public float pctCargo;

        public int generatorCount;
        public int irrigationCount;

        public bool lowIce;
    }
}
