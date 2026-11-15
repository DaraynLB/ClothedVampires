# Clothed Vampires — Progress Update, November 14 (evening)

## What we're building

This is the fix for the problem you reported: your vampire in a burka was still overcome by the sun. The cause was Alpha Genes' **vicious UV sensitivity**, which knocks a pawn unconscious in sunlight through Vanilla Expanded Framework, something Clothed Vampires had never covered.

Now clothing protects against Alpha Genes' sunlight effects too:

- **Vicious UV sensitivity:** a dressed pawn stays awake in the sun.
- **Minor UV sensitivity:** a dressed pawn keeps full sight.
- **Lethal UV sensitivity:** a dressed pawn takes no sunlight burns.

And the mod has a settings window for the first time, with the switches you asked for.

## What we tested

Everything below ran on **RimWorld 1.7** with your full mod list, including Alpha Genes, Vanilla Expanded Framework, Big and Small, both underwear mods and Combat Extended, with the mod's debug logging on so every blocked knockout and burn showed in the log.

**Your scenario.** Lazy, given vicious UV sensitivity and fully dressed, spent an in-game hour in full daylight. The log shows the knockout blocked at every sunlight check, and her Consciousness stayed at 100%. Take her pants off and she collapsed straight away.

**Underwear still doesn't count.** Andy, wearing pants but only Undergarments Core underwear on his torso, collapsed in the sun. (He got back up once we put a roof over his pen, which confirmed it really was the sunlight.)

**Lethal UV.** Axle, dressed, took no burns for an in-game hour. Without his pants he took two sunlight burns within about half a minute; once he put them back on, the burns stopped and the old ones stayed. Then we set him on fire while dressed: the fire burned him normally, while the mod kept blocking only the sunlight.

**Minor UV.** Apicia, dressed, kept 100% sight; without pants it dropped to 90%; dressed again, back to 100%.

**The switches.** We turned each switch off and on in a running game. With knockout off, Grae (dressed, vicious UV) collapsed and Apicia's sight dropped; switched back on, Grae got back up. With burns off, Axle took a sunlight burn within seconds. With slowness off, Dixon (dressed, mild UV) slowed from 4.51 to 4.06 cells/second; with mood off, his "Sunlight sensitivity -6" came back. Each came right again when switched back on.

**Genes only vs Absolute.** We gave Jake the vicious UV *condition* without the gene. On Genes only he collapsed; on Absolute he got back up.

**Bonuses kept.** Jake also has Alpha Genes' **UV powered** gene: dressed in the sun, he kept his full ×120% speed in both modes.

**Settings.** The window matched the design. Settings survived a full restart, Reset to defaults works, and with debug logging off the mod wrote nothing to the log.

**Without Vanilla Expanded Framework.** With only Harmony, the official expansions and this mod, it loaded cleanly and showed the knockout and burns switches grayed out with a note.

**RimWorld 1.6.** On your 1.6 mod list (with Combat Extended), a dressed vicious-UV colonist, Sev, stayed awake in the sun.

## Issues we found (and fixed)

- **The mod was removing good sunlight effects too.** While planning, we found that Alpha Genes' UV powered gene (×120% speed in the sun) uses the same game mechanism as the sunlight penalties, and the version you have now removes that bonus from dressed pawns. Now the mod only ever removes penalties. We found this by reading the code; we didn't test the old version in a game, but the current release would have done it for any dressed UV-powered pawn.
- **The first 1.6 test failed.** On 1.6 the mod didn't load, because the 1.6 folder held the 1.7 version of the mod. The two versions were being built in the same temporary folder and one could overwrite the other. Each now builds in its own folder; we rebuilt several times to confirm, and the 1.6 test then passed. This only affected the test build: the version on the Workshop is fine. (GitHub issue #2.)
- **A few test-procedure surprises,** none of them bugs: the mood list takes about half a minute to update; walls without a roof don't block the sun; and genes like mild UV and UV powered don't show in the Health tab, only in the genes window.

## Decisions you made

- **One switch for knockout and sight loss,** called "Knockout and other sunlight conditions", since both come from the same kind of effect.
- **Debug logging,** off by default, so tests could be proven from the log.
- **Penalties only:** clothing never removes a sunlight bonus, in either mode. Slowness counts only effects that slow a pawn; mood only bad moods; conditions only those that weaken a capacity; burns always.
- **Test with your full mod list,** plus a quick load with almost no mods and a 1.6 check.

## Where this leaves it

All of the above works on RimWorld 1.7 and 1.6, and the fix for the 1.6 build problem is in. It's ready to publish once you've had a look.

Not tested this round:

- Turning all four switches off **at the same time** (each was tested on its own).
- A naked or underwear-only colonist with the plain **mild** UV gene; the same checks passed last round and the clothing check hasn't changed.
- Removing the mod from a running colony.
