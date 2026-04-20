---
name: gbearos-util-dist-mirror
description: ⚙️ [PRIMITIVE] Mirrors deployed minified PB scripts into dist/ at repo root.
---

# Utility Primitive: Release artifact mirroring (`dist-mirror` / `dist mirror`)

## Purpose

Mirror the deployed minified `script.cs` outputs into the repository’s `dist/` folder for release/copy-paste consumption.

## Preconditions (required)

- **Assume `gbearos-util-mdk-build` has already run successfully** in the same workflow.
- You must have the **resolved PB1/PB2 deployed `script.cs` paths** from `gbearos-util-mdk-build`’s handoff block.

## Logic (execute exactly)

1. Ensure `dist/` exists at the repository root (create it if missing).
2. Copy the deployed PB1 minified script:
   - Source: `<pb1_output_dir>\script.cs` (from `gbearos-util-mdk-build`)
   - Destination: `dist/GbearOS_PB1_Core.cs`
3. Copy the deployed PB2 minified script:
   - Source: `<pb2_output_dir>\script.cs` (from `gbearos-util-mdk-build`)
   - Destination: `dist/GbearOS_PB2_Display.cs`
4. If any copy fails or a source file is missing, **abort** and report the exact missing path / error.

## Triggers

Treat hyphen and space as interchangeable in user phrasing (case-insensitive unless noted).

- `dist-mirror`, `dist mirror`

Equivalent natural-language requests to mirror deployed minified scripts into `dist/` also count.
