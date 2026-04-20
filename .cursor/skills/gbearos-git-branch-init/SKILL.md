---
name: gbearos-git-branch-init
description: 🚂 [ORCHESTRATOR] Establishes MDK2 baseline via gbearos-util-mdk-build, ensures a docs/todo plan cites that baseline (creates or minimally patches an existing plan), then (after Proceed) branches from main and pushes upstream.
---

# Architectural sandbox Orchestrator (`git-branch-init` / `git branch init`)

Act as a **Senior Software Architect & DevOps Engineer**. This is a **Composite Action Orchestrator**. It must delegate MDK2 build/budget baseline acquisition to the utility primitive and must not embed raw MDK parsing or budget logic.

**Constraints:**

- All plans **must** live under `docs/todo/`.
- This workflow must branch **off `main`**.

## Execution order (must follow exactly)

### 1) Utility: MDK2 build + baseline budget audit (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-mdk-build/SKILL.md`
2. If it aborts, stop this workflow immediately.
3. Preserve the baseline outputs from its report:
   - PB1/PB2 deployed `script.cs` paths
   - PB1/PB2 UTF-8 character counts and remaining headroom

### 2) Ensure we are branching off `main` (required)

From the repo root:

1. Run `git branch --show-current`.
2. If the active branch is not `main`, **stop** and instruct the user to switch to `main` before continuing.
3. Run `git fetch origin` (keep `origin` as the primary remote).

### 3) Plan file under `docs/todo/` (required, before creating the branch)

Derive:

- `[title]` as a short kebab-case name for this work (when creating a new file from scratch).
- A Conventional Commit `type` (pick exactly one): `feat`, `fix`, `perf`, `chore`, `docs`, `refactor`.
- Proposed branch name: `[type]/[title]`
- When reusing an existing `docs/todo/<stem>-plan.md`, derive `[title]` from `<stem>` (the kebab-case work name in the filename) unless the user explicitly asked for a different branch slug; keep branch name and canonical plan filename aligned.

**Existing plan first (do not duplicate)**

Before creating or overwriting anything:

1. If the user **named a path** under `docs/todo/` (or pasted one), treat that file as the **canonical plan** for this branch init.
2. Else, scan `docs/todo/` for `*-plan.md`. If one file clearly matches the **described branch purpose** (same feature/fix area and intent), treat it as the canonical plan.
3. If a canonical plan exists:
   - **Read it** and compare to what the user said this branch is for.
   - If the purpose **matches** (same workstream; no contradiction on scope or constraints): **do not** create a second plan file and **do not** replace the whole document. Only **edit the file if something required is missing or stale** (see **Minimum edits** below).
   - If the purpose **does not match** (different feature, conflicting scope, or wrong doc): **stop** and ask the user which to do: (a) point at the correct existing `docs/todo/` file, (b) create a **new** `docs/todo/[title]-plan.md` with a distinct `[title]`, or (c) abandon branch init until the plan story is clear. Do not silently overwrite an unrelated plan.

**Minimum edits** (when using an existing canonical plan)

Apply **only** what is missing; skip the rest:

- If **Estimated Character Impact** (or equivalent baseline grounding) does not cite the **current** `gbearos-util-mdk-build` PB1/PB2 paths, counts, and headroom: **add or replace** that subsection so it cites this run’s baseline. Prefer a short dated one-line note if the only change is refreshed numbers.
- If a required section from **Full plan contents** (below) is wholly absent, **insert** that section; do not rewrite unrelated sections.
- If all required sections are present and baseline is current: **no** doc change for step 3.

**New plan file** (only when there is no suitable existing plan)

Create `docs/todo/[title]-plan.md` and populate it from the user request.

**Full plan contents** (required for any plan file that is **created from scratch**; when patching an existing file, only add missing parts):

1. **Technical Approach** — Restate every execution-critical requirement from the user request (constraints, limits, forbidden actions).
2. **File Checklist** — Files to create/modify (concrete paths; call out high-risk touch points).
3. **Estimated Character Impact** — **Character Impact Grounding must cite the baseline from `gbearos-util-mdk-build`.**
   - Include the exact PB1/PB2 baseline lines (counts + remaining) and the resolved `script.cs` paths reported by `gbearos-util-mdk-build`.
   - Use those values as the baseline when estimating post-change size/headroom.
4. **High-Risk Systems** — Systems/files with the highest coupling or break risk.
5. **Cleanup reminder** — The plan must end with exactly:

```markdown
### Cleanup reminder

**DELETE THIS FILE BEFORE FINAL PR SUBMISSION.**
```

### 4) Hold for approval (hard gate)

Present:

- The proposed branch name (`[type]/[title]`)
- The path to the plan file actually used (existing canonical doc or newly created `docs/todo/[title]-plan.md`)
- Whether the plan file was **unchanged**, **patched** (say what was added), or **created**
- The baseline PB1/PB2 counts from `gbearos-util-mdk-build` that were cited or verified in the plan

**Stop.** Do not create the branch or push until the user replies **Proceed** (or provides edits).

### 5) Create branch and push (after approval only)

From the repo root, in order:

1. `git checkout -b [type]/[title]`
2. `git push -u origin <branch-name>`

Where `<branch-name>` is exactly `[type]/[title]`.

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `git-branch-init`, `git branch init`

Equivalent requests to ensure a `docs/todo/` plan cites the MDK2 baseline and then create/push a new sandbox branch off `main` also count. When the user already has or references a plan doc for the described work, prefer that file and only edit it when baseline or required sections are missing or stale.
