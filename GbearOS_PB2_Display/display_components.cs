// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: display_components.cs
// Purpose: Display component contract for modular PB2 UI (measure + draw in a viewport).
// PB Association: PB2
// Dependencies: Sandbox.ModAPI.Ingame, VRageMath
// Key Methods: — (IDisplayComponent)

using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    /// <summary>Modular LCD fragment: reports height and draws into a sprite viewport.</summary>
    public interface IDisplayComponent
    {
        /// <summary>Returns the height this component will consume within <paramref name="bounds"/>.</summary>
        float Measure(
            LCDRenderer host,
            VRageMath.Vector2 panelSize,
            VRageMath.RectangleF bounds,
            string filter,
            InventorySummaryDTO inventory,
            InventoryDynamicDTO dynamicItems,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            WarningDTO warnings);

        /// <summary>Draws the component within <paramref name="viewport"/> (panel Y scale, column X clip).</summary>
        void Draw(
            LCDRenderer host,
            MySpriteDrawFrame frame,
            VRageMath.Vector2 panelSize,
            VRageMath.RectangleF viewport,
            string filter,
            float yTop,
            float clipTop,
            float clipBottom,
            InventorySummaryDTO inventory,
            InventoryDynamicDTO dynamicItems,
            RefineryStatusDTO refineryStatus,
            IceStatusDTO ice,
            PowerStatusDTO power,
            WarningDTO warnings);
    }
}
