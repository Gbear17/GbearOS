## Phase 2 — Protocol hardening (string safety)

### Decisions (locked)
- **2A**: **Ingestion sanitization** — sanitize player-controlled strings once at scan/ingress and store sanitized values for all downstream DTO/render use.

### Technical Approach
- Sanitize player-controlled text at the **grid-scanning / ingestion** layer in PB1 so delimiter characters (`;`, `|`, `\` per wire rules) cannot corrupt the semicolon-delimited IGC DTO or pipe-array fields.
- Apply sanitization **before** DTO population, serialization, and `SenderEnvelope` MAC signing so PB2 verifies the same bytes it renders.
- Prefer strip/replace to a single consistent safe substitute; **no** per-tick re-sanitization of cached values; keep instruction overhead minimal.

### File Checklist
- `GbearOS_PB1_Core/inventory_scanner.cs` — primary grid/block name and label ingress; confirm all scan paths that fill DTO-bound strings.
- `GbearOS_PB1_Core/refinery_manager.cs`, `ice_manager.cs`, `power_manager.cs`, `config_parser.cs` — any user or block-derived strings that reach DTOs or warnings.
- `GbearOS_PB1_Core/Program.cs` — orchestration; only if composition sites live here.
- Shared DTO/serialization/MAC modules under PB1 as identified during implementation (exact paths TBD in code search).

### Estimated Character Impact (baseline grounding)
**MDK2 baseline (2026-04-17, `util-mdk-build`):**
- **PB1**: 53132 / 100000 (remaining 46868) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: 42872 / 100000 (remaining 57128) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

Post-change estimates: small PB1 growth from sanitizer helpers + call sites; PB2 unchanged unless wire parsing tests/docs require updates.

### High-Risk Systems
- **DTO wire format + MAC**: signing must cover the final serialized string; order of sanitize → populate → serialize → sign must stay consistent.
- **Pipe-array and escaped fields**: policy must align with PB2 parsing expectations for `|` and `\`.
- **Budget**: PB1 headroom ~47k UTF-8 characters; avoid hot-path allocations and duplicate passes.

### Goal
Prevent player-controlled text from corrupting the **semicolon-delimited** DTO wire format (and any "raw string" DTO fields) while keeping instruction cost minimal.

### Scope (what this phase covers)
- Identify all **player-controlled string sources** PB1 may ingest (block names, group names, any text injected into warnings/status lines).
- Define a **sanitization policy** for delimiter safety:
  - Replace `;` with a safe character (e.g., `,` or space).
  - Optionally normalize `\r\n` / `\n` to space for single-line fields.
  - Avoid allocations in hot paths: sanitize once per refresh / ingestion, not per tick.
- Apply sanitization **before**:
  - DTO field population,
  - serialization,
  - `SenderEnvelope` MAC computation (so PB2 renders exactly what was signed).

### Work items
- [ ] Build a list of DTO fields that can carry text:
  - [ ] Pipe-array string fields (escaped `|`/`\`)
  - [ ] Raw string fields (explicitly must not contain `;`)
- [ ] Inventory PB1 text composition sites that may include user strings:
  - [ ] Warnings (`WarningDTO.activeMessage` and/or per-subsystem text)
  - [ ] Refinery "priority line" strings
  - [ ] Any name tables/rows shown on LCD (via PB2) that originate from PB1 strings
- [ ] Decide the exact substitution character for `;` (keep it consistent across the project).
- [ ] Implement sanitization at ingestion:
  - [ ] Ensure cached names/labels used later are the sanitized representation.
  - [ ] Confirm no additional per-tick sanitize work is introduced.

### Acceptance criteria
- [ ] A block name containing `;` cannot corrupt DTO parsing on PB2.
- [ ] PB2 continues to verify MAC successfully for sanitized payloads.
- [ ] No perceptible UI regressions (the displayed name is sanitized but stable).

### Notes / risks
- This phase intentionally changes **displayed text** for edge-case names; that is acceptable for wire safety.
- Avoid sanitizing values that never enter DTO strings (keep the change narrowly scoped).

### Cleanup reminder

**DELETE THIS FILE BEFORE FINAL PR SUBMISSION.**
