// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: config_parser.cs
// Purpose: Defines Config defaults and ConfigParser — INI parse, validation, merge, and rewrite of PB1 Custom Data.
// PB Association: PB1
// Dependencies: Config (same file), IMyProgrammableBlock, MyIni
// Key Methods: LoadConfig, WriteDefaultsIfMissingOrInvalid

using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class Config
    {
        public Dictionary<string, double> IngotTargets = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        public double GeneratorLargeTargetIce = 50000;
        public double GeneratorSmallTargetIce = 10000;
        public double IrrigationLargeTargetIce = 15000;
        public double IrrigationSmallTargetIce = 5000;
        public double CargoReserveIce = 200000;
        public double MinimumCargoIce = 50000;

        public double ReactorLargeUraniumTarget = 200;
        public double ReactorSmallUraniumTarget = 50;

        public double ReactorLargeLower = 0.25;
        public double ReactorLargeUpper = 0.80;
        public double ReactorSmallLower = 0.20;
        public double ReactorSmallUpper = 0.70;
        public double EngineLargeLower = 0.40;
        public double EngineLargeUpper = 0.90;
        public double EngineSmallLower = 0.35;
        public double EngineSmallUpper = 0.85;

        public double BatteryChargeTarget = 0.25;
        public double BatteryDischargeTarget = 0.80;
        public bool EnablePowerAutomation = false;
        public double SolarMinimumOutput = 0.05;

        public bool EnableRefineryBalancing = true;
        public double RefineryHysteresis = 0.05;

        public string IrrigationTag = "[Irrigator]";
        public string ManualTag = "[Manual]";

        public bool ShowOres = true;
        public bool ShowIngots = true;
        public bool ShowComponents = true;
        public bool ShowAmmo = true;
        public bool ShowDynamicItems = true;

        public bool EnableDebug = false;

        /// <summary>[Network] PBID for IGC envelope (e.g. CMD-A84B).</summary>
        public string PBID = "CMD-DEFAULT";

        /// <summary>[Network] SharedKey for SenderEnvelope MAC; empty = weak authentication.</summary>
        public string NetworkSharedKey = "";

        public bool EnableNetwork = true;
    }

    public class ConfigParser
    {
        private readonly IMyProgrammableBlock _pb;
        private readonly VRage.Game.ModAPI.Ingame.Utilities.MyIni _ini = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();

        private const string SecIng = "IngotTargets";
        private const string SecIce = "IceTargets";
        private const string SecRea = "ReactorTargets";
        private const string SecBat = "BatteryThresholds";
        private const string SecRef = "RefinerySettings";
        private const string SecBlk = "BlockTags";
        private const string SecDisp = "DisplayFilters";
        private const string SecDbg = "Debug";
        private const string SecNet = "Network";
        private const string SecDoc = "Documentation";

        public ConfigParser(IMyProgrammableBlock pb)
        {
            _pb = pb;
        }

        public Config LoadConfig()
        {
            ParseOrClear();
            var c = new Config();
            ReadConfig(c);
            MergeDefaultIngotTargetsIfEmpty(c);
            ValidateAndCorrectValues(c);
            WriteConfigToIni(c);
            _pb.CustomData = _ini.ToString();
            return c;
        }

        public void WriteDefaultsIfMissingOrInvalid(Config config)
        {
            if (config == null)
            {
                return;
            }

            ParseOrClear();
            ReadConfig(config);
            MergeDefaultIngotTargetsIfEmpty(config);
            ValidateAndCorrectValues(config);
            WriteConfigToIni(config);
            _pb.CustomData = _ini.ToString();
        }

        public void TryPersistModdedIngotTargets(Config config, List<string> moddedOreSubtypes)
        {
            if (config == null || moddedOreSubtypes == null)
            {
                return;
            }

            ParseOrClear();
            ReadConfig(config);
            MergeDefaultIngotTargetsIfEmpty(config);
            bool any = false;
            if (config.IngotTargets != null)
            {
                for (int i = 0; i < moddedOreSubtypes.Count; i++)
                {
                    string ore = moddedOreSubtypes[i];
                    if (string.IsNullOrEmpty(ore) || InventoryScanner.IsVanillaOreSubtype(ore)
                        || config.IngotTargets.ContainsKey(ore)
                        || config.IngotTargets.ContainsKey("Ingot/" + ore))
                    {
                        continue;
                    }

                    if (!_ini.ContainsKey(SecIng, ore))
                    {
                        _ini.Set(SecIng, ore, 500);
                    }

                    config.IngotTargets[ore] = 500;
                    any = true;
                }
            }

            if (!any)
            {
                return;
            }

            ValidateAndCorrectValues(config);
            WriteConfigToIni(config);
            _pb.CustomData = _ini.ToString();
        }

        /// <summary>First segment of PBID (before '-'), alphanumeric only, max 3 uppercase; <paramref name="defaultIfExtractEmpty"/> if none.</summary>
        private static string ExtractPbidPrefix(string raw, string defaultIfExtractEmpty)
        {
            if (string.IsNullOrEmpty(raw))
                return defaultIfExtractEmpty;
            int dash = raw.IndexOf('-');
            string first = dash < 0 ? raw : raw.Substring(0, dash);
            string cleaned = "";
            for (int i = 0; i < first.Length && cleaned.Length < 3; i++)
            {
                char ch = first[i];
                if (char.IsLetterOrDigit(ch))
                    cleaned += char.ToUpperInvariant(ch);
            }
            return cleaned.Length > 0 ? cleaned : defaultIfExtractEmpty;
        }

        private string ComposeBoundPbid(string rawRead, string defaultPrefix)
        {
            string suffix = _pb.EntityId.ToString("X");
            suffix = suffix.Substring(Math.Max(0, suffix.Length - 4));
            string prefix = ExtractPbidPrefix(rawRead, defaultPrefix);
            return prefix + "-" + suffix;
        }

        private void ParseOrClear()
        {
            VRage.Game.ModAPI.Ingame.Utilities.MyIniParseResult r;
            if (!_ini.TryParse(_pb.CustomData ?? "", out r))
            {
                _ini.Clear();
            }
        }

        private void ReadConfig(Config c)
        {
            c.IngotTargets.Clear();
            var ks = new List<VRage.Game.ModAPI.Ingame.Utilities.MyIniKey>();
            _ini.GetKeys(SecIng, ks);
            for (int i = 0; i < ks.Count; i++)
            {
                string k = ks[i].Name;
                double v = _ini.Get(SecIng, k).ToDouble(0);
                if (v < 0)
                {
                    v = 0;
                }

                c.IngotTargets[k] = v;
            }

            c.GeneratorLargeTargetIce = ClampNn(_ini.Get(SecIce, "GeneratorLargeTargetIce").ToDouble(50000));
            c.GeneratorSmallTargetIce = ClampNn(_ini.Get(SecIce, "GeneratorSmallTargetIce").ToDouble(10000));
            c.IrrigationLargeTargetIce = ClampNn(_ini.Get(SecIce, "IrrigationLargeTargetIce").ToDouble(15000));
            c.IrrigationSmallTargetIce = ClampNn(_ini.Get(SecIce, "IrrigationSmallTargetIce").ToDouble(5000));
            c.CargoReserveIce = ClampNn(_ini.Get(SecIce, "CargoReserveIce").ToDouble(200000));
            c.MinimumCargoIce = ClampNn(_ini.Get(SecIce, "MinimumCargoIce").ToDouble(50000));

            c.ReactorLargeUraniumTarget = ClampNn(_ini.Get(SecRea, "ReactorLargeUraniumTarget").ToDouble(200));
            c.ReactorSmallUraniumTarget = ClampNn(_ini.Get(SecRea, "ReactorSmallUraniumTarget").ToDouble(50));

            c.ReactorLargeLower = Clamp01(_ini.Get(SecBat, "ReactorLargeLower").ToDouble(0.25));
            c.ReactorLargeUpper = Clamp01(_ini.Get(SecBat, "ReactorLargeUpper").ToDouble(0.80));
            c.ReactorSmallLower = Clamp01(_ini.Get(SecBat, "ReactorSmallLower").ToDouble(0.20));
            c.ReactorSmallUpper = Clamp01(_ini.Get(SecBat, "ReactorSmallUpper").ToDouble(0.70));
            c.EngineLargeLower = Clamp01(_ini.Get(SecBat, "EngineLargeLower").ToDouble(0.40));
            c.EngineLargeUpper = Clamp01(_ini.Get(SecBat, "EngineLargeUpper").ToDouble(0.90));
            c.EngineSmallLower = Clamp01(_ini.Get(SecBat, "EngineSmallLower").ToDouble(0.35));
            c.EngineSmallUpper = Clamp01(_ini.Get(SecBat, "EngineSmallUpper").ToDouble(0.85));
            c.BatteryChargeTarget = Clamp01(_ini.Get(SecBat, "BatteryChargeTarget").ToDouble(0.25));
            c.BatteryDischargeTarget = Clamp01(_ini.Get(SecBat, "BatteryDischargeTarget").ToDouble(0.80));
            c.EnablePowerAutomation = _ini.Get(SecBat, "EnablePowerAutomation").ToBoolean(false);
            c.SolarMinimumOutput = ClampNn(_ini.Get(SecBat, "SolarMinimumOutput").ToDouble(0.05));

            c.EnableRefineryBalancing = _ini.Get(SecRef, "EnableRefineryBalancing").ToBoolean(true);
            c.RefineryHysteresis = ClampNn(_ini.Get(SecRef, "RefineryHysteresis").ToDouble(0.05));

            c.IrrigationTag = _ini.Get(SecBlk, "IrrigationTag").ToString("[Irrigator]");
            c.ManualTag = _ini.Get(SecBlk, "ManualTag").ToString("[Manual]");
            if (c.IrrigationTag != null)
            {
                c.IrrigationTag = c.IrrigationTag.Trim();
            }

            if (c.ManualTag != null)
            {
                c.ManualTag = c.ManualTag.Trim();
            }

            c.ShowOres = _ini.Get(SecDisp, "ShowOres").ToBoolean(true);
            c.ShowIngots = _ini.Get(SecDisp, "ShowIngots").ToBoolean(true);
            c.ShowComponents = _ini.Get(SecDisp, "ShowComponents").ToBoolean(true);
            c.ShowAmmo = _ini.Get(SecDisp, "ShowAmmo").ToBoolean(true);
            c.ShowDynamicItems = _ini.Get(SecDisp, "ShowDynamicItems").ToBoolean(true);

            c.EnableDebug = _ini.Get(SecDbg, "EnableDebug").ToBoolean(false);

            string rawPbid = _ini.Get(SecNet, "PBID").ToString("");
            if (string.IsNullOrWhiteSpace(rawPbid))
                rawPbid = _ini.Get(SecNet, "SenderId").ToString("");
            if (rawPbid != null)
                rawPbid = rawPbid.Trim();
            c.PBID = ComposeBoundPbid(rawPbid ?? "", "CMD");
            c.NetworkSharedKey = _ini.Get(SecNet, "SharedKey").ToString("");
            c.EnableNetwork = _ini.Get(SecNet, "EnableNetwork").ToBoolean(true);

            if (c.NetworkSharedKey != null)
            {
                c.NetworkSharedKey = c.NetworkSharedKey.Trim();
            }
        }

        private void ValidateAndCorrectValues(Config config)
        {
            var ingotKeys = new List<string>(config.IngotTargets.Keys);
            for (int ik = 0; ik < ingotKeys.Count; ik++)
            {
                string ingotKey = ingotKeys[ik];
                if (config.IngotTargets[ingotKey] < 0)
                {
                    config.IngotTargets[ingotKey] = 0;
                }
            }

            config.GeneratorLargeTargetIce = ClampNn(config.GeneratorLargeTargetIce);
            config.GeneratorSmallTargetIce = ClampNn(config.GeneratorSmallTargetIce);
            config.IrrigationLargeTargetIce = ClampNn(config.IrrigationLargeTargetIce);
            config.IrrigationSmallTargetIce = ClampNn(config.IrrigationSmallTargetIce);
            config.CargoReserveIce = ClampNn(config.CargoReserveIce);
            config.MinimumCargoIce = ClampNn(config.MinimumCargoIce);
            config.ReactorLargeUraniumTarget = ClampNn(config.ReactorLargeUraniumTarget);
            config.ReactorSmallUraniumTarget = ClampNn(config.ReactorSmallUraniumTarget);
            config.ReactorLargeLower = Clamp01(config.ReactorLargeLower);
            config.ReactorLargeUpper = Clamp01(config.ReactorLargeUpper);
            config.ReactorSmallLower = Clamp01(config.ReactorSmallLower);
            config.ReactorSmallUpper = Clamp01(config.ReactorSmallUpper);
            config.EngineLargeLower = Clamp01(config.EngineLargeLower);
            config.EngineLargeUpper = Clamp01(config.EngineLargeUpper);
            config.EngineSmallLower = Clamp01(config.EngineSmallLower);
            config.EngineSmallUpper = Clamp01(config.EngineSmallUpper);
            config.BatteryChargeTarget = Clamp01(config.BatteryChargeTarget);
            config.BatteryDischargeTarget = Clamp01(config.BatteryDischargeTarget);
            config.SolarMinimumOutput = ClampNn(config.SolarMinimumOutput);
            config.RefineryHysteresis = ClampNn(config.RefineryHysteresis);

            if (config.NetworkSharedKey != null)
            {
                config.NetworkSharedKey = config.NetworkSharedKey.Trim();
            }

            config.PBID = ComposeBoundPbid(config.PBID == null ? "" : config.PBID.Trim(), "CMD");
        }

        private void WriteConfigToIni(Config c)
        {
            _ini.Clear();

            _ini.Set(SecDoc, "ConfigurationManual", "docs/configuration.md");
            _ini.SetComment(SecDoc, "ConfigurationManual", "Full Custom Data reference for PB1/PB2 — open in the GbearOS repository (see also docs/architecture/user_config_system.md).");

            _ini.Set(SecNet, "EnableNetwork", c.EnableNetwork);
            _ini.SetComment(SecNet, "EnableNetwork", "See docs/configuration.md — set false for offline mode (no IGC DTO send).");
            _ini.Set(SecNet, "PBID", c.PBID ?? "CMD-0000");
            _ini.SetComment(SecNet, "PBID", "Format: ABC-XXXX. You may change the 3-letter prefix. The 4-character suffix is locked to this block's ID and will auto-reset if changed.");
            _ini.Set(SecNet, "SharedKey", c.NetworkSharedKey ?? "");
            _ini.SetComment(SecNet, "SharedKey", "MAC secret; must match PB2.");

            _ini.Set(SecIce, "GeneratorLargeTargetIce", c.GeneratorLargeTargetIce);
            _ini.Set(SecIce, "GeneratorSmallTargetIce", c.GeneratorSmallTargetIce);
            _ini.Set(SecIce, "IrrigationLargeTargetIce", c.IrrigationLargeTargetIce);
            _ini.Set(SecIce, "IrrigationSmallTargetIce", c.IrrigationSmallTargetIce);
            _ini.Set(SecIce, "CargoReserveIce", c.CargoReserveIce);
            _ini.SetComment(SecIce, "CargoReserveIce", "Triggers LOW ICE warning.");
            _ini.Set(SecIce, "MinimumCargoIce", c.MinimumCargoIce);
            _ini.SetComment(SecIce, "MinimumCargoIce", "Cargo ice above: to gen/irr.");

            _ini.Set(SecRea, "ReactorLargeUraniumTarget", c.ReactorLargeUraniumTarget);
            _ini.Set(SecRea, "ReactorSmallUraniumTarget", c.ReactorSmallUraniumTarget);

            _ini.Set(SecBat, "ReactorLargeLower", c.ReactorLargeLower);
            _ini.Set(SecBat, "ReactorLargeUpper", c.ReactorLargeUpper);
            _ini.Set(SecBat, "ReactorSmallLower", c.ReactorSmallLower);
            _ini.Set(SecBat, "ReactorSmallUpper", c.ReactorSmallUpper);
            _ini.Set(SecBat, "EngineLargeLower", c.EngineLargeLower);
            _ini.Set(SecBat, "EngineLargeUpper", c.EngineLargeUpper);
            _ini.Set(SecBat, "EngineSmallLower", c.EngineSmallLower);
            _ini.Set(SecBat, "EngineSmallUpper", c.EngineSmallUpper);
            _ini.Set(SecBat, "BatteryChargeTarget", c.BatteryChargeTarget);
            _ini.SetComment(SecBat, "BatteryChargeTarget", "Recharge below this fraction.");
            _ini.Set(SecBat, "BatteryDischargeTarget", c.BatteryDischargeTarget);
            _ini.SetComment(SecBat, "BatteryDischargeTarget", "Auto above this fraction.");
            _ini.Set(SecBat, "EnablePowerAutomation", c.EnablePowerAutomation);
            _ini.SetComment(SecBat, "EnablePowerAutomation", "Solar-driven reactor/engine toggle.");
            _ini.Set(SecBat, "SolarMinimumOutput", c.SolarMinimumOutput);
            _ini.SetComment(SecBat, "SolarMinimumOutput", "Backup if solar below this MW.");

            _ini.Set(SecRef, "EnableRefineryBalancing", c.EnableRefineryBalancing);
            _ini.SetComment(SecRef, "EnableRefineryBalancing", "Script queues; off = vanilla.");
            _ini.Set(SecRef, "RefineryHysteresis", c.RefineryHysteresis);
            _ini.SetComment(SecRef, "RefineryHysteresis", "Top-ore switch hysteresis.");

            _ini.Set(SecBlk, "IrrigationTag", c.IrrigationTag ?? "[Irrigator]");
            _ini.SetComment(SecBlk, "IrrigationTag", "O2/H2 farm ice supply tag.");
            _ini.Set(SecBlk, "ManualTag", c.ManualTag ?? "[Manual]");
            _ini.SetComment(SecBlk, "ManualTag", "Ignore tagged blocks.");

            _ini.Set(SecDisp, "ShowOres", c.ShowOres);
            _ini.Set(SecDisp, "ShowIngots", c.ShowIngots);
            _ini.Set(SecDisp, "ShowComponents", c.ShowComponents);
            _ini.Set(SecDisp, "ShowAmmo", c.ShowAmmo);
            _ini.Set(SecDisp, "ShowDynamicItems", c.ShowDynamicItems);

            _ini.Set(SecDbg, "EnableDebug", c.EnableDebug);

            foreach (var kv in c.IngotTargets)
            {
                _ini.Set(SecIng, kv.Key, kv.Value);
            }
        }

        private static void MergeDefaultIngotTargetsIfEmpty(Config config)
        {
            if (config == null || config.IngotTargets == null || config.IngotTargets.Count > 0)
            {
                return;
            }

            AddDefaultIngotTargetsTo(config.IngotTargets);
        }

        private static void AddDefaultIngotTargetIfMissing(Dictionary<string, double> targets, string key, double value)
        {
            if (!targets.ContainsKey(key))
            {
                targets[key] = value;
            }
        }

        private static void AddDefaultIngotTargetsTo(Dictionary<string, double> targets)
        {
            AddDefaultIngotTargetIfMissing(targets, "Iron", 125000);
            AddDefaultIngotTargetIfMissing(targets, "Nickel", 25800);
            AddDefaultIngotTargetIfMissing(targets, "Silicon", 17500);
            AddDefaultIngotTargetIfMissing(targets, "Cobalt", 14800);
            AddDefaultIngotTargetIfMissing(targets, "Silver", 6100);
            AddDefaultIngotTargetIfMissing(targets, "Gold", 9000);
            AddDefaultIngotTargetIfMissing(targets, "Magnesium", 15000);
            AddDefaultIngotTargetIfMissing(targets, "Platinum", 4500);
            AddDefaultIngotTargetIfMissing(targets, "Uranium", 2600);
            AddDefaultIngotTargetIfMissing(targets, "Gravel", 22500);
        }

        private static double ClampNn(double v)
        {
            return v < 0 ? 0 : v;
        }

        private static double Clamp01(double v)
        {
            if (v < 0)
            {
                return 0;
            }

            return v > 1 ? 1 : v;
        }
    }
}
