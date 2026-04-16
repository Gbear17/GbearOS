// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: igc_serializer_deserialize.cs
// Purpose: PB2-only partial — Deserialize path for IGC DTO bodies (pairs with GbearOS_Shared/igc/igc_serializer.cs).
// PB Association: PB2
// Dependencies: Same DTO set as IGCSerializer
// Key Methods: Deserialize generic

using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public static partial class IGCSerializer
    {
        /// <summary>Routes by compile-time <see cref="Type"/> — safe under MDK2 minification (no runtime type name strings).</summary>
        public static T Deserialize<T>(string data)
        {
            try
            {
                if (typeof(T) == typeof(InventorySummaryDTO))
                    return (T)(object)DeserializeInventorySummary(data);
                if (typeof(T) == typeof(RefineryStatusDTO))
                    return (T)(object)DeserializeRefineryStatus(data);
                if (typeof(T) == typeof(IceStatusDTO))
                    return (T)(object)DeserializeIceStatus(data);
                if (typeof(T) == typeof(PowerStatusDTO))
                    return (T)(object)DeserializePowerStatus(data);
                if (typeof(T) == typeof(InventoryDynamicDTO))
                    return (T)(object)DeserializeInventoryDynamic(data);
                if (typeof(T) == typeof(WarningDTO))
                    return (T)(object)DeserializeWarning(data);
            }
            catch
            {
            }
            return default(T);
        }

        private static InventorySummaryDTO DeserializeInventorySummary(string data)
        {
            InventorySummaryDTO d = new InventorySummaryDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new InventorySummaryDTO();
            if (p.Length > 1) float.TryParse(p[1], out d.ironOre);
            if (p.Length > 2) float.TryParse(p[2], out d.nickelOre);
            if (p.Length > 3) float.TryParse(p[3], out d.cobaltOre);
            if (p.Length > 4) float.TryParse(p[4], out d.siliconOre);
            if (p.Length > 5) float.TryParse(p[5], out d.magnesiumOre);
            if (p.Length > 6) float.TryParse(p[6], out d.silverOre);
            if (p.Length > 7) float.TryParse(p[7], out d.goldOre);
            if (p.Length > 8) float.TryParse(p[8], out d.platinumOre);
            if (p.Length > 9) float.TryParse(p[9], out d.uraniumOre);
            if (p.Length > 10) float.TryParse(p[10], out d.stoneOre);
            if (p.Length > 11) float.TryParse(p[11], out d.iceOre);
            if (p.Length > 12) float.TryParse(p[12], out d.ironIngot);
            if (p.Length > 13) float.TryParse(p[13], out d.nickelIngot);
            if (p.Length > 14) float.TryParse(p[14], out d.cobaltIngot);
            if (p.Length > 15) float.TryParse(p[15], out d.siliconIngot);
            if (p.Length > 16) float.TryParse(p[16], out d.magnesiumPowder);
            if (p.Length > 17) float.TryParse(p[17], out d.silverIngot);
            if (p.Length > 18) float.TryParse(p[18], out d.goldIngot);
            if (p.Length > 19) float.TryParse(p[19], out d.platinumIngot);
            if (p.Length > 20) float.TryParse(p[20], out d.uraniumIngot);
            if (p.Length > 21) float.TryParse(p[21], out d.componentsTotal);
            if (p.Length > 22) float.TryParse(p[22], out d.ammoTotal);
            if (p.Length > 23) float.TryParse(p[23], out d.toolsTotal);
            if (p.Length > 24) float.TryParse(p[24], out d.bottlesTotal);
            if (p.Length > 25) float.TryParse(p[25], out d.cargoUsed);
            if (p.Length > 26) float.TryParse(p[26], out d.cargoMax);
            if (p.Length > 27) float.TryParse(p[27], out d.cargoPercent);
            return d;
        }

        private static RefineryStatusDTO DeserializeRefineryStatus(string data)
        {
            RefineryStatusDTO d = new RefineryStatusDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new RefineryStatusDTO();
            if (p.Length > 1) d.refineryNames = DeserializeStringArray(p[1]);
            if (p.Length > 2) d.currentOre = DeserializeStringArray(p[2]);
            if (p.Length > 3) d.oreAmounts = DeserializeFloatArray(p[3]);
            if (p.Length > 4) d.outputIngot = DeserializeStringArray(p[4]);
            if (p.Length > 5) d.outputAmounts = DeserializeFloatArray(p[5]);
            if (p.Length > 6) d.isWorking = DeserializeBoolArray(p[6]);
            if (p.Length > 7) d.hasOre = DeserializeBoolArray(p[7]);
            if (p.Length > 8) d.priorityLine1 = p[8];
            if (p.Length > 9) d.priorityLine2 = p[9];
            return d;
        }

        private static IceStatusDTO DeserializeIceStatus(string data)
        {
            IceStatusDTO d = new IceStatusDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new IceStatusDTO();
            if (p.Length > 1) float.TryParse(p[1], out d.totalIce);
            if (p.Length > 2) float.TryParse(p[2], out d.generatorIce);
            if (p.Length > 3) float.TryParse(p[3], out d.irrigationIce);
            if (p.Length > 4) float.TryParse(p[4], out d.cargoIce);
            if (p.Length > 5) float.TryParse(p[5], out d.pctTotal);
            if (p.Length > 6) float.TryParse(p[6], out d.pctGen);
            if (p.Length > 7) float.TryParse(p[7], out d.pctIrr);
            if (p.Length > 8) float.TryParse(p[8], out d.pctCargo);
            int iv;
            if (p.Length > 9 && int.TryParse(p[9], out iv)) d.generatorCount = iv;
            if (p.Length > 10 && int.TryParse(p[10], out iv)) d.irrigationCount = iv;
            if (p.Length > 11) d.lowIce = ParseBool(p[11]);
            return d;
        }

        private static PowerStatusDTO DeserializePowerStatus(string data)
        {
            PowerStatusDTO d = new PowerStatusDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new PowerStatusDTO();
            if (p.Length > 1) float.TryParse(p[1], out d.batteryStored);
            if (p.Length > 2) float.TryParse(p[2], out d.batteryMax);
            if (p.Length > 3) float.TryParse(p[3], out d.batteryInput);
            if (p.Length > 4) float.TryParse(p[4], out d.batteryOutput);
            if (p.Length > 5) float.TryParse(p[5], out d.batteryMaxInput);
            if (p.Length > 6) float.TryParse(p[6], out d.batteryMaxOutput);
            if (p.Length > 7) float.TryParse(p[7], out d.reactorMaxOutput);
            if (p.Length > 8) float.TryParse(p[8], out d.engineMaxOutput);
            if (p.Length > 9) float.TryParse(p[9], out d.reactorOutput);
            if (p.Length > 10) float.TryParse(p[10], out d.engineOutput);
            int ic;
            if (p.Length > 11 && int.TryParse(p[11], out ic)) d.batteryCount = ic;
            if (p.Length > 12 && int.TryParse(p[12], out ic)) d.reactorCount = ic;
            if (p.Length > 13 && int.TryParse(p[13], out ic)) d.engineCount = ic;
            if (p.Length > 14) d.lowPower = ParseBool(p[14]);
            return d;
        }

        private static InventoryDynamicDTO DeserializeInventoryDynamic(string data)
        {
            InventoryDynamicDTO d = new InventoryDynamicDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new InventoryDynamicDTO();
            if (p.Length > 1) d.itemNames = DeserializeStringArray(p[1]);
            if (p.Length > 2) d.itemAmounts = DeserializeFloatArray(p[2]);
            if (p.Length > 3) d.itemTypes = DeserializeStringArray(p[3]);
            return d;
        }

        private static WarningDTO DeserializeWarning(string data)
        {
            WarningDTO d = new WarningDTO();
            if (string.IsNullOrEmpty(data))
                return d;
            string[] p = data.Split(';');
            if (p.Length == 0 || p[0] != ProtocolVersion)
                return new WarningDTO();
            if (p.Length > 1) d.lowIce = ParseBool(p[1]);
            if (p.Length > 2) d.lowPower = ParseBool(p[2]);
            if (p.Length > 3) d.cargoFull = ParseBool(p[3]);
            if (p.Length > 4) d.noRefineries = ParseBool(p[4]);
            if (p.Length > 5) d.refineryStalled = ParseBool(p[5]);
            if (p.Length > 6) d.assemblerStalled = ParseBool(p[6]);
            if (p.Length > 7)
            {
                int code;
                if (int.TryParse(p[7], out code))
                    d.activeCode = code;
            }
            if (p.Length > 8) d.activeMessage = p[8];
            if (p.Length > 9) d.isNominal = ParseBool(p[9]);
            return d;
        }

        private static bool ParseBool(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            if (s[0] == '1' && s.Length == 1)
                return true;
            if (s.Length == 4 && (s[0] == 't' || s[0] == 'T')
                && (s[1] == 'r' || s[1] == 'R')
                && (s[2] == 'u' || s[2] == 'U')
                && (s[3] == 'e' || s[3] == 'E'))
                return true;
            return false;
        }

        private static string[] DeserializeStringArray(string s)
        {
            if (s == null || s.Length == 0)
                return new string[0];
            int n = CountEscapedPipeSegments(s);
            string[] parts = new string[n];
            FillEscapedPipeSegments(s, parts);
            return parts;
        }

        private static float[] DeserializeFloatArray(string s)
        {
            if (s == null || s.Length == 0)
                return new float[0];
            int n = CountRawPipeSegments(s);
            float[] result = new float[n];
            int seg = 0;
            int start = 0;
            for (int i = 0; i <= s.Length; i++)
            {
                if (i == s.Length || s[i] == '|')
                {
                    int len = i - start;
                    string part = len > 0 ? s.Substring(start, len) : string.Empty;
                    float.TryParse(part, out result[seg]);
                    seg++;
                    start = i + 1;
                }
            }
            return result;
        }

        private static bool[] DeserializeBoolArray(string s)
        {
            if (s == null || s.Length == 0)
                return new bool[0];
            int n = CountRawPipeSegments(s);
            bool[] result = new bool[n];
            int seg = 0;
            int start = 0;
            for (int i = 0; i <= s.Length; i++)
            {
                if (i == s.Length || s[i] == '|')
                {
                    int len = i - start;
                    string part = len > 0 ? s.Substring(start, len) : string.Empty;
                    result[seg] = ParseBool(part);
                    seg++;
                    start = i + 1;
                }
            }
            return result;
        }

        private static int CountEscapedPipeSegments(string s)
        {
            int count = 1;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    i++;
                    continue;
                }
                if (s[i] == '|')
                    count++;
            }
            return count;
        }

        private static void FillEscapedPipeSegments(string s, string[] dest)
        {
            StringBuilder sb = new StringBuilder(32);
            int outIdx = 0;
            int i = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (c == '\\' && i + 1 < s.Length)
                {
                    char n = s[i + 1];
                    if (n == '\\' || n == '|')
                        sb.Append(n);
                    else
                    {
                        sb.Append('\\');
                        sb.Append(n);
                    }
                    i += 2;
                }
                else if (c == '|')
                {
                    dest[outIdx++] = sb.ToString();
                    sb.Length = 0;
                    i++;
                }
                else
                {
                    sb.Append(c);
                    i++;
                }
            }
            dest[outIdx++] = sb.ToString();
        }

        private static int CountRawPipeSegments(string s)
        {
            int count = 1;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '|')
                    count++;
            }
            return count;
        }
    }
}

