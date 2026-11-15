# Feature Specification: Clothing Blocks Alpha Genes Sunlight Effects, With Settings

**Feature Branch**: `002-vef-sunlight-settings`

**Created**: 2026-11-14

**Status**: Draft

**Input**: User description: "Make clothing protect sunlight-sensitive pawns from the Vanilla Expanded Framework sunlight effects used by Alpha Genes: the knockout from vicious and minor UV sensitivity, and the burns from lethal UV sensitivity. Add mod settings with four on/off protections (slowness, mood, knockout, burns), all on by default, and a 'Genes only / Absolute' mode, defaulting to 'Genes only', that applies to all four. Gray out the knockout and burn options with a note when Vanilla Expanded Framework isn't loaded."

## Context

A customer's vampire, carrying Alpha Genes' **vicious UV sensitivity**, went outside fully
dressed (a burka) and was still overcome by the sun. Clothed Vampires only blocks the normal
game's sunlight penalties (slowness and the "sunlight sensitivity" mood thought). Alpha
Genes' sunlight genes instead give the pawn a sunlight-driven condition from Vanilla
Expanded Framework, which clothing never blocked:

- **Vicious UV sensitivity**: knocked unconscious in direct sunlight.
- **Minor UV sensitivity**: reduced sight in direct sunlight.
- **Lethal UV sensitivity**: repeated permanent burns in direct sunlight.

This feature makes clothing block those too, and gives players settings to choose which
protections apply and how widely. "Covered" keeps the existing definition: torso and legs
covered by clothing that isn't underwear (spec 001, verified in game).

## Clarifications

### Session 2026-11-14

- Q: Should the "knockout" switch also stop Alpha Genes' minor UV sensitivity from reducing a covered pawn's sight in the sun? → A: Yes. One switch, labeled "Knockout and other sunlight conditions", covers vicious UV's knockout and minor UV's sight loss.
- Q: Should the settings include a "Debug logging" switch (off by default) that writes a log line whenever clothing blocks a knockout or a sunlight burn? → A: Yes. Off by default; when on, log each blocked knockout/sunlight condition or burn (pawn, effect, gene or not), at most once per sunlight check per pawn. Slowness and mood blocks are not logged.

### Session 2026-11-14 (planning)

- Q: How should the mod decide which sunlight effects count as penalties (Alpha Genes' UV-powered and Shambler genes use the same mechanics for bonuses, and the shipped mod wrongly blocks them)? → A: Penalties only, per switch: slowness blocks an "in sunlight" stat effect only if it lowers movement speed (with any other stat changes in that same effect); mood blocks only thoughts with a negative mood value; conditions only if the sunlight stage reduces a capacity; burns always (burn damage is never a bonus). Bonuses are never blocked, in either mode.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A dressed vicious-UV vampire stays awake in the sun (Priority: P1)

A player's pawn has Alpha Genes' vicious UV sensitivity. Dressed in clothing that covers
torso and legs, the pawn walks outside in daylight and keeps working. Undressed, or only
partly dressed, the pawn is knocked out as before.

**Why this priority**: This is the customer's reported bug. It makes the mod's core
promise ("dressed vampires can go outside") true for the most popular vampire gene mod.

**Independent Test**: With Alpha Genes and Vanilla Expanded Framework loaded and default
settings, give a pawn vicious UV sensitivity, dress them in shirt and pants, and keep them
outdoors in clear daylight for a full in-game hour; then remove the pants.

**Acceptance Scenarios**:

1. **Given** default settings and a pawn with vicious UV sensitivity wearing torso and leg
   clothing, **When** they stand in direct sunlight for an in-game hour, **Then** they stay
   conscious and their consciousness is not capped by the sunlight condition.
2. **Given** the same pawn with only a shirt, **When** they stand in direct sunlight,
   **Then** they are knocked unconscious as in the normal game with Alpha Genes.
3. **Given** the same pawn wearing only underwear (UnderWhere or Undergarments Core),
   **When** they stand in direct sunlight, **Then** they are knocked unconscious.
4. **Given** a pawn already knocked out by the sun, **When** they become covered (for
   example, carried indoors and dressed, then taken back out), **Then** they recover and
   stay conscious in sunlight.

---

### User Story 2 - Lethal and minor UV sensitivity are covered too (Priority: P1)

A pawn with lethal UV sensitivity takes no sunlight burns while covered, and a pawn with
minor UV sensitivity keeps full sight while covered.

**Why this priority**: Same root cause and same expectation as Story 1. A vampire mod user
expects every sunlight gene in that family to respect clothing.

**Independent Test**: Give one pawn lethal UV sensitivity and another minor UV sensitivity,
dressed, outdoors for an in-game hour; compare health and sight with undressed copies.

**Acceptance Scenarios**:

1. **Given** default settings and a covered pawn with lethal UV sensitivity, **When** they
   stand in direct sunlight for an in-game hour, **Then** they receive no burns from the
   sun.
2. **Given** an uncovered pawn with lethal UV sensitivity, **When** they stand in direct
   sunlight, **Then** they receive burns as in the normal game with Alpha Genes.
3. **Given** a covered pawn with lethal UV sensitivity, **When** they catch fire from a real
   fire, **Then** they burn normally (only sunlight burns are blocked).
4. **Given** default settings and a covered pawn with minor UV sensitivity, **When** they
   stand in direct sunlight, **Then** their sight is not reduced by the sunlight condition;
   uncovered, it is.
5. **Given** burns a pawn already received before being covered, **When** they become
   covered, **Then** those burns stay (the mod prevents new sunlight burns; it never heals).

---

### User Story 3 - Players choose which protections apply (Priority: P2)

A player opens the mod's settings and turns individual protections on or off: slowness,
mood, knockout (sunlight conditions) and burns. Changes apply in the running game without a
restart.

**Why this priority**: Different colonies want different difficulty; some players want the
sun to stay dangerous for vampires (constitution, Principle IV).

**Independent Test**: In a running game with a covered pawn outdoors, toggle each switch
off and on and watch the matching effect return and disappear.

**Acceptance Scenarios**:

1. **Given** a fresh install, **When** the player opens the settings, **Then** all four
   protections are on and the mode is "Genes only".
2. **Given** a covered pawn with mild UV sensitivity outdoors, **When** the player turns off
   the slowness protection, **Then** the sunlight slowdown returns while the mood protection
   still applies, and the reverse for the mood switch.
3. **Given** a covered pawn with vicious UV sensitivity outdoors, **When** the player turns
   off the knockout protection, **Then** the pawn is knocked out at the next sunlight
   check; turning it back on lets them recover.
4. **Given** a covered pawn with lethal UV sensitivity outdoors, **When** the player turns
   off the burns protection, **Then** sunlight burns resume.
5. **Given** any combination of settings, **When** the game is closed and reopened,
   **Then** the settings are the same.
6. **Given** all four protections off, **When** covered and uncovered pawns stand in the
   sun, **Then** both behave exactly as without the mod.

---

### User Story 4 - Players choose "Genes only" or "Absolute" (Priority: P2)

The player chooses whether clothing blocks only sunlight effects that come from the pawn's
genes ("Genes only", default) or every sunlight effect of the protected kinds, whatever its
source ("Absolute").

**Why this priority**: Other mods may use the same sunlight mechanics for things that aren't
genes, including effects that are good in sunlight. Players decide whether clothing should
block those.

**Independent Test**: Give a covered pawn a sunlight condition of the protected kind that
does not come from a gene (for example, added directly with developer tools), outdoors;
switch the mode and observe.

**Acceptance Scenarios**:

1. **Given** "Genes only" and a covered pawn whose sunlight condition comes from one of
   their genes, **When** they stand in the sun, **Then** clothing blocks it.
2. **Given** "Genes only" and a covered pawn with the same kind of sunlight condition that
   did not come from a gene, **When** they stand in the sun, **Then** clothing does not
   block it.
3. **Given** "Absolute" and the same pawn as in 2, **When** they stand in the sun, **Then**
   clothing blocks it.
4. **Given** either mode, **When** a protection's switch is off, **Then** that protection
   does nothing regardless of mode.

---

### User Story 5 - Settings explain themselves without Vanilla Expanded Framework (Priority: P3)

A player without Vanilla Expanded Framework opens the settings and sees the knockout and
burns switches grayed out, with a short note that they need Vanilla Expanded Framework
(used by Alpha Genes). Slowness, mood and the mode still work.

**Why this priority**: Avoids confusion and bug reports from players with a smaller mod
list (constitution, Principles I and IV).

**Independent Test**: Load with only Harmony, the DLCs and Clothed Vampires; open the
settings; check the log.

**Acceptance Scenarios**:

1. **Given** Vanilla Expanded Framework is not loaded, **When** the player opens the
   settings, **Then** the knockout and burns switches are visible, grayed out, and labeled
   with a note saying they need Vanilla Expanded Framework.
2. **Given** Vanilla Expanded Framework is not loaded, **When** the game starts, **Then**
   the mod loads with no errors and at most one informational or warning line about the
   missing optional mod.
3. **Given** Vanilla Expanded Framework is loaded but Alpha Genes is not, **When** the
   player opens the settings, **Then** the knockout and burns switches are enabled (other
   mods may use the same sunlight effects).

---

### Edge Cases

- **Covering mid-knockout**: a pawn knocked out by the sun who becomes covered wakes up at
  the next sunlight check (Vanilla Expanded Framework checks about every 8 seconds of
  normal-speed game time); not instantly.
- **Uncovering in the sun**: a covered pawn who removes clothing outdoors gets the sunlight
  condition back at the next check.
- **Settings changed mid-game**: take effect at the next sunlight check; no restart, no save
  needed.
- **Mode switched while a condition is active**: the next check follows the new mode.
- **Burns already taken** are never healed or removed by the mod.
- **Real fire**: burns from actual fire are untouched, even for lethal-UV pawns.
- **Effects that are good in sunlight** (for example Alpha Genes' UV-powered ×1.2 speed or
  the Shambler gene's melee bonus): never blocked, in either mode. Clothing only removes
  sunlight penalties (constitution, Principle III). This also fixes the shipped version,
  which removed those bonuses from covered pawns.
- **A sunlight stat effect that only weakens something other than movement** (for example
  melee only): not blocked by the slowness switch.
- **Night, indoors or under a roof**: no change from the normal game (no sunlight, nothing
  to block).
- **Removing the mod mid-colony**: sunlight conditions resume at the next check; nothing is
  left in the save.
- **Underwear**: still never counts, for every protection.
- **Both game versions**: all behavior is the same on 1.6 and 1.7.

## Requirements *(mandatory)*

### Functional Requirements

**Protection from Vanilla Expanded Framework sunlight effects**

- **FR-001**: While covered and the knockout protection is on, a pawn MUST NOT be affected
  by sunlight-driven condition stages of the kind used by Alpha Genes' vicious and minor UV
  sensitivity (knockout, reduced sight); the condition MUST behave as it does out of
  sunlight.
- **FR-002**: While covered and the burns protection is on, a pawn MUST NOT receive
  sunlight-triggered burns of the kind used by Alpha Genes' lethal UV sensitivity.
- **FR-003**: Burns from real fire MUST be unaffected by the burns protection.
- **FR-004**: The mod MUST NOT heal, remove or reduce injuries or conditions the pawn
  already has; it only prevents new sunlight effects while covered.
- **FR-005**: A pawn who becomes covered while affected MUST be freed from the sunlight
  condition at the next sunlight check, and one who becomes uncovered MUST be affected
  again at the next check.

**Existing protections (unchanged behavior, now switchable)**

- **FR-006**: The existing slowness and mood protections (spec 001) MUST keep their
  behavior when on, and MUST each be individually switchable.
- **FR-007**: The definition of "covered" MUST stay the single shared one: torso and legs
  covered by non-underwear clothing.

**Settings**

- **FR-008**: The mod MUST have a settings window under Options → Mod Settings → Clothed
  Vampires with four on/off switches: slowness, mood, "Knockout and other sunlight
  conditions" (covering vicious UV's knockout and minor UV's sight loss) and burns, all on by
  default.
- **FR-009**: The settings MUST include a mode choice, "Genes only" (default) or
  "Absolute", applying to all four protections.
- **FR-010**: In "Genes only" mode, a protection MUST apply only to sunlight effects that
  come from one of the pawn's active genes. In "Absolute" mode it MUST apply to every
  sunlight effect of its kind on a covered pawn.
- **FR-011**: Settings MUST persist across game restarts and MUST take effect in a running
  game without restarting or reloading.
- **FR-012**: When Vanilla Expanded Framework is not loaded, the knockout and burns switches
  MUST be shown grayed out with a short explanation that they need Vanilla Expanded
  Framework (used by Alpha Genes).
- **FR-013**: Every setting MUST have a tooltip explaining it in plain words.
- **FR-014**: With all four protections off, the game MUST behave exactly as without the
  mod.
- **FR-020**: Protections MUST only block sunlight penalties, never bonuses, in either mode:
  the slowness switch only blocks an "in sunlight" stat effect that lowers movement speed
  (together with the rest of that effect); the mood switch only blocks thoughts whose mood
  value is negative; the conditions switch only blocks sunlight stages that reduce a
  capacity; the burns switch blocks every sunlight burn. A covered pawn with Alpha Genes'
  UV-powered gene MUST keep its sunlight speed bonus.
- **FR-019**: The settings MUST include a "Debug logging" switch, off by default. When on,
  the mod MUST write one log line each time clothing blocks a sunlight condition (knockout
  or sight loss) or a sunlight burn, naming the pawn, the effect, and whether it came from a
  gene, at most once per sunlight check per pawn. Slowness and mood blocks MUST NOT be
  logged. When off, the mod MUST log nothing during play.

**Compatibility and safety**

- **FR-015**: The mod MUST load with zero errors with Vanilla Expanded Framework and Alpha
  Genes present, and with them absent (at most one log line noting the absence).
- **FR-016**: The mod MUST NOT add anything to save files beyond its own settings, and MUST
  remain safe to add to or remove from an existing colony.
- **FR-017**: All behavior MUST be the same on RimWorld 1.6 and 1.7.

**Documentation**

- **FR-018**: A player's settings guide (`docs/settings-guide.md`) MUST be written, matching
  the in-game labels and defaults; the Workshop description, README and a dated update note
  MUST be updated, all from recorded test results.

### Key Entities

- **Protection**: one of slowness, mood, "Knockout and other sunlight conditions" (short
  name: knockout), burns; on or off.
- **Protection mode**: "Genes only" or "Absolute"; applies to all protections.
- **Debug logging**: on or off (default off); controls the blocked-effect log lines.
- **Covered**: the shared clothing check (torso + legs, no underwear).
- **Sunlight effect source**: whether a given sunlight effect on a pawn comes from one of
  the pawn's active genes or from something else.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: With default settings, a covered pawn with vicious UV sensitivity stays
  conscious for a full in-game hour of direct sunlight (0 knockouts); an uncovered one is
  knocked out within the same hour.
- **SC-002**: With default settings, a covered pawn with lethal UV sensitivity receives 0
  sunlight burns over a full in-game hour of direct sunlight; an uncovered one receives at
  least one.
- **SC-003**: Each of the four switches, turned off in a running game, brings back its
  effect on a covered pawn within one sunlight check (about 8 seconds at normal speed for
  knockout and burns; immediately for slowness; within about 30 seconds for mood), with no
  restart.
- **SC-004**: Settings survive 1 full game restart unchanged.
- **SC-005**: The mod loads with 0 errors on 1.7 and 1.6, with and without Vanilla Expanded
  Framework and Alpha Genes.
- **SC-006**: All existing spec 001 behaviors (slowness, mood, underwear) still pass with
  default settings.
- **SC-007**: The customer's scenario (vicious-UV vampire, dressed head to toe, outdoors in
  daylight) results in no knockout.
- **SC-008**: With debug logging on, every blocked knockout and sunlight burn during a test
  appears in the log (one line per pawn per sunlight check); with it off, the mod writes 0
  lines during play.
- **SC-009**: A covered pawn with Alpha Genes' UV-powered gene moves at its full sunlight
  bonus speed (×1.2) in direct sunlight, in both modes.

## Assumptions

- Alpha Genes' sunlight genes use the Vanilla Expanded Framework mechanics found in their
  published definitions: a sunlight-driven condition stage (vicious: knockout; minor:
  reduced sight) and sunlight-triggered burns (lethal: 15 permanent burn damage
  periodically). The plan confirms exact names against the versions installed.
- "Comes from a gene" means the effect is declared by one of the pawn's currently active
  genes. For the slowness penalty, that the penalty belongs to a gene's definition; for the
  mood thought, that the thought requires a gene; for conditions and burns, that the
  condition is one a gene gives the pawn.
- Other mods using the same Vanilla Expanded Framework sunlight mechanics are covered by
  the knockout and burns switches in the same way; none is tested by name.
- Testing follows the spec 001 decision: gameplay on the user's full mod list on 1.7, a
  load-only check with a minimal list, and a 1.6 smoke test.
- Alpha Genes' "UV-powered" genes are not sunlight penalties: their sunlight bonus is kept
  (FR-020), and their out-of-sunlight slowdown is not a sunlight effect, so it is out of
  scope.
- Wandering and pathing choices of sunlight-sensitive pawns stay out of scope.
