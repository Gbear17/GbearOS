---
name: gbearos-branch-complete
description: >-
  🚂 [ORCHESTRATOR] Manager/Supervisor. Runs a strict sequential pipeline by instructing execution of sub-skills
  (git sync / git-sync → pr submit / pr-submit → PR-ready hold → pr merge / pr-merge). If started on main, delegates
  to git branch init / git-branch-init to create a branch first. Does not calculate budgets or perform
  MDK/doc acceptance overlays.
---

# Branch completion pipeline (`branch-complete` / `branch complete`)

Act as a **Senior DevOps Engineer** and **strict pipeline supervisor**. This is a **Manager/Supervisor** skill: it does not do build/budget math, MDK parsing, or documentation acceptance redefinition. It only sequences the pipeline and enforces explicit checkpoints.

## Non-negotiable execution contract

1. **Single-threaded orchestration** — Run **one phase at a time**. Do **not** spawn parallel agents, background jobs, or overlapping tool streams for **git-sync** / **git sync**, **pr-submit** / **pr submit**, or **pr-merge** / **pr merge** work.
2. **Delegate to sub-skills and utilities** — This supervisor must instruct the AI to run sub-skills using their project + user skill entrypoints (e.g. `git-sync` / `git sync`, `pr-submit` / `pr submit`, `pr-merge` / `pr merge`) and must not inline their logic.
3. **Checkpoints are blocking** — After each phase (and after the PR-ready hard hold), **stop** and wait for the user before starting the next phase, unless the contract below says **abort** (then the entire pipeline ends).
4. **Triggers:** see **Triggers (summary)** below.

---

## Phase 0 — Preconditions (supervisor-only)

### Step 0) Branch safety preflight (required)

From the repo root:

1. Determine the current branch:
   - `git branch --show-current`

If the active branch is **not** `main`, skip the “forgot to branch” workflow and continue to **Checkpoint 0 — Branch ready**.

If the active branch **is** `main`, run the **“forgot to branch”** workflow:

1. **Read-only diff preflight (required; no mutation approval):** From the repo root, the agent **must** run **silently** (no STOP prompt): `git status`, `git diff` (unstaged), and `git diff --cached` (staged). These commands are read-only. Optionally also `git diff HEAD` and/or `git diff --name-only HEAD` for a combined view—**do not** skip the unstaged/staged pair.
2. **Ground the branch name in the diffs:** Formulate the suggested Conventional Commit `type` and kebab-case `[title]` **from the actual diff content** (paths, edits, scope)—not from chat context alone. If there are no local changes, say so and derive a minimal name from any stated intent, then still delegate **git branch init** with that summary.
3. Draft and present:
   - A suggested Conventional Commit `type` (pick exactly one): `feat`, `fix`, `perf`, `chore`, `docs`, `refactor`
   - A short kebab-case `[title]` derived from the diff evidence (e.g. `docs-contracts-update`)
   - Proposed branch name: `[type]/[title]`
   - A 1–2 sentence “branch description” (why this branch exists)
4. Delegate branch creation via the existing orchestrator (avoid duplicating its behavior):
   - Instruct execution of **git branch init** (`git-branch-init` / `git branch init`) using the proposed `[type]/[title]` and description as the user-request summary input.
5. After **git branch init** completes:
   - Confirm we are no longer on `main` (`git branch --show-current`)
   - Continue the pipeline at **Phase A**.

**Checkpoint 0 — Branch ready**

- Summarize whether we started on `main` and (if so) what branch was created.
- **Wait** for explicit **Proceed** / **Go** before starting Phase A.

---

## Phase A — Git sync (includes docs) (`git-sync` / `git sync`)

1. Instruct execution of **git sync** (`git-sync` / `git sync`) end-to-end for this branch.
2. Treat Phase A as successful only if **git sync** reports it **completed the commit and push** to `origin` (it runs **gbearos-doc-sync** then build/budget gates via utilities).
3. Do not start **pr submit** (`pr-submit` / `pr submit`) until Phase A is fully complete and Checkpoint A is cleared.

### Failure policy (supervisor — hard stop)

**Abort the entire `branch-complete` pipeline** (do **not** run pr-submit or pr-merge) if **any** of the following occur:

- `git push` **fails** (auth, rejected, non-fast-forward, remote hook failure, etc.).
- The working tree or index cannot be reconciled due to **merge conflicts** or **cherry-pick/rebase** conflicts during any step **git sync** would reasonably perform before a successful push.

**Checkpoint A — Private remote synchronized**

- Confirm **push succeeded** (remote and branch name).
- **Wait** for **Proceed** / **Go** before Phase C.

---

## Phase C — Pull request submission (`pr-submit` / `pr submit`)

1. Instruct execution of **pr submit** (`pr-submit` / `pr submit`) end-to-end for this branch.
2. Do not start **pr merge** (`pr-merge` / `pr merge`) during this phase.

**Checkpoint C — PR opened (pr-submit complete)**

- Report PR **URL or number** returned by `gh`, and base **`main`** / head branch.
- **Wait** for **Proceed** / **Go** before the **Hard Hold** below.

---

## Hard Hold D — PR created and merge-ready (CRITICAL)

**Stop the entire pipeline** here. Ask the user **exactly** this confirmation question (verbatim), then **wait**:

> **Is the PR created and ready to merge?** Reply **yes** to continue to **pr merge** (`pr-merge` / `pr merge`), or **no** / describe blockers to stop here.

Rules:

- Treat **yes** (clear affirmative) as authorization to enter **Phase E** only — **not** as bypass of **pr-merge**’s own internal approvals.
- Treat **no**, silence, or “not yet” as **pipeline complete** from the supervisor’s perspective (user may run **pr merge** (`pr-merge` / `pr merge`) later manually).
- If the user says the PR is not created or `gh` failed, **do not** enter Phase E; return them to fixing **pr-submit** outside this run.

---

## Phase E — Merge to `main` (`pr-merge` / `pr merge`) — optional continuation

**Only** if Hard Hold **D** received an explicit **yes**:

1. Instruct execution of **pr merge** (`pr-merge` / `pr merge`) in full.
2. Never run Phase E concurrently with any other phase.

**Checkpoint E — Pipeline finished**

- Short final summary: merged or not, local **main** state, whether the feature branch was deleted locally/remotely as applicable.

---

## Supervisor checklist (copy for the agent)

- [ ] Phase 0: branch safety preflight completed (when on `main`: read-only `git status`, `git diff`, `git diff --cached` run; branch name grounded in diffs; **git branch init** (`git-branch-init` / `git branch init`) if applicable); user acknowledged Checkpoint 0
- [ ] Phase A: **git sync** (`git-sync` / `git sync`) executed and reported successful push; push succeeded or **pipeline aborted**; Checkpoint A cleared
- [ ] Phase C: **pr submit** (`pr-submit` / `pr submit`) executed; PR URL captured; Checkpoint C cleared
- [ ] Hard Hold D: user answered **yes** before any merge work, else stopped
- [ ] Phase E: **pr merge** (`pr-merge` / `pr merge`) executed (if authorized) including its own merge approval and post-merge cleanup

---

## Triggers (summary)

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `branch-complete`, `branch complete`

Equivalent requests to **sequentially** run **git sync** (including documentation sync), **pr submit**, **confirm PR readiness**, and optionally **pr merge** and clean up on the **active Git repository**, with **strict checkpoints** and **no parallel execution**, also count.
