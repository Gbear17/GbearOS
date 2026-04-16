## Phase 4 — PB2 maintainability refactor (module interface + registry)

### Decisions (locked)
- **4B**: **Static dictionary registration** for tag/command → module mapping (initialized once; no per-tick registration).

### Goal
Make PB2 display logic modular and predictable so new UI features are “add a module + register it” work, without changing PB1 automation boundaries.

### Scope (what this phase covers)
- Formalize a PB2 display module contract (conceptually “measure + draw”).
- Standardize tag/command dispatch:
  - Single-tag panels (e.g., `[INV]`, `[PWR]`, `[WARN]`, etc.).
  - `[GbearOS]` virtual dashboards composed from a command list in panel Custom Data.
- Use a **static registry** (dictionary) for tag → module creation/selection, with zero per-tick allocations.

### Work items
- [ ] Define a single conceptual module interface used consistently by:
  - [ ] Single-tag templates
  - [ ] Virtual dashboard modules
- [ ] Implement a static tag/command registry:
  - [ ] Keys: normalized command/tag identifiers
  - [ ] Values: non-capturing factories or reusable instances as appropriate
- [ ] Normalize module lifecycle expectations:
  - [ ] Stateless modules vs modules that hold per-panel state
  - [ ] Where scroll state and paging state live (panel entry vs module)
- [ ] Update docs to reflect “how to add a module” steps and constraints (no allocations, decimated heavy work, etc.).

### Acceptance criteria
- [ ] Adding a new PB2 module does not require changes scattered across unrelated renderer logic.
- [ ] No per-tick dynamic registration or allocations are introduced on the render hot path.
- [ ] Existing tags/commands behave identically before/after the refactor.

### Notes / risks
- A dictionary registry is safe in SE only if it is built once and does not rely on capturing lambdas or per-refresh allocations.
