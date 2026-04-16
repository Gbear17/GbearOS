> **AI Summary:** Documents how often PB1 and PB2 enter Main and how heavy work is throttled to fit in-game tick budgets.

# Update frequencies and performance constraints

## Purpose

Summarize how often each programmable block **enters** `Main` and how **heavy work** is scheduled, aligned with the current scripts.

---

## PB1 — `Update10`

PB1 uses **`Runtime.UpdateFrequency = UpdateFrequency.Update10`** (every 10 ticks).

PB1 should:

- Avoid scanning the entire grid every tick; **cache** block lists where the implementation already does.
- On every **fifth** `Update10` (`_tickCounter % 5 == 0`), PB1 reloads the terminal list with `GetBlocks` and rebuilds **typed** `_cachedCargo` and `_cachedRefineries` once for that snapshot. Inventory and refinery work on the other ticks consumes those lists (and the shared `_globalBlocks` snapshot) without re-classifying blocks for cargo/refinery roles.
- **Inventory extract (`InvExtractSlice`)** runs inside the inventory phase as **five resumable sub-steps** (`_exPart` 0–4), bounded by `CurrentInstructionCount` and a per-slice **move budget** (instruction safety):
  - **`_exPart == 0`:** Critical — drain **`[Component]`**-tagged cargo inventories to **primary `[Cargo]`** only (can resume next tick). Uses the default **`storageMoveBudget` of 5** transfers per slice.
  - **`_exPart == 1`:** Critical — drain **`[Ingot]`**-tagged cargo inventories to **primary `[Cargo]`** only. Default **5** moves per slice.
  - **`_exPart == 2`:** Assembler **output** — move **components** via full storage routing (`MoveItemToStorageAt`). Default **5** moves per slice.
  - **`_exPart == 3`:** Refinery **output** (`GetRefineryOutputInventory` → refinery **output** slot, never the ore input) — move **all stacks** in that inventory via full storage routing (`MoveItemToStorageAt` with refinery input blocked as a destination). Uses a higher **`refineryExtractBudget` of 20** moves per slice so refineries drain faster while other phases stay at **5** for tighter instruction headroom.
  - **`_exPart == 4`:** Assembler **input** clog — when input volume **≥ 90%** of max, drain **ingots** to **primary `[Cargo]`** only until **&lt; 90%** or budget exhausted (resumable across assemblers). Default **5** moves per slice.
- **Torch PB Limiter:** `Main` calls **`PBLimiter(argument)`** first; while **`DateTime.UtcNow` &lt; `_pblTime`**, the script **returns immediately** (no `U()`, IGC, or status broadcast). If **`Main`** catches an exception whose type name contains **`GracefulShutDown`** (Torch Advanced PB Limiter), PB1 sets **`_pblTime = UtcNow + 60s`** and stores **`_pblReason`**, then returns.
- Reuse DTO instances where practical; avoid string concatenation in tight loops and avoid unnecessary allocations.
- Keep automation and serialization bounded so the tick stays within game limits. DTO bodies use **`Serializer`** / **`IGCSerializer`**; PB1 wraps outbound DTO traffic with **`SenderEnvelope`** per [igc_contract.md](./igc_contract.md).

PB1 may:

- Scan inventories, manage refineries, ice, and power according to its internal scheduling.

PB1 must not:

- Render dashboards or write LCD text (see [pb1_pb2_rules.md](./pb1_pb2_rules.md)).

---

## PB2 — `Update10` with decimated heavy path

PB2 also registers **`Update10`**, not `Update100`. It still **throttles** expensive work:

- **Every** `Update10`: virtual scroll animation, selective redraws, panel refresh countdown handling.
- **Every tenth** `Update10` (`_tick10 % 10 == 0`): DTO dirty-flag passes, phased **single-tag template** rendering (`[INV]` / `[REF]` / … panels—see [engine.md](./engine.md)), and some telemetry echo/status LCD updates.

So PB2 **enters** at the same coarse cadence as PB1’s `Update10`, but **CPU-heavy** display passes run at roughly **one-tenth** of that rate. Details: [engine.md](./engine.md).

PB2 should:

- Process incoming IGC when delivered (`UpdateType.IGC`) and during its `Update10` path as implemented.
- Stay free of automation and inventory APIs.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [engine.md](./engine.md) | PB2 split loop and renderer responsibilities |
| [igc_contract.md](./igc_contract.md) | IGC channels, envelope, cadence notes |
| [configuration.md](../configuration.md) | **`[Network]`**, **`SharedKey`**, **`EnableNetwork`** |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | PB1 vs PB2 separation of duties |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
