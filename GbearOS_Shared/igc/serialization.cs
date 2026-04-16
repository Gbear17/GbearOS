// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: serialization.cs
// Purpose: Script-level Serializer facade — shared Serialize entrypoint delegating to IGCSerializer.
// PB Association: Shared
// Dependencies: IGCSerializer
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
    /// <summary>
    /// PB1 Serialize entry point; delegates to IGCSerializer.
    /// PB2 Deserialize entry point lives in GbearOS_PB2_Display/serialization_deserialize.cs.
    /// </summary>
    public static partial class Serializer
    {
        public static string Serialize(object dto)
        {
            return IGCSerializer.Serialize(dto);
        }
    }
}
