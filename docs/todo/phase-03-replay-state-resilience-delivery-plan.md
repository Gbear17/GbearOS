# Phase 3 replay resilience — delivery branch (pipeline plan)

> **Framing:** Implementation and normative docs are complete on this branch; narrative archive is [`docs/history/phase-03-replay-state-resilience.md`](../history/phase-03-replay-state-resilience.md). This file exists for `gbearos-git-branch-init` baseline grounding only.

## 1. Technical approach

- Land **decision 3A** (silence-based replay baseline expiry, 90s wall clock since last **accepted** envelope per `PBID`) via `SenderEnvelope` + PB2 `IGCParser`, without weakening `SharedKey` gating, MAC verification, or envelope parsing.
- Follow **`gbearos-git-sync`** after branch creation: doc-sync pass, contract gate, MDK build/budget, `dist/` mirror, artifact verification, then commit and push with explicit user approval on the commit message.

## 2. File checklist

- `GbearOS_Shared/network/SenderEnvelope.cs` — `TryParse` signature, `ReplaySilenceExpiryTicks`, `EvictStaleReplayState`
- `GbearOS_PB2_Display/igc_parser.cs` — wall-clock maps, eviction call, `Route` utc snapshot
- `docs/architecture/igc_contract.md`, `docs/architecture/network-layer.md` — replay contract
- `docs/history/phase-03-replay-state-resilience.md`, `docs/history/README.md`, `docs/todo/README.md`, `docs/ROADMAP.md`
- Removed: `docs/todo/phase-03-replay-state-resilience-plan.md` (superseded by history + this delivery note)

## 3. Estimated character impact (MDK baseline, this run)

From `gbearos-util-mdk-build` / deployed UTF-8 byte counts on `script.cs` (`output=auto`):

- **PB1**: 58510 / 100000 (remaining 41490) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: 48407 / 100000 (remaining 51593) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

Delta vs pre–Phase 3 PB2 deploy (~47532 bytes) is expected from replay/eviction logic; headroom remains safe.

## 4. High-risk systems

- **Shared wire:** `SenderEnvelope.TryParse` (all PB2 DTO ingress)
- **PB2 routing:** `IGCParser.ProcessMessages` / `Route` (`SYS_STATUS` vs envelope paths)
