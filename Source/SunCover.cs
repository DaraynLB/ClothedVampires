using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ClothedVampires
{
    public static class SunCover
    {
        // Layers used by underwear mods (UnderWhere, Undergarments Core).
        // Apparel that sits only on these layers doesn't count as clothing.
        private static readonly string[] UnderwearLayerNames = { "Underwear", "UnderwearTop", "Undergarments" };

        private static HashSet<ApparelLayerDef> underwearLayers;

        private static HashSet<ApparelLayerDef> UnderwearLayers
        {
            get
            {
                if (underwearLayers == null)
                {
                    underwearLayers = new HashSet<ApparelLayerDef>();
                    foreach (string name in UnderwearLayerNames)
                    {
                        ApparelLayerDef layer = DefDatabase<ApparelLayerDef>.GetNamedSilentFail(name);
                        if (layer != null)
                        {
                            underwearLayers.Add(layer);
                        }
                    }
                }
                return underwearLayers;
            }
        }

        // True when both the torso and the legs are covered by non-underwear apparel.
        public static bool IsCovered(Pawn pawn)
        {
            if (pawn?.apparel == null)
            {
                return false;
            }
            bool torso = false;
            bool legs = false;
            List<Apparel> worn = pawn.apparel.WornApparel;
            for (int i = 0; i < worn.Count; i++)
            {
                ApparelProperties props = worn[i].def.apparel;
                if (props == null || IsUnderwear(props))
                {
                    continue;
                }
                if (props.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
                {
                    torso = true;
                }
                if (props.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                {
                    legs = true;
                }
                if (torso && legs)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsUnderwear(ApparelProperties props)
        {
            if (UnderwearLayers.Count == 0 || props.layers.NullOrEmpty())
            {
                return false;
            }
            for (int i = 0; i < props.layers.Count; i++)
            {
                if (!UnderwearLayers.Contains(props.layers[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
