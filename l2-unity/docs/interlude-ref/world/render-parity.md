# Interlude world render parity and static performance

Issue: [#1673](https://github.com/FioraSolutions/Fineage/issues/1673).
Built on the lossless map dump from #1670 (`world/maps/*.unr.json.gz`).
Compared against Fineage Meta (`TerrainInfo0.txt`, `StaticMeshActor.json`, `.t3d`),
importers/bakers under `Assets/Scripts/Tools/Terrain` and `Assets/Editor/Terrain`,
and shipped tile assets. **No Unity session and no batchmode were run.**
UE2 property semantics come from the official actors and from Cruma's typed
TerrainInfo/DecorationLayer/StaticMeshActor readers — not from guessed lists.

Derived tables: `render-parity-tiles.tsv`, `render-perf.tsv`, `render-parity-holes.tsv`.

## Gap table

| Category | What the original does | What Fineage does | Verdict | Fix |
|---|---|---|---|---|
| Terrain heightmap scale | `TerrainInfo.TerrainScale` tagged value is the UU size of one heightmap vertex (all play-first tiles: `(128, 128, 76)`). Heightmap is 256×256 (`GeneratedSectorCounter`=256). Fineage sizes the Unity terrain as `counter * TerrainScale` (height range 256×76 UU ≈ 370.6 m). Cruma's typed `getScale().z` is `76/256=0.296875` (internal per-unit). | Parser reads `TerrainMap`, `TerrainScale`, and `Location`. On shipped tiles the tagged scale vectors match. `GeneratedSectorCounter` is hard-coded to 256 (correct here, not parsed). | **Scale matches on shipped tiles.** Confirm imported height amplitude in a later Unity pass. | Keep dump-backed scale/location. Parse `GeneratedSectorCounter`. |
| Terrain layers | Each `TerrainLayer` has Texture, AlphaMap, U/VScale, U/VPan, `TextureRotation`, `LayerRotation`, `TextureMapAxis`, `bUseAlpha`, layer `Scale`, and `TerrainMatrix`. Play-first tiles: LayerRotation is 0; one non-zero TextureRotation on `22_23` (65066 UU); TextureMapAxis ≠ 0 on `22_22` and `22_21`. | Parser keeps Texture, AlphaMap, UScale, VScale, and WPlane X/Y as UV offset; drops rotation, axis, `bUseAlpha`, and layer Scale. **Meta drops real textures** vs the official actor (e.g. Gludio `19_21` missing Obase_1; Giran `22_22` missing INNS_05/INNS_06; `22_21` missing GUS05, GIS01, 22_21_map; Dion `20_22` missing DI_S4). Empty `none` slots are also omitted. | **Partial, and Meta is incomplete outside TI.** UV scale/offset imported; missing splat textures and dropped axis/rotation are visible bugs. | Rebuild layers from the official dump (every slot, including axis/rotation), not the thinned Meta text. |
| Terrain holes | Official `QuadVisibilityBitmap` is a 256×256 (65536-bit) mask of hidden quads. Cruma typed read (payload missing from the #1670 JSON, which only has `array_size=2048`): `19_21` 1191 hidden, `22_22` 3814, `20_22` 261, `21_22` 118, `16_25` 7, `21_21`/`21_23` 49, `19_19` 25. `EdgeTurnBitmap` flips triangulation (thousands of true bits; not holes). | Parser never reads either bitmap. TI Meta has the full 2048 words (and matches official hidden counts). **Meta outside TI only stores 64 words and 0 hidden** — the bitmaps were truncated. Unity terrain is a continuous surface. | **Missing, and Meta is unusable for holes outside TI.** Giran town and Gludio should show large cutouts (moat/water/buildings) and do not. | Import the official 65536-bit mask (Cruma `UBitSet` / re-dump) and apply Unity terrain holes; do not trust the 64-word Meta. |
| DecoLayers (grass/bushes) | Each layer has StaticMesh, DensityMap, optional ColorMap/ScaleMap, `ShowOnTerrain`, `FadeoutRadius` Min/Max (UU), `DensityMultiplier` Min/Max, `MaxPerQuad`, `Seed`, `AlignToTerrain`, `RandomYaw`, `ScaleMultiplier`, lighting flags. Example on `17_25`: Grass009 fade 300–1500 UU (5.7–28.6 m), MaxPerQuad=20, density 10–50. | Parser keeps ShowOnTerrain, DensityMap, StaticMesh, and ScaleMultiplier only. `GenerateDecoLayers` writes Unity detail layers with **raw density-texture R (0–255) and no MaxPerQuad / DensityMultiplier / FadeoutRadius**. GPU instancing is used for Unity terrain details; baked `*_DecoInstances.asset` exists only on `17_25` (and the lobby). Other shipped tiles have a `*_DecoLayer.prefab` only. | **Wrong density and draw distance.** Fade and per-quad caps are ignored; density mapping is not the official multiplier. | Drive deco placement and fade from the dump; cap at MaxPerQuad; instance every shipped tile the same way. |
| Trees / SpeedTree | No `SpeedTree` (or similar) actors in these tiles. Trees/grass are DecoLayers plus StaticMeshActors (e.g. Mother Tree). | No SpeedTree importer (correct). Tree-scale meshes go through the SMA path. | **N/A for SpeedTree.** Tree meshes follow the SMA gaps below. | — |
| StaticMeshActors — counts | Every `.unr` has a concrete SMA count (see the per-tile table). `17_25` official **986**. | `StaticMeshActor.json` **984** (−2). The `.t3d` has **981**. Other tiles similarly drift. | **Count mismatch** on shipped tiles. Missing actors are a visibility bug. | Rebuild the placement table from the #1670 dump (every SMA, no t3d/JSON thinning). |
| StaticMeshActors — transform | Official: `Location`, `Rotation`/`SwayRotationOrig`, `DrawScale` (only when ≠ 1; 258 on `17_25`), `DrawScale3D` (19 on `17_25`, including X=−1 mirrors). | Interlude bake reads `.t3d` (`SwayRotationOrig`, `DrawScale`, `DrawScale3D`). JSON used by the older builder has `yaw/pitch/roll` but `L2StaticMesh` deserializes `eulerAngles` — those fields do not bind. JSON also lacks `hasDrawScale3D`, so mirrors would take the wrong axis path if that importer is used. | **Transforms mostly present in t3d Meta, fragile in JSON.** Mirrors and any actor without `SwayRotationOrig` are at risk. | One dump-backed placement path; keep DrawScale / DrawScale3D / Rotation together. |
| StaticMeshActors — lighting & materials | 835/986 SMA on `17_25` reference a `StaticMeshInstance` (baked vertex / light color). `bUnlit`, `bShadowCast`, `bStaticLighting` are authored. Skins/CullDistance are rarely overridden on these starting tiles (0 on `17_25`). 207 actors have `StepSound_*`. | `StaticMeshSunMasks.json` exists only for `16_24`, `16_25`, `17_25`. Other starting-zone tiles have no sun-mask data in the tree. SMA materials are bound by `L2InterludeMaterialBinder` to URP Lit via a shortened UE2 shader chain. Unlit / shadow / static-lighting flags are not first-class on `L2StaticMesh`. | **Vertex lighting incomplete outside three TI tiles.** Material flags are flattened. | Import `StaticMeshInstance` color streams (or sun masks) for every tile; honour bUnlit / shadow flags from the dump. |
| StaticMeshActors — LOD / cull | Lineage-specific `L2LodViewDuration` + `L2CurrentLod` on 149 actors on `17_25`; `bIgnoredRange` on 15; `LODBias` rare. `CullDistance` is not authored on these tiles (engine default). | Those properties are not parsed or applied. Visibility work (#1591, #1609, #1652) is a separate frustum/far-object path, not the official per-actor LOD. | **Official LOD is unused.** | One visibility/LOD system driven by L2Lod* / bIgnoredRange / any future CullDistance, per #1652. |
| Water | `WaterVolume` brushes (TI village 1, Gludio `19_21` 9, Elven `21_19` 3, Dion `20_22` 2, Giran town `22_22` 8, Giran south `22_23` 13, harbour `21_23` 5). Materials, panning, envmap, and shorelines live on the volume — not as StaticMeshes. | `L2InterludeWaterVolumes` builds **one flat quad at max Z** per volume, shared `L2WaterSurface.mat` (URP Lit transparent, hard-coded tint, UV / 6 m). TI tiles ship `*_Mat_Water.mat`; some towns ship `*_Water.asset`. **No water asset on Giran harbour `21_23` (5 volumes), `22_23` (13), Dion `20_22` (2), `21_21` (9), `21_22` (3), `19_19` (4).** | **Approximated where present; missing on the harbour and several town tiles.** | Build water from dump bounds and official materials; ship a volume for every official WaterVolume. |
| Sky / fog / zone | `ZoneInfo.bDistanceFog` is on. Fog end is usually 15000 UU (285.7 m); Dion `20_22` is 1500–10000; Giran `22_22` has a 500–16000 zone; `22_21` ends at 20000; `21_21` at 10000. `SkyZoneInfo` has TexU/VPanSpeed and LensFlare. `NSun`/`NMoon` are present. | Runtime `DayNightCycle`, `L2CelestialDiscs`, and `L2WorldAmbientLUT` are global. Per-tile fog start/end/color and SkyZoneInfo panner/flare are not applied from the dump. | **Global LUT, not per-zone fog.** | Apply dump-backed ZoneInfo fog and SkyZoneInfo per loaded tile (with #1671). |
| Materials | UE2 `Shader` / `Combiner` / `FinalBlend` / `TexPanner` / `TexOscillator` / `TexEnvMap` / `TexScaler` chains. 748 committed `*.props.txt` under `Resources/Data/Textures` mention TexPanner 14, TexEnvMap 4, Combiner 2, TexOscillator 1 (plus many Shader slots). | `L2Ue2ShaderProps` classifies a chain as Opaque, MaskedCutout, TranslucentWater, or HiddenCollisionOnly, then `L2InterludeMaterialBinder` assigns a URP Lit `_BaseMap`. TexPanner/Combiner/EnvMap are not executed (except a one-off `L2VillageStatuePanner`). | **Diffuse + alpha only.** Animated and combined materials are static approximations. | A single material path that implements the UE2 ops from the props/dump, not per-mesh hacks. |

## Per-tile comparison (play-first zones)

Columns: official SMA / Fineage JSON (Δ) / t3d; terrain layers official→Meta; deco official→Meta; official hidden quads → Meta bitmap words; WaterVolume → shipped asset/mat; sun masks.

### Talking Island

| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |
|---|---|---|---|---|---|---|---|---|---|
| `16_24` | yes | yes | 10→10 | 4→4 | 0→2048 words (0 hidden) | 336→336 (+0) / — | 1→mat | yes | 135 / 0 / 200 |
| `16_25` | yes | yes | 11→11 | 4→4 | 7→2048 words (7 hidden) | 754→754 (+0) / — | 2→mat | yes | 230 / 8 / 527 |
| `16_26` | **no** | n/a | 10→— | 0→— | 0→0 words (0 hidden) | 0→**no tile** | 1→no | no | 0 / 0 / 0 |
| `17_24` | yes | yes | 10→10 | 0→0 | 0→2048 words (0 hidden) | 0→no SMA meta | 1→mat | no | 0 / 0 / 0 |
| `17_25` | yes | yes | 11→11 | 4→4 | 0→2048 words (0 hidden) | 986→984 (-2) / 981 | 1→mat | yes | 149 / 29 / 835 |
| `17_26` | **no** | n/a | 10→— | 0→— | 0→0 words (0 hidden) | 0→**no tile** | 1→no | no | 0 / 0 / 0 |
| `18_25` | **no** | n/a | 10→— | 0→— | 0→0 words (0 hidden) | 0→**no tile** | 1→no | no | 0 / 0 / 0 |
| `18_26` | **no** | n/a | 10→— | 0→— | 0→0 words (0 hidden) | 0→**no tile** | 1→no | no | 0 / 0 / 0 |

- `16_24` deco: Grass009 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; grass010 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=8; grass001 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=5; grass007 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=6
- `16_24` ZoneInfo fogEnd=15000.0 color=None
- `16_25` deco: Grass009 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; grass010 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=8; grass001 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=5; grass008 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=5
- `16_25` ZoneInfo fogEnd=15000.0 color=None
- `16_26` ZoneInfo fogEnd=15000.0 color=None
- `17_24` ZoneInfo fogEnd=15000.0 color=None
- `17_25` deco: Grass009 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; grass010 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=8; grass001 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=5; grass007 fade 1000.0–2400.0 UU (45.7 m), MaxPerQuad=8
- `17_25` ZoneInfo fogEnd=15000.0 color={'r': -82, 'g': -84, 'b': -92, 'a': 0}; LevelInfo fogEnd=None color=None
- `17_26` ZoneInfo fogEnd=15000.0 color=None
- `18_25` ZoneInfo fogEnd=15000.0 color=None
- `18_26` ZoneInfo fogEnd=15000.0 color=None

### Town of Gludio

| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |
|---|---|---|---|---|---|---|---|---|---|
| `19_21` | yes | yes | 9→8 | 3→0 | 1191→64 words (0 hidden) | 1547→— (n/a) / 1546 | 9→asset | no | 520 / 18 / 908 |
| `19_19` | yes | yes | 11→9 | 3→0 | 25→64 words (0 hidden) | 1511→— (n/a) / 1511 | 4→no | no | 344 / 0 / 1167 |

- `19_21` deco: Grass009 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; Gludio_General_grass001 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=8; Gludio_General_grass014 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=7
- `19_21` ZoneInfo fogEnd=15000.0 color=None; LevelInfo fogEnd=None color=None
- `19_19` deco: Darkelf_grass002 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; grass_darkelf004 fade 1400.0–2500.0 UU (47.6 m), MaxPerQuad=8; Elf_grass_001 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20
- `19_19` ZoneInfo fogEnd=None color={'r': 83, 'g': 96, 'b': 106, 'a': 0}

### Elven Village

| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |
|---|---|---|---|---|---|---|---|---|---|
| `21_19` | yes | yes | 10→8 | 3→0 | 0→64 words (0 hidden) | 1333→— (n/a) / 1333 | 3→asset | no | 398 / 0 / 934 |
| `21_18` | yes | yes | 10→9 | 6→0 | 0→64 words (0 hidden) | 2309→— (n/a) / 2309 | 1→no | no | 72 / 0 / 2153 |
| `21_20` | yes | yes | 10→8 | 3→0 | 0→64 words (0 hidden) | 891→— (n/a) / 890 | 2→asset | no | 337 / 0 / 553 |
| `20_19` | yes | yes | 10→8 | 3→0 | 0→64 words (0 hidden) | 1112→— (n/a) / 1112 | 1→asset | no | 408 / 0 / 704 |

- `21_19` deco: Elf_grass_001 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; elf_grass002 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=9; Oren_elf_grass0055 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=9
- `21_19` ZoneInfo fogEnd=15000.0 color=None; LevelInfo fogEnd=None color=None
- `21_18` deco: Darkelf_grass002 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; grass_darkelf006 fade 1400.0–2400.0 UU (45.7 m), MaxPerQuad=7; grass_darkelf001 fade 1400.0–2400.0 UU (45.7 m), MaxPerQuad=4; oren_grass001 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=15…
- `21_18` ZoneInfo fogEnd=None color={'r': -87, 'g': -109, 'b': 126, 'a': 0}
- `21_20` deco: Elf_grass_001 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; Oren_elf_grass0011 fade 1400.0–2400.0 UU (45.7 m), MaxPerQuad=9; Oren_elf_grass0055 fade 1400.0–2400.0 UU (45.7 m), MaxPerQuad=9
- `21_20` ZoneInfo fogEnd=15000.0 color=None
- `20_19` deco: Elf_grass_001 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; elf_grass002 fade 800.0–2400.0 UU (45.7 m), MaxPerQuad=8; Oren_elf_grass001 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=8
- `20_19` ZoneInfo fogEnd=15000.0 color=None

### Town of Dion

| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |
|---|---|---|---|---|---|---|---|---|---|
| `20_22` | yes | yes | 11→9 | 4→0 | 261→64 words (0 hidden) | 1993→— (n/a) / 1993 | 2→no | no | 483 / 18 / 1509 |

- `20_22` deco: dion_grass012 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; Dion_decostone01 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=6; Dion_flower_001 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=6; dion_grass007 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=4
- `20_22` ZoneInfo fogEnd=10000.0 color=None

### Giran Castle Town / Harbour

| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |
|---|---|---|---|---|---|---|---|---|---|
| `22_22` | yes | yes | 11→7 | 3→0 | 3814→64 words (0 hidden) | 1936→— (n/a) / 1936 | 8→asset | no | 294 / 22 / 1440 |
| `22_21` | yes | yes | 11→8 | 7→0 | 0→64 words (0 hidden) | 1217→— (n/a) / 1217 | 0→no | no | 315 / 0 / 901 |
| `22_23` | yes | yes | 13→11 | 6→0 | 0→64 words (0 hidden) | 1189→— (n/a) / 1189 | 13→no | no | 100 / 0 / 833 |
| `21_21` | yes | yes | 9→8 | 3→0 | 49→64 words (0 hidden) | 4164→— (n/a) / 4164 | 9→no | no | 831 / 214 / 2530 |
| `21_22` | yes | yes | 9→8 | 6→0 | 118→64 words (0 hidden) | 1978→— (n/a) / 1978 | 3→no | no | 675 / 2 / 1302 |
| `21_23` | yes | yes | 9→8 | 5→0 | 49→64 words (0 hidden) | 2738→— (n/a) / 2738 | 5→no | no | 306 / 292 / 2428 |

- `22_22` deco: dion_grass012 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=15; Gludio_General_grass004 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=7; Dion_flower_001 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=5
- `22_22` ZoneInfo fogEnd=15000.0 color=None; ZoneInfo fogEnd=16000.0 color={'r': 57, 'g': -58, 'b': -80, 'a': 0}
- `22_21` deco: dion_grass011 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=10; dion_grass010 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=5; dion_grass017 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=3; dion_grass012 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=20…
- `22_21` ZoneInfo fogEnd=20000.0 color={'r': 121, 'g': 120, 'b': -121, 'a': 0}
- `22_23` has 0 layers with nonzero LayerRotation and 1 with nonzero TextureRotation (dropped by the parser).
- `22_23` deco: dion_grass012 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; Gludio_General_grass004 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=7; inna_grass_reed01 fade 3000.0–3500.0 UU (66.7 m), MaxPerQuad=7; inna_grass_reed02 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=3…
- `22_23` ZoneInfo fogEnd=15000.0 color=None
- `21_21` deco: dion_grass012 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=15; dion_grass008 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=3; dion_grass009 fade 1500.0–2400.0 UU (45.7 m), MaxPerQuad=2
- `21_21` ZoneInfo fogEnd=10000.0 color=None
- `21_22` deco: dion_grass018 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=7; dion_grass016 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=4; dion_grass017 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=5; mandragora fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=3…
- `21_22` ZoneInfo fogEnd=15000.0 color=None
- `21_23` deco: dion_grass012 fade 300.0–1500.0 UU (28.6 m), MaxPerQuad=20; dion_grass007 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=4; dion_grass009 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=4; Dion_decostone01 fade 1500.0–2500.0 UU (47.6 m), MaxPerQuad=6…
- `21_23` ZoneInfo fogEnd=15000.0 color=None

## Static performance estimates (no Unity timings)

These are **inventory estimates** from official actor counts and Fineage shipped assets.
They are not p50/p95 frame times. Draw-call columns assume one call per uninstanced SMA
plus one per terrain layer, deco layer, water volume, and the terrain surface.
UE `CullDistance` is not authored on these starting tiles; Lineage `L2LodViewDuration` is.

| Zone | Tile | SMA (unique meshes) | Est. draw calls uninstanced → instanced SMA | Deco layers (fade max m) | Fineage deco instancing | Sun masks | Lights |
|---|---|---|---|---|---|---|---:|
| Talking Island | `16_24` | 336 (46) | 352 → 62 | 4 (28.6,45.7,45.7,45.7) | prefab_only | yes | 0 |
| Talking Island | `16_25` | 754 (91) | 772 → 109 | 4 (28.6,45.7,45.7,45.7) | prefab_only | yes | 27 |
| Talking Island | `16_26` | 0 (0) | 12 → 12 | 0 (—) | no | no | 0 |
| Talking Island | `17_24` | 0 (0) | 12 → 12 | 0 (—) | prefab_only | no | 0 |
| Talking Island | `17_25` | 986 (190) | 1003 → 207 | 4 (28.6,45.7,45.7,45.7) | yes | yes | 58 |
| Talking Island | `17_26` | 0 (0) | 12 → 12 | 0 (—) | no | no | 0 |
| Talking Island | `18_25` | 0 (0) | 12 → 12 | 0 (—) | no | no | 0 |
| Talking Island | `18_26` | 0 (0) | 12 → 12 | 0 (—) | no | no | 0 |
| Town of Gludio | `19_21` | 1547 (281) | 1569 → 303 | 3 (28.6,47.6,47.6) | prefab_only | no | 126 |
| Town of Gludio | `19_19` | 1511 (58) | 1530 → 77 | 3 (28.6,47.6,28.6) | prefab_only | no | 1 |
| Elven Village | `21_19` | 1333 (174) | 1350 → 191 | 3 (28.6,47.6,47.6) | prefab_only | no | 25 |
| Elven Village | `21_18` | 2309 (115) | 2327 → 133 | 6 (28.6,45.7,45.7,28.6,38.1,45.7) | prefab_only | no | 2 |
| Elven Village | `21_20` | 891 (83) | 907 → 99 | 3 (28.6,45.7,45.7) | prefab_only | no | 0 |
| Elven Village | `20_19` | 1112 (82) | 1127 → 97 | 3 (28.6,45.7,45.7) | prefab_only | no | 0 |
| Town of Dion | `20_22` | 1993 (312) | 2011 → 330 | 4 (28.6,47.6,47.6,47.6) | prefab_only | no | 174 |
| Giran Castle Town / Harbour | `22_22` | 1936 (287) | 1959 → 310 | 3 (28.6,45.7,45.7) | prefab_only | no | 91 |
| Giran Castle Town / Harbour | `22_21` | 1217 (59) | 1236 → 78 | 7 (28.6,47.6,47.6,47.6,47.6,47.6,47.6) | prefab_only | no | 0 |
| Giran Castle Town / Harbour | `22_23` | 1189 (105) | 1222 → 138 | 6 (28.6,45.7,66.7,45.7,45.7,45.7) | prefab_only | no | 36 |
| Giran Castle Town / Harbour | `21_21` | 4164 (167) | 4186 → 189 | 3 (28.6,45.7,45.7) | prefab_only | no | 1749 |
| Giran Castle Town / Harbour | `21_22` | 1978 (170) | 1997 → 189 | 6 (47.6,47.6,47.6,47.6,28.6,47.6) | prefab_only | no | 132 |
| Giran Castle Town / Harbour | `21_23` | 2738 (179) | 2758 → 199 | 5 (28.6,47.6,47.6,47.6,47.6) | prefab_only | no | 2378 |

### Bottlenecks (architectural, from the data)

1. **Uninstanced StaticMeshActors** dominate the estimate. Giran `21_23` / `21_21` and Elven `21_19` have thousands of SMA. Fineage ships a `StaticMeshes.prefab` of individual objects; only deco on `17_25` is GPU-instanced via `DecoInstances`. One instancing path for SMA (grouped by mesh+material) is the fix, not per-tile hacks.
2. **Official L2 LOD is unused.** Hundreds of actors carry `L2LodViewDuration` / `L2CurrentLod` / `bIgnoredRange`. Without them, far meshes stay in the same bucket as the village street (see #1652, #1609).
3. **Deco overdraw / wrong fade.** Official fade radii are a few tens of metres; Unity detail distance or an unfaded prefab will either clip grass too early or keep it to the horizon. Density without MaxPerQuad can explode instance counts (cap is `MaxPerQuad × 256²` per layer — an upper bound, not the painted count).
4. **Lights.** Giran harbour `21_23` has 2378 official Light actors and `21_21` has 1749. Baking/sun masks exist only on three TI tiles (`16_24`, `16_25`, `17_25`).
5. **Water and materials** are cheap but wrong (one shared transparent quad / URP Lit). Not the main CPU cost; they are a parity cost.

## Unity frame-time measurements (not run)

Nikita may have the editor open on `~/Projects/Fineage`. This pass did **not** start Unity or batchmode.
When the Mac is free, measure p50/p95 frame time, draw calls, SetPass, tris, batches, CPU/GPU, and memory on this reference route, with the same quality settings, after a 10 s settle:

1. Open the worktree (or a copy), not a dirty playtest checkout you care about, in Unity 6000.3.x.
2. Enable the Stats / Profiler (CPU, GPU, Memory, Rendering) and the Frame Debugger.
3. Walk or fly the route, 20+ seconds each, recording Profiler markers:
   - Talking Island village, tile `17_25` (Cedric / harbour approach).
   - Town of Gludio, tile `19_21`.
   - Elven Village / Mother Tree, tile `21_19`.
   - Dion fields / town, tile `20_22`.
   - Giran harbour, tile `21_23`, and Giran town `22_22`.
4. Capture a screenshot of the Rendering profiler at each stop and append numbers to `render-perf.tsv` (p50/p95 ms, draw calls, SetPass, tris, used-memory).
5. Note whether GPU instancing, terrain pixel error, shadow distance, and texture streaming are on, and whether SMA are instanced or individual MeshRenderers.

## Follow-up issues

Opened from this audit and linked from [#1673](https://github.com/FioraSolutions/Fineage/issues/1673):

- [#1685](https://github.com/FioraSolutions/Fineage/issues/1685) — terrain holes (`QuadVisibilityBitmap`) not applied; Meta truncated outside TI
- #1685 follow-up: official 2048-word bitmaps are in Meta/t3d and `quadvisibility/`; the mesh converter cuts hidden quads. Shipped meshes still need the Unity re-bake in `Docs/INTERLUDE_TILE_BAKE.md`.
- [#1686](https://github.com/FioraSolutions/Fineage/issues/1686) — terrain layers: Meta drops splat textures; parser drops axis/rotation
- #1686 follow-up: official layer slots are rebuilt in Meta/t3d (axis/rotation included); missing splat PNGs imported from official `.utx`. Shipped materials still need a Unity mesh re-bake.
- [#1687](https://github.com/FioraSolutions/Fineage/issues/1687) — DecoLayers fade, density, and MaxPerQuad ignored; Meta missing outside TI
- [#1688](https://github.com/FioraSolutions/Fineage/issues/1688) — StaticMeshActor counts, transforms, vertex lighting, and L2 LOD
- [#1689](https://github.com/FioraSolutions/Fineage/issues/1689) — water is a flat shared quad and missing on the harbour and several towns
- #1689 follow-up: official WaterVolume bounds and shader names are in Meta `watervolumes.json` for every in-repo tile that has volumes (including harbour `21_23` and `22_23`). Shipped meshes/prefab children still need a Unity water bake.
- [#1690](https://github.com/FioraSolutions/Fineage/issues/1690) — UE2 world materials approximated to URP Lit diffuse
- [#1691](https://github.com/FioraSolutions/Fineage/issues/1691) — per-tile ZoneInfo fog and SkyZoneInfo not applied
- [#1692](https://github.com/FioraSolutions/Fineage/issues/1692) — starting-zone perf: uninstanced SMA, unused L2 LOD, deco fade

## Files

- `render-parity.md` (this file)
- `render-parity-tiles.tsv` — per-tile official vs Fineage counts
- `render-perf.tsv` — static draw-call / mesh / deco estimates
- `render-parity-holes.tsv` — official 256×256 QuadVisibility hidden-quad counts (Cruma typed `UBitSet`; the #1670 JSON omits the array payload)
- Regenerator for holes: `tools/interlude-extract/DumpTerrainHoles.java`
- Official actor dumps remain in `world/maps/` from #1670
- Regenerator: `tools/interlude-extract/build_render_parity.py`

