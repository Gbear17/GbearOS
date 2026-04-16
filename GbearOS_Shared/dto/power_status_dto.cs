// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: power_status_dto.cs
// Purpose: Wire DTO for battery/reactor/engine/solar aggregates and low-power flag for PB2 gauges.
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
    public class PowerStatusDTO
    {
        public float batteryStored;
        public float batteryMax;
        public float batteryInput;
        public float batteryOutput;
        public float batteryMaxInput;
        public float batteryMaxOutput;

        public float reactorOutput;
        public float engineOutput;
        public float reactorMaxOutput;
        public float engineMaxOutput;

        public int batteryCount;
        public int reactorCount;
        public int engineCount;

        public bool lowPower;
    }
}
