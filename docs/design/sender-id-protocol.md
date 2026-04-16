> **AI Summary:** Design-level specification of the v1.4 sender envelope—wire layout, MAC, timestamps, and performance constraints distinct from inner DTO encoding.

# Status
>
> **Status:** Implemented (design history; see normative docs below)
> **Normative behavior:** [`docs/architecture/network-layer.md`](../architecture/network-layer.md), [`docs/architecture/igc_contract.md`](../architecture/igc_contract.md)

# Sender ID protocol — stateless MAC envelope and performance (NOC Phase 1, v1.4)

## Purpose

Specify the **GbearOS sender envelope** wrapped around higher-layer payloads—distinct from DTO field encoding in [igc_contract.md](../architecture/igc_contract.md). This envelope provides **identity**, **replay protection** via **monotonic timestamps**, and **authentication** via a **lightweight deterministic MAC** suitable for in-game programmable blocks.

**Design constraint:** Parsing and verification must remain compatible with a **10 Hz** tick budget and **minified script size** under the **100,000-character** deployment limit.

## Scope and modularity

The protocol is **transport-agnostic**: it defines a **string on the wire** after the game delivers a message body. **NetworkManager** (or equivalent) validates the envelope and forwards the **Payload** substring to channel routers **without** depending on specific grid types, block lists, or vessel metadata—only on **SenderId**, **Timestamp**, **MAC**, and policy.

**Stateless sender:** The originator does **not** maintain a rolling counter or shared sequence state across restarts; each frame is self-describing (**Timestamp** + **MAC**). The receiver holds only **minimal soft state**: the **last accepted timestamp per SenderId** for **replay protection**—a small, bounded table—not a synchronized secret progression.

## On-wire structure — stateless MAC protocol

Frames use a **pipe-delimited** top-level layout (**four** fields):

```text
SenderId|Timestamp|PayloadB64|MAC
```

| Field | Semantics |
|-------|-----------|
| **`SenderId`** | `[Role]-[ShortGuid]` as defined in [network-layer.md](../architecture/network-layer.md). |
| **`Timestamp`** | **UTC** logical time in **`DateTime.UtcNow.Ticks`** (Int64 string form), **monotonic per sender process**: if multiple frames are built in the same system tick, implementations **bump** the timestamp so each frame is strictly greater than the previous (replay-safe burst). |
| **`PayloadB64`** | **Standard Base64** (no line breaks) of the application string encoded as **UTF-8**. Empty application payload → empty string (not `"AA=="`). This keeps **`|`** and other characters in IGC DTO bodies from colliding with envelope pipes. |
| **`MAC`** | **FNV-1a** digest (see below) over the **logical** (decoded) **`Payload`** concatenated with **`SenderId`**, **`Timestamp`**, and **`SharedKey`**—not over the Base64 form. |

**Parsing:** Bounded **`Split('|', 4)`** yields four segments; decode **`PayloadB64`** with **`Convert.FromBase64String`** (reject malformed Base64). Verify **MAC** and **Timestamp** using the **decoded** payload string. If arity is wrong, Base64 is invalid, or checks fail, **drop** at ingress (see [network-layer.md](../architecture/network-layer.md)).

## Timestamp logic (replay protection)

- **Sender:** Set **`Timestamp`** to **`DateTime.UtcNow.Ticks`** when building the frame (or immediately before broadcast within the same **Update10** turn).
- **Receiver:** For each **`SenderId`**, accept the frame only if **`IncomingTimestamp` > `LastSeenTimestamp[SenderId]`** (strictly greater). On accept, update **`LastSeenTimestamp[SenderId]`** to **`IncomingTimestamp`**.
- **Duplicates and rewind:** Equal or older timestamps are **replays** or clock artifacts—**drop** at the Data Link boundary (**replay protection** / **ACL** deny).
- **Clock skew:** Optional policy (e.g. small future skew window) is implementation-defined; keep checks **integer compares** only to stay within the **10 Hz** path.

This model avoids **shared sequence state** on the sender that would be lost after **programmable block recompiles**, **world reloads**, or hostile **grid concealment / memory wipe** scenarios—see [network-layer.md](../architecture/network-layer.md).

## MAC logic (authentication / integrity)

**Goal:** **Spoofing** resistance: an adversary without **`SharedKey`** cannot forge a valid **`MAC`** for an arbitrary **`Payload`**.

**Construction:** Compute a **lightweight deterministic hash** (e.g. **FNV-1a** over UTF-16 code units) over the **concatenation** of:

- **`SenderId`**
- **`Timestamp`** (wire string)
- **`Payload`**
- **`SharedKey`**

**GbearOS implementation (`SenderEnvelope`):** **`Wrap`** sets **`MAC`** from **`Fnv1aHash(senderId + timestamp + payload + sharedKey)`** on the **decoded** application string, then places **`Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))`** in **`PayloadB64`**. **`TryParse`** decodes Base64 → payload, then **`Fnv1aFold`** over **`senderId`**, **`timestamp`**, **`payload`**, **`sharedKey`**. On-wire **`MAC`** is **8** uppercase hexadecimal digits.

Order and separators must be **fixed** and documented in code so sender and receiver agree. **`SharedKey`** is pre-provisioned (Custom Data, etc.); key agreement is out of scope for this wire spec.

**Critical — do not use `string.GetHashCode()`:** The .NET **`string.GetHashCode()`** implementation is **not** stable across processes or runtime versions and may inject **per-process randomization**. It is **unsuitable** for wire **MAC** verification and **must not** be used for this protocol.

**Security note:** FNV-1a is a **fast integrity check**, not a modern **cryptographic MAC**. It raises the bar for casual forgery within the **100k** script budget; deployments needing stronger **authentication** should swap in a vetted keyed primitive if the environment allows.

## Performance profile

To honor **10 Hz** scripts and the **100k** character ceiling:

| Technique | Rationale |
|-----------|-----------|
| **Int64 timestamp compare + dictionary lookup** | **O(1)** per sender after parse; no modular state machine on the sender. |
| **Bounded `Split('|', 4)` + Base64 middle field** | Fixed envelope shape; DTO bodies may contain **`|`** without breaking framing. |
| **`Convert` / `Encoding.UTF8`** | Small **BCL** surface; acceptable on **10 Hz** paths for typical DTO sizes. |
| **Compact FNV-1a loop** | Small, branch-light **C#** suitable for **Update10**; no heavy crypto APIs. |
| **No nested serialization** | Envelope stays **flat**; **Payload** passes through to **IGC** / DTO parsers unchanged after verification. |
| **Fail-fast ingress** | Failed **MAC** or timestamp checks exit before **`;`** split or UI work—preserving **tick** budget. |

**Character budget:** Keep **`SenderId`** compact, **`Timestamp`** as decimal digits, **`MAC`** as a short fixed-width hex or base64-like encoding per implementation. Total envelope overhead should remain a small fraction of the merged script so merged outputs stay under **100,000** characters.

## Security notes

- **Confidentiality:** This layer does **not** encrypt **Payload**; it provides **authentication** and **replay protection** at ingress.
- **ACL mapping:** After successful **MAC** and timestamp checks, map **SenderId** → **role-based ACL** before delivering **Payload** to subscribers.

## Related documents (normative implementation)

| Document | Topic |
|----------|--------|
| [network-layer.md](../architecture/network-layer.md) | Zero-trust ingress, identified nodes |
| [igc_contract.md](../architecture/igc_contract.md) | How envelopes wrap IGC DTO traffic |
| [configuration.md](../configuration.md) | **`SharedKey`** provisioning |
| [README.md](./README.md) | Design folder scope vs `docs/architecture/` |

## Document control

| Version | Scope |
|---------|--------|
| **v1.2** | NOC Phase 1 (superseded): Three-field envelope with rolling **Sequence**. |
| **v1.3** | NOC Phase 1 (superseded wire): four pipe fields with **raw** payload (broken when payload contains **`|`**). |
| **v1.4** | **PayloadB64** (UTF-8 Base64) + **monotonic** emit timestamps; **MAC** over decoded payload; **100k** / **10 Hz** constraints. |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
