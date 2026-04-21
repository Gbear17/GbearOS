> **Historical / archival** — Moved from `docs/todo/skills-branch-naming-diff-preflight-plan.md` under **§3.3.5 explicit user override** (“archive now”). Planning text here is **superseded** by the current orchestrator SKILL files and repo pipeline rules. For **current** behavior, see [`.cursor/rules/gbearos-pipeline.mdc`](../../.cursor/rules/gbearos-pipeline.mdc), [`.cursor/skills/gbearos-branch-complete/SKILL.md`](../../.cursor/skills/gbearos-branch-complete/SKILL.md), and [`.cursor/skills/gbearos-git-branch-init/SKILL.md`](../../.cursor/skills/gbearos-git-branch-init/SKILL.md).

# Plan: pipeline skills — diff-grounded branch naming *(archived)*

## Technical Approach

- Update **branch-complete** and **git-branch-init** orchestrator skills so that, before proposing a branch name, the agent runs read-only `git status`, `git diff`, and `git diff --cached`, and grounds `feat/` / `fix/` / `chore/` / etc. names in that evidence.
- Do not change PB1/PB2 script source; no MDK character budget impact from this work.
- Pipeline mutations (branch, commit, push) continue to follow existing Proceed gates and user orchestrator rules.

## File Checklist

- `.cursor/skills/gbearos-branch-complete/SKILL.md` — Phase 0 preflight and checklist (done locally).
- `.cursor/skills/gbearos-git-branch-init/SKILL.md` — step 2b diff-grounded naming; plan stem alignment note (done locally).

## Estimated Character Impact

**Character Impact Grounding (baseline from `gbearos-util-mdk-build` this run):**

- **PB1**: 53470 / 100000 (remaining 46530) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core\script.cs`
- **PB2**: 42867 / 100000 (remaining 57133) — `C:\Users\garre\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display\script.cs`

This change set touches only `.cursor/skills/*.md` and `docs/todo/`; **no** impact on deployed `script.cs` sizes.

## High-Risk Systems

- **Low risk:** documentation-only skill updates. Miswording could confuse agents; verify against workspace branch-prefix and mutation-approval rules.

---

Copyright (c) 2026 Garrett Wyrick. Archived from `docs/todo/skills-branch-naming-diff-preflight-plan.md` as part of `gbearos-doc-sync`. Licensed under GPL-3.0 — see repository `LICENSE`.
