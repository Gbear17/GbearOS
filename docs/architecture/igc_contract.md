> **AI Summary:** Normative IGC channels, inner DTO wire format, and outer SenderEnvelope layering between PB1 and PB2.

# IGC contract

## Purpose

Define **IGC channel tags**, **message type identifiers**, and the **on-wire string format** used between PB1 and PB2. Structured telemetry uses two layers:

1. **Inner payload (DTO body):** produced and consumed by **`IGCSerializer`**, with script entry points **`Serializer.Serialize`** and generic **`Serializer.Deserialize`** (e.g. `Deserialize<InventorySummaryDTO>`). One string, split on **`;`**, **`p[0]`** = protocol version, **`p[1]`** onward = DTO fields. If `p[0]` is wrong, deserialization yields a **default/empty DTO** (no throw across the public API).
2. **Outer sender envelope (DTO channels):** PB1 wraps the inner string with **`SenderEnvelope.Wrap`** before broadcast; PB2 verifies with **`SenderEnvelope.TryParse`** then passes the decoded inner string to **`Serializer.Deserialize`**. Normative wire rules (**v1.4:** Base64 middle field, monotonic timestamps): [network-layer.md](./network-layer.md) and this document; **historical** narrative (non-contract): [`sender-id-protocol-noc-phase1-v14.md`](../history/sender-id-protocol-noc-phase1-v14.md). Code: **`GbearOS_Shared/network/SenderEnvelope.cs`**.

## Telemetry design (at a glance)

- **Typed DTO channels**: PB1 publishes structured telemetry on dedicated IGC tags (inventory, refinery, ice, power, dynamic inventory, warnings). Channel names are constants in **`GbearOS_Shared`** (see **`GbearOS_Shared/igc/channels.cs`**).
- **`SYS_STATUS`**: plain-text dashboard lines from any programmable block. PB2 maintains a status registry (stale entries expire) for `[STATUS]` panels. `SYS_STATUS` is **not** a DTO channel and is **not** wrapped in `SenderEnvelope` from PB1’s status broadcast path.
- **Cadence**: PB1 aligns with **`Update10`** for automation and telemetry. PB2 registers **`Update10`**; UI work is split/decimated to protect the render cadence (see [engine.md](./engine.md)).

## Channels

Logical channel tags (broadcast string names) include:

| Channel | Direction / use |
|---------|------------------|
| `SYS_STATUS` | Any programmable block → PB2: **plain text** for the status dashboard. **Not** DTO-encoded and **not** wrapped in **`SenderEnvelope`** in the current PB1 implementation. PB2 still processes it only if **`SharedKey`** is configured (non-empty)—see § *Ingress on PB2* below. Registry keyed by **IGC message source**; stale entries expire. Rendered on panels whose name contains `[STATUS]`. |
| `PB1ToPB2_InventorySummary` | PB1 → PB2: inner `InventorySummaryDTO` string inside **`SenderEnvelope`** |
| `PB1ToPB2_RefineryStatus` | PB1 → PB2: inner `RefineryStatusDTO` inside envelope |
| `PB1ToPB2_IceStatus` | PB1 → PB2: inner `IceStatusDTO` inside envelope |
| `PB1ToPB2_PowerStatus` | PB1 → PB2: inner `PowerStatusDTO` inside envelope |
| `PB1ToPB2_InventoryDynamic` | PB1 → PB2: inner `InventoryDynamicDTO` inside envelope |
| `PB1_WARNINGS` | PB1 → PB2: inner `WarningDTO` inside envelope |
| `PB2_ACK` | PB2 → PB1: optional acknowledgements (contract reserved; usage as implemented in scripts) |
| `PB2ToPB1` | PB2 → PB1: reverse tag (e.g. listener registration on PB1) |

Channel constants live in **`GbearOS_Shared`** as **`IGCChannels`** (see **`GbearOS_Shared/igc/channels.cs`**).

## Sender envelope (DTO channels)

For the **PB1 → PB2** rows above (all except **`SYS_STATUS`** as sent from PB1’s `SendDto` path):

- **On wire (v1.4):** `PBID|Timestamp|PayloadB64|MAC` (first field is the PB1 **`[Network]` `PBID`** string—the same logical **sender identity** described in [network-layer.md](./network-layer.md)).  
  - **`PayloadB64`** = **Base64** (UTF-8, no line breaks) of the **inner** DTO string (semicolon-separated, protocol field first). Inner bodies may contain **`|`** (e.g. pipe-separated array fields); Base64 avoids colliding with envelope delimiters.  
  - **`Timestamp`** = decimal string; **monotonic per PB1 process** (`SenderEnvelope` bumps if `UtcNow.Ticks` would repeat) so back-to-back `SendDto` calls in one tick still pass replay checks.  
  - **`MAC`** = **8** uppercase hex digits from **32-bit FNV-1a** over **`PBID + Timestamp +` (decoded inner string) `+ SharedKey`**—not over the Base64 text.  
- **Replay:** PB2 keeps **last accepted envelope ticks per `PBID`** (envelope field 0); incoming timestamp must be **strictly greater** than the last accepted for that sender **unless** that sender has had **no accepted envelopes** for **`SenderEnvelope.ReplaySilenceExpiryTicks`** (90 seconds of wall-clock silence, `DateTime.UtcNow.Ticks`), in which case PB2 clears the baseline so a restarted PB1 can resume. **`SenderEnvelope.EvictStaleReplayState`** prunes expired senders so the cache stays bounded. MAC and malformed-envelope checks are unchanged.

**PB1 transmit policy:** `SendDto` requires a **non-empty** `SharedKey` (orchestrator echoes **`NET BLOCKED: SharedKey missing.`** and does not send). **`EnableNetwork`** gates the **five** primary telemetry broadcasts; **`PB1_WARNINGS`** still uses `SendDto` and the same key requirement.

## Ingress on PB2

Before any branch in **`Route`**, if **`SharedKey`** is empty, the handler **returns** (no **`SYS_STATUS`** registry update, no DTO updates). With a configured key, **`SYS_STATUS`** is stored as raw text; other tags require successful **`SenderEnvelope.TryParse`**.

## Message type identifiers

`IGCSerializer.Deserialize(string messageType, string data)` dispatches on the **C# type name** string. **`IGCMessageTypes`** exposes the same strings as constants:

| Constant (example) | `messageType` value |
|--------------------|---------------------|
| `InventorySummary` | `InventorySummaryDTO` |
| `RefineryStatus` | `RefineryStatusDTO` |
| `PowerStatus` | `PowerStatusDTO` |
| `IceStatus` | `IceStatusDTO` |
| `Warning` | `WarningDTO` |
| `InventoryDynamic` | `InventoryDynamicDTO` |

`IGCMessageTypes` constants live in **`GbearOS_Shared`** (see **`GbearOS_Shared/igc/message_types.cs`**).

## Wire format (inner DTO payloads)

The following applies to the **Payload** string **inside** the envelope (after **`TryParse`**), not to the outer pipe-delimited frame.

1. **Record separator:** semicolon **`;`** between top-level fields.  
2. **Protocol field:** after `string[] p = data.Split(';')`, **`p[0]`** must equal **`"1"`** (current `ProtocolVersion` in code). Otherwise the deserializer returns a fresh default DTO.  
3. **Payload:** fields **`p[1]`**, **`p[2]`**, … map to DTO members in the fixed order below. Missing trailing segments are treated as absent (`TryParse` failures leave defaults).  
4. **String arrays** (embedded in a single `p[k]`): elements concatenated with **`|`**. Backslash escaping: **`\\`** → `\`, **`\|`** → `|` (see `AppendEscapedString` / `FillEscapedPipeSegments`).  
5. **Float and bool arrays:** split on **`|`** only (no escape processing). Bool scalars use **`1`** / **`0`** (and `ParseBool` accepts a limited `true` form).  
6. **No JSON** for DTO bodies. **No extra type prefix** inside the string beyond field `0` (the protocol version).

### PB1 string ingress (decision 2A)

Player- or game-derived text that populates DTO string fields is sanitized once at PB1 ingestion using **`FormattingUtils.SanitizeIngressWireText`** in **`GbearOS_Shared/utils/formatting_utils.cs`** (replaces `;`, `|`, `\`, CR, and LF with a space). Sanitization runs before serialization and **`SenderEnvelope` MAC** so PB2 verifies the same payload it deserializes. **`SYS_STATUS`** remains plain text and is **not** covered by this DTO/MAC path.

### `InventorySummaryDTO`

After `p[0] == "1"`, indices **1–27** are **floats** in order:

`ironOre`, `nickelOre`, `cobaltOre`, `siliconOre`, `magnesiumOre`, `silverOre`, `goldOre`, `platinumOre`, `uraniumOre`, `stoneOre`, `iceOre`, `ironIngot`, `nickelIngot`, `cobaltIngot`, `siliconIngot`, `magnesiumPowder`, `silverIngot`, `goldIngot`, `platinumIngot`, `uraniumIngot`, `componentsTotal`, `ammoTotal`, `toolsTotal`, `bottlesTotal`, `cargoUsed`, `cargoMax`, `cargoPercent`.

**Example (abbreviated):**  
`1;10;0;0;0;0;0;0;0;0;0;5;100;50;0;0;0;0;0;0;0;200;10;2;1;1;1000;5000;20`

### `RefineryStatusDTO`

| Index | Field |
|------:|--------|
| 1 | `refineryNames` — string array |
| 2 | `currentOre` — string array |
| 3 | `oreAmounts` — float array |
| 4 | `outputIngot` — string array |
| 5 | `outputAmounts` — float array |
| 6 | `isWorking` — bool array |
| 7 | `hasOre` — bool array |
| 8 | `priorityLine1` — raw string (no pipe array) |
| 9 | `priorityLine2` — raw string |

Because top-level fields are split on **`;`**, `priorityLine1` and `priorityLine2` **must not contain a semicolon** in the wire string (the implementation does not escape them).

**Example:**  
`1;RefA|RefB;Iron|;;100|0;Ingot|;;50|0;1|0;1|0;Prio line 1;Prio line 2`

### `IceStatusDTO`

Indices **1–8** floats (`totalIce`, `generatorIce`, `irrigationIce`, `cargoIce`, `pctTotal`, `pctGen`, `pctIrr`, `pctCargo`), **9–10** ints (`generatorCount`, `irrigationCount`), **11** bool (`lowIce` as `1`/`0`).

**Example:**  
`1;5000;2000;1000;2000;80;40;20;40;2;4;0`

### `PowerStatusDTO`

Indices **1–10** floats (battery/reactor/engine totals and flows per field order in code), **11–13** ints (`batteryCount`, `reactorCount`, `engineCount`), **14** bool `lowPower`.

### `InventoryDynamicDTO`

| Index | Field |
|------:|--------|
| 1 | `itemNames` — string array |
| 2 | `itemAmounts` — float array |
| 3 | `itemTypes` — string array |

**Example:**  
`1;ItemA|ItemB;12|3;Ore|Ingot`

### `WarningDTO`

| Index | Field |
|------:|--------|
| 1 | `lowIce` |
| 2 | `lowPower` |
| 3 | `cargoFull` |
| 4 | `noRefineries` |
| 5 | `refineryStalled` |
| 6 | `assemblerStalled` |
| 7 | `activeCode` (int) |
| 8 | `activeMessage` (raw string) |
| 9 | `isNominal` |

`activeMessage` (**index 8**) is a raw field: it **must not contain `;`**, or top-level splitting will corrupt the payload.

**Example:**  
`1;0;0;0;0;0;0;0;;1`

## Serialization rules (policy)

- PB1 must **not** hand-build inner DTO strings; use **`Serializer.Serialize`** (delegates to **`IGCSerializer.Serialize`**) so field order and escaping stay correct, then wrap with **`SenderEnvelope.Wrap`** for DTO channels.
- PB2 must verify the envelope when required, then deserialize inner data only through **`Serializer.Deserialize`** (generic) / **`IGCSerializer`** for the typed channels above.
- **`SYS_STATUS`** remains **free-form text** (not envelope-wrapped from PB1’s status broadcast path; still subject to PB2’s empty-key gate).

## Cadence (as implemented)

- PB1 drives its work on **`Update10`** and sends telemetry on that cadence.
- PB2 registers **`Update10`** and handles **`UpdateType.IGC`** when messages arrive; inside **`Update10`** it also drains/processes IGC and runs the display loop (with **decimated** heavy work—see [engine.md](./engine.md)).

---

## Related documents

| Document | Topic |
|----------|--------|
| [README.md](./README.md) | Architecture doc index |
| [artifact_verification.md](./artifact_verification.md) | Canonical shared inventory + minified artifact spot-check (Phase 1) |
| [network-layer.md](./network-layer.md) | **Sender identity (`PBID` on wire)**, zero-trust ingress, v1.3 envelope semantics |
| [configuration.md](../configuration.md) | **`SharedKey`**, **`EnableNetwork`**, **`PBID`** |
| [`sender-id-protocol-noc-phase1-v14.md`](../history/sender-id-protocol-noc-phase1-v14.md) | **Archive:** historical envelope/MAC narrative (v1.4); current rules in this file + [`network-layer.md`](./network-layer.md) |
| [pb1_pb2_rules.md](./pb1_pb2_rules.md) | Serialization responsibilities |
| [update_frequencies.md](./update_frequencies.md) | Tick cadence |

---

Copyright (c) 2026 Garrett Wyrick. Documentation is part of GbearOS, licensed under GPL-3.0 — see repository `LICENSE`.
