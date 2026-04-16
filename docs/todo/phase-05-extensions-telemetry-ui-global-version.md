## Phase 5 — Extension work (new telemetry + new UI) with strict global versioning

### Decisions (locked)
- **5B (strict, no-compat)**: Use a **global protocol version bump** when DTO layouts change and **do not** keep backward compatibility (assume PB1 and PB2 are always updated together).

### Goal
Add new features (telemetry, DTO fields, display modules) safely and predictably, while keeping PB1/PB2 boundaries intact and avoiding accidental mixed-version ambiguity.

### Scope (what this phase covers)
- Adding new UI modules/tags/commands in PB2.
- Adding new telemetry in PB1:
  - Prefer extending an existing DTO when coherent.
  - Add a new DTO + channel only when necessary.
- Global protocol version governance:
  - One protocol version constant governs all typed DTO payloads.
  - A DTO layout change requires a version bump.
  - Mixed-version behavior is explicitly unsupported (acceptable “no signal / dropped telemetry” outcomes until both blocks are updated).

### Work items (per extension)
- [ ] Define the feature’s data contract:
  - [ ] Which DTOs are extended or added?
  - [ ] Which IGC channels are used?
  - [ ] Send cadence and update cost expectations
- [ ] Update `GbearOS_Shared` first (DTO + serializer/deserializer expectations).
- [ ] PB1:
  - [ ] Collect the data inside an existing phase slice when possible.
  - [ ] Populate DTO fields and broadcast with `SenderEnvelope`.
- [ ] PB2:
  - [ ] Receive + verify + deserialize into local state.
  - [ ] Implement/extend module(s) and tag/command mapping.
- [ ] Increment global protocol version when DTO layout changes.
- [ ] Update docs (architecture + configuration if applicable).

### Acceptance criteria
- [ ] New feature works with both PBs updated to the same version.
- [ ] MAC verification and replay protections remain intact for all typed channels.
- [ ] UI cadence remains stable (no stutter regressions introduced by the extension).

### Notes / risks
- “No backward compatibility” simplifies implementation but increases the chance that partial updates look like network failure. This is acceptable by decision.
- Prefer feature flags/config toggles for optional modules rather than adding high-frequency channels for rarely used data.
