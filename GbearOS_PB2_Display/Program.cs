// Copyright (c) 2026 Garrett Wyrick.
//
// [GbearOS Component]
// Name: Program.cs (PB2_Display)
// Purpose: PB2 entry point — IGC message handling and DTO-driven LCD refresh.
// PB Association: PB2
// Dependencies: LCDRenderer, WarningFormatter, IGCParser
// Key Methods: Main, Update10, ProcessIGCMessages

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // ---------------------------------------------------------
        // Fields
        // ---------------------------------------------------------

        private LCDRenderer _lcdRenderer;
        private WarningFormatter _warningFormatter;
        private IGCParser _igcParser;

        private int _tick10 = 0;

        // ---------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _lcdRenderer = new LCDRenderer();
            _warningFormatter = new WarningFormatter();
            _igcParser = new IGCParser();

            _lcdRenderer.Init(GridTerminalSystem, Me, _warningFormatter, _igcParser);
            _igcParser.Init(this);
        }

        public void Save()
        {
        }

        // ---------------------------------------------------------
        // Main Entry Point
        // ---------------------------------------------------------

        public void Main(string argument, UpdateType updateSource)
        {
            try
            {
                if ((updateSource & UpdateType.IGC) != 0)
                {
                    ProcessIGCMessages();
                }

                if ((updateSource & UpdateType.Update10) != 0)
                {
                    Update10();
                }
            }
            catch (Exception e)
            {
                string err = "PB2 ERROR:\n" + e.ToString();
                Echo(err);
            }
        }

        // ---------------------------------------------------------
        // Update10 Loop (Display Refresh)
        // ---------------------------------------------------------

        private void Update10()
        {
            _tick10++;
            bool isFullTick = (_tick10 % 10 == 0);

            if (!_igcParser.EnableNetwork && isFullTick)
            {
                Echo("NETWORK OFFLINE");
            }

            ProcessIGCMessages();

            double secondsSinceData;
            if (_igcParser.LastPb1DataTicks == 0)
            {
                secondsSinceData = 0;
            }
            else
            {
                secondsSinceData = (DateTime.UtcNow.Ticks - _igcParser.LastPb1DataTicks) / (double)TimeSpan.TicksPerSecond;
            }

            string pbId = _igcParser.PBID ?? "";
            string telemetry = "=== " + pbId + " DISPLAY MANAGER ===\n" +
                               "Last Run: " + Runtime.LastRunTimeMs.ToString("F4") + " ms\n" +
                               "Instructions: " + Runtime.CurrentInstructionCount.ToString() + " / " + Runtime.MaxInstructionCount.ToString();
            _igcParser.SelfStatus = telemetry;

            if (secondsSinceData > 5.0 || _igcParser.LastPb1DataTicks == 0)
            {
                _lcdRenderer.RenderNoSignal(secondsSinceData);
                if (isFullTick)
                {
                    Echo(telemetry);
                    Echo("STATUS: NO SIGNAL FROM ORCHESTRATOR");
                }
            }
            else
            {
                _lcdRenderer.RenderAll(
                    _igcParser.InventorySummary,
                    _igcParser.RefineryStatus,
                    _igcParser.IceStatus,
                    _igcParser.PowerStatus,
                    _igcParser.InventoryDynamic,
                    _igcParser.Warnings,
                    isFullTick
                );
                if (isFullTick)
                {
                    Echo(telemetry);
                }
            }
        }

        private void ProcessIGCMessages()
        {
            _igcParser.ProcessMessages();
        }
    }
}
