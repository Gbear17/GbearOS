> **AI Summary:** Explains how PB2 preserves virtual panel scroll state when the LCD block list is rebuilt from the grid.

# Scroll state persistence across panel list rescans

## Purpose

Explain how **virtual** **`[GbearOS]`** panels keep scroll animation state in **`PanelEntry`** (`ScrollPos`, `TargetScrollPos`, `ScrollPauseTicks`, `NeedsRedraw`). When the **panel list** is rebuilt from `GridTerminalSystem.GetBlocksOfType`, naïve reinitialization would **zero** those fields and cause **scroll amnesia**. The **PB2 LCD renderer** reconnects state by panel reference across rescans.

---

## 1. Rescan cadence

- `RefreshPanelsIfNeeded` runs at the start of every `RenderAll`.
- While `_refreshCountdown > 0`, the method **decrements** and **returns** without scanning.
- When the counter reaches **0**, the method:
  - Sets `_refreshCountdown = 100` for the **next** interval.
  - Refreshes `_tmpPanels`, copies **old** `_panels` into scratch, clears `_panels`, and repopulates from current grid query.

Initialization sets `_refreshCountdown = 100` so the first scan is also delayed. With PB2 on **`Update10`**, **100** skipped passes between full rescans approximates **~10 s** between rescans under nominal game tick rates (exact wall time varies with simulation load).

---

## 2. Old-panel scratch recovery

**Before `_panels.Clear()`:** copy every existing `PanelEntry` into **`_oldPanelsScratch`**.

**After** allocating a new `PanelEntry e` for grid panel `p`:

1. Default: `ScrollPos = 0`, `TargetScrollPos = 0`, `ScrollPauseTicks = 0`, `NeedsRedraw = false`.
2. **Recovery loop:** for each entry in `_oldPanelsScratch`, if `old.Panel == p` (**reference equality** on `IMyTextPanel`), copy:
   - `ScrollPos`
   - `TargetScrollPos`
   - `ScrollPauseTicks`
3. `NeedsRedraw` remains **false** after recovery unless later logic sets it; the next scroll update may set `NeedsRedraw` again.

Panels **removed** from the grid have no matching `p` and need no entry. **New** panels get zeros. **Same block instance** after rescan keeps **continuous** scroll behavior.

---

## 3. Interaction with animation

`UpdateVirtualScrollsAndSyncFields` runs **after** `RefreshPanelsIfNeeded` in `RenderAll`. Recovered `ScrollPos` / `TargetScrollPos` are therefore valid inputs for the same tick’s **forward**, **pause**, and **rewind** branches (see [pagination.md](./pagination.md)).

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [engine.md](./engine.md) | When `RefreshPanelsIfNeeded` runs in the split loop |
| [pagination.md](./pagination.md) | Scroll state machine |
| [lcd_panels_and_layout.md](./lcd_panels_and_layout.md) | Virtual panel command lists |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
