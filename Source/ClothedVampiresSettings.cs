using Verse;

namespace ClothedVampires
{
    public enum ProtectionMode
    {
        GenesOnly,
        Absolute
    }

    public enum ProtectionKind
    {
        Slowness,
        Mood,
        Conditions,
        Burns
    }

    // Player settings. Defaults are documented in docs/settings-guide.md and
    // specs/002-vef-sunlight-settings/contracts/settings-and-log.md.
    public class ClothedVampiresSettings : ModSettings
    {
        public const bool DefaultBlockSlowness = true;
        public const bool DefaultBlockMood = true;
        public const bool DefaultBlockConditions = true;
        public const bool DefaultBlockBurns = true;
        public const ProtectionMode DefaultMode = ProtectionMode.GenesOnly;
        public const bool DefaultDebugLogging = false;

        public bool blockSlowness = DefaultBlockSlowness;
        public bool blockMood = DefaultBlockMood;
        public bool blockConditions = DefaultBlockConditions;
        public bool blockBurns = DefaultBlockBurns;
        public ProtectionMode mode = DefaultMode;
        public bool debugLogging = DefaultDebugLogging;

        public bool IsOn(ProtectionKind kind)
        {
            switch (kind)
            {
                case ProtectionKind.Slowness: return blockSlowness;
                case ProtectionKind.Mood: return blockMood;
                case ProtectionKind.Conditions: return blockConditions;
                case ProtectionKind.Burns: return blockBurns;
                default: return false;
            }
        }

        public void Reset()
        {
            blockSlowness = DefaultBlockSlowness;
            blockMood = DefaultBlockMood;
            blockConditions = DefaultBlockConditions;
            blockBurns = DefaultBlockBurns;
            mode = DefaultMode;
            debugLogging = DefaultDebugLogging;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref blockSlowness, "blockSlowness", DefaultBlockSlowness);
            Scribe_Values.Look(ref blockMood, "blockMood", DefaultBlockMood);
            Scribe_Values.Look(ref blockConditions, "blockConditions", DefaultBlockConditions);
            Scribe_Values.Look(ref blockBurns, "blockBurns", DefaultBlockBurns);
            Scribe_Values.Look(ref mode, "mode", DefaultMode);
            Scribe_Values.Look(ref debugLogging, "debugLogging", DefaultDebugLogging);
        }
    }
}
