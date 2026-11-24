using System;
using HarmonyLib;

namespace HopperChains;

/// <summary>
/// Marks the nightly hopper pass so a chain handoff stays inside the current
/// run-order window instead of pulling a later/earlier hopper out of sequence.
/// </summary>
[HarmonyPatch(typeof(HopperHelper), nameof(HopperHelper.RunWindow), new Type[] { typeof(int) })]
internal static class HopperRunWindowPatch
{
    [HarmonyPrefix]
    private static void Prefix(int __0)
    {
        HopperChainRouter.EnterNightlyWindow(__0);
    }

    [HarmonyFinalizer]
    private static Exception Finalizer(Exception __exception)
    {
        HopperChainRouter.ExitNightlyWindow();
        return __exception;
    }
}
