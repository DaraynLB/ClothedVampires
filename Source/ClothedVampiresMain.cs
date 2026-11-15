using HarmonyLib;
using Verse;

namespace ClothedVampires
{
    [StaticConstructorOnStartup]
    public static class ClothedVampiresMain
    {
        static ClothedVampiresMain()
        {
            var harmony = new Harmony("daraynlb.clothedvampires");
            harmony.PatchAll();
            Log.Message("[ClothedVampires] Mod loaded, Harmony patches applied.");
            Log.Message(VefCompat.TryPatch(harmony));
        }
    }
}
