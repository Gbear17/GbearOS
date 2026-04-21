> **AI Summary:** User-facing LCD layout—how PB2 discovers `[GbearOS]` panels and renders module commands from panel Custom Data.

# LCD panels and layout

## Purpose

User-facing layout for PB2-driven **LCDs**. PB2 discovers panels by a **name tag** (`[GbearOS]`) and then renders **modules** based on a **command list** stored in the panel’s **Custom Data**. This is separate from the **PB2 programmable block** **`[Network]`** INI—see [configuration.md](../configuration.md).

PB2 must run on the **same construct** as the panels it discovers (`IsSameConstructAs` the PB2 programmable block).

---

## Virtual layout engine (`[GbearOS]`)

For **PB2-controlled** panels (single-module or multi-module dashboards):

1. Put **`[GbearOS]`** in the panel **custom name**.
   - Optional: **`[Manual]`** in the name excludes the panel from PB2 auto-discovery.
2. In **Custom Data**, list **commands** one per line, e.g. `[HEAD:My Base]` then `[INV]`, `[REF]`, `[PWR]`, etc.
   - **`[HEAD:...]`** — optional title line(s) at the top.
   - **`[INV]`**, **`[REF]`**, **`[ICE]`**, **`[PWR]`**, **`[WARN]`**, **`[STATUS]`** — modules; **`[INV:Subtype]`** filters to one material (mini view).
   - If Custom Data is empty/whitespace, PB2 defaults the command list to **`[INV]`**.

PB2 parses these commands, renders **subheaders** (`--- MODULE NAME ---`), and runs **virtual scrolling** when content exceeds the viewport.

**Implementation details:** [engine.md](./engine.md), [pagination.md](./pagination.md).

---

## Configuration summary

- **PB1 programmable block** — Full INI in **Custom Data** (automation, display filters, **`[Network]`**, etc.). Canonical key reference: [configuration.md](../configuration.md). Behavior and persistence rules: [user_config_system.md](./user_config_system.md).
- **PB2 programmable block** — **`[Network]`** only in **Custom Data** (`EnableNetwork`, **`SharedKey`**); must match PB1 for IGC. See [configuration.md](../configuration.md).
- **PB2 `[GbearOS]` panels** — Panel **Custom Data** defines **module order** and optional **HEAD** titles / **INV** filters (not the same field as the PB2 script block’s network INI). Empty Custom Data on a `[GbearOS]` panel defaults to inventory.

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [configuration.md](../configuration.md) | PB1 full INI; PB2 script **`[Network]`** |
| [user_config_system.md](./user_config_system.md) | Custom Data persistence rules |
| [engine.md](./engine.md) | Split loop, virtual viewport, culling |
| [pagination.md](./pagination.md) | Virtual scroll behavior |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
