using System;

namespace HopperChains;

/// <summary>
/// Makes a chain of intake and outlet hoppers move items all the way to the
/// end instead of stopping in the first receiving container.
/// </summary>
public sealed class ModEntry : IMod
{
    internal const string ModId = "hopper_chains";

    internal static bool Enabled { get; private set; }

    internal static ModLog Log { get; private set; }

    private static string lastWarning;

    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            Settings.Initialize();
        }
        catch (Exception ex)
        {
            Log.Error("Could not initialize settings: " + ex);
        }
    }

    public void OnEnable()
    {
        if (Enabled)
        {
            return;
        }

        Enabled = true;
        Log.Log("Hopper Chains enabled.");
    }

    public void OnDisable()
    {
        if (!Enabled)
        {
            return;
        }

        Enabled = false;
        Log.Log("Hopper Chains disabled.");
    }

    internal static void WarnOnce(string message)
    {
        if (message == lastWarning)
        {
            return;
        }

        lastWarning = message;
        if (Log != null)
        {
            Log.Warning(message);
        }
    }
}
