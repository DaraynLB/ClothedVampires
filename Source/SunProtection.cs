using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ClothedVampires
{
    // The single decision every protection goes through (data-model.md):
    // switch on && the effect is a penalty && (Absolute || it comes from a gene) && pawn covered.
    public static class SunProtection
    {
        private static readonly ClothedVampiresSettings fallback = new ClothedVampiresSettings();

        public static ClothedVampiresSettings Settings => ClothedVampiresMod.Settings ?? fallback;

        public static bool IsOn(ProtectionKind kind) => Settings.IsOn(kind);

        public static bool ShouldBlock(ProtectionKind kind, Pawn pawn, bool isPenalty, bool fromGene)
        {
            ClothedVampiresSettings settings = Settings;
            if (!settings.IsOn(kind) || !isPenalty)
            {
                return false;
            }
            if (settings.mode == ProtectionMode.GenesOnly && !fromGene)
            {
                return false;
            }
            return SunCover.IsCovered(pawn);
        }

        public static void DebugLog(string message)
        {
            if (Settings.debugLogging)
            {
                Log.Message("[ClothedVampires DEBUG] " + message);
            }
        }
    }

    // Which sunlight effects are penalties (spec FR-020, research R3). Bonuses are never blocked.
    public static class PenaltyRules
    {
        private static readonly Dictionary<ConditionalStatAffecter, bool> slowdownCache =
            new Dictionary<ConditionalStatAffecter, bool>();
        private static readonly Dictionary<HediffDef, bool> conditionCache = new Dictionary<HediffDef, bool>();

        // An "in sunlight" stat effect is a penalty for the slowness switch when it lowers MoveSpeed.
        public static bool IsSlowdown(ConditionalStatAffecter affecter)
        {
            if (affecter == null)
            {
                return false;
            }
            if (!slowdownCache.TryGetValue(affecter, out bool result))
            {
                result = LowersMoveSpeed(affecter);
                slowdownCache[affecter] = result;
            }
            return result;
        }

        private static bool LowersMoveSpeed(ConditionalStatAffecter affecter)
        {
            if (affecter.statFactors != null)
            {
                foreach (StatModifier mod in affecter.statFactors)
                {
                    if (mod.stat == StatDefOf.MoveSpeed && mod.value < 1f)
                    {
                        return true;
                    }
                }
            }
            if (affecter.statOffsets != null)
            {
                foreach (StatModifier mod in affecter.statOffsets)
                {
                    if (mod.stat == StatDefOf.MoveSpeed && mod.value < 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsNegativeMood(ThoughtDef def, int stageIndex)
        {
            if (def?.stages == null || stageIndex < 0 || stageIndex >= def.stages.Count)
            {
                return false;
            }
            ThoughtStage stage = def.stages[stageIndex];
            return stage != null && stage.baseMoodEffect < 0f;
        }

        // A sunlight condition is a penalty when the stage it reaches in sunlight reduces a capacity.
        public static bool StageReducesCapacity(HediffDef def, float sunlightSeverity)
        {
            if (def == null)
            {
                return false;
            }
            if (!conditionCache.TryGetValue(def, out bool result))
            {
                result = ComputeStageReducesCapacity(def, sunlightSeverity);
                conditionCache[def] = result;
            }
            return result;
        }

        private static bool ComputeStageReducesCapacity(HediffDef def, float severity)
        {
            if (def.stages.NullOrEmpty())
            {
                return false;
            }
            int index = def.StageAtSeverity(severity);
            if (index < 0 || index >= def.stages.Count)
            {
                return false;
            }
            HediffStage stage = def.stages[index];
            if (stage?.capMods == null)
            {
                return false;
            }
            foreach (PawnCapacityModifier cap in stage.capMods)
            {
                if (cap.setMax < 1f || cap.offset < 0f || cap.postFactor < 1f)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // Whether a sunlight effect comes from one of the pawn's genes (spec FR-010, research R4).
    public static class GeneSources
    {
        private static HashSet<ConditionalStatAffecter> geneAffecters;
        private static readonly Dictionary<GeneDef, List<HediffDef>> grantedHediffs =
            new Dictionary<GeneDef, List<HediffDef>>();

        public static bool IsGeneAffecter(ConditionalStatAffecter affecter)
        {
            if (geneAffecters == null)
            {
                geneAffecters = new HashSet<ConditionalStatAffecter>();
                foreach (GeneDef gene in DefDatabase<GeneDef>.AllDefsListForReading)
                {
                    if (gene.conditionalStatAffecters != null)
                    {
                        foreach (ConditionalStatAffecter a in gene.conditionalStatAffecters)
                        {
                            geneAffecters.Add(a);
                        }
                    }
                }
            }
            return affecter != null && geneAffecters.Contains(affecter);
        }

        public static bool IsGeneThought(ThoughtDef def)
        {
            return def != null && !def.requiredGenes.NullOrEmpty();
        }

        public static bool HediffFromActiveGene(Pawn pawn, HediffDef hediffDef)
        {
            List<Gene> genes = pawn?.genes?.GenesListForReading;
            if (genes == null || hediffDef == null)
            {
                return false;
            }
            for (int i = 0; i < genes.Count; i++)
            {
                Gene gene = genes[i];
                if (gene == null || !gene.Active)
                {
                    continue;
                }
                if (!grantedHediffs.TryGetValue(gene.def, out List<HediffDef> granted))
                {
                    granted = VefCompat.HediffDefsGrantedBy(gene.def);
                    grantedHediffs[gene.def] = granted;
                }
                if (granted.Contains(hediffDef))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
