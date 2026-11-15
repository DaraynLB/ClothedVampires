# Test Progress: 002-vef-sunlight-settings

Results of the quickstart checks ([quickstart.md](quickstart.md)). The release docs are
written from this file only.

## V1. Build — PASS (2026-11-14)

- `dotnet build` from `Source/`: Build succeeded, 0 warnings, 0 errors, net472 and net10.0;
  no version-specific code needed.
- Mods folder unchanged from spec 001's layout contract (`About/` incl.
  `PublishedFileId.txt`, `LoadFolders.xml`, `1.6/`, `1.7/`); deployed DLLs byte-identical to
  the repo's.

## V2. Load and settings window (1.7, full list) — PASS (2026-11-14)

- Launch `2026-11-14 17:28:12`, RimWorld 1.7.5005, full mod list **including Combat
  Extended**, VEF, Alpha Genes, Big and Small, UnderWhere, Undergarments Core (about 50 mods).
- Player.log line 637: `[ClothedVampires] Mod loaded, Harmony patches applied.`; line 662:
  `[ClothedVampires] Vanilla Expanded Framework sunlight effects found; knockout and burn
  protection active.` (both VEF prefixes applied on Harmony 3.0). No ERROR/WARN from the mod.
- Screenshot: Options → Mod Settings → Clothed Vampires shows the four switches (all on) with
  the contract labels, "Genes only" selected, Debug logging off, Reset to defaults, footer
  note. Burns tooltip text matches the contract.

## V3. Vicious UV knockout — in progress

Pawn **Lazy** (Tyler 'Lazy' Hodges, female baseliner), quicktest map, given
`AG_Sensitivity_Vicious` with the developer add-gene action; wearing shirt, pants and
underwear. Game running at ~15 TPS (large mod list incl. CE).

| Step | Verdict | Evidence |
|---|---|---|
| 1. Covered, ~1 in-game hour of sun (customer's scenario) | PASS | Log lines 1331, 1354, 1377, 1400, 1423: `[ClothedVampires DEBUG] Blocked sunlight condition 'vicious UV sensitivity' on Lazy (from gene: yes)` — 5 checks × 500 ticks ≈ 1 in-game hour, each logged only while in sunlight. Screenshot: Health shows Vicious UV sensitivity, Consciousness 100%, activity Wandering; 6h, clear. 0 ERROR/WARN |
| 2. Not covered (torso only) | PASS | Screenshot: Lazy's Gear = only "Synthread parka" (torso, no legs) after dropping pants; status Incapacitated, "Downed, unconscious", "Colonist needs rescue"; 6h, clear. Log: no "Blocked" lines after line 1445 (not protected, as expected), 0 ERROR/WARN |
| 3. Torso covered only by underwear | PASS | Pawn **Andy** (Andrew 'Andy' Brown), gene added, inside an unroofed wooden pen. Gear: "Guinea pig fur Basic undergarments" (Undergarments Core, torso + legs on the underwear layer) + "Synthread pants": legs covered, torso only by underwear → not covered. Status Incapacitated, "Downed, unconscious". Log lines 1446, 1469: `Blocked sunlight condition 'vicious UV sensitivity' on Andy (from gene: yes)` while fully dressed, none after stripping. VEF's own sunlight check knocked him out, so the pen cell is in sunlight (walls alone don't roof). 0 ERROR/WARN |
| Extra: roofed | PASS (normal-game check) | Andy (torso underwear only, knocked out) after a roof was added over the pen (roof overlay green): standing again. The knockout depends only on sunlight; under a roof VEF's own check sets the harmless stage. Not mod code |
| 4. Recovers when protection returns | moved to V5 | A downed pawn can't change clothes; covered by the knockout switch off → on test in V5 (same code path) |

## V4. Lethal and minor UV — PASS (2026-11-14)

| Step | Verdict | Evidence |
|---|---|---|
| 1. Lethal, covered, ~1 in-game hour | PASS | Pawn **Axle** (Axle Hurtle): Gear = Synthread T-shirt + Synthread pants (+ backpack); gene `AG_Sensitivity_Lethal` added. Log (after line 1491) lines +1, +24, +47, +70, +93: `Blocked sunlight burn 'lethal UV sensitivity' on Axle (from gene: yes)` (5 checks). Screenshot: Health = Lethal UV sensitivity only, no burns, all capacities 100%; 8h, clear |
| 2. Lethal, uncovered | PASS | Axle dropped pants. Log after line 1836: last block for Axle at +24 (17:42:36); then +70 (17:42:45) `[TattooMagic DEBUG] Pawn.PostApplyDamage: Axle ranged-hit Axle for 30.0 dmg` (the sunlight burn; instigator is the pawn itself, as in VEF's code). Screenshot: Permanent burn x3 (torso), Permanent burn on ribcage, stomach, pelvis; pain Severe; 8h, clear |
| 2b. Lethal, re-dressed: no new burns; existing stay (FR-004, FR-005) | PASS | Log after line 1836: uncovered damage at +70 (17:42:45, 30) and +117 (17:43:11, 22.5); after re-wearing pants, blocks at +159…+425 (8 checks, 17:43:32–17:44:30) and no damage after 17:43:11. Screenshot: earlier permanent burns remain (torso x3, ribcage, stomach, right leg, left leg, pelvis). Test procedure: the user removed one burn with dev tools so Axle could walk to his pants (manual, not the mod) |
| 4. Minor, covered | PASS (re-check) | Pawn **Apicia** (Cia 'Apicia' Api), gene `AG_UVSensitivity_Minor`. First screenshot (taken right after adding the gene): Sight 90% "Affected by: Minor UV sensitivity" — the condition starts at its sunlight stage until VEF's first check. Log +116 (17:40:53): `Blocked sunlight condition 'minor UV sensitivity' on Apicia (from gene: yes)`. Re-check screenshot: Sight 100%, condition still present; 8h, clear. Note: a newly added sunlight gene can show its penalty for up to one VEF check (~500 ticks) before the mod's first block |
| 3. Real fire on covered lethal-UV pawn (FR-003) | PASS | Log after line 1836: +710 (17:46:26) and +748 (17:46:29) `[TattooMagic DEBUG] Fire/burn damage result: Axle actually took 3.0 dmg from Flame`; between them +729 `Blocked sunlight burn 'lethal UV sensitivity' on Axle` — sunlight burn blocked while real fire still burns. Screenshot: new plain "Burn" on right leg, "Burn needs tending", Axle on fire |
| 4c. Minor, re-dressed: recovers (FR-005) | PASS | Screenshot after re-wearing pants: Sight 100%, Minor UV sensitivity still present; 9h, clear. Same code path as the knockout (conditions prefix), so this also demonstrates V3 step 4's recovery when covered again |
| 4b. Minor, uncovered | PASS | Apicia dropped pants: her blocks continue to +558 (17:44:59, after line 1836) then stop. Screenshot: Sight 90% (Minor UV sensitivity), pants on the ground; 9h, clear |

## V5. Switches — PASS (2026-11-14)

Pawns: **Grae** (new colonist, vicious UV, dressed), **Apicia** (minor UV, dressed),
**Axle** (lethal UV, dressed). Log lines are after line 2602.

| Step | Verdict | Evidence |
|---|---|---|
| Knockout switch off | PASS | Settings file saved 17:50:21 with `blockConditions False`. Apicia's blocks stop after 17:49:57; screenshot: Sight 90% while dressed. Grae collapsed. Axle's burn blocks continue (+77, 17:50:41): other switches unaffected |
| Knockout switch on again: recovery (also V3 step 4, FR-005) | PASS | Settings file saved 17:51:30 back to default (`debugLogging True` only). +134 (17:51:37) `Blocked sunlight condition 'vicious UV sensitivity' on Grae (from gene: yes)`; game message "Grae, Cave explorer is no longer incapable of walking"; Grae standing outdoors, 9h, clear. Apicia's blocks resume (+115, +172) |
| Burns switch off | PASS | Settings file saved 17:52:59 with `blockBurns False`; log after line 2849, +1 (17:53:02): `[TattooMagic DEBUG] Pawn.PostApplyDamage: Axle ranged-hit Axle for 25.0 dmg` (sunlight burn), no Axle blocks after. Screenshot: new permanent burns (torso x4, right arm, left tibia, ribcage), Axle downed (already heavily burned from V4), still dressed |
| Slowness switch off | PASS | Settings file saved 18:05:03 with `blockSlowness False`. Dixon (dressed: button-down shirt + pants, in sun): Move speed shows "Mild UV sensitivity (in sunlight): x90%", 4.06 c/s (= 4.51 × 0.9) |
| Mood switch off | PASS | Settings file saved 18:07:03 with `blockMood False` (slowness back on, so no longer stored). Dixon (dressed, in sun): Needs shows "Sunlight sensitivity -6" again; 10h, clear |
| Mood switch on again | PASS | Dixon's Needs back to "Very low expectations +24" only; "Sunlight sensitivity -6" gone |
| Burns switch on again | PASS | Settings file saved 17:53:57 back to default; +59 (17:54:07) `Blocked sunlight burn 'lethal UV sensitivity' on Axle (from gene: yes)`; only one sunlight hit (17:53:02) during the off period, none after |
| Debug logging off: no mod lines during play | PASS (absence) | Settings file saved 18:13:41 (only `mode Absolute`; debug back to default off). Before: block lines every few seconds (last 18:13:33). 18:13:41–18:14:23 (>40 s of play per user): Player.log unchanged, 0 lines |
| Settings persistence across restart (SC-004) | PASS | See V9: Absolute + debug off survived the relaunch |
| Reset to defaults | PASS | Screenshot after clicking: Genes only, all four switches on, Debug logging off |
| Settings persistence (file) | PASS | `Config/Mod_ClothedVampires_ClothedVampiresMod.xml` written on each change; only non-default values stored. Restart check pending |

## V6. Genes only vs Absolute — PASS (2026-11-14)

Pawn **Jake** (dressed: button-down shirt + pants + backpack; gene UV powered only), given
`AG_Sensitivity_Vicious_Hediff` directly via Health tab → Dev tool → add hediff (not from a gene).

| Step | Verdict | Evidence |
|---|---|---|
| Absolute: non-gene condition blocked | PASS | Settings file saved 18:10:59 with `mode Absolute`. Log +77 (18:11:14): `Blocked sunlight condition 'vicious UV sensitivity' on Jake (from gene: no)`; game message "Jake, Explorer is no longer incapable of walking"; screenshot: Consciousness 100%, outdoors 10h, clear |
| Absolute: gene-caused effects still blocked | PASS | After 18:10:59: +39 Apicia (minor), +58 Grae (vicious), +96 Axle (lethal burn), all `from gene: yes` |
| Genes only: non-gene condition not blocked | PASS | Settings file at default (Genes only). Screenshot: Jake Incapacitated, "Downed, unconscious", Consciousness 10%, Vicious UV sensitivity on whole body; outdoors 10h, clear. Log after line 4161: no lines for Jake |

## V7. Bonuses kept — PASS (both modes, 2026-11-14)

Absolute: settings file saved 18:12:52 with `mode Absolute`; Jake (covered, in sun) Move speed
still "UV powered (in sunlight): x120%", 5.76 c/s; his non-gene vicious condition blocked at
the same time (Consciousness 100%).

| Step | Verdict | Evidence |
|---|---|---|
| Jake (`AG_UVPowered_Mild`, baseliner, xenogene), covered, in sun, Genes only | PASS | Gear: Synthread button-down shirt + Synthread pants + backpack. Move speed "UV powered (in sunlight): x120%", Fast walker +0.20, final 5.76 c/s; 10h, clear. The shipped version removed this line for covered pawns |

Test-procedure note: vanilla mild UV and Alpha Genes' UV powered add no health condition
(they are stat effects and a mood thought only), so they don't appear in the Health tab;
check the Bio → Genes window instead.

## V8 / V5 baseline: Dixon (mild UV), covered, all switches on

| Step | Verdict | Evidence |
|---|---|---|
| Covered mild-UV pawn in sun | PASS | Pawn **Dixon** (Derek 'Dixon' Dolbeare, baseliner, `UVSensitivity_Mild` xenogene); in sunlight per user; Gear: Synthread button-down shirt + Synthread pants. Screenshots: Needs = "Very low expectations +24" only, no "Sunlight sensitivity"; Move speed 4.51 c/s (Large x98%), no "in sunlight" line; 10h, clear |

## V8. Spec 001 regression — PASS (2026-11-14, by equivalent evidence)

- Covered mild-UV pawn (Dixon) in sun with defaults: no slowdown, no "Sunlight sensitivity"
  (V8/V5 baseline above).
- The penalties apply when protection doesn't: slowness/mood switches off brought back the
  ×90% and -6 on the same pawn (V5), which exercises the same patched methods as an
  uncovered pawn.
- Underwear doesn't count: Andy (torso only Undergarments Core underwear + pants) was not
  protected (V3 step 3).
- Not repeated in this spec: a naked mild-UV pawn and an underwear-only mild-UV pawn
  (both passed in spec 001; the covered check `SunCover.IsCovered` is unchanged).

## V9. Without VEF (load-only) — PASS (2026-11-14)

- Launch `2026-11-14 18:15:48`, 1.7.5005; ModsConfig active: Harmony, Core, Biotech,
  Anomaly, Clothed Vampires.
- Player.log line 250 `[ClothedVampires] Mod loaded, Harmony patches applied.`; line 275
  `[ClothedVampires] Vanilla Expanded Framework not found; knockout and burn protection
  unavailable.` No ERROR/WARN from the mod (line 198 is the unrelated missing Workshop item
  3807619463).
- Screenshot (Mod Settings): "Knockout and other sunlight conditions" and "Burns from
  sunlight" drawn grayed (dimmed checkmarks) with "Needs Vanilla Expanded Framework (used by
  Alpha Genes)."; slowness, mood, mode and debug usable.
- Restart persistence (SC-004): Absolute still selected and Debug logging still off after the
  relaunch (as set at 18:13:41).

## V10. 1.6 smoke test — PASS on re-run (2026-11-14)

- First attempt FAILED: the 1.6 folder held the net10.0 DLL (issue #2, see below).
- Re-run after the fix: RimWorld 1.6.4871 rev591 (`Player.log` at the 1.6 location), 47-mod
  list incl. VEF, Alpha Genes, Combat Extended, Big and Small.
- Line 49 `[ClothedVampires] Mod loaded, Harmony patches applied.`; line 50 `[ClothedVampires]
  Vanilla Expanded Framework sunlight effects found; knockout and burn protection active.`
  No exceptions or errors from the mod (lines 38–42: Big and Small / Alpha Genes SoundDef
  warnings).
- Dev quicktest, pawn **Sev** (Sev Shadow, baseliner, vicious UV gene added, dressed): 15 lines
  `Blocked sunlight condition 'vicious UV sensitivity' on Sev (from gene: yes)` (last at lines
  149–151). Screenshot: Sev standing ("Wandering"), Consciousness 98%, outdoors 6h, clear.

## Notes from the 1.7 session

- All four switches were verified one at a time (each off brings its effect back, on blocks
  it again); "all four off at once" was not run as a separate step.
- Test-procedure notes: walls alone don't roof a cell (VEF still sees sunlight in an
  unroofed pen); vanilla mild UV and UV powered add no health condition; Health tab → Dev
  tool is the easy way to add a hediff; Spawn thing and Add Gene search internal names.
- Full session log copied to the scratchpad (`Player-002-full-session.log`).

## Issues found

### #2 — 1.6 build shipped the .NET 10 (1.7) DLL (FIXED)

- Found in V10 (2026-11-14): RimWorld 1.6.4871 log, `Error while instantiating a mod of type
  ClothedVampires.ClothedVampiresMod: System.TypeLoadException: VTable setup ... failed`, plus
  type-load errors for `System.Collections, Version=10.0.0.0` and `System.Runtime,
  Version=10.0.0.0`, and `Error in static constructor of ClothedVampires.ClothedVampiresMain`.
- Cause: `1.6/Assemblies/ClothedVampires.dll` was the net10.0 build (byte-identical to the 1.7
  DLL, MD5 fb9559ec…). `AppendTargetFrameworkToOutputPath=false` also stops the SDK splitting
  `obj\` per framework, so both builds shared `obj\Debug\` and raced. `main` (spec 001) was
  unaffected: its 1.6 DLL is `.NETFramework,Version=v4.7.2`.
- Fix: `<IntermediateOutputPath>obj\$(Configuration)\$(TargetFramework)\</IntermediateOutputPath>`
  in `Source/ClothedVampires.csproj` (as in DakotaFarmTweaks). Clean rebuild + 3 forced
  rebuilds: 1.6 = v4.7.2, 1.7 = v10.0 every time, repo and Mods folder; 0 warnings/errors.
- The 1.7 DLL after the fix is byte-identical to the one tested in V2–V9 (MD5 unchanged), so
  those results stand. Quickstart V1 now includes the framework check.
- GitHub: https://github.com/DaraynLB/ClothedVampires/issues/2 (the fix commit closes it).
