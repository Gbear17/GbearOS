> **AI_SYSTEM_INDEX**  
> Non-normative hub for `docs/design/`: UX experiments and notes only. **Contracts and behavior:** [`docs/architecture/`](../architecture/). **INI keys:** [`docs/configuration.md`](../configuration.md). **Repo tooling:** root [`README.md`](../../README.md). **Future plans:** [`docs/ROADMAP.md`](../ROADMAP.md).

> **AI Summary:** Scope of the design folder—non-normative experiments and pointers to formal architecture under docs/architecture.

# Design notes (`docs/design/`)

This folder holds **non-normative** material: UX ideas, mock-up notes, and experiments that are **not** part of the formal architecture under [`docs/architecture/`](../architecture/). **Normative** IGC and envelope behavior: [`igc_contract.md`](../architecture/igc_contract.md), [`network-layer.md`](../architecture/network-layer.md). **INI / Custom Data keys:** [`configuration.md`](../configuration.md).

The same `docs/` tree may be checked out from either:

- a **modular** development repository, or  
- a **public** repository that ships merged scripts,

so design files here should avoid duplicating **build**, **deploy**, or **Git** process (see the root **`README.md`** in your clone). Roadmap and future-feature planning live in [`docs/ROADMAP.md`](../ROADMAP.md), not in `docs/architecture/`.

## Files in this folder

| Document | Purpose |
|----------|---------|
| [sender-id-protocol.md](./sender-id-protocol.md) | SenderId + `SenderEnvelope` wire spec and MAC notes (design exploration; see `docs/architecture/` for normative contracts) |

## Related locations

| Location | Role |
|----------|------|
| [`docs/architecture/`](../architecture/) | Normative in-game behavior and contracts |
| [`docs/configuration.md`](../configuration.md) | INI / Custom Data key reference |
| [Root `README.md`](../../README.md) | Build, deploy, modular layout |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
