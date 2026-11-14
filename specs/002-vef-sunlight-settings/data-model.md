# Data Model: Clothing Blocks Alpha Genes Sunlight Effects, With Settings

## ClothedVampiresSettings (persisted, mod settings file only)

| Field | Type | Default | Scribe key | Notes |
|---|---|---|---|---|
| `blockSlowness` | bool | true | `blockSlowness` | in-sunlight stat effects that lower movement speed |
| `blockMood` | bool | true | `blockMood` | in-sunlight thoughts with negative mood |
| `blockConditions` | bool | true | `blockConditions` | "Knockout and other sunlight conditions" (VEF StageBySunlight) |
| `blockBurns` | bool | true | `blockBurns` | VEF sunlight burns |
| `mode` | enum `ProtectionMode { GenesOnly, Absolute }` | `GenesOnly` | `mode` | applies to all four |
| `debugLogging` | bool | false | `debugLogging` | FR-019 |

Rules: every field has a `Scribe_Values.Look` with its default; `Reset()` restores all
defaults; values are read live by the patches (no caching of settings).

## ProtectionKind (enum, not persisted)

`Slowness`, `Mood`, `Conditions`, `Burns`. Maps one-to-one to the four switches.

## Protection decision (computed, not stored)

```text
ShouldBlock(kind, pawn, isPenalty, fromGene) =
    switchOn(kind)
    && isPenalty
    && (mode == Absolute || fromGene)
    && SunCover.IsCovered(pawn)
```

| Kind | isPenalty | fromGene |
|---|---|---|
| Slowness | affecter lowers MoveSpeed (factor < 1 or offset < 0) | affecter listed in a `GeneDef.conditionalStatAffecters` |
| Mood | current stage `baseMoodEffect < 0` | `ThoughtDef.requiredGenes` non-empty |
| Conditions | stage at `sunlightStageIndex` has a capMod with `setMax < 1`, `offset < 0` or `postFactor < 1` | an active gene's `GeneExtension` grants this hediff def |
| Burns | always | an active gene's `GeneExtension` grants this hediff def |

## Caches (built once, lazily, after defs load; not persisted)

| Cache | Key → value | Source |
|---|---|---|
| Gene affecters | set of `ConditionalStatAffecter` | all `GeneDef.conditionalStatAffecters` |
| Slowdown per affecter | `ConditionalStatAffecter` → bool | R3 rule |
| Gene-granted hediffs | `GeneDef` → set of `HediffDef` | VEF `GeneExtension.hediffToWholeBody`, `hediffsToBodyParts[*].hediff` |
| Conditions penalty per hediff | `HediffDef` → bool | stage at `sunlightStageIndex` |

## State transitions (per sunlight hediff, per check)

```text
in sunlight, protected      → severity = sunlessStageIndex   (as if out of sun)   [debug log]
in sunlight, not protected  → severity = sunlightStageIndex  (VEF original)
out of sunlight             → severity = sunlessStageIndex   (VEF original)
burns: in sunlight, protected → no damage [debug log]; otherwise VEF original
```

Switching a setting, the mode, or the pawn's clothing changes the outcome at the next check.
