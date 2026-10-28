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
| `22_22` | full | Race village core (#490) |
| `22_21` | missing | West-adjacent to `22_22`; Mac bake only (see below) |
| `21_22` | missing | North-adjacent to `22_22`; Mac bake only (see below) |
| `22_23` | missing | East-adjacent to `22_22`; Mac bake only (see below) |

## Mac one-tile recipe (#476 — `22_21`, `21_22`, `22_23`, …)

Prerequisites: Interlude client on disk (not in git), `l2-unity/External/L2TerrainExtractor` built locally (Gradle; not vendored in repo), Docker (Linux UModel export) or native UModel, Blender, Unity Editor matching the project.

Set `FINEAGE_INTERLUDE_TILE` to the map square id (`XX_YY`). Examples below use `22_23` (east of `22_22`), `22_21` (west), and `21_22` (north).

```bash
export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
export FINEAGE_INTERLUDE_TILE=22_23   # or 21_22, 22_21, etc.

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

Cloud agents cannot run steps 1–4 without the client; land tooling/docs + honest manifest until a Mac bake completes.

Helper scripts live under `tools/interlude/` (`prepare_interlude_tile.py`, `export_tile_staticmeshes.sh`, `run_mac_tile_bake.sh`).

## Constraints

- Do not commit NCSoft client binaries.
- Mac/local Editor owns actual bakes; this document + Editor tooling are safe to land via PR without a client on CI/ cloud VMs.
