# Test Progress: 001-dual-build-docs

Results of the quickstart checks ([quickstart.md](quickstart.md)). The release docs are
written from this file only.

## V1. Build output — PASS (2026-11-14)

- `dotnet build` from `Source/`: Build succeeded, 0 warnings, 0 errors (3.6 s).
- net10.0 compiled with **no source changes** (T009 not needed).
- Repo: `1.6/Assemblies/ClothedVampires.dll`, `1.7/Assemblies/ClothedVampires.dll`; no root
  `Assemblies/`.
- Mods folder (`RimWorld/Mods/ClothedVampires/`): `About/About.xml`,
  `About/PublishedFileId.txt`, `LoadFolders.xml`, `1.6/Assemblies/ClothedVampires.dll`,
  `1.7/Assemblies/ClothedVampires.dll`. The old root `Assemblies/` was removed by the
  deploy step. No `0Harmony.dll`, `.pdb` or `.deps.json`. Deployed DLLs byte-identical to
  the repo's.
- Note: `About/PublishedFileId.txt` (Workshop item 3819404247, written by the game's
  uploader on 2026-10-13) existed only in the Mods folder. Added to the repo as T025 at the
  user's request; deployed copy unchanged. That rebuild gave one transient MSB3061 warning
  (`obj/Debug/ref/ClothedVampires.dll` locked by another process, likely the editor's C#
  language server; intermediate file only). The next build was clean: 0 warnings, 0 errors.

## V2. Clean load on 1.7, optional mods absent (load-only) — PASS (2026-11-14)

- User decision: functional checks run on the full mod list; V2 kept as a load-only check
  to satisfy constitution Principles I/II (optional mods absent).
- Launch `2026-11-14 17:00:02`, RimWorld 1.7.5005, Harmony Mod v3.0.1.0 (library
  3.0.0-rc.3). ModsConfig active list: `brrainz.harmony`, Core, Biotech, Anomaly,
  `daraynlb.clothedvampires`. Reached the main menu.
- Player.log line 250: `[ClothedVampires] Mod loaded, Harmony patches applied.` (exactly
  one). No ERROR/WARN from Clothed Vampires.
- Line 198 ERROR "Created WorkshopItem for 3807619463 but there is no folder for it" —
  another Workshop item (not this mod, 3819404247); appears on every launch regardless of
  mod list.

## V2b. Clean load with optional mods present (1.7) — PASS (2026-11-14)

- Launch `2026-11-14 16:37:17`, RimWorld 1.7.5005, Harmony Mod v3.0.1.0 (library
  3.0.0-rc.3). Active (from the quicktest's "Initializing new game with mods" list): Harmony,
  Core, Biotech, Anomaly, Better Prerequisites, Vanilla Expanded Framework
  (`OskarPotocki.VanillaFactionsExpanded.Core`), Alpha Genes, Big and Small (Core, main,
  Heaven, More Xenos, Yokai, Lamias), Clothed Vampires, Downgrade, Undergarments Core,
  UnderWhere, UnderWhere add-on, Homely sleepwear fix. No Combat Extended.
- Player.log line 331: `[ClothedVampires] Mod loaded, Harmony patches applied.` (exactly
  one). No ERROR/WARN, Harmony exception or UnityStubs line from Clothed Vampires.
- Other entries, not from this mod: line 198 ERROR missing folder for Workshop item
  3807619463 (another mod); lines 244–281 WARN unresolved SoundDefs in Alpha Genes / Big and
  Small; line 430 WARN duplicate trait in pawn generation; Vulkan/GOG overlay warnings.
- The game flags Undergarments Core, UnderWhere, the UnderWhere add-on and the Homely fix
  as "incompatible version" (not marked for 1.7); they loaded anyway.
- Note: this launch was meant to be V2 (minimal list) but the mod list wasn't reduced, so it
  is recorded as V2b. V2 still pending.

## V3. Protection works on 1.7 — PASS

Session: launch 2026-11-14 16:37, 1.7.5005, full mod list (see V2b), Dev quicktest map,
7h, clear, outdoors 38°F. Test pawn: **Dmitry** ("Human (sanguophage)", made via Debug
actions). Note: in this mod list Dmitry's sanguophage carries **mild** UV sensitivity
(-6 mood, ×90% speed), not the vanilla sanguophage's intense one; another mod (likely Big
and Small) changes the gene set. Mild is the other vanilla gene the mod covers, so the
expected penalties for this run are -6 / ×90%. He also has the Gigantism trait (×90%
speed, not sunlight-related).

| Step | Verdict | Evidence |
|---|---|---|
| 1. Undressed, in sun: mood | PASS | Screenshot A: Needs tab "Sunlight sensitivity -6" (also Naked -6, Cold -8) |
| 1. Undressed, in sun: speed | PASS | Screenshot B: Move speed "Mild UV sensitivity (in sunlight): x90%", final 3.89 c/s |
| 2. Shirt only: speed | PASS | Screenshot: Gear tab = only "Synthread button-down shirt"; Move speed "Mild UV sensitivity (in sunlight): x90%", final 3.89 c/s |
| 2. Shirt only: mood | PASS (re-check) | First screenshot was taken before the mood thoughts refreshed (no "Sunlight sensitivity" yet). Re-check after ~30 s at speed 1: Gear = only the shirt; Needs = "Sunlight sensitivity -6", Naked x2 -11, Chilly -4; outdoors, 7h, clear. Test-procedure timing, not a defect: situational mood thoughts refresh periodically, while stats are computed on view. No log lines after line 481 |
| 3. Shirt + pants: mood | PASS | Screenshot: Needs tab without "Sunlight sensitivity" (only +30, +10, Chilly -4); same 7h / clear / outdoors |
| 3. Shirt + pants: speed | PASS | Screenshot: Move speed has no "in sunlight" line; final 4.32 c/s (= 3.89 ÷ 0.9). Log: 0 ERROR/WARN after line 481 |
| 4. Duster only: mood | PASS | Screenshots: Gear = only "Camelhide duster (poor)"; Needs = Chilly -4, Unsightly environment -5, no "Sunlight sensitivity", no "Naked"; outdoors, 8h, clear |
| 4. Duster only: speed | PASS | Screenshot: Move speed has no "in sunlight" line; final 4.32 c/s |
| 5. Roofed, undressed | SKIPPED (user decision) | Checks only the normal game's roof behavior; the mod has no roof code. No roofed building on the quicktest map |

**V3 result: PASS** (steps 1–4; step 5 skipped by decision), 2026-11-14.

## V4. Underwear doesn't count — PASS

Same session and pawn as V3 (full mod list, Dmitry, mild UV sensitivity).

| Step | Verdict | Evidence |
|---|---|---|
| UnderWhere only: mood | PASS | Screenshots: Gear = "Hyperweave tanktop (masterwork)" + "Devilstrand boxers (masterwork)"; UnderWhere defs (v1.6 `Clothing.xml`): tanktop = Torso on `UnderwearTop`, boxers = Legs on `Underwear`, so torso and legs covered by underwear only. Needs = "Sunlight sensitivity -6", Chilly -4 (no "Naked": UnderWhere underwear counts for modesty); outdoors, 9h, clear |
| UnderWhere only: speed | PASS | Screenshot: Move speed "Mild UV sensitivity (in sunlight): x90%", final 3.90 c/s (+0.01 per masterwork item) |
| Undergarments Core only: mood | PASS | Item: "Basic undergarments" (`UGC_Apparel_IndustrialUndergarments`, one piece covering Torso + Legs on the `Undergarments` layer; Spawn thing searches defNames, so search `UGC`). Screenshots: Gear = only "Bluefur Basic undergarments (legendary)"; Needs = "Sunlight sensitivity -6"; outdoors, 10h, clear |
| Undergarments Core only: speed | PASS | Screenshot: Move speed "Mild UV sensitivity (in sunlight): x90%", final 3.89 c/s |
| Clothes over Undergarments Core: mood | PASS | Screenshots: Gear = Bluefur basic undergarments + Synthread button-down shirt + Synthread pants; Needs = +30, +10, Unsightly environment -5, Hungry -6, no "Sunlight sensitivity"; outdoors, 11h, clear |
| Clothes over Undergarments Core: speed | PASS | Screenshot: Move speed has no "in sunlight" line; final 4.32 c/s |

Log for the whole V3/V4 run: no lines after 481 (no errors, warnings or exceptions).

**V4 result: PASS** (both underwear mods, 2026-11-14). Tested with both UnderWhere and
Undergarments Core loaded at once rather than one at a time (user decision: full mod list);
each mod's own items were tested separately, which exercises each mod's layers on its own.

## V5. Mid-colony update (1.7) — PASS

Substitute agreed with the user (no older save available): saved the test session as
`CV-Test` and reloaded it in the same session. Equivalent for this mod because it stores
nothing in saves (constitution Principle V); the save is reused for V6 on 1.6.

| Check | Verdict | Evidence |
|---|---|---|
| Reload: no errors from the mod | PASS | Line 503 `Loading game from file CV-Test`; 0 ERROR. Two WARNs, not from this mod: line 482 (on save, `DebugLoadIDsSavingErrorsChecker`) and line 531 (on load, `RimWorld.Trait.ExposeData`) — a trait's `suppressedBy` points at `Gene_665`, a gene removed when Dmitry's xenotype was changed with the debug action. No Clothed Vampires frames in either stack |
| Protection after reload: mood | PASS | Screenshot: Needs = +30, +10, Unsightly environment -5, Hungry -6, no "Sunlight sensitivity"; undergarments + shirt + pants; outdoors, 12h, clear |
| Protection after reload: mood (re-check) | PASS | Screenshot at 13h: still no "Sunlight sensitivity" |
| Protection after reload: speed | PASS | Screenshot at 13h, clear, outdoors: Move speed has no "in sunlight" line (Gigantism x90%, Fast runner +0.20 only); final 4.32 c/s |

**V5 result: PASS** (load, mood and speed all observed), 2026-11-14.

## V6. 1.6 smoke test with a save — PASS (2026-11-14)

- RimWorld **1.6.4871 rev591** (Steam default branch; `LastPlayedVersion.txt` 1.6.4871),
  log at the 1.6 location (`…\RimWorld by Ludeon Studios\Player.log`).
- Mod list: the user's larger 1.6 list (~47 mods), including **Combat Extended**, Vanilla
  Expanded Framework, Alpha Genes, Big and Small (all), Undergarments Core, UnderWhere,
  TattooMagic and other DaraynLB mods.
- Log line 53: `[ClothedVampires] Mod loaded, Harmony patches applied.` (once). Line 87:
  `Loading game from file CV-Test` — the save made on 1.7 loaded on 1.6. No exceptions and
  no Harmony errors; the only mention of the mod after load is the mod list (line 98).
- Other entries, not from this mod: lines 23–26 PrefsData XML errors (1.7-only prefs keys
  read by 1.6 after the branch switch); lines 42–46 unresolved SoundDefs (Big and Small,
  Alpha Genes); line 146 the known `Gene_665` trait reference (debug xenotype change, see V5).
- Screenshots (Dmitry Bennett, outdoors, 12h, clear): Gear = Bluefur basic undergarments +
  Synthread button-down shirt + Synthread pants; Needs = +30, +10, Unsightly environment -5,
  Hungry -6, no "Sunlight sensitivity"; Move speed = Gigantism x90%, Fast runner +0.20, no
  "in sunlight" line, final 4.32 c/s (CE's "Carried weight x100%" also listed).
- Bonus coverage: protection works with Combat Extended loaded on 1.6.

## Issues found

No mod defects. Test-procedure notes:

- V3 step 2 mood first read INCONCLUSIVE: the Needs tab was read before situational mood
  thoughts refreshed. Re-check after ~30 s passed. Quickstart V3 now says to wait ~30 s.
- `Gene_665` trait-reference warnings on save/load come from changing Dmitry's xenotype with
  the debug action, not from this mod.
- Spawn thing searches defNames, not labels (e.g. `UGC` for "Basic undergarments").

## Summary (2026-11-14)

All checks PASS: V1, V2 (load-only), V2b, V3 (step 5 skipped by decision), V4, V5, V6.
Versions: 1.7.5005 (full testing, no CE) and 1.6.4871 (smoke test, with CE).
