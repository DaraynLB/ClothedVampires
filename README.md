# Clothed Vampires

A RimWorld mod for 1.6 and 1.7. Sunlight-sensitive pawns are unaffected by the sun while they are dressed.

## What it does

A pawn counts as covered when both the torso and the legs are covered by clothing. One item that covers both (a duster, a robe) is enough, and so is a shirt plus pants. Underwear from UnderWhere or Undergarments Core does not count, even when it covers the torso and legs.

While covered, a pawn standing in sunlight ignores:

- the movement and mood penalties of the Biotech genes *mild UV sensitivity* and *intense UV sensitivity* (sanguophages and others)
- the sunlight penalties of any modded gene that uses the game's own "in sunlight" condition

## What it doesn't touch

- **Alpha Genes' vicious and lethal UV sensitivity.** These use Vanilla Expanded Framework's own sunlight effects (a knockout and burns), which clothing doesn't block yet. Support for them, with settings, is the next planned feature.
- Night-time bonuses and plain daytime penalties from modded genes
- Where sunlight-sensitive pawns choose to wander or relax
- Pawns that are not wearing clothes on both torso and legs: they behave as in the normal game
- Save files: the mod stores nothing in them

## Requirements

- RimWorld 1.6 or 1.7 with Biotech. The game loads the right build automatically.
- [Harmony](https://steamcommunity.com/workshop/filedetails/?id=2009463077), loaded before this mod

## Build

Needs the .NET 10 SDK. From `Source/`:

```
dotnet build
```

One build produces both versions: `1.6/Assemblies/ClothedVampires.dll` (net472, for RimWorld 1.6) and `1.7/Assemblies/ClothedVampires.dll` (net10.0, for RimWorld 1.7). `LoadFolders.xml` tells the game which one to load. Harmony is not bundled; the Harmony mod supplies it.

The build also publishes the mod to `RimWorld/Mods/ClothedVampires`, copying only `About/`, `LoadFolders.xml`, `1.6/` and `1.7/`. It removes the old root `Assemblies/` folder left by 1.6-only versions.

## Install

Subscribe on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3819404247), or build it, or copy `About/`, `LoadFolders.xml`, `1.6/` and `1.7/` into `RimWorld/Mods/ClothedVampires`. Enable it in the mod list after Harmony.

## Development

Features go through [Spec Kit](https://github.com/github/spec-kit): specs, plans, tasks and in-game test records live in `specs/`. Player-facing documents (Workshop description, update notes) live in `docs/`.

## Credits

Built with the help of Claude (Anthropic).
