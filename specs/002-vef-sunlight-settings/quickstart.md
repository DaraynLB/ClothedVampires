# Quickstart: Validating Alpha Genes Sunlight Protection and Settings

Follows the spec 001 testing decisions: gameplay on the user's **full mod list** on 1.7
(includes Vanilla Expanded Framework, Alpha Genes, Big and Small, UnderWhere,
Undergarments Core); a **load-only** minimal check; a **1.6 smoke test**. Record results
in `test-progress.md`.

General rules (from spec 001):
- After any clothing or setting change, run speed 1 for ~30 real seconds before reading the
  mood list (it refreshes periodically). Knockout and burns update at VEF's check, every 500
  ticks (~8 s at speed 1). Move speed updates immediately.
- Spawn thing searches defNames; Debug actions: search by keyword, don't guess menu paths.
- Turn **Debug logging on** for V3–V7 so knockout/burn blocks show in the log.
- Daytime, clear weather, pawn outdoors and unroofed unless stated.

Fixtures: add Alpha Genes genes with the developer "add gene" action (search `gene` in
Debug actions); the genes are `AG_Sensitivity_Vicious`, `AG_UVSensitivity_Minor`,
`AG_Sensitivity_Lethal`, `AG_UVPowered_Mild`. Use expendable pawns for lethal UV.

## V1. Build

`dotnet build` from `Source/`: succeeds, no warnings from the mod; both DLLs; Mods folder
layout unchanged from spec 001 (contract in spec 001).

**Framework check (issue #2):** each DLL must carry its own framework stamp, in the repo and
in the Mods folder: `1.6/Assemblies/ClothedVampires.dll` → `.NETFramework,Version=v4.7.2`,
`1.7/Assemblies/ClothedVampires.dll` → `.NETCoreApp,Version=v10.0`
(`grep -aoE "v4\.7\.2|v10\.0" <dll>`). The two DLLs must not be identical.

## V2. Load and settings window (1.7, full list)

1. Log: one "Mod loaded" line and "Vanilla Expanded Framework sunlight effects found…";
   no ERROR/WARN from the mod.
2. 📸 Options → Mod Settings → Clothed Vampires: matches
   [contracts/settings-and-log.md](contracts/settings-and-log.md) (labels, defaults, tooltips).

## V3. Vicious UV knockout (US1, SC-001, SC-007)

Pawn with `AG_Sensitivity_Vicious`, default settings:
1. Shirt + pants, outdoors 1 in-game hour: stays conscious; log shows "Blocked sunlight
   condition 'vicious UV sensitivity'… (from gene: yes)" lines.
2. Shirt only: knocked out within ~8 s.
3. Underwear only (UnderWhere or `UGC` basic undergarments): knocked out.
4. While knocked out, carry indoors, dress in shirt + pants, bring back out: recovers and
   stays conscious.

## V4. Lethal and minor UV (US2, SC-002)

1. Lethal, shirt + pants, outdoors 1 in-game hour: no new burns; "Blocked sunlight burn" lines.
2. Lethal, shirt only: burns appear within ~8 s (end the test quickly).
3. Lethal, covered, set on fire (Debug actions: search `fire`/`flame`): burns from fire.
4. Minor, covered: Health → capacities: Sight not reduced; uncovered: reduced (-10%).
5. Existing burns from step 2 stay after dressing (FR-004).

## V5. Switches (US3)

With a covered mild-UV (vanilla), vicious-UV and lethal-UV pawn outdoors, turn each switch
off then on: slowness → ×90% returns/disappears; mood → "sunlight sensitivity" returns
after ~30 s; knockout → knocked out at next check / recovers; burns → burns resume / stop.
All off: covered and uncovered pawns behave identically (FR-014). Restart the game:
settings unchanged (SC-004).

## V6. Genes only vs Absolute (US4)

1. Covered pawn **without** the gene, with `AG_Sensitivity_Vicious_Hediff` added directly
   (Debug actions: search `hediff`): Genes only → knocked out (log: none); Absolute → stays
   conscious (log: "from gene: no").
2. Same pawn with the gene: blocked in both modes.

## V7. Bonuses kept (FR-020, SC-009)

Covered pawn with `AG_UVPowered_Mild` in sun: Move speed shows the "in sunlight" ×1.2 (not
removed), in both modes. Uncovered: same. (Shipped version removed it when covered.)

## V8. Spec 001 regression (SC-006)

Mild-UV sanguophage: naked = penalties; shirt + pants = none; underwear only = penalties.

## V9. Without VEF (US5, load-only, 1.7 minimal list)

Harmony + Core + Biotech + Anomaly + Clothed Vampires: log "Vanilla Expanded Framework not
found…", no errors; 📸 settings window shows the two VEF switches grayed out with the note.

## V10. 1.6 smoke test

1.6 with the user's 1.6 list (incl. VEF, Alpha Genes, CE): clean load; a covered
vicious-UV pawn stays conscious in sun (log line).

## Pass criteria

V1–V10 pass. Map: SC-001/007 = V3, SC-002 = V4, SC-003/004 = V5, SC-005 = V2+V9+V10,
SC-006 = V8, SC-008 = V3–V6 logs + off-check in V5, SC-009 = V7; FR-012 = V9; FR-020 = V7.
