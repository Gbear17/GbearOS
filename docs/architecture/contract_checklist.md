> **Note:** This checklist is a *developer gate*, not a player-facing contract.

## Contract checklist (PB1/PB2 boundary + shared contracts)

### Purpose
Keep PB1 and PB2 aligned on the **same** shared wire contracts while ensuring PB1 cannot accidentally compile PB2-only logic.

### A. Shared contract surface (source of truth)
Confirm these files are the canonical definitions and remain coherent together:
- `GbearOS_Shared/igc/channels.cs` (IGC tags)
- `GbearOS_Shared/igc/message_types.cs` (DTO message type strings, if used)
- `GbearOS_Shared/network/SenderEnvelope.cs` (envelope format + MAC + replay semantics)
- `GbearOS_Shared/igc/igc_serializer.cs` (ProtocolVersion + DTO field order + escaping rules)
- `GbearOS_Shared/igc/serialization.cs` (shared `Serializer.Serialize` facade)

### B. PB2-only files must live in PB2 project
Confirm these files exist under PB2 and **do not** exist under `GbearOS_Shared/`:
- `GbearOS_PB2_Display/igc_serializer_deserialize.cs` (PB2-only `IGCSerializer.Deserialize<T>`)
- `GbearOS_PB2_Display/serialization_deserialize.cs` (PB2-only `Serializer.Deserialize<T>`)

### C. Project include rules (prevents drift)
Confirm:
- `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj` compiles `..\GbearOS_Shared\**\*.cs`
- `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj` compiles `..\GbearOS_Shared\**\*.cs`
- PB2-only files are compiled by PB2 due to living in `GbearOS_PB2_Display/` (no shared-path include needed).

### D. No accidental “PB1 deserialize” usage
Confirm PB1 does not call deserialize entrypoints:
- No `Serializer.Deserialize<...>` usage under `GbearOS_PB1_Core/`
- No `IGCSerializer.Deserialize<...>` usage under `GbearOS_PB1_Core/`

### E. Artifact sanity (required for release; see also artifact verification)
Normative procedure: [`artifact_verification.md`](./artifact_verification.md).

- Build and deploy via MDK2 (`dotnet build`).
- Run **`gbearos-util-contract-check`**, then mirror deployed outputs into `dist/` (**`gbearos-util-dist-mirror`**) so `dist/GbearOS_PB1_Core.cs` and `dist/GbearOS_PB2_Display.cs` match the deployed `script.cs` files.
- Perform the **semantic spot-check** on those artifacts (channel string literals, DTO type names, envelope mechanics) against `GbearOS_Shared` as described in `artifact_verification.md`.

