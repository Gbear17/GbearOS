> **AI Summary:** Scroll state machine, easing, and paging for virtual GbearOS LCD panels when content exceeds the viewport.

# Virtual panel pagination and scroll animation

## Purpose

Describe the scroll state machine inside the **PB2 LCD renderer** for **virtual** **`[GbearOS]`** panels (`entry.Commands != null`) when scrollable content exceeds the viewport. Command lists and panel setup: [lcd_panels_and_layout.md](./lcd_panels_and_layout.md).

---

## 1. Derived quantities

For each virtual panel:

- `headH`, `totalContent` — from `MeasureVirtualContent`.
- `yCut = textureSize.Y * 0.95703125f`.
- `viewportH = yCut - headH` — visible height below the measured header.
- `maxScroll = totalContent - viewportH` — maximum `ScrollPos` so the tail of content can align with the viewport bottom.

If `totalContent <= viewportH`, scroll state is cleared: `ScrollPos = 0`, `TargetScrollPos = 0`, `ScrollPauseTicks = 0`.

---

## 2. Scroll step and overlap (90% / 10%)

```csharp
float scrollStep = viewportH * 0.90f;
```

Each **logical** page advance moves the scroll target by **90%** of the viewport height, leaving **10%** of the previous view still visible (**overlap**), which stabilizes reading across page boundaries.

The same factor is used when clamping the next target to `maxScroll`:

```csharp
e.TargetScrollPos = e.ScrollPos + scrollStep;
if (e.TargetScrollPos > maxScroll)
    e.TargetScrollPos = maxScroll;
```

---

## 3. Forward animation (~1.2 s per `scrollStep`)

When `TargetScrollPos > ScrollPos` (scrolling **down** toward the next stop):

```csharp
float step = scrollStep / 12f;
e.ScrollPos += step;
if (e.ScrollPos >= e.TargetScrollPos)
    e.ScrollPos = e.TargetScrollPos;
e.NeedsRedraw = true;
```

This runs on **every** `Update10` tick while animating. **12** steps at **`Update10`** cadence yields **~1.2 s** of real time per full `scrollStep` distance (game tick rate dependent).

---

## 4. Pause and next target

When `ScrollPos == TargetScrollPos` (idle at a stop):

- `ScrollPauseTicks` increments each tick.
- When `ScrollPauseTicks >= 50` (~**5 s** at `Update10` pacing, subject to game tick rate):
  - Reset `ScrollPauseTicks = 0`.
  - If `ScrollPos >= maxScroll - 5f` (within **5 px** of bottom): set `TargetScrollPos = 0f` to start **rewind** to top.
  - Else: advance `TargetScrollPos` by `scrollStep`, clamped to `maxScroll`.
- `NeedsRedraw = true` on each such decision tick.

---

## 5. Smooth rewind (easing)

When `TargetScrollPos < ScrollPos` (typically `TargetScrollPos == 0` after bottom timeout):

```csharp
float dist = e.ScrollPos - e.TargetScrollPos;
float rewindStep = dist * 0.15f;
if (rewindStep < 20f)
    rewindStep = 20f;
e.ScrollPos -= rewindStep;
if (e.ScrollPos <= e.TargetScrollPos)
    e.ScrollPos = e.TargetScrollPos;
e.NeedsRedraw = true;
```

**Distance-proportional** step (15% of remaining distance per tick) slows near the target; a **minimum 20** pixel step prevents **stalling** when `dist` is small. When `ScrollPos` reaches `TargetScrollPos`, the state machine returns to the **pause** branch.

---

## 6. Page indicator (`RenderToPanelVirtual`)

When `totalScrollable > viewportHeight`:

- `maxS = totalScrollable - viewportHeight`
- `stepS = viewportHeight * 0.90f` — **same** step as animation (`scrollStep`).
- `totalPages = (int)Math.Ceiling(maxS / stepS) + 1`
- Current page:
  - If `entry.ScrollPos >= maxS - 5f` → `curPage = totalPages` (force **last** page near end to avoid off-by-one from integer truncation).
  - Else `curPage = (int)(entry.ScrollPos / stepS) + 1` (integer truncation replaces `Floor` for minified build compatibility).

The label is drawn at the top-right (`size.X * 0.97f`, `size.Y * 0.025f`).

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [engine.md](./engine.md) | Split loop, culling, when virtual scroll runs |
| [persistence.md](./persistence.md) | Scroll state across panel list rescans |
| [lcd_panels_and_layout.md](./lcd_panels_and_layout.md) | `[GbearOS]` Custom Data commands |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
