// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: serialization.cs
// Purpose: Script-level Serializer facade — generic Deserialize and Serialize delegating to IGCSerializer.
// PB Association: Shared
// Dependencies: IGCSerializer
// Key Methods: Serialize, Deserialize

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
    /// PB1 Serialize and PB2 Deserialize entry points; delegates to IGCSerializer.
    /// </summary>
    public static class Serializer
    {
        public static string Serialize(object dto)
        {
            return IGCSerializer.Serialize(dto);
        }

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
