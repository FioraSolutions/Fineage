# Convert L2 race actors & town NPCs (PSK/PSA → Unity)

Why Human/Elf “just work” and a fresh Orc/NPC import looks giant or tears the head: **the working assets were already baked to Unity meters in Blender**. A raw Interlude PSK is still in Unreal units (~52.5 UU ≈ 1 m). If you skip the bake (or fight it with Unity `globalScale` alone), you get exactly the bugs we hit on Immortal Plateau.

Companion doc for armor textures/meshes: [`../convert-armor/README.md`](../convert-armor/README.md).

---

## 1. The rule (non-negotiable)

Two valid pipelines — **pick one, never mix**:

### A) Meter FBX (player races after `pskimport(bScaleDown=True)`)

| Stage | Value |
|-------|--------|
| Blender pelvis Z | **0.25–0.30** |
| Unity `globalScale` | **`1`** |
| Prefab scale | **`(1,1,1)`** |

### B) Raw UU FBX (town NPCs from umodel — clergy)

| Stage | Value |
|-------|--------|
| On-disk FBX pelvis Z | **~30** (leave UU) |
| Unity `globalScale` | **`0.019`** (same as `a_common_people_MElf`) |
| Avatar | **`NoAvatar`** + Generic |
| Prefab scale | **`(1,1,1)`** |

**Never** Blender-Apply Scale on an animated UU FBX and then re-extract clips — that exploded clergy limbs (leg to infinity, NPCs fused). For pipeline B, scale is Unity `globalScale` only.

### B2) Meter town NPCs (teleporter / casino builders)

Fresh `a_teleporter_*` / `a_casino_*` imports use **pipeline A numbers** via Blender `bScaleDown=True`, then `L2TownNpcFbxImport.ConfigureMeterFbx` (`globalScale=1` + forced reimport + height assert). Do **not** YAML-flip `globalScale` while Library still holds UU verts — that makes giants.

| Asset | Pelvis Z | Pipeline |
|-------|----------|----------|
| `MOrc_anim.fbx` | ~0.25 | A |
| `.work/npc-export/FBX/e_clergyman_*` | ~30 | B |

---

## 2. Why we had to “допиливать” Orc

Working Human/Elf pipeline (see convert-armor):

1. `pskimport(..., bScaleDown=True)` → already near meters  
2. FBX export with `apply_unit_scale=True`  
3. Unity import at `globalScale: 1` (or historically inconsistent meta that still left the mesh at meters)

Orc clergy first pass:

1. PSA/PSK dumped to FBX **still in UU** (pelvis Z ≈ 31)  
2. Textures missing → “bare flesh legs”  
3. Unity `globalScale: 0.019` on meta **did not reliably reach play mode** (YAML edits while Editor open, pool `OriginalLocalScale`, etc.)  
4. Prefab `localScale = 0.019` was a band-aid and easy to double-apply after a real bake  

Player Orc face/head:

1. Anim skeleton rebuilt from a **new** meter FBX  
2. Face/hair briefly pointed at an **old** `Orc/Models/` FBX → bind mismatch  
3. A remap fallback that sent missing face bones to **Pelvis** stretched the skull open  

So the client code was not “different races need different renderers”. The **content contract** was violated: meter skeleton + UU NPC, or face FBX from a different convert pass than the anim FBX.

---

## 3. Player race recipe (MOrc / FOrc / …)

Same skeleton for **anim + armor + face + hair**.

### 3.1 Sources

- Anim: `{Race}_anim.psa` + bind mesh `{Race}_m000_b.psk`  
- Body: `{Race}_m###_{u,l,g,b}.psk`  
- Face: `{Race}_m000_f.psk`  
- Hair: `{Race}_m###_m00_{bh,ah}.psk`  

Use the patched importer: `tools/interlude/psk_import_patched.py` (Blender 5 fcurves).

### 3.2 Blender

```python
pskimport(
    filepath,
    context=bpy.context,
    bScaleDown=True,        # cm→m style; OR bake ×0.019 yourself if left False
    bDontInvertRoot=True,
    bReorientBones=False,
)
# Assert pelvis Z ~0.25–0.30 before export
```

Export FBX:

- `object_types={"ARMATURE","MESH"}`  
- `add_leaf_bones` consistent across anim/armor/face (player pieces often keep `*_end`)  
- `axis_forward="-Z"`, `axis_up="Y"`  
- `apply_unit_scale=True`  

Drop under:

```
Assets/Resources/Data/Animations/{Package}/{Race}/Models/
  {Race}_anim.fbx          # or Models/anim/
  {Race}_m001_u.fbx …
  {Race}_m000_f.fbx
  {Race}_m000_m00_bh.fbx …
```

### 3.3 Unity menus

1. `Fineage / Build Orc Race Prefabs` (or race equivalent) — materials, face/hair wrappers, containers  
2. Convert PSA → `Assets/L2ImportTmp/{Race}_anim.fbx`, then `L2MOrcAnimImporter` / race anim importer — clips + override controller + rebuild anim skeleton from `Models/anim`  

Face/hair prefabs **must** instance the FBX under `{Race}/Models/`, not a leftover flat `Orc/Models/` copy.

### 3.4 Runtime (already in client)

- `SkinnedMeshSync` remaps armor/face/hair onto the anim skeleton by bone **name** (case / `_end` / space↔underscore aliases).  
- Anim carrier mesh `*_m000_b` / `*_m000_b.ao` stays disabled (`L2ActorMeshPolicy`).  
- Lobby and world both equip weapons through `Gear.SyncEquippedWeapons` (dual fist / lrhand).

---

## 4. Town NPC recipe (e.g. `e_clergyman_MOrc_m00`)

### 4.1 Convert

1. UModel → PSK/PSA + TGA → FBX (keep UU; copy from `.work/npc-export/FBX/`)  
2. Do **not** Apply Scale in Blender on the animated file  
3. Unity: `globalScale = 0.019`, `avatarSetup = NoAvatar`, extract clips via `L2OrcNpcPrefabBuilder`  

### 4.2 Unity layout

```
Assets/Resources/Data/Animations/LineageNPCs/{mesh}/
  {mesh}.prefab
  Model/{mesh}.fbx          # globalScale: 1
  Model/*.anim              # extracted clips (mostly rotations — OK across scale bake)
Assets/Resources/Data/SysTextures/LineageNpcsTex/
  {mesh}_t00_{b00,b01,f,h}.tga
  Materials/{mesh}_t00_*.mat   # required — ModelTable loads Materials/
```

Menu: `Shnok / Characters / Build Orc Clergy NPC Prefabs` (`L2OrcNpcPrefabBuilder`) — assigns L2Actor mats, animator override, click_area.

`Npcgrp` `texture_name` like `LineageNPCsTex.e_clergyman_MOrc_m00_t00_b00` resolves via `LoadArmorMaterial` → `Data/SysTextures/LineageNpcsTex/Materials/...` (folder on disk is `LineageNpcsTex`; loader also tries case variants).

### 4.3 Do not

- Ship UU FBX and hope `globalScale: 0.019` survives  
- Mix space-bone NPC clips with underscore-bone player animators without path remap  
- Leave mats only next to the TGA (not under `Materials/`) — NPC spawns pink/bare  

---

## 5. Acceptance checklist (one test asset)

1. Blender: pelvis Z ~0.25–0.30  
2. Unity: FBX `globalScale = 1`, prefab scale `(1,1,1)`  
3. In scene next to a known-good Human/Elf: height within ~10%  
4. Player: walk cycle — face/hair stay on the neck (no hollow skull)  
5. Dual fist: both castets in **lobby and** world  
6. NPC: textured (not flesh-only), idle clip plays, click_area usable  

If (1) fails, stop — fix the FBX. Do not add more client hacks.

---

## 6. Related code

| Piece | Role |
|-------|------|
| `tools/interlude/psk_import_patched.py` | Blender 5 PSK/PSA import |
| `L2OrcRacePrefabBuilder` | Race prefabs / face-hair wrappers |
| `L2MOrcAnimImporter` | Clips + override + skeleton rebuild |
| `L2OrcNpcPrefabBuilder` | Clergy NPC prefab + mats |
| `SkinnedMeshSync` | Bone remap onto anim skeleton |
| `L2ActorMeshPolicy` | Hide `*_m000_b` carrier mesh |
| `docs/convert-armor/README.md` | Armor texture + mesh detail |
