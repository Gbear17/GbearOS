> **Note:** `docs/ROADMAP.md` is a curated planning index. It is **not** a player-facing contract. Status values are maintained by `util-doc-sync` after reconciling docs with current source behavior.

# ROADMAP

## Phased execution plan (PB1/PB2 system hardening + expansion)

This phased plan is the current **implementation driver** for near-term work. Detailed checklists live under `docs/todo/`.

1. **Phase 1 — Contract + build-output reconciliation**  
   Doc: `docs/todo/phase-01-contract-build-output-reconciliation.md`  
   Decision: **1A** (`GbearOS_Shared` is canonical; spot-check merged/minified outputs)

2. **Phase 2 — Protocol hardening (string safety)**  
   Doc: `docs/todo/phase-02-protocol-hardening-string-safety.md`  
   Decision: **2A** (sanitize player-controlled strings at ingestion; store sanitized values)

3. **Phase 3 — Replay-state resilience**  
   Doc: `docs/todo/phase-03-replay-state-resilience.md`  
   Decision: **3A** (silence-based replay-baseline expiry; MAC verification remains mandatory)

4. **Phase 4 — PB2 maintainability refactor (module interface + registry)**  
   Doc: `docs/todo/phase-04-pb2-module-interface-and-registry.md`  
   Decision: **4B** (static dictionary registration; initialized once; no per-tick registration)

5. **Phase 5 — Extension work (telemetry + UI) with strict global versioning**  
   Doc: `docs/todo/phase-05-extensions-telemetry-ui-global-version.md`  
   Decision: **5B (strict)** (global protocol bumps; assume PB1+PB2 update together; no backward compatibility)

6. **Phase 6 — Instruction-budget optimization (targeted) with lightweight state**  
   Doc: `docs/todo/phase-06-instruction-budget-optimization-lightweight-state.md`  
   Decision: **6A (lightweight)** (limited stateful caching/dirty flags with periodic full refresh guardrails)

## IGC / networking (NOC)

- [ ] (ID: ROAD-001) [STATUS: PARTIAL] [AREA: DOCS] Public-release workflow hardening (public vs private routing/tooling). (DOC: docs/design/README.md) (EVIDENCE: docs-only)
- [ ] (ID: ROAD-002) [STATUS: IMPLEMENTED] [AREA: IGC] NOC Phase 1 — `SenderEnvelope` stateless MAC envelope (`SenderId|Timestamp|PayloadB64|MAC`) + replay window + `SharedKey` gating. (DOC: docs/architecture/network-layer.md) (EVIDENCE: code)
- [ ] (ID: ROAD-003) [STATUS: PLANNED] [AREA: IGC] NOC Phase 2 — stronger-than-FNV authentication / key lifecycle (beyond current integrity check). (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)
- [ ] (ID: ROAD-004) [STATUS: PLANNED] [AREA: IGC] NOC Phase 3 — multi-hop IGC telemetry routing via laser antennas. (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)

## PB2 UI / presentation

- [ ] (ID: ROAD-005) [STATUS: IMPLEMENTED] [AREA: PB2] Refactor renderer into reusable display modules (command → module dispatch). (DOC: docs/architecture/lcd_panels_and_layout.md) (EVIDENCE: code)
- [ ] (ID: ROAD-006) [STATUS: PLANNED] [AREA: LCD] Expand module/library coverage (more panels, more modules, more layout presets). (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)
- [ ] (ID: ROAD-007) [STATUS: PLANNED] [AREA: LCD] Mini-stat commands for inline LCD injections (text or compact sprite snippets). (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)

## PB3 compute coprocessor

- [ ] (ID: ROAD-008) [STATUS: PLANNED] [AREA: IGC] Introduce PB3 as a heavy-math coprocessor to protect PB2’s ~10 Hz render cadence. (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)
- [ ] (ID: ROAD-009) [STATUS: PLANNED] [AREA: IGC] Kinematic projections (stopping distance, time-to-stop, collision-vector forecasting). (DOC: docs/ROADMAP.md) (EVIDENCE: docs-only)

## Tooling / release hygiene

- [ ] (ID: ROAD-010) [STATUS: DEPRECATED] [AREA: BUILD] Python-based script size guard referencing private paths (`C:\\SpaceEngineers1\\shared\\build\\deploy.py`). Superseded by `dotnet build` + deployed-script budget checks via `util-mdk-build` (orchestrated by `git-sync`). (DOC: README.md) (EVIDENCE: mixed)

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
