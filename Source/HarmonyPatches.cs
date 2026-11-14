using HarmonyLib;
using RimWorld;
using Verse;

namespace ClothedVampires
{
    // Stat penalties from sunlight genes (UV sensitivity, Big and Small darkness genes).
    [HarmonyPatch(typeof(ConditionalStatAffecter_InSunlight), nameof(ConditionalStatAffecter_InSunlight.Applies))]
    public static class ConditionalStatAffecter_InSunlight_Applies_Patch
    {
        public static void Postfix(StatRequest req, ref bool __result)
        {
            if (__result && req.Thing is Pawn pawn && SunCover.IsCovered(pawn))
            {
                __result = false;
            }
        }
    }

    // Mood penalty from the UV sensitivity genes.
    [HarmonyPatch(typeof(ThoughtWorker_InSunlight), "CurrentStateInternal")]
    public static class ThoughtWorker_InSunlight_CurrentStateInternal_Patch
    {
        public static void Postfix(Pawn p, ref ThoughtState __result)
        {
            if (__result.Active && SunCover.IsCovered(p))
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
