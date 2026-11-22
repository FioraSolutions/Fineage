# Login screen (Lobby.unr + login UI) — extraction audit for #932

Step 1 of the login-screen rebuild. What of the Interlude original is already
extracted losslessly and committed, what is missing, and the exact commands to
close the gaps (run on Nikita's Mac — the client lives under gitignored `ref/`,
the cloud VM has no client). Companion script: `tools/interlude-extract/extract_login_lobby.sh`.

Related: `Docs/LOBBY_INTERLUDE_SOURCES.md`, `Docs/agent-tips/login-forest-lobby.md`,
`l2-unity/docs/interlude-ref/world/README.md`, `l2-unity/docs/ui-original/README.md`.

## 1. Already extracted and committed (lossless)

### Scene

| Item | Where | Notes |
|------|-------|-------|
| `Lobby.unr` full property dump | `l2-unity/docs/interlude-ref/world/env/Lobby.unr.json.gz` | 3544 exports, 520 imports, 0 failures (Cruma `ExportClientPackages`). |
| Login camera | dump: `Camera Camera25` | Location (-8262.64, 238302.64, -6687.03), Rotation (pitch 63792, yaw 33692, roll 0), Tag `U2Viewport2`. `LevelInfo0.CameraLocationDynamic`/`CameraRotationDynamic` match it. **No `FovAngle` stored** — see gap G1. (Camera27/29 store FovAngle=90 explicitly; Camera25 does not, so it inherits the class default — unverified.) |
| Terrain | dump: `TerrainInfo TerrainInfo0` | TerrainMap `Lobby_terrain_t:Height`, scale (64,64,128), location (-16384,245760,160.65), 256 sectors, **21 TerrainLayers** with resolved texture refs (T_sland/T_oren/T_Schtgart/orc/T_Rune) + alphamaps (Lobby_terrain_t D_M_*/H_M_*...). QuadVisibilityBitmap array size present, bits NOT serialized — gap G3. |
| Deco layers | dump: 5 DecoLayers on TerrainInfo0 | Dwarf_grass001/DW_deco01, Grass009/HELF_deco01, Darkelf_grass002/DE_deco01, Delf_Grass_001/ORC_deco01, dion_grass015/Lobby_deco02_RV — densitymaps + meshes named. |
| Zones / fog | dump: ZoneInfo0/1/2, SkyZoneInfo0 | ZoneInfo0: bDistanceFog=True, raw color (-46,-48,-60) = signed-byte form of unsigned (210,208,196), start 500, end 10000. SkyZoneInfo0 at (229,262985,24471) + lens flares. |
| Lighting | dump: NMovableSunLight0, NSun, NMoon, 37 Lights | Sun brightness 70, rotation pitch -8846 yaw 25324. |
| Emitters | dump: 33 Emitters + 33 SpriteEmitters | Fully linked via `Emitters` object array, ~23 non-default props each. Also present in shared `official-world-emitters.json.gz`. |
| Static meshes | dump: 1372 StaticMeshActors | Full transforms. `StaticMeshActor73` = `L2_Lobby:lobby_back` (mossy arch) at (-9790,238363,-6520) DrawScale 4.0. 3× moonplane near skybox. No `L2_Lobby_sky_elf` actor in map. |
| Scene managers | dump: 94 SceneManagers, 136 ActionMoveCamera | Cinematic intro sequences (Elf_Wizard_Kwoman etc.) — not part of login vista. |
| Package name indexes | `docs/interlude-ref/packages/*/manifest.json` | L2_Lobby_T.utx (5 tex), l2_lobby.usx (3 meshes), l2_skies.utx (66), Lobby_terrain_t.utx, L2Font.utx (start_logo-k, start_minilogo-k). |

### Unity assets already imported

- Terrain: `TerrainData/l2_lobby_layer_0..20`, `Height.Height.bmp`, all 5 deco
  density maps, terrain layer textures, `T_texture/layer0.png`.
- Meshes: FBX `lobby_back`, `moonplane`, `L2_Lobby_sky_elf`; grass FBX for
  Dwarf_grass001, Grass009, Darkelf_grass002.
- Textures: `Jungle03.png`, `L2_Lobby_elfsky.png`, `l2_skies/moon.png`.

### Login UI

| Item | Where | Notes |
|------|-------|-------|
| Window definitions | `l2-unity/docs/ui-original/Login.json` | Recovered from `nwindow.dll` disasm (RVA evidence per field). LoginLogoWnd 256x256 TOP_CENTER (0,50), NCAuthWnd 256x108 CENTER (backTex `L2UI_CH3.LoginWnd.logon_back`, ID/PWD inputs, Login/Exit buttons), NCLoginFunctionWnd 120x138 BOTTOM_RIGHT (-34,-62) with 5 buttons (backTex `L2UI_CH3.LobbyWnd.lobby_menuback`). |
| UI textures | `l2-unity/Assets/Resources/UI/Textures/...` | `textures-missing.txt` empty — all login UI textures present. |
| Button captions | EN l10n in repo | ID/PWD/Log In/Exit/New Account/Lost Account/Options/Production Team/Replay match retail strings. sysstring-e.dat itself not decoded — gap G7 (low priority). |

## 2. Gaps (must be extracted on the Mac)

| # | Gap | Why it blocks parity | How to close |
|---|-----|----------------------|--------------|
| G1 | ~~**Engine.u / UWindow.u class defaults**~~ **CLOSED** — `scripts/Engine.u/*.uc` + `scripts/UWindow.u/*.uc` committed (e3597629). **FOV finding: `class Camera extends PlayerController`; `PlayerController.uc` defaults `FovAngle=50.0`/`DefaultFOV=50.0`/`DesiredFOV=50.0` — official login-vista FOV is 50, not the code's 35.** AmbientSoundType enum confirmed (`ASType1`: 0 Always / 1 Day / 2 Night / 3 Water). `build_lobby_bake_meta.py` resolves both from these files (error if absent) — no 'unconfirmed' WARN. | — | Done. |
| G2 | **Property dumps of the lobby packages** (L2_Lobby_T.utx, l2_lobby.usx, Lobby_terrain_t.utx, l2_skies.utx, L2Font.utx) — only name indexes exist | Material chains (Shader→Combiner→TexPanner/TexRotator) for sky/water/lobby textures are unread; terrain alphamap formats unverified. | `extract_login_lobby.sh` step: `export_client_packages.sh -- Lobby L2_Lobby_T l2_lobby Lobby_terrain_t l2_skies L2Font` |
| G3 | **Terrain QuadVisibilityBitmap/TIntMap payloads** for Lobby (bits not serialized in JSON) | Terrain holes (vista sightlines) can't bake. | `extract_login_lobby.sh` step: `DumpTerrainHoles` sidecar for Lobby → `.work/lobby-extract/quadvisibility/` |
| G4 | **BSP brushes of the skybox** (Models/Brushes/Polys export with 0 props — binary payloads) | Skybox box with L2_Lobby_sky* faces can't rebuild; sky currently absent. | `extract_login_lobby.sh` step: `BrushDump` on Lobby.unr → `.work/lobby-extract/l2_lobby/brushes.t3d` |
| G5 | **l2_skies.utx texture pixels** (L2_Lobby_sky, L2_Lobby_sky_front, L2_Lobby_back, moon, HazeRing/Cloud/StarField shaders' inputs) | No skybox faces in Unity. | **`extract_login_lobby.sh --only 6`** — no umodel: `ExportNamedTextures.java` self-decrypts the package and writes PNGs; name list derived from committed `env/l2_skies.json`; placed into `Textures/l2_skies/`. |
| G6 | **L2Font.utx start_logo-k / start_minilogo-k pixels** | Logo texture in Unity is `start_logo-EU.png` of unknown provenance. | Same step 6 → `Data/UI/Assets/Login/start_logo-k.png` (+minilogo). |
| G7 | ~~**sysstring-e.dat decode**~~ **CLOSED** — `dats/sysstring-e.tsv` committed (2031 strings; e3597629). | — | Done. |
| G8 | **9 meshes + 1 terrain alphamap**: deco grasses `dion_grass015`, `Delf_Grass_001` (field_deco_S.usx); trees `Dwarftree01/04/06`, `Orctree01-04` (rionsctgart_tree_s.usx — Lobby imports the group `tree` from **two** packages, `RionSctgart_tree_s` and `speaking_tree_s`, and these seven sit in the first); terrain alphamap PNG `Lobby_terrain_t.H_M_05` | Two of five deco layers can't spawn; vista trees missing; one human-village splat layer has no weight map. Found by `build_lobby_bake_meta.py` asset verification. | Alphamap: step 6 (no umodel). Meshes: **`extract_login_lobby.sh --only 7`** — the want-list and each mesh's package come from the Lobby.unr import chain (`lobby_missing_meshes.py`), then Docker umodel per-object + Blender pskx→FBX + `place_login_assets.py meshes --list`. umodel is the only geometry path; no official macOS arm64 build — linux/amd64 binary in Docker (repo-standard pattern). |
| G9 | ~~**Lobby bake Meta inputs not in git**~~ **CLOSED** — `tools/interlude-extract/build_lobby_bake_meta.py` generates `TerrainInfo0.txt` / `l2_lobby.t3d` / `StaticMeshActorIndividuals.txt` from the committed lossless dump (+G3/G4 sidecars), verifying every referenced asset. | — | Done in this branch. |
| G10 | **nwindow.dll second disasm pass** (ID/PWD label rects at OnPaint RVA 0x1017F390, NCLoginServerWnd rect, MiniLogo) | Label placement guesswork. | Analysis task (no Mac command); RVAs already documented in `l2-unity/docs/ui-original/README.md`. |

## 3. Exact Mac commands

One-shot (covers G1–G8):

```bash
cd ~/Projects/Fineage
L2_INTERLUDE_CLIENT_ROOT=~/Downloads/L2_Interlude_Client/Lineage\ II \
  bash tools/interlude-extract/extract_login_lobby.sh
```

Remaining textures + meshes (G5, G6, G8) only — **textures do not need umodel**
(`ExportNamedTextures.java` decrypts and writes PNGs directly); meshes go through
Docker umodel + Blender:

```bash
L2_INTERLUDE_CLIENT_ROOT=~/Downloads/L2_Interlude_Client/Lineage\ II \
  bash tools/interlude-extract/extract_login_lobby.sh --only 6,7
```

Step 7 prerequisites: Docker running; Blender on PATH; the UE Viewer linux
binary at `l2-unity/External/UEViewer/umodel` (from https://www.gildor.org/downloads —
no official macOS arm64 build exists; the linux/amd64 binary in Docker is the
supported path, same pattern as `tools/interlude/export_tile_staticmeshes.sh`).

Step 8 (camera BSP zones) is Step-3 feedstock: Lobby has three `ZoneInfo`
actors with disagreeing fog (`ZoneInfo0` 500–10000 grey, `ZoneInfo1` 300–3500,
`ZoneInfo2` 0–4000 purple), so the per-tile "pick the terrain zone" selection
is ambiguous by design. The vista fog is the zone containing Camera25, resolved
authoritatively by `ZoneAtPoint.java` walking the level BSP nodes (self-checked
against every ZoneInfo's own stored Location; hard ERROR if the node layout
fails validation) → `world/env/Lobby.camera-zones.tsv`.

Step 9 (l2_skies material chains): the skybox brushes reference
`L2_Skies.Shaders.*` (`Cloud_Final`, `HazeRing_Final`, `SkybackgroundColor` +
TexPanner/TexRotator/TexOscillator modifiers). The shared material path reads
umodel-format `.props.txt`. `L2_Lobby_T` has zero material exports and
`Lobby_terrain_t` only Palettes. Brush face textures such as
`FX_E_T.WaterSurfaceShaderSet.WaterShader01` and `Castle_Kent_t.Kent_capet_m`
are Texture PNGs, exported by step 6 into the bake's umodel layout
(`.work/lobby-extract/mesh-export/{package}/Texture/{name}.png`), not by this
material-chain step. Docker umodel export → `Textures/l2_skies/<Group>/`.

### Mac rerun: l2_lobby brush textures

The bake reads `L2_UMODEL_EXPORT_DIR` (`.work/lobby-extract/mesh-export`) and
fails before materials if a brush/BSP texture is neither already under
`Assets/Resources/Data/Textures/{package}/` nor in that export. Rerun **step 6
only** (it derives the package list from `Brushes.json` and `Lobby.brushes.t3d`).
Steps 1–5 and 7–9 stay as they are.

```bash
cd ~/Projects/Fineage
git checkout cursor/login-step3-vista-5766 && git pull
L2_INTERLUDE_CLIENT_ROOT=~/Downloads/L2_Interlude_Client/Lineage\ II \
  bash tools/interlude-extract/extract_login_lobby.sh --only 6
```

Step 6 exits 1 naming `package=` and `texture=` when a referenced name is not a
Texture in the client package (or the `.utx` is absent). Then rerun the Unity
`l2_lobby` bake (same command as the PR runbook) with that export dir:

```bash
export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
export UNITY="/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity"
FINEAGE_INTERLUDE_TILE=l2_lobby \
L2_UMODEL_EXPORT_DIR="$PWD/.work/lobby-extract/mesh-export" \
"$UNITY" -batchmode -nographics -quit -projectPath l2-unity \
  -executeMethod L2InterludeTileBatchCli.RunTileBakeCli
```

Commit the bake outputs under `Maps/l2_lobby/` and any new PNGs the bake copies
into `Assets/Resources/Data/Textures/{package}/`. Do not commit `.utx` or other
original client binaries.

Run the remaining pass in one command:

```bash
L2_INTERLUDE_CLIENT_ROOT=~/Downloads/L2_Interlude_Client/Lineage\ II \
  bash tools/interlude-extract/extract_login_lobby.sh --only 6,7,8,9
```

Then commit: new dumps under `l2-unity/docs/interlude-ref/` (world/env +
scripts/Engine.u + scripts/UWindow.u), placed assets under
`Textures/l2_skies/`, `Textures/lobby_terrain_t/H_M_05.png`,
`Data/UI/Assets/Login/start_logo-k.png` / `start_minilogo-k.png`,
`StaticMeshes/RionSctgart_tree_s/`, `StaticMeshes/field_deco_S/` (the script's
final commit hint lists exact paths). Sidecars under `.work/lobby-extract/`
(brushes.t3d, quadvisibility) are build inputs — commit as the other tiles do
(`Maps/<tile>/Meta/brushes.t3d` pattern, i.e. `Maps/l2_lobby/Meta/brushes.t3d`
after Step 2 generator runs).

## 4. Root-cause hypothesis for the broken scene (verified in code)

`LoginForestPresenter` is a hand-tuned side scene, not the shared pipeline:

- Consumes derived lossy `LoginForestActors.json` (761 actors, transforms only;
  generator not in git) instead of the lossless `Lobby.unr.json.gz` dump.
- `cam.clearFlags = SolidColor`, flat background (0.24,0.34,0.39) → the "flat
  blue-grey fill, no sky" Nikita saw. Sky meshes skipped by name heuristics.
- `ForceUnlitDirt()` replaces the real baked terrain (which HAS all 21 official
  layers + alphamaps in `TerrainData/l2_lobby_layer_*`) with beachstone1 tiled
  12x12 + mud tint. The agent-tips claim that `Lobby_terrain_t/D_M_*` are
  "near-black modulate leftovers" is a misdiagnosis: they are the alphamaps
  (splat weights), matching #1755 R8 splat work.
- ZoneInfo distance fog parsed from JSON but never applied
  (`fogEnabled: 0` in `LobbyAccountEnvironmentProfile`); the JSON fog values
  match no dumped zone — hand-made. Per-tile ZoneInfo/SkyZoneInfo rendering
  exists in the shared pipeline (#1769) and is unused by login.
- Hardcoded sun `Euler(42,-30,0)` instead of NMovableSunLight0
  (brightness 70, pitch -8846, yaw 25324).
- Name-heuristic materials (`WantsCutout` by "leaf"/"grass") with hardcoded
  tints instead of the UE2 material chain (#1767).
- `MeshLocalYawFix = 90°` + dominant-axis `RemapDrawScale` compensation hacks.

Fix direction (Steps 2-3): feed Lobby.unr through the shared world pipeline —
generate lobby bake Meta inputs from the committed lossless dump, reuse
terrain bake / material chain / zone fog / sky / emitter mechanisms, and delete
the hand-tuned presenter.

## 5. Step 3 landed (this branch) — vista on the shared pipeline

- **`LoginForestPresenter` + derived `LoginForestActors.json` deleted** (with
  `LobbyMapInterludeContract.LoginForestMetaResource`). The vista is the baked
  additive `l2_lobby` scene, not a runtime-built side scene.
- **`Cameras.prefab` Login camera = Lobby.unr Camera25 exactly**: position
  (4539.098, -127.372, -157.384), euler (9.580, 185.076, 0), vertical FOV
  38.5526 (= UE2 `FovAngle=50` horizontal at the 4:3 reference aspect),
  physical-projection off so `fieldOfView` is authoritative. Hand-placed
  ~56 m / 36°-off transform and FOV 35 are gone.
- **`L2Ue2Fov`** (`Fineage.Game/Rendering`): UE2 horizontal→Unity vertical FOV
  conversion, `OfficialHfovDeg=50`. `LoginLobbyPresentation.EnterForestLogin`
  applies it at the live aspect (Vert-); account-lobby cameras are untouched
  (their official source is the other 14 t3d cameras — follow-up, not
  invented). EditMode tests: `L2Ue2FovEditModeTests`.
- **Bake-side reproducibility**: `L2T3DInfoParser.ParseLobbyCameras` reads
  `Class=Camera` actors (Tag, FovAngle); `L2CameraBuilder.BuildLobbyCamerasFromT3d`
  builds them with default-50 inheritance logged per camera (no silent
  fallback — the t3d we generate already resolves FovAngle from Engine.u).
- **Stale scene rig deleted**: `Login_`/`CharSelect_`/`Human_`/`Elf_`/
  `DarkElf_`/`Orc_`/`Dwarf_` under the inactive scene-local `Cameras` root
  (44 scene YAML docs; reference scan proved no outside users, no code
  `GameObject.Find` on those names).
- **Proof harness without the presenter**: `LoginForestShot` captures from the
  editor poll once splash is gone and `#LoginWindow` is laid out (>200 px),
  same `/tmp/fineage-login-forest.outpath` protocol.
- Parity test now asserts `Class=Camera` / `Camera25` in the committed
  `l2_lobby.t3d` instead of the deleted JSON.

Still open (needs the Mac + Unity): `--only 6,7,8,9` assets (G5/G6/G8),
lobby re-bake including scoping the `L2BspPolyFlags.IsBrushTextureRenderable`
`L2_Skies.*` exclusion so the lobby skybox bakes, zone-fog wiring via
`SetActiveMap("Lobby")` + the explicit zone actor from the step-8
camera-zones TSV, and the side-by-side proof shot against the original.
