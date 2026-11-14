# Research: Dual 1.6/1.7 Support and Documentation Baseline

No NEEDS CLARIFICATION items remained in the Technical Context. These are the decisions
behind it.

## R1. 1.7 reference package version

- **Decision**: pin `Krafs.Rimworld.Ref` `1.7.5005-unstable` for `net10.0`.
- **Rationale**: the installed game reports `1.7.5005 rev1082` (`Version.txt`), and NuGet's
  newest 1.7 package on 2026-11-14 is `1.7.5005-unstable`. Matching the game avoids
  compiling against members that differ from what runs. A floating `1.7.*` would not match
  a prerelease.
- **Alternatives**: `1.7.4984-unstable` (TattooMagic's pin). It works, but it is older than
  the installed game for no reason.

## R2. .NET Framework 4.7.2 reference assemblies

- **Decision**: no machine-level Developer Pack needed. Keep the current setup.
- **Rationale**: the existing `net472` build already resolves
  `Microsoft.NETFramework.ReferenceAssemblies.net472` 1.0.3 from NuGet (seen in
  `Source/obj/project.assets.json`), and `C:\Program Files (x86)\Reference Assemblies\...`
  doesn't exist on this machine, yet the 1.6 DLL builds. The .NET 10 SDK (10.0.302) is
  installed for the `net10.0` build.
- **Alternatives**: install the Developer Pack. Unnecessary here.

## R3. Harmony on 1.7

- **Decision**: keep compiling against `Lib.Harmony` 2.3.* with `ExcludeAssets="runtime"`.
- **Rationale**: the mod uses only attribute Prefix/Postfix patches through `PatchAll()`.
  The uplift workflow verified that these apply unchanged against the Harmony 3.0 library
  the Harmony mod loads on 1.7. No reverse patches or transpilers are involved.
- **Alternatives**: a Lib.Harmony 3.x package. Only needed if a patch fails.

## R4. Patched game members on 1.7

- **Decision**: expect no source changes; confirm by compiling `net10.0` and by the log.
- **Rationale**: the patch targets are `ConditionalStatAffecter_InSunlight.Applies(StatRequest)`
  and `ThoughtWorker_InSunlight.CurrentStateInternal(Pawn)`; `SunCover` uses
  `Pawn.apparel.WornApparel`, `ApparelProperties.bodyPartGroups/layers`,
  `BodyPartGroupDefOf.Torso/Legs` and `DefDatabase<ApparelLayerDef>`. A missing or renamed
  member fails the compile (attribute targets via `nameof`) or logs a Harmony error at load
  (the string target `"CurrentStateInternal"`), and the quickstart checks both. If one
  differs, branch with `#if NET10_0_OR_GREATER` and record it in the uplift skill.

## R5. Deploy cleanup of the old root `Assemblies/`

- **Decision**: the deploy target runs `RemoveDir` on `Mods/ClothedVampires/Assemblies`
  before copying, as in the uplift workflow.
- **Rationale**: the live Mods folder currently holds `About/` and `Assemblies/`. `/` is a
  load folder on both versions, so a leftover root DLL would load next to the versioned one
  and apply every patch twice.

## R6. Built DLLs in git

- **Decision**: keep committing the built DLLs (now `1.6/Assemblies/` and
  `1.7/Assemblies/`).
- **Rationale**: the initial commit already tracks `Assemblies/ClothedVampires.dll`, and
  `.gitignore` excludes only `bin/`/`obj/`. Keeping the DLLs means the GitHub repo works as
  a downloadable mod, matching how it's set up today.
- **Alternatives**: ignore the version folders and ship only via Workshop. That would change
  how the repo is used, which is out of scope.

## R7. Documentation scope and references

- **Decision**: write `docs/steam-workshop-description.txt` and
  `docs/updates/<date>-1-7-support.md`, modeled on DakotaFarmTweaks
  (`C:\RimWorldDev\DakotaFarmTweaks\docs\`). No settings guide. The Workshop
  description leaves out the settings-guide link and the Settings section. Links point to
  https://github.com/DaraynLB/ClothedVampires (default branch `main`) and its Issues page.
- **Rationale**: the release documentation rules, minus the parts that need settings.
  The update note's audience is the customer whose Alpha Genes report started this work,
  so "what's next" names that fix (spec 002).

## R8. Test fixtures

- **Decision**: test with a Dev Mode-spawned **sanguophage**, which has
  `UVSensitivity_Intense`: 0.8× move speed in sunlight and the -12 "sunlight sensitivity"
  mood thought (`SunlightSensitivity_Major`).
- **Rationale**: a vanilla xenotype with the strongest vanilla effect makes the
  difference easy to see, and needs no other mods. Covered = any torso + legs apparel
  (e.g. a button-down shirt + pants, or a duster).
