using System;
using System.Collections.Generic;
using System.Linq;

namespace HopperChains;

/// <summary>
/// After an intake hopper moves items into a container, any linked intake
/// hopper in that container is run too, so the items continue down the chain.
/// </summary>
internal static class HopperChainRouter
{
    private static bool cascading;
    private static int nightlyWindow;

    internal static void EnterNightlyWindow(int window)
    {
        nightlyWindow = window;
    }

    internal static void ExitNightlyWindow()
    {
        nightlyWindow = 0;
    }

    internal static void CascadeFrom(GameItem intake)
    {
        if (!ModEntry.Enabled || !Settings.Enabled || cascading)
        {
            return;
        }

        if (intake == null || !HopperHelper.IsIntake(intake) || !GeneralHelper.IsItemOwned(intake))
        {
            return;
        }

        cascading = true;
        try
        {
            RunChain(intake);
        }
        catch (Exception ex)
        {
            ModEntry.WarnOnce("Hopper chain stopped: " + ex.Message);
        }
        finally
        {
            cascading = false;
        }
    }

    private static void RunChain(GameItem intake)
    {
        int maxDepth = Settings.MaxChainLength;
        Queue<(GameItem item, int depth)> queue = new Queue<(GameItem, int)>();
        HashSet<GameItem> visited = new HashSet<GameItem>();
        queue.Enqueue((intake, 0));
        visited.Add(intake);

        while (queue.Count > 0)
        {
            (GameItem current, int depth) = queue.Dequeue();
            if (depth >= maxDepth)
            {
                continue;
            }

            GameInventory destination = HopperHelper.FindOutlet(current)?.parentInventory;
            if (destination == null || destination == current.parentInventory)
            {
                continue;
            }

            foreach (GameItem next in FindChainIntakes(destination))
            {
                if (!visited.Add(next))
                {
                    continue;
                }

                if (Settings.RequireOnAdd && !HopperHelper.IsOnAdd(next))
                {
                    continue;
                }

                if (nightlyWindow > 0 && !FollowsNightlyOrder(current, next))
                {
                    continue;
                }

                int moved;
                try
                {
                    moved = HopperHelper.RunAndRecord(next);
                }
                catch (Exception ex)
                {
                    ModEntry.WarnOnce("Could not run chained hopper " + Label(next) + ": " + ex.Message);
                    continue;
                }

                if (moved <= 0)
                {
                    continue;
                }

                int nextDepth = depth + 1;
                queue.Enqueue((next, nextDepth));

                if (Settings.LogTransfers)
                {
                    ModEntry.Log?.Log("Chain hop " + nextDepth + ": " + Label(next) + " moved " + moved + " unit(s).");
                }
            }
        }
    }

    private static bool FollowsNightlyOrder(GameItem current, GameItem next)
    {
        int currentOrder = HopperHelper.GetOrder(current);
        int nextOrder = HopperHelper.GetOrder(next);
        return HopperHelper.WindowOf(nextOrder) == nightlyWindow && nextOrder >= currentOrder;
    }

    private static List<GameItem> FindChainIntakes(GameInventory destination)
    {
        return destination.childItems
            .Where(item => item != null
                && HopperHelper.IsIntake(item)
                && GeneralHelper.IsItemOwned(item))
            .OrderBy(HopperHelper.GetOrder)
            .ThenBy(item => item.uniqueId)
            .ToList();
    }

    private static string Label(GameItem item)
    {
        return item == null ? "?" : item.GetDisplayName(preferCustomName: true);
    }
}
