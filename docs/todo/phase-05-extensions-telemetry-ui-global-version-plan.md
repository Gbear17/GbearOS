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

### Technical Approach
- **Strict global versioning (5B)**: one shared protocol version governs typed DTO payloads; any DTO wire-layout change bumps the version; PB1+PB2 must ship together (no backward-compat shims).
- **Shared contracts first**: extend or add DTOs + serializers in `GbearOS_Shared/` before PB1/PB2 feature wiring.
- **PB1 boundary**: collect/aggregate telemetry in existing phase slices when possible; broadcast only via `SenderEnvelope` on documented channels.
- **PB2 boundary**: verify + deserialize + render; add/extend PB2 modules via the static registry pattern (no per-tick dynamic registration/allocs on the render hot path).
- **Operational expectation**: mixed versions may present as dropped telemetry / no-signal until both blocks match; treat as acceptable by decision.

### File Checklist
- `GbearOS_Shared/` — DTOs, `IGCChannels` / message identifiers as needed, serializer expectations
- `GbearOS_PB1_Core/` — scan/collect paths, DTO population, `SendDto`/broadcast sites
- `GbearOS_PB2_Display/` — IGC routing/deserialize, module registry + renderer integration, docs for new tags/commands
- `docs/architecture/` + root `README.md` — normative contract updates for new channels/tags/versioning rules
- `docs/ROADMAP.md` — status/DOC pointers when scope becomes implementable in-repo

### Estimated Character Impact
Baseline (from `gbearos-util-mdk-build`, 2026-04-20):
- **PB1**: `53470` / 100000 (remaining `46530`) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: `43907` / 100000 (remaining `56093`) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

Expectation:
- Phase 5 extensions will grow **PB1** (new scan/aggregate/serialize paths) and/or **PB2** (new modules + deserialize glue) depending on the feature.
- Any DTO layout change must also account for **shared** growth + coordinated version bump overhead across both scripts.

### High-Risk Systems
- **Global protocol version + DTO wire format** (mixed-version failure modes; serializer drift)
- **IGC channel naming + `SenderEnvelope` MAC/replay path** (must remain correct for all typed channels)
- **PB2 render cadence** (instruction budget; avoid hot-path allocations / per-tick registration)

### Notes / risks
- “No backward compatibility” simplifies implementation but increases the chance that partial updates look like network failure. This is acceptable by decision.
- Prefer feature flags/config toggles for optional modules rather than adding high-frequency channels for rarely used data.
