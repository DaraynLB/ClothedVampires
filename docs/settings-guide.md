# Clothed Vampires — Player's Guide to the Settings

Everything you can change in this mod is in one settings window, with no files to edit. This guide explains what each setting does, with examples from real test games.

You can't break anything by experimenting. Every change applies in a running game without a reload or a restart, and **Reset to defaults** puts everything back.

## Where to find it

In RimWorld's main menu or in a running game: **Options → Mod Settings → Clothed Vampires**.

Hover over any setting for a short explanation.

## The idea in one paragraph

Some genes make a pawn suffer in direct sunlight: slower walking, a bad mood, and with Alpha Genes a knockout, weaker sight or burns. This mod lets **clothing** protect them. A pawn is **covered** when clothing covers both the **torso and the legs**: one item that covers both (a duster) is enough, and so is a shirt plus pants. **Underwear doesn't count**, from UnderWhere or Undergarments Core, even a one-piece set that covers torso and legs. A covered pawn in sunlight is spared the penalties you switch on below. Sunlight **bonuses** are never removed.

## What clothing protects against

Four switches, all **on** by default. Turn one off and that penalty comes back for covered pawns, exactly as in the normal game.

### Slowness in sunlight

**Choices:** on or off. **Default:** on.

Covered pawns aren't slowed down by the sun. This applies to any "in sunlight" effect that lowers walking speed, such as Biotech's UV sensitivity genes.

Example from testing: a dressed colonist with mild UV sensitivity walked at **4.51 cells/second** in the sun with this switch on, and **4.06** (the normal game's ×90%) with it off.

This switch only acts on effects that **slow** a pawn. A sunlight effect that only changes something else (say, melee) isn't touched.

### Bad mood in sunlight

**Choices:** on or off. **Default:** on.

Covered pawns don't get negative "in sunlight" mood thoughts, such as **"Sunlight sensitivity -6"** from mild UV sensitivity. Turn it off and the thought comes back.

Mood updates a little slowly in RimWorld: after a change, allow about half a minute of game time before the mood list catches up.

### Knockout and other sunlight conditions

**Choices:** on or off. **Default:** on. **Needs Vanilla Expanded Framework.**

This covers sunlight conditions from Alpha Genes (through Vanilla Expanded Framework):

| Gene | In sunlight, uncovered | In sunlight, covered |
|---|---|---|
| Vicious UV sensitivity | knocked unconscious (Consciousness 10%) | stays awake |
| Minor UV sensitivity | Sight 90% | Sight 100% |

The change happens at the game's next sunlight check, about every 8 seconds at normal speed. So a covered pawn who was knocked out gets back up shortly after, and a pawn who undresses in the sun is affected again shortly after. A gene added mid-game can show its effect until the first check.

### Burns from sunlight

**Choices:** on or off. **Default:** on. **Needs Vanilla Expanded Framework.**

Covered pawns with Alpha Genes' **lethal UV sensitivity** take no sunlight burns. Uncovered, they take permanent burns at every sunlight check, which quickly downs them.

A few things this setting never does:

- **It never stops real fire.** A covered lethal-UV pawn who catches fire still burns.
- **It never heals.** Burns taken before the pawn was covered stay.

## Which sunlight effects count

**Choices:** Genes only or Absolute. **Default:** Genes only.

- **Genes only:** clothing blocks sunlight penalties that come from **the pawn's own genes**. A sunlight condition given some other way (an implant, a drug, another mod, or added with the developer tools) still affects them.
- **Absolute:** clothing blocks **every** sunlight penalty of the kinds switched on above, whatever caused it.

Example from testing: a dressed colonist with no vicious UV gene was given the vicious UV condition directly. On **Genes only** he collapsed in the sun; on **Absolute** he got back up. Colonists whose condition came from the gene were protected in both modes.

**Bonuses are never removed, in either mode.** A dressed colonist with Alpha Genes' **UV powered** gene kept the full **×120%** walking speed in the sun on both Genes only and Absolute.

## Debug logging

**Choices:** on or off. **Default:** off.

When on, the mod writes a line to the game's log each time clothing blocks a knockout, another sunlight condition, or a sunlight burn, for example:

`[ClothedVampires DEBUG] Blocked sunlight condition 'vicious UV sensitivity' on Grae (from gene: yes)`

Slowness and mood blocks aren't logged. Leave this off for normal play; when it's off, the mod writes nothing to the log during play.

## Defaults and Reset to defaults

| Setting | Default |
|---|---|
| Slowness in sunlight | on |
| Bad mood in sunlight | on |
| Knockout and other sunlight conditions | on |
| Burns from sunlight | on |
| Which sunlight effects count | Genes only |
| Debug logging | off |

**Reset to defaults** restores this table. Your settings are kept between game sessions.

## Without Vanilla Expanded Framework

If Vanilla Expanded Framework isn't loaded, **Knockout and other sunlight conditions** and **Burns from sunlight** are shown **grayed out** with the note *"Needs Vanilla Expanded Framework (used by Alpha Genes)."* There's nothing for them to act on. Slowness, mood and the mode still work, and the mod loads without errors.

## Compatibility notes

Tested together with Alpha Genes, Vanilla Expanded Framework, Big and Small, UnderWhere, Undergarments Core and **Combat Extended** (on RimWorld 1.7 and 1.6). None of these change how the settings behave.

## Adding or removing the mod mid-colony

The mod stores nothing in your save file besides its own settings, and a saved game reloads with protection still working. If you remove the mod, sunlight penalties simply apply again in the normal way. (Removing the mod from a running colony wasn't tested.)

To turn the mod's effect off without removing it, untick all four switches: covered pawns then suffer in the sun exactly as uncovered ones do.
