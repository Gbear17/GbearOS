> **AI Summary:** PB2 display pipeline—split Update10 work, virtual module layout, and Y culling for sprite rendering.

# PB2 display engine — split loop and virtual viewport

## Purpose

Describe the **PB2 programmable block** display pipeline: **split-loop timing**, the **`[GbearOS]`** virtual module layout, and **Y-band culling** for transparent panels. PB1 uses **`Update10`** for automation and outbound IGC (see [update_frequencies.md](./update_frequencies.md)); PB2’s split loop applies only to the display block.

Panel naming and **LCD** Custom Data (vs PB2 script **`[Network]`** INI) are covered in [lcd_panels_and_layout.md](./lcd_panels_and_layout.md).

---

## 1. Split-loop timing (Update10 entry vs decimated heavy work)

### Script cadence

PB2 sets `Runtime.UpdateFrequency = UpdateFrequency.Update10` and runs the main display path on `UpdateType.Update10` (`Main` → `Update10()`).

Each invocation:

1. Increments `_tick10`.
2. Sets `isFullTick = (_tick10 % 10 == 0)`.

So **one out of every ten** `Update10` callbacks is a **full tick**. At the usual coupling between game ticks and programmable block `Update10`, that is the **same decimation ratio** as running the heavy path on **`Update100`** while still entering the script every **`Update10`** (roughly **~10 Hz** script entry vs **~1 Hz** “full” heavy pass, subject to game simulation rate).

### Work partitioned by `isFullTick`

**Every `Update10` (inside the LCD renderer’s `RenderAll`):**

- `RefreshPanelsIfNeeded()` — panel list refresh when countdown reaches zero.
- `UpdateVirtualScrollsAndSyncFields` — scroll position animation, target advancement, pause logic.
- `TryRenderVirtualPanels` — redraw `[GbearOS]` panels when command list inputs are dirty or scrolling requires a redraw.

**Only when `isFullTick` is true:**

- Recompute DTO dirty flags via `DtoChanged` against `_last*` caches; assign `_last*` to current DTO references.

**Telemetry** (`Echo`, `RefreshStatusLcd`) is also gated on `isFullTick` in `Update10()` to limit status churn.

### Rationale

Virtual scrolling and sprite redraw need **smooth** updates; DTO diffing is **cheaper** when decimated. Keeping scroll math on the fast path avoids stutter while bounding instruction cost on the slow path.

---

## 2. Virtual display engine (`[GbearOS]`)

### Command model

Panels whose custom name contains `[GbearOS]` load a **command list** from **panel** **Custom Data** (`ParseCustomDataCommands`): `HEAD`, `INV`, `REF`, `PWR`, `ICE`, `WARN`, `STATUS`, plus layout directives such as `COL`.

If Custom Data is empty or whitespace, PB2 defaults the command list to **`[INV]`**.

`MeasureVirtualContent` computes:

- **Header height** — base offset plus `HEAD` line increments.
- **Total scrollable height** — per-command `SubheaderHeight` (skipped for filtered `INV`) plus `ModuleHeight`.

The visible band for scrolling uses `yCutoff = textureSize.Y * 0.95703125f` and `viewportHeight = yCutoff - headerHeight`. Content is positioned with `drawStart = headerHeight - entry.ScrollPos`.

### Module registry (static, allocation-safe)

PB2 uses a **static dictionary registry** for tag/command → module mapping, initialized once during renderer init:

- **Registry keys** are normalized command tokens (`INV`, `REF`, `PWR`, `ICE`, `WARN`, `STATUS`).
- **Registry values** are module instances implementing `IDisplayComponent`.
- **Dispatch** for measure/draw resolves a module key from the parsed command and calls `Measure` / `Draw` with the active column bounds from `LayoutManager`.

This keeps module dispatch centralized and avoids per-tick registration work on the render hot path.

---

## 3. Coordinate-based culling (transparent LCDs)

**Fields:** `_clipTop` (default `-1`), `_clipBot` (default `9999f`).

**Injection points:** sprite/text enqueue helpers begin with:

```csharp
if (_clipTop >= 0f && (y < _clipTop || y > _clipBot))
    return;
```

**`RenderToPanelVirtual` sequence:**

1. Draw `HEAD` lines while `_clipTop < 0` (culling **off**) so titles remain visible.
2. Optionally draw the `PAGE x/y` indicator (same unculled phase in current code).
3. Set `_clipTop = headerHeight`, `_clipBot = yCutoff`.
4. Iterate modules: subheaders and sprites go through the enqueue helpers; any sprite whose **Y** lies outside `[headerHeight, yCutoff]` is **not** enqueued.
5. Set `_clipTop = -1f` before `FlushSprites`.

**Semantics:** This is a **per-sprite Y band test**, not a GPU scissor. It reduces **off-panel bleed** on **transparent** surfaces where the game may still rasterize text at extreme coordinates. Wide or tall sprites are not clipped on X or by height—only the anchor **Y** is tested, matching the implementation.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Index of architecture docs |
| [lcd_panels_and_layout.md](./lcd_panels_and_layout.md) | Template tags, `[GbearOS]` commands, PB1 vs PB2 Custom Data |
| [pagination.md](./pagination.md) | Scroll step, pause, rewind, page indicator |
| [persistence.md](./persistence.md) | Panel list rebuild and scroll state recovery |
| [update_frequencies.md](./update_frequencies.md) | PB1/PB2 cadence and decimation |
| [igc_contract.md](./igc_contract.md) | IGC and **`SenderEnvelope`** (display consumes admitted payloads) |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
