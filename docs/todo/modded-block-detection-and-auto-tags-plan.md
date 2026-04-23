## Modded block detection and auto-tags

### Goal
Harden PB1/PB2 behavior on modded blocks by broadening storage discovery safely (tag-aware first), strengthening classification logic (especially power producers), and optionally adding auto-tagging and production automation behind explicit opt-in.

### Scope (what this plan covers)
- Storage discovery and routing should not depend exclusively on `IMyCargoContainer` when mods provide “cargo-like” blocks that only expose inventories.
- Ice “cargo” accounting should include tagged modded storage where appropriate.
- Power producer classification should rely on stable interfaces and definition ids first, not `BlockDefinition.ToString()` substring heuristics.
- Optional: automatic storage tagging (`[A:*]`) and allowlist-driven modded production automation behind explicit configuration gates.

### Technical Approach (phased rollout)
#### Phase 0 — Reproduce and classify failures (no behavior change)
- Add a temporary PB1 debug matrix for representative vanilla + modded blocks:
  - `HasInventory`, `InventoryCount`
  - cast success/failure: `IMyCargoContainer`, `IMyRefinery`, `IMyAssembler`, `IMyProductionBlock`, `IMyPowerProducer`, `IMyReactor`, `IMyBatteryBlock`
  - `BlockDefinition.TypeIdString` / `SubtypeId`

#### Phase 1 — Introduce a first-class “tagged storage” cache
- Extend PB1 cache refresh (`InventoryScanner.FillConstructBlockCaches` + PB1 `Program.cs` wiring) to build:
  - vanilla cargo (`IMyCargoContainer`) for compatibility
  - tagged storage terminals (`IMyTerminalBlock` with inventories) for any explicitly tagged storage blocks
- Rules:
  - same-construct + manual exclusions apply
  - explicit tag match only (do not auto-include every inventory block)
  - multi-inventory blocks must be enumerated via `HasInventory` / `InventoryCount` / `GetInventory(i)`

#### Phase 2 — Refactor inventory scanner routing to consume tagged storage
- Seed routing from cargo + tagged storage.
- Generalize destination selection to accept terminal blocks and enumerate inventories safely.
- Audit all uses of `_cachedCargoContainers` / “cachedCargo” parameters so they represent “storage candidates,” not “must be vanilla cargo.”

#### Phase 2.5 — Auto-tagging (opt-out; never rewrites manual tags)
- Manual tags: `[M:<Tag>]`; auto tags: `[A:<Tag>]`.
- Must never remove/rewrite any `[M:*]` tag.
- Add Food tagging (`[M:Food]` / `[A:Food]`) with an isolated `IsFood(itemType)` helper (initially: `TypeId == MyObjectBuilder_ConsumableItem`).
- Use fullness thresholds with hysteresis + pressure signals, and distinguish conveyor disconnection from storage pressure (guard `IsConnectedTo` cost by batching).
- Throttle renames; prefer lazy cleanup (do not force-evacuate on tag removal).

#### Phase 3 — Fix ice “cargo” accounting for modded storage
- Seed ice cargo inventories from tagged storage blocks (not only `IMyCargoContainer`).
- Replace `b is IMyCargoContainer` gates with “vanilla cargo OR tagged storage OR irrigation OR generator” logic consistent with Phase 1 rules.

#### Phase 4 — Harden power producer classification
- Prefer interfaces (`IMySolarPanel`, etc.) and definition ids (`BlockDefinition.TypeId` / `SubtypeId`) over `ToString()` substrings.
- Add guarded fallbacks only when needed; prefer config allowlists over broad substring scans.

#### Phase 5 (optional) — Modded production automation (explicit opt-in)
- Do not treat non-`IMyRefinery`/`IMyAssembler` blocks as refiners/assemblers by default.
- Add an allowlist-driven mode mapping `BlockDefinition` ids to behavior via `IMyProductionBlock` APIs, with strict validation and clear warnings.

#### Phase 6 — Verification
- Run MDK build/budget gate and in-game regression checklist (vanilla unchanged; modded tagged storage participates; ice/power render correctly).

### File Checklist
- `GbearOS_PB1_Core/inventory_scanner.cs` (storage caches + routing)
- `GbearOS_PB1_Core/Program.cs` (cache refresh wiring)
- `GbearOS_PB1_Core/ice_manager.cs` (cargo ice relevance + distribution)
- `GbearOS_PB1_Core/power_manager.cs` (power producer classification)
- `GbearOS_Shared/utils/*` (only if shared helpers are required for safe classification)
- `docs/architecture/modded_block_support.md` (normative behavior + limitations)
- `docs/configuration.md` (only if new config keys are introduced)

### Estimated Character Impact
Baseline (from `gbearos-util-mdk-build`, 2026-04-20):
- **PB1**: `53470` / 100000 (remaining `46530`) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: `43907` / 100000 (remaining `56093`) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

Expectation:
- Majority of growth will land in **PB1** (classification + routing + optional auto-tagging / automation).
- PB2 should remain largely unchanged unless new telemetry/UI is introduced.

### High-Risk Systems
- Inventory routing correctness (false positives/negatives on modded inventories)
- Performance-sensitive loops (classification must avoid allocations and expensive connectivity checks)
- Any DTO schema changes (would require coordinated protocol version bump if new fields/channels are introduced)
