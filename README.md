# GbearOS (Space Engineers)

**Copyright (c) 2026 Garrett Wyrick.** Licensed under GPL-3.0 — see [`LICENSE`](LICENSE).

GbearOS is a **two-programmable-block system** for **Space Engineers**: **PB1 (Core)** scans the grid, runs automation (refinery, ice, power), packs telemetry into shared **DTOs**, and broadcasts them over **IGC**; **PB2 (Display)** verifies incoming traffic and draws **sprite-based LCD dashboards** from that data on about a **10 Hz** display cadence.

---

## Player quick start

1. **Install the scripts** — If your copy of the repo includes a `dist/` folder with release **minified** scripts, copy them into Space Engineers local programmable-block scripts as you normally would. **This tree may not ship `dist/`**; if it is missing, build and deploy from source (one line: [Developer setup](#developer-setup) → [`docs/setup.md`](docs/setup.md)).
2. **Tag the blocks** — Add **`[GbearOS]`** to each programmable block’s **name** (or Custom Data where your workflow expects it) so PB1/PB2 participate in the script’s discovery rules. LCDs use name tags like **`[INV]`** / **`[GbearOS]`** as described under [LCD tag guide](#lcd-tag-guide).
3. **Networking handshake** — In **Custom Data**, set the same **`SharedKey`** on **PB1** and **PB2** under **`[Network]`** (see [Connectivity & security](#connectivity--security)). Empty or mismatched keys cause telemetry to be dropped at the gate.
4. **Wire the LCDs** — Use the command tags below on text panels (names or panel Custom Data for virtual dashboards). PB2 discovers panels on the **same construct** as the PB2 block.

| Tag / command | What it drives |
|---------------|----------------|
| `[INV]` | Inventory summary: capacity bar, ore/ingot columns, components (and dynamic rows when data is present). Optional **`[INV:Subtype]`** filters to one material. |
| `[REF]` | Refinery queues, balancing hints, per-refinery rows. |
| `[PWR]` | Battery / reactor / engine bars and grid power snapshot. |
| `[ICE]` | Ice targets vs actual mass across generators, irrigation, and cargo. |
| `[WARN]` | Warning vs nominal channel (paired with PB1 warning DTO). |
| `[STATUS]` | System / status strip (when implemented in the virtual dashboard). |
| `[HEAD:Title]` | Optional title line(s) on **`[GbearOS]`** multi-module panels. |
| `[COL]` / `[COL:LEFT]` / `[COL:RIGHT]` / `[COL:FULL]` | Column layout for the virtual dashboard (see [`docs/architecture/lcd_panels_and_layout.md`](docs/architecture/lcd_panels_and_layout.md)). |

---

## Features

What you get on the wire and on screen maps directly to the shared **DTOs** and PB2 **LCDRenderer** modules:

- **Inventory** — `InventorySummaryDTO` + `InventoryDynamicDTO`: stock bars, per-category visibility (ores, ingots, components, ammo, dynamic buckets) respecting PB1 **`[DisplayFilters]`**.
- **Refinery** — `RefineryStatusDTO`: priority ore, hysteresis-friendly balancing state, per-block rows.
- **Ice** — `IceStatusDTO`: generator and irrigation targets vs actuals, cargo reserve, **LOW ICE** semantics when below configured thresholds.
- **Power** — `PowerStatusDTO`: fractional charge bands, solar floor, optional automation toggles (configured on PB1).
- **Warnings** — `WarningDTO` + dedicated **`[WARN]`** rendering; formatter may show **`(no data)`** when a slice has not arrived yet.
- **LCD engine** — Script-drawn sprites (monospace / simple textures), virtual scrolling on tall **`[GbearOS]`** dashboards, **NO SIGNAL** full-screen state when PB2 has no valid telemetry (see [Standalone vs. system mode](#standalone-vs-system-mode)).

Normative contracts and deeper behavior: [`docs/architecture/`](docs/architecture/).

---

## Installation

- **Release layout:** Some distributions include **`dist/`** with merged or minified `*.cs` for paste into the Space Engineers script editor—use those when present.
- **From this repository:** There may be **no** `dist/` folder. Use **MDK2** + **.NET 4.8**, copy `mdk.local.ini.example` → `mdk.local.ini` for each project, then run **`dotnet build`** from the repo root so MDK deploys to your local `IngameScripts` path. Step-by-step: [`docs/setup.md`](docs/setup.md).

---

## LCD tag guide

- **PB2-driven panels** — Put **`[GbearOS]`** in the panel **custom name** so PB2 discovers it. Optional **`[Manual]`** in the name excludes the panel from discovery.
- **Commands live in panel Custom Data** — In the panel’s **Custom Data**, list commands **one per line**: optional **`[HEAD:…]`**, then modules such as **`[INV]`**, **`[REF]`**, **`[ICE]`**, **`[PWR]`**, **`[WARN]`**, **`[STATUS]`**, and column directives **`[COL]`** / **`[COL:LEFT]`** / **`[COL:RIGHT]`** / **`[COL:FULL]`**.
  - For a “single-module” panel, just put **one** command (e.g. `[INV]`) in Custom Data.
  - If Custom Data is empty/whitespace, PB2 defaults the command list to **`[INV]`**.

Full detail: [`docs/architecture/lcd_panels_and_layout.md`](docs/architecture/lcd_panels_and_layout.md).

---

## Standalone vs. system mode

- **Standalone mode** — PB1 can run alone (automation + IGC broadcast). PB2 can run alone (local LCD discovery and rendering). Without PB1 telemetry, PB2 cannot fabricate grid-wide **inventory / ice / power / refinery** DTOs: expect **NO SIGNAL** / **WAITING FOR TELEMETRY…** on displays when no valid packets arrive, and **`(no data)`** in warning text paths when a module has nothing to show.
- **Dual-block system (intended)** — PB1 **orchestrates** scanning and telemetry; PB2 **displays** synchronized dashboards from verified IGC input.

---

## Connectivity & security

- **SharedKey** — PB1 **`[Network]`** and PB2 **`[Network]`** must use the **same** `SharedKey` (after trim). Telemetry uses a **`SenderEnvelope`**: inner payload is **UTF-8 Base64**; the **MAC** is a fixed-width **hex** value derived from **FNV-1a** over the logical fields plus the shared key. **Mismatch or parse failure → message ignored.**
- **Sender identity (PB1)** — Wire **`pbId`** follows **`[Prefix]-[Suffix]`**: up to **3** user-editable alphanumeric characters (from **`PBID`** / legacy **`SenderId`** in Custom Data, default prefix **`CMD`**) plus a **4-character hex** suffix taken from the block’s **entity id** (stable for that block). The script **rebinds** the suffix if hand-edited.

Details: [`docs/architecture/igc_contract.md`](docs/architecture/igc_contract.md), [`docs/architecture/network-layer.md`](docs/architecture/network-layer.md).

---

## PB1 vs PB2 responsibilities (non-negotiable)

Shared contracts live under [`GbearOS_Shared/`](GbearOS_Shared/) (DTOs, channel names, serializers, `SenderEnvelope`).

| Block | Responsibility |
|------:|----------------|
| **PB1 (Core / Orchestrator)** | Discovers blocks on the construct, runs refinery / ice / power logic, builds DTOs, wraps/broadcasts telemetry on IGC. **Must not** drive LCDs. |
| **PB2 (Display Manager)** | Receives IGC, verifies/parses telemetry, and renders LCDs on a ~10 Hz cadence. **Must not** run automation or scanning APIs. |

---

## Documentation

- **Configuration manual (Custom Data / INI keys)**: [`docs/configuration.md`](docs/configuration.md)
- **Architecture index (start here)**: [`docs/architecture/README.md`](docs/architecture/README.md)
- **System overview**: [`docs/architecture/system_overview.md`](docs/architecture/system_overview.md)
- **IGC contract (channels, DTO wire format, `SenderEnvelope`)**: [`docs/architecture/igc_contract.md`](docs/architecture/igc_contract.md)
- **Network ingress model (`SenderId`, replay, MAC)**: [`docs/architecture/network-layer.md`](docs/architecture/network-layer.md)

---

## Developer setup

For instructions on how to build GbearOS from source, see [`docs/setup.md`](docs/setup.md).
