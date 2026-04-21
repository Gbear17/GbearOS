## Archive — Phase 4: PB2 maintainability refactor (module interface + registry)

Status: **IMPLEMENTED (archived)**  
Canonical current behavior: [`docs/architecture/engine.md`](../architecture/engine.md), [`docs/architecture/lcd_panels_and_layout.md`](../architecture/lcd_panels_and_layout.md), root [`README.md`](../../README.md)

### Decision (locked)
- **4B**: **Static dictionary registration** for tag/command → module mapping (initialized once; no per-tick registration).

### Goal (historical)
Make PB2 display logic modular and predictable so new UI features are “add a module + register it” work, without changing PB1 automation boundaries.

### Scope (historical)
- Formalize a PB2 display module contract (“measure + draw”).
- Standardize tag/command dispatch for `[GbearOS]` LCD panels, where:
  - `[GbearOS]` in the **panel Custom Name** enables PB2 control.
  - The module command list lives in the panel **Custom Data** (one bracket command per line).
- Use a **static registry** (dictionary) for tag/command → module selection, with zero per-tick allocations introduced on the render hot path.

### Work items (historical checklist)
- Define a single module interface used consistently by:
  - single-module panels (one module command in Custom Data)
  - multi-module dashboards (multiple module commands in Custom Data)
- Implement a static tag/command registry:
  - Keys: normalized command/tag identifiers
  - Values: reusable instances (stateless) or non-capturing factories (when per-panel state is required)
- Normalize module lifecycle expectations:
  - Stateless modules vs modules that hold per-panel state
  - Where scroll state and paging state live (panel entry vs module)
- Update documentation so “how to add a module” is clear and constraints are explicit (no per-tick registration; no hot-path allocations).

### Acceptance criteria (archival notes)
- Adding a new PB2 module does not require changes scattered across unrelated renderer logic.
- No per-tick dynamic registration or allocations are introduced on the render hot path.
- Existing tags/commands behave identically before/after the refactor (player-visible behavior preserved; internal structure improved).

### Notes / risks (archived)
- A dictionary registry is safe in Space Engineers only if it is built once and does not rely on capturing lambdas or per-refresh allocations.

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
