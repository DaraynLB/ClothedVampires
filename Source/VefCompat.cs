using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ClothedVampires
{
    // Optional support for Vanilla Expanded Framework's sunlight hediff comps (used by Alpha Genes).
    // VEF is a soft dependency (constitution, Principle I): everything is found by name, and a
    // missing type only disables the part that needs it. Names confirmed in
    // specs/002-vef-sunlight-settings/research.md (R1, R2).
    public static class VefCompat
    {
        private const string StageCompName = "VEF.AnimalBehaviours.HediffComp_StageBySunlight";
        private const string StagePropsName = "VEF.AnimalBehaviours.HediffCompProperties_StageBySunlight";
        private const string BurnCompName = "VEF.AnimalBehaviours.HediffComp_HighlyFlammable";
        private const string BurnPropsName = "VEF.AnimalBehaviours.HediffCompProperties_HighlyFlammable";
        private const string GeneExtensionName = "VEF.Genes.GeneExtension";
        private const string HediffToBodypartsName = "VEF.Genes.HediffToBodyparts";

        public static readonly Type StageCompType;
        public static readonly Type BurnCompType;
        private static readonly Type geneExtensionType;

        // Cached field accessors: no allocation when read in tick paths.
        private static readonly AccessTools.FieldRef<object, float> sunlightStageIndex;
        private static readonly AccessTools.FieldRef<object, float> sunlessStageIndex;
        private static readonly AccessTools.FieldRef<object, bool> sunlightBurns;
        private static readonly AccessTools.FieldRef<object, int> burnTickInterval;
        private static readonly AccessTools.FieldRef<object, HediffDef> hediffToWholeBody;
        private static readonly AccessTools.FieldRef<object, IList> hediffsToBodyParts;
        private static readonly AccessTools.FieldRef<object, HediffDef> bodyPartsHediff;

        public static bool ConditionsAvailable { get; private set; }
        public static bool BurnsAvailable { get; private set; }

        static VefCompat()
        {
            StageCompType = AccessTools.TypeByName(StageCompName);
            BurnCompType = AccessTools.TypeByName(BurnCompName);
            geneExtensionType = AccessTools.TypeByName(GeneExtensionName);

            Type stageProps = AccessTools.TypeByName(StagePropsName);
            Type burnProps = AccessTools.TypeByName(BurnPropsName);
            Type bodyPartsType = AccessTools.TypeByName(HediffToBodypartsName);

            sunlightStageIndex = FieldRefOrNull<float>(stageProps, "sunlightStageIndex");
            sunlessStageIndex = FieldRefOrNull<float>(stageProps, "sunlessStageIndex");
            sunlightBurns = FieldRefOrNull<bool>(burnProps, "sunlightBurns");
            burnTickInterval = FieldRefOrNull<int>(burnProps, "tickInterval");
            hediffToWholeBody = FieldRefOrNull<HediffDef>(geneExtensionType, "hediffToWholeBody");
            hediffsToBodyParts = FieldRefOrNull<IList>(geneExtensionType, "hediffsToBodyParts");
            bodyPartsHediff = FieldRefOrNull<HediffDef>(bodyPartsType, "hediff");

            ConditionsAvailable = StageCompType != null && sunlightStageIndex != null && sunlessStageIndex != null;
            BurnsAvailable = BurnCompType != null && sunlightBurns != null && burnTickInterval != null;
        }

        private static AccessTools.FieldRef<object, F> FieldRefOrNull<F>(Type type, string fieldName)
        {
            if (type == null || AccessTools.Field(type, fieldName) == null)
            {
                return null;
            }
            try
            {
                return AccessTools.FieldRefAccess<F>(type, fieldName);
            }
            catch (Exception e)
            {
                Log.Warning($"[ClothedVampires] Could not access {type.FullName}.{fieldName}: {e.Message}");
                return null;
            }
        }

        // The hediffs a gene grants through VEF's GeneExtension (hediffToWholeBody and
        // hediffsToBodyParts). Empty when VEF isn't loaded or the gene has no extension.
        public static List<HediffDef> HediffDefsGrantedBy(GeneDef geneDef)
        {
            var result = new List<HediffDef>();
            if (geneExtensionType == null || geneDef?.modExtensions == null)
            {
                return result;
            }
            foreach (DefModExtension ext in geneDef.modExtensions)
            {
                if (ext == null || !geneExtensionType.IsInstanceOfType(ext))
                {
                    continue;
                }
                if (hediffToWholeBody != null)
                {
                    HediffDef whole = hediffToWholeBody(ext);
                    if (whole != null)
                    {
                        result.Add(whole);
                    }
                }
                if (hediffsToBodyParts != null && bodyPartsHediff != null)
                {
                    IList parts = hediffsToBodyParts(ext);
                    if (parts != null)
                    {
                        foreach (object part in parts)
                        {
                            HediffDef partHediff = part != null ? bodyPartsHediff(part) : null;
                            if (partHediff != null)
                            {
                                result.Add(partHediff);
                            }
                        }
                    }
                }
            }
            return result;
        }

        // Applies the manual prefixes on VEF's comps when they exist. Returns the startup line.
        public static string TryPatch(Harmony harmony)
        {
            if (ConditionsAvailable)
            {
                ConditionsAvailable = PatchPrefix(harmony, StageCompType, nameof(StageBySunlightPrefix));
            }
            if (BurnsAvailable)
            {
                BurnsAvailable = PatchPrefix(harmony, BurnCompType, nameof(HighlyFlammablePrefix));
            }
            if (!ConditionsAvailable && !BurnsAvailable)
            {
                return "[ClothedVampires] Vanilla Expanded Framework not found; knockout and burn protection unavailable.";
            }
            return "[ClothedVampires] Vanilla Expanded Framework sunlight effects found; knockout and burn protection active.";
        }

        private static bool PatchPrefix(Harmony harmony, Type compType, string prefixName)
        {
            try
            {
                var target = AccessTools.Method(compType, "CompPostTickInterval");
                if (target == null)
                {
                    Log.Warning($"[ClothedVampires] {compType.FullName}.CompPostTickInterval not found; that protection is unavailable.");
                    return false;
                }
                harmony.Patch(target, prefix: new HarmonyMethod(typeof(VefCompat), prefixName));
                return true;
            }
            catch (Exception e)
            {
                Log.Warning($"[ClothedVampires] Could not patch {compType.FullName}: {e.Message}");
                return false;
            }
        }

        // VEF's HediffComp_StageBySunlight sets the sunlight stage every 500 ticks when the pawn is in
        // sunlight. For a protected pawn, do exactly what it does out of sunlight (set the sunless
        // stage) and skip it (research R2; constitution Principle V).
        public static bool StageBySunlightPrefix(HediffComp __instance, int delta)
        {
            HediffWithComps parent = __instance.parent;
            Pawn pawn = parent?.pawn;
            if (pawn == null || pawn.Map == null || !Gen.IsHashIntervalTick(pawn, 500, delta)
                || !SunProtection.IsOn(ProtectionKind.Conditions))
            {
                return true;
            }
            object props = __instance.props;
            float sunlightSeverity = sunlightStageIndex(props);
            bool isPenalty = PenaltyRules.StageReducesCapacity(parent.def, sunlightSeverity);
            bool fromGene = GeneSources.HediffFromActiveGene(pawn, parent.def);
            if (!SunProtection.ShouldBlock(ProtectionKind.Conditions, pawn, isPenalty, fromGene))
            {
                return true;
            }
            parent.Severity = sunlessStageIndex(props);
            if (SunProtection.Settings.debugLogging && pawn.Position.InSunlight(pawn.Map))
            {
                SunProtection.DebugLog($"Blocked sunlight condition '{parent.def.label}' on {pawn.LabelShort} (from gene: {(fromGene ? "yes" : "no")})");
            }
            return false;
        }

        // VEF's HediffComp_HighlyFlammable damages the pawn every tickInterval ticks; with
        // sunlightBurns it does so in sunlight. For a protected pawn, skip that check. Comps without
        // sunlightBurns (real-fire behavior) are never touched.
        public static bool HighlyFlammablePrefix(HediffComp __instance, int delta)
        {
            object props = __instance.props;
            if (!sunlightBurns(props))
            {
                return true;
            }
            HediffWithComps parent = __instance.parent;
            Pawn pawn = parent?.pawn;
            int interval = burnTickInterval(props);
            if (pawn == null || pawn.Map == null || interval <= 0 || !Gen.IsHashIntervalTick(pawn, interval, delta)
                || !SunProtection.IsOn(ProtectionKind.Burns))
            {
                return true;
            }
            bool fromGene = GeneSources.HediffFromActiveGene(pawn, parent.def);
            if (!SunProtection.ShouldBlock(ProtectionKind.Burns, pawn, true, fromGene))
            {
                return true;
            }
            if (SunProtection.Settings.debugLogging && pawn.Position.InSunlight(pawn.Map))
            {
                SunProtection.DebugLog($"Blocked sunlight burn '{parent.def.label}' on {pawn.LabelShort} (from gene: {(fromGene ? "yes" : "no")})");
            }
            return false;
        }
    }
}
