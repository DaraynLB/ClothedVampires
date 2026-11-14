# Research: Clothing Blocks Alpha Genes Sunlight Effects, With Settings

All names below were confirmed on 2026-11-14 by decompiling the installed assemblies
(`ilspycmd`) and reading the installed defs, as the constitution requires before relying on
another mod's internals:

- RimWorld 1.7.5005 `Assembly-CSharp.dll`
- Vanilla Expanded Framework (Workshop 2023507013) `1.6/Assemblies/VEF.dll` and
  `1.7/Assemblies/VEF.dll`
- Alpha Genes (Workshop 2891845502) `1.6/` and `1.7/` defs
- Lib.Harmony 2.3.6

## R1. How Alpha Genes applies its sunlight effects

- **Decision**: treat Alpha Genes as data on top of two VEF hediff comps; patch the comps, not
  Alpha Genes.
- **Findings** (identical in Alpha Genes 1.6 and 1.7; `diff` of both folders is empty):

| Gene (defName) | Hediff | VEF comp | Sunlight effect |
|---|---|---|---|
| `AG_Sensitivity_Vicious` | `AG_Sensitivity_Vicious_Hediff` | `HediffCompProperties_StageBySunlight` (sunlight 1, sunless 0.1) | stage 2: Consciousness `setMax 0.1` (knockout) |
| `AG_UVSensitivity_Minor` | `AG_UVSensitivity_Minor_Hediff` | `HediffCompProperties_StageBySunlight` (sunlight 1, sunless 0.1) | stage 1+: Sight `offset -0.1` |
| `AG_Sensitivity_Lethal` | `AG_Sensitivity_Lethal_Hediff` | `HediffCompProperties_HighlyFlammable` (`sunlightBurns` true, `VEF_PermanentBurn` 15 every 500 ticks) | burns |

  Each gene attaches its hediff through `VEF.Genes.GeneExtension.hediffToWholeBody`
  (a `DefModExtension` on the `GeneDef`). `GeneExtension` also has
  `List<HediffToBodyparts> hediffsToBodyParts` (`HediffToBodyparts.hediff`).
- Alpha Genes also has a minor-UV mood thought `AG_SunlightSensitivity_Minor`
  (`ThoughtWorker_InSunlight`, `requiredGenes` = `AG_UVSensitivity_Minor`, -3), already
  covered by the existing mood patch.

## R2. VEF comp internals (patch targets)

- **Decision**: Harmony **prefixes** on both comps' `CompPostTickInterval`, applied manually
  only when the types are found (constitution Principle I).
- **Findings** (same code in VEF 1.6 and 1.7):
  - `VEF.AnimalBehaviours.HediffComp_StageBySunlight : HediffComp`,
    `public override void CompPostTickInterval(ref float severityAdjustment, int delta)`.
    Every 500 ticks (`Gen.IsHashIntervalTick(pawn, 500, delta)`) it sets
    `parent.Severity = Props.sunlightStageIndex` if `pawn.Map != null &&
    SanguophageUtility.InSunlight(pawn.Position, pawn.Map)`, else `Props.sunlessStageIndex`.
    It does nothing else (base `HediffComp.CompPostTickInterval` is empty).
  - `VEF.AnimalBehaviours.HediffComp_HighlyFlammable : HediffComp`, same signature. Every
    `Props.tickInterval` ticks: damages the pawn if `(pawn.IsBurning() && !sunlightBurns) ||
    (sunlightBurns && Map != null && InSunlight(...))`. With `sunlightBurns` true, real fire
    plays no part in this comp; real fire damage comes from the game's fire system.
  - Props classes `HediffCompProperties_StageBySunlight` (`public float sunlightStageIndex,
    sunlessStageIndex`) and `HediffCompProperties_HighlyFlammable` (`public bool
    sunlightBurns; public int tickInterval; DamageDef damageToInflict; float damageAmount`).
- **Why prefix, not postfix**: a postfix would let VEF set the sunlight stage first; the
  severity setter notifies the health system on a stage change, which can down the pawn
  before we set it back. A prefix that, for a protected pawn, does exactly what the original
  does out of sunlight (set `sunlessStageIndex`) and skips the original avoids that and
  satisfies Principle V (only values the original would set).
- **Why not a transpiler** (replace the `InSunlight` call): transpilers are untested on the
  Harmony 3.0 library that 1.7 loads (uplift skill note). Prefixes and manual patches are
  verified there.
- **Reading VEF fields without a compile reference**: `AccessTools.FieldRefAccess<F>(Type,
  string)` (Lib.Harmony 2.3.6, confirmed) gives cached `FieldRef<object, F>` delegates, so
  reading the props costs no allocation in the tick path.

## R3. Which sunlight effects are penalties (FR-020)

- **Decision** (user, option A):
  - Slowness: an `in sunlight` stat affecter is a penalty if it lowers `MoveSpeed`
    (a `statFactors` entry < 1 or a `statOffsets` entry < 0 for `MoveSpeed`).
  - Mood: the thought stage's `baseMoodEffect` < 0.
  - Conditions: the sunlight stage reduces a capacity (any `capMods` entry with
    `setMax < 1`, `offset < 0` or `postFactor < 1`).
  - Burns: always a penalty.
- **Findings** that forced it: Alpha Genes `AG_UVPowered_Mild/Major` (MoveSpeed ×1.2 in sun)
  and the Shambler gene (MeleeCooldownFactor 0.75, MeleeDamageFactor 1.25 in sun) use
  `ConditionalStatAffecter_InSunlight` as bonuses; the shipped mod blocks them. A plain
  "factor < 1" test fails on cooldown-style stats, so the rule keys on MoveSpeed only.
  Big and Small darkness genes' sunlight penalties checked include MoveSpeed offsets
  (-0.30, -0.50).
- **Field names confirmed**: `Verse.ConditionalStatAffecter.statFactors / statOffsets`
  (`List<StatModifier>`), `RimWorld.ThoughtStage.baseMoodEffect`,
  `Verse.PawnCapacityModifier.setMax` (default 999), `offset`, `postFactor` (default 1).

## R4. "Comes from a gene" (FR-010)

- **Decision**, per protection:
  - Slowness: the affecter instance is listed in some `GeneDef.conditionalStatAffecters`
    (the stat system only applies active genes' affecters, so membership is enough).
  - Mood: the thought's `ThoughtDef.requiredGenes` is non-empty.
  - Conditions and burns: one of the pawn's **active** genes has a `GeneExtension` whose
    `hediffToWholeBody` or `hediffsToBodyParts[*].hediff` is this hediff's def.
- Built once, lazily, after defs load: a set of gene affecters, and a map
  `GeneDef → HediffDefs it grants` (read through `FieldRefAccess`, no VEF reference).
- **Alternatives**: tracking which gene added each hediff at runtime (needs save data,
  violates Principle V); hediff `isBad` (Alpha Genes' hediffs are `isBad false`).

## R5. Settings and entry point

- **Decision**: add `ClothedVampiresMod : Mod` (settings) and `ClothedVampiresSettings :
  ModSettings`; keep the `[StaticConstructorOnStartup]` class for Harmony so defs and other
  mods' assemblies are loaded when VEF types are resolved. Settings load in the `Mod`
  constructor, before any patch runs.
- UI: `Listing_Standard` with `Widgets.CheckboxLabeled(Rect, string, ref bool, bool
  disabled, ...)` (confirmed signature) for the grayed-out switches, `RadioButton` for the
  mode, tooltips via `TooltipHandler`, and a Reset to defaults button.
- Labels are hard-coded English for now (no `Languages/` folder, so the deploy target is
  unchanged). Translation can come later.

## R6. Debug logging (FR-019)

- **Decision**: when on, one `[ClothedVampires DEBUG]` line per blocked check: the prefixes
  already run once per comp check (every 500 ticks), so "per pawn per check" holds per
  sunlight hediff. Logged only when the pawn is actually in sunlight at that check (when the
  block changed something). Off: nothing during play.

## R7. Detecting VEF for the gray-out (FR-012)

- **Decision**: `AccessTools.TypeByName("VEF.AnimalBehaviours.HediffComp_StageBySunlight")`
  and `…HediffComp_HighlyFlammable`; each switch is enabled when its comp type exists. One
  `Log.Message` at startup if absent (FR-015 allows one line).

## R8. Test fixtures

- Alpha Genes genes have `selectionWeight 0`, so test pawns get them with the developer
  "add gene" action, or by adding the hediff directly for the "not from a gene" case
  (Genes only vs Absolute). The plan's quickstart says to search Debug actions rather than
  guess menu paths.
- Debug logging makes knockout and burn checks log-provable (R6); slowness/mood stay
  screenshot-based as in spec 001.
