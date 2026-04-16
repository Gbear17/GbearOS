# PB2 modular UI refactor — execution plan

## 1. Technical approach

**Project:** GbearOS PB2 Modular UI Refactor.

**Goal:** Refactor the **PB2_Display** engine away from **monolithic, hardcoded templates** toward a **component-based system** with explicit granularity:

1. **Templates** — Full dashboards (examples given: **`[INV]`**, **`[PWR]`**).
2. **Sub-Templates** — Filtered dashboards (example: **`[INV:Ores]`** — subtype-filtered view).
3. **Modules** — Compact **single-line or double-line** items (example: **`[INV:Iron]`**).

**Layout requirement:** Implement a **LayoutManager** that can enforce **column-based boundaries** using **panel CustomData tags** of the form **`[2COL:1]`** and **`[2COL:2]`** so modules can be grouped into a **two-column layout** (column 1 vs column 2 within the logical layout pass).

**Parser / discovery context (existing behavior to preserve or extend):** Today, **`[GbearOS]`** in the LCD **custom name** drives **virtual dashboard** mode where **panel Custom Data** lists commands one per line (e.g. **`[HEAD:...]`**, **`[INV]`**, **`[INV:Subtype]`** for filtered inventory). Single-tag panels use **name substrings** only. New layout tags apply in the same **panel Custom Data** command stream unless a later decision narrows scope.

**PB1 / PB2 split (non-negotiable from architecture):** PB2 remains **display-only** — no inventory/refinery scanning or automation; IGC DTO consumption and sprite layout only. Any refactor must not move data acquisition onto PB2.

**Build / concat:** Any new PB2 source file must be wired into **`build/pb2_build.md`** in the correct order and remain valid for the single-script concat pipeline.

**Character budget:** Space Engineers programmable block **minified script limit is 100,000 characters** for the shipped script; PB2 growth must stay under cap with headroom (see estimate section).

---

## 2. File checklist

### High risk (from audit)

- **`GbearOS_PB2_Display/lcd_renderer.cs`** — Owns panel discovery, **`ParseCustomDataCommands`**, **`DisplayCommand`**, **`MeasureVirtualContent` / `ModuleHeight`**, virtual scroll measurement, and sprite emission. **Highest coupling** for templates, **`[INV:]`** filtering, and viewport math; LayoutManager will either live here or be called from here.
- **`GbearOS_PB2_Display/Program.cs`** — PB2 **`Update10`** entry, ties **`LCDRenderer`** to IGC-cached DTOs and status LCD mirroring; behavioral regressions surface as “blank panels” or tick stalls.
- **`GbearOS_PB2_Display/igc_parser.cs`** — Network **`CustomData`** bootstrap for PB2; do not conflate with LCD panel Custom Data, but mistakes here break all rendering.
- **`build/pb2_build.md`** — Authoritative concat order; a missing or mis-ordered new file breaks deploy / in-game compile.
- **`docs/architecture/lcd_panels_and_layout.md`** — Normative user-facing contract for **`[GbearOS]`**, template name tags, and **`[INV:Subtype]`**; must be updated when **`[2COL:1]`** / **`[2COL:2]`** semantics are finalized.
- **`docs/architecture/engine.md`** — Split-loop and virtual viewport responsibilities; two-column layout may affect **decimated heavy path** vs **every-tick scroll** assumptions.
- **`docs/architecture/pagination.md`** — Scroll step (**90%** viewport overlap), easing, and **`MeasureVirtualContent`** alignment; column layout changes **totalScrollableHeight** semantics.
- **`docs/architecture/persistence.md`** — Documents **`PanelEntry`** scroll state recovery across rescans; new per-panel layout state must follow the same **panel reference** reconnection pattern if persisted.

### Additional (expected)

- **`GbearOS_PB2_Display/layout_manager.cs`** (or equivalent new unit) — Encapsulate **2COL** grouping and column width/clip math if **`lcd_renderer.cs`** would otherwise grow further.
- **`GbearOS_PB2_Display/warning_formatter.cs`** — Touch only if warning strip layout interacts with modular rows.
- **`GbearOS_Shared/dto/*.cs`** — Read-only unless a display concern requires new derived fields (prefer formatting in PB2 from existing DTOs).
- **`README.md`** — Update PB2 bullet / size-guard note only if the README’s qualitative size band changes materially after implementation.

---

## 3. Estimated character impact

**Baseline (this session, `python C:\SpaceEngineers1\shared\build\deploy.py --target-repo .` on `dev` / current tree):**

- **`dist/minified/PB2_min.cs`:** **71,913** characters  
- **`dist/minified/PB1_min.cs`:** **85,923** characters (listed for context; PB1 is the tighter band)  
- Engine limit: **100,000** characters (minified)

**Delta:** A LayoutManager plus refactored modular rendering will likely add **multi-k to low-five-figure** characters to PB2 depending on how much **`lcd_renderer.cs`** is split versus inlined. **Headroom on PB2 is ~28k** before hitting the cap at the measured baseline—comfortable but not unlimited if large new abstractions duplicate draw helpers.

**Hard re-check:** After substantive edits, rerun the same **`deploy.py`** command and compare **`PB2_min.cs`** line in the **Size Report**.

---

## 4. High-risk systems

- **Virtual `[GbearOS]` command pipeline** — Line-based **`[Type:Arg]`** parsing, **`MaskFromCommandList`**, and **`HEAD` / `INV` filter** behavior are easy to break with new token types (**`2COL`**) if parsing order or mask logic is wrong.
- **Scroll height accounting** — **`MeasureVirtualContent`** and **`ModuleHeight`** must stay consistent with draw paths; **`2COL`** rows that advance two modules on one “logical row” can desync measurement from painting and break scroll bounds.
- **Sprite clipping** — Renderer already uses **`_clipTop` / `_clipBot`**; column halves need coherent clip rectangles so text does not bleed between columns.
- **Concat / single-namespace flattening** — New types must remain PB2-safe (no forbidden APIs) and deploy-clean.

---

### Cleanup reminder

**DELETE THIS FILE BEFORE FINAL PR SUBMISSION.**
