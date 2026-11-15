---

description: "Task list for 002-vef-sunlight-settings"
---

# Tasks: Clothing Blocks Alpha Genes Sunlight Effects, With Settings

**Input**: Design documents from `specs/002-vef-sunlight-settings/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/settings-and-log.md, quickstart.md

**Tests**: no automated tests. In-game verification is mandatory (constitution, Principle
II). Story phases end with verification tasks from [quickstart.md](quickstart.md) (V1–V10).
Record every result in `specs/002-vef-sunlight-settings/test-progress.md` (date, version,
mod list, evidence). Follow spec 001's decisions: gameplay on the user's full mod list,
load-only minimal check, 1.6 smoke test.

**Organization**: US1 vicious knockout (P1), US2 lethal burns + minor sight (P1), US3
switches + bonus fix (P2), US4 Genes only / Absolute (P2), US5 without VEF (P3).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: can run in parallel (different files, no dependencies on incomplete tasks)
- Paths are relative to the repo root `C:\RimWorldDev\ClothedVampires\`
- Names of VEF/Alpha Genes/game members are confirmed in `research.md` (R1–R5); use them
  exactly, and add Harmony patch parameters only by their real names (`__instance`,
  `delta`, `req`, `p`, `__result`)

---

## Phase 1: Setup

- [X] T001 Commit the spec 002 planning files (`specs/002-vef-sunlight-settings/` spec, plan, research, data-model, contracts, quickstart, checklists, tasks) on branch `002-vef-sunlight-settings` so code changes are separate commits — done 2026-11-14 (`5b22a1a`)
- [X] T002 Confirm Steam is back on the RimWorld 1.7 beta branch (`C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Version.txt` shows 1.7.x) and that `Krafs.Rimworld.Ref` 1.7 in `Source/ClothedVampires.csproj` still matches it; note any change in `specs/002-vef-sunlight-settings/research.md` — done: game 1.7.5005, ref 1.7.5005-unstable unchanged

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: settings, the shared protection decision, VEF discovery, and the rewired
vanilla patches. Every story depends on these.

- [X] T003 [P] Create `Source/ClothedVampiresSettings.cs`: `ClothedVampiresSettings : ModSettings` with `blockSlowness`, `blockMood`, `blockConditions`, `blockBurns` (all `true`), `mode` (`ProtectionMode.GenesOnly`), `debugLogging` (`false`); `ExposeData()` with `Scribe_Values.Look` for each field and its default; `Reset()`; enums `ProtectionMode { GenesOnly, Absolute }` and `ProtectionKind { Slowness, Mood, Conditions, Burns }`; `bool IsOn(ProtectionKind)` (data-model.md) — done
- [X] T004 [P] Create `Source/VefCompat.cs` (discovery only in this task): resolve by name with `AccessTools.TypeByName` the types `VEF.AnimalBehaviours.HediffComp_StageBySunlight`, `VEF.AnimalBehaviours.HediffCompProperties_StageBySunlight`, `VEF.AnimalBehaviours.HediffComp_HighlyFlammable`, `VEF.AnimalBehaviours.HediffCompProperties_HighlyFlammable`, `VEF.Genes.GeneExtension`, `VEF.Genes.HediffToBodyparts`; cached `AccessTools.FieldRefAccess<F>(Type, string)` refs for `sunlightStageIndex`, `sunlessStageIndex`, `sunlightBurns`, `tickInterval`, `hediffToWholeBody`, `hediffsToBodyParts`, `hediff`; public `ConditionsAvailable` / `BurnsAvailable`; `HediffDefsGrantedBy(GeneDef)` reading the `GeneExtension` in `geneDef.modExtensions`; every lookup null-safe so a missing type disables only that part (constitution Principle I) — done
- [X] T005 Create `Source/SunProtection.cs`: `ShouldBlock(ProtectionKind kind, Pawn pawn, bool isPenalty, bool fromGene)` = switch on && isPenalty && (mode Absolute || fromGene) && `SunCover.IsCovered(pawn)` (check the cheap terms first); `PenaltyRules` with cached `IsSlowdown(ConditionalStatAffecter)` (a `MoveSpeed` entry in `statFactors` < 1 or `statOffsets` < 0), `IsNegativeMood(ThoughtDef, int stageIndex)` (`baseMoodEffect` < 0), `StageReducesCapacity(HediffDef, float severity)` (stage for that severity has a `capMods` entry with `setMax < 1`, `offset < 0` or `postFactor < 1`); `GeneSources` with lazily built set of all `GeneDef.conditionalStatAffecters`, `IsGeneAffecter(affecter)`, `IsGeneThought(ThoughtDef)` (`requiredGenes` non-empty), and `HediffFromActiveGene(Pawn, HediffDef)` (any active gene whose def grants it, via `VefCompat.HediffDefsGrantedBy`, cached per `GeneDef`); `DebugLog(string)` writing `[ClothedVampires DEBUG] …` only when `debugLogging` is on; no per-call allocations (depends on T003, T004) — done
- [X] T006 Create `Source/ClothedVampiresMod.cs`: `ClothedVampiresMod : Mod` that loads `ClothedVampiresSettings` via `GetSettings<>()` into a static `Settings` property in its constructor, `SettingsCategory()` = "Clothed Vampires", and `DoSettingsWindowContents(Rect)` drawing the window exactly as in `specs/002-vef-sunlight-settings/contracts/settings-and-log.md`: section headers, four switches with tooltips (`Widgets.CheckboxLabeled(rect, label, ref value, disabled)`; the two VEF switches use `disabled: !VefCompat.ConditionsAvailable` / `!VefCompat.BurnsAvailable` and show the note "Needs Vanilla Expanded Framework (used by Alpha Genes)." when unavailable), the Genes only / Absolute radio buttons with tooltips, the Debug logging switch, and a Reset to defaults button (depends on T003, T004) — done
- [X] T007 Rewire `Source/HarmonyPatches.cs`: the `ConditionalStatAffecter_InSunlight.Applies` postfix gains `ConditionalStatAffecter_InSunlight __instance` and blocks only when `SunProtection.ShouldBlock(Slowness, pawn, PenaltyRules.IsSlowdown(__instance), GeneSources.IsGeneAffecter(__instance))`; the `ThoughtWorker_InSunlight.CurrentStateInternal` postfix gains `ThoughtWorker_InSunlight __instance` and blocks only when `ShouldBlock(Mood, p, PenaltyRules.IsNegativeMood(__instance.def, __result.StageIndex), GeneSources.IsGeneThought(__instance.def))`; keep the existing `__result.Active` / `__result` early-outs first (FR-006, FR-020) (depends on T005) — done
- [X] T008 Update `Source/ClothedVampiresMain.cs`: after `harmony.PatchAll()`, call `VefCompat.TryPatch(harmony)` and log exactly one of the two VEF startup lines from the contract after the existing "Mod loaded" line (depends on T004) — done
- [X] T009 Build from `Source/` with `dotnet build`: both frameworks compile with no warnings from the mod's code; if net10.0 differs from net472 at a call site, branch with `#if NET10_0_OR_GREATER` and note it in `C:\Users\gulle\.claude\skills\rimworld-1-7-uplift\SKILL.md` — done 2026-11-14: both frameworks, 0 warnings, 0 errors; no `#if` needed

**Checkpoint**: the mod builds with settings and the rewired vanilla patches; VEF effects not yet blocked.

---

## Phase 3: User Story 1 - A dressed vicious-UV vampire stays awake (Priority: P1) 🎯 MVP

**Goal**: clothing blocks the VEF sunlight-condition stages (vicious knockout).

**Independent Test**: quickstart V3 with default settings and Debug logging on.

- [X] T010 [US1] In `Source/VefCompat.cs`, add `TryPatch(Harmony)` part 1: if `HediffComp_StageBySunlight` was found, manually patch its `CompPostTickInterval` with a prefix `(HediffComp __instance, int delta)`: if `!Gen.IsHashIntervalTick(pawn, 500, delta)` or `pawn.Map == null` → return true; read `sunlightStageIndex`/`sunlessStageIndex` from `__instance.props` through the cached field refs; `isPenalty = PenaltyRules.StageReducesCapacity(parent.def, sunlightStageIndex)`; `fromGene = GeneSources.HediffFromActiveGene(pawn, parent.def)`; if `SunProtection.ShouldBlock(Conditions, …)` → set `parent.Severity = sunlessStageIndex`, and if `pawn.Position.InSunlight(pawn.Map)` debug-log `Blocked sunlight condition '<label>' on <pawn> (from gene: yes|no)`, return false; else return true (research R2; Principle V) — done
- [X] T011 [US1] Build (T009 checks) and confirm the deployed Mods folder still matches spec 001's `specs/001-dual-build-docs/contracts/published-layout.md` — done 2026-11-14: clean build; Mods folder layout unchanged, deployed DLLs match
- [X] T012 [US1] Run quickstart **V1**, **V2** and **V3** with the user on 1.7 (full mod list, Debug logging on): startup lines, settings window screenshot against the contract, vicious-UV pawn covered for 1 in-game hour (no knockout; debug lines), shirt only and underwear only (knocked out), knocked-out pawn dressed and returned (recovers); record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14** (1.7.5005, full list incl. CE): V1, V2, V3 steps 1–3; step 4 (recovery) shown via V4 minor-UV re-dress and the V5 switch test
- [X] T013 [US1] If T012 fails: diagnose from Player.log, fix in `Source/*.cs`, rebuild, relaunch, re-run the failed steps, log under "Issues found" in `specs/002-vef-sunlight-settings/test-progress.md` (draft a GitHub bug issue for the user first if it's a mod defect); otherwise mark done "no issues" — no issues (2026-11-14)

**Checkpoint**: the customer's bug is fixed (SC-001, SC-007).

---

## Phase 4: User Story 2 - Lethal and minor UV covered too (Priority: P1)

**Goal**: clothing blocks VEF sunlight burns; minor UV's sight loss is covered by US1's code.

**Independent Test**: quickstart V4.

- [X] T014 [US2] In `Source/VefCompat.cs`, add `TryPatch` part 2: if `HediffComp_HighlyFlammable` was found, manually patch its `CompPostTickInterval` with a prefix `(HediffComp __instance, int delta)`: if `sunlightBurns` is false → return true (real-fire behavior untouched, FR-003); if `!Gen.IsHashIntervalTick(pawn, tickInterval, delta)` or `pawn.Map == null` → return true; `fromGene = GeneSources.HediffFromActiveGene(pawn, parent.def)`; if `SunProtection.ShouldBlock(Burns, pawn, true, fromGene)` → if in sunlight debug-log `Blocked sunlight burn '<label>' on <pawn> (from gene: yes|no)`, return false; else return true — done (written with T010, one build)
- [X] T015 [US2] Build (T009 checks) — done (same build as T011)
- [X] T016 [US2] Run quickstart **V4** with the user on 1.7 (Debug logging on): lethal covered 1 in-game hour (no new burns; debug lines), lethal shirt only (burns), lethal covered and set on fire (fire burns), minor covered vs uncovered (Sight capacity), existing burns stay; record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14** (1.7.5005, full list incl. CE): lethal covered/uncovered/re-dressed, real fire, minor covered/uncovered/re-dressed
- [X] T017 [US2] If T016 fails: fix, rebuild, re-run, log under "Issues found"; otherwise mark done "no issues" — no issues (2026-11-14)

---

## Phase 5: User Story 3 - Players choose which protections apply (Priority: P2)

**Goal**: the four switches work live and persist; bonuses are never blocked (FR-020).

**Independent Test**: quickstart V5, V7, V8.

- [X] T018 [US3] Run quickstart **V5** with the user on 1.7: each switch off/on with covered mild-UV, vicious-UV and lethal-UV pawns (effects return/disappear at the stated timings), all four off = covered and uncovered identical, Debug logging off = no mod lines during play, restart = settings unchanged; record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14** (1.7.5005, full list incl. CE): each switch off/on (knockout, burns, slowness, mood) with live effect; debug off = no lines; Absolute + debug off survived restart; Reset to defaults works. "All four off at once" not run separately
- [X] T019 [US3] Run quickstart **V7** (covered `AG_UVPowered_Mild` pawn keeps the "in sunlight" ×1.2 speed in both modes) and **V8** (spec 001 regression: naked / shirt + pants / underwear with a mild-UV sanguophage); record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14**: V7 UV powered ×120% kept on a covered pawn in both modes; V8 by equivalent evidence (see test-progress)
- [X] T020 [US3] If T018–T019 fail: fix in `Source/*.cs`, rebuild, re-run, log under "Issues found"; otherwise mark done "no issues" — no issues (2026-11-14)

---

## Phase 6: User Story 4 - Genes only or Absolute (Priority: P2)

**Goal**: the mode decides whether non-gene sunlight effects are blocked.

**Independent Test**: quickstart V6.

- [X] T021 [US4] Run quickstart **V6** with the user on 1.7 (Debug logging on): covered pawn without the gene but with `AG_Sensitivity_Vicious_Hediff` added directly → Genes only: knocked out, no debug line; Absolute: conscious, "from gene: no" line; pawn with the gene blocked in both modes; record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14**: Genes only leaves a directly-added vicious condition alone (Jake downed, no log line); Absolute blocks it ("from gene: no", Jake recovers); gene effects blocked in both modes
- [X] T022 [US4] If T021 fails: fix in `Source/SunProtection.cs` or `Source/VefCompat.cs`, rebuild, re-run, log under "Issues found"; otherwise mark done "no issues" — no issues (2026-11-14)

---

## Phase 7: User Story 5 - Settings explain themselves without VEF (Priority: P3)

**Goal**: clean load and grayed-out VEF switches when VEF is absent.

**Independent Test**: quickstart V9 (load-only, minimal list).

- [X] T023 [US5] Run quickstart **V9** with the user on 1.7 with only Harmony, Core, Biotech, Anomaly and Clothed Vampires: log "Vanilla Expanded Framework not found…", no errors; screenshot of the settings window with the two VEF switches grayed out and the note; record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14** (1.7.5005, minimal list): "VEF not found" line, no errors; VEF switches grayed with the note
- [X] T024 [US5] If T023 fails: fix in `Source/VefCompat.cs` or `Source/ClothedVampiresMod.cs`, rebuild, re-run, log under "Issues found"; otherwise mark done "no issues" — no issues (2026-11-14)

---

## Phase 8: Release (1.6 check and documentation)

- [X] T025 Run quickstart **V10** with the user: switch Steam to 1.6, load with the user's 1.6 list (VEF, Alpha Genes, CE), clean load (1.6 log `…\RimWorld by Ludeon Studios\Player.log`), covered vicious-UV pawn stays conscious in sun (debug line); switch back to 1.7; record in `specs/002-vef-sunlight-settings/test-progress.md` — **PASS 2026-11-14 on re-run** (1.6.4871, 47 mods incl. VEF, Alpha Genes, CE): first attempt failed with issue #2 (1.6 folder held the net10.0 DLL; fixed with a per-framework IntermediateOutputPath in `Source/ClothedVampires.csproj`); re-run clean load, covered vicious-UV pawn Sev blocked 15 times and stayed up
- [X] T026 [P] Write `docs/settings-guide.md` (constitution: required now that settings exist), modeled on `C:\RimWorldDev\DakotaFarmTweaks\docs\settings-guide.md`: title, intro, where to find it, the idea in one paragraph, one section per setting with **Choices** and **Default** matching `specs/002-vef-sunlight-settings/contracts/settings-and-log.md` exactly, what each never does (never removes bonuses, never heals, real fire still burns), Genes only vs Absolute with examples, debug logging, defaults table / Reset, compatibility (VEF/Alpha Genes needed for the two switches), adding/removing mid-colony; only tested claims — done 2026-11-14
- [X] T027 [P] Update `docs/steam-workshop-description.txt`: add the settings-guide link (`https://github.com/DaraynLB/ClothedVampires/blob/main/docs/settings-guide.md`) and a Settings section (one bullet per setting with default); move Alpha Genes from "doesn't touch (yet)" to what it does (vicious, minor, lethal) with tested numbers; state that sunlight bonuses are kept; update "How this mod was built" with what was tested this round — done 2026-11-14
- [X] T028 [P] Update `README.md`: what it does now (VEF/Alpha Genes effects, settings, penalties only), what it doesn't touch, link to `docs/settings-guide.md` — done 2026-11-14 (also notes the per-framework obj folder)
- [X] T029 [P] Write `docs/updates/<YYYY-MM-DD>-alpha-genes-sunlight.md` for the customer (dated the day testing finished), modeled on spec 001's note: what we're building (their burka vampire), what we tested (real pawn names and numbers from `test-progress.md`), issues found, decisions made (minor UV under the knockout switch, debug logging, penalties only and the UV-powered bonus fix), where this leaves it — done 2026-11-14 as `docs/updates/2026-11-14-alpha-genes-sunlight.md`
- [X] T030 Review T026–T029 against `specs/002-vef-sunlight-settings/test-progress.md`: every claim traces to a recorded result or is marked untested; labels and defaults match the contract and the in-game window — done 2026-11-14: two claims softened (Lazy's exact outfit; the old-version bonus removal is from code reading, not tested); labels match `Source/ClothedVampiresMod.cs`; `docs/` not deployed
- [X] T031 Final consistency pass: `git status` shows only intended changes; Mods folder layout unchanged from spec 001's contract; all tasks in `specs/002-vef-sunlight-settings/tasks.md` ticked with dated notes — done 2026-11-14: only intended changes; 1.6 DLL = v4.7.2, 1.7 DLL = v10.0; Mods folder layout unchanged
- [X] T032 Commit on `002-vef-sunlight-settings` (`Block Alpha Genes / VEF sunlight effects when covered; add settings` + Co-Authored-By line); ask the user before pushing or opening a PR — done 2026-11-14 (local commit, closes #2; push/PR awaiting the user)

---

## Dependencies & Execution Order

- Setup (T001–T002) → Foundational (T003–T009) → stories.
- Foundational: T003 and T004 in parallel; T005 needs both; T006 needs T003+T004; T007 needs T005; T008 needs T004; T009 after all.
- US1 (T010–T013) needs Foundational. US2 (T014–T017) needs Foundational; T014 edits the same file as T010, so do it after T010.
- US3–US5 are verification of Foundational + US1/US2 code: they need T010 and T014 built.
- Release (T025–T032) after all stories.

**Recommended build-then-test order** (fewest game relaunches): implement T010 and T014,
build once, then run T012, T016, T018–T019 and T021 in **one** 1.7 session (full list),
then T023 (minimal list relaunch), then T025 (1.6). Fix tasks (T013, T017, T020, T022,
T024) apply only if something fails.

## Parallel Opportunities

- T003 ∥ T004 (different new files).
- T026 ∥ T027 ∥ T028 ∥ T029 (different files) once testing is recorded.

```text
# Foundational
T003 Source/ClothedVampiresSettings.cs   ∥   T004 Source/VefCompat.cs
# Release docs
T026 docs/settings-guide.md  ∥  T027 docs/steam-workshop-description.txt  ∥  T028 README.md  ∥  T029 docs/updates/...
```

## Implementation Strategy

1. **MVP**: Phases 1–3. Vicious-UV pawns stay awake when dressed (the customer's bug).
2. Add US2 (burns), then verify US3–US5 in the same session.
3. 1.6 smoke test and release docs, then commit; push/PR and Workshop upload only with the
   user's go-ahead.

## Appended after implementation (2026-11-14)

- [X] T033 Make the user-supplied Workshop preview image compliant: move the source `About/Designer (2).png` (1536×1024, 2.87 MB) to `Art/Preview-source.png` (raw art, never published); write `About/Preview.png` (the exact name RimWorld's Workshop uploader and mod list use) at full 1536×1024, 256-colour palette (median cut + k-means, Floyd–Steinberg dither), 0.51 MB — under Steam's 1 MB preview limit; mean colour error 2.8/255, green speech bubble preserved (an earlier median-cut-only palette turned it beige). Kept 3:2 rather than cropping to 16:9 so neither the title nor the paws are cut; Steam doesn't require 16:9. Deployed by the build (byte-identical in the Mods folder) — done 2026-11-14
