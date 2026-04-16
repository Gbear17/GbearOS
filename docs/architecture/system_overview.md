> **AI Summary:** High-level picture of GbearOS—PB1 automation versus PB2 display, IGC-only coupling, and how the repo may be laid out.

# System overview

## What GbearOS is

**GbearOS** is a **sprite-oriented LCD display and construct automation stack** for **Space Engineers**. Two programmable blocks on the **same grid (construct)** cooperate: one performs scans and automation and publishes telemetry; the other subscribes to that telemetry and renders UI. They do not share memory or direct references—only **IGC** messages.

## Logical roles

| Block | Role |
|-------|------|
| **PB1** | **Automation and data plane:** discovers blocks on the construct, runs refinery / ice / power logic, builds **DTOs**, serializes with **`Serializer`**, wraps PB2-bound traffic in **`SenderEnvelope`**, and **broadcasts** on IGC. Does not write LCDs or parse display-bound DTO replies beyond whatever reverse traffic the script registers for (e.g. optional listener tags). |
| **PB2** | **Presentation plane:** receives IGC (including `UpdateType.IGC`), verifies **`SenderEnvelope`** when configured, deserializes inner payloads with **`Serializer`**, and drives **text panels** via the sprite API. Loads **`[Network]`** on the **PB2** block only ([configuration.md](../configuration.md)). Does not run refinery/inventory automation or mutate production blocks. |

## How the codebase may appear

Depending on which repository or branch you use, you may see:

- **Modular sources** — multiple compilation units merged in a defined order; or  
- **Single-file scripts** — one merged readable artifact per programmable block (some public or release checkouts ship these as the primary source view).

Behavior in-game is defined by the **merged** script contents, not by how many files existed before merging. Repository layout, build order, and tooling are described in the **root `README.md`** of your checkout; this folder documents **behavior and contracts** only.

## Update and performance principles

- Prefer **construct-scoped** terminal queries (`IsSameConstructAs`) so PB2 only touches panels on its own grid.
- **Hot paths:** avoid LINQ and unnecessary allocations; cache block lists where the scripts already do.
- **Separation:** PB2 does **not** re-implement PB1’s scanning or queue management; it **renders** the telemetry PB1 (or any compatible sender) puts on the wire.

## IGC and DTOs (summary)

- **DTO channels** carry an outer **`SenderEnvelope`** frame (`SenderId|Timestamp|Payload|MAC`); the **Payload** is the inner DTO string, produced and consumed through **`Serializer`** / **`IGCSerializer`**. Inner layout is **semicolon-separated**, with **protocol version in the first field** after splitting—see [igc_contract.md](./igc_contract.md). **`SYS_STATUS`** from PB1’s dashboard path is plain text and **not** envelope-wrapped; PB2 still requires a configured **`SharedKey`** to process any **`Route`** traffic.
- **Channel names** are fixed string tags (e.g. broadcast names); the script holds them as constants in a shared **`IGCChannels`**-style contract.
- **`SYS_STATUS`** is **not** DTO-encoded: senders push plain text; PB2 aggregates lines in a small **status registry** for `[STATUS]` panels (subject to PB2 **`SharedKey`** policy—see [igc_contract.md](./igc_contract.md)).

Mandatory boundaries: [pb1_pb2_rules.md](./pb1_pb2_rules.md). PB2 renderer timing: [engine.md](./engine.md), [update_frequencies.md](./update_frequencies.md).

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Full architecture index |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | Mandatory PB1 vs PB2 boundaries |
| [igc_contract.md](./igc_contract.md) | IGC channels, envelope, inner DTO format |
| [configuration.md](../configuration.md) | Custom Data keys (PB1 + PB2 `[Network]`) |
| [engine.md](./engine.md) / [update_frequencies.md](./update_frequencies.md) | PB2 timing and split loop |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
