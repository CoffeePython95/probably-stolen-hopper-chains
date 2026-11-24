using System;
using HarmonyLib;

namespace HopperChains;

/// <summary>
/// Hooks the public transfer method used by the intake window, outlet window,
/// "On Add"/"On Move" scheduling, and the nightly hopper windows.
/// </summary>
[HarmonyPatch(typeof(HopperHelper), nameof(HopperHelper.RunAndRecord), new Type[] { typeof(GameItem) })]
internal static class HopperRunAndRecordPatch
{
    [HarmonyPostfix]
    private static void Postfix(GameItem __0, int __result)
    {
        if (__result <= 0)
        {
            return;
        }

        HopperChainRouter.CascadeFrom(__0);
    }
}
