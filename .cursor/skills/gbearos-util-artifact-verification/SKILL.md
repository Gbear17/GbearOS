---
name: gbearos-util-artifact-verification
description: ⚙️ [PRIMITIVE] Runs the PowerShell artifact gate on dist/ via scripts/artifact-verification.ps1 (relative to this skill root), reports exit code and full merged console output (stdout + stderr), and never runs git. Does not manually open, grep, or audit dist/GbearOS_PB1_Core.cs or dist/GbearOS_PB2_Display.cs; all minified checks stay inside the script. Use when the user asks for artifact verification / artifact-verification, after dist mirror / dist-mirror in a build workflow, or when a dist artifact gate is needed.
---

# Utility Primitive: Artifact verification gate (`artifact-verification` / `artifact verification`)

## Purpose

Run the repository’s **deterministic dist/ artifact gate** (string-anchor checks on minified PB outputs) as a single subprocess. Report **exit code** and **complete console output** so the user sees the same result as a local run.

Rationale and check inventory live in `docs/architecture/artifact_verification.md` (reference only; do not duplicate the checklist inside this skill).

## Hard constraints

- **Never run git commands.**
- **Do not modify source code or scripts** for this step—execute and report only.
- **Do not** open, search (`grep`/`rg`), or otherwise manually audit `dist/GbearOS_PB1_Core.cs` or `dist/GbearOS_PB2_Display.cs`. All minified verification is owned by **`scripts/artifact-verification.ps1`** (path relative to this skill’s root folder).

## Preconditions (required)

- Working directory is the **repository root** (the folder that contains `dist/`).
- **`dist/` is populated** with mirrored minified artifacts—typically after **`gbearos-util-mdk-build`** (deploy) and **`gbearos-util-dist-mirror`**. If `dist/` is missing expected files, the script exits non-zero (see table below).

## Logic (execute exactly)

From the repository root, run the gate in a subprocess and **merge stderr into stdout** (`2>&1`) so the captured transcript is complete.

Bundled script (relative to this skill root, per Cursor skill layout): `scripts/artifact-verification.ps1`.

Preferred invocation (Windows PowerShell 5.1 compatible):

- `powershell -NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1" 2>&1`

If the environment already uses PowerShell 7+ for automation, `pwsh` with the same `-NoProfile -ExecutionPolicy Bypass -File ".cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1" 2>&1` is acceptable.

**Capture:**

- The **full text stream** (all lines from the merged output).
- The process **exit code** after the command completes (e.g. in PowerShell, `$LASTEXITCODE` immediately after the invocation).

**Do not** substitute manual reads of `dist/*.cs` for script output.

### Exit code table

| Code | Meaning |
|------|---------|
| `0` | **PASS** — all artifact checks passed. |
| `1` | **FAIL** — one or more checks failed (see script output for `FAIL:` lines). |
| `2` | **Missing dist inputs** — expected files under `dist/` not present (build and mirror first). |

### Handoff summary block (required)

End the report with a short block downstream orchestrators can copy:

- `Artifact verification: <PASS \| FAIL \| MISSING_DIST>`
- `Exit code: <n>`
- `Transcript:` (full merged stdout/stderr from the run)

On **PASS** (`0`), you may add one line: `dist gate: script-owned checks only (no AI-side dist inspection).`

## Failure policy

- If the exit code is **non-zero**, treat the gate as failed: include the **full transcript** and the numeric code; do not attempt to “fix” dist content by editing files in this primitive.
- If the script cannot be started (missing file, policy error), report the error from the shell and **do not** fall back to manual dist auditing.

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `artifact-verification`, `artifact verification`

Also when the user asks for the **dist artifact gate** in natural language, or as a follow-on after **dist mirror** (`dist-mirror` / `dist mirror`) (and a successful build/deploy path) when release-style verification is required.

## Related skills

- **`gbearos-util-mdk-build`** — build + resolve deployed `script.cs` paths + UTF-8 budget.
- **`gbearos-util-dist-mirror`** — copy deployed minified scripts into `dist/GbearOS_PB1_Core.cs` and `dist/GbearOS_PB2_Display.cs`.
- **`gbearos-git-sync`** — invokes this gate as part of the synced pipeline when configured.
