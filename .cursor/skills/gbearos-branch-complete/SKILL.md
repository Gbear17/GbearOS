---
name: gbearos-branch-complete
description: >-
  🚂 [ORCHESTRATOR] Manager/Supervisor. Runs a strict sequential pipeline: gated git sync → PR submission → PR-ready hold
  → optional merge. Delegates only by instructing the AI to read and follow specific orchestrator SKILL.md paths (repo-local
  and user-level). On main with local work, runs diff-grounded branch naming before branch-init. Does not calculate budgets
  or perform MDK/doc acceptance overlays.
---

# Branch completion pipeline (`branch-complete` / `branch complete`)

Act as a **Senior DevOps Engineer** and **strict pipeline supervisor**. This is a **Manager/Supervisor** skill: it does not do build/budget math, MDK parsing, or documentation acceptance redefinition. It only sequences the pipeline and enforces explicit checkpoints.

## Canonical SKILL.md paths (delegation targets)

Use these paths when instructing the AI what to read and follow **end to end** (including each file’s own **Proceed** / approval gates). **Do not** type skill names, aliases, or trigger phrases into the shell as if they were commands.

| Phase intent | SKILL.md to read and follow |
|--------------|------------------------------|
| New branch from `main` (MDK baseline, `docs/todo/`, push upstream) | **Repo:** `.cursor/skills/gbearos-git-branch-init/SKILL.md` |
| Full gated doc sync, build, dist, commit, push | **Repo:** `.cursor/skills/gbearos-git-sync/SKILL.md` |
| Open a PR | **User profile:** `%USERPROFILE%\.cursor\skills\pr-submit\SKILL.md` (Windows) or `~/.cursor/skills/pr-submit/SKILL.md` (macOS/Linux) |
| Merge PR and clean up branches | **User profile:** `%USERPROFILE%\.cursor\skills\pr-merge\SKILL.md` (Windows) or `~/.cursor/skills/pr-merge/SKILL.md` (macOS/Linux) |

---

## Non-negotiable execution contract

1. **Single-threaded orchestration** — Run **one phase at a time**. Do **not** spawn parallel agents, background jobs, or overlapping tool streams for work that belongs to **gbearos-git-sync**, **pr-submit**, or **pr-merge**.
2. **Delegate only via SKILL.md paths** — This supervisor must instruct the AI to **read and follow** the exact `SKILL.md` file path for each sub-orchestrator (table above), from repository root or user profile as listed. **Never** instruct typing skill triggers, skill aliases, or shorthand (for example strings resembling CLI names) into PowerShell, bash, or any terminal.
3. **Sub-skills stay encapsulated** — Do not inline the logic of those orchestrators; always delegate by the path-based instruction above.
4. **Checkpoints are blocking** — After each phase (and after the PR-ready hard hold), **stop** and wait for the user before starting the next phase, unless the contract below says **abort** (then the entire pipeline ends).
5. **STOP prompt for mutations** — Follow workspace pipeline rules: read-only git introspection may run without the consolidated STOP question; state-changing `git` / `gh` / `dotnet` steps require user authorization per the target `SKILL.md` and repo policy.

---

## Phase 0 — Preconditions (supervisor-only)

### Step 0) Branch safety and smart branch naming preflight (required)

From the repo root:

1. Determine the current branch:
   - `git branch --show-current`

If the active branch is **not** `main`, skip the “forgot to branch” workflow and continue to **Checkpoint 0 — Branch ready**.

If the active branch **is** `main`, run the **“forgot to branch”** workflow:

#### Smart branch naming preflight (read-only; no STOP)

1. **Run read-only inspection silently** — From the repo root, the agent **must** run **without** emitting a STOP/authorization prompt: `git status`, `git diff` (unstaged), and `git diff --cached` (staged). These commands are read-only. Optionally also `git diff HEAD` and/or `git diff --name-only HEAD` for a combined view—**do not** skip the unstaged/staged pair.
2. **Derive the branch identity from evidence** — Infer the suggested Conventional Commit `type` and kebab-case `[title]` **strictly from the actual code and path diffs** surfaced by those commands (files touched, hunks, scope)—**not** from chat topic alone. If there are **no** local changes, state that explicitly; then derive a minimal `type`/`[title]` only from a concise summary the user already gave for **why** a branch is needed, and still delegate branch creation below.
3. **Present for user visibility** (before delegating):
   - One Conventional Commit `type`: `feat`, `fix`, `perf`, `chore`, `docs`, or `refactor`
   - A short kebab-case `[title]` justified by diff evidence (or the empty-tree case above)
   - Proposed branch name: `[type]/[title]`
   - A 1–2 sentence branch description tied to what the diffs actually change (or the stated intent if there are no diffs)
4. **Delegate branch creation** — Instruct the AI to **read and follow** `.cursor/skills/gbearos-git-branch-init/SKILL.md` **end to end**, using the proposed `[type]/[title]` and description as the user-request summary input. Do not duplicate that skill’s internal steps here.
5. After **gbearos-git-branch-init** completes:
   - Confirm we are no longer on `main` (`git branch --show-current`)
   - Continue the pipeline at **Phase A**

**Checkpoint 0 — Branch ready**

- Summarize whether we started on `main` and (if so) what branch was created.
- **Wait** for explicit **Proceed** / **Go** before starting Phase A.

---

## Phase A — Gated git sync (includes docs)

1. Instruct the AI to **read and follow** `.cursor/skills/gbearos-git-sync/SKILL.md` **end to end** for this branch.
2. Treat Phase A as successful only if that orchestrator reports it **completed the commit and push** to `origin` (it runs **gbearos-doc-sync** then build/budget gates via utilities).
3. Do not start Phase C until Phase A is fully complete and Checkpoint A is cleared.

### Failure policy (supervisor — hard stop)

**Abort the entire branch-complete pipeline** (do **not** run PR submission or merge phases) if **any** of the following occur:

- `git push` **fails** (auth, rejected, non-fast-forward, remote hook failure, etc.).
- The working tree or index cannot be reconciled due to **merge conflicts** or **cherry-pick/rebase** conflicts during any step **gbearos-git-sync** would reasonably perform before a successful push.

**Checkpoint A — Private remote synchronized**

- Confirm **push succeeded** (remote and branch name).
- **Wait** for **Proceed** / **Go** before Phase C.

---

## Phase C — Pull request submission

1. Instruct the AI to **read and follow** the **pr-submit** orchestrator at `%USERPROFILE%\.cursor\skills\pr-submit\SKILL.md` (Windows) or `~/.cursor/skills/pr-submit/SKILL.md` (macOS/Linux) **end to end** for this branch.
2. Do not start Phase E during this phase.

**Checkpoint C — PR opened**

- Report PR **URL or number** returned by `gh`, and base **`main`** / head branch.
- **Wait** for **Proceed** / **Go** before **Hard Hold D** below.

---

## Hard Hold D — PR created and merge-ready (CRITICAL)

**Stop the entire pipeline** here. Ask the user **exactly** this confirmation question (verbatim), then **wait**:

> **Is the PR created and ready to merge?** Reply **yes** to continue to the **pr-merge** orchestrator (`SKILL.md` under your user profile `.cursor/skills/`), or **no** / describe blockers to stop here.

Rules:

- Treat **yes** (clear affirmative) as authorization to enter **Phase E** only — **not** as bypass of **pr-merge**’s own internal approvals.
- Treat **no**, silence, or “not yet” as **pipeline complete** from the supervisor’s perspective (the user may run the **pr-merge** `SKILL.md` workflow later in a new session).
- If the user says the PR is not created or `gh` failed, **do not** enter Phase E; they should fix PR submission using the **pr-submit** `SKILL.md` outside this run.

---

## Phase E — Merge to `main` (optional continuation)

**Only** if Hard Hold **D** received an explicit **yes**:

1. Instruct the AI to **read and follow** the **pr-merge** orchestrator at `%USERPROFILE%\.cursor\skills\pr-merge\SKILL.md` (Windows) or `~/.cursor/skills/pr-merge/SKILL.md` (macOS/Linux) in full.
2. Never run Phase E concurrently with any other phase.

**Checkpoint E — Pipeline finished**

- Short final summary: merged or not, local **main** state, whether the feature branch was deleted locally/remotely as applicable.

---

## Supervisor checklist (copy for the agent)

- [ ] **Phase 0:** Branch safety complete; if on `main`: silent read-only `git status`, `git diff`, `git diff --cached` run; branch `type`/`[title]` grounded in **actual diffs**; delegation via **read and follow** `.cursor/skills/gbearos-git-branch-init/SKILL.md` if applicable; user acknowledged Checkpoint 0
- [ ] **Phase A:** `.cursor/skills/gbearos-git-sync/SKILL.md` followed; successful push or **pipeline aborted**; Checkpoint A cleared
- [ ] **Phase C:** User-level **pr-submit** `SKILL.md` followed; PR URL captured; Checkpoint C cleared
- [ ] **Hard Hold D:** User answered **yes** before any merge work, else stopped
- [ ] **Phase E:** User-level **pr-merge** `SKILL.md` followed (if authorized), including its own merge approval and post-merge cleanup

---

## How users invoke this orchestrator (summary)

Natural-language requests to **sequentially** run gated git sync (including documentation sync), **PR submission**, **confirm PR readiness**, and optionally **merge** and clean up on the **active Git repository**, with **strict checkpoints** and **no parallel execution**, count as this workflow. User phrasing may include `branch-complete` or `branch complete` (hyphen and space interchangeable; case-insensitive unless noted).

**Execution remains path-based:** the agent always delegates by **read and follow [exact SKILL.md path]**—never by typing orchestrator nicknames into the terminal.
