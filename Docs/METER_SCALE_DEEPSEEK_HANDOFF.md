# Meter-scale migration — implementation handoff (2026-11-16)

Audience: implementation agent continuing the current branch. This is a WIP handoff, not
an acceptance report. The canonical contract and forbidden approaches are in
`Docs/METER_SCALE_MIGRATION.md`; read it before changing assets or tools.

## Start here

Work in the primary client checkout `/Users/nikitarakitin/Projects/Fineage`, on the
current branch `feat/mfighter-meter-contract-clean`. It already contains earlier migration
commits; do not create a second checkout or a sibling worktree. The uncommitted migration
pile is being preserved as one WIP commit. Do not push directly to `dev` or `main`.

Run one bounded task at a time: one rig or one asset family, then one focused verification.
Do not launch background loops, repeated full imports, or an all-rig pass after every edit.
A failing harness is not permission to relax tolerances or replace baselines.

## Contract and current implementation

Target: FBX `UnitScaleFactor=100`, root scale 1, legacy factor `100 * 0.019 = 1.9` baked
into the correct geometry/transforms, Unity `globalScale=1`, `useFileScale=true`, and bone
`lossyScale=1`. World-space bones, skin samples, and bounds must match the original.

Current pieces:

- `tools/interlude/fbx_meter_patch.py`: binary FBX patcher. It retargets take translation
  and scale curves and uses `has_legacy_carrier` to avoid dividing bind matrices by 100
  when no legacy carrier exists. This does not prove all edge cases are fixed.
- `L2MeterMigration`: importer, clip, and prefab migration. Optional `gearcopies` invokes
  `SyncEmbeddedSkeletons` for embedded/flattened armor skeletons.
- `L2MeterRigs`: rig discovery, including Human `Faces` and `Hair` and `Player_`, `User_`,
  `Pawn_` prefabs.
- `L2MeterContractHarness`: baseline comparison. `NearPosition` compensates the root world
  offset before comparing positions. Clip lookup uses a path hash set to avoid quadratic
  work. The normal vertex check uses `skin.transform.TransformPoint(mesh.vertices[i])`.
- `L2MeterChainDiag`: read-only diagnostic for importer and prefab scale chains. It is an
  untested WIP utility, not an acceptance gate.
- `L2OrcRacePrefabBuilder` and `L2TownNpcFbxImport` preserve an already metered importer
  instead of reapplying `0.019`. These edits must be reviewed with the final migration.

Do not run the old PSK re-export or `L2MFighterMeterMigration`, Blender round-trip assets,
hand-edit YAML, change GUIDs/materials/weights/UVs, or rebuild flattened anim subtrees.

## Measure first (no asset mutation)

Use the installed Unity 6000.3.27f1. Close the interactive Unity editor for this project
before running batchmode; concurrent import will corrupt the measurement.

```bash
cd /Users/nikitarakitin/Projects/Fineage
UNITY=/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity
"$UNITY" -batchmode -quit -projectPath "$PWD/l2-unity" \
  -executeMethod L2MeterContractHarness.VerifyCli \
  -logFile /tmp/fineage-meter-resume.log
```

Expect exit 1 while failures remain. Read the complete last rig summary before choosing a
fix. A nonzero Unity exit is a failed gate, not a successful diagnostic run.

### Last measurements — not a clean result

The last saved full log `/tmp/fineage-final-rigs3.log` says `rigs=83 failed=12`. It lists
11 nonzero rig summaries; the remaining failing group is
`LineageNPCs/a_casino_MHuman_m00/Model` with 16 unbound clip curves (its summary says
`failures=0`, which is inconsistent and must be investigated). Treat the fresh run as
authoritative. Earlier `70/83` was an intermediate observation, not a stable final count.

Last known rig summary failures:

| Rig | Main evidence in last log |
|-----|---------------------------|
| `DarkElf/FDarkElf` | 10: hair/face/armor vertex deltas and three actor `.ao` scales near 100 |
| `Elf/MElf` | 4: actor body samples and `MElf_m000_f` vertex delta |
| `Fighter/FFighter` | 7: embedded gear copies `m000/m001` g,l,u and `m003_u`, approximately 100x samples |
| `Fighter/MFighter` | 3: anim mesh, `MFighter_m002_u`, and `MFighter_h_2_2_bh` raw-unit mesh |
| `LineageMonsters/aracne_m00` | 6: small bone deltas plus a material vertex delta |
| `LineageMonsters/keltir_m00` | 1: small vertex delta |
| `LineageMonsters/pet_wolf_a_m00` | 1: small vertex delta |
| `LineageMonsters/werewolf_m00` | 1: vertex delta at a large world offset |
| `LineageMonsters/wolf_m00` | 4: vertex delta and three unbound `bip01` damage curves |
| `Orc/FOrc` | 19: actor bone/vertex deltas, including large-world-coordinate cases |
| `Orc/MOrc` | 45: actor bone/vertex deltas, including local-coordinate `Player_MOrc` |
| `LineageNPCs/a_casino_MHuman_m00/Model` | 16 unbound `bip01` curves despite zero in rig summary |

Several small monster failures may be import precision, but that is a hypothesis, not a
reason to change the `1e-4` tolerance. First compare local-space transforms/vertices at a
neutral root to distinguish floating-point precision from a real bind-pose difference.
For unbound curves, compare actual case-sensitive transform paths in the baseline and
current prefab. Do not merely skip unresolved curves or rename bones to silence the check.

The preservation snapshot is large: 3067 staged files, including Unity-generated metadata
outside the core migration tools. Review that scope before any PR. `git diff --check` on
this snapshot reports trailing spaces in Unity-generated `*.clips-converted.json.meta`
files; do not confuse that warning with harness acceptance or bulk-edit generated YAML.

## Repair order

1. Reproduce and record the exact current failures once. Save a copy of the log outside
   the repo if needed; do not commit logs or temporary probe scenes.
2. Fix one self-contained regression: `Fighter/MFighter` first. Its documented pilot was
   previously green, so compare the three failing assets with their previously working
   committed state before adding another migration mechanism.
3. Fix flattened gear behavior on one `FFighter` asset, using the already migrated FBX as
   the source of truth. Preserve existing GameObjects/fileIDs, world bind result,
   renderer bone mapping, and `localBounds`. Only then run `gearcopies` on that rig.
4. Investigate `FDarkElf`/`MElf` bind matrices and actor scales separately from gear
   copies. `has_legacy_carrier` fixed an over-division case; do not apply a global extra
   100 or 0.01 multiplier to make a sampled vertex appear correct.
5. Investigate Orc actor skeletons and case-sensitive `bip01` curve binding. Check source
   path and imported hierarchy; do not rewrite clips until the binding mismatch is known.
6. Resolve small monster deltas and the harness's missing casino failures last. Do not
   group a 100x scale regression with a millimetre/submillimetre precision discrepancy.
7. Run a fresh full gate only after focused checks pass. It must report `rigs=83 failed=0`
   and zero failures in every rig summary. Then verify `git diff --check`, compile/test,
   and perform the visual playtest required by the migration spec.

Focused read-only diagnostic, when needed (it writes `/tmp/fineage-meter-chain.txt`):

```bash
"$UNITY" -batchmode -quit -projectPath "$PWD/l2-unity" \
  -executeMethod L2MeterChainDiag.DumpPrefabCli \
  Assets/Resources/Data/Animations/Fighter/FFighter_m000_u.prefab \
  -logFile /tmp/fineage-meter-chain.log
```

The existing migration CLI is:

```bash
L2_METER_RUN_PREFABS=1 "$UNITY" -batchmode -quit -projectPath "$PWD/l2-unity" \
  -executeMethod L2MeterMigration.RunCli prefabs Fighter/FFighter gearcopies \
  -logFile /tmp/fineage-meter-ffighter.log
```

Do not rerun that command blindly: it mutates assets. First inspect the source FBX, the
flattened prefab, and what `SyncEmbeddedSkeletons` would change. Do not run all races or
reconvert already-recorded clips. The `*.clips-converted.json` markers are required for
idempotence and are not visual baselines.

## Explicit preservation rules

- Never regenerate, edit, delete, or relax `*.baseline.json` to make the current assets
  pass. If a baseline itself is suspected invalid, stop and report evidence to the user.
- `MFighter_h_2_2_bh.prefab` contains `FFighter_m002_m00_bh.mo` with intentional scale
  100 for a raw-unit mesh. Preserve it; clearing that scale shrinks hair 100x.
- `click_area` scales are intentional hand-authored proxies, not skeleton regressions.
- Warnings about `legacyBoneScale` are not automatically failures; check the actual bone
  path and documented exception. Do not remove expected click-proxy scale to get zero.
- Keep runtime and baked weapon `0.01` compensation until every character race is green.
  Weapons already use meter imports; an earlier audit found 390/390 LineageWeapons FBX at
  `globalScale=1`, `useFileScale=true`. Recheck the final set once, not after every edit.
- Do not delete working asset changes or reset the branch to make failures disappear.
  Before finishing, inspect the WIP diff and split/rebase it into small reviewable PRs.

## Done means

- All 83 discovered rigs pass the unmodified baseline harness with zero failures.
- All skeleton bones have unit lossy scale, excluding documented non-skeleton exceptions.
- Every animation curve resolves to its intended transform and migrated clips are not
  scaled again on a rerun.
- Weapons, character/armor attachments, and all supported NPCs/monsters retain their
  original visual size and placement in both rest and animated poses.
- Standing, running, and attacking characters with equipped weapons have been visually
  checked against the same camera as before, with evidence attached to the PR.
- The final branch is rebased onto `origin/dev`, delivered via PR, and required
  `build-and-test` is green. Do not wait for non-required `unity-editmode`.

The preservation WIP commit alone satisfies none of these acceptance criteria.
