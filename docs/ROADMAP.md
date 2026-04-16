> **Note:** `docs/ROADMAP.md` is a curated planning index. It is **not** a player-facing contract. Status values are maintained by `doc-sync` after reconciling docs with current source behavior.

# ROADMAP

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

- [ ] (ID: ROAD-010) [STATUS: DEPRECATED] [AREA: BUILD] Python-based script size guard referencing private paths (`C:\\SpaceEngineers1\\shared\\build\\deploy.py`). Superseded by `dotnet build` + deployed-script budget checks in `doc-sync` / `git-sync`. (DOC: README.md) (EVIDENCE: mixed)

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
