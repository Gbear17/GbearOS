> **AI Summary:** How PB1 should detect and persist modded ores and blocks without coupling PB2 to block implementations.

# Modded block support

## Purpose

How **PB1** should detect, classify, and interact with modded blocks in Space Engineers so telemetry remains accurate without coupling **PB2** to block implementations. PB2 **never** classifies blocks; it only renders DTOs.

---

## 1. General rules

**PB1 should:**

- Detect modded blocks using **SubtypeId**, not display names.
- Classify using **interfaces** (`IMyRefinery`, `IMyAssembler`, `IMyBatteryBlock`, `IMyCargoContainer`, …).
- Treat modded blocks like vanilla unless a dedicated exception exists.
- Use shared **block/item helper** utilities where the codebase provides them for subtype checks.

**PB1 must not:**

- Rely on player-visible block names for logic.
- Assume vanilla-only subtype IDs or inventory layouts.
- Perform LCD or sprite work.

---

## 2. Supported modded categories (conceptual)

PB1 is expected to tolerate modded variants of:

- **Production:** refineries, assemblers, arc/blast/chemical processors, printers.
- **Power:** batteries, reactors, hydrogen engines, solar, fusion-style producers.
- **Storage / utility:** cargo, O2/H2 generators, irrigation-style blocks, ice consumers.

PB1 treats unexpected subtypes as **candidates** for generic interfaces, not hard failures.

---

## 3. Classification heuristics (examples)

Rules are illustrative of the style used in code; authoritative behavior is the implementation.

| Kind | Typical test |
|------|----------------|
| Refinery | `IMyRefinery` and/or subtype id contains `Refinery`, `Furnace`, … |
| Assembler | `IMyAssembler` and/or subtype id contains `Assembler`, `Factory`, … |
| Battery | `IMyBatteryBlock` |
| Reactor | `IMyReactor` |
| Engine / generator | `IMyPowerProducer` + subtype id contains `Engine` / `Generator` |
| Irrigation | CustomName tag and/or subtype id contains `Irrigation`, `Hydroponic`, … |
| Cargo | `IMyCargoContainer` |

---

## 4. Modded items

PB1 discovers items via **`MyItemType`**; dynamic rows use **`InventoryDynamicDTO`** on the wire. PB2 lists dynamic entries without assuming a fixed vanilla set.

---

## 5. Ice distribution

Modded consumers of ice should still flow through the same **ice totals / targets** model so **`IceStatusDTO`** reflects generator vs irrigation vs cargo splits where the scan includes those blocks.

---

## 6. Performance

PB1 should cache block lists, avoid full-grid rescans every tick, and keep **`Update10`** work bounded. PB2 performs **no** block scans for automation.

---

## 7. Serialization

Modded dynamic rows use the same **inner** DTO **delimiter rules** as other payloads ([igc_contract.md](./igc_contract.md)): semicolon-separated fields, **`|`** array joins, no JSON. On PB1→PB2 DTO channels the inner string is carried inside **`SenderEnvelope`**; PB2 verifies the envelope (when configured) before **`Serializer.Deserialize`**.

---

## 8. Safety

Skip blocks with missing or unusable inventories; never assume modded blocks expose vanilla-perfect behavior.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [igc_contract.md](./igc_contract.md) | Wire format, **`SenderEnvelope`**, **`Serializer`** |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | PB1 scan vs PB2 display |
| [update_frequencies.md](./update_frequencies.md) | **`Update10`** constraints |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
