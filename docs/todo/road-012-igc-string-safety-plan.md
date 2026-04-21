# ROAD-012 — Phase 2 IGC string safety (delivery branch)

> **Status:** Implementation complete on branch `feat/road-012-igc-string-safety`; this file records MDK baseline for branch-init / merge gates.

## Technical Approach

- Deliver **ROAD-012** / **2A**: `FormattingUtils.SanitizeIngressWireText` at PB1 ingress for DTO-bound strings; refinery + inventory dynamic paths; normative docs (`igc_contract`, `contract_checklist`, `README`).
- Archive completed todos per `gbearos-util-doc-sync`: Phase 2 plan, IGC deserialize split plan → `docs/history/`.
- Do not skip MDK2 / contract-check / dist / artifact gates before merge-oriented sync.

## File Checklist

- `GbearOS_Shared/utils/formatting_utils.cs` — sanitizer.
- `GbearOS_PB1_Core/refinery_manager.cs`, `inventory_scanner.cs` — call sites.
- `docs/architecture/igc_contract.md`, `contract_checklist.md`, `README.md`, `ROADMAP.md`, `docs/history/**`, `docs/todo/README.md`.

## Estimated Character Impact (MDK2 baseline)

**Baseline (2026-04-20, `gbearos-util-mdk-build` after `dotnet build` on both projects, `output=auto`):**

- **PB1**: 53470 / 100000 (remaining 46530) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: 42867 / 100000 (remaining 57133) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

## High-Risk Systems

- DTO wire + MAC ordering (sanitize before serialize/sign).
- `dist/` artifact spot-check vs `GbearOS_Shared` after deploy.
