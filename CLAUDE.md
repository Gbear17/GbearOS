# GbearOS — Claude Code instructions

This file contains GbearOS-specific pipeline workflows. Global workflows (`wip-save`, `pr-submit`, `pr-merge`) live in the global `~/.claude/CLAUDE.md`.

---

## Pipeline policy

**Read-only git introspection** (`git status`, `git diff`, `git log`, `git branch --show-current`, `gh pr view`, `gh pr checks`) may run freely without asking.

**State-changing commands** — any `git add`, `git commit`, `git push`, `git merge`, `git checkout` that changes branch or files, `gh pr create`, `gh pr merge`, `dotnet build`, `dotnet publish` — require explicit user authorization. Before running any of these, stop and ask a single consolidated question:

> A state-changing step is needed for: **[short description]**.
> Should I proceed with **[workflow name]**? Reply **proceed**, **yes**, **go**, **ok**, or **approve** to authorize.

Wait for an affirmative reply before taking any mutating action.

**Read-only introspection inside workflows** (e.g. `git status`, `git diff`, `git diff --cached` during branch-naming preflight) does not require an authorization prompt.

## Workflow routing

| Trigger | Workflow |
|---------|----------|
| `doc-sync` | Align docs with code (filesystem only, no git, no build) |
| `git-sync` | Full gated commit + push (doc sync → contract check → build → dist → verify → commit) |
| `git-branch-init` | New branch: baseline build, plan alignment, branch from `main`, push upstream |
| `branch-complete` | End-to-end: git sync → PR → hard hold → optional merge |
| `release-cut` | Tag and publish a GitHub Release from `main` |

---

## Workflow: `doc-sync`

**Trigger:** "doc-sync", "doc sync", "sync docs", "align docs with code"

**Constraint:** Never run git commands. Never modify source files. Treat `*.cs` source as ground truth, not `dist/`.

### 0. Discovery

Resolve the repository root (folder containing `README.md` and `docs/`). If ambiguous, stop and ask.

**GbearOS detection:** If both `*PB1_Core/*.csproj` and `*PB2_Display/*.csproj` exist, the MDK2 manifest check (§1.3) and IGC cross-check (§2.2) are **required** for this run.

### 0.1 Documentation tiers

| Tier | Paths | Requirement |
|------|-------|-------------|
| Normative (must match code) | `README.md`, `docs/**` excluding `docs/todo/`, `docs/design/`, `docs/history/` | No fictional behavior, no missing player-facing coverage |
| Curated non-normative | `docs/design/**` | Must be maintained; not a player contract |
| Planning | `docs/todo/**` | May describe future work; must be clearly framed as future |
| Archival | `docs/history/**` | Records completed decisions; not a current contract |

### 1. Scan

**1.1** Gather: root README, enumerate all `docs/` Markdown files (classify by tier), enumerate source files via `.csproj` manifests first then `**/*.cs`. Exclude `dist/`, `**/bin/`, `**/obj/`, `**/MDK/` from truth scans.

**1.3 MDK2 manifest vs disk** (required when `.csproj` files exist):
- Parse `Compile Include` / `Compile Remove` from both `.csproj` files
- Inventory on-disk `*.cs` under each PB subtree
- Report: **Orphaned** (on disk, not in csproj) and **Missing** (in csproj, not on disk)

### 2. Gap analysis

**2.0 Bidirectional coverage:**
- Docs → Code: normative docs must not describe behavior as implemented unless traceable to source (types, constants, config keys, dispatch logic, IGC identifiers, renderer behavior)
- Code → Docs: every player-facing behavior, tag token, IGC channel, and Custom Data key that exists in source must be documented somewhere in normative docs

**2.1 Header validation** (when `// [GbearOS Component]` blocks exist): For each `*.cs` under `GbearOS_Shared/`, `GbearOS_PB1_Core/`, `GbearOS_PB2_Display/` with a `// [GbearOS Component]` header — verify `Key Methods` are public methods that exist, and `Dependencies` cross-check against `using` directives.

**2.2 IGC cross-check** (when `*Shared/igc/` exists): Enumerate channel/tag constants from `*Shared/igc/`. Cross-check normative docs for: docs-only channels (documented but not in shared IGC), code-only channels (defined but undocumented), naming drift. Treat `docs/history/**` as non-authoritative.

### 3. Updates — README

`README.md` is the player-ready User Manual. Synthesize from DTOs, renderer/panel code, `docs/configuration.md`, and architecture docs. All claims must be traceable to source.

**Required sections and ordering:**
1. **Player Quick Start** (near the top): install from `dist/`, tag blocks with `[GbearOS]`, set matching `SharedKey` on both PBs, wire LCDs with tag commands
2. **Features**: derived from telemetry DTOs and LCD/renderer behavior — what players *see* and *do*
3. **Installation**: grounded in what's on disk (check whether `dist/` exists); actual SE programmable-block install steps
4. **LCD Tag Guide**: built from renderer logic and `docs/configuration.md`; only include tags the code actually honors
5. **Standalone vs. system mode**: PB1 can run alone (automation only); PB2 can run alone (local LCDs only, but shows **NO SIGNAL** / **WAITING FOR TELEMETRY…** for grid-wide metrics without PB1); dual-block is the intended experience
6. **Connectivity & Security**: `SharedKey` must match on both blocks (Base64 + FNV-1a MAC; mismatch → message ignored); PBID is `[Prefix]-[Hex Suffix]` where prefix is user-configurable (3 chars, default `CMD`) and hex suffix is auto-generated from entity ID
7. **PB1 vs PB2 responsibility table**
8. Links to `docs/configuration.md`, `docs/architecture/`, `docs/setup.md`

**Preserve:** GbearOS branding, copyright/license notice, PB1/PB2 split table, doc links.
**Exclude from README:** internal pipeline paths, build tooling details, roadmap checkboxes, script size limit discussion.

### 3.2 Doc index hygiene

Every `README.md` inside a `docs/` subfolder must list and link all `.md` files in that folder. When a file is archived (§3.3), update indexes and fix inbound links.

### 3.3 Plan curation and archival

Non-normative tiers (`docs/design/**`, `docs/todo/**`): each file must clearly frame intent as planned / partial / implemented / deprecated. Must not read as a normative player contract.

**When to archive** — a `docs/design/` or `docs/todo/` file is 100% implemented when **both**:
1. Every substantive deliverable is satisfied in source (types, dispatch, config keys, IGC identifiers, renderer behavior)
2. The same behavior is documented in normative docs with no material gap

If either condition fails, do not archive.

**Archive workflow** (when 100% implemented, or when user explicitly requests early):
1. Confirm normative docs remain sufficient
2. Create `docs/history/<descriptive-name>.md` with the prior substantive content, reframed as historical; link to current normative docs
3. Delete the original `docs/design/…` or `docs/todo/…` file (no stub)
4. Update indexes, fix inbound links, retarget `docs/ROADMAP.md` `DOC:` fields for this item to normative paths with status `IMPLEMENTED`
5. Update `docs/history/README.md`

**Never delete** `docs/history/**` unless the user explicitly requests it for a specific path.

Minimal hygiene edits (links, typos, framing corrections) are allowed without archiving.

### 3.4 ROADMAP normalization (last step)

Each item in `docs/ROADMAP.md` must follow:

```
- [ ] (ID: ROAD-001) [STATUS: PLANNED] [AREA: LCD] Short title. (DOC: docs/architecture/example.md) (EVIDENCE: code|docs-only|mixed)
```

- IDs: unique and stable
- Status values: `PLANNED`, `PARTIAL`, `IMPLEMENTED`, `DEPRECATED`, `BLOCKED`
- If `STATUS: IMPLEMENTED`, `DOC:` must point to normative docs only (not design/todo/history)
- Area vocabulary: `LCD`, `IGC`, `PB1`, `PB2`, `CONFIG`, `BUILD`, `DOCS`

### 4. New doc proposals

If a module exists without documentation, draft a summary and proposed location, then ask for permission before creating new files. **Exception:** creating `docs/history/**` material (and deleting the archived source per §3.3) does not require a separate prompt when running doc-sync.

### Closing

Summarize: stale claims removed, missing coverage added, links fixed, plans archived, ROADMAP updated. Do not run builds or push git changes in doc-sync.

---

## Workflow: `git-sync`

**Trigger:** "git-sync", "git sync", equivalent requests for a gated commit + push

Run each phase in order. Abort immediately if any phase fails.

### Phase 0 — doc-sync
Run the **doc-sync** workflow above in full.

### Phase 1 — contract-check
Run the **contract-check** sub-workflow below.

### Phase 2 — mdk-build
Run the **mdk-build** sub-workflow below. Preserve the PB1/PB2 script paths and character counts for the approval gate.

### Phase 3 — dist-mirror
Run the **dist-mirror** sub-workflow below.

### Phase 4 — artifact-verification
Run the **artifact-verification** sub-workflow below. If exit code is non-zero, abort.

### Phase 5 — Working tree review

```
git branch --show-current
git status
git diff
```

### Phase 6 — Draft commit message

Draft a Conventional Commit message grounded in the diff (e.g. `feat(scope): …`). Imperative title, short body only when it clarifies *why*.

### Phase 7 — Approval gate (hard stop)

Present:
- Proposed commit message
- PB1/PB2 character counts from Phase 2
- Artifact verification result from Phase 4 (exit code + PASS/FAIL/MISSING_DIST; full transcript if non-pass)
- What will be staged

**Stop. Do not run `git add`, `git commit`, or `git push` until the user replies Proceed.**

### Phase 8 — Commit and push (after approval only)

```
git add .
git commit -m "<approved message>"
git push origin <current-branch>
```

If nothing to commit, say so and do not create an empty commit.

---

## Workflow: `git-branch-init`

**Trigger:** "git-branch-init", "git branch init", equivalent requests to create a new task branch

Run each phase in order. Abort immediately if any phase fails.

### Phase 1 — mdk-build baseline

Run the **mdk-build** sub-workflow. Preserve PB1/PB2 paths and character counts.

### Phase 2 — Verify we are on `main`

```
git branch --show-current
```

If not `main`: stop and instruct the user to switch to `main` first. Then:

```
git fetch origin
```

### Phase 2b — Diff-grounded branch naming (read-only, no authorization prompt)

Run silently:

```
git status
git diff
git diff --cached
```

- If either diff is non-empty: derive Conventional Commit `type` and kebab-case `[title]` **from the actual diffs** (files touched, paths, scope) — not from chat topic alone.
- If both diffs are empty: derive from the user's stated intent and any relevant `docs/todo/` plan filename.

Conventional Commit types: `feat`, `fix`, `perf`, `chore`, `docs`, `refactor`.
Proposed branch name format: `[type]/[title]`

### Phase 3 — Plan file alignment (before creating the branch)

**Find before creating:**
1. If the user named a `docs/todo/` path, use it.
2. Else scan `docs/todo/` for `*-plan.md` matching the branch purpose.
3. Else scan `docs/history/` for a matching plan — if work is already implemented, use the history file as the canonical plan and do not create a new `docs/todo/` file.
4. If found and purpose matches: **do not create a duplicate**; only patch what is missing.
5. If found but purpose does not match: stop and ask the user to clarify.
6. If nothing found: create `docs/todo/[title]-plan.md`.

**Required plan contents** (for new files; for patches, only add missing sections):
1. **Technical Approach** — Execution-critical requirements, constraints, forbidden actions
2. **File Checklist** — Concrete paths to create/modify; high-risk touch points
3. **Estimated Character Impact** — Must cite the exact PB1/PB2 baseline from Phase 1 (paths + counts + remaining headroom)
4. **High-Risk Systems** — Highest-coupling or break-risk files

Do not add "delete this file before PR" footers. Archival is handled by doc-sync.

### Phase 4 — Approval gate (hard stop)

Present:
- Proposed branch name
- Path to plan file used (existing / patched / created)
- Whether plan was unchanged, patched (what was added), or created
- Baseline PB1/PB2 counts cited in the plan

**Stop. Do not create the branch or push until the user replies Proceed.**

### Phase 5 — Create branch and push (after approval only)

```
git checkout -b [type]/[title]
git push -u origin [type]/[title]
```

---

## Workflow: `branch-complete`

**Trigger:** "branch-complete", "branch complete", equivalent requests for a gated end-to-end branch wrap-up

Single-threaded. One phase at a time. No parallel execution.

### Phase 0 — Branch safety and naming preflight

```
git branch --show-current
```

**If on a feature branch:** skip to Checkpoint 0.

**If on `main`:** run the "forgot to branch" workflow:
1. Run silently (no approval prompt): `git status`, `git diff`, `git diff --cached`
2. Derive branch `type` and `[title]` from the diffs (same rules as git-branch-init Phase 2b)
3. Present: proposed `type`, `[title]`, branch name `[type]/[title]`, 1–2 sentence description grounded in diffs
4. Run the **git-branch-init** workflow using the proposed name
5. Confirm we are no longer on `main`

**Checkpoint 0:** Summarize branch state. **Wait for explicit Proceed before Phase A.**

### Phase A — Gated git sync

Run the **git-sync** workflow in full. Phase A succeeds only when push to `origin` completes.

**Abort the entire pipeline** (do not proceed to PR phases) if:
- `git push` fails for any reason
- Working tree cannot be reconciled due to merge conflicts

**Checkpoint A:** Confirm push succeeded (remote and branch name). **Wait for Proceed before Phase C.**

### Phase C — Pull request submission

Run the **pr-submit** workflow (from global CLAUDE.md).

**Checkpoint C:** Report PR URL. **Wait for Proceed before Hard Hold D.**

### Hard Hold D — Merge readiness (critical stop)

Ask the user **exactly**:

> **Is the PR created and ready to merge?** Reply **yes** to continue to the merge step, or **no** / describe blockers to stop here.

- **yes** → proceed to Phase E
- **no** / silence / "not yet" → pipeline complete; user may merge manually later

### Phase E — Merge to `main` (only if Hard Hold D answered yes)

Run the **pr-merge** workflow (from global CLAUDE.md).

**Checkpoint E:** Short final summary — merged or not, current local branch, whether feature branch was cleaned up.

---

## Workflow: `release-cut`

**Trigger:** "release-cut", "release cut", equivalent requests to tag and publish a GitHub Release

### Phase 1 — Branch verification (hard gate)

```
git branch --show-current
```

If not exactly `main`: abort. Releases can only be cut from `main`.

### Phase 2 — Pristine state check (hard gate)

```
git status
```

If any uncommitted changes: abort. Run **git-sync** first.

### Phase 3 — contract-check

Run the **contract-check** sub-workflow. If it fails, abort.

### Phase 4 — Collect version and notes

Ask the user for:
- Semantic version tag (e.g. `v1.0.1`)
- Brief release notes summary

### Phase 5 — Approval gate (hard stop)

Present:
- Version tag
- Release notes (exact text)
- Files to attach: `dist/GbearOS_PB1_Core.cs`, `dist/GbearOS_PB2_Display.cs`

**Stop. Do not tag or publish until the user replies Proceed.**

### Phase 6 — Git tagging (after approval only)

```
git tag -a <version> -m "<notes>"
git push origin <version>
```

### Phase 7 — GitHub Release (after approval only)

```
gh release create <version> --title "<version>" --notes "<notes>" dist/GbearOS_PB1_Core.cs dist/GbearOS_PB2_Display.cs
```

---

## Sub-workflow: `contract-check`

Read-only. No git. No source modifications. Abort immediately on the first failing check.

**Required files must exist:**
- `GbearOS_Shared/igc/channels.cs`
- `GbearOS_Shared/network/SenderEnvelope.cs`
- `GbearOS_Shared/igc/igc_serializer.cs`
- `GbearOS_Shared/igc/serialization.cs`
- `GbearOS_PB2_Display/igc_serializer_deserialize.cs`
- `GbearOS_PB2_Display/serialization_deserialize.cs`
- `GbearOS_PB1_Core/GbearOS_PB1_Core.csproj`
- `GbearOS_PB2_Display/GbearOS_PB2_Display.csproj`

**PB2-only files must not exist in Shared:**
- `GbearOS_Shared/igc/igc_serializer_deserialize.cs`
- `GbearOS_Shared/igc/serialization_deserialize.cs`

**Both `.csproj` files must include Shared:** verify each contains `Compile Include="..\GbearOS_Shared\**\*.cs"`.

**PB1 must not call deserialize entrypoints:** search `GbearOS_PB1_Core/` for `Serializer.Deserialize` or `IGCSerializer.Deserialize`. Abort and report file/line if any hits found.

**On success, report:**
```
Contract gate: PASS
Shared compiled by: PB1 + PB2
PB2-only deserialize: PB2 project only
```

---

## Sub-workflow: `mdk-build`

No git. No source modifications. Abort and report on any failure — no guessing.

**Step 1 — Build:**

```
dotnet build
```

If build fails, abort and report error output. Budget results are N/A.

**Step 2 — Resolve deployment paths:**

Parse `GbearOS_PB1_Core/mdk.local.ini` and `GbearOS_PB2_Display/mdk.local.ini`. For each:
1. Extract `output=` value
2. Resolve `%APPDATA%` and any other `%VARNAME%` tokens
3. If `output=auto`, resolve to `%APPDATA%\SpaceEngineers\IngameScripts\local\<ProjectFolderName>`
4. Verify `<output_dir>\script.cs` exists (abort and report the exact missing path if not)

**Step 3 — UTF-8 character budget:**

Read each deployed `script.cs` as UTF-8. Compute character count. Remaining headroom = `100000 - count`.

Report:
```
PB1: <count> / 100000 (remaining <remaining>) — <full path to script.cs>
PB2: <count> / 100000 (remaining <remaining>) — <full path to script.cs>
```

**Handoff block** (required — downstream steps depend on this):
```
PB1 output dir: <pb1_output_dir>
PB2 output dir: <pb2_output_dir>
PB1 script: <pb1_output_dir>\script.cs
PB2 script: <pb2_output_dir>\script.cs
PB1: <count> / 100000 (remaining <remaining>)
PB2: <count> / 100000 (remaining <remaining>)
```

---

## Sub-workflow: `dist-mirror`

Requires the mdk-build handoff block (PB1/PB2 output dirs) from the same workflow run.

1. Ensure `dist/` exists at repo root (create if missing)
2. Copy `<pb1_output_dir>\script.cs` → `dist/GbearOS_PB1_Core.cs`
3. Copy `<pb2_output_dir>\script.cs` → `dist/GbearOS_PB2_Display.cs`

Abort and report the exact path if any source file is missing or any copy fails.

---

## Sub-workflow: `artifact-verification`

Run the PowerShell gate as a subprocess. Do **not** manually read or grep `dist/*.cs` — all minified checks live inside the script.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1" 2>&1
```

Use `pwsh` instead of `powershell` if PowerShell 7+ is the active environment.

Capture full merged output (stdout + stderr) and exit code.

| Exit code | Meaning |
|-----------|---------|
| `0` | PASS — all checks passed |
| `1` | FAIL — one or more checks failed (see `FAIL:` lines in output) |
| `2` | MISSING_DIST — expected `dist/` files not present (run mdk-build and dist-mirror first) |

**Required handoff summary:**
```
Artifact verification: <PASS | FAIL | MISSING_DIST>
Exit code: <n>
Transcript: <full merged output>
```

On PASS, add: `dist gate: script-owned checks only (no AI-side dist inspection).`

If exit code is non-zero, include the full transcript and do not attempt to fix dist content in this step.
