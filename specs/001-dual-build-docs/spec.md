# Feature Specification: Dual 1.6/1.7 Support and Documentation Baseline

**Feature Branch**: `001-dual-build-docs`

**Created**: 2026-11-14

**Status**: Draft

**Input**: User description: "Close the two constitution gaps for Clothed Vampires before any new gameplay feature. (1) Support RimWorld 1.6 and 1.7 with a dual build: multi-target net472 (1.6) + net10.0 (1.7), outputs in 1.6/Assemblies and 1.7/Assemblies, a LoadFolders.xml, About.xml listing 1.6 and 1.7, the root-level Assemblies/ folder removed from the repo and from the deployed Mods folder, Harmony still not bundled, and the deploy step publishing only About/, LoadFolders.xml, 1.6/ and 1.7/. Gameplay behavior must not change at all: clothed (torso + legs, underwear excluded) sunlight-sensitive pawns still ignore the vanilla UV sensitivity slowness and mood penalties, on both versions. (2) Create the docs/ baseline: docs/steam-workshop-description.txt and a dated docs/updates/ progress note describing the current mod (it has no settings yet, so no settings guide until a feature adds settings), written from actual test results; README updated for the dual build. Out of scope: the Vanilla Expanded Framework / Alpha Genes sunlight support and the settings toggles, which will be the next spec."

## Context

This feature closes the two gaps recorded when the project constitution (v1.0.0) was
ratified: the mod only supports RimWorld 1.6, and it has no player-facing documentation.
It changes no gameplay. The next feature (Vanilla Expanded Framework / Alpha Genes
sunlight support with settings) builds on top of it.

Because this feature is about how the mod is packaged, the published folder layout is
the user-visible contract here, and the requirements name it directly.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Play with the mod on RimWorld 1.7 (Priority: P1)

A player on RimWorld 1.7 (Steam's beta branch) enables Clothed Vampires. The game loads
the build made for 1.7, and a sanguophage dressed in a shirt and pants walks through
sunlight at full speed with no "in sunlight" mood penalty, exactly as on 1.6 today.

**Why this priority**: 1.7 is where all future work and full testing happen (constitution,
Principle II), and players are moving to it.

**Independent Test**: Load the mod on 1.7 with Harmony, then use Dev Mode to spawn a pawn
with intense UV sensitivity. Compare the pawn's move speed and mood in sunlight while
undressed and while wearing torso and leg clothing.

**Acceptance Scenarios**:

1. **Given** RimWorld 1.7 with Harmony and Clothed Vampires enabled, **When** the game
   starts, **Then** the log shows the mod's "Mod loaded, Harmony patches applied" line and
   no errors or warnings mentioning the mod.
2. **Given** a pawn with intense UV sensitivity wearing clothing that covers torso and legs,
   **When** the pawn stands in sunlight, **Then** its move speed has no sunlight penalty and
   it has no "in sunlight" mood thought.
3. **Given** the same pawn wearing only a shirt (torso, no legs), **When** it stands in
   sunlight, **Then** it gets the normal sunlight move-speed penalty and mood thought.
4. **Given** the same pawn wearing only underwear from an underwear mod (if installed),
   **When** it stands in sunlight, **Then** it gets the normal penalties.

---

### User Story 2 - Keep playing on RimWorld 1.6 (Priority: P2)

A player still on RimWorld 1.6 (Steam's main branch) updates the mod. It keeps working
exactly as before, with nothing to change on their side.

**Why this priority**: Most players are still on 1.6 until 1.7 leaves beta. Breaking them
would be worse than not supporting 1.7.

**Independent Test**: Switch Steam to 1.6, load a game with the mod, and confirm a clean
load (constitution's 1.6 smoke test). Optionally repeat the scenario 2 check.

**Acceptance Scenarios**:

1. **Given** RimWorld 1.6 with Harmony and Clothed Vampires enabled, **When** the game
   starts, **Then** the mod loads with zero dev-console errors.
2. **Given** a save made with the previous (1.6-only) version of the mod, **When** it is
   loaded with the new version, **Then** it loads without errors and the protection still
   works.

---

### User Story 3 - Understand the mod before and after installing (Priority: P3)

A player browsing the Steam Workshop reads what the mod does, what it needs, what it
doesn't touch and how it was built. The person who asked for the work (the customer) gets
a dated progress note explaining what changed and what was tested.

**Why this priority**: Required for every release (constitution, Development Workflow),
but only useful once the build works.

**Independent Test**: Read the Workshop description and progress note against the test
results from Stories 1 and 2; every claim must match something that was actually tested.

**Acceptance Scenarios**:

1. **Given** the finished Workshop description, **When** a player reads it, **Then** it states
   the supported versions (1.6 and 1.7), the requirements (Biotech, Harmony loaded first),
   what counts as covered, what the mod doesn't touch, a bug-report link and the
   "how this mod was built" disclosure.
2. **Given** the progress note, **When** the customer reads it, **Then** it explains the 1.7
   support, what was tested on which version, any issues found, and what comes next (the
   Alpha Genes sunlight support).
3. **Given** the README, **When** a developer reads it, **Then** the build, install and
   supported-version notes match the new dual-version layout.

---

### Edge Cases

- **Old deployed files**: a player's or developer's Mods folder still holds the old
  root-level `Assemblies/` folder from the 1.6-only version. The deploy step must remove it,
  otherwise both DLLs load on every version.
- **Harmony not bundled**: neither version's folder may contain a copy of Harmony; the
  Harmony mod supplies it.
- **Mod order**: the mod is loaded before Harmony. Behavior stays as today: RimWorld's
  normal dependency warning applies.
- **1.7 compile differences**: if any game API differs on 1.7, the fix must branch per
  version at the call site and keep 1.6 behavior unchanged.
- **No settings exist yet**: the docs must not mention settings or a settings guide.
- **Mid-colony update**: an existing colony updated from the 1.6-only version keeps
  working, since the mod stores nothing in saves.

## Requirements *(mandatory)*

### Functional Requirements

**Versions and packaging**

- **FR-001**: The mod MUST declare support for RimWorld 1.6 and 1.7.
- **FR-002**: The project MUST produce two builds of the mod's code from one codebase: one
  for 1.6 in `1.6/Assemblies/` and one for 1.7 in `1.7/Assemblies/`.
- **FR-003**: The mod MUST include a load-folder map at its root that loads the shared
  root content plus `1.6/` on 1.6, and the shared root content plus `1.7/` on 1.7.
- **FR-004**: The root-level `Assemblies/` folder MUST be removed from the repository and
  MUST NOT exist in the deployed Mods folder after a build.
- **FR-005**: Neither build folder MUST contain a copy of Harmony or other build
  by-products the game doesn't need (debug symbols, dependency manifests).
- **FR-006**: A single build command MUST produce both builds and publish only `About/`,
  the load-folder map, `1.6/` and `1.7/` into RimWorld's Mods folder.

**Behavior (unchanged)**

- **FR-007**: On both versions, a pawn whose torso and legs are covered by non-underwear
  clothing MUST ignore the sunlight move-speed penalty and the "in sunlight" mood thought
  of the Biotech mild and intense UV sensitivity genes, as the mod does today.
- **FR-008**: Pawns that are not covered MUST get the normal game's sunlight penalties on
  both versions.
- **FR-009**: The definition of "covered" MUST be unchanged: torso and legs, from one item
  or several, with underwear from UnderWhere / Undergarments Core not counting.
- **FR-010**: The mod MUST load with zero errors on 1.7 (full test) and on 1.6 (smoke test).

**Documentation**

- **FR-011**: `docs/steam-workshop-description.txt` MUST exist, in Steam formatting,
  following the release documentation structure in the modding workflow, minus the
  settings guide link and settings section (the mod has no settings yet).
- **FR-012**: A dated progress note `docs/updates/YYYY-MM-DD-<topic>.md` MUST exist,
  written for the customer, dated the day the work finishes.
- **FR-013**: Every claim in the Workshop description and progress note MUST be backed by a
  test actually performed in this feature; untested areas MUST be stated as untested.
- **FR-014**: `README.md` MUST be updated for the supported versions, the two builds, the
  build requirements and which folders are published.
- **FR-015**: The docs MUST be tracked in git and MUST NOT be published to the Mods
  folder.

### Key Entities

- **Version build**: the mod's compiled code for one game version (1.6 or 1.7), and the
  folder it ships in.
- **Load-folder map**: the file that tells RimWorld which folders to load on each version.
- **Release documents**: the Workshop description and the dated progress note.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: On RimWorld 1.7, the mod loads with 0 errors and 0 warnings attributed to it.
- **SC-002**: On RimWorld 1.6, the mod loads with 0 errors attributed to it.
- **SC-003**: On 1.7, a covered pawn with intense UV sensitivity in sunlight shows the same
  move speed as out of sunlight and has no "in sunlight" mood thought; an uncovered one
  shows the normal 20% slowdown and the mood thought.
- **SC-004**: One build command produces exactly one mod DLL per version and nothing else in
  the version folders, and the deployed mod folder contains only `About/`, the load-folder
  map, `1.6/` and `1.7/`.
- **SC-005**: 100% of statements in the Workshop description and progress note trace to a
  recorded test result or are marked untested.

## Assumptions

- The machine already has the .NET 10 SDK and the .NET Framework 4.7.2 Developer Pack.
- The current source compiles for 1.7 without changes, as TattooMagic's did; any
  difference found is fixed per version without changing 1.6 behavior.
- Full functional testing happens on 1.7; 1.6 gets a load smoke test before release, per
  the constitution.
- The public repository is https://github.com/DaraynLB/ClothedVampires (confirmed
  2026-11-14; empty at that date). Its default branch is `main`, the first branch pushed.
  The Workshop description links to its Issues page.
- The progress note's audience is the customer who reported the Alpha Genes sunlight
  problem; it mentions that report as what comes next.
- No settings guide is written in this feature; the next feature (which adds settings)
  creates it.
- The mod's packageId, name and author (`daraynlb.clothedvampires`, DaraynLB) are
  unchanged.
