// Copyright (c) 2026 Garrett Wyrick.
//
// [GbearOS Component]
// Name: pb2_module_registry.cs
// Purpose: Central tag/command parsing + module registry for PB2 display modules (static dictionary; initialized once).
// PB Association: PB2
// Dependencies: IDisplayComponent, LayoutManager
// Key Methods: InitModuleRegistry

using System;
using System.Collections.Generic;

namespace IngameScript
{
    public partial class LCDRenderer
    {
        private struct ModuleSpec
        {
            public string Key;
            public string SubheaderLabel;
            public Func<LCDRenderer, IDisplayComponent> Create;
        }

        private static readonly Dictionary<string, CommandType> _commandTypeByToken =
            new Dictionary<string, CommandType>(StringComparer.OrdinalIgnoreCase)
            {
                { "HEAD", CommandType.Head },
                { "INV", CommandType.Inv },
                { "REF", CommandType.Ref },
                { "PWR", CommandType.Pwr },
                { "ICE", CommandType.Ice },
                { "WARN", CommandType.Warn },
                { "STATUS", CommandType.Status },
                { LayoutManager.CmdCol, CommandType.Col },
            };

        private static readonly Dictionary<CommandType, ModuleSpec> _moduleSpecByType =
            new Dictionary<CommandType, ModuleSpec>
            {
                { CommandType.Inv, new ModuleSpec { Key = "INV", SubheaderLabel = "INVENTORY", Create = CreateInventoryModule } },
                { CommandType.Ref, new ModuleSpec { Key = "REF", SubheaderLabel = "REFINERY STATUS", Create = CreateRefineryModule } },
                { CommandType.Pwr, new ModuleSpec { Key = "PWR", SubheaderLabel = "POWER GRID STATUS", Create = CreatePowerModule } },
                { CommandType.Ice, new ModuleSpec { Key = "ICE", SubheaderLabel = "ICE STATUS", Create = CreateIceModule } },
                { CommandType.Warn, new ModuleSpec { Key = "WARN", SubheaderLabel = "WARNING STATUS", Create = CreateWarningModule } },
                { CommandType.Status, new ModuleSpec { Key = "STATUS", SubheaderLabel = "SYSTEM STATUS", Create = CreateStatusModule } },
            };

        private static IDisplayComponent CreateInventoryModule(LCDRenderer host) { return new InventoryDisplayModule(host); }
        private static IDisplayComponent CreateRefineryModule(LCDRenderer host) { return new RefineryDisplayModule(host); }
        private static IDisplayComponent CreatePowerModule(LCDRenderer host) { return new PowerDisplayModule(host); }
        private static IDisplayComponent CreateIceModule(LCDRenderer host) { return new IceDisplayModule(host); }
        private static IDisplayComponent CreateWarningModule(LCDRenderer host) { return new WarningDisplayModule(host); }
        private static IDisplayComponent CreateStatusModule(LCDRenderer host) { return new StatusDisplayModule(host); }

        private static CommandType ParseCommandTypeString(string typeToken)
        {
            if (string.IsNullOrEmpty(typeToken))
                return CommandType.Unknown;
            CommandType t;
            return _commandTypeByToken.TryGetValue(typeToken.Trim(), out t) ? t : CommandType.Unknown;
        }

        public void InitModuleRegistry()
        {
            if (_displayModuleByTag == null)
                _displayModuleByTag = new Dictionary<string, IDisplayComponent>(StringComparer.OrdinalIgnoreCase);
            else
                _displayModuleByTag.Clear();

            foreach (var kvp in _moduleSpecByType)
            {
                var spec = kvp.Value;
                if (string.IsNullOrEmpty(spec.Key) || spec.Create == null)
                    continue;
                _displayModuleByTag[spec.Key] = spec.Create(this);
            }
        }

        private static string ModuleKeyForCommand(ref DisplayCommand cmd)
        {
            if (cmd.TypeEnum == CommandType.Unknown)
                return cmd.UnknownKind;
            ModuleSpec spec;
            return _moduleSpecByType.TryGetValue(cmd.TypeEnum, out spec) ? spec.Key : null;
        }

        private static string SubheaderLabelForCommand(CommandType t, string unknownKind)
        {
            if (t == CommandType.Unknown)
                return unknownKind != null ? unknownKind : "";
            ModuleSpec spec;
            return _moduleSpecByType.TryGetValue(t, out spec) ? spec.SubheaderLabel : "";
        }

        // NOTE: We intentionally do not parse module tags from LCD Custom Name.
        // Only panels with "[GbearOS]" in Custom Name are processed, and module commands come from Custom Data.
    }
}
