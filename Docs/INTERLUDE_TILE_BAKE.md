# Interlude map tile bake (#476)

One-shot workflow to convert **local** NCSoft Interlude client map squares into Fineage Unity assets under `l2-unity/Assets/Resources/Data/Maps/{XX_YY}/`. Raw client files must **never** be committed.

Parent epic: [#71](https://github.com/FioraSolutions/Fineage/issues/71). Related: [#72](https://github.com/FioraSolutions/Fineage/issues/72), [#73](https://github.com/FioraSolutions/Fineage/issues/73).

## Local client layout (Mac example — docs only)

```text
~/Downloads/L2_Interlude_Client/Lineage II/
  maps/           # XX_YY.unr (157 Interlude maps)
  textures/       # t_XX_YY.utx height/splat packages
  staticmeshes/
  systextures/
  system/
```

Set paths in Unity: **Shnok → 0. [Interlude] Batch tile bake (#476)** (stored in `EditorPrefs`, overridable via env `L2_INTERLUDE_CLIENT_ROOT`, `L2_UMODEL_EXPORT_DIR`, etc.).

## Phase A — UModel / meta (outside Unity menus)

Per new tile `XX_YY` (repeat for each map square you add):

1. Export **`maps/XX_YY.unr`** to T3D (actors + terrain info) into  
   `Assets/Resources/Data/Maps/XX_YY/Meta/XX_YY.t3d`.
2. Export height/splat textures from **`textures/t_XX_YY.utx`** (and shared packages referenced in `TerrainInfo0.txt`) into the UModel export tree used by Shnok importers (FBX/PNG + `.props.txt`).
3. Generate/commit meta sidecars already used on TI: `TerrainInfo0.txt`, `StaticMeshActor.json`, `Brushes.json`, `XX_YY_AmbientSounds.json`, height `.bin` / normals as applicable (existing TI folders are the template).

Tools: UModel (see `L2EffectUModelClient` / `L2EffectPackageRoots` for package discovery patterns). This phase is **local-only**; cloud agents do not run it.

## Phase B — Shnok menu order (per tile, T3D workflow)

Run in this order for each `XX_YY` (matches Talking Island manual bake):

| Order | Unity menu | Purpose |
|------:|------------|---------|
| 0 | **Shnok/[Brush] Import Textures** | Import brush wall/floor PNGs from `Brushes.json` |
| 1 | **Shnok/1. [StaticMeshes] Import Textures and models** | Pick `Meta/XX_YY.t3d` or `StaticMeshActorIndividuals.txt`; copies FBX/PNG from UModel export |
| 2 | **Shnok/2. [Material] Generate staticmesh materials** | Once per batch (all props under `Assets/Resources/Data/Textures` + SysTextures) |
| 3 | **Shnok/3. [StaticMeshes] Generate staticmeshes** | Place static mesh actors from T3D into scene → save `StaticMeshes.prefab` |
| 4 | **Shnok/4. [Terrain] Generate terrain** | Heightmap + UV + deco layers → save `UnityTerrain/XX_YY.prefab` |
| 5 | **Shnok/5. [Brush] (T3D) Build brushes** | ProBuilder BSP brushes → save `Brushes.prefab` |
| 8 | **Shnok/8. [Camera] (T3D) Build cameras** | Optional interpolation points |
| 9 | **Shnok/9. [AmbientSound] (T3D) Import sounds** | Stage WAV from UModel sound export |
| 9.1 | **Shnok/9.1. [AmbientSound] (T3D) Build ambient sounds** | FMOD emitters → `XX_YY_AmbientSounds.prefab` |
| 6 | **Shnok/6. [Terrain] Convert terrain to mesh** | Collider/render mesh + splat material (`XX_YY.prefab`, `_Mesh.asset`, `_Mat.mat`) |
| 7 | **Shnok/7. [Terrain] Generate deco layer mesh** | Instanced deco → `XX_YY_DecoLayer.prefab` |

JSON-variant menus (`Shnok/[Brush] (JSON)…`, `[StaticMeshes] (JSON)…`, `[AmbientSound] (JSON)…`) are the same pipeline when meta is JSON instead of T3D.

Debug-only entries (`Shnok/[Debug]…`) are not part of the production bake.

## Phase C — Batch entrypoint (#476 tooling)

| Action | Menu |
|--------|------|
| Configure client + UModel paths, allow/deny lists, run queued steps | **Shnok/0. [Interlude] Batch tile bake (#476)…** |
| Regenerate progress manifest | **Shnok/0.1. [Interlude] Refresh tile manifest** |

The batch window runs the same Phase B steps programmatically for every `maps/XX_YY.unr` discovered under the configured client root (or repo tiles if client path is unset), filtered by allow/deny lists.

Implementation: `L2InterludeTileBakePipeline`, `L2InterludeClientSettings`, `L2InterludeTileManifest`.

## Manifest

`Docs/INTERLUDE_TILE_MANIFEST.json` lists:

- tiles seen in local client (`inClient`)
- tiles with repo folders (`inRepo`)
- `importStatus`: `missing` | `partial` | `full`
- key artifact paths detected under `Assets/Resources/Data/Maps/{tile}/`

Refresh after each batch or via **Shnok/0.1**.

## Already imported in repo

| Tile | Status | Notes |
|------|--------|--------|
| `16_24`, `16_25`, `17_24`, `17_25` | full | TI smoke |
| `16_26`, `17_26`, `18_25`, `18_26` | partial | TI shoreline (Phase A: height, TerrainInfo0 with 2048-word holes, water, ambient, t3d). Unity bake + streamed scene still required. |
| `22_22` | full | Giran town (#490) |
| `22_21` | full | West of Giran (#510) |
| `22_23` | full | East of Giran (#512) |
| `21_22` | full | North of Giran (#512) |
| `21_23` | full | Giran harbor |
| `23_22` | full | Giran castle |
| `20_22` | full | Dion |
| `21_21` | full | Northwest of Giran |
| `23_21` | full | Southwest of Giran (toward Hunter) |
| `23_23` | full | Southeast of Giran (toward Heine) |
| `21_19` | full | Elven Village (#559) — first tile with world Location + streamable scene |

## Mac one-tile recipe (#476 — 3×3 ring around `22_22`)

Prerequisites: Interlude client on disk (not in git), `tools/interlude/L2TerrainExtractor` (vendored; build with `./gradlew fatJar`) or override `L2_TERRAIN_EXTRACTOR` to a local checkout under `l2-unity/External/L2TerrainExtractor`, Docker (Linux UModel export) or native UModel, Blender, Unity Editor matching the project.

Set `FINEAGE_INTERLUDE_TILE` to the map square id (`XX_YY`). The eight neighbors of full `22_22` are `21_21` … `23_23` (excluding center). Examples below use `22_23` (east), `22_21` (west), and `21_22` (north); the same env applies to `21_21`, `21_23`, `23_21`, `23_22`, and `23_23`.

```bash
export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
export FINEAGE_INTERLUDE_TILE=22_23   # or 21_21, 21_22, 21_23, 22_21, 23_21, 23_22, 23_23, etc.

# Phase A + mesh export + Unity batch (or run steps individually)
chmod +x tools/interlude/run_mac_tile_bake.sh
./tools/interlude/run_mac_tile_bake.sh all
```

Equivalent manual steps (same as #490 / `22_22`; substitute `XX_YY` / `T_XX_YY`):

1. **L2TerrainExtractor** — output to `.work/interlude-extract/XX_YY/` (`XX_YY_metadata.txt`, height raw, splat PNGs, `staticmeshes.json`).
2. **`python3 tools/interlude/prepare_interlude_tile.py XX_YY`** — writes `Maps/XX_YY/Meta/*` and `Textures/T_XX_YY/*`.
3. **`export_tile_staticmeshes.sh`** + **`link_export_package_dirs.py`** + **`pskx_to_fbx_batch.py`** — populate `L2_UMODEL_EXPORT_DIR` (default `.work/umodel-export-XX_YY`).
4. **Unity batchmode** — `FINEAGE_INTERLUDE_TILE=XX_YY`, `L2_INTERLUDE_CLIENT_ROOT`, `L2_UMODEL_EXPORT_DIR`; `-executeMethod L2InterludeTileBatchCli.RunTileBakeCli`.
5. Commit `Assets/Resources/Data/Maps/XX_YY/`, new textures under `T_XX_YY/`, any new static mesh FBX; run **Shnok/0.1** or rely on batch CLI manifest write.

**`22_23` check:** client needs `maps/22_23.unr` and `textures/t_22_23.utx`; after bake, manifest entry `22_23` should move to `full` like `22_22`.

**`21_22` check:** client needs `maps/21_22.unr` and `textures/t_21_22.utx`; after bake, manifest entry `21_22` should move to `full` like `22_22`.

**`22_21` check:** client needs `maps/22_21.unr` and `textures/t_22_21.utx`; after bake, manifest entry `22_21` should move to `full` like `22_22`.

**`21_21` check:** client needs `maps/21_21.unr` and `textures/t_21_21.utx`; after bake, manifest entry `21_21` should move to `full` like `22_22`.

**`21_23` check:** client needs `maps/21_23.unr` and `textures/t_21_23.utx`; after bake, manifest entry `21_23` should move to `full` like `22_22`.

**`23_21` check:** client needs `maps/23_21.unr` and `textures/t_23_21.utx`; after bake, manifest entry `23_21` should move to `full` like `22_22`.

**`23_22` check:** client needs `maps/23_22.unr` and `textures/t_23_22.utx`; after bake, manifest entry `23_22` should move to `full` like `22_22`.

**`23_23` check:** client needs `maps/23_23.unr` and `textures/t_23_23.utx`; after bake, manifest entry `23_23` should move to `full` like `22_22`.

Cloud agents cannot run steps 1–4 without the client; land tooling/docs + honest manifest until a Mac bake completes.

Helper scripts live under `tools/interlude/` (`prepare_interlude_tile.py`, `export_tile_staticmeshes.sh`, `run_mac_tile_bake.sh`).

## Constraints

- Do not commit NCSoft client binaries.
- Mac/local Editor owns actual bakes; this document + Editor tooling are safe to land via PR without a client on CI/ cloud VMs.

## World placement + streaming scene (Elven Village `21_19`)

`prepare_interlude_tile.py` now writes `GeneratedSectorCounter` and `Location` (from the extractor metadata) into `TerrainInfo0` / T3D. Without them the terrain bakes at the Unity origin instead of its world square — **tiles baked before this fix (Giran ring, Dion) sit at origin and need a re-bake**.

Streaming in Play Mode needs `Assets/Resources/Scenes/XX_YY.unity` in Build Settings (SceneLoader uses `Application.CanStreamedLevelBeLoaded`). Generate it after the bake:

```bash
FINEAGE_INTERLUDE_TILES=21_19 Unity -batchmode -nographics -quit -projectPath l2-unity \
  -executeMethod L2InterludeTileSceneCli.RunCli
```

Extractor note: run the jar directly (`java -jar tools/interlude/L2TerrainExtractor/build/libs/l2terrain-1.0.0-all.jar …`); each tile dir always gets `staticmeshes.json` (empty list when the map has no StaticMeshActors). Ground textures are resolved from regional `T_*.utx` packages then from per-tile `t_XX_YY.utx` when still missing.

## One-click auto-bake (Editor, GPU) — `Shnok/0.0. [Interlude] Auto-bake tiles (Elven Village)…`

Run **inside the open Unity Editor** (terrain Texture2DArrays need a GPU; batchmode `-nographics` produces 0xCD garbage).
Default tiles: `20_19,20_20,21_19,21_20` (buttons: "All with t3d", "All .unr in client").
Per tile: Phase A (`run_mac_tile_bake.sh phase-a`, uses the prebuilt `L2TerrainExtractor/build/libs/*-all.jar`, one pass extracts all tiles into `.work/interlude-extract/`)
→ Meshes (`run_mac_tile_bake.sh meshes`: umodel in Docker + Blender psk→fbx into `.work/umodel-export-XX_YY`, only when FBX coverage < 100%)
→ Unity `DefaultSteps` (+materials once) → `L2InterludeTileSceneCli.BuildScene` → diagnostics (t3d StaticMeshActors vs placed MeshFilters, missing FBX per package, alphamap garbage check, `_Splatmaps`, ground collider).
Prereqs: Docker Desktop running, Blender, Java 17+, client at `~/Downloads/L2_Interlude_Client/Lineage II`. Generated Maps/Scenes stay local (no LFS).


## TI shoreline tiles `16_26`, `17_26`, `18_25`, `18_26` (#1673)

Phase A (extractor + `prepare_interlude_tile.py` + `write_ambient_sounds_json.py`) is in the repo: height BMP, `TerrainInfo0.txt` with official Location / 2048-word `QuadVisibilityBitmap`, `watervolumes.json`, 291 ambient objects, and t3d brushes. These maps have **0 StaticMeshActors**, **0 DecoLayers**, and **1 WaterVolume** each (same pattern as `17_24`).

Heightmap preview (no Unity): `l2-unity/docs/interlude-ref/world/ti-shore-tiles-heightmaps.png`.

Remaining Unity bake (terrain mesh, deco prefab, streamable scene, EditorBuildSettings). **Do not use `-nographics`** (Texture2DArrays come out as 0xCD garbage). Close any other Unity Editor first (`pgrep Unity`), and run against this worktree — never `~/Projects/Fineage`:

```bash
export FINEAGE_INTERLUDE_TILES=16_26,17_26,18_25,18_26
/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath "$(pwd)/l2-unity" \
  -executeMethod L2InterludeTileBatchCli.RunTerrainExitCli \
  -logFile .work/unity-ti-shore-bake.log
```

`RunTerrainExitCli` matches the `17_24` shoreline shape (terrain + brushes + mesh + deco + scene) and skips the global material / static-mesh import (these tiles have no SMA). After prefabs exist, Shnok 0.4 can build water surfaces from `Maps/<tile>/Meta/watervolumes.json`, and Shnok 9.1 can build ambient prefabs from `*_AmbientSounds.json`.


## Terrain holes (`QuadVisibilityBitmap`) (#1685)

Official Interlude tiles have a 256×256 visibility mask. Phase A now stores the
full 2048-word bitmap in `Meta/TerrainInfo0.txt` and in the TerrainInfo actor of
`Meta/<tile>.t3d` (do not use the old 64-word all-visible truncation). Official
words live in `l2-unity/docs/interlude-ref/world/quadvisibility/`.

The bake parser reads the bitmap and `ConvertTerrainToMesh` skips hidden quads
on the heightmap mesh (not the safenet). Use 256 mesh subdivisions so the cut
matches the official grid 1:1. `16_24` / `16_25` / `17_24` have no t3d;
`LoadMetadata` falls back to `TerrainInfo0.txt`.

Shipped meshes stay continuous until Unity re-bakes. Close any other Unity
Editor first (`pgrep Unity`), do **not** use `-nographics`, and run against
this worktree — never `~/Projects/Fineage`. Tiles with hidden quads:

`16_25` (7), `17_21` (36), `17_22` (2390), `18_14` (108), `19_18` (49),
`19_19` (25), `19_21` (1191), `20_17` (33), `20_18` (4172), `20_20` (28),
`20_22` (261), `21_21` (49), `21_22` (118), `21_23` (49), `22_22` (3814),
`23_12` (29), `23_13` (10), `23_21` (3), `23_22` (294), `23_23` (49),
`24_12` (21).

```bash
export FINEAGE_INTERLUDE_TILES=16_25,17_21,17_22,18_14,19_18,19_19,19_21,20_17,20_18,20_20,20_22,21_21,21_22,21_23,22_22,23_12,23_13,23_21,23_22,23_23,24_12
/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath "$(pwd)/l2-unity" \
  -executeMethod L2TerrainMeshCli.RunCli \
  -logFile .work/unity-terrain-holes-1685.log
```


## Terrain deco layers

UE2 does not store deco instances. The bake writes `Assets/Resources/Data/Maps/<tile>/<tile>_Deco.bytes`: DecoLayer scalars plus RLE density/scale/color maps (`L2DC`). Do not set `convertDecoLayer` onto the old YAML / prefab path. Deco-only does not rebuild terrain meshes and does not need `-nographics` off for a GPU blit (it reads the PNG on the CPU). `FINEAGE_TERRAIN_MESH=1` together with `FINEAGE_DECO_LAYER=1` also writes the heightmap mesh and then does need a GPU.

Close any other Unity Editor first. Run from the Fineage repo, not a second checkout. The 41 shipped tiles:

```bash
export FINEAGE_DECO_LAYER=1
export FINEAGE_INTERLUDE_TILES=16_24,16_25,16_26,17_21,17_22,17_24,17_25,17_26,18_13,18_14,18_15,18_25,18_26,19_14,19_15,19_17,19_18,19_19,19_21,20_17,20_18,20_19,20_20,20_22,21_18,21_19,21_20,21_21,21_22,21_23,22_21,22_22,22_23,23_11,23_12,23_13,23_21,23_22,23_23,24_11,24_12
"/Applications/Unity/Unity.app/Contents/MacOS/Unity" \
  -batchmode \
  -projectPath "$(pwd)/l2-unity" \
  -executeMethod L2TerrainMeshCli.RunCli \
  -logFile .work/unity-deco-archive.log
```

A non-zero exit means at least one tile logged a missing official field (tile id and layer index). Those bytes are not a stand-in for the missing layer.


## Terrain layers (#1686)

Rebuild official `TerrainLayer` slots (texture, alphamap, U/VScale, UPan/VPan, `TextureRotation`, `TextureMapAxis`, `bUseAlpha`, layer Scale) from the #1670 map dump:

```bash
python3 tools/interlude-extract/rebuild_terrain_layers.py \
  --export-dir .work/tex-export
```

The bake parser reads those fields and skips `Texture=None` slots (no silent Base substitution). `_Layer_N_Xform` carries rotation (UE rotator units) and axis into `L2_TerrainLayerUv`. `T_themepark.17_16BG` is referenced on `19_17` but the official `T_themepark.utx` was not in this client tree — bake logs a missing-texture error for that layer.

Shipped meshes/materials still need Unity `L2TerrainMeshCli.RunCli` (no `-nographics`) on every in-repo tile so the new layers paint. Close other Unity instances first; use this worktree, not `~/Projects/Fineage`.


## Water volumes (#1689)

Official `WaterVolume` bounds and the map's water Shader/FinalBlend are written to `Maps/<tile>/Meta/watervolumes.json`:

```bash
# build the fat jar once (do not commit it)
cd tools/interlude/L2TerrainExtractor && gradle fatJar
python3 tools/interlude-extract/rebuild_water_volumes.py \
  --maps-dir "$L2_CLIENT/maps" \
  --jar tools/interlude/L2TerrainExtractor/build/libs/l2terrain-1.0.0-all.jar
```

`WaterVolumeDump.java` reads official brush bounding boxes. The #1670 map dump supplies the official shader name (almost always `FX_E_T.WaterSurfaceShaderSet.WaterShader01`; `21_22` uses `ssq_water_01sh`). Volume brushes are often untextured physics boxes — do not invent a shared tint if the official Unity material is missing; bake logs an error.

Shipped `*_Water.asset` / prefab children still need Unity (editor closed, no `-nographics`, this worktree not `~/Projects/Fineage`):

```bash
export FINEAGE_INTERLUDE_TILES=19_19,20_22,21_21,21_22,21_23,22_23,23_22,23_23
# plus every other in-repo tile that has Meta/watervolumes.json
# Unity 6000.3.27f1 -batchmode -quit -projectPath <worktree>/l2-unity
# -executeMethod L2InterludeWaterVolumes.RunCli
# -logFile .work/unity-water-volumes-1689.log
```

Tiles without `StaticMeshes.prefab` (the TI shoreline set) must run `L2InterludeTileBatchCli.RunTerrainExitCli` first.


## Deco layers (#1687)

Rebuild official `DecoLayers` (StaticMesh, DensityMap, FadeoutRadius, DensityMultiplier, MaxPerQuad, Seed, AlignToTerrain, RandomYaw, lighting flags) from the #1670 map dump:

```bash
python3 tools/interlude-extract/rebuild_deco_layers.py \
  --export-dir .work/deco-export
```

Density maps are official PNGs from `L2DecoLayer.utx` / `L2DecoLayer_aden.utx` / `Primitive_FieldOBJ_T.utx`. The bake parser reads fade/density/MaxPerQuad. `GenerateDecoLayers` and `DecoToMesh` remap the density-map sample through DensityMultiplier and cap at MaxPerQuad (no raw 0-255, no uncapped scatter). Missing density maps or StaticMeshes log an error. Per-layer fade is stored in UU; Unity terrain details still share one global draw distance.

Shipped `*_DecoLayer.prefab` / `*_DecoInstances.asset` still need a Unity tile bake (no `-nographics`, editor closed, this worktree not `~/Projects/Fineage`):

```bash
# included in the terrain mesh / tile-exit bake for each in-repo tile that has DecoLayers
export FINEAGE_INTERLUDE_TILES=17_21,17_22,18_14,19_14,19_15,19_18,19_19,19_21,20_17,20_18,20_19,20_20,20_22,21_18,21_19,21_20,21_21,21_22,21_23,22_21,22_22,22_23,23_11,23_12,23_13,23_21,23_22,23_23,24_11,24_12
# Unity 6000.3.27f1 -batchmode -quit -projectPath <worktree>/l2-unity
# -executeMethod L2TerrainMeshCli.RunCli
# -logFile .work/unity-deco-layers-1687.log
```
