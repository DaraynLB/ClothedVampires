using HarmonyLib;
using RimWorld;
using Verse;

namespace ClothedVampires
{
    // "In sunlight" stat effects (Biotech UV sensitivity, Big and Small darkness genes, ...).
    // Only effects that slow the pawn are blocked; sunlight bonuses are kept (spec FR-020).
    [HarmonyPatch(typeof(ConditionalStatAffecter_InSunlight), nameof(ConditionalStatAffecter_InSunlight.Applies))]
    public static class ConditionalStatAffecter_InSunlight_Applies_Patch
    {
        public static void Postfix(ConditionalStatAffecter_InSunlight __instance, StatRequest req, ref bool __result)
        {
            if (!__result || !(req.Thing is Pawn pawn) || !SunProtection.IsOn(ProtectionKind.Slowness))
            {
                return;
            }
            if (SunProtection.ShouldBlock(ProtectionKind.Slowness, pawn,
                    PenaltyRules.IsSlowdown(__instance), GeneSources.IsGeneAffecter(__instance)))
            {
                __result = false;
            }
        }
    }

    // "In sunlight" mood thoughts (Biotech UV sensitivity, Alpha Genes minor UV, ...).
    // Only negative thoughts are blocked (spec FR-020).
    [HarmonyPatch(typeof(ThoughtWorker_InSunlight), "CurrentStateInternal")]
    public static class ThoughtWorker_InSunlight_CurrentStateInternal_Patch
    {
        public static void Postfix(ThoughtWorker_InSunlight __instance, Pawn p, ref ThoughtState __result)
        {
            if (!__result.Active || !SunProtection.IsOn(ProtectionKind.Mood))
            {
                return;
            }
            ThoughtDef def = __instance.def;
            if (SunProtection.ShouldBlock(ProtectionKind.Mood, p,
                    PenaltyRules.IsNegativeMood(def, __result.StageIndex), GeneSources.IsGeneThought(def)))
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
