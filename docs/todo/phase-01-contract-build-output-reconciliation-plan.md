# Phase 1 bootstrap — contract & build-output reconciliation

## Technical Approach

- Treat **`GbearOS_Shared`** as the **single canonical** source of truth for IGC contracts: `IGCChannels`, `IGCMessageTypes`, `SenderEnvelope`, and shared serialize path (`IGCSerializer` / `Serializer` facade per `docs/architecture/contract_checklist.md`).
- Verify that **deployed minified artifacts** (MDK2 `script.cs` for PB1 and PB2) **semantically** match those contracts: same channel names, envelope wrap/parse rules, DTO field order, protocol version handling, and escaping rules that affect on-wire behavior. **Byte-identical minified text is not required** (decision **1A**: spot-check, not paranoia about the pipeline).
- Preserve **strict architectural isolation**:
  - PB2-only deserialize lives only under `GbearOS_PB2_Display/` (`igc_serializer_deserialize.cs`, `serialization_deserialize.cs`); it must not move into `GbearOS_Shared/`.
  - PB1 must not call `Serializer.Deserialize` / `IGCSerializer.Deserialize` generics anywhere under `GbearOS_PB1_Core/`.
- Keep Phase 1 **behavior-preserving**: no intentional gameplay or wire-behavior changes; document drift if found rather than silently “fixing” unrelated code.
- Align documentation with reality where gaps are found; prefer **`util-doc-sync`** on the branch after substantive edits (or note items for a doc pass).
- Preserve **cadence invariants** called out in `docs/todo/phase-01-contract-build-output-reconciliation.md` (PB2 virtual dashboard / decimation / Y-band culling; PB1 `Update10` phased work)—this phase validates they are not accidentally altered by contract work.

## File Checklist

| Area | Paths / actions |
|------|------------------|
| Canonical contracts | `GbearOS_Shared/igc/channels.cs`, `GbearOS_Shared/igc/message_types.cs`, `GbearOS_Shared/network/SenderEnvelope.cs`, `GbearOS_Shared/igc/igc_serializer.cs`, `GbearOS_Shared/igc/serialization.cs` |
| PB2-only deserialize | `GbearOS_PB2_Display/igc_serializer_deserialize.cs`, `GbearOS_PB2_Display/serialization_deserialize.cs` — verify placement and that PB1 never includes them |
| Project includes | `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj`, `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj` — shared glob includes |
| Ingress / send paths | `GbearOS_PB1_Core/Program.cs` (or send helpers), `GbearOS_PB2_Display/igc_parser.cs` (or equivalent) — envelope + channel usage |
| Gates | `docs/architecture/contract_checklist.md`; optional automation via `util-contract-check` primitive |
| Release mirror | After verification, `dist/` via `util-dist-mirror` in `git-sync` (not required for this bootstrap plan) |
| Phase spec (read-only reference) | `docs/todo/phase-01-contract-build-output-reconciliation.md` |

**High-risk touch points:** `SenderEnvelope` MAC input string, replay map, and any change that could let PB1 compile deserialize code or duplicate contract definitions outside `GbearOS_Shared`.

## Estimated Character Impact

Baseline from **`util-mdk-build`** (this machine, post-`dotnet build`, `output=auto` in `mdk.local.ini`):

- **PB1**: `53132` / 100000 (remaining **46868**) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: `42872` / 100000 (remaining **57128**) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

**Handoff (resolved output directories):**

- PB1: `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core`
- PB2: `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display`

Phase 1 reconciliation is expected to be **checklist- and doc-heavy**; any code edits should be minimal. If verification tooling or small guards are added, estimate deltas against the baselines above and keep combined PB scripts under the **100,000** UTF-8 character limit.

## High-Risk Systems

1. **`SenderEnvelope`** — MAC construction, Base64 payload, timestamp/replay interaction with PB2 ingress.
2. **`IGCSerializer` / DTO field order** — semicolon split order and pipe escaping; raw string fields that forbid `;`.
3. **PB1/PB2 boundary** — accidental `Deserialize` usage on PB1 or shared-folder drift that merges PB2-only code into PB1’s compilation unit.

### Cleanup reminder

**DELETE THIS FILE BEFORE FINAL PR SUBMISSION.**
