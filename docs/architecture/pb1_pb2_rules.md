> **AI Summary:** Mandatory split-boundary rules—what PB1 may automate versus what PB2 may render and configure.

# PB1 / PB2 architectural rules

## Purpose

Define strict boundaries between **PB1** (automation / telemetry producer) and **PB2** (display consumer). These rules are **mandatory** for changes that stay compatible with the GbearOS split.

---

## PB1 — Core logic (programmable block 1)

### Responsibilities

- Scan grid blocks on the construct.
- Count items and run refinery / ice / power automation as implemented.
- Build **DTOs** (data-only objects).
- Serialize inner DTOs with **`Serializer.Serialize`** (→ **`IGCSerializer`**), wrap with **`SenderEnvelope.Wrap`** for PB2-bound DTO channels, and broadcast on IGC.
- Register listeners for any reverse channels the script uses (e.g. optional PB2 → PB1 tags).
- Use `Echo` for **operational** terminal output where implemented (status dashboard, config/network warnings such as missing **`SharedKey`**). Verbose traces belong behind **`EnableDebug`**.

### Forbidden

PB1 **must not**:

- Write to LCDs or modify text panel contents for UI output.
- Render UI sprites or layout for dashboards.
- Rely on **PB2 → PB1** IGC payloads to drive automation unless a dedicated, documented handler exists (a registered listener that only **drains** the queue is acceptable).
- Use `IMyTextPanel` or `WriteText` for presentation.
- Depend on PB2 internals or assume PB2 is present beyond IGC contracts.

### Update frequency

- Runs with **`UpdateFrequency.Update10`**.
- Must remain deterministic and within instruction limits; cache block lists and avoid heavy per-tick full-grid scans.
- PB1 uses a **macro tick** (`_tickCounter % 5`) that advances **only after** each phase’s subsystem reports a **full pass** complete (`InventoryPassStep`, `RefineryPassStep`, `IcePassStep`, `PowerPassStep`, then network). Heavy inventory/refinery work is **sliced** using `Runtime.CurrentInstructionCount` against an internal cap (below the engine limit) plus **`_lastProcessedIndex`** continuation so partial passes do not advance the macro tick.
- Block list refresh (`GetBlocks` + cargo/refinery caches) runs when the macro phase is **`0`** and **`InventoryScanner.InvPassIdle()`** so multi-tick inventory passes do not re-scan the entire grid every frame.

---

## PB2 — Display engine (programmable block 2)

### Responsibilities

- Receive IGC messages (`UpdateType.IGC` and during `Update10` processing as implemented).
- Load **`[Network]`** from **this** programmable block’s Custom Data (`EnableNetwork`, **`SharedKey`**) on init; see [user_config_system.md](./user_config_system.md) and [configuration.md](../configuration.md).
- Verify **`SenderEnvelope`** where applicable, then parse inner payloads via **`Serializer.Deserialize`** (→ **`IGCSerializer`**) into shared DTO types.
- Format and render inventory, refinery, power, ice, dynamic inventory, and warning views on LCDs.
- Handle display-only logic (scroll, layout, culling).

### Forbidden

PB2 **must not**:

- Scan inventories or refineries for automation purposes.
- Count items for automation or modify production queues.
- Move items or change block operational state for automation.
- Use `IMyRefinery`, `IMyAssembler`, or inventory mutation APIs for base management.
- Depend on PB1 **internals** beyond **IGC contracts**, **DTO** shapes, and **documented** paired settings (**`SharedKey`**, **`PBID`**).
- Read or modify **PB1** programmable block **Custom Data** (or any non-local automation config). **Exception:** PB2 may read/write **`[Network]`** keys **only** on **its own** programmable block—see [configuration.md](../configuration.md).

### Update frequency

- Registers **`UpdateFrequency.Update10`**.
- Uses a **split loop**: lightweight work every `Update10`, heavier DTO diffing and **single-tag template** rotation **decimated** (see [engine.md](./engine.md)).

---

## Shared rules

- PB1 and PB2 communicate **only** through **IGC** for cross-block data (plus the same construct for panel discovery).
- All structured telemetry uses DTO types agreed for the project; on the wire from PB1 they appear as **`SenderEnvelope`** frames whose **Payload** is serialized with **`IGCSerializer`** (entry point **`Serializer`**).
- Channel tags and message-type strings are centralized in shared **constants** (`IGCChannels`, `IGCMessageTypes`) in repositories that ship them as separate compilation units; merged single-file builds contain the same identifiers.
- **`SYS_STATUS`** is an exception: **plain text**, not the DTO wire format.
- No shared mutable state between scripts except what the game provides (e.g. storage) unless explicitly designed—assume **none**.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [igc_contract.md](./igc_contract.md) | IGC, **`SenderEnvelope`**, **`Serializer`** |
| [configuration.md](../configuration.md) | Custom Data keys and network security |
| [system_overview.md](./system_overview.md) | High-level PB1/PB2 roles |
| [update_frequencies.md](./update_frequencies.md) | Cadence |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
