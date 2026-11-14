# Data Model: Dual 1.6/1.7 Support and Documentation Baseline

This feature has no runtime data: nothing is saved, and there are no settings. The
"entities" are the packaging pieces from the spec.

## Version build

| Field | 1.6 | 1.7 |
|---|---|---|
| Target framework | `net472` | `net10.0` |
| Game reference package | `Krafs.Rimworld.Ref` 1.6.* | `Krafs.Rimworld.Ref` 1.7.5005-unstable |
| Output folder | `1.6/Assemblies/` | `1.7/Assemblies/` |
| Allowed contents | `ClothedVampires.dll` only | `ClothedVampires.dll` only |

Rules: same source files for both; no `0Harmony.dll`, `.pdb` or `.deps.json`.

## Load-folder map (`LoadFolders.xml`)

| Game version | Folders loaded, in order |
|---|---|
| 1.6 | `/`, `1.6` |
| 1.7 | `/`, `1.7` |

Rule: `/` holds no assemblies (no root `Assemblies/`).

## Release documents

| Document | Audience | Created when | Source of truth |
|---|---|---|---|
| `docs/steam-workshop-description.txt` | Workshop visitors | after testing | quickstart results |
| `docs/updates/YYYY-MM-DD-1-7-support.md` | the customer | after testing, dated that day | quickstart results |
| `README.md` (updated) | developers | with the build change | the new layout |

Rule: every claim traces to a recorded test result, or is labelled untested (FR-013).
