# Clothed Vampires

A RimWorld mod for 1.6 and 1.7. Sunlight-sensitive pawns are unaffected by the sun while they are dressed.

## What it does

A pawn counts as covered when both the torso and the legs are covered by clothing. One item that covers both (a duster, a robe) is enough, and so is a shirt plus pants. Underwear from UnderWhere or Undergarments Core does not count, even when it covers the torso and legs.

While covered, a pawn standing in sunlight is spared these sunlight **penalties** (each can be switched off in the settings):

- **Slowness:** "in sunlight" stat effects that lower movement speed, such as Biotech's mild and intense UV sensitivity
- **Mood:** negative "in sunlight" thoughts, such as "Sunlight sensitivity"
- **Knockout and other sunlight conditions** (needs Vanilla Expanded Framework): Alpha Genes' vicious UV sensitivity (knockout) and minor UV sensitivity (reduced sight)
- **Burns** (needs Vanilla Expanded Framework): Alpha Genes' lethal UV sensitivity

A **Genes only** (default) / **Absolute** setting decides whether clothing blocks only penalties that come from the pawn's genes, or every sunlight penalty of those kinds. Sunlight **bonuses** (such as Alpha Genes' UV powered ×120% speed) are never removed, in either mode.

Every setting is explained in the [player's guide to the settings](docs/settings-guide.md).

## What it doesn't touch

- Sunlight bonuses
- Real fire, and injuries a pawn already has
- Night-time bonuses and plain daytime penalties from modded genes
- Where sunlight-sensitive pawns choose to wander or relax
- Pawns that are not wearing clothes on both torso and legs: they behave as in the normal game
- Save files: the mod stores nothing in them besides its own settings

## Requirements

- RimWorld 1.6 or 1.7 with Biotech. The game loads the right build automatically.
- [Harmony](https://steamcommunity.com/workshop/filedetails/?id=2009463077), loaded before this mod
- Optional: Vanilla Expanded Framework (included with Alpha Genes) for the knockout and burn protection. It is detected at runtime; the mod has no compile-time reference to it.

## Build

Needs the .NET 10 SDK. From `Source/`:

```
dotnet build
```

One build produces both versions: `1.6/Assemblies/ClothedVampires.dll` (net472, for RimWorld 1.6) and `1.7/Assemblies/ClothedVampires.dll` (net10.0, for RimWorld 1.7). `LoadFolders.xml` tells the game which one to load. Each framework builds in its own `obj/<Configuration>/<TargetFramework>/` folder, so the two builds can't overwrite each other's DLL. Harmony is not bundled; the Harmony mod supplies it.

The build also publishes the mod to `RimWorld/Mods/ClothedVampires`, copying only `About/`, `LoadFolders.xml`, `1.6/` and `1.7/`. It removes the old root `Assemblies/` folder left by 1.6-only versions.

## Install

Subscribe on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3819404247), or build it, or copy `About/`, `LoadFolders.xml`, `1.6/` and `1.7/` into `RimWorld/Mods/ClothedVampires`. Enable it in the mod list after Harmony.

## Development

Features go through [Spec Kit](https://github.com/github/spec-kit): specs, plans, tasks and in-game test records live in `specs/`. Player-facing documents (settings guide, Workshop description, update notes) live in `docs/`.

## Credits

Built with the help of Claude (Anthropic).
