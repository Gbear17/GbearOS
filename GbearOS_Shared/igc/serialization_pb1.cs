// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: serialization_pb1.cs
// Purpose: PB1-only Serializer facade — Serialize only (no Deserialize; saves concat size).
// PB Association: PB1
// Dependencies: IGCSerializer (serialize partial)
// Key Methods: Serialize

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class Serializer
    {
        public static string Serialize(object dto)
        {
            return IGCSerializer.Serialize(dto);
        }
    }
}
