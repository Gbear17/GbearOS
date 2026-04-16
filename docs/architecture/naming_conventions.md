> **AI Summary:** Naming patterns for scripts, DTOs, IGC identifiers, and public types across modular and merged builds.

# Naming conventions

## Purpose

Consistent naming for programmable block scripts, DTOs, IGC identifiers, and public types so automation and display code stay readable across **modular** and **single-file** checkouts.

Physical **file names** and **merge order** belong in the repository root **`README.md`** (private dev tree); this document covers **logical** naming only.

---

## 1. Programmable block scripts

Each ship/station uses **two** scripts in-game:

- **PB1** — automation / telemetry producer (`Program` entry in the PB1 merged script).
- **PB2** — display consumer (`Program` entry in the PB2 merged script).

Supporting types and partial logic may live in additional types in the same merged compilation unit; prefer **one public `Program` class** per block as Space Engineers expects.

---

## 2. Class names

Use **PascalCase** for all classes.

Examples: `RefineryStatusDTO`, `IGCChannels`, `IGCSerializer`, `Serializer`, `SenderEnvelope`, `LCDRenderer`.

---

## 3. DTO naming

DTOs end with **`DTO`**, are **reference types** holding telemetry fields, and avoid behavior beyond trivial defaults.

Examples: `RefineryStatusDTO`, `InventorySummaryDTO`, `WarningDTO`.

---

## 4. IGC channel names

Channel tag strings are **`ALL_CAPS_WITH_UNDERSCORES`**, matching the constants in **`IGCChannels`** (e.g. `SYS_STATUS`, `PB1ToPB2_InventorySummary`).

---

## 5. IGC message type strings

`IGCSerializer.Deserialize` keys off the **managed type name** string (e.g. `"InventorySummaryDTO"`). **`IGCMessageTypes`** mirrors those literals for send/receive sites.

---

## 6. Function names

Use **PascalCase** for methods; prefer **verb-first** where it reads naturally in C# (`ScanIce`, `Serialize`, `RenderAll`).

---

## 7. Variable names

Use **camelCase** for locals and fields that are not constants (`refineryList`, `scrollPos`).

---

## 8. Constants

Use **`ALL_CAPS_WITH_UNDERSCORES`** for `const` values that represent fixed protocol or tuning values exposed as named constants.

---

## 9. File names (modular repositories only)

When sources are split across files, use **lowercase_with_underscores.cs** for modules (e.g. `inventory_scanner.cs`, `lcd_renderer.cs`). Entry points are commonly a single `Program` class in an entry file (for example `Program.cs` in MDK2 projects). Exact paths are not normative here.

---

## 10. PB1 / PB2 prefixes

Optional prefixes on private fields (`_pb1…`, `_pb2…`) are acceptable for clarity; **shared** serialization and DTO types must **not** embed PB role in their public names except where the type is inherently directional (e.g. channel constants).

---

## 11. Warning and log keys

Human-facing or diagnostic keys for warnings often use **`ALL_CAPS`** tokens in messages (`LOW_ICE`, `LOW_POWER`) as implemented in the warning pipeline.

---

## 12. LCD tag naming

Panel **CustomName** substrings use **bracketed tags** (`[INV]`, `[WARN]`, `[GbearOS]`, …). PB2 matching is **substring**-based; typos break discovery.

---

## 13. IGC DTO wire format (naming-level summary)

**Inner** DTO payloads (the **Payload** inside **`SenderEnvelope`** on PB1→PB2 channels) are a **single string**:

- **Top-level fields:** separated by **`;`**.
- **First field** after split: **protocol version** (currently `"1"`). Mismatch → default DTO on deserialize.
- **Embedded string arrays:** elements joined with **`|`**; use **`\|`** and **`\\`** escapes inside elements per **`IGCSerializer`**.
- **Embedded float/bool arrays:** joined with **`|`** without escape processing.

Do **not** use JSON for DTO bodies. Full field order and envelope rules: [igc_contract.md](./igc_contract.md).

---

## 14. Terminal output (`Echo`)

- **PB1:** Use `Echo` for **operational** status and documented warnings (see [pb1_pb2_rules.md](./pb1_pb2_rules.md)); gate verbose traces with **`EnableDebug`** in config ([configuration.md](../configuration.md)).
- **PB2:** `Echo` for **errors** or decimated telemetry summaries; avoid spamming every `Update10` unless gated.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [igc_contract.md](./igc_contract.md) | Channels, **`Serializer`**, **`SenderEnvelope`** |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | PB1 vs PB2 boundaries |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
