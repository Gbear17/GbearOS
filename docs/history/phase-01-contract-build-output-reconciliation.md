> **Archive notice:** Historical planning record for **Phase 1 — contract + build-output reconciliation** (decision **1A**). **Current release procedure and semantic spot-checks** are normative in [`docs/architecture/artifact_verification.md`](../architecture/artifact_verification.md)—not in this file.

> **AI Summary:** Completed phase checklist for aligning `GbearOS_Shared` contracts with minified PB1/PB2 artifacts; superseded by automated gates and `.cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1` as documented under architecture.

## Phase 1 — Contract + build-output reconciliation

### Decisions (locked)
- **1A**: `GbearOS_Shared` is canonical; **spot-check** merged/minified outputs rather than assuming the build pipeline is untrustworthy.

### Goal
Establish high confidence that **shared contracts** (channels, envelope, DTO wire formats) are identical between source (`GbearOS_Shared`) and the **final in-game artifacts** (PB1 + PB2 merged/minified scripts).

### Scope (what this phase covers)
- **IGC channel constants** are identical across PB1/PB2 and match `GbearOS_Shared`.
- **`SenderEnvelope`** rules are identical (MAC input fields, Base64 rules, timestamp monotonic behavior, replay check semantics, SharedKey gating).
- **DTO serialization** matches contract (semicolon field order, protocol version prefix behavior, pipe-array escaping rules, “raw string field must not contain `;`” constraints).
- **PB2 cadence invariants** are preserved (virtual dashboard smooth path every `Update10`; decimated heavy work and template phase rotation; Y-band culling behavior).
- **PB1 phased work invariants** are preserved (Update10, phase completion gating, refresh timing assumptions).

### Work items
- [x] Inventory the authoritative contract sources in `GbearOS_Shared`:
  - [x] Channel constants (e.g., `IGCChannels`)
  - [x] Message type strings (e.g., `IGCMessageTypes`)
  - [x] `SenderEnvelope` (MAC, Base64, replay)
  - [x] DTO serializers/deserializers (e.g., `IGCSerializer` / `Serializer`)
- [x] Establish an “artifact verification checklist” for each release build:
  - [x] PB1 uses the same channel names + envelope logic
  - [x] PB2 uses the same channel names + envelope parse/verify logic
  - [x] Protocol version constant(s) match expectations
  - [x] Any string-escaping helpers are present and unchanged
- [x] Validate the build/merge process includes the shared code in both PBs (no missing/duplicated definitions).
- [x] Confirm that doc expectations reflect current code reality (if drift is found, capture it for gbearos-doc-sync).

### Acceptance criteria
- [x] We can point to a single “contract truth” in `GbearOS_Shared` and show PB1/PB2 outputs conform to it.
- [x] Any drift discovered is either corrected in a later phase or explicitly recorded as a known mismatch.

### Notes / risks
- This phase should be **behavior-preserving** (no functional changes).
- “Byte-for-byte” minified similarity is not required; **semantic** contract equivalence is required (names/ordering/rules that affect on-wire parsing and verification).

### Completion
- **Normative procedure:** [`docs/architecture/artifact_verification.md`](../architecture/artifact_verification.md)
- **Roadmap:** `ROAD-011` set to **IMPLEMENTED** in [`docs/ROADMAP.md`](../ROADMAP.md)

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
