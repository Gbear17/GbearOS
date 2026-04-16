// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: igc_serializer.cs
// Purpose: PB1+PB2 shared — semicolon positional Serialize for PB1→PB2 DTO bodies (protocol v1).
// PB Association: Shared
// Dependencies: InventorySummaryDTO, RefineryStatusDTO, IceStatusDTO, PowerStatusDTO, InventoryDynamicDTO, WarningDTO
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
    /// Positional semicolon-separated serialization.
    /// PB2-only generic deserialize lives in GbearOS_PB2_Display/igc_serializer_deserialize.cs (typeof-routed).
    /// </summary>
    public static partial class IGCSerializer
    {
        private const string ProtocolVersion = "1";

        public static string Serialize(object dto)
        {
            if (dto == null)
                return string.Empty;

            Type t = dto.GetType();
            if (t == typeof(InventorySummaryDTO))
                return SerializeInventorySummary((InventorySummaryDTO)dto);
            if (t == typeof(RefineryStatusDTO))
                return SerializeRefineryStatus((RefineryStatusDTO)dto);
            if (t == typeof(IceStatusDTO))
                return SerializeIceStatus((IceStatusDTO)dto);
            if (t == typeof(PowerStatusDTO))
                return SerializePowerStatus((PowerStatusDTO)dto);
            if (t == typeof(InventoryDynamicDTO))
                return SerializeInventoryDynamic((InventoryDynamicDTO)dto);
            if (t == typeof(WarningDTO))
                return SerializeWarning((WarningDTO)dto);
            return string.Empty;
        }

        private static string SerializeInventorySummary(InventorySummaryDTO d)
        {
            StringBuilder sb = new StringBuilder(512);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(d.ironOre).Append(';');
            sb.Append(d.nickelOre).Append(';');
            sb.Append(d.cobaltOre).Append(';');
            sb.Append(d.siliconOre).Append(';');
            sb.Append(d.magnesiumOre).Append(';');
            sb.Append(d.silverOre).Append(';');
            sb.Append(d.goldOre).Append(';');
            sb.Append(d.platinumOre).Append(';');
            sb.Append(d.uraniumOre).Append(';');
            sb.Append(d.stoneOre).Append(';');
            sb.Append(d.iceOre).Append(';');
            sb.Append(d.ironIngot).Append(';');
            sb.Append(d.nickelIngot).Append(';');
            sb.Append(d.cobaltIngot).Append(';');
            sb.Append(d.siliconIngot).Append(';');
            sb.Append(d.magnesiumPowder).Append(';');
            sb.Append(d.silverIngot).Append(';');
            sb.Append(d.goldIngot).Append(';');
            sb.Append(d.platinumIngot).Append(';');
            sb.Append(d.uraniumIngot).Append(';');
            sb.Append(d.componentsTotal).Append(';');
            sb.Append(d.ammoTotal).Append(';');
            sb.Append(d.toolsTotal).Append(';');
            sb.Append(d.bottlesTotal).Append(';');
            sb.Append(d.cargoUsed).Append(';');
            sb.Append(d.cargoMax).Append(';');
            sb.Append(d.cargoPercent);
            return sb.ToString();
        }

        private static string SerializeRefineryStatus(RefineryStatusDTO d)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(SerializeStringArray(d.refineryNames)).Append(';');
            sb.Append(SerializeStringArray(d.currentOre)).Append(';');
            sb.Append(SerializeFloatArray(d.oreAmounts)).Append(';');
            sb.Append(SerializeStringArray(d.outputIngot)).Append(';');
            sb.Append(SerializeFloatArray(d.outputAmounts)).Append(';');
            sb.Append(SerializeBoolArray(d.isWorking)).Append(';');
            sb.Append(SerializeBoolArray(d.hasOre)).Append(';');
            sb.Append(d.priorityLine1 != null ? d.priorityLine1 : string.Empty).Append(';');
            sb.Append(d.priorityLine2 != null ? d.priorityLine2 : string.Empty);
            return sb.ToString();
        }

        private static string SerializeIceStatus(IceStatusDTO d)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(d.totalIce).Append(';');
            sb.Append(d.generatorIce).Append(';');
            sb.Append(d.irrigationIce).Append(';');
            sb.Append(d.cargoIce).Append(';');
            sb.Append(d.pctTotal).Append(';');
            sb.Append(d.pctGen).Append(';');
            sb.Append(d.pctIrr).Append(';');
            sb.Append(d.pctCargo).Append(';');
            sb.Append(d.generatorCount).Append(';');
            sb.Append(d.irrigationCount).Append(';');
            sb.Append(d.lowIce ? '1' : '0');
            return sb.ToString();
        }

        private static string SerializePowerStatus(PowerStatusDTO d)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(d.batteryStored).Append(';');
            sb.Append(d.batteryMax).Append(';');
            sb.Append(d.batteryInput).Append(';');
            sb.Append(d.batteryOutput).Append(';');
            sb.Append(d.batteryMaxInput).Append(';');
            sb.Append(d.batteryMaxOutput).Append(';');
            sb.Append(d.reactorMaxOutput).Append(';');
            sb.Append(d.engineMaxOutput).Append(';');
            sb.Append(d.reactorOutput).Append(';');
            sb.Append(d.engineOutput).Append(';');
            sb.Append(d.batteryCount).Append(';');
            sb.Append(d.reactorCount).Append(';');
            sb.Append(d.engineCount).Append(';');
            sb.Append(d.lowPower ? '1' : '0');
            return sb.ToString();
        }

        private static string SerializeInventoryDynamic(InventoryDynamicDTO d)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(SerializeStringArray(d.itemNames)).Append(';');
            sb.Append(SerializeFloatArray(d.itemAmounts)).Append(';');
            sb.Append(SerializeStringArray(d.itemTypes));
            return sb.ToString();
        }

        private static string SerializeWarning(WarningDTO d)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append(ProtocolVersion).Append(';');
            sb.Append(d.lowIce ? '1' : '0').Append(';');
            sb.Append(d.lowPower ? '1' : '0').Append(';');
            sb.Append(d.cargoFull ? '1' : '0').Append(';');
            sb.Append(d.noRefineries ? '1' : '0').Append(';');
            sb.Append(d.refineryStalled ? '1' : '0').Append(';');
            sb.Append(d.assemblerStalled ? '1' : '0').Append(';');
            sb.Append(d.activeCode).Append(';');
            sb.Append(d.activeMessage != null ? d.activeMessage : string.Empty).Append(';');
            sb.Append(d.isNominal ? '1' : '0');
            return sb.ToString();
        }

        private static string SerializeStringArray(string[] a)
        {
            if (a == null || a.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder(a.Length * 8);
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    sb.Append('|');
                AppendEscapedString(sb, a[i]);
            }
            return sb.ToString();
        }

        private static string SerializeFloatArray(float[] a)
        {
            if (a == null || a.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder(a.Length * 12);
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    sb.Append('|');
                sb.Append(a[i].ToString());
            }
            return sb.ToString();
        }

        private static string SerializeBoolArray(bool[] a)
        {
            if (a == null || a.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder(a.Length * 2);
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    sb.Append('|');
                sb.Append(a[i] ? '1' : '0');
            }
            return sb.ToString();
        }

        private static void AppendEscapedString(StringBuilder sb, string s)
        {
            if (s == null)
                return;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\\')
                {
                    sb.Append('\\');
                    sb.Append('\\');
                }
                else if (c == '|')
                {
                    sb.Append('\\');
                    sb.Append('|');
                }
                else
                    sb.Append(c);
            }
        }
    }
}
