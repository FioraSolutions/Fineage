# Interlude (C6) detailed area maps — original assets & import

## Original client behaviour (research)

Retail **Interlude** drives the Alt+M world map from `MinimapWnd` / `NCMinimapWnd` in `nwindow.dll` (layout in `interface.xdat`):

| Piece | Original location |
|-------|-------------------|
| World map tiles | `systextures/L2Font-e.utx` → `Minimap.int_worldmap1` … `int_worldmap6` (1024×1024 tiles, 2×3 grid; `Localization.ini`: `English_Minimap=L2Font-e.Minimap.int_worldmap`) |
| Detailed town/dungeon art | `systextures/L2Font-e.utx` → **`TownMap.*`** textures (e.g. `map_giran_castle_town`, `map_elven_village`) |
| Some starter villages (also) | `systextures/town_map.utx` → `town_map_elf_t00`, `town_map_orc_t00`, etc. |
| World map **"+"** zoom buttons | Hotspot positions from **`system/zonename-e.dat`** (`coords[0]`/`coords[1]` or `TownBtnLocX`/`TownBtnLocY` in newer text dumps) — pixel coords on the world map art |
| Detailed map projection | Same dat: `coords[2]`/`coords[3]` = world origin on the detailed texture, `coords[4]`/`coords[5]` = texture size (often 1024×1024), `unk01` / `TownMapScale` ≈ **0.05** |
| Zone → auto town map | Client maps **zone name id** (from zonename + `l2zonename.utx` color masks) to a detailed map index when opening the map inside a town |
| "+" / back button art | `systextures/L2UI_CH3.utx` → `Minimap.MapButton_ZoomIn1/2`, `MapButton_ZoomOut1/2` |
| Player / party pins | `L2UI.MinimapWnd.MinimapPos`, `L2UI_CH3.Minimap.minimap_party` |
| Frame chrome | `L2UI_CH3.utx` → `Minimap.MapBack` (imported as `Data/UI/Map/MinimapFrame.png`) |

**Server:** zone title still comes from the server (`ExSetCompassZoneCode`); the **map texture and projection** are client-side from `zonename-e.dat` + `L2Font-e.utx`.

Fineage replays the NC map table via `StreamingAssets/Data/Map/detailed_maps_interlude.json` (values traced from Interlude `NCMinimapWnd` research and public `arrMapInfo` dumps).

## Already in this repository

| Unity Resources path | Original |
|---------------------|----------|
| `Data/UI/Map/WorldMap_Black.png` | Composed/world map (calibrated to `MapPanner` L2 bounds) |
| `Data/UI/Map/Town/town_map_elf_t00.png` | `town_map.utx` → `town_map_elf_t00` |
| `Data/UI/Map/Town/town_map_darkelf_t00.png` | `town_map.utx` → `town_map_darkelf_t00` |
| `Data/UI/Map/Town/town_map_dwarf_t00.png` | `town_map.utx` → `town_map_dwarf_t00` |
| `Data/UI/Map/Town/town_map_orc_t00.png` | `town_map.utx` → `town_map_orc_t00` |
| `Data/UI/Map/Town/town_map_talking_t00.png` | `town_map.utx` (retail also has `L2Font-e.TownMap.map_talkin_island_village`) |
| `Data/UI/Map/Town/btn_town_plus.png` | `L2UI_CH3.Minimap.MapButton_ZoomIn1` |
| `Data/UI/Map/Town/btn_town_plus_over.png` | `L2UI_CH3.Minimap.MapButton_ZoomIn2` |
| `Data/UI/Map/Town/btn_town_close.png` | Back control (map zoom out icon) |
| `Data/UI/Map/Town/town_pin.png` | Player pin on detailed layer |

## Missing — export on a Mac from Interlude client

Import rule: PNG under `l2-unity/Assets/Resources/Data/UI/Map/Detailed/<name>.png` (no fake maps), set `importedResource` in JSON, commit. **At runtime**, missing entries log:

`[DetailedMap] Missing texture for zone '<name>' (id=…) Expected …`

### Town maps (`L2Font-e.utx`, group `TownMap`)

| Texture name | Suggested Unity path |
|--------------|---------------------|
| `map_talkin_island_village` | `Data/UI/Map/Detailed/map_talkin_island_village` |
| `map_gludin_village` | `Data/UI/Map/Detailed/map_gludin_village` |
| `map_gludio_castle_town` | `Data/UI/Map/Detailed/map_gludio_castle_town` |
| `map_dark_elven_village` | `Data/UI/Map/Detailed/map_dark_elven_village` (optional if using `town_map_darkelf_t00` only) |
| `map_dion_castle_town` | `Data/UI/Map/Detailed/map_dion_castle_town` |
| `map_floran_village` | `Data/UI/Map/Detailed/map_floran_village` |
| `map_town_of_oren` | `Data/UI/Map/Detailed/map_town_of_oren` |
| `map_elven_village` | `Data/UI/Map/Detailed/map_elven_village` |
| `map_giran_castle_town` | `Data/UI/Map/Detailed/map_giran_castle_town` |
| `map_hunters_village` | `Data/UI/Map/Detailed/map_hunters_village` |
| `map_town_of_aden` | `Data/UI/Map/Detailed/map_town_of_aden` |
| `map_heine` | `Data/UI/Map/Detailed/map_heine` |
| `map_orc_village` | `Data/UI/Map/Detailed/map_orc_village` |
| `map_dwarven_village` | `Data/UI/Map/Detailed/map_dwarven_village` |
| `map_rune_village` | `Data/UI/Map/Detailed/map_rune_village` |
| `map_godard_castle_town` | `Data/UI/Map/Detailed/map_godard_castle_town` |

Package: **`systextures/L2Font-e.utx`** (decrypt → umodel/UE viewer).

### Dungeons / catacombs / raids (same `TownMap` group; rows from `zonename-e.dat`)

Export **per texture name** referenced in `zonename-e.dat` `map` / `TownMapTex` field. Common Interlude examples (names vary by language build):

| Example texture | Notes |
|-----------------|-------|
| `dungeon_map_cruma_tower1` … `dungeon_map_cruma_tower3` | Cruma floors |
| `dungeon_map_antharas` | Lair of Antharas |
| `DUNGEON_ANT` / `dungeon_map_ant nest` | Ant Nest (see community dat examples) |
| `dungeon_map_forbidden_path` (and similar) | Catacomb / necropolis instances |

Also need **`system/zonename-e.dat`** (or equivalent text dump) merged into `detailed_maps_interlude.json` for:

- `coords[0]`/`[1]` or `TownBtnLocX`/`TownBtnLocY` — "+" on world map (`-1` = none)
- `coords[2]`/`[3]` or `TownMapX`/`TownMapY` — projection origin
- `coords[4]`/`[5]` or `TownMapWidth`/`TownMapHeight`
- `unk01` or `TownMapScale`
- Zone **color id** ↔ `textures/l2zonename.utx` 128×128 masks for auto-open inside the zone

### World map tiles (optional fidelity pass)

| Package | Textures |
|---------|----------|
| `systextures/L2Font-e.utx` | `Minimap.int_worldmap1` … `int_worldmap6` |

Fineage currently uses a single composed `WorldMap_Black.png` instead of six tiles.

## Extraction on developer Mac

1. Copy from Interlude install: `systextures/L2Font-e.utx`, `systextures/town_map.utx`, `system/zonename-e.dat`.
2. Decrypt `.utx` if needed (`l2encdec` / project toolchain).
3. Export PNGs (umodel `-game=l2` or `L2AssetPipeline` texture convert).
4. Drop into `Assets/Resources/…` and update `importedResource` in `detailed_maps_interlude.json`.

Reference paths under gitignored `ref/l2-interlude-client/` when available (`interface.xdat`, `systextures/`).