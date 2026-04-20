---
name: gbearos-doc-sync
description: >-
  🚂 [ORCHESTRATOR] Documentation synchronizer for GbearOS. Filesystem-only (NEVER runs
  git). Single sequential flow: Discovery (§0) → Scan (§1) → Gap Analysis (§2) →
  Updates (§3–§4). Enforces docs↔source reconciliation (no fictional implemented
  behavior, no missing coverage for player-facing contracts), curates docs/design/,
  docs/todo/, docs/history/ (archive to history; remove completed source files), and
  normalizes docs/ROADMAP.md last.
  Does NOT build or deploy scripts; release build/budget checks belong to git sync / git-sync (gbearos-git-sync skill).
---

# Public Release documentation sync (`doc-sync` / `doc sync`)

When the user asks for **documentation sync** (e.g. `doc-sync` / `doc sync`, “sync docs”, “align docs with code”), run a pass on the **currently open workspace** (or the roots the user specifies). This orchestrator is a **single-purpose Public Release synchronizer** and operates **exclusively on the local filesystem**.

## CRITICAL CONSTRAINT (must obey)

- **Never run git commands.** No branch detection, no commit, no push, no status/log, no checkout.
- Default to **documentation-only** edits unless the user explicitly requests source changes.
- Treat **source of truth** as the **modular source code** in the repo (e.g. `*.cs`, manifests). **Do not** treat compiled/minified artifacts under `dist/` as behavioral truth. Do not update docs based on `dist/`.

---

## 0. Discovery (do this first)

Infer from what exists on disk—do not assume a single framework.

| Question | How to resolve |
|----------|----------------|
| Where is the entry doc? | Usually root `README.md`; sometimes `README.rst` or a scoped `packages/foo/README.md` in monorepos. |
| Where are design/architecture notes? | Common: `docs/`, `doc/`, `wiki/`, `architecture/`; often `docs/architecture/` and `docs/design/`. |
| Where is primary source? | Common: `src/`, `lib/`, `app/`, `packages/*`, `scripts/`, language-specific roots—use what exists. |
| Project markers? | README, `CONTRIBUTING.md`, or `AGENTS.md` may define blockquote tags, merge-order lists, or source banner patterns—prefer those over guessing. |

**GbearOS-style detection:** If the workspace contains both `*PB1_Core/*.csproj` and `*PB2_Display/*.csproj` (MDK2 project manifests), treat §1.3, §2.1, and §2.2 as **required** checks for this run.

### 0.1 Documentation tiers (required classification)

Treat the following as the complete documentation universe for this run:

- **Normative docs (must match code)**:
  - Root `README.md`
  - `docs/**` **excluding** `docs/todo/**`, `docs/design/**`, and `docs/history/**`
- **Curated, non-normative docs (must be maintained, but not contract)**:
  - `docs/design/**`
- **Planning docs (excluded from “must match code” guarantees)**:
  - `docs/todo/**`
- **Historical / archival docs (completed plans; not contract)**:
  - `docs/history/**`

Normative docs must not claim implemented behavior that does not exist in source. Planning and design docs may describe future work, but must remain clearly framed as future work. Historical docs must not read as the current player-facing contract; they record **what was decided or attempted** and point to normative docs for **current behavior**.

---

## 1. Scan (filesystem + MDK2 checks)

### 1.0 Repository root confirmation

Resolve the **repository root** for this pass (the folder containing `README.md` and `docs/`).

If you **cannot** confirm the intended root, stop and ask (exact text):

`I am unable to confirm the repository root. Please provide the folder name or path to continue.`

### 1.1 Multi-tier blueprint content (after root is known)

Before editing documentation, gather:

1. **Root overview** — Opening sections of `README.md` (capture any normative blockquote tags the project uses).
2. **Doc inventory** — Enumerate all Markdown files under `docs/` and classify them into tiers (§0.1). Identify doc indexes (`README.md`, `SUMMARY.md`, `index.md`) and summary patterns.
3. **Code inventory** — Enumerate primary source trees dynamically:
   - Prefer manifests (`*.csproj`, `*.sln(x)`); then scan for source files (`**/*.cs`).
   - Exclude build outputs and release artifacts from truth scans: `dist/**`, `**/bin/**`, `**/obj/**`, `**/MDK/**`.
4. **Code anchors** — Search for file banners that tie modules to docs (pattern: `// [GbearOS Component]` where present).

### 1.3 MDK2 manifest vs disk (required when `.csproj` manifests exist)

Use the MDK2 project manifests as the source-of-truth inclusion list:

1. Treat `*PB1_Core/*.csproj` as the PB1 manifest and `*PB2_Display/*.csproj` as the PB2 manifest.
2. Parse XML inclusions/exclusions (`Compile Include`, `Compile Remove`, globs, etc.) and normalize to repo-relative paths.
3. Inventory on-disk `*.cs` files under each PB’s subtree.
4. Report:
   - **Orphaned Files**: present on disk but not included by the `.csproj`
   - **Missing Files**: included by the `.csproj` but missing on disk

---

## 2. Gap analysis

- Compare **documented** behavior (root README + **normative** `docs/` per §0.1) to **actual** source.
- Flag stale claims, missing docs, broken links, naming drift, and “docs that describe removed features as implemented”.
- Do not invent behavior—only reconcile what code and existing docs support. **Exception (README only):** §3.1 grants **grounded synthesis** (features, installation, LCD tags) when traced to DTOs, `ScreenRenderer`/panel logic, and configuration keys—still no fictional keys, channels, or UI that the codebase does not implement.

### 2.0 Coverage requirements (bidirectional)

`gbearos-doc-sync` must enforce both directions:

- **Docs → Code (no fiction):** Normative docs must not describe behavior as implemented unless there is clear evidence in source (types, constants, enums, config keys, parsing/dispatch logic, or other authoritative code paths).
- **Code → Docs (no silent features):** Every **player-facing** behavior, command grammar, tag token, IGC channel/identifier, and Custom Data key that exists in source must be documented **somewhere** in normative docs. If coverage is missing, update existing docs or create new docs under `docs/` as needed.

Do not attempt to document every internal helper method; focus on externally visible behavior and contracts.

### 2.1 Header validation (required when `// [GbearOS Component]` exists)

For each `*.cs` under `GbearOS_Shared/`, `GbearOS_PB1_Core/`, and `GbearOS_PB2_Display/` that begins with a `// [GbearOS Component]` block (e.g. `// Name:`, `// Dependencies:`, `// Key Methods:`):

1. **Key Methods**: verify each declared method exists as a **public** method on the file’s primary API surface. Flag missing/typo/non-public/obsolete entries.
2. **Dependencies**: cross-check against `using` directives and actual references. Flag documented-only dependencies or missing critical dependencies where headers are intended to be normative.

### 2.2 IGC cross-check (required when `*Shared/igc/` exists)

1. Enumerate channel/tag constants and message identifiers defined under `*Shared/igc/` (and any adjacent shared contract files that hold IGC names).
2. Cross-check `docs/architecture/`, `docs/design/`, `docs/todo/`, `docs/history/`, and root `README.md` for IGC names presented as **current** contract. Flag:
   - docs-only channels (documented as current behavior but not in shared IGC)
   - code-only channels (defined but undocumented in normative docs)
   - naming drift (same intent, different spelling)
3. Treat `docs/history/**` as non-authoritative: if it contradicts shared IGC or normative docs, prefer fixing or clearly labeling archival context rather than treating history as contract truth.

---

## 3. Updates (documentation)

### 3.1 Standard README logic (root `README.md` is the User Manual)

The root `README.md` is the **professional, player-ready User Manual** for the public repo. It must read clearly for players while staying **technically accurate**—every claim should be traceable to source (`docs/`, DTOs, IGC contracts, renderer, Custom Data keys). Act as a **Technical Authoring Agent**: bridge code reality to player language; do not dump internal jargon without a plain-English gloss.

#### 3.1.1 Freedom of synthesis (dynamic feature mapping)

The AI has **creative freedom** to synthesize these sections from an **indexed pass** over the GbearOS codebase (DTOs under `*Shared/`, PB2 display/renderer and layout code, PB1 managers, `docs/configuration.md`, architecture docs):

- **Features** — Not a generic bullet list. Derive what players *see* and *do* from **telemetry DTOs** (inventory, ice, power, warnings, etc.) and from **LCD / `ScreenRenderer` (and related panel) behavior**: panels, tags, sprites, refresh cadence, and warning UX as implemented.
- **Installation** — Grounded in what ships in-repo (e.g. `dist/` minified scripts when present) and how players install programmable blocks in Space Engineers; align wording with actual artifact layout after scan.
- **LCD Tag Guide** — Built from **renderer logic and tag/panel conventions** in code (and `docs/configuration.md` where it defines Custom Data / LCD commands). List real tag tokens and behaviors, not placeholders.

If the scan cannot confirm a detail, omit it or point to `docs/configuration.md` / `docs/architecture/` instead of guessing.

#### 3.1.2 Standalone vs. system mode (required prose)

Explicitly explain both operational states:

- **Standalone mode:** PB1 (automation only) and PB2 (local LCDs only) **may run independently**. State clearly that **PB2** will show **no usable grid-wide telemetry** (e.g. inventory, ice, power aggregates) for metrics that come from PB1—typically **“NO DATA”** (or equivalent string the code uses) for those channels **unless** PB2 receives telemetry from PB1.
- **Dual-block system:** The **intended** experience: PB1 **orchestrates** (scanning, DTOs, IGC broadcast); PB2 **displays** synchronized dashboards.

#### 3.1.3 Connectivity and security (required section)

Include a **“Connectivity & Security”** (or equivalently titled) section with **exactly** this substance:

- **SharedKey:** Both programmable blocks **must** use **matching** `SharedKey` strings in Custom Data. Telemetry is verified using **Base64** and **FNV-1a** signatures; packets that fail verification (including **key mismatch**) are **ignored**.
- **PBID logic:** PB1 uses a **two-part** sender id on the wire: **`[User Prefix]-[Hex Suffix]`** from **`[Network]` `PBID`** (default prefix **`CMD`**).
  - The **3-character prefix** is **user-configurable** in Custom Data (human-friendly identification).
  - The **4-character hex suffix** is **automatically generated** by the programmable block and is **unique** (guaranteed uniqueness as implemented).

Wording may vary for readability; technical claims must not contradict the above.

#### 3.1.4 Player quick start (must lead the player path)

The README must give a **clear, step-first** path near the top (after a short intro/license pointer as needed):

1. **Installation** from `dist/` (minified scripts or whatever the repo actually ships—confirm on disk).
2. **Block tagging** — e.g. **`[GbearOS]`** in Custom Data or block name, per project convention from `docs/configuration.md` / code.
3. **Networking handshake** — matching **`SharedKey`** on both blocks (tie to §3.1.3).
4. **LCD commands** — a **compact table** of common panel/tag commands synthesized from **renderer logic** (e.g. `[STATUS]`, `[INV]`, `[ICE]`, `[PWR]`—only include tags the code actually honors; extend/correct from source during sync).

Then deeper sections (features, modes, security detail, links to full configuration/architecture) follow.

#### 3.1.5 Preserve / exclude (editorial guardrails)

- **PRESERVE**
  - Project branding (**GbearOS**)
  - Copyright / license notice
  - The **PB1 vs PB2 responsibility table** (or equivalent clear split)
  - Direct links to `docs/configuration.md`, `docs/architecture/`, and **developer build** instructions at `docs/setup.md` (one line is enough—README stays player-first)
- **EXCLUDE** from the root README (keep in `docs/` or `docs/setup.md`)
  - Internal-only tooling and private pipeline paths
  - Python build/deploy commands unless the public release explicitly documents them
  - Roadmap checkboxes / speculative planning blocks
  - “Private dev” markers
  - The script-size **100k character limit** in body text (keep size/budget gating in release tooling; avoid player-facing limit chatter unless you intentionally document it)

#### 3.1.6 Rewrite workflow

1. Complete §1 scan and §2 reconciliation so DTOs, IGC names, and renderer tags are loaded.
2. Rebuild `README.md` following §3.1.1–§3.1.5:
   - **Player Quick Start** (§3.1.4) immediately after the short intro / license pointer.
   - Then **Features**, **Installation**, and **LCD Tag Guide** (§3.1.1)—the Quick Start’s install line stays minimal; these sections add player-facing depth grounded in code.
   - Then **Standalone vs. system mode** (§3.1.2) and **Connectivity & Security** (§3.1.3).
   - Preserve the **PB1 vs PB2** split and **doc links** (§3.1.5).
3. Verify all links are **relative** and correct.
4. Keep length appropriate for GitHub front page: scannable headings, tables where they help, no dump of full INI reference (defer to `docs/configuration.md`).

### 3.2 Docs index hygiene

Ensure every `README.md` within a `docs/` subfolder lists and links to **all `.md` files in that same folder**. When §3.3 archives a file (history created, source path removed), update indexes and inbound links so entries do not rot.

### 3.3 Curate `docs/design/**`, `docs/todo/**`, and `docs/history/**` (required)

`docs/design/**` and `docs/todo/**` are **non-normative** planning tiers (per §0.1). `docs/history/**` holds **archived** plan/design narrative after completion. None of these tiers are player-facing contracts.

#### 3.3.1 Framing and accuracy (both `docs/design/` and `docs/todo/`)

- Each `docs/design/*.md` and `docs/todo/*.md` must clearly frame intent as **planned / partial / implemented / deprecated** (choose one) and must not read as a normative user contract.
- If a feature is **not** fully complete per §3.3.3, keep the file, but ensure framing matches reality and the file does not contradict normative docs.

#### 3.3.2 “100% implemented” (agent judgment)

For a given `docs/design/*.md` or `docs/todo/*.md`, treat the plan as **100% implemented** when **both** are true:

1. **Source:** By agent judgment, every **substantive** deliverable described by the plan (work items, phases, acceptance-style bullets, or equivalent structure) is satisfied in code at a **complete** level for the plan’s stated scope—not merely a partial slice—using the same class of evidence as §2 (types, dispatch, config keys, IGC identifiers, renderer behavior, etc.).
2. **Normative docs:** The same behavior surface is **already documented** in **normative** docs (per §0.1), with no material gap between what the plan claimed would exist and what normative docs describe as implemented.

If either condition fails, **do not** run the §3.3.4 archive workflow for that file.

#### 3.3.3 Stability before completion (no surprise archival)

Until §3.3.2 is satisfied, **do not** run §3.3.4 (no archival move, no removal of the `docs/design/` or `docs/todo/` source file)—**except**:

- **Minimal hygiene edits** are allowed (links, typos, status label corrections, de-conflicting statements that violate normative truth), and
- The user **explicitly** authorizes early retirement for a named path (or a clearly bounded set), in which case follow §3.3.5.

#### 3.3.4 Automatic archive workflow at 100% (default: history only; **no stub**)

When §3.3.2 is satisfied for a `docs/design/` or `docs/todo/` file:

1. Ensure normative docs remain sufficient after any edits from this pass (README / `docs/architecture/` / `docs/configuration.md` / other normative paths).
2. **Create** `docs/history/<descriptive-name>.md` containing the **archived narrative** (the prior substantive content of the plan/design), reframed as **historical** (completion date optional; must link to normative “current behavior” docs).
3. **Delete** the original `docs/design/...` or `docs/todo/...` source file (**do not** leave a stub or placeholder file at that path).
4. Update indexes per §3.2 (remove table rows / links that pointed at the deleted path), fix **inbound links** from other docs, and retarget `docs/ROADMAP.md` **`DOC:`** fields that referenced the removed file to **normative** paths (per §3.4.1 when status is `IMPLEMENTED`).
5. Maintain `docs/history/README.md` if the repo uses per-folder indexes (create a minimal index when adopting `docs/history/` for the first time—match existing repo conventions).

**Deletion policy:**

- **`docs/history/**`:** **never** delete unless the user **explicitly** requests deletion for that path (or an explicit bulk rule in the same user message/run instructions).
- **`docs/design/**` and `docs/todo/**`:** **do not** delete ad hoc. **Do** delete the specific source file as step **3** above when completing §3.3.4 (archive replaces that path with nothing—navigation lives in `docs/history/` + normative docs).

#### 3.3.5 Explicit user override (early or destructive)

If the user explicitly requests **delete** or **archive now** for a specific `docs/design/` or `docs/todo/` file **before** §3.3.2 is satisfied, follow their instruction:

- **Archive now:** run §3.3.4 early (history file + **delete** source path; **no stub**), but the `docs/history/**` preamble must clearly state **partial / superseded early** and link to normative docs for what is actually true in code.
- **Delete without history:** only if explicitly requested (rare); fix inbound links and any `docs/ROADMAP.md` references.

#### 3.3.6 Historical tier maintenance

- `docs/history/**` must remain clearly archival (no “this is how the mod works today” tone unless quoting legacy context).
- If archival text contradicts normative docs or source, correct with a short archival disclaimer and pointers—do not “freeze” incorrect contract claims in history without context.

### 3.4 ROADMAP maintenance (required, last)

After all other doc updates are complete, normalize and update `docs/ROADMAP.md` using a machine-readable structure.

#### 3.4.1 Canonical item format

Each roadmap item must be a single line checklist entry:

`- [ ] (ID: ROAD-001) [STATUS: PLANNED] [AREA: LCD] Short title. (DOC: docs/architecture/example.md) (EVIDENCE: code|docs-only|mixed)`

Rules:
- IDs are unique and stable.
- Allowed statuses: `PLANNED`, `PARTIAL`, `IMPLEMENTED`, `DEPRECATED`, `BLOCKED`.
- `AREA` is a small controlled vocabulary (e.g. `LCD`, `IGC`, `PB1`, `PB2`, `CONFIG`, `BUILD`, `DOCS`).
- If `STATUS` is `IMPLEMENTED`, `DOC:` must point to **normative** docs only (not `docs/design/`, `docs/todo/`, or `docs/history/`).

#### 3.4.2 Status resolution (deterministic)

- `IMPLEMENTED`: clear source evidence exists **and** normative docs exist (the `DOC:` reference is valid and describes the behavior).
- `PARTIAL`: some source evidence exists but behavior/docs are incomplete.
- `PLANNED`: no source evidence; may exist in `docs/design/` or `docs/todo/`.
- `DEPRECATED`: removed/superseded in code and normative docs describe replacement.
- `BLOCKED`: explicitly marked blocked (use sparingly).

---

## 4. Updates (new docs)

If a new module/directory exists without documentation:

1. Draft a short summary of what is missing.
2. Propose a concrete location under the existing doc layout.
3. Ask for permission before creating new files.

**Exception:** Creating `docs/history/**` material—and removing the archived `docs/design/` or `docs/todo/` source file per §3.3.4—is allowed without a separate permission prompt when the user invoked **doc sync** (`doc-sync` / `doc sync`) / documentation sync.

---

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `doc-sync`, `doc sync`

Equivalent natural-language requests to align or synchronize documentation with code also count.

---

## 6. Closing

Summarize what documentation was reconciled (stale claims removed, missing coverage added, links fixed, any design/todo items moved to `docs/history/` with source files removed) and confirm `docs/ROADMAP.md` was updated last. Do not run builds or deploy scripts in `gbearos-doc-sync`; release validation belongs to **git sync** (`git-sync` / `git sync`, gbearos-git-sync skill).
