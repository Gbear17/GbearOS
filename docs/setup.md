# Developer setup

Use this guide after cloning **GbearOS** to build the PB1 (Core) and PB2 (Display) scripts locally and deploy them into Space Engineers’ local script folders.

## Prerequisites

- **Malware’s Development Kit 2 (MDK2)** for Visual Studio — [recommended](https://github.com/malware-dev/mdk-2) for editing, IntelliSense, and MDK-aware project behavior.
- **.NET Framework 4.8 Developer Pack** (or an SDK/tooling setup that can build `net48` / `netframework48` MDK projects). Ensure `dotnet build` can target .NET Framework 4.8 on your machine.

## MDK2 initialization

Each script project ships a template for machine-local settings:

| Project | Template |
|--------|----------|
| PB1 Core | [`GbearOS_PB1_Core/mdk.local.ini.example`](../GbearOS_PB1_Core/mdk.local.ini.example) |
| PB2 Display | [`GbearOS_PB2_Display/mdk.local.ini.example`](../GbearOS_PB2_Display/mdk.local.ini.example) |

1. Copy `mdk.local.ini.example` to `mdk.local.ini` in **`GbearOS_PB1_Core/`**.
2. Copy `mdk.local.ini.example` to `mdk.local.ini` in **`GbearOS_PB2_Display/`**.

The real `mdk.local.ini` files are ignored by Git so your paths stay private.

## Path configuration

Open each new `mdk.local.ini` and set the **`output=`** line to your own Space Engineers **AppData** script location.

The templates use this shape (replace the username segment with your Windows user name, and keep or adjust the final folder name to match how you organize local scripts):

`C:\Users\REPLACE_WITH_YOUR_USERNAME\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB1_Core`  
`C:\Users\REPLACE_WITH_YOUR_USERNAME\AppData\Roaming\SpaceEngineers\IngameScripts\local\GbearOS_PB2_Display`

You may use `output=auto` instead if your MDK environment is configured for automatic deployment; the explicit path is the portable, documented default for new contributors.

## Build

From the **repository root** (where `GbearOS.slnx` lives), run:

```bash
dotnet build
```

That builds **both** projects. MDK uses your per-project `mdk.local.ini` **`output=`** paths so compiled scripts are written to the Space Engineers local script directories you configured.

If a project fails to find its `mdk.local.ini`, create it from the `.example` files as described above.
