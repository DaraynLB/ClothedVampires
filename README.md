# Clothed Vampires

A RimWorld 1.6 mod. Sunlight-sensitive pawns are unaffected by the sun while they are dressed.

## What it does

A pawn counts as covered when both the torso and the legs are covered by clothing. One item that covers both (a duster, a robe) is enough, and so is a shirt plus pants. Underwear from UnderWhere or Undergarments Core does not count.

While covered, a pawn standing in sunlight ignores:

- the movement and mood penalties of the Biotech genes *mild UV sensitivity* and *intense UV sensitivity* (sanguophages and others)
- the sunlight penalties of any modded gene that uses the vanilla "in sunlight" condition, such as the darkness genes in Big and Small - Genes & More

## What it doesn't touch

- Night-time bonuses and plain daytime penalties from modded genes
- Where sunlight-sensitive pawns choose to wander or relax
- Pawns that are not wearing clothes on both torso and legs: they behave as in vanilla

## Requirements

- RimWorld 1.6 with Biotech
- [Harmony](https://steamcommunity.com/workshop/filedetails/?id=2009463077)

## Build

From `Source/`:

```
dotnet build
```

The DLL lands in `Assemblies/`, and `About/` and `Assemblies/` are copied to RimWorld's `Mods/ClothedVampires` folder.

## Install

Build it, or copy `About/` and `Assemblies/` into `RimWorld/Mods/ClothedVampires`. Enable it in the mod list after Harmony. It can be added to or removed from an existing save.

## Credits

Built with the help of Claude (Anthropic).
