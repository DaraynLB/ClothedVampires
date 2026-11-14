---

description: "Task list for 001-dual-build-docs"
---

# Tasks: Dual 1.6/1.7 Support and Documentation Baseline

**Input**: Design documents from `specs/001-dual-build-docs/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/published-layout.md, quickstart.md

**Tests**: No automated tests. In-game verification is mandatory under constitution
Principle II, so the user-story phases contain verification tasks taken from
[quickstart.md](quickstart.md) (V1–V6). Record each result in
`specs/001-dual-build-docs/test-progress.md` with the date, game version and what was seen.

**Organization**: Grouped by user story. US1 = play on 1.7 (P1), US2 = keep 1.6 working
(P2), US3 = documentation (P3, also the release documentation phase).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Paths are relative to the repo root `C:\RimWorldDev\ClothedVampires\`

---

## Phase 1: Setup

**Purpose**: Confirm the starting point before changing the build.

- [ ] T001 Confirm the current branch is `001-dual-build-docs` and the only untracked/modified files are under `specs/001-dual-build-docs/` (`git status`); commit any spec changes first so the build change is its own commit
- [ ] T002 Re-check the newest `Krafs.Rimworld.Ref` 1.7 version (`curl -s https://api.nuget.org/v3-flatcontainer/krafs.rimworld.ref/index.json`) and the installed game version in `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Version.txt`; if either moved past 1.7.5005, use the version matching the game in T003 and note it in `specs/001-dual-build-docs/research.md` (R1)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The dual build itself. Both user stories need it before any in-game test.

**⚠️ CRITICAL**: No in-game verification until T008 passes.

- [ ] T003 Convert `Source/ClothedVampires.csproj` to multi-target: replace `<TargetFramework>net472</TargetFramework>` with `<TargetFrameworks>net472;net10.0</TargetFrameworks>`; remove the root `<OutputPath>`; add conditional PropertyGroups setting `OutputPath` to `..\1.6\Assemblies\` (net472) and `..\1.7\Assemblies\` (net10.0, plus `<GenerateDependencyFile>false</GenerateDependencyFile>`); move `Krafs.Rimworld.Ref` into conditional ItemGroups (`1.6.*` for net472, `1.7.5005-unstable` or the T002 version for net10.0); keep `Lib.Harmony` 2.3.* with `ExcludeAssets="runtime"`, and keep `LangVersion`, `RootNamespace`, `AssemblyName`, `DebugType none`, `Nullable disable`, `GenerateAssemblyInfo false`, `AppendTargetFrameworkToOutputPath false` (template: `rimworld-1-7-uplift` skill step 1, or `C:\RimWorldDev\TattooMagic\Source\*.csproj`)
- [ ] T004 Replace the `PublishToModsFolder` target in `Source/ClothedVampires.csproj` with the versioned one: `Condition="'$(TargetFramework)' == ''"`, `RemoveDir` of `$(ModPublishDir)\Assemblies`, and Copy steps for `LoadFolders.xml`, `About/**`, `1.6/**`, `1.7/**` only; keep the existing `RimWorldModsPath`/`ModPublishDir` properties (uplift skill step 2)
- [ ] T005 [P] Create `LoadFolders.xml` at the repo root loading `/` + `1.6` on v1.6 and `/` + `1.7` on v1.7, exactly as in `specs/001-dual-build-docs/contracts/published-layout.md`
- [ ] T006 [P] Add `<li>1.7</li>` to `supportedVersions` in `About/About.xml`; change nothing else (packageId, name, author, dependencies, description stay as they are)
- [ ] T007 Remove the root assembly folder from the repo: `git rm -r Assemblies` (deletes `Assemblies/ClothedVampires.dll`)
- [ ] T008 Build from `Source/` with `dotnet build` and run quickstart **V1**: build succeeds with no warnings from the mod's code; `1.6/Assemblies/` and `1.7/Assemblies/` each contain only `ClothedVampires.dll`; no root `Assemblies/` in the repo; `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\ClothedVampires\` contains only `About/`, `LoadFolders.xml`, `1.6/`, `1.7/` (old `Assemblies/` gone) and no `0Harmony.dll`, `.pdb` or `.deps.json`. Create `specs/001-dual-build-docs/test-progress.md` and record the result
- [ ] T009 Only if the net10.0 compile in T008 fails: fix each failing call site in `Source/*.cs` with `#if NET10_0_OR_GREATER` branches (never forked files), keep the 1.6 path unchanged, rebuild, and add an entry to "1.7 API differences found" in `C:\Users\gulle\.claude\skills\rimworld-1-7-uplift\SKILL.md`; otherwise mark this task done with "not needed"

**Checkpoint**: both DLLs built and deployed in the contract layout.

---

## Phase 3: User Story 1 - Play with the mod on RimWorld 1.7 (Priority: P1) 🎯 MVP

**Goal**: The mod loads cleanly on 1.7 and clothing still blocks the vanilla UV sensitivity
slowness and mood penalties.

**Independent Test**: quickstart V2 + V3 on 1.7 with only Harmony and Clothed Vampires
(plus Biotech), then V4 (underwear mods) and V2b (optional mods present).

- [ ] T010 [US1] Run quickstart **V2** with the user on RimWorld 1.7 (refresh RimSort, Harmony before Clothed Vampires): read `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Logs\Player.log` header lines filtered for `clothedvampires|harmony|WARN|ERROR`; pass = exactly one `[ClothedVampires] Mod loaded, Harmony patches applied.`, no ERROR/WARN or `[UnityStubs] … Stub!` from the mod; record in `specs/001-dual-build-docs/test-progress.md`
- [ ] T011 [US1] Run quickstart **V3** steps 1–5 with the user on 1.7 using a Dev Mode sanguophage (undressed → shirt only → shirt + pants → single torso+legs item → roofed); pass = penalties ("sunlight sensitivity" -12, sunlight ×80% move speed) only when not covered or only partly covered; record each step's observation in `specs/001-dual-build-docs/test-progress.md`
- [ ] T012 [US1] Run quickstart **V4** with the user on 1.7 (required, constitution Principle II): once with UnderWhere only and once with Undergarments Core only; underwear alone = full penalties, shirt + pants over underwear = protected; record the result per mod in `specs/001-dual-build-docs/test-progress.md`
- [ ] T013 [US1] Run quickstart **V2b** with the user on 1.7 (constitution Principle I): enable Vanilla Expanded Framework, Alpha Genes, Big and Small - Framework and Big and Small - Genes & More; pass = exactly one "Mod loaded" line and no ERROR/WARN mentioning Clothed Vampires; record in `specs/001-dual-build-docs/test-progress.md` as the baseline for spec 002
- [ ] T014 [US1] Run quickstart **V5** on 1.7: load an existing save that has a sanguophage (or one made with the old 1.6-only build); pass = no errors from the mod and V3 step 3 still holds; record in `specs/001-dual-build-docs/test-progress.md`
- [ ] T015 [US1] If any of T010–T014 fails: diagnose from Player.log, fix in `Source/*.cs` or `Source/ClothedVampires.csproj` (version-specific fixes via `#if NET10_0_OR_GREATER`), rebuild (T008 checks again), re-run the failed check, and log the problem and fix in `specs/001-dual-build-docs/test-progress.md` under "Issues found"; otherwise mark done with "no issues"

**Checkpoint**: 1.7 support works on its own (MVP).

---

## Phase 4: User Story 2 - Keep playing on RimWorld 1.6 (Priority: P2)

**Goal**: 1.6 players see no change.

**Independent Test**: quickstart V6 on 1.6.

- [ ] T016 [US2] Run quickstart **V6** with the user: switch Steam's beta branch to 1.6, refresh RimSort, load an existing save that has a sanguophage (spec US2 scenario 2), read `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log` (1.6 location); pass = the "Mod loaded" line, the save loads with zero errors from the mod, and V3 step 3 holds (shirt + pants in sunlight = no penalties); switch back to 1.7 afterwards; record in `specs/001-dual-build-docs/test-progress.md`
- [ ] T017 [US2] If T016 fails: fix (1.6 path only, without changing 1.7 behavior), rebuild, re-run V6 and the 1.7 V2 check, and log it under "Issues found" in `specs/001-dual-build-docs/test-progress.md`; otherwise mark done with "no issues"

**Checkpoint**: both versions verified.

---

## Phase 5: User Story 3 - Documentation (Priority: P3) — Release documentation

**Goal**: README, Workshop description and customer update note, all backed by
`test-progress.md`.

**Independent Test**: every claim in the three documents traces to a recorded result or is
marked untested (SC-005).

- [ ] T018 [P] [US3] Update `README.md`: requirements say RimWorld 1.6 or 1.7 (the right version loads automatically) with Biotech and Harmony; Build section says one `dotnet build` from `Source/` produces `1.6/Assemblies/` and `1.7/Assemblies/` and needs the .NET 10 SDK; Install section says the build publishes `About/`, `LoadFolders.xml`, `1.6/` and `1.7/` to `RimWorld/Mods/ClothedVampires` (or copy those four by hand); keep "What it does", "What it doesn't touch" and Credits accurate
- [ ] T019 [P] [US3] Write `docs/steam-workshop-description.txt` in Steam BBCode following the `rimworld-modding` release-docs structure and `C:\RimWorldDev\DakotaFarmTweaks\docs\steam-workshop-description.txt` as the model: `[h1]Clothed Vampires[/h1]`, italic tagline, pitch; bug reports → https://github.com/DaraynLB/ClothedVampires/issues; Requirements (RimWorld 1.6 or 1.7, Biotech, Harmony loaded first); What it does (covered = torso + legs, underwear doesn't count; blocks the mild/intense UV sensitivity slowness and mood penalty, and modded genes using the same sunlight condition); Compatibility (only what was tested, plus save-safe add/remove); What it doesn't touch; How this mod was built (Claude + Spec Kit, person-made decisions, person-played tests naming the versions tested, link to the repo); Credits. No settings-guide link and no Settings section (no settings yet)
- [ ] T020 [P] [US3] Write `docs/updates/<YYYY-MM-DD>-1-7-support.md` (dated the day testing finished) for the customer, modeled on `C:\RimWorldDev\DakotaFarmTweaks\docs\updates\2026-10-23-fertility-harvest-yield.md`: title `# Clothed Vampires — Progress Update, <Month Day>`; What we're building (the Alpha Genes report, and why 1.7 support and docs came first); Moving to RimWorld 1.7; What we tested (from `test-progress.md`, real pawn names and numbers); Issues we found (and fixed); Decisions made (dual build, docs baseline before the fix, gene-only/absolute toggle design for the next step); Where this leaves it (next: spec 002, the Alpha Genes / Vanilla Expanded Framework sunlight fix with settings; anything untested stated plainly)
- [ ] T021 [US3] Review T018–T020 against `specs/001-dual-build-docs/test-progress.md`: remove or mark untested any claim without a recorded result (FR-013, SC-005); confirm `docs/` is not copied by the deploy target (FR-015)

**Checkpoint**: release documents ready.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [ ] T022 Amend `.specify/memory/constitution.md`: remove TODO(DUAL_BUILD) and TODO(DOCS) from the Sync Impact Report as resolved by spec 001, bump the version from 1.0.1 to 1.0.2 (PATCH) and set Last Amended to that day
- [ ] T023 Run the final consistency pass: `git status` shows only intended changes (`.csproj`, `LoadFolders.xml`, `About/About.xml`, removed `Assemblies/`, new `1.6/` and `1.7/` DLLs, `README.md`, `docs/`, spec files); the Mods folder still matches `specs/001-dual-build-docs/contracts/published-layout.md`; mark all tasks in `specs/001-dual-build-docs/tasks.md` done
- [ ] T024 Commit on `001-dual-build-docs` with message `Support RimWorld 1.6 and 1.7 with a dual build, add docs baseline` (plus the Co-Authored-By line); ask the user before pushing the branch or opening a PR to `main`

---

## Dependencies & Execution Order

- **Setup (T001–T002)** → **Foundational (T003–T009)** → user stories.
- Within Foundational: T003 → T004 (same file); T005, T006 in parallel with T003/T004; T007 any time before T008; T008 after T003–T007; T009 only if T008's net10.0 compile fails.
- **US1 (T010–T015)** depends on T008. It is the MVP.
- **US2 (T016–T017)** depends on T008; it can run before or after US1, but it needs a Steam branch switch, so doing it after US1 avoids switching twice.
- **US3 (T018–T021)** depends on the test results from US1 and US2 (T018 can start any time after T008).
- **Polish (T022–T024)** last.

## Parallel Opportunities

- T005 and T006 (different files) alongside T003/T004.
- T018, T019 and T020 (three different files) once testing is recorded.
- In-game checks run sequentially with the user.

```text
# After T008 and testing:
T018 README.md
T019 docs/steam-workshop-description.txt
T020 docs/updates/<date>-1-7-support.md
```

## Implementation Strategy

1. **MVP**: Phases 1–3. The mod builds both versions and works on 1.7. Stop and confirm with
   the user.
2. **Increment 2**: Phase 4 (1.6 smoke test) before any release.
3. **Increment 3**: Phase 5 docs, then Phase 6 and commit.
4. Release (Workshop upload, push, PR) only after the user agrees. Spec 002 (Alpha Genes /
   VEF sunlight support with settings) starts from this branch's merged result.
