# Hopper Chains

Makes chains of **Probably Stolen** hoppers work.

Vanilla hoppers move items exactly one step: an intake hopper in container A,
linked to an outlet hopper in container B, empties A into B. If B contains a
second intake hopper linked to an outlet in container C, the game does not run
that second hopper when the first one delivers. Items sit in B until you move
them by hand, or until the nightly hopper pass reaches that hopper.

This mod hooks the end of a successful hopper transfer and runs every linked
intake hopper in the receiving container, then repeats for each following
container. A chain like

```text
container A --[intake]--> container B --[intake]--> container C --> ...
```

now moves items all the way through in one go.

## Why vanilla stops

The game's `HopperHelper` supports an `On Add` toggle: an intake hopper watches
its parent inventory and queues itself when a non-hopper item is added.
However, `HopperHelper.Schedule` refuses to queue anything while a hopper
transfer is running:

```csharp
// HopperHelper.Schedule
if (!batchRunning && !transferring && intake != null && !IsAway())
```

An intake hopper moves items inside `HopperHelper.Run`, which sets
`transferring = true` for the whole transfer. The receiving container's
`onSlotAddItemEndFunc` still fires, but `Schedule` drops the event. Enabling
`On Add` on the second hopper therefore works for items you add by hand or a
machine adds, but not for items delivered by another hopper. There is no
setting combination that makes a hopper chain work in vanilla.

## Setup

In each container after the first, place an **intake** hopper and link it to the
**outlet** hopper in the next container. Repeat:

```text
A: items + intake #1          B: outlet #1 + intake #2          C: outlet #2
                                (link intake #2 to outlet #2)
```

The `On Add` toggle on the hoppers in B, C, ... is not required for
hopper-to-hopper handoffs. The mod follows the chain when items arrive from
another hopper, so a hopper that only receives items that way keeps forwarding
them. Manual or machine additions still use the vanilla `On Add` toggle on the
receiving hopper. Filters, keep amounts, and colours still apply to each intake.
The order slider still controls the nightly hopper pass: during that pass a
handoff only continues into the same run-order window and never backwards.
During the day a handoff runs the receiving hopper immediately.

## Settings

The game's **Mods** screen shows:

- **Enabled** – turn the whole mod on or off.
- **Only chain hoppers with 'On Add' enabled** – off by default. Turn it on if
  you want a receiving hopper to forward only when its own `On Add` toggle is
  on.
- **Maximum chain depth** – safety limit, default 32. A loop is also stopped as
  soon as it reaches a hopper that already ran in the same transfer.
- **Log chain transfers** – writes each hop to `Player.log`.

## Build and install

```powershell
.\build.ps1 -GameDir "E:\SteamLibrary\steamapps\common\Probably Stolen" -Install
```

Then open the game's **Mods** screen, enable and trust **Hopper Chains**, and
restart the game. The loader applies the Harmony patch automatically.

## Compatibility

Built for game version `0.46D`. The mod patches the public
`HopperHelper.RunAndRecord` and `HopperHelper.RunWindow` methods, so it works
with the intake window's **Transfer now** button, the outlet window,
`On Add`/`On Move` scheduling, and the nightly hopper passes. Hoppers still do
not run while you are outside the store, matching vanilla behaviour.
