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

Fineage loads `StreamingAssets/Data/Map/detailed_maps_interlude.json`. Values in that file come from Interlude `system/zonename-e.dat` (Lineage2Ver413, Interlude DDF: `coords[0..5]` + `unk01` scale + `map` texture name) and the 1024×1024 `TownMap` textures in `systextures/L2Font-e.utx`.

## Imported (this repository)

### World map chrome

| Unity Resources path | Original |
|---------------------|----------|
| `Data/UI/Map/WorldMap_Black.png` | Composed/world map (calibrated to `MapPanner` L2 bounds) |
| `Data/UI/Map/Town/btn_town_plus.png` | `L2UI_CH3.Minimap.MapButton_ZoomIn1` |
| `Data/UI/Map/Town/btn_town_plus_over.png` | `L2UI_CH3.Minimap.MapButton_ZoomIn2` |
| `Data/UI/Map/Town/btn_town_close.png` | Back control (map zoom out icon) |
| `Data/UI/Map/Town/town_pin.png` | Player pin on detailed layer |

The older 512×512 `town_map.utx` village maps remain under `Data/UI/Map/Town/town_map_*`. The Alt+M detailed layer now uses the official `L2Font-e.TownMap` art below.

### Official TownMap / dungeon maps (`Data/UI/Map/Detailed/`)

All 22 textures named in Interlude `zonename-e.dat` `map` field, exported from `L2Font-e.utx` group `TownMap` as 1024×1024 PNG (umodel `-game=l2 -export -png`). JSON `zoneIds` are the zonename `nbr` values; width/height/origin/hotspot/scale come from that dat.

| Texture | Zone id | Notes |
|---------|--------:|-------|
| `map_talkin_island_village` | 35 | Talking Island Village |
| `map_gludin_village` | 21 | |
| `map_gludio_castle_town` | 20 | |
| `map_dark_elven_village` | 107 | |
| `map_dion_castle_town` | 48 | |
| `map_floran_village` | 58 | |
| `map_town_of_oren` | 101 | |
| `map_elven_village` | 113 | |
| `map_giran_castle_town` | 43 | |
| `map_hunters_village` | 74 | |
| `map_town_of_aden` | 77 | |
| `map_heine` | 118 | |
| `map_orc_village` | 62 | |
| `map_dwarven_village` | 64 | |
| `map_rune_village` | 291 | Rune Township |
| `map_godard_castle_town` | 269 | Town of Goddard |
| `map_town_of_schuttgart` | 326 | Present in this Interlude client dat + utx |
| `dungeon_map_elven_ruin` | 32 | Elven Ruins |
| `dungeon_map_dungeon_of_trial` | 60 | Cave of Trials |
| `dungeon_map_abaodoned_mime` | 66 | Abandoned Coal Mines (original typo) |
| `dungeon_map_black_magic_laboratory` | 110 | School of Dark Arts |
| `dungeon_map_elven_underground_fortress` | 114 | Elven Fortress |

Hotspot and origin numbers previously guessed for Rune/Goddard/Heine/Orc/Dwarven were replaced with the dat. Rune is **291** (not 118 — that id is Heine); Goddard is **269** (not 291).

## Not in this Interlude client (no fallback art)

`L2Font-e.utx` has no Cruma, Antharas, catacomb, or necropolis TownMap textures. The corresponding `zonename-e.dat` rows exist (e.g. Cruma Tower ids 54–56, Antharas' Lair 45, the seven catacombs and eight necropolises) but their `map` field is empty and all six coords are `-1`. Opening Alt+M in those zones correctly stays on the world map. Do not invent placeholder textures.

Clan hall (`AgitMap.*`) and six `Minimap.int_worldmap*` tiles were not imported; Fineage still uses the composed `WorldMap_Black.png`.

## Extraction

1. Interlude install: `systextures/L2Font-e.utx`, `system/zonename-e.dat`.
2. `umodel -game=l2 -export -png -groups` on the utx (package is readable without a separate decrypt).
3. Decrypt the dat (`l2encdec -c decode -p 413 --legacy`) and parse with the Interlude DDF:

```
UINT nbr; UINT zone_color_id; UINT x_world_grid; UINT y_world_grid;
FLOAT top_z; FLOAT bottom_z; ASCF zone_name;
INT coords[6]; FLOAT unk01; ASCF map;
```

4. PNGs go in `Assets/Resources/Data/UI/Map/Detailed/`; `importedResource` in the JSON must match. Missing files log `[DetailedMap] Missing texture for zone '…'`.

Reference paths under gitignored `ref/l2-interlude-client/` when available.
