// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: item_type_helpers.cs
// Purpose: Classifies item subtype strings as ore, ingot, component, ammo, tool, or bottle for inventory logic.
// PB Association: Shared
// Dependencies: None (string rules only)
// Key Methods: IsOre, IsIngot

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
    /// Subtype-name classification for inventory items. Patterns favor modded names (e.g. containing Ore, Ingot, Component).
    /// Vanilla ore/ingot subtypes are often plain material names (e.g. Iron) with no such tokens; use TypeId when you need those cases.
    /// </summary>
    public static class ItemTypeHelpers
    {
        public static bool IsOre(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            return ContainsIgnoreCase(subtype, "Ore");
        }

        public static bool IsIngot(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            if (ContainsIgnoreCase(subtype, "Ingot"))
            {
                return true;
            }

            return ContainsIgnoreCase(subtype, "Powder");
        }

        public static bool IsComponent(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            if (ContainsIgnoreCase(subtype, "Component"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Plate")
                || ContainsIgnoreCase(subtype, "Tube")
                || ContainsIgnoreCase(subtype, "Girder"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Motor")
                || ContainsIgnoreCase(subtype, "Computer")
                || ContainsIgnoreCase(subtype, "Explosives")
                || ContainsIgnoreCase(subtype, "Superconductor")
                || ContainsIgnoreCase(subtype, "SolarCell"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "MetalGrid")
                || ContainsIgnoreCase(subtype, "BulletproofGlass")
                || ContainsIgnoreCase(subtype, "Display"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Medical")
                || ContainsIgnoreCase(subtype, "PowerCell")
                || ContainsIgnoreCase(subtype, "Communication")
                || ContainsIgnoreCase(subtype, "Reactor")
                || ContainsIgnoreCase(subtype, "Thrust")
                || ContainsIgnoreCase(subtype, "Gravity")
                || ContainsIgnoreCase(subtype, "Detector")
                || ContainsIgnoreCase(subtype, "Targeting")
                || ContainsIgnoreCase(subtype, "Piston")
                || ContainsIgnoreCase(subtype, "Rotor")
                || ContainsIgnoreCase(subtype, "ArmorPanel")
                || ContainsIgnoreCase(subtype, "HydrogenEngine"))
            {
                return true;
            }

            return false;
        }

        public static bool IsAmmo(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            if (ContainsIgnoreCase(subtype, "Ammo")
                || ContainsIgnoreCase(subtype, "Magazine")
                || ContainsIgnoreCase(subtype, "Missile")
                || ContainsIgnoreCase(subtype, "Shell")
                || ContainsIgnoreCase(subtype, "Rocket"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "NATO")
                || ContainsIgnoreCase(subtype, "Caliber")
                || ContainsIgnoreCase(subtype, "Artillery")
                || ContainsIgnoreCase(subtype, "Cannon")
                || ContainsIgnoreCase(subtype, "MassDriver")
                || ContainsIgnoreCase(subtype, "AssaultCraft"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Flare")
                || ContainsIgnoreCase(subtype, "Pistol")
                || ContainsIgnoreCase(subtype, "Rifle")
                || ContainsIgnoreCase(subtype, "Gun"))
            {
                return true;
            }

            return false;
        }

        public static bool IsTool(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            if (ContainsIgnoreCase(subtype, "PhysicalGun"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Welder")
                && !ContainsIgnoreCase(subtype, "Component"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Grinder")
                && !ContainsIgnoreCase(subtype, "Component"))
            {
                return true;
            }

            if (ContainsIgnoreCase(subtype, "Drill")
                && !ContainsIgnoreCase(subtype, "Component"))
            {
                return true;
            }

            return false;
        }

        public static bool IsBottle(string subtype)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return false;
            }

            if (ContainsIgnoreCase(subtype, "Bottle"))
            {
                return true;
            }

            return ContainsIgnoreCase(subtype, "Canister");
        }

        private static bool ContainsIgnoreCase(string haystack, string needle)
        {
            return haystack.IndexOf(needle, StrIX.C) >= 0;
        }
    }
}
