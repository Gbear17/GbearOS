# Plan: IGC deserialize PB2 placement

> **Status:** Partial — shared wire contracts remain under `GbearOS_Shared/igc/`; PB2-only deserialize lives under `GbearOS_PB2_Display/`. This checklist tracks remaining doc/roadmap alignment, not a greenfield split.

## Technical Approach

- Complete **branch-complete** from `main` with local WIP: split PB2-only deserialize logic out of `GbearOS_Shared` into `GbearOS_PB2_Display`, keep shared wire contracts coherent, and align project files and roadmap/docs with the new layout.
- **Branch off `main`** only; use conventional branch `refactor/igc-deserialize-pb2-split`.
- **Do not skip** MDK2 build gates before merge-oriented sync: treat deployed `script.cs` sizes as the budget baseline for further edits.

## File Checklist

- `GbearOS_Shared/igc/igc_serializer.cs` — shared serializer surface; avoid breaking PB1/PB2 contract.
- `GbearOS_Shared/igc/serialization.cs` — shared framing/types.
- `GbearOS_Shared/igc/message_types.cs` — inner payload type identifiers (shared).
- `GbearOS_PB2_Display/igc_serializer_deserialize.cs` — PB2-local generic deserialize (`typeof`-routed).
- `GbearOS_PB2_Display/serialization_deserialize.cs` — PB2-local deserialize helpers.
- `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj` — verify PB1 manifest includes only intended compile set.
- `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj` — verify PB2 manifest includes only intended compile set.
- `docs/ROADMAP.md` — reflect current trajectory.
- `docs/architecture/contract_checklist.md` — keep aligned with shared vs PB2-only placement.
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

When the split and docs are **fully** complete per the **`util-doc-sync`** documentation skill, archive this plan under `docs/history/` and **remove** this file (no placeholder).
