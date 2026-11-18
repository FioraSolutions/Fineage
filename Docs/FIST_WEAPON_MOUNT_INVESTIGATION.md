# Fist weapon mount — investigation log (2026-11-17)

**Status: OPEN — root cause narrowed, not fixed.** Parked on purpose; this file preserves
every measurement so the next attempt does not redo the dead ends.

Symptom: fist weapons (claws, knuckles, cestuses) mount crookedly on characters in the
world. Reported per item by the maintainer:

- **A-grade (`fist_blade`) — looks correct.**
- **S-grade (`demon_splinter`) — crooked.**
- **B-grade — mixed**, some fine, some crooked.

Reproduced on Human and Orc fighters, in the world idle pose (not the lobby). Both hands.

---

## 1. Repository state was checked first — nothing was damaged

The concern was that a second agent working in the tree had broken something. It had not.

- Branch: `feat/mfighter-meter-contract-clean`
- HEAD: `5f9ad5ee20 Merge origin/dev into feat/mfighter-meter-contract-clean`
- The only files the other agent touched in the window were
  `l2-unity/Assets/Editor/L2MeterMigration.cs` and
  `l2-unity/Assets/Editor/L2MeterContractHarness.cs`. Neither intersects weapon mounting.
- Uncommitted weapon-prefab diffs are Unity's own serialization churn
  (`m_ForceMeshLod`, `m_GlobalIlluminationMeshLod`, `m_MaskInteraction`), not a revert.
- All diagnostic scripts under `l2-unity/Assets/Editor/Diag/` are intact.

## 2. Hypotheses that are disproven — do not retry these

### 2.1 The PSK bind pose is not applied by the game

Weapons are UE2 `SkeletalMesh` with a single root bone. The bind pose lives in the `REFSKELT`
chunk of each `.psk`. Measured against the shipped FBX:

| model | bind angle | FBX vertices |
|---|---|---|
| `demon_splinter` | 36.25° | raw PSK exactly (`rms = 0.000000`) |
| `fist_blade` | 98.42° | raw PSK exactly |
| `great_pata` | 167.00° | raw PSK exactly |
| `dragon_grinder` | 180.00° | raw PSK exactly |

The FBX files carry **raw** `PNTS0000` vertices, never the bind-posed ones.

The decisive counter-example is the swords, which the maintainer reports sit correctly:
`great_sword` has bind 98.4°, `angel_slayer` 148.6°, `sword_of_damascus` 180.0°. Their bind
angles are as varied as the fists'. If the game applied the bind pose, those swords would be
crooked too. **It does not.**

⇒ Applying `bind` on top of the socket in `AbstractMeshManager` is the wrong fix.

### 2.2 `inverse(bind)` (the UE2 `AttachToBone` shape) is also wrong

Tested as a candidate: `handWorld * inverse(bind) * v`. It throws the mesh off the hand:

| model | centroid distance from socket, `identity` | with `inverse(bind)` |
|---|---|---|
| `demon_fangs` | 0.175 m | 0.497 m |
| `demon_splinter` | 0.050 m | 0.298 m |

→ 30–50 cm away from the fist. Dead end.

### 2.3 The character hand socket is not the problem

`Weapon_R_Bone` / `Weapon_L_Bone` were read straight from the original body `.psk` files for
every playable race. Human, Orc, Elf, Dwarf, Dark Elf and Shaman agree to within 1–2°:

```
MFighter_m000_u   Weapon_R_Bone q=(-0.5261,+0.4767,-0.5012,+0.4948) pos=(+2.692,+0.524,+1.219)
MOrc_m000_u       Weapon_R_Bone q=(-0.5179,+0.4813,-0.5183,+0.4811) pos=(+5.421,+0.600,+1.944)
MElf_m000_u       Weapon_R_Bone q=(-0.5156,+0.4832,-0.5122,+0.4882) pos=(+2.554,+0.216,+1.227)
MDwarf_m000_u     Weapon_R_Bone q=(-0.5010,+0.4959,-0.5053,+0.4977) pos=(+2.569,+0.439,+1.483)
FDarkElf_m000_u   Weapon_R_Bone q=(-0.5258,+0.4763,-0.5061,+0.4905) pos=(+2.258,+0.623,+0.884)
MShaman_m000_u    Weapon_R_Bone q=(-0.5085,+0.4846,-0.5172,+0.4890) pos=(+3.274,+0.255,+1.946)
```

A 1–2° spread cannot produce "one grade fits, another is sideways". The earlier socket repair
in `3aa2106f0d` ("align weapon sockets") already handled the one real outlier (orcs drifted
1.2–2.7 cm; that commit fixed it).

⇒ Do not re-run `L2WeaponSocketRepair`.

### 2.4 The mount convention `identity` is correct for swords

`sword_of_damascus` geometry: handle near `X ≈ -2.5`, blade running to `X = +30.8`. Mounted
with an identity local transform on the socket, the blade points out of the fist correctly.
This is the reference behaviour that any fist fix must not regress.

## 3. What the meshes actually look like

Raw `.psk` bounds per weapon (UE units; multiply by 0.019 for metres). The axis each mesh is
built along differs per item — this is why "some fit, some don't":

| model | raw bounds X | raw bounds Y | raw bounds Z | long axis |
|---|---|---|---|---|
| `fist_blade` (A, **reported OK**) | −5.6 … +5.6 | **−10.5 … +1.3** | −0.8 … +0.8 | −Y |
| `demon_splinter` (S, **crooked**) | −7.4 … +7.4 | **−10.3 … +4.4** | −0.4 … +7.2 | −Y |
| `great_pata` (B) | −3.6 … +3.6 | −7.0 … +4.0 | +0.2 … +4.8 | −Y |
| `demon_fangs` | **−7.9 … +17.8** | −6.5 … +6.5 | −2.0 … +2.0 | +X (like a sword) |
| `chakram` | −4.7 … +4.7 | −9.0 … +0.6 | −0.3 … +0.3 | −Y (flat disc) |
| `dragon_grinder` | −10.3 … +10.3 | −10.0 … +7.8 | −2.8 … +2.9 | symmetric in X |

Note `fist_blade` and `demon_splinter` share the same long axis (−Y) yet the maintainer
reports one as correct and the other as crooked. **Direction alone does not explain it.**

## 4. The strongest lead: a re-export on 2026-11-14

`fist_blade` — the one item reported as *correct* — is not the original geometry. It was
rebuilt by commit `cb28bf61af` ("… flat fist weapons get an edge …", 2026-11-14), together
with `knuckle_dust` and `vipers_canine`. Those three are the only fist weapons re-exported:

| model | PSK verts | FBX verts | export date | exporter |
|---|---|---|---|---|
| `fist_blade` | 129 | **258** | **2026-11-14** | Blender 5.2.3 |
| `knuckle_dust` | 74 | **148** | **2026-11-14** | Blender 5.2.3 |
| `vipers_canine` | 100 | **200** | **2026-11-14** | Blender 5.2.3 |
| `demon_splinter` | 141 | 141 | 2026-08-25 | Blender 3.6.16 |
| `great_pata` | 63 | 63 | 2026-08-25 | Blender 3.6.16 |
| `demon_fangs` | 98 | 98 | 2026-08-25 | Blender 3.6.16 |
| `arthro_nail` | 275 | 275 | 2026-08-25 | Blender 3.6.16 |
| `foxs_nail` | 68 | 68 | 2026-08-25 | Blender 3.6.16 |
| `chakram` | 89 | 89 | 2026-08-25 | Blender 3.6.16 |
| `dragon_grinder` | 102 | 102 | 2026-08-25 | Blender 3.6.16 |

The re-export doubled the vertex count (a solidify/thicken pass) and changed the geometry
envelope. Old `fist_blade` before that commit:

```
OLD  n=129  X[-0.0563, +0.0563]  Y[-0.1054, +0.0135]  Z[-0.0081, +0.0081]   (metres)
NEW  n=258  X[-0.1280, +0.1280]  Y[-0.2180, +0.0320]  Z[-0.0350, +0.0350]
```

So the mesh got thicker **and wider** — not a pure thickness change. Zero of the 258 new
vertices coincide with any raw PSK vertex, so the model was edited, not merely re-scaled.

**Correlation:** the three items touched by that pass are the A-grade fist and two other
fist weapons; the seven items the maintainer calls crooked are all untouched originals. This
is the strongest signal found, but it is a correlation — it has **not** been confirmed
visually, because the renders could not be made legible (see §6).

### Bind poses (verified against raw bytes, correct)

`REFSKELT` record layout: name `64s`, then `flags/parent/children 3i` (bytes 64–75), quat
`(x,y,z,w)` at bytes **76–91**, position at bytes **92–103**.

| model | bind quaternion | bind position |
|---|---|---|
| `fist_blade` | (−0.6533, +0.2706, +0.2706, +0.6533) | (−3.81, −0.72, −0.02) |
| `the_fist_of_hero` | (−0.6533, −0.2706, −0.2706, +0.6533) | (−1.26, +6.84, +1.41) |
| `great_pata` | (+0.9936, +0.0000, −0.0000, +0.1132) | (−0.01, −0.54, +1.92) |
| `bellion_cestus` | (−0.1951, +0.0000, +0.0000, +0.9808) | (−0.00, −0.30, +1.97) |
| `demon_splinter` | (−0.1075, +0.2438, +0.1606, +0.9504) | (+15.17, −1.28, −4.04) |
| `demon_fangs` | (+0.3536, +0.6124, +0.3536, +0.6124) | (+18.61, −0.00, −0.01) |
| `arthro_nail` | (+0.5000, −0.5000, −0.5000, +0.5000) | (−4.83, −4.31, +1.06) |
| `foxs_nail` | (+0.5000, −0.5000, −0.5000, +0.5000) | (−6.36, +0.66, +0.40) |
| `chakram` | (+0.7071, +0.7071, +0.0000, +0.0000) | (−0.01, −4.20, +0.00) |
| `dragon_grinder` | (+1.0000, +0.0000, +0.0000, +0.0000) | (+2.81, +0.03, −5.54) |

An earlier session mis-read these (quaternion taken from byte 64 instead of 76); the table
above is the corrected one.

## 5. Derived per-weapon correction (produced, NOT validated)

Anchoring on `fist_blade` as "correct today", the relative rotation for each weapon is

```
correction = inverse(bind_anchor) * bind_weapon
```

| model | relative angle | correction quaternion |
|---|---|---|
| `fist_blade` (anchor) | 0.00° | (0, 0, 0, 1) |
| `the_fist_of_hero` | 90.00° | (0, 0, −0.7071, 0.7071) |
| `great_pata` | 109.78° | (+0.7230, −0.2995, +0.2382, −0.5751) |
| `bellion_cestus` | 79.62° | (+0.5133, −0.2126, −0.3182, +0.7682) |
| `demon_splinter` | 73.64° | (+0.5732, −0.1737, −0.0220, +0.8005) |
| `demon_fangs` | 129.01° | (+0.7011, −0.0923, +0.5610, +0.4305) |
| `arthro_nail` | 148.60° | (+0.6533, −0.2706, −0.6533, −0.2706) |
| `foxs_nail` | 148.60° | (+0.6533, −0.2706, −0.6533, −0.2706) |
| `chakram` | 148.60° | (+0.6533, +0.2706, +0.6533, −0.2706) |
| `dragon_grinder` | 98.42° | (+0.6533, −0.2706, +0.2706, −0.6533) |

**Caveat — this table rests on an assumption that is not proven.** It presumes `fist_blade`
looks right *because* of its untouched local transform, i.e. that its bind frame is the
correct one. But `fist_blade`'s mesh was rebuilt on 2026-11-14 by an ad-hoc pass, so its
*bind pose* still describes the **old** geometry while its *vertices* are the new ones.
Anchoring to a stale bind frame is therefore questionable. Treat §5 as a starting point for
experiments, not as a fix.

Left-hand mirroring interacts with all of this: `AbstractMeshManager.SetParentGo` flips
`localScale.x` for fist weapons in the left slot, so any rotation correction must be checked
on both hands, not just the right.

## 6. Rendering attempts — and why they are not evidence

A dozen batch-render harnesses were written; none produced a trustworthy frame. Recorded so
nobody repeats them thinking the pictures were fine:

- `L2FistGrid`, `L2FistCorrected`, `L2FistCloseup`, `L2FistAttachmentModels` — lit renders.
  Thin claws against a dark background came out as an indistinct smear; framing cut the hand
  off in some shots.
- `L2FistSilhouette` — unlit flat-colour pass intended to fix the above. **Every output was
  1-colour (blank).** `Unlit/Color` was applied after `SkinnedMeshSync`, so the paired body
  pieces rendered empty and nothing was drawn. Verified blank with `magick identify -format %k`.
- One earlier comparison montage was also blank. **Conclusions drawn from it were discarded.**

**Lesson for next time:** assert the render is non-blank before judging it, e.g.
`magick identify -format '%k' shot.png` must be far above 1. A batch camera that silently
renders nothing is worse than no render.

Also note: batchmode cannot run while the Unity editor is open — it exits 1 on the project
lock (`Temp/UnityLockfile`). Close the editor first.

## 7. Diagnostic scripts kept in the tree

Untracked, under `l2-unity/Assets/Editor/Diag/` (safe to delete once fixed):

| script | what it does |
|---|---|
| `L2FistBindVariants.cs` | mounts one weapon under each mirror convention on both hands |
| `L2FistBindBothHands.cs` | per-weapon check on MOrc and MFighter, both hands |
| `L2FistSocketGeometry.cs` | socket vs hand-bone offsets, palm vertices bound to the socket |
| `L2FistPrefabAudit.cs` | how many meshes a weapon prefab carries and their local bounds |
| `L2FistHandFrame.cs` | measures a weapon in hand space (centroid, long axis, reach) |
| `L2FistModelCompare.cs` | PSK vs FBX vertex comparison |
| `L2FistPrefabAudit.cs` | prefab renderer / transform inventory |
| `L2FistRotationTest.cs` | rotation sweep on the socket |
| `L2FistAcrossRaces.cs` | same weapon across race rigs |
| `L2FistAttachmentModels.cs` | `identity` vs `bind` vs `inverse(bind)` |
| `L2FistGrid.cs` | first contact sheet (420 px, too small) |
| `L2FistCorrected.cs` | raw vs per-weapon correction, both hands |
| `L2FistCloseup.cs` | 900 px close-ups, two angles |
| `L2FistSilhouette.cs` | unlit silhouette pass (produced blank output, see §6) |
| `L2OrcFistAssembled.cs` | assembled orc fist check |

## 8. Next steps when this is resumed

1. Decide the question in §5: is `fist_blade` correct *because of* its transform, or only
   because its geometry was rebuilt? Compare against a fist weapon that was never rebuilt
   (`bellion_cestus` is the smallest, 73 verts, least clutter).
2. Get **one** legible render before changing anything. Fix the silhouette pass (§6):
   apply the flat material *before* `SkinnedMeshSync`, and assert `%k > 1` on every output.
3. Look for the ad-hoc 2026-11-14 thicken script. It is **not** in `tools/interlude/`
   (`pskx_to_fbx_batch.py`, `psk_to_fbx_armor.py` contain no solidify/thicken step), so it
   was likely run outside the repo. Re-deriving it would let the other 7 fists be rebuilt
   the same way as the three that reportedly fit.
4. Whatever ships, verify **both hands** and at least Human + Orc, in the world idle pose.

Do not touch `L2WeaponSocketRepair` (§2.3) and do not add a bind-pose multiply in
`AbstractMeshManager.SetParentGo` (§2.1).
