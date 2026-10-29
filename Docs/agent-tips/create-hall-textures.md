# Create / CharSelect hall — cyan materials

The character-create hall is the Talking Island village in `l2_lobby`, not the forest login presenter. Proof shot: `~/Desktop/fineage-empty-roster-create.png` (windmill sails, jagged cards on the ground by the pond, solid cyan hair on the two human women).

On this project, that cyan is **SpeedTree8 assigned to ordinary meshes**. It is not Unity’s magenta `Hidden/InternalErrorShader`, and `LobbyBrokenMaterialHider` will not hide it. The PNGs were already in git. This fix did **not** re-export from the Interlude client.

## Where the hall is

| What | Path |
| --- | --- |
| Scene | `l2-unity/Assets/Resources/Scenes/l2_lobby.unity` |
| Camera | `CharSelect_` near `(4543, -117, -120)` |
| Placed props | `l2-unity/Assets/Resources/Data/Maps/l2_lobby/StaticMeshes.prefab` |
| Deco instances | `l2_lobby/l2_lobby_DecoLayer.prefab` (no SpeedTree mats; leave it) |
| Inactive water planes | scene objects `Water` / `Water (1)` are `m_IsActive: 0`. Not the cyan shards |

## How to find the cyan slots

SpeedTree shader guid: `9920c1f1781549a46ba081a2a15a16ec`  
(`Library/PackageCache/com.unity.render-pipelines.universal@…/Shaders/Nature/SpeedTree8_PBRLit.shadergraph`).

1. Collect `guid: <32hex>, type: 2` from `StaticMeshes.prefab`.
2. Open each `.mat`. If `m_Shader` guid is the SpeedTree guid above, that renderer is a cyan candidate.
3. Do **not** bulk-retarget every SpeedTree mat in the repo (ships, leaves, interiors). Only the hall objects below.

`Shnok/2` (`L2MaterialBuilder.BuildTransprentMaterial`) creates that SpeedTree material for every `.props.txt` with `OutputBlending = OB_Masked`, and **deletes and recreates** those materials on the next run. Re-running Shnok/2 on these folders puts the cyan back.

## What was wrong, and what to assign

Working shaders already in the project:

| Use | Shader | Guid |
| --- | --- | --- |
| Grass cards | `L2Grass.shadergraph` | `9498d48daa06c9e47be1e46e8ef081b3` |
| Hair (same as male) | `L2/World/ActorModulate2X` | `c7e2a91b4f6d48e0a1b35d8c90e47a12` |
| Windmill sail RGB | URP Lit | `933532a4fcc9baf4fa0491de14d08ed7` |

`L2Grass` samples `_BaseMap`. Copying only `_MainTex` leaves the card untextured. Set both to the same PNG. Match `Dwarf_grass01_1.mat` (`_AlphaThreshold` 0.08, instancing on).

`ActorModulate2X` hair mat (copy `MFighter_m000_t00_m00_ah.mat`): `_ALPHATEST_ON`, `_Cull` 0, `_Cutoff` 0.498039216, `_BaseMap` and `_MainTex`, queue 2450.

### 1. Windmill sails

| | |
| --- | --- |
| Object | `Dion_SV_S.DS_Windmill_propeller` in `StaticMeshes.prefab` |
| Material | `Textures/dion_sv_t/Materials/di_s_windmillcolth_a.mat` (guid `1a9fa02031826ee4abd4a2a3f2e2bead`) |
| PNG | `Textures/dion_sv_t/di_windmillcloth.png` (guid `90083394725d594479bf4835ad845ec8`) |
| Props | `Textures/dion_sv_t/Materials/di_s_windmillcolth_a.props.txt` — `Diffuse = di_windmillcloth`, `Opacity = None`, `OB_Masked`, `TwoSided` |

The PNG RGB is the beige cloth. Its alpha is almost all 0, so alpha-clip hides the sails. Assign **opaque** URP Lit, `_Cull` 0, `_BaseMap` + `_MainTex` = that PNG. `di_windmillcloth.mat` is the same shader but was not the propeller slot, and it culls backfaces.

There is a second file `Textures/dion_sv_t/Shader/di_s_windmillcolth_a.mat`. The prefab uses the `Materials/` guid, not the `Shader/` copy.

### 2. Jagged cyan on the ground / pond

Those shards are grass cards, not the pond shader. Waterfall sheets (`talking_island_warterfallsurface*`) use `Effects/waterfall001/waterfall001.mat` (`TexAnimAlpha.shadergraph`, guid `af9325f574958b44984be0e2c256ac77`). Leave them.

| Prefab object | Material |
| --- | --- |
| `field_deco_S.grass_oren_S.Oren_elf_grass001` and `…0011` | `field_deco_T/Materials/grass_elf001_1.mat` → `grass_elf001.png` |
| `…Oren_elf_grass005` and `…0055` | `grass_elf005_1.mat` → `grass_elf005.png` |
| `…elf_grass003` | `elf_grass003_1.mat` → `elf_grass003.png` |
| `…grass_darkelf004` | `Darkelf_grass004_1.mat` → `Darkelf_grass004.png` |
| `field_deco_S.innadril_grass.inna_grass_06` | `inna_grass_06_1.mat` → `inna_grass_06.png` |

All five PNGs are under `Textures/field_deco_T/` and already have a real alpha (mostly 0, blades non-zero), same pattern as `Dwarf_grass01.png`.

### 3. Female human hair

Default create appearance is hair style 0 / color 0 (`F*_m000_t00_m00_{ah,bh}`).

| Race | Before | Texture example |
| --- | --- | --- |
| `FMagic` (all 64 `*m00_ah` / `*m00_bh`) | SpeedTree → solid cyan | `SysTextures/FMagic/FMagic_m000_t00_m00_ah_ori.png` |
| `FFighter` (all 56 `*m00_*`) | URP Lit, not SpeedTree | `SysTextures/FFighter/FFighter_m000_t00_m00_ah.png` |
| `MFighter` / `MMagic` | already `ActorModulate2X` | do not retarget |

Both female sets were rewritten to `ActorModulate2X` so they match male hair. Prefabs live in `Animations/Human/{FFighter,FMagic}/Hair/` and override the FBX renderer slot. Meshes: `Animations/Fighter/{FFighter,FMagic}/Models/*_m00_ah.fbx`. Those FBX files have UVs; the embedded Phong `material_0` diffuse is ~0.8 gray, not cyan. The cyan was the material asset’s shader.

Do not retarget armor mats (`*_t00_u.mat`, `*_g`, `*_b`, `*_l`). Only `*m00_ah.mat` and `*m00_bh.mat`.

## Interlude client — only if the PNG is actually missing

```text
~/Downloads/L2_Interlude_Client/Lineage II/
  systextures/    # FFighter, FMagic, …
  staticmeshes/
  textures/       # dion_sv_t.utx, field_deco packages, t_XX_YY.utx
  maps/
```

Env used by the tile tools: `L2_INTERLUDE_CLIENT_ROOT` (see `Docs/INTERLUDE_TILE_BAKE.md`). Raw client files stay out of git.

This hall fix did not run UModel. Use the existing pipeline only when the PNG or FBX is absent:

1. `export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"`
2. UModel export of the package (`dion_sv_t`, `field_deco`, or the systexture package) into `L2_UMODEL_EXPORT_DIR` (tile helper: `tools/interlude/export_tile_staticmeshes.sh`, then `link_export_package_dirs.py`, `pskx_to_fbx_batch.py`).
3. Copy the PNG next to the existing texture (keep the `.meta` guid if the material already points at it).
4. Do **not** run `Shnok/2` afterwards if the material was hand-retargeted. That menu recreates `OB_Masked` mats as SpeedTree8.

Import settings that match the files already in the repo: Default texture type, sRGB on, `alphaIsTransparency` 0, `alphaUsage` 1 so the alpha channel is kept for grass and hair clip. Do not import them as normal maps. Do not turn the windmill cloth material into alpha-clip unless a new export has a real opacity mask; the committed PNG’s alpha is empty.

## Gotchas

- Forest login work (`LoginForestPresenter`, uncommitted `field_deco_S/*.fbx.meta` serializer bumps) is a different tree. Stash it. Do not commit it with this hall fix.
- A Unity player build rewrites `ProjectSettings`, URP assets, and unrelated `.mat` files. Stage only the hall materials.
- `NewPhysicalSkillsState.cs` had an unused `using UnityEditorInternal` that fails any standalone compile. Player scripts must not reference `UnityEditor*`.
- Public RP macOS player: `Fineage/Build` entry `FineagePublicMacBuild` (`Assets/Editor/FineagePublicMacBuild.cs`), scripting define `FINEAGE_PUBLIC_PLAYTEST`, output `~/Desktop/Fineage-macOS-dev-create/Fineage.app`. Do not use `FineageLocalMacBuild` (`FINEAGE_LOCAL_BACKEND` → `127.0.0.1:9106`). Hosts stay `StreamingAssets/fineage-endpoint.json`: `login.fineage.evpoint.by:2106`, `game.fineage.evpoint.by:7777`.
