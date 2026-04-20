---
name: gbearos-util-mdk-build
description: ⚙️ [PRIMITIVE] Runs dotnet build, resolves MDK2 deployed script paths from mdk.local.ini, and audits UTF-8 character counts/headroom for PB1 and PB2.
---

# Utility Primitive: MDK2 build + budget audit (`mdk-build` / `mdk build`)

## Purpose

Provide a single, repeatable **Space Engineers MDK2 build gate** and **minified script budget audit** (PB1 + PB2).

## Hard constraints

- **Never run git commands.**
- **Do not modify source code.** This is build + measure + report only.
- If build fails or deployed outputs cannot be located, **abort** and report what is missing (no guessing).

## Logic (execute exactly)

### 1) Build gate (required)

From the repository root, run:

- `dotnet build`

If the build fails, **abort** and report the error output. Budget results are **N/A**.

### 2) Resolve deployment paths (required)

Locate and parse `mdk.local.ini` in both directories:

- `GbearOS_PB1_Core/mdk.local.ini`
- `GbearOS_PB2_Display/mdk.local.ini`

For each file:

1. Extract the `output=` value.
2. Resolve environment variables (at minimum `%APPDATA%`; also resolve any other `%VARNAME%` tokens present).
3. If `output=auto`, resolve to MDK2’s default local deploy folder:
   - `%APPDATA%\SpaceEngineers\IngameScripts\local\<ProjectFolderName>`
4. Treat the resolved `output=` directory as the folder containing the deployed `script.cs`.
5. Verify each deployed script exists:
   - `<pb1_output_dir>\script.cs`
   - `<pb2_output_dir>\script.cs`

If either deployed `script.cs` is missing, **abort** and report which file is missing and the resolved directory.

### 3) UTF-8 character budget audit (required)

For each deployed `script.cs`:

- Read the file as **UTF-8**.
- Compute the **character count**.
- Compute remaining headroom against the **100,000** character limit:
  - `remaining = 100000 - count`

Report results in this exact format:

- **PB1**: `<count>` / 100000 (remaining `<remaining>`) — `<resolved full path to script.cs>`
- **PB2**: `<count>` / 100000 (remaining `<remaining>`) — `<resolved full path to script.cs>`

### 4) Handoff block (required)

End with a short handoff block that downstream orchestrators/utilities can copy verbatim:

- Resolved PB1 output directory: `<pb1_output_dir>`
- Resolved PB2 output directory: `<pb2_output_dir>`
- Resolved PB1 script path: `<pb1_output_dir>\script.cs`
- Resolved PB2 script path: `<pb2_output_dir>\script.cs`
- PB1 count + remaining: `<count>` / 100000 (remaining `<remaining>`)
- PB2 count + remaining: `<count>` / 100000 (remaining `<remaining>`)

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `mdk-build`, `mdk build`

Equivalent natural-language requests for an MDK2 build + deployed-script UTF-8 budget audit also count.

