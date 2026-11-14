# Contract: Settings Window and Log Output

The player-visible interface for this feature. The settings guide, Workshop text and tests
must match it exactly.

## Settings window

Location: **Options → Mod Settings → Clothed Vampires**.

```text
What clothing protects against
  [x] Slowness in sunlight
  [x] Bad mood in sunlight
  [x] Knockout and other sunlight conditions     (needs Vanilla Expanded Framework)
  [x] Burns from sunlight                        (needs Vanilla Expanded Framework)

Which sunlight effects count
  (•) Genes only
  ( ) Absolute

  [ ] Debug logging

  [Reset to defaults]
```

| Control | Default | Tooltip (gist) |
|---|---|---|
| Slowness in sunlight | on | Covered pawns aren't slowed by the sun. Sunlight speed bonuses are never removed. |
| Bad mood in sunlight | on | Covered pawns don't get negative "in sunlight" mood thoughts. |
| Knockout and other sunlight conditions | on | Covered pawns aren't knocked out (vicious UV) or half-blinded (minor UV) by the sun. |
| Burns from sunlight | on | Covered pawns don't take sunlight burns (lethal UV). Real fire still burns. |
| Genes only | selected | Clothing blocks only sunlight penalties caused by the pawn's genes. |
| Absolute | — | Clothing blocks every sunlight penalty of the kinds switched on, whatever its source. |
| Debug logging | off | Logs each blocked knockout/condition and burn. Leave off for normal play. |
| Reset to defaults | — | Restores the defaults above. |

When Vanilla Expanded Framework isn't loaded: the two VEF switches are drawn grayed out
(can't be toggled) with the note "Needs Vanilla Expanded Framework (used by Alpha Genes)."
Their saved values are kept.

Covered (for every switch): torso and legs covered by clothing; underwear doesn't count.

## Log lines

Startup (always, exactly one of):

```text
[ClothedVampires] Mod loaded, Harmony patches applied.
```

plus one of:

```text
[ClothedVampires] Vanilla Expanded Framework sunlight effects found; knockout and burn protection active.
[ClothedVampires] Vanilla Expanded Framework not found; knockout and burn protection unavailable.
```

Debug (only with Debug logging on; one line per blocked check per sunlight hediff):

```text
[ClothedVampires DEBUG] Blocked sunlight condition '<hediff label>' on <pawn> (from gene: <yes|no>)
[ClothedVampires DEBUG] Blocked sunlight burn '<hediff label>' on <pawn> (from gene: <yes|no>)
```

With Debug logging off, the mod writes nothing during play. Slowness and mood blocks are
never logged.
