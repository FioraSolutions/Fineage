# Character meter-scale migration — hand-off spec

Audience: implementation agent. Follow it literally. Do not improvise a new pipeline.

Status: the recipe below was **measured** in Unity 6000.3.27f1 batchmode on this repo
(2026-11-15). Numbers in "Reference values" are real outputs, not estimates.

---

## 0. What is wrong today

For every character/armor/face/hair FBX Unity reports:

```
globalScale = 0.019
useFileScale = false
```

and the FBX itself carries `Lcl Scaling = 100` on the `.ao` node.
So the effective chain for a bone is:

```
FBX local position  x100 (node scale)  x0.019 (importer)  = world
```

Consequences:

* every bone in every character prefab has `lossyScale = 100`;
* a meter weapon (its own FBX is `UnitScaleFactor=100`, node scale 1) has to be
  shrunk on attach so it does not become 100x:
  * runtime: `AbstractMeshManager.Mesh.Core.cs` → `CompensateMeterWeaponAgainstAnimScale()` forces `0.01`;
  * baked: `L2WeaponPrefabBuilder.CharacterAnimScaleCompensation = 0.01f`.

That 0.01 is a crutch over a broken unit contract, not a design.

## 1. Target contract

One contract for characters and weapons:

| | FBX | Unity `ModelImporter` | node scale in prefab |
|---|---|---|---|
| weapon (already done) | `UnitScaleFactor = 100` | `globalScale = 1`, `useFileScale = true` | 1 |
| character / armor / face / hair (target) | `UnitScaleFactor = 100` | `globalScale = 1`, `useFileScale = true` | 1 |

Hard requirement: **nothing may change visually.** The migrated asset must produce
the same world-space bone positions, vertex positions and renderer bounds as today.
That is the acceptance gate, and it is what the verification harness checks.

---

## 2. Do NOT re-export the characters from PSK

There is a staged PSK re-export in `.work/mfighter-meter-fbx/` (144 FBX) and an
editor script `Assets/Editor/L2MFighterMeterMigration.cs` that installs it.
**Do not run it.** Measured problems:

| | shipping rig | PSK re-export |
|---|---|---|
| root bone name | `bip01` | `Bip01` (capital B) |
| skin bone count in Unity | 70 | 92 |
| `bip01` world Y | `0.4457` | `0.2237` |
| file count | 74 | 71 (`_i` x2 and `m003_m00_ah` missing) |

All 62 clips in `Fighter/MFighter/Clips/*.anim` bind paths like
`MFighter_m000_b.ao/bip01/Bip01_Pelvis/...`. Renaming the bone silently unbinds
every clip (no error, pose just stops applying). Re-exporting from PSK also
loses the unit factor (see §3), so the character comes out ~2x too small.

## 3. The recipe that was verified: normalise the shipping FBX

Do not rebuild geometry from PSK. Take the FBX that ships today and rewrite its *units*
so that the node scale becomes 1 while world coordinates stay identical.

The needed factor is the product of what we are removing:

```
factor = nodeScale x legacyGlobalScale = 100 x 0.019 = 1.9
```

### 3a. Use the numeric patcher, not Blender

`tools/interlude/fbx_meter_patch.py` edits the binary FBX in place:

```
python3 tools/interlude/fbx_meter_patch.py --batch <src_dir> <dst_dir>
```

It changes, and changes only:

| record | legacy | meter |
|---|---|---|
| `GlobalSettings.UnitScaleFactor` (+`OriginalUnitScaleFactor`) | `1` | `100` |
| `Model` `Lcl Scaling` (`.ao` / `.mo` / root) | `100` | `1` |
| `Model` `Lcl Translation` (every bone) | — | `x1.9` |
| `Geometry` `Vertices` | — | `x1.9` |
| `Pose` `PoseNode` / `Matrix` | — | linear `÷100`, translation `x1.9` |
| `Deformer`(Cluster) `TransformLink` | — | linear `÷100`, translation `x1.9` |
| `Deformer`(Cluster) `Transform` | — | translation `x1.9` |

Bone names, hierarchy, skin weights, UVs, normals, materials and the `.meta` GUIDs are
never touched. A parse→serialize round trip is node-for-node identical.

### 3b. Why not the Blender recipe (measured, do not repeat)

The earlier version of this document described a Blender re-bake
(`import_scene.fbx` → `transform_apply(scale)` → export with `FBX_SCALE_ALL`, baking
`1.9` on the roots). It was run on all 75 MFighter files and is **wrong** in two ways
that only show up when the result is imported by Unity:

1. **Blender 5.x already collapses the node `x100`.** An imported root comes in at
   `scale = 1`, not `100`, so the `x100` step is dead code and the `1.9` step ends up
   doubling the intended factor.
2. **Blender rebuilds the rest pose from `BindPose`; Unity builds it from
   `Lcl Translation`.** In `MFighter_anim.fbx` those disagree: Blender moved `bip01`
   from world Y `0.445677` to `0.463699` (4%). Body FBX (`MFighter_m000_b.fbx`) happened
   to agree, so the bug would only surface as a subtly detached pelvis on the anim
   prefab, never as a crash.

The Unity importer is the authority. Verify with `L2MeterProbe` (§7a): the accepted
contract is `globalScale = 1`, `useFileScale = true`, node scale 1, `lossyScale = 1`
everywhere, and world bones/skin vertices identical to the legacy import.

### 3c. Import settings

Change **only** these two fields on each touched `*.fbx.meta`:

```
globalScale: 1
useFileScale: 1
```

Leave `animationType`, `avatarSetup`, `importAnimation`, `meshCompression`, rig
and material remaps exactly as they are now. Set them through `ModelImporter`
in an editor script, not by hand-editing YAML.

## 5. Animation clips

Every clip in `Fighter/MFighter/Clips/*.anim` (62 files) was authored against the
`x100` node and must be rescaled. Two curve families, two rules:

* `m_LocalPosition*` bindings whose `path` is under the `.ao` skeleton: multiply
  every key `value` by `100`. Slopes (`inSlope`/`outSlope`) also x100.
  (`time` unchanged.)
* `m_LocalScale*` on the `.ao` root path: the curve is a constant `100`; replace
  the value with `1` and zero the slopes. Leave other paths' scale curves alone
  (they are already `1`).
* Rotation curves: untouched.

Do it via `AnimationUtility.GetCurveBindings` / `SetEditorCurve`, never by
regex on YAML text.

Do not use the `LooksAlreadyMeter` style heuristic from
`L2MFighterMeterMigration.cs`; it makes the operation non-idempotent. Instead make
the tool re-runnable by recording which clips were converted:
`L2MeterMigration` writes `Assets/MeterContract/<Race>.clips-converted.json` and skips
every clip listed there, so a second run reports `clips=0 skipped=62`.

## 6. Prefabs

`Player_<Race>`, `User_<Race>`, `Pawn_<Race>` nest `<Race>_anim.prefab` /
`<Race>_anim_user.prefab`, which are flattened skeleton copies (no nested FBX
instance): ~195-231 prefab blocks each, with bone transforms and overrides.
Armor/face/hair prefabs, by contrast, **are** nested `PrefabInstance`s of their
FBX, so a normal FBX reimport propagates automatically.

So:

* armor / face / hair prefabs: after the FBX reimport, only delete stale
  `m_LocalScale = 100` overrides that pinned the old `100` (e.g.
  `Fighter/MFighter_m002_u.prefab` overrides root scale to `100`).
  **Only overrides on a nested instance of a migrated FBX qualify.** A prefab's own
  local object can legitimately hold `100` — `MFighter_h_2_2_bh.prefab` does, for a mesh
  stored in raw units; clearing it shrinks the hair 100×.
* anim prefabs: the skeleton must follow the new FBX. `L2MeterMigration` copies the new
  local transforms onto the existing objects by path instead of destroying and
  re-instantiating the subtree. Two measured reasons:

  1. Rebuilding churns prefab `fileID`s. `Player_MFighter` / `User_MFighter` /
     `Pawn_MFighter` override the carrier `SkinnedMeshRenderer` **by fileID**
     (`m_Mesh -> null`, `m_Materials[0] -> null`) to hide the skeleton placeholder.
     A rebuild silently resurrects a ghost body mesh.
  2. `SkinnedMeshRenderer.localBounds` is a serialized cache in renderer space. It is
     not recomputed when the skeleton changes, so it stays 100× too small and the
     character is culled while every transform reads correct. Copy `localBounds` from
     the FBX renderer as well.

  Also force the anim prefab root's own scale to 1.

## 7. Verification harness — mandatory gate

Write an editor script that instantiates each prefab and compares against a
baseline captured from the **current** (pre-migration) tree. Fail loudly with a
non-zero exit code. Do not eyeball this.

For each of `Player_<Race>`, `User_<Race>`, `Pawn_<Race>` plus every armor,
face and hair prefab:

* every bone's `position` and `lossyScale`, matched by transform path;
* every `SkinnedMeshRenderer`: `bones.Length`, `sharedMesh.bounds`,
  world-space `bounds.center`/`bounds.size`;
* at least 16 sampled vertices pushed through `renderer.transform.TransformPoint`
  (or `SkinToWorld`) per renderer.

Tolerance: `1e-4` absolute on positions/bounds, `1e-3` relative on scales.

Baseline capture must happen **before** touching anything, and the baseline file
must be committed (small JSON) so the check is repeatable in CI.

### 7a. Probe the import contract directly

`Assets/Editor/L2MeterProbe.cs` imports every FBX in a folder under a chosen
`(globalScale, useFileScale)` and dumps world bone positions, `lossyScale`, skin bounds
and skinned vertex positions (`BakeMesh`), plus the same vertices after rotating one
bone, so a wrong bind matrix cannot hide behind a correct rest pose.

```
L2_METER_DUMP_DIR=Assets/L2ImportTmp L2_METER_DUMP_CONFIG=1,1 \
L2_METER_DUMP_OUT=/tmp/meter-new.json \
Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterProbe.DumpCli
```

`BakeMesh` already returns vertices in renderer space — do **not** push them through
`renderer.transform.TransformPoint` or the probe reports a phantom 100× error.

Measured result of the numeric patch on all 75 MFighter files, comparing legacy
(`0.019`, `useFileScale=false`) with meter (`1`, `useFileScale=true`):

```
bones 95 vs 95, 0 missing, 0 moved (>1e-5)
skinned vertices   max error 1.5e-7
posed   vertices   max error 1.3e-7
lossyScale         100 -> 1 on every bone
```

### Reference values (measured on the current tree, MFighter)

Use these as a smoke check that the baseline capture is sane:

```
MFighter_anim.fbx      (anim skeleton)
  bip01          worldPos = (0.005541, 0.445677, -0.151771)   legacy worldScale = 100
  Weapon_R_Bone  worldPos = (0.240426, 0.542963, -0.251208)
  skin           bones = 70, worldBounds = (0.455107, 0.288473, 0.541346)

MFighter_m000_b.fbx    (body piece)
  bip01          worldPos = (0.000000, 0.483724, -0.001269)
  Weapon_R_Bone  worldPos = (0.246545, 0.470378, 0.004693)
  skin           bones = 70, worldBounds = (0.354391, 0.280538, 0.163431)
```

After migration the same numbers must hold with `lossyScale = 1` everywhere.

Known delta: the Blender re-export used to write all 92 bones as skin clusters, so
`bones.Length` became 92 instead of 70 (extra bones carry zero weight). The numeric
patcher (§3a) keeps the original 70 clusters. Either way, assert on vertex world
positions, not on bone count.

Pilot result on MFighter (batchmode, commit on this branch):

```
verified prefabs=116 clips=63 legacyBoneScale=3 failures=0
```

The 3 remaining non-unit scopes are **not** skeleton bones and are expected:

* `MFighter_h_2_2_bh.prefab` — a stray `FFighter_m002_m00_bh.mo` object holding a raw-unit
  mesh at scale `100`;
* `click_area` in `Player`/`User`/`Pawn` — a hand-authored click proxy at
  `0.3 × 0.9 × 0.3`.

Do not "fix" those in this migration; they are unrelated to the character unit contract.

## 8. Only then remove the crutches

Order matters. Do not touch this section until the harness is green for the race.

1. `AbstractMeshManager.CompensateMeterWeaponAgainstAnimScale` already branches on
   `parent.lossyScale < 10f` and then sets scale `1`. That is already
   migration-safe, so leave it until **all** races are migrated; it keeps
   unmigrated races working. Do not delete it in the pilot PR.
2. `L2WeaponPrefabBuilder.CharacterAnimScaleCompensation` and any prefab with a
   baked `0.01` root scale (today only
   `LineageWeapons/soul_separator_m00_wp.prefab`) — remove in the **last** PR,
   after every race reports `lossyScale = 1`.

## 9. Scope and order

* One race per PR, pilot = `MFighter`. Branch off `dev`, PR into `dev`.
* Race order after the pilot: whichever race you need for playtest next; the
  pipeline is identical. Races share the pattern `<Race>/Models/*.fbx`,
  `<Race>/Clips/*.anim`, `Human/<Race>/{Faces,Hair}`.
* MFighter file inventory: 74 FBX in `Models`, 62 clips, 142 armor prefabs,
  72 hair prefabs, 10 face prefabs.
* Keep the tools in `tools/interlude/` (`fbx_meter_patch.py`) and the Unity steps as
  editor scripts (`L2MeterMigration.cs`, `L2MeterContractHarness.cs`, `L2MeterProbe.cs`),
  so the migration is reproducible by re-running, not a one-off.
* The Blender re-bake described in earlier revisions of this document is superseded and
  the script was removed; see §3b. Do not reintroduce it.

Full pilot run, in order:

```
# 1. baseline on the untouched tree (commit it)
Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterContractHarness.CaptureCli

# 2. rewrite the FBX units outside Unity, then install them
python3 tools/interlude/fbx_meter_patch.py --batch <Race>/Models <tmp>/models
python3 tools/interlude/fbx_meter_patch.py --batch <Race>/Models/anim <tmp>/anim

# 3. importers, clips, prefabs (idempotent)
Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterMigration.RunCli all

# 4. gate
Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterContractHarness.VerifyCli
```

## 10. Do not

* Do not hand-edit prefab, `.anim` or `.meta` YAML.
* Do not change `.meta` GUIDs, mesh data, UVs, weights, materials or textures.
* Do not re-export from PSK (see §2).
* Do not re-bake through Blender (see §3b).
* Do not run `L2MFighterMeterMigration.Run()`: its `CopyStagingIntoModels` step installs
  the broken staging set.
* Do not migrate monster/other-race FBX in the pilot PR.
* Do not remove the runtime 0.01 compensator before all races are green.
* Do not "rebuild" anim prefab subtrees (see §6).

## 11. Acceptance

* Harness from §7 green for every touched prefab, baseline committed.
* Every bone under a migrated race reports `lossyScale ~= 1`.
* All 62 clips still bind (assert: clip binding path resolves to a transform in
  the prefab for every `m_LocalPosition`/`m_LocalRotation`/`m_LocalScale` binding).
* `build-and-test` green; branch rebased on `origin/dev`.
* Playtest screenshot/recording of a standing, running and attacking character
  with a weapon equipped, at the same camera as before, pasted into the PR.
