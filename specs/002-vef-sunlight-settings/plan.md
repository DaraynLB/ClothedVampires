# Implementation Plan: Clothing Blocks Alpha Genes Sunlight Effects, With Settings

**Branch**: `002-vef-sunlight-settings` | **Date**: 2026-11-14 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/002-vef-sunlight-settings/spec.md`

## Summary

Extend Clothed Vampires so clothing also blocks Vanilla Expanded Framework's sunlight
effects used by Alpha Genes (vicious UV knockout, minor UV sight loss, lethal UV burns),
add a settings window (four protection switches, a Genes only / Absolute mode, debug
logging, reset), and fix the shipped bug where covered pawns lost sunlight *bonuses*
(only penalties are blocked now). The VEF effects are blocked by Harmony prefixes on
`HediffComp_StageBySunlight` and `HediffComp_HighlyFlammable`, applied by reflection only
when VEF is loaded. Every protection funnels through one decision: switch on, pawn
covered (`SunCover`), effect is a penalty, and (in Genes only mode) the effect comes from
one of the pawn's genes.

## Technical Context

**Language/Version**: C# (`LangVersion` latest), dual target `net472` (1.6) / `net10.0` (1.7)

**Primary Dependencies**: Lib.Harmony 2.3.* (compile-time; Harmony mod at runtime);
Krafs.Rimworld.Ref 1.6.* / 1.7.5005-unstable. **No compile reference to VEF or Alpha
Genes**: VEF types and fields are reached by name through `AccessTools` (research R2, R4).

**Storage**: mod settings only (`ModSettings` + `Scribe_Values`); nothing in saves.

**Testing**: in-game through RimSort, Dev Mode and Player.log, now with debug logging for
knockout and burns; screenshots for slowness, mood and the settings window.

**Target Platform**: RimWorld 1.7 (full testing) and 1.6 (smoke test), Biotech required;
VEF + Alpha Genes optional.

**Project Type**: RimWorld C# mod (single class library)

**Performance Goals**: VEF prefixes do no work except on their own check ticks (every 500
ticks per hediff), where they do one covered check and one gene lookup; the stat and
thought postfixes add only a dictionary/set lookup per call. No per-call allocations
(constitution, Target Platform → Performance).

**Constraints**: Principles I (soft dependency), III (penalties only), IV (settings), V
(no save data; only values VEF itself would set).

**Scale/Scope**: ~6 source files (3 new), 1 settings window, docs: settings guide (new),
Workshop description, README, update note.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / rule | Status | How |
|---|---|---|
| I. Soft compatibility | Pass | VEF types resolved with `AccessTools.TypeByName`; manual patches only if found; one `Log.Message` if absent; no compile reference. Verified load with and without VEF in the quickstart. |
| II. Verify before done | Pass (planned) | Quickstart: 1.7 full list (with VEF/Alpha Genes), 1.7 minimal (without), 1.6 smoke. |
| III. Narrow scope | Pass | Penalties-only rule (R3, FR-020) fixes the shipped bonus-blocking bug; one shared `SunCover` check; all switches off = vanilla. |
| IV. Player control | Pass | Four switches + mode + debug; defaults documented; `Scribe_Values`; live effect; grayed-out VEF switches with explanation. |
| V. Save safety | Pass | Only mod settings persisted; the prefix sets only `sunlessStageIndex`, the value VEF sets out of sunlight. |
| Platform: dual build, no bundled Harmony | Pass | Unchanged from spec 001. |
| Performance | Pass | See Technical Context; caches built once, lazily. |
| Workflow: cite other mods' internals | Pass | research.md R1–R4 cite decompiled names and def files. |
| Workflow: release docs incl. settings guide | Pass (planned) | Final phase writes `docs/settings-guide.md` (required now that settings exist). |

No violations; Complexity Tracking empty.

**Post-design re-check**: unchanged, all pass. The settings and log contract
([contracts/settings-and-log.md](contracts/settings-and-log.md)) fixes the labels, defaults
and log format the docs and tests rely on.

## Design

```text
ClothedVampiresMod (Mod)            loads ClothedVampiresSettings, draws the window
ClothedVampiresMain (static ctor)   PatchAll (vanilla patches) + VefCompat.TryPatch(harmony)
SunCover                            IsCovered(pawn)  (unchanged)
SunProtection                       ShouldBlock(kind, pawn, isPenalty, fromGene)
                                    = settings.On(kind) && isPenalty
                                      && (mode == Absolute || fromGene) && SunCover.IsCovered(pawn)
GeneSources                         lazy caches: gene stat affecters; GeneDef → granted HediffDefs;
                                    HediffFromActiveGene(pawn, hediffDef)
PenaltyRules                        IsSlowdown(affecter), IsNegative(thought stage),
                                    StageReducesCapacity(hediffDef, stage)  (cached per def)
HarmonyPatches                      stat postfix (kind Slowness) and thought postfix (kind Mood),
                                    now asking SunProtection
VefCompat                           finds VEF comp types/fields; manual prefixes:
                                    StageBySunlight → kind Conditions; HighlyFlammable → kind Burns
```

**StageBySunlight prefix** (`__instance`, `int delta`): if not a 500-tick check for the pawn
→ run original. Else if the pawn is not on a map or not protected → run original.
Else set `parent.Severity = sunlessStageIndex`, debug-log if the pawn is in sunlight, skip
original. Penalty test: the hediff def's stage at `sunlightStageIndex` reduces a capacity.

**HighlyFlammable prefix**: if `sunlightBurns` is false → run original (real-fire
behavior). If not a `tickInterval` check → run original. If protected → debug-log when in
sunlight, skip original (no damage). Else run original.

## Project Structure

### Documentation (this feature)

```text
specs/002-vef-sunlight-settings/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/settings-and-log.md
├── checklists/requirements.md
├── test-progress.md          # created during testing
└── tasks.md                  # /speckit-tasks
```

### Source Code (repository root)

```text
Source/
├── ClothedVampires.csproj        # unchanged
├── ClothedVampiresMain.cs        # + VefCompat.TryPatch
├── ClothedVampiresMod.cs         # new: Mod subclass, settings window
├── ClothedVampiresSettings.cs    # new: ModSettings, defaults, Reset
├── SunCover.cs                   # unchanged
├── SunProtection.cs              # new: decision, PenaltyRules, GeneSources
├── HarmonyPatches.cs             # stat/thought postfixes use SunProtection
└── VefCompat.cs                  # new: reflection + manual prefixes
docs/
├── settings-guide.md             # new
├── steam-workshop-description.txt
├── updates/<date>-alpha-genes-sunlight.md
README.md
```

**Structure Decision**: same single-project dual build as spec 001; new code split by
responsibility so the VEF reflection is isolated in one file.

## Complexity Tracking

No constitution violations to justify.
