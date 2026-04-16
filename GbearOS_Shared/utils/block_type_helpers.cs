// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: block_type_helpers.cs
// Purpose: Terminal-block kind checks (refinery, assembler, battery, reactor, cargo, gas gen, oxygen farm, etc.).
// PB Association: Shared
// Dependencies: IMyTerminalBlock and ingame interfaces
// Key Methods: IsRefinery, IsCargo

using System;
using System.Text;
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class BlockTypeHelpers
    {
        public static bool IsRefinery(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "Refinery") >= 0
                   || IndexOfOrdinalIgnoreCase(sub, "BlastFurnace") >= 0;
        }

        public static bool IsAssembler(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "Assembler") >= 0;
        }

        public static bool IsBattery(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "Battery") >= 0;
        }

        public static bool IsHydrogenEngine(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "HydrogenEngine") >= 0;
        }

        public static bool IsReactor(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "Reactor") >= 0;
        }

        public static bool IsCargo(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "CargoContainer") >= 0
                   || IndexOfOrdinalIgnoreCase(sub, "Cargo") >= 0
                   || IndexOfOrdinalIgnoreCase(sub, "MediumContainer") >= 0;
        }

        public static bool IsGasGenerator(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "OxygenGenerator") >= 0
                   || IndexOfOrdinalIgnoreCase(sub, "GasGenerator") >= 0;
        }

        public static bool IsOxygenFarm(IMyTerminalBlock block)
        {
            string sub = SubtypeNameOrEmpty(block);
            if (sub.Length == 0)
            {
                return false;
            }

            return IndexOfOrdinalIgnoreCase(sub, "OxygenFarm") >= 0;
        }

        private static string SubtypeNameOrEmpty(IMyTerminalBlock block)
        {
            if (block == null)
            {
                return string.Empty;
            }

            string s = block.BlockDefinition.SubtypeName;
            return s != null ? s : string.Empty;
        }

        private static int IndexOfOrdinalIgnoreCase(string haystack, string needle)
        {
            return haystack.IndexOf(needle, StrIX.C);
        }
    }
}
