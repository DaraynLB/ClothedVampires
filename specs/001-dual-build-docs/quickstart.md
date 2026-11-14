# Quickstart: Validating Dual 1.6/1.7 Support

Run these after implementation. Results feed the docs (FR-013), so record each one in
`tasks.md` or `test-progress.md`.

## Prerequisites

- .NET 10 SDK (10.0.302 installed); net472 reference assemblies come from NuGet.
- RimWorld with Biotech, the Harmony mod, and RimSort.
- For V4: UnderWhere and Undergarments Core (both installed on this machine).
- For V2b: Vanilla Expanded Framework, Alpha Genes and the Big and Small mods (all
  installed on this machine).

## Mod list decision (2026-11-14)

Functional checks (V3, V4, V5) run on the user's **full mod list** (Alpha Genes, Vanilla
Expanded Framework, Big and Small, both underwear mods and more), not a minimal one. The
customer's bug happened with a large mod list, so a minimal list would hide the
interactions that matter. V2 (minimal list) is kept as a **load-only** check (launch to
the main menu, read the log, quit) to satisfy the constitution's "optional mods absent"
rule (Principles I and II); user chose this over amending the constitution.

## V1. Build output (no game needed)

From `Source/`: `dotnet build`

Expect:
- `Build succeeded` with no warnings from the mod's code.
- Repo: `1.6/Assemblies/ClothedVampires.dll` and `1.7/Assemblies/ClothedVampires.dll`
  only; no root `Assemblies/`.
- Mods folder matches [contracts/published-layout.md](contracts/published-layout.md):
  `About/`, `LoadFolders.xml`, `1.6/`, `1.7/`, and nothing else; no `0Harmony.dll`,
  `.pdb` or `.deps.json`.

## V2. Clean load on 1.7 (full test version)

1. Refresh RimSort, enable Harmony then Clothed Vampires, launch on 1.7.
2. Read `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Logs\Player.log`,
   filtering header lines for `clothedvampires|harmony|WARN|ERROR`.

Expect: one `[ClothedVampires] Mod loaded, Harmony patches applied.`; no ERROR/WARN
mentioning the mod; no `[UnityStubs] … Stub!` line from the mod's code.

## V2b. Clean load with the optional mods present (1.7)

Constitution Principle I: the mod must load cleanly with its optional mods present too.
Enable, after Clothed Vampires' dependencies: Vanilla Expanded Framework, Alpha Genes,
Big and Small - Framework and Big and Small - Genes & More (UnderWhere and Undergarments
Core are covered by V4). Launch on 1.7 and read the log as in V2.

Expect: the same single "Mod loaded" line and no ERROR/WARN mentioning Clothed Vampires.
Other mods' own warnings are noise. This load is also the baseline for spec 002.

## V3. Protection works on 1.7

Dev Mode on → Dev quicktest. Daytime, clear weather, pawn standing outdoors and unroofed.

After every clothing change, run at speed 1 for about 30 real seconds before reading the
mood list: situational mood thoughts refresh periodically, so the Needs tab lags behind.
Move speed updates immediately.

1. Spawn a **sanguophage** colonist (intense UV sensitivity) and strip its clothes.
   Expect, in sunlight: Needs tab shows **"sunlight sensitivity" -12**; Move speed (info
   card) includes the sunlight ×80% factor.
2. Give it a shirt (torso) only. Expect: same penalties (legs uncovered).
3. Add pants (torso + legs). Expect within a few seconds: the thought is gone and the
   Move speed has no sunlight factor.
4. Swap to a single item covering both (e.g. a duster or robe), with nothing else.
   Expect: still protected.
5. Move it under a roof, undressed. Expect: no penalties (the normal game, for reference).

## V4. Underwear doesn't count (required)

Constitution Principle II: this feature repackages the underwear-layer check, so it is
tested with the underwear mods present. On 1.7, enable UnderWhere, then repeat with
Undergarments Core instead (each on its own, so a pass can't come from the other):

1. Dress the sanguophage in that mod's underwear only (top and bottom). Expect: full
   sunlight penalties.
2. Add a shirt and pants over the underwear. Expect: protected.

Record the result per mod.

## V5. Mid-colony update

Load a save made with the previous 1.6-only build (or any existing save with a
sanguophage). Expect: loads with no errors from the mod; V3 step 3 still holds.

## V6. 1.6 smoke test (before release)

Switch Steam's beta branch to 1.6, refresh RimSort, and **load an existing save that has a
sanguophage** (spec US2 scenario 2), e.g. one made before this change.
Read `…\RimWorld by Ludeon Studios\Player.log` (1.6 location).
Expect: the "Mod loaded" line, the save loads with zero errors from the mod, and V3 step 3
holds (the sanguophage in shirt + pants in sunlight has no penalties).
Switch back to 1.7 afterwards.

## Pass criteria

All of V1, V2, V2b, V3, V4, V5 and V6 pass. Map to the spec:
SC-001 = V2 + V2b, SC-002 = V6, SC-003 = V3, SC-004 = V1, SC-005 = docs review in the
final phase; FR-009 = V3 + V4; US2 scenario 2 = V6.
