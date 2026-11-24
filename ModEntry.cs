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
            ModSettings.Initialize();
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
        ModHook.OnGameLoadedNormal += OnGameLoaded;
        Log.Log("HopperChains enabled.");
    }

    public void OnDisable()
    {
        Enabled = false;
        ModHook.OnGameLoadedNormal -= OnGameLoaded;
        Log.Log("HopperChains disabled.");
    }

    private static void OnGameLoaded()
    {
        if (!Enabled || !PlayerStore.HasInstance)
        {
            return;
        }

        try
        {
            int hoppers = 0;
            foreach (GameItem item in PlayerStore.Instance.FindAllItem())
            {
                if (HopperHelper.IsHopper(item) && GeneralHelper.IsItemOwned(item))
                {
                    hoppers++;
                }
            }

            Log.Log("Store loaded; " + hoppers + " hopper(s) found.");
        }
        catch (Exception ex)
        {
            WarnOnce("Could not inspect the store: " + ex.Message);
        }
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
