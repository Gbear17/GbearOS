## Phase 2 — Protocol hardening (string safety)

### Decisions (locked)
- **2A**: **Ingestion sanitization** — sanitize player-controlled strings once at scan/ingress and store sanitized values for all downstream DTO/render use.

### Goal
Prevent player-controlled text from corrupting the **semicolon-delimited** DTO wire format (and any “raw string” DTO fields) while keeping instruction cost minimal.

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
  - [ ] Refinery “priority line” strings
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
