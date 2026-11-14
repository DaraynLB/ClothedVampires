# Contract: Published Mod Layout

What RimWorld, RimSort and the Steam Workshop uploader see in
`<RimWorld>/Mods/ClothedVampires/` after `dotnet build` from `Source/`. This is the
mod's external interface for this feature.

## Required

```text
Mods/ClothedVampires/
├── About/
│   └── About.xml
├── LoadFolders.xml
├── 1.6/
│   └── Assemblies/
│       └── ClothedVampires.dll     # net472
└── 1.7/
    └── Assemblies/
        └── ClothedVampires.dll     # net10.0
```

## Forbidden

- `Assemblies/` at the root (removed by the deploy step if left from the 1.6-only version)
- `0Harmony.dll` anywhere
- `*.pdb`, `*.deps.json` anywhere
- `Source/`, `docs/`, `specs/`, `.git/`, `.claude/`, `.specify/`, `README.md`

## About.xml

| Element | Value |
|---|---|
| `packageId` | `daraynlb.clothedvampires` (unchanged) |
| `name` | `Clothed Vampires` (unchanged) |
| `author` | `DaraynLB` (unchanged) |
| `supportedVersions` | `1.6`, `1.7` |
| `modDependencies` | `brrainz.harmony` (unchanged) |
| `loadAfter` | `brrainz.harmony` (unchanged) |

## LoadFolders.xml

```xml
<loadFolders>
  <v1.6><li>/</li><li>1.6</li></v1.6>
  <v1.7><li>/</li><li>1.7</li></v1.7>
</loadFolders>
```

## Log contract

On load, each version logs exactly one line
`[ClothedVampires] Mod loaded, Harmony patches applied.` and no ERROR or WARN line
mentioning `ClothedVampires`.
