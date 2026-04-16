> **AI_SYSTEM_INDEX**  
> Normative hub for `docs/architecture/`: PB1 vs PB2 roles, IGC/DTO contracts, PB2 LCD engine, network ingress, configuration semantics. Use the **Start here** table for canonical filenames; build/deploy remain in the root `README.md` (repo-specific), and future planning lives in [`docs/ROADMAP.md`](../ROADMAP.md).

> **AI Summary:** Index of normative architecture documents and navigation for PB1 versus PB2 roles, IGC, LCD engine, and configuration links.

# Architecture documentation

Normative **“what”** and **“why”** for GbearOS: in-game behavior, IGC contracts, UI engine behavior, and coding rules. These files are **environment-agnostic**: they apply whether you work from a **modular** source tree or a **single merged script** per programmable block.

- **Repository layout, build, deploy, and Git promotion** live in the root **`README.md`**—not here.
- **Roadmap / future architecture** lives in [`docs/ROADMAP.md`](../ROADMAP.md).
- Prefer **logical names** (PB1, PB2, DTO types, `Serializer` / `IGCSerializer`, `SenderEnvelope`, LCD renderer) over physical file paths.

## Start here

| Document | Purpose |
|----------|---------|
| [configuration.md](../configuration.md) | **Configuration manual:** PB1/PB2 Custom Data, every INI section and key, validation clamps, strict template enforcement, network security |
| [system_overview.md](./system_overview.md) | Roles of PB1 vs PB2, IGC/DTO summary, dual presentation of sources |
| [lcd_panels_and_layout.md](./lcd_panels_and_layout.md) | LCD template tags, `[GbearOS]` virtual layout, configuration notes |
| [engine.md](./engine.md) | PB2 split loop (`Update10` vs decimated heavy work), virtual viewport, Y culling |
| [pagination.md](./pagination.md) | Virtual scroll timing, easing, page indicator |
| [persistence.md](./persistence.md) | Panel list recovery across grid rescans |
| [update_frequencies.md](./update_frequencies.md) | Script cadence and performance constraints |
| [igc_contract.md](./igc_contract.md) | IGC channels, **`SenderEnvelope`** outer frame, inner DTO wire format (`Serializer` / **`IGCSerializer`**) |
| [contract_checklist.md](./contract_checklist.md) | Developer gate: shared vs PB2-only deserialize placement, `.csproj` include rules, PB1 entrypoint bans |
| [network-layer.md](./network-layer.md) | Identified nodes, SenderId format, zero-trust Data Link ingress |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | Mandatory boundaries between automation and display |
| [naming_conventions.md](./naming_conventions.md) | Naming patterns for types, channels, and identifiers |
| [user_config_system.md](./user_config_system.md) | Custom Data **behavior** (PB1 full INI, PB2 `[Network]` only, strict rewrite rules) |
| [modded_block_support.md](./modded_block_support.md) | Modded block handling notes |

## Design explorations

Non-normative notes and UX experiments may live under [../design/](../design/).

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
