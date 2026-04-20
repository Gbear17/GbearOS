# GbearOS configuration manual

**Copyright (c) 2026 Garrett Wyrick.** This document describes the **INI-style Custom Data** consumed by GbearOS scripts as implemented in the source tree (`Config` / `ConfigParser` on PB1; network bootstrap on PB2). For persistence and boundary rules (strict rewrite, what PB2 may store), see [user_config_system.md](architecture/user_config_system.md). PB1 is authoritative for full automation config; PB2 reads **only** the `[Network]` keys under [PB2 Display block](#pb2-display-block-pb2) below.

---

## Getting started

### Where to edit

| Block | Custom Data | Scope |
|-------|-------------|--------|
| **PB1** (GbearOS Core / orchestrator) | Full configuration | All sections in this manual except the PB2-only note |
| **PB2** (GbearOS Display) | Minimal INI | `[Network]` only (`EnableNetwork`, `SharedKey`) |

In Space Engineers, open the programmable block **Control Panel** → **Edit** → use the **Custom Data** field (or equivalent editor entry for the block).

### Strict template enforcement (PB1)

After **every** load, validation, or persist cycle that writes PB1 Custom Data, the script rebuilds the file from scratch:

1. **`WriteConfigToIni` clears the entire in-memory INI** (`_ini.Clear()`), then writes **only** the keys and sections the orchestrator defines.
2. **Custom Data is replaced** with that serialized INI string. Anything not re-emitted—extra sections, unknown keys, handwritten notes, or duplicate keys—is **dropped** on save.
3. **Comments** in Custom Data are **not** preserved as freeform user notes. Only **script-authored** `SetComment` strings on specific keys are written back (short cheat-sheet lines plus the longer `[Network] EnableNetwork` pointer to this file).
4. **Parse failures:** If Custom Data cannot be parsed as INI, the parser clears the in-memory INI and proceeds with defaults, then writes a fresh template.

**Operational implication:** Treat PB1 Custom Data as a **generated contract**, not a scratchpad. Keep operational notes in blueprints, external docs, or grid text panels.

### PB2 network template

PB2 **initializes once** (constructor): it parses Custom Data, ensures `[Network]` keys exist, writes the template back to Custom Data, and caches `SharedKey` / `EnableNetwork`. It does **not** perform the same full wipe as PB1’s `ConfigParser`; it only maintains the Network keys it touches.

---

## Section breakdown (PB1)

| Section | Purpose |
|---------|---------|
| `[IngotTargets]` | Per–subtype ingot stock targets (vanilla defaults merged when empty; modded ores can be appended at runtime). |
| `[IceTargets]` | Ice mass targets for generators, irrigators, cargo reserve, and distribution floor. |
| `[ReactorTargets]` | Uranium amount targets for large/small reactors. |
| `[BatteryThresholds]` | Fractional charge bands for reactors/engines/batteries; solar floor; power automation toggle. |
| `[RefinerySettings]` | Refinery balancing toggle and priority hysteresis. |
| `[BlockTags]` | Substrings used to tag irrigation supply and manually excluded blocks. |
| `[DisplayFilters]` | Booleans controlling which inventory categories feed PB1-side display-related logic / DTO content. |
| `[Debug]` | Verbose logging toggle. |
| `[Network]` | IGC envelope identity, shared secret, and orchestrator transmit enable. |

---

## Key reference (PB1)

### `[IngotTargets]`

Keys are **dynamic**: any name becomes an ingot target key (case-insensitive). Values are **non-negative doubles** (kg-style amounts as used by refinery logic).

| Key pattern | Type | Default when absent |
|-------------|------|---------------------|
| *(any subtype name)* | `double` (≥ 0) | If the **entire section is empty** after read, the script seeds **vanilla defaults** (see table below). New modded ores discovered at runtime may be added with default **500** per ore. |

**Vanilla seed defaults** (used only when no `[IngotTargets]` keys were loaded):

| Key | Default |
|-----|---------|
| Iron | 125000 |
| Nickel | 25800 |
| Silicon | 17500 |
| Cobalt | 14800 |
| Silver | 6100 |
| Gold | 9000 |
| Magnesium | 15000 |
| Platinum | 4500 |
| Uranium | 2600 |
| Gravel | 22500 |

**Read path:** Negative values read from INI are clamped to **0** immediately. `ValidateAndCorrectValues` clamps any negative dictionary values to **0**.

---

### `[IceTargets]`

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `GeneratorLargeTargetIce` | `double` (≥ 0) | 50000 | Target ice mass for **large** oxygen generators. |
| `GeneratorSmallTargetIce` | `double` (≥ 0) | 10000 | Target ice mass for **small** oxygen generators. |
| `IrrigationLargeTargetIce` | `double` (≥ 0) | 15000 | Target ice for **large** irrigation-related blocks (farms). |
| `IrrigationSmallTargetIce` | `double` (≥ 0) | 5000 | Target ice for **small** irrigation-related blocks. |
| `CargoReserveIce` | `double` (≥ 0) | 200000 | Base-wide ice below this level contributes to **LOW ICE** warning semantics. |
| `MinimumCargoIce` | `double` (≥ 0) | 50000 | Ice in cargo **above** this threshold is eligible for distribution toward generators and irrigators. |

---

### `[ReactorTargets]`

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `ReactorLargeUraniumTarget` | `double` (≥ 0) | 200 | Target uranium mass for **large** reactors. |
| `ReactorSmallUraniumTarget` | `double` (≥ 0) | 50 | Target uranium mass for **small** reactors. |

---

### `[BatteryThresholds]`

All **Lower** / **Upper** / **Battery\*** fraction keys use a **0.0–1.0** logical range (fraction of charge or normalized band). `SolarMinimumOutput` is a **non-negative** power threshold (MW, as interpreted by power automation).

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `ReactorLargeLower` | `double` [0, 1] | 0.25 | Lower charge fraction bound for **large reactor** automation band. |
| `ReactorLargeUpper` | `double` [0, 1] | 0.80 | Upper charge fraction bound for **large reactor** automation band. |
| `ReactorSmallLower` | `double` [0, 1] | 0.20 | Lower bound — **small reactor**. |
| `ReactorSmallUpper` | `double` [0, 1] | 0.70 | Upper bound — **small reactor**. |
| `EngineLargeLower` | `double` [0, 1] | 0.40 | Lower bound — **large** hydrogen engine. |
| `EngineLargeUpper` | `double` [0, 1] | 0.90 | Upper bound — **large** hydrogen engine. |
| `EngineSmallLower` | `double` [0, 1] | 0.35 | Lower bound — **small** hydrogen engine. |
| `EngineSmallUpper` | `double` [0, 1] | 0.85 | Upper bound — **small** hydrogen engine. |
| `BatteryChargeTarget` | `double` [0, 1] | 0.25 | Grid charge below this fraction drives **recharge** behavior in automation. |
| `BatteryDischargeTarget` | `double` [0, 1] | 0.80 | Charge above this fraction allows returning toward **auto** discharge behavior. |
| `EnablePowerAutomation` | `bool` | false | Enables solar-driven reactor/engine toggling and related logic. |
| `SolarMinimumOutput` | `double` (≥ 0) | 0.05 | If solar output falls below this (MW), backup power path can engage when automation is enabled. |

---

### `[RefinerySettings]`

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `EnableRefineryBalancing` | `bool` | true | When true, script manages refinery queues / balancing; when false, vanilla refinery behavior is left dominant. |
| `RefineryHysteresis` | `double` (≥ 0) | 0.05 | Hysteresis band for switching top-priority ore (reduces thrashing near targets). |

---

### `[BlockTags]`

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `IrrigationTag` | `string` | `[Irrigator]` | Substring in block **Custom Name** marking ice supply for O₂/H₂ farm irrigation path. Leading/trailing whitespace trimmed. |
| `ManualTag` | `string` | `[Manual]` | Substring marking blocks the orchestrator **skips**. Trimmed. |

---

### `[DisplayFilters]`

These booleans control which inventory categories are included in PB1-driven summary / dynamic inventory behavior consumed downstream (e.g. PB2 display).

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `ShowOres` | `bool` | true | Include ores in relevant displays. |
| `ShowIngots` | `bool` | true | Include ingots. |
| `ShowComponents` | `bool` | true | Include components. |
| `ShowAmmo` | `bool` | true | Include ammo. |
| `ShowDynamicItems` | `bool` | true | Include dynamic / other item buckets as implemented. |

---

### `[Debug]`

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `EnableDebug` | `bool` | false | Enables extra echo / debug buffer output on the orchestrator when true. |

---

### `[Network]` (PB1)

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `EnableNetwork` | `bool` | true | When **false**, PB1 **does not** broadcast the five primary telemetry DTOs to PB2 (inventory, refinery, ice, power, dynamic). |
| `PBID` | `string` | `CMD-DEFAULT` | Sender identity on the **`SenderEnvelope`** IGC wrapper (`PREFIX-XXXX`: up to **3** alphanumeric prefix characters, then **`-`**, then a **4**-character **hex** suffix from this block’s **entity id**). Trimmed; if empty after trim, the script uses **`CMD`** plus the bound suffix. The script **rewrites** Custom Data with **`PBID`** as the canonical key. |
| `SharedKey` | `string` | `""` | Shared secret for envelope MAC. Must **match PB2** `SharedKey`. Trimmed. |

---

## PB2 Display block (PB2)

PB2 Custom Data uses section **`[Network]`** only:

| Key | Type | Default | Function |
|-----|------|---------|----------|
| `EnableNetwork` | `bool` | true | When **false**, PB2 echoes **`NETWORK OFFLINE`** on full display ticks; LCD path shows **no signal** from PB1 when no data arrives. |
| `PBID` | `string` | `DIS` + bound suffix | This block’s identity string (default prefix **`DIS`** when the value is empty; **4**-character hex suffix from this block’s **entity id**). Used for PB2-local features (for example **`[STATUS:…]`** filters). Not the PB1 envelope sender. |
| `SharedKey` | `string` | `""` | Must **byte-for-byte match** PB1 after trim. If empty, **all** routed IGC messages (including `SYS_STATUS` handling inside `Route`) are **ignored** at the parser gate. |

On PB2 **program compile / world load**, `LoadNetworkSharedKeyFromCustomData` rewrites `Custom Data` to ensure these keys and comments exist, and **removes** any obsolete **`SenderId`** key under **`[Network]`** if present.

---

## Validation rules (mathematical clamping)

Rules below are applied in **`ReadConfig`** and again in **`ValidateAndCorrectValues`** where applicable (PB1). PB2 applies **`[Network]`** trimming and **`PBID`** composition only inside `LoadNetworkSharedKeyFromCustomData`.

| Category | Rule | Effect |
|----------|------|--------|
| **Non-negative scalars** (`ClampNn`) | \(x < 0 \rightarrow 0\) | Ice targets, uranium targets, solar minimum, refinery hysteresis, all `[IngotTargets]` values. |
| **Unit interval** (`Clamp01`) | \(x < 0 \rightarrow 0\); \(x > 1 \rightarrow 1\) | All reactor/engine/battery fraction keys in `[BatteryThresholds]`. |
| **Strings** | `.Trim()` | `IrrigationTag`, `ManualTag`, `PBID`, `SharedKey`. |
| **`PBID` (PB1)** | After read | Composed via **`ComposeBoundPbid`**: prefix from **`[Network]` `PBID`** (or **`CMD`** when empty), suffix from entity id; hand-edited suffix is **rebound** to the block if it does not match. |
| **Booleans** | `ToBoolean(default)` | Uses the defaults in the tables above when keys are missing or non-parseable. |

The script **does not** reject invalid files; it **corrects** values and **rewrites** Custom Data to the canonical template.

---

## Security protocol: network enable and shared key

### Shared secret requirement

| Component | Behavior when `SharedKey` is null or empty (after trim) |
|-----------|-----------------------------------------------------------|
| **PB1 `SendDto`** | **Hard block:** no IGC send; terminal echoes **`NET BLOCKED: SharedKey missing.`** Applies to **every** DTO send that uses `SendDto`, including **warnings**. |
| **PB2 `Route`** | **Hard block:** immediate return; no envelope parse, no DTO updates, **no** `SYS_STATUS` registration for that message path. |

There is **no** fallback to unsigned payloads through this code path: the orchestrator treats a missing key as a **complete transmit block** for wrapped DTOs.

### EnableNetwork (PB1)

| `EnableNetwork` | Effect on PB1 |
|-----------------|---------------|
| **true** | Normal operation: the five telemetry `SendDto` calls run on the scheduled tick (still subject to **`SharedKey`** non-empty for any `SendDto`). |
| **false** | Those five telemetry broadcasts are **skipped**. Warning DTOs still call `SendDto` and therefore still require a **non-empty** `SharedKey` unless you also supply a key or accept the hard block behavior. |

**Recommendation:** For **offline / lab** PB1, set **`EnableNetwork=false`** and set a **non-empty** `SharedKey` if you still want warnings to transmit without re-enabling the main telemetry burst—or accept that `SendDto` will no-op with the block message when the key is empty.

### EnableNetwork (PB2)

| `EnableNetwork` | Effect on PB2 |
|-----------------|---------------|
| **true** | Normal listener behavior (subject to non-empty `SharedKey` for actual routing). |
| **false** | Terminal indicates **NETWORK OFFLINE** on decimated ticks; display shows **no PB1 signal** when no data is received. |

### Pairing checklist

1. Set **identical** `SharedKey` on **PB1** and **PB2** `[Network]`.
2. Set **`PBID`** on PB1 as needed (prefix only is user-facing; suffix is bound to the block); align with any filtering documented in [network-layer.md](architecture/network-layer.md).
3. Use **`EnableNetwork=false`** on either side only when you intentionally want **standalone** behavior and understand **PB1 `SendDto`** still enforces the key unless you provide one.

---

## Related documentation

| Document | Topic |
|----------|--------|
| [architecture/README.md](architecture/README.md) | Index of all normative architecture docs |
| [architecture/user_config_system.md](architecture/user_config_system.md) | Custom Data **behavior** (PB1 strict template, PB2 `[Network]` only). **This** file is the **key catalog**. |
| [architecture/igc_contract.md](architecture/igc_contract.md) | IGC channels, **`SenderEnvelope`**, inner DTO layout, **`Serializer`**. |
| [architecture/network-layer.md](architecture/network-layer.md) | Sender identity and ingress policy. |
| [architecture/pb1_pb2_rules.md](architecture/pb1_pb2_rules.md) | PB1 vs PB2 responsibilities. |
