> **AI_SYSTEM_INDEX**  
> **Historical / archival** documentation: completed plans and superseded design narratives. **Current contracts:** [`docs/architecture/`](../architecture/), [`docs/configuration.md`](../configuration.md). **Active planning:** [`docs/todo/`](../todo/), [`docs/ROADMAP.md`](../ROADMAP.md).

# Documentation history (`docs/history/`)

This folder holds **archived** material moved out of [`docs/design/`](../design/) or [`docs/todo/`](../todo/) after work reached **completion** in code and **normative** docs. Files here are **not** player-facing contracts.

## Files in this folder

| Document | Purpose |
|----------|---------|
| [igc-deserialize-pb2-split.md](./igc-deserialize-pb2-split.md) | Archived plan: PB2-only IGC deserialize placement; see [`contract_checklist.md`](../architecture/contract_checklist.md) §§ B–D |
| [road-012-igc-string-safety-delivery-note.md](./road-012-igc-string-safety-delivery-note.md) | Archived branch delivery note for ROAD-012; technical detail in [`phase-02-protocol-hardening-string-safety.md`](./phase-02-protocol-hardening-string-safety.md) |
| [phase-03-replay-state-resilience.md](./phase-03-replay-state-resilience.md) | Archived Phase 3 plan (replay baseline silence expiry); see [`igc_contract.md`](../architecture/igc_contract.md), [`network-layer.md`](../architecture/network-layer.md) |
| [phase-04-pb2-module-interface-and-registry.md](./phase-04-pb2-module-interface-and-registry.md) | Archived Phase 4 plan (PB2 module interface + registry); see [`engine.md`](../architecture/engine.md), [`lcd_panels_and_layout.md`](../architecture/lcd_panels_and_layout.md) |
| [phase-02-protocol-hardening-string-safety.md](./phase-02-protocol-hardening-string-safety.md) | Archived Phase 2 plan (DTO string ingress / 2A); see [`igc_contract.md`](../architecture/igc_contract.md) § PB1 string ingress |
| [skills-branch-naming-diff-preflight.md](./skills-branch-naming-diff-preflight.md) | Archived plan: diff-grounded pipeline branch naming; see [`.cursor/skills/gbearos-branch-complete/SKILL.md`](../../.cursor/skills/gbearos-branch-complete/SKILL.md) and [`.cursor/skills/gbearos-git-branch-init/SKILL.md`](../../.cursor/skills/gbearos-git-branch-init/SKILL.md) |
| [phase-01-contract-build-output-reconciliation.md](./phase-01-contract-build-output-reconciliation.md) | Archived Phase 1 plan (contract + build-output reconciliation); see [`artifact_verification.md`](../architecture/artifact_verification.md) for current gate |
| [sender-id-protocol-noc-phase1-v14.md](./sender-id-protocol-noc-phase1-v14.md) | Archived NOC Phase 1 sender envelope / MAC design (v1.4); see architecture docs for current behavior |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
