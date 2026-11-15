using UnityEngine;
using Verse;

namespace ClothedVampires
{
    // Settings window. Labels, defaults and tooltips follow
    // specs/002-vef-sunlight-settings/contracts/settings-and-log.md.
    public class ClothedVampiresMod : Mod
    {
        public static ClothedVampiresSettings Settings { get; private set; }

        private const string VefNote = "Needs Vanilla Expanded Framework (used by Alpha Genes).";

        public ClothedVampiresMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ClothedVampiresSettings>();
        }

        public override string SettingsCategory() => "Clothed Vampires";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ClothedVampiresSettings s = Settings;
            var list = new Listing_Standard();
            list.Begin(inRect);

            list.Label("What clothing protects against");
            list.Gap(4f);
            Checkbox(list, "Slowness in sunlight", ref s.blockSlowness, false,
                "Covered pawns aren't slowed down by the sun.\n\nSunlight speed bonuses (such as Alpha Genes' UV powered) are never removed.");
            Checkbox(list, "Bad mood in sunlight", ref s.blockMood, false,
                "Covered pawns don't get negative \"in sunlight\" mood thoughts, such as sunlight sensitivity.\n\nGood sunlight thoughts are never removed.");
            Checkbox(list, "Knockout and other sunlight conditions", ref s.blockConditions, !VefCompat.ConditionsAvailable,
                "Covered pawns aren't knocked out (Alpha Genes' vicious UV sensitivity) or half-blinded (minor UV sensitivity) by the sun.\n\nNeeds Vanilla Expanded Framework.");
            Checkbox(list, "Burns from sunlight", ref s.blockBurns, !VefCompat.BurnsAvailable,
                "Covered pawns don't take sunlight burns (Alpha Genes' lethal UV sensitivity). Real fire still burns, and burns already taken stay.\n\nNeeds Vanilla Expanded Framework.");

            list.Gap();
            list.Label("Which sunlight effects count");
            list.Gap(4f);
            if (list.RadioButton("Genes only", s.mode == ProtectionMode.GenesOnly, 0f,
                "Clothing blocks only sunlight penalties caused by the pawn's genes."))
            {
                s.mode = ProtectionMode.GenesOnly;
            }
            if (list.RadioButton("Absolute", s.mode == ProtectionMode.Absolute, 0f,
                "Clothing blocks every sunlight penalty of the kinds switched on above, whatever caused it (a gene, an implant, a drug or another mod)."))
            {
                s.mode = ProtectionMode.Absolute;
            }

            list.Gap();
            Checkbox(list, "Debug logging", ref s.debugLogging, false,
                "Writes a line to the log each time clothing blocks a knockout, another sunlight condition, or a sunlight burn. Leave off for normal play.");

            list.Gap();
            Rect resetRect = list.GetRect(30f, 0.3f);
            if (Widgets.ButtonText(resetRect, "Reset to defaults"))
            {
                s.Reset();
            }
            TooltipHandler.TipRegion(resetRect, "Restores every setting to its default.");

            list.Gap();
            GUI.color = Color.gray;
            list.Label("Covered means clothing over both the torso and the legs. Underwear doesn't count. Changes apply immediately.");
            GUI.color = Color.white;

            list.End();
        }

        // A checkbox row that can be grayed out, with a tooltip and an optional note underneath.
        private static void Checkbox(Listing_Standard list, string label, ref bool value, bool disabled, string tooltip)
        {
            Rect row = list.GetRect(Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(row);
            Widgets.CheckboxLabeled(row, label, ref value, disabled);
            TooltipHandler.TipRegion(row, tooltip);
            if (disabled)
            {
                GUI.color = Color.gray;
                Text.Font = GameFont.Tiny;
                Rect note = list.GetRect(Text.LineHeight);
                Widgets.Label(new Rect(note.x + 12f, note.y, note.width - 12f, note.height), VefNote);
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
            }
            list.Gap(2f);
        }
    }
}
