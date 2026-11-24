using System;

namespace HopperChains;

/// <summary>
/// Settings shown in the game's Mods screen.
/// </summary>
internal static class ModSettings
{
    private const string GeneralSection = "General";
    private const string ChainSection = "Chaining";
    private const string DiagnosticSection = "Diagnostics";

    private static ConfigEntry<bool> enabled;
    private static ConfigEntry<bool> requireOnAdd;
    private static ConfigEntry<int> maxChainLength;
    private static ConfigEntry<bool> logTransfers;

    internal static void Initialize()
    {
        ModConfig config = ModHelper.GetConfig(ModEntry.ModId);

        enabled = config.Define(
            "enabled",
            true,
            GeneralSection,
            () => "Enabled",
            () => "Forward items through chains of linked hoppers.");

        requireOnAdd = config.Define(
            "require_on_add",
            false,
            ChainSection,
            () => "Only chain hoppers with 'On Add' enabled",
            () => "On: a receiving hopper continues the chain only when its 'On Add' toggle is on. Off: every linked intake hopper in the receiving container continues the chain. Off is what makes chains work without extra setup.");

        maxChainLength = config.Define(
            "max_chain_length",
            32,
            ChainSection,
            () => "Maximum chain depth",
            1,
            128,
            () => "Safety limit so a loop of hoppers cannot run forever. A loop is also stopped when it reaches a hopper that already ran in the same transfer.");

        logTransfers = config.Define(
            "log_transfers",
            false,
            DiagnosticSection,
            () => "Log chain transfers",
            () => "Write each hopper-to-hopper handoff to Player.log. Useful when setting up a long chain.");
    }

    internal static bool Enabled
    {
        get { return enabled == null || enabled.Value; }
    }

    internal static bool RequireOnAdd
    {
        get { return requireOnAdd != null && requireOnAdd.Value; }
    }

    internal static int MaxChainLength
    {
        get { return maxChainLength == null ? 32 : Math.Max(1, maxChainLength.Value); }
    }

    internal static bool LogTransfers
    {
        get { return logTransfers != null && logTransfers.Value; }
    }
}
