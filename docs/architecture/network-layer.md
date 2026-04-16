> **AI Summary:** Logical network model—SenderId, node classes, and zero-trust ingress before IGC payload routing.

# Network layer — identified nodes and zero-trust ingress (NOC Phase 1, v1.3)

## Purpose

Define the **logical network layer** for GbearOS: how participants are **named**, **classified**, and **admitted** before higher-layer protocols (for example, IGC channel semantics in [igc_contract.md](./igc_contract.md)) apply. Provisioning **`SharedKey`** and **`SenderId`**: [configuration.md](../configuration.md). This document is **normative for design**; implementation may evolve while preserving the invariants below.

## Identified node model

An **identified node** is any GbearOS endpoint that may originate or terminate traffic on the in-game programmable-block network. It is characterized by:

| Attribute | Description |
|-----------|-------------|
| **Sender identity** | A stable, human-auditable **SenderId** (see below). |
| **Trust posture** | Whether the node is **authenticated** to the local security domain (cryptographic or pre-provisioned binding), not merely “reachable.” |
| **Capability surface** | What the node is allowed to do after admission (enforced by **ACLs** and policy above this layer). |

Nodes are **logical peers** at this layer. No assumption is made about physical grid topology, block type, or vessel class—the **NetworkManager** (or equivalent orchestration component) reasons only in terms of **identifiers and policy**, keeping the design **modular** and **decoupled from specific grid types**.

## SenderId format

The **SenderId** is a string that binds **role** to a compact unique label:

**`[Role]-[ShortGuid]`**

- **`Role`**: A constrained vocabulary naming the functional class of the node (for example, automation vs. display vs. relay). Roles drive **ACL** defaults and observability; they are not a substitute for cryptographic proof.
- **`ShortGuid`**: A shortened, high-**entropy** handle derived from a unique origin (for example, a grid or programmable block instance key) such that collision probability across the fleet remains negligible for the intended scale.

**Entropy note:** The ShortGuid portion should be drawn from a space large enough that casual spoofing and accidental collision are impractical; operational policy may require registration or out-of-band attestation for sensitive roles.

## Zero-trust ingress policy (data link boundary)

GbearOS adopts a **zero-trust** stance at the **ingress demarcation** modeled here as the **Data Link layer** boundary: the first point where a raw frame or message string becomes an **internal protocol data unit** for GbearOS processing.

**Policy (normative intent):**

1. **Unauthenticated or unverified frame** — Admission requires both:
   - **Replay protection:** **`Timestamp`** must **strictly increase** per **`SenderId`** relative to the receiver’s **last seen** value (see [sender-id-protocol.md](../design/sender-id-protocol.md)).
   - **Authentication:** **`MAC`** must validate against **`SharedKey`** and the claimed **`SenderId`**, **`Timestamp`**, and **`Payload`** (lightweight deterministic **MAC**, e.g. FNV-1a over the agreed concatenation—**not** `string.GetHashCode()`).

   Any frame that fails format checks, **timestamp** policy, or **MAC** verification is **dropped** at this boundary: no parsing of application payload for trust decisions beyond what the **MAC** covers, no fan-out to subscribers, and no state mutation based on that frame.

2. **Default deny** — Absence of a positive admission decision is equivalent to **deny**; there is no “trust the transport” shortcut.

3. **Separation of concerns** — **Ingress ACL** evaluates envelope fields first; only **admitted** **Payload** strings are passed to **IGC** DTO parsers or the **10 Hz** UI path.

**Rationale:** Dropping at the Data Link layer limits **attack surface** (**replay**, **spoofing**, malformed higher-layer parsing), contains **blast radius**, and keeps per-message work bounded—consistent with the **10 Hz** cadence and **minified script** size under the **100,000-character** limit.

### Stateless resilience (grid concealment / memory wipe)

The **v1.3** envelope is **stateless on the sender**: the programmable block does **not** rely on a **long-lived secret counter** that must survive RAM-only state. Each transmission carries a fresh **`DateTime.UtcNow.Ticks`** stamp and a **MAC** derived from **`SharedKey`** plus the wire fields. That design mitigates the **“Grid Concealment / Memory Wipe”** class of issues where **Torch Concealment**-style plugins or similar mechanisms hide or reset grid context: the sender can resume issuing valid frames after recompile or reload using only **config-held** **`SharedKey`** and **`SenderId`**, without resynchronizing a lost **sequence**. **Server pauses** and **PB recompiles** likewise do not require a separate **sequence** bootstrap—at the cost of the receiver retaining **last-seen timestamp per SenderId** (small, soft state) for **replay protection**.

## Relationship to higher layers

- **Network layer (this document):** Who may speak, under what identity, and whether the frame enters the system at all.
- **Transport / application (e.g. IGC DTO contract):** What the payload means once the frame is **admitted**; see [igc_contract.md](./igc_contract.md).

The **NetworkManager** should depend on **abstractions** (interfaces or delegates) for “resolve identity,” “evaluate ACL,” and “deliver admitted payload,” not on concrete grid or block implementations—preserving **testability** and **modularity** across vessel and station configurations.

## Implementation mapping (current tree)

| Topic | Location |
|-------|----------|
| Envelope wrap / parse | **`GbearOS_Shared/network/SenderEnvelope.cs`** — **`Wrap`**, **`TryParse`**; MAC = **8** hex digits (**`X8`**) from **32-bit FNV-1a**; replay = strictly increasing **`DateTime.UtcNow.Ticks`** per **`SenderId`** on PB2. |
| PB1 send path | **`GbearOS_PB1_Core/Program.cs`** — **`SendDto`** (requires non-empty **`SharedKey`**); **`EnableNetwork`** gates the five primary telemetry sends. |
| PB2 ingress | **`GbearOS_PB2_Display/igc_parser.cs`** — empty **`SharedKey`** ⇒ **`Route`** returns immediately (no **`SYS_STATUS`**, no DTOs); else **`SYS_STATUS`** raw, other tags via **`SenderEnvelope.TryParse`** then **`Serializer.Deserialize`**. |
| Config | **`[Network]`** in PB1 and PB2 Custom Data — see [configuration.md](../configuration.md). |

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [igc_contract.md](./igc_contract.md) | On-wire envelope usage, PB2 ingress |
| [sender-id-protocol.md](../design/sender-id-protocol.md) | Wire spec and MAC construction |
| [configuration.md](../configuration.md) | **`SharedKey`**, **`SenderId`**, **`EnableNetwork`** |

## Document control

| Version | Scope |
|---------|--------|
| **v1.2** | NOC Phase 1: Identified Node model, SenderId format, zero-trust ingress (prior envelope semantics). |
| **v1.3** | **Stateless** admission: **timestamp** progression + **MAC** validation; resilience notes (Concealment, pauses, recompiles); **10 Hz** / **100k** alignment. |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
