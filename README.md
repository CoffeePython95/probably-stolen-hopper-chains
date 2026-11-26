# Hopper Chains

A **Probably Stolen** mod that makes chains of hoppers forward items through linked containers.

## Features

- An intake hopper's delivery triggers every linked intake hopper in the
  receiving container, then repeats down the chain.
- Works with the intake window's **Transfer now** button, the outlet window,
  `On Add`/`On Move`, and the nightly hopper pass.
- Respects each intake's filter, keep amount, colour, and run order.
- The `On Add` toggle is not required for hopper-to-hopper handoffs, but
  manual and machine additions still use it.
- Loop protection: a visited-hopper set plus a configurable maximum chain
  depth.
- Settings for enabling, requiring `On Add`, depth limit, and transfer
  logging.

## Install (players)

No build tools are needed.

1. Go to the [latest release](../../releases/latest).
2. Download `HopperChains-v<version>.zip`.
3. Extract the `HopperChains` folder from the zip into:

   ```text
   <Steam library>\steamapps\common\Probably Stolen\Mods\
   ```

   The result should look like:

   ```text
   Probably Stolen\Mods\HopperChains\manifest.xml
   Probably Stolen\Mods\HopperChains\HopperChains.dll
   ```

4. Start the game, open **Mods**, trust/enable **Hopper Chains**, and restart.

The mod targets **Probably Stolen 0.46D**.

## Usage

Set up each container after the first with an **intake** hopper linked to the
**outlet** hopper in the next container:

```text
A: items + intake #1          B: outlet #1 + intake #2          C: outlet #2
                                (link intake #2 to outlet #2)
```

When a hopper delivers items into B, the linked intake in B runs immediately and
sends them on to C, and so on. The `On Add` toggle is not needed on the
receiving hoppers for this hopper-to-hopper handoff. Manual or machine
additions still use the vanilla `On Add` toggle on the receiving hopper.

Filters, keep amounts, and colours still apply to each intake. The order slider
still controls the nightly hopper pass: during that pass a handoff only
continues into the same run-order window and never backwards. During the day a
handoff runs the receiving hopper immediately.

## Settings

The game's **Mods** screen shows:

- **Enabled** - turn the whole mod on or off.
- **Only chain hoppers with 'On Add' enabled** - off by default.
- **Maximum chain depth** - safety limit, default 32.
- **Log chain transfers** - writes each hop to `Player.log`.

## Build from source

Requirements:

- Probably Stolen installed locally.
- .NET 8 SDK.

From this folder:

```powershell
.\build.ps1 -GameDir "E:\SteamLibrary\steamapps\common\Probably Stolen"
```

This produces:

```text
dist\HopperChains\                  unpacked mod folder
dist\HopperChains-v0.1.0.zip       release archive
```

To build, package and install directly into the local game:

```powershell
.\build.ps1 -GameDir "E:\SteamLibrary\steamapps\common\Probably Stolen" -Install
```

Then enable/trust the mod in the game's **Mods** screen and restart. Pass
`-GameDir` if the game is installed elsewhere.

The build uses the game's own `Assembly-CSharp.dll`, so it cannot be compiled
on a machine without the game installed.

## License

[MIT](LICENSE)
