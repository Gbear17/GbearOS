// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: serialization_deserialize.cs
// Purpose: PB2-only Serializer facade — generic Deserialize delegating to IGCSerializer.
// PB Association: PB2
// Dependencies: IGCSerializer
// Key Methods: Deserialize

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static partial class Serializer
    {
        public static T Deserialize<T>(string data)
        {
            try
            {
                if (data == null)
                    return default(T);
                return IGCSerializer.Deserialize<T>(data);
            }
            catch
            {
            }
            return default(T);
        }
    }
}

