> **Note:** This document is a **developer release gate**, not a player-facing contract. It implements **Phase 1 (decision 1A)** from the archived checklist [`docs/history/phase-01-contract-build-output-reconciliation.md`](../history/phase-01-contract-build-output-reconciliation.md): `GbearOS_Shared` is canonical; **minified** PB outputs are **spot-checked** for semantic alignment, not byte-compared.

# Artifact verification (shared contracts vs PB1/PB2 build outputs)

## Purpose

Give a repeatable way to show that **IGC wire contracts** in **`GbearOS_Shared`** are what end up in **MDK2-deployed minified scripts** (`script.cs`) and in **`dist/`** mirrors, without requiring identical minified source text.

## Canonical contract inventory (source of truth)

| Concern | Location |
|---------|----------|
| IGC broadcast channel tags | `GbearOS_Shared/igc/channels.cs` (`IGCChannels`) |
| Inner DTO `messageType` strings | `GbearOS_Shared/igc/message_types.cs` (`IGCMessageTypes`) |
| Outer frame wrap/parse, MAC, replay | `GbearOS_Shared/network/SenderEnvelope.cs` |
| Protocol version, field order, escaping | `GbearOS_Shared/igc/igc_serializer.cs` |
| `Serializer.Serialize` facade | `GbearOS_Shared/igc/serialization.cs` |
| PB2-only `Deserialize<T>` | `GbearOS_PB2_Display/igc_serializer_deserialize.cs`, `GbearOS_PB2_Display/serialization_deserialize.cs` only |

PB1 sends and PB2 receives using **`IGCChannels`** / **`IGCMessageTypes`** identifiers from the shared compilation unit included by both `.csproj` files (see [`contract_checklist.md`](./contract_checklist.md)).

## Automated gates (every sync / release)

Run in order (Cursor project skills under `.cursor/skills/`; `gbearos-doc-sync` is an orchestrator, then `gbearos-util-contract-check` and other utilities; `gbearos-git-sync` orchestrates the full sequence including commit/push after those gates):

1. **`gbearos-util-contract-check`** — shared files present, PB2-only deserialize not in `GbearOS_Shared/`, both projects glob-include `GbearOS_Shared`, PB1 does not reference deserialize entrypoints.
2. **`gbearos-util-mdk-build`** — runs **`dotnet build`** (MDK2 deploys minified `script.cs` for PB1 and PB2 per `mdk.local.ini` `output=`) and reports **UTF-8 character budget** vs 100000 for both deployed scripts.
3. **`gbearos-util-dist-mirror`** — copy deployed scripts to `dist/GbearOS_PB1_Core.cs` and `dist/GbearOS_PB2_Display.cs`.
4. **`gbearos-util-artifact-verification`** — from the repo root, run `.cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1` via PowerShell (see that skill for exact invocation). Exit code **0** means the checks in § Semantic spot-checks below passed on `dist/`; **1** = failed checks; **2** = missing `dist/` inputs. **Do not** replace this step with ad-hoc greps of `dist/*.cs`—the script owns minified verification.

**`gbearos-git-sync`** (git sync / `git-sync`) runs **`gbearos-doc-sync`** first, then steps 1–4 below, before any commit/push gate.

## Semantic spot-checks (minified `script.cs` or `dist/*.cs`)

MDK **minifies** identifiers; **string literals for channel names and DTO type names must still appear verbatim** (they are not renamed). After a build:

1. **Channel parity** — Search each artifact for IGC channel substrings from `channels.cs`. **PB1** must contain all constants including `PB2ToPB1` (listener registration). **PB2** embeds the listener tags it registers (it does **not** contain `PB2ToPB1`, which is PB1-only). The script at `.cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1` encodes this split.
2. **DTO message types** — MDK minification may strip runtime DTO **name** strings on PB2; rely on **protocol `Split(';')` counts** and shared serialize constants in source review rather than grepping `*DTO` in minified PB2 unless your build still contains them.
3. **Envelope mechanics** — Confirm minified output still contains the logical pieces of `SenderEnvelope` (e.g. FNV-style hash routine, four-part `|` split, Base64 decode path, `X8` MAC width). Exact local variable names differ after minification.
4. **Inner protocol prefix** — Shared serializer uses `ProtocolVersion` = `"1"` as the first semicolon-separated field for typed DTO bodies; spot-check that the serialized pattern is still consistent with [`igc_contract.md`](./igc_contract.md) (no need to grep the private constant name if `1;` field order is unchanged in source review).

## Cadence and scheduling invariants

Phase 1 does **not** change gameplay timing. **PB1** phased passes and **PB2** decimated render work remain documented in [`update_frequencies.md`](./update_frequencies.md) and [`engine.md`](./engine.md). If future edits touch `Program.cs`, `igc_parser.cs`, or the PB2 renderer loop, re-verify those docs still match behavior.

## Drift and known mismatches

If spot-checks fail or contracts diverge, fix the **source** in `GbearOS_Shared` / project layout first, rebuild, and re-run this page. If a temporary mismatch must ship, record it in the phase todo or a short note under `docs/todo/` until resolved.

## Related documents

| Document | Role |
|----------|------|
| [`contract_checklist.md`](./contract_checklist.md) | Structural gate: includes, PB2-only files, PB1 deserialize ban |
| [`igc_contract.md`](./igc_contract.md) | Normative wire format |
| [`network-layer.md`](./network-layer.md) | Ingress and **`PBID`** / sender-identity policy |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
