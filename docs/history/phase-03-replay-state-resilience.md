> **Archive:** Completed Phase 3 plan. **Current behavior:** [`docs/architecture/igc_contract.md`](../architecture/igc_contract.md) (sender envelope / replay), [`docs/architecture/network-layer.md`](../architecture/network-layer.md) (implementation mapping).

# Phase 3 — Replay-state resilience (archived)

## MDK baseline (UTF-8 byte counts on deployed `script.cs`, `output=auto`)

- **PB1** (unchanged vs pre-change tree): 58510 / 100000 (remaining 41490) — `%APPDATA%\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2** (after Phase 3): 48407 / 100000 (remaining 51593) — `%APPDATA%\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

## Delivery summary

- **3A (silence-based expiry):** `SenderEnvelope.TryParse` accepts optional wall-clock tracking per sender; after `ReplaySilenceExpiryTicks` (90s) with no **accepted** packet, PB2 drops the replay baseline for that `PBID` so a recompiled PB1 can send lower `UtcNow.Ticks` again. MAC verification and parse order are unchanged.
- **Bounded cache:** `SenderEnvelope.EvictStaleReplayState` removes expired senders; `GbearOS_PB2_Display/igc_parser.cs` invokes it at the start of each `ProcessMessages` pass using a reusable scratch list.

## Original plan narrative (historical)

### Decisions (locked)

- **3A**: **Silence-based expiry** — PB2 expires per-`SenderId` replay baselines after \(N\) seconds since last **accepted** packet. MAC verification remains mandatory.

### Goal

Avoid “permanent offline” behavior when PB1 is recompiled/restarted and PB2’s replay cache rejects packets due to timestamp monotonicity assumptions.

### Scope (what this phase covers)

- PB2 replay-cache policy for typed DTO channels wrapped in `SenderEnvelope`:
  - Maintain last accepted timestamp per `SenderId`.
  - Reject packets with non-increasing timestamps **unless** the sender’s baseline has expired due to silence.
- Define “silence” as **no accepted messages** for a given `SenderId` for the expiry window.
- Ensure the change does **not** weaken:
  - SharedKey gating,
  - MAC verification,
  - envelope parsing correctness.

### Acceptance criteria (met)

- After PB1 recompile/restart, PB2 re-establishes telemetry reception without manual resets (after \(N\) seconds of sender silence at most, \(N = 90\) wall seconds).
- PB2 still rejects messages with invalid MAC or malformed envelopes.
- Replay-cache does not grow unbounded over long sessions (`EvictStaleReplayState`).

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
