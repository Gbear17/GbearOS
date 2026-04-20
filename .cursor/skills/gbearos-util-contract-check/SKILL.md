---
name: gbearos-util-contract-check
description: ⚙️ [PRIMITIVE] Verifies PB1/PB2 contract invariants (shared wire contracts + PB2-only file placement) without running git.
---

# Utility Primitive: Contract checklist gate (`contract-check` / `contract check`)

## Purpose
Provide a fast, repeatable verification that:
- PB1 and PB2 compile the same shared contracts from `GbearOS_Shared/`, and
- PB2-only deserialize code lives only in `GbearOS_PB2_Display/` (so PB1 cannot compile it).

This is the mechanical “Phase 1” gate described in:
- `docs/architecture/contract_checklist.md`

## Hard constraints
- **Never run git commands.**
- **Do not modify source code.** Read/verify/report only.
- If a required invariant fails, **abort** and report the exact failing check.

## Logic (execute exactly)

### 1) Required files exist (fail fast)
Verify these files exist:
- `GbearOS_Shared/igc/channels.cs`
- `GbearOS_Shared/network/SenderEnvelope.cs`
- `GbearOS_Shared/igc/igc_serializer.cs`
- `GbearOS_Shared/igc/serialization.cs`
- `GbearOS_PB2_Display/igc_serializer_deserialize.cs`
- `GbearOS_PB2_Display/serialization_deserialize.cs`
- `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj`
- `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj`

If any are missing, **abort**.

### 2) PB2-only files are not in shared
Confirm these files do **not** exist:
- `GbearOS_Shared/igc/igc_serializer_deserialize.cs`
- `GbearOS_Shared/igc/serialization_deserialize.cs`

If any exist, **abort**.

### 3) Project include rules compile shared contracts
Read both `.csproj` files and verify they each contain a `Compile Include="..\\GbearOS_Shared\\**\\*.cs"` item.

If either project does not compile shared contracts, **abort**.

### 4) PB1 does not call deserialize entrypoints
Search under `GbearOS_PB1_Core/` for:
- `Serializer.Deserialize`
- `IGCSerializer.Deserialize`

If any hits are found, **abort** and report the file paths/lines.

### 5) Report success summary (required)
On success, report a short pass block:
- `Contract gate: PASS`
- `Shared compiled by: PB1 + PB2`
- `PB2-only deserialize: PB2 project only`

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `contract-check`, `contract check`

Equivalent natural-language requests to verify PB1/PB2 shared-contract layout also count.
