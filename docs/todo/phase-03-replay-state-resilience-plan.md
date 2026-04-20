## Phase 3 — Replay-state resilience (PB1 reset/recompile friendliness)

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

### Work items
- [ ] Identify the PB2 data structure used to track replay state (per-sender last accepted ticks).
- [ ] Choose an expiry window \(N\) (start with a conservative value aligned to expected telemetry cadence and typical downtime during recompiles).
- [ ] Implement eviction/expiry logic:
  - [ ] Track last-seen time for accepted packets (wall-time or PB2-local tick time).
  - [ ] Periodically evict stale `SenderId` entries to allow fresh baselines.
- [ ] Ensure `SYS_STATUS` handling remains consistent with current SharedKey gating policy.

### Acceptance criteria
- [ ] After PB1 recompile/restart, PB2 re-establishes telemetry reception without manual resets (after \(N\) seconds of sender silence at most).
- [ ] PB2 still rejects messages with invalid MAC or malformed envelopes.
- [ ] Replay-cache does not grow unbounded over long sessions.

### Notes / risks
- Expiry should be based on **time since last accepted packet**, not “incoming timestamp age”.
- Keep logic minimal and allocation-free; run any eviction pass on the already-decimated tick cadence where possible.
