# Implementation Plan: Dual 1.6/1.7 Support and Documentation Baseline

**Branch**: `001-dual-build-docs` | **Date**: 2026-11-14 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-dual-build-docs/spec.md`

## Summary

Convert Clothed Vampires from a single 1.6 (`net472`) build into the standard dual build
used by TattooMagic: one project targeting `net472` (RimWorld 1.6) and `net10.0`
(RimWorld 1.7), each DLL in its own version folder, selected by a new `LoadFolders.xml`.
The root `Assemblies/` folder is removed from the repo and from the deployed Mods folder,
and the deploy target publishes only `About/`, `LoadFolders.xml`, `1.6/` and `1.7/`.
No C# source changes are expected (TattooMagic needed none). Then verify in game on 1.7
(full) and 1.6 (smoke), and write the documentation baseline (Workshop description,
dated update note, README) from those results.

## Technical Context

**Language/Version**: C# (`LangVersion` latest), targets `net472` and `net10.0`

**Primary Dependencies**: `Krafs.Rimworld.Ref` 1.6.* (resolves 1.6.4871) for net472 and
`1.7.5005-unstable` for net10.0 (matches the installed game, 1.7.5005 rev1082);
`Lib.Harmony` 2.3.* compile-time only (`ExcludeAssets="runtime"`); the Harmony mod
supplies Harmony at runtime (Harmony library 3.0 on 1.7)

**Storage**: N/A. The mod stores nothing in saves and has no settings yet.

**Testing**: In-game verification through RimSort, Dev Mode and Player.log (constitution
Principle II); no automated test project. Build output checked by listing files.

**Target Platform**: RimWorld 1.7 (Waylith / .NET 10 CoreCLR) and 1.6 (Unity / Mono),
Windows, Biotech required

**Project Type**: RimWorld C# mod (single class-library project)

**Performance Goals**: No change from today. Packaging only; no new per-tick code.

**Constraints**: Gameplay behavior identical on both versions; no bundled `0Harmony.dll`;
no root `Assemblies/` shipped; no `.pdb` or `.deps.json` in version folders

**Scale/Scope**: 3 source files (unchanged), 1 `.csproj`, `About.xml`, new
`LoadFolders.xml`, README, 2 new docs files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / section | Status | Notes |
|---|---|---|
| I. Soft compatibility | Pass | No new dependencies. Underwear-mod detection stays name-based (`GetNamedSilentFail`). |
| II. Verify before done | Pass (planned) | Quickstart has full 1.7 checks and a 1.6 smoke test, both read from Player.log. |
| III. Narrow scope | Pass | No behavior change. `SunCover` untouched. |
| IV. Settings | N/A | The mod has no settings; spec 002 adds them. |
| V. Save safety | Pass | Nothing added to saves; mid-colony update is a quickstart check. |
| Platform: dual build | **Resolves TODO(DUAL_BUILD)** | This feature is the fix. |
| Platform: Harmony not bundled | Pass | `ExcludeAssets="runtime"` kept; output checked for `0Harmony.dll`. |
| Workflow: release docs | **Resolves TODO(DOCS)** | Workshop description and update note in the final phase; settings guide deferred to 002 (no settings exist). |
| Workflow: cite other mods' internals | N/A | No patches on other mods in this feature. |

No violations. Complexity Tracking left empty.

**Post-design re-check (after Phase 1)**: unchanged, all pass. The layout contract
([contracts/published-layout.md](contracts/published-layout.md)) encodes the platform
rules, and the quickstart covers Principles II and V.

## Project Structure

### Documentation (this feature)

```text
specs/001-dual-build-docs/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── published-layout.md
├── checklists/
│   └── requirements.md
└── tasks.md             # /speckit-tasks
```

### Source Code (repository root)

```text
ClothedVampires/
├── .gitignore
├── README.md                     # updated: versions, build, install
├── LoadFolders.xml               # new
├── About/
│   └── About.xml                 # + <li>1.7</li>
├── 1.6/Assemblies/               # new: net472 output (ClothedVampires.dll)
├── 1.7/Assemblies/               # new: net10.0 output (ClothedVampires.dll)
├── Assemblies/                   # REMOVED (git rm)
├── docs/                         # new, tracked, never published
│   ├── steam-workshop-description.txt
│   └── updates/YYYY-MM-DD-1-7-support.md
└── Source/
    ├── ClothedVampires.csproj    # multi-target + new deploy target
    ├── ClothedVampiresMain.cs    # unchanged
    ├── HarmonyPatches.cs         # unchanged
    └── SunCover.cs               # unchanged
```

**Structure Decision**: the standard dual-build layout from the `rimworld-modding` /
`rimworld-1-7-uplift` workflow, identical to TattooMagic's. The built DLLs in `1.6/` and
`1.7/` stay tracked in git, as `Assemblies/ClothedVampires.dll` is today, so the repo
holds a ready-to-use mod.

## Complexity Tracking

No constitution violations to justify.
