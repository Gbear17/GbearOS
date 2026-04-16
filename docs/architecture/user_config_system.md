> **AI Summary:** Normative rules for INI Custom Data on each programmable block—what may be stored, rewritten, and validated.

# User configuration system (PB1 / PB2)

## Purpose

Describe how GbearOS loads, validates, and persists **INI-style Custom Data** on programmable blocks. For a **complete key list** (types, defaults, clamps, and network security), see **[`docs/configuration.md`](../configuration.md)**.

This document states **normative behavior** for the split-script architecture: what each block may store, when the file is rewritten, and how invalid input is corrected.

---

## Configuration surfaces

| Block | Custom Data | Authority |
|-------|-------------|-----------|
| **PB1** (orchestrator) | Full INI: `[IngotTargets]`, `[IceTargets]`, `[ReactorTargets]`, `[BatteryThresholds]`, `[RefinerySettings]`, `[BlockTags]`, `[DisplayFilters]`, `[Debug]`, `[Network]` | **Authoritative** for automation and telemetry options. |
| **PB2** (display) | **`[Network]` only:** `EnableNetwork`, `SharedKey` | **Not** a copy of PB1 config; must **match PB1** for `SharedKey` (and optional `EnableNetwork` policy). LCD **panel** Custom Data (`[GbearOS]` command lists) is separate—see [lcd_panels_and_layout.md](./lcd_panels_and_layout.md). |

---

## PB1 — load, validate, persist

### When Custom Data is read and written

- On **script construction** (`LoadConfig`), PB1 parses Custom Data (or starts from empty if parse fails), merges defaults, validates, then **writes back** a canonical INI string to **PB1** Custom Data.
- **Persistence paths** (`WriteDefaultsIfMissingOrInvalid`, `TryPersistModdedIngotTargets`) also **read → validate → write** so the stored file always matches the in-memory `Config`.

### Strict template enforcement

`WriteConfigToIni` begins with **`_ini.Clear()`**, then emits **only** keys owned by `ConfigParser`. Therefore:

- **Unknown sections and keys** are **removed** on the next save (they are not retained).
- **User-added freeform comments** outside script `SetComment` strings are **not** preserved.
- **Ordering** is the canonical order produced by the writer, not the user’s original order.

Parse failure (`TryParse` false) clears the in-memory INI before defaults are applied, then a fresh template is written.

### Read semantics (while parsing)

- **Missing sections or keys** use defaults defined in `ReadConfig` / `Config` (see [`configuration.md`](../configuration.md)).
- **Keys** are **case-insensitive** (MyIni).
- **String values** for tags and network fields are **trimmed**; empty `SenderId` after trim becomes **`CMD-DEFAULT`**.
- **Unknown keys** in a section may be read if present (e.g. dynamic `[IngotTargets]` names); anything **not** re-emitted by `WriteConfigToIni` is **dropped** on save.

### Validation (correction, not abort)

- **Non-negative** (`ClampNn`): ice, uranium, solar minimum, refinery hysteresis, ingot target values; negatives become **0**.
- **Unit interval** (`Clamp01`): battery/reactor/engine fraction keys in `[BatteryThresholds]`; values clamp to **[0, 1]**.
- Invalid numbers are treated as missing via `ToDouble(default)` / `ToBoolean(default)` as implemented in code.
- Execution **never** stops solely due to bad config; values are corrected and the template is rewritten.

---

## PB2 — network bootstrap only

- On **PB2** `Init`, `LoadNetworkSharedKeyFromCustomData` parses the **PB2** programmable block’s Custom Data, ensures **`[Network]`** keys exist, writes a small template (including script comments), and caches **`SharedKey`** and **`EnableNetwork`**.
- PB2 **does not** parse PB1’s INI, refinery tags, or display filters from PB1 Custom Data.
- PB2 **must not** modify **PB1** Custom Data. Writing **its own** block’s Custom Data for the network template is allowed.

### IGC routing gate

If **`SharedKey`** is null or empty after trim, PB2’s message **`Route`** returns immediately: **no** `SYS_STATUS` updates, **no** DTO ingestion—see [`configuration.md`](../configuration.md) and [`igc_contract.md`](./igc_contract.md).

---

## Section reference (PB1)

Behavioral summary only; defaults and types are in [`configuration.md`](../configuration.md).

| Section | Role |
|---------|------|
| `[IngotTargets]` | Dynamic keys → ingot target masses; vanilla seeds when section empty. |
| `[IceTargets]` | Generator/irrigation ice targets, cargo reserve, minimum cargo ice for distribution. |
| `[ReactorTargets]` | Uranium targets for large/small reactors. |
| `[BatteryThresholds]` | Reactor/engine charge bands, battery charge/discharge targets, `EnablePowerAutomation`, `SolarMinimumOutput`. |
| `[RefinerySettings]` | `EnableRefineryBalancing`, `RefineryHysteresis`. |
| `[BlockTags]` | `IrrigationTag`, `ManualTag`. |
| `[DisplayFilters]` | Category booleans consumed on PB1 when building telemetry. |
| `[Debug]` | `EnableDebug`. |
| `[Network]` | `EnableNetwork`, `SenderId`, `SharedKey`. |

**Refinery priority lines** on the wire (`RefineryStatusDTO.priorityLine1` / `priorityLine2`) are **computed** in PB1 refinery logic from ore targets and balancing—they are **not** configured via a separate `[RefineryPriorities]` INI section in the current `ConfigParser`.

---

## Performance and safety

- PB1 avoids re-parsing Custom Data every tick; config is loaded at construction and updated when persistence helpers run (e.g. after scans that add modded ingot keys).
- PB1 **must not** use PB2-only concerns when parsing; PB2 **must not** drive automation from its network INI beyond enable/key policy.

---

## Summary

| Rule | PB1 | PB2 |
|------|-----|-----|
| Owns full base automation config | Yes | No |
| Reads `[Network]` on **this** block | Yes | Yes |
| Rewrites Custom Data to canonical template | Yes (full INI) | Yes (`[Network]` only, on Init) |
| Unknown INI keys preserved | No (dropped on PB1 save) | N/A |

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [configuration.md](../configuration.md) | Full INI key reference, validation, security |
| [igc_contract.md](./igc_contract.md) | How **`[Network]`** keys relate to IGC |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | Which block owns which config surface |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
