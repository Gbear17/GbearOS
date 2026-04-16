# Plan: IGC deserialize PB2 placement

## Technical Approach

- Complete **branch-complete** from `main` with local WIP: split PB2-only deserialize logic out of `GbearOS_Shared` into `GbearOS_PB2_Display`, keep shared wire contracts coherent, and align project files and roadmap/docs with the new layout.
- **Branch off `main`** only; use conventional branch `refactor/igc-deserialize-pb2-split`.
- **Do not skip** MDK2 build gates before merge-oriented sync: treat deployed `script.cs` sizes as the budget baseline for further edits.
- **Delete this plan file before final PR submission** (per workflow).

## File Checklist

- `GbearOS_Shared/igc/igc_serializer.cs` — shared serializer surface; avoid breaking PB1/PB2 contract.
- `GbearOS_Shared/igc/serialization.cs` — shared framing/types.
- `GbearOS_Shared/igc/igc_serializer_deserialize.cs` — **remove** (moved off shared).
- `GbearOS_Shared/igc/serialization_pb1.cs` — **remove** or supersede per new split.
- `GbearOS_PB2_Display/igc_serializer_deserialize.cs` — **add** PB2-local deserialize.
- `GbearOS_PB2_Display/serialization_deserialize.cs` — **add** PB2-local deserialize helpers.
- `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj` — project references / compile items after shared file moves.
- `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj` — include new PB2-only sources.
- `docs/ROADMAP.md` — reflect current trajectory.
- `docs/architecture/contract_checklist.md` — optional doc alignment with contracts (untracked).
- Existing `docs/todo/phase-*.md` — leave as curated backlog unless this branch explicitly updates them.

## Estimated Character Impact

**Character Impact Grounding (MDK2 baseline after `dotnet build`, `output=auto`):**

- **PB1**: 53132 / 100000 (remaining 46868) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: 42872 / 100000 (remaining 57128) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

Moving deserialize from shared into PB2-only sources may **increase PB2** deployed size and **decrease PB1** depending on how the minifier merges symbols; re-run `util-mdk-build` after substantive edits and keep both scripts under the 100,000 limit with comfortable headroom.

## High-Risk Systems

- **Shared IGC wire** — any mismatch breaks cross-PB messaging or save/replay assumptions.
- **MDK2 merge/minify** — duplicate types or partial classes across folders can fail build or inflate one script disproportionately.
- **Contract docs vs code** — `docs/architecture/contract_checklist.md` and roadmap must stay aligned with what PB1/PB2 actually ship.

### Cleanup reminder

**DELETE THIS FILE BEFORE FINAL PR SUBMISSION.**
