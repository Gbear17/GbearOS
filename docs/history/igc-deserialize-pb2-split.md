> **Historical / archival** — Branch plan narrative (completed). **Current layout and gates:** [`docs/architecture/contract_checklist.md`](../architecture/contract_checklist.md) §§ **A–D** (shared serialize vs PB2-only deserialize, `.csproj` globs, PB1 deserialize ban); release procedure [`docs/architecture/artifact_verification.md`](../architecture/artifact_verification.md). Shared `Serializer.Serialize` notes PB2 deserialize location in [`GbearOS_Shared/igc/serialization.cs`](../../GbearOS_Shared/igc/serialization.cs).

# Plan: IGC deserialize PB2 placement *(archived)*

> **Status:** **Implemented** — PB2-only generic deserialize and helpers live under **`GbearOS_PB2_Display/`** (`igc_serializer_deserialize.cs`, `serialization_deserialize.cs`). Shared **`GbearOS_Shared/igc/`** retains wire contracts, **`IGCSerializer`** serialize surface, and **`Serializer`** serialize facade; **`serialization.cs`** documents that deserialize entry points are PB2-local. Project manifests use shared globs for both PBs; doc alignment (contract checklist, artifact verification, architecture index) matches this layout.

## Technical approach *(as completed)*

- PB2-only deserialize logic was split out of shared compilation so PB1 cannot link deserialize entry points; shared wire contracts stayed coherent.
- Branch work used conventional naming (`refactor/igc-deserialize-pb2-split`); merge gates relied on MDK2 build + budget discipline.

## File checklist *(complete)*

- [x] `GbearOS_Shared/igc/igc_serializer.cs` — shared serializer surface (serialize path).
- [x] `GbearOS_Shared/igc/serialization.cs` — shared framing/types; PB2 deserialize location called out in comments.
- [x] `GbearOS_Shared/igc/message_types.cs` — inner payload type identifiers (shared).
- [x] `GbearOS_PB2_Display/igc_serializer_deserialize.cs` — PB2-local generic deserialize (`typeof`-routed).
- [x] `GbearOS_PB2_Display/serialization_deserialize.cs` — PB2-local deserialize helpers.
- [x] `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj` — includes shared glob only; PB1 does not compile PB2-only files.
- [x] `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj` — includes shared glob + PB2-local sources.
- [x] `docs/ROADMAP.md` — no standalone roadmap ID required for this refactor; phased IGC work tracked separately.
- [x] `docs/architecture/contract_checklist.md` — aligned with shared vs PB2-only placement (§§ B–D).

## Estimated character impact *(historical note)*

**MDK2 baseline (example paths from plan, 2026):**

- **PB1** / **PB2** deployed `script.cs` sizes remain subject to `gbearos-util-mdk-build` / 100k limits; moving deserialize to PB2 affects PB2 size vs PB1 as minifier assigns symbols.

## High-risk systems *(mitigated)*

- **Shared IGC wire** — unchanged contract surface; only compile placement of deserialize changed.
- **MDK2 merge/minify** — PB2-only files remain outside `GbearOS_Shared/` to avoid PB1 pulling deserialize.
- **Contract docs vs code** — `contract_checklist.md` and `artifact_verification.md` are the living gates.

---

Copyright (c) 2026 Garrett Wyrick. Archived from `docs/todo/igc-deserialize-pb2-split-plan.md` via `gbearos-util-doc-sync` §3.3.4. Licensed under GPL-3.0 — see repository `LICENSE`.
