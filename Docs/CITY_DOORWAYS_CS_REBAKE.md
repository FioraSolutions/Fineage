# City doorways — BSP CSG + static mesh materials (rebake)

## Root causes (Elven Village pilot)

1. **BSP:** `L2BrushBuilder.BuildMerged` emitted **every** `CSG_Subtract` brush polygon as opaque world geometry (~2700+ solid polys on `20_20`). In UE2, subtract brushes only carve; they are not part of the visible BSP. AntiPortal caps on **add** brushes mark doorway holes; those must punch coplanar wall faces.
2. **Static meshes:** Doorway blockers on meshes such as `O_E_GDS_Main` use UE2 shader slots with **no diffuse** (e.g. `034_shader`) — visible only as collision in the client. Unity imported them as URP Lit with no alpha clip, so faces read as solid walls. The old Blender hack (`tools/interlude/fixtures/elven_village/OLD_HACK_punch_gds_main_doorway.py`) is **not** used in the pipeline.
3. **Bridge railings:** Mats like `elf_vi_0002` need **AlphaTest cutout** from umodel `*.props.txt`. A name-based “force opaque” override masked masked railings as solid walls.

## Fix (generic)

- `Fineage.TerrainBsp`: ordered brush parse, portal cap collection, subtract AABB carving (`L2BspCsgCarver`).
- `L2BrushBuilder`: render **CSG_Add** only; carve with subtract volumes + AntiPortal rects; skip `PF_Invisible` / `PF_NotSolid` / `PF_Portal`; collision mesh omits non-solid polys.
- `L2Ue2ShaderProps` + `L2InterludeMaterialBinder`: props-driven opaque / masked cutout / water / **collision-only** (`L2/World/CollisionOnly` shader).

## Rebake on Mac (starting villages / Elven tiles)

Prereqs: `L2_INTERLUDE_CLIENT_ROOT` → retail Lineage II folder (`maps/`, `textures/`, `staticmeshes/`).

```bash
export L2_INTERLUDE_CLIENT_ROOT="/path/to/Lineage2"
cd /path/to/Fineage

# Full pipeline for one tile (extract → umodel meshes → Unity Shnok bake):
./tools/interlude/run_mac_tile_bake.sh all 20_19
./tools/interlude/run_mac_tile_bake.sh all 20_20
./tools/interlude/run_mac_tile_bake.sh all 21_19
./tools/interlude/run_mac_tile_bake.sh all 21_20

# Or phases:
./tools/interlude/run_mac_tile_bake.sh phase-a 20_20   # BrushDump + staticmeshes.json + height
./tools/interlude/run_mac_tile_bake.sh meshes 20_20    # umodel + FBX
./tools/interlude/run_mac_tile_bake.sh unity 20_20       # Unity batchmode L2InterludeTileBakePipeline
```

After mesh export, in Unity (or batch bake menu path):

1. **Shnok/0.1. [Interlude] Bind StaticMesh textures** — runs `L2InterludeMaterialBinder.Bind` (cutout + collision-only).
2. **Shnok/5. [Brush] (T3D)** or full tile bake — rebuilds `Brushes.prefab` with CSG carving.

Batch entry used by `run_mac_tile_bake.sh unity`: `L2InterludeBrushesCli` / `L2InterludeTileBakePipeline` with `BuildBrushes` + `ImportStaticMeshes` + material steps.

## Tests

- **CI `build-and-test`:** `python3 scripts/check-elven-bsp-csg-fixtures.py`
- **Unity EditMode:** `ElvenVillageBspCsgEditModeTests` (fixtures under `tools/interlude/fixtures/elven_village/`)

Do **not** commit client `.umx` / `.u` / `.utx` binaries; fixtures are brush T3D dumps only.