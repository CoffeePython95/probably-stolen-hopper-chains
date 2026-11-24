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
    internal static void Postfix(GameItem __0, int __result)
    {
        if (__result <= 0)
        {
            return;
        }

        HopperChainRouter.CascadeFrom(__0);
    }
}

/// <summary>
/// Marks the nightly hopper pass so a chain handoff stays inside the current
/// run-order window instead of pulling a later/earlier hopper out of sequence.
/// </summary>
[HarmonyPatch(typeof(HopperHelper), nameof(HopperHelper.RunWindow), new Type[] { typeof(int) })]
internal static class HopperRunWindowPatch
{
    [HarmonyPrefix]
    internal static void Prefix(int __0)
    {
        HopperChainRouter.EnterNightlyWindow(__0);
    }

    [HarmonyFinalizer]
    internal static Exception Finalizer(Exception __exception)
    {
        HopperChainRouter.ExitNightlyWindow();
        return __exception;
    }
}
