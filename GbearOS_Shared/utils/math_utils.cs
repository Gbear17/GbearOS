// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: math_utils.cs
// Purpose: Small numeric helpers — clamp, lerp, safe division, and rounded float strings for UI and ratios.
// PB Association: Shared
// Dependencies: None
// Key Methods: Clamp, SafeDiv

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static class MathUtils
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                float tmp = min;
                min = max;
                max = tmp;
            }
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float SafeDiv(float numerator, float denominator)
        {
            if (denominator == 0f)
                return 0f;
            return numerator / denominator;
        }

        public static string RoundTo(float value, int decimals)
        {
            if (decimals < 0)
                decimals = 0;
            else if (decimals > 15)
                decimals = 15;

            switch (decimals)
            {
                case 0: return value.ToString("F0");
                case 1: return value.ToString("F1");
                case 2: return value.ToString("F2");
                case 3: return value.ToString("F3");
                case 4: return value.ToString("F4");
                case 5: return value.ToString("F5");
                case 6: return value.ToString("F6");
                case 7: return value.ToString("F7");
                case 8: return value.ToString("F8");
                case 9: return value.ToString("F9");
                case 10: return value.ToString("F10");
                case 11: return value.ToString("F11");
                case 12: return value.ToString("F12");
                case 13: return value.ToString("F13");
                case 14: return value.ToString("F14");
                default: return value.ToString("F15");
            }
        }
    }
}
