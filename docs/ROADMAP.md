> **Note:** This document captures **speculative** / forward-looking ideas for the broader **GbearOS ecosystem**. Items here may be re-scoped, postponed, or abandoned without notice; treat it as planning notes, not a player-facing commitment.

# Roadmap and future architecture

## NOC architecture (network operations)

- [x] Infrastructure phase — Dual-homed Git routing (private dev vs public release workflow).
- [x] NOC phase 1 — Stateless envelope on DTO channels: **SenderEnvelope** (`SenderId|Timestamp|PayloadB64|MAC`), **FNV-1a** MAC, per-sender replay window on PB2; config: **`SharedKey`**, **`SenderId`**, **`EnableNetwork`**. See [architecture/network-layer.md](architecture/network-layer.md) and [architecture/igc_contract.md](architecture/igc_contract.md).
- [ ] NOC phase 2 — Stronger-than-FNV authentication / key lifecycle (if required beyond current integrity check).
- [ ] NOC phase 3 — Multi-hop IGC telemetry routing via laser antennas.

## UI and presentation framework (PB2)

- [ ] Refactor current static templates into reusable, dynamic display modules.
- [ ] Expand template library (target: **~90%** high-performance sprite-based, **~10%** text-based mini-stats).
- [ ] Introduce extensive **mini-stat** commands for inline LCD data injection.

## Physics and compute coprocessor (PB3)

- [ ] Introduce **PB3** on the network as a **heavy-math coprocessor** to protect PB2’s **~10 Hz** render cadence.
- [ ] Implement kinematic projections: stopping distance, time-to-stop, and collision-vector forecasting (“will I hit something if I don’t brake?”).

## Unscheduled enhancements

_Use this subsection for opportunistic improvements (tooling, docs, perf experiments) that do not yet belong to a named phase above._

- [ ] **Script size guard** — After material changes, run **`python C:\SpaceEngineers1\shared\build\deploy.py --target-repo .`** (from this repo root) and confirm **`dist/minified/PB1_min.cs`** and **`PB2_min.cs`** stay under the **100,000** character engine limit. Latest minified snapshot: both scripts are **under** the cap; PB1 occupies the higher **~85k–100k** risk band, so re-check when adding PB1 logic or shared code it pulls in.

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
