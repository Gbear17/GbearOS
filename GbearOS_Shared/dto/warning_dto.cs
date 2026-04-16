// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: warning_dto.cs
// Purpose: Wire DTO for consolidated operational warning flags and optional active diagnostic code/message.
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
    public class WarningDTO
    {
        public bool lowIce;
        public bool lowPower;
        public bool cargoFull;
        public bool noRefineries;
        public bool refineryStalled;
        public bool assemblerStalled;

        public int activeCode;
        public string activeMessage;
        public bool isNominal;
    }
}
