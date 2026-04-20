---
name: gbearos-release-cut
description: 🚂 [ORCHESTRATOR] Tags a new version on main and publishes a GitHub Release with compiled binaries attached.
---

# Release cut Orchestrator (`release-cut` / `release cut`)

Act as a **Principal DevOps Architect**. This Orchestrator publishes official releases and therefore must be strict about branch, cleanliness, and explicit approval.

## Execution order (must follow exactly)

### 1) Main branch verification (hard gate)

From the repo root:

1. `git branch --show-current`
2. If the output is not exactly `main`:
   - **Abort immediately** and tell the user releases can only be cut from the `main` branch.

### 2) Pristine state check (hard gate)

From the repo root:

1. `git status`
2. If there are any uncommitted changes:
   - **Abort immediately** and instruct the user to run **git sync** (`git-sync` / `git sync`) first.

### 3) Utility: Contract checklist gate (delegate)

1. Read and execute:
   - `.cursor/skills/gbearos-util-contract-check/SKILL.md`
2. If it aborts for any reason, stop this workflow immediately.

### 4) Version collection (required)

Ask the user for:

- The Semantic Version tag (example: `v1.0.1`)
- A brief summary for the Release Notes

### 5) Hold for user approval (hard gate)

Present for confirmation:

- The version tag
- The release notes (exact text)
- The two `dist/` files that will be attached:
  - `dist/GbearOS_PB1_Core.cs`
  - `dist/GbearOS_PB2_Display.cs`

**Stop.** Do not create tags, push tags, or publish a release until the user replies **Proceed** (or equivalent explicit approval).

### 6) Git tagging (after approval only)

From the repo root, in order:

1. `git tag -a <version> -m "<notes>"`
2. `git push origin <version>`

### 7) GitHub CLI publish (after approval only)

From the repo root:

- `gh release create <version> --title "<version>" --notes "<notes>" dist/GbearOS_PB1_Core.cs dist/GbearOS_PB2_Display.cs`

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `release-cut`, `release cut`

Equivalent requests to tag and publish an official GitHub Release from `main` also count.
