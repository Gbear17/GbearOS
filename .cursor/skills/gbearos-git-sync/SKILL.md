---
name: gbearos-git-sync
description: 🚂 [ORCHESTRATOR] Delegates MDK2 build+budget audit, dist mirroring, and artifact verification to primitives, then stages/commits/pushes to origin after an explicit Proceed gate.
---

# Git synchronization Orchestrator (`git-sync` / `git sync`)

Act as a **Senior DevOps Engineer**. This is a **Composite Action Orchestrator**: it must **not** contain raw MDK2 build/path parsing/mirroring logic. It sequences the workflow and delegates heavy lifting to Utility Primitives.

## Execution order (must follow exactly)

### 0) Documentation sync (delegate to orchestrator)

1. Read and execute:
   - `.cursor/skills/gbearos-doc-sync/SKILL.md`
2. If it aborts for any reason, stop this workflow immediately.

### 1) Utility: Contract checklist gate (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-contract-check/SKILL.md`
2. If it aborts for any reason, stop this workflow immediately.

### 2) Utility: MDK2 build + budget audit (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-mdk-build/SKILL.md`
2. If it aborts for any reason, stop this workflow immediately.
3. Preserve the reported:
   - PB1/PB2 deployed `script.cs` paths
   - PB1/PB2 UTF-8 character counts and remaining headroom

### 3) Utility: Release artifact mirroring (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-dist-mirror/SKILL.md`
2. If it aborts for any reason, stop this workflow immediately.

### 4) Utility: Artifact verification gate (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-artifact-verification/SKILL.md`
2. If the exit code is **non-zero**, or the script cannot be invoked, stop this workflow immediately.
3. Preserve the **handoff summary** from that skill (exit code, merged transcript, PASS/FAIL/MISSING_DIST).

### 5) Git working tree review (required)

From the repo root:

1. Determine the current branch:
   - `git branch --show-current`
2. Review local changes:
   - `git status`
   - `git diff`

### 6) Draft a Conventional Commit message (required)

- Draft a Conventional Commits message appropriate to the diff (e.g. `feat(scope): ...`, `fix(scope): ...`).
- Keep the title imperative and concise; include a short body only when it materially clarifies *why*.

### 7) Hold for user approval (hard gate)

Present:

- The proposed commit message
- The PB1/PB2 character counts from Step 2
- The artifact verification handoff from Step 4 (exit code + outcome; full transcript if non-pass or if the user needs the log)
- A short “Ready to commit & push” summary stating what will be staged

**Stop.** Do not run `git add`, `git commit`, or `git push` until the user replies **Proceed** (or equivalent explicit approval).

### 8) Commit and push (after approval only)

From the repo root, in order:

1. `git add .`
2. `git commit -m "<approved message>"`
   - If there is nothing to commit, say so and do not create an empty commit.
3. Push to `origin` using the branch name from Step 5:
   - `git push origin <current-branch>`

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `git-sync`, `git sync`

Equivalent natural-language requests for this commit-then-push workflow also count.
