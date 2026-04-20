## Phase 6 — Instruction-budget optimization (targeted) with lightweight stateful caching

### Decisions (locked)
- **6A (lightweight)**: Use limited stateful caching/dirty flags in PB1’s highest-cost areas (especially block discovery / scan scheduling) with **periodic full refresh** guardrails.

### Goal
Recover instruction headroom only where needed, without destabilizing behavior or introducing hard-to-debug stale caches.

### Scope (what this phase covers)
- **PB1**:
  - Target the biggest costs first: construct scans, block discovery/filtering, deep inventory iteration, repeated terminal queries.
  - Introduce lightweight caches where they reduce repeated work meaningfully.
  - Ensure no phase can starve network broadcasts indefinitely.
- **PB2**:
  - Reduce unnecessary redraw triggers and expensive text measurement work.
  - Preserve smooth virtual scroll behavior (don’t “render only on decimated ticks”).

### Work items
- [ ] Measure/identify hotspots (instruction spikes, stutter moments, slow refresh scenarios).
- [ ] PB1 targeted improvements (examples):
  - [ ] Cache block lists longer; only rebuild on phase-0 refresh windows + periodic refresh timer.
  - [ ] Minimize `GetBlocksOfType` and repeated `IsSameConstructAs` checks by grouping/caching where safe.
  - [ ] Keep existing multi-tick pass continuation model; avoid full rescans mid-pass.
  - [ ] Add starvation guardrails (e.g., “ensure telemetry phase runs at least every X seconds even under load” if needed).
- [ ] PB2 targeted improvements (examples):
  - [ ] Cache expensive measurement results where text is stable.
  - [ ] Tighten “dirty” conditions so full redraw isn’t triggered unnecessarily.
  - [ ] Keep culling logic behaviorally identical (Y-band anchor-based culling).
- [ ] Add a periodic full refresh path as a correctness backstop for any cache.

### Acceptance criteria
- [ ] Fewer instruction-limit faults on large constructs under normal gameplay.
- [ ] No functional regressions (telemetry remains accurate; UI remains responsive and correct).
- [ ] Cache invalidation is explicit and has a periodic safety refresh.

### Notes / risks
- Avoid “perfect” dirty-flag systems; prefer simple invalidation rules and periodic refresh.
- Changes here are the most regression-prone; keep each optimization small and verifiable.
