# Missing MFighter dual SpAtk clips (Mac re-bake)

These AnimTween sequences exist in `AnimTween.txt` but no shipped `Resources` clip was found under `Data/Animations/Fighter/MFighter/Clips/`. Do not commit `.ukx`, `.psa`, `.psk`, or `.fbx`. On a Mac with the Interlude client and umodel:

```bash
umodel -game=l2 -export -out="$HOME/l2-export/Fighter" \
  "$HOME/Downloads/L2_Interlude_Client/Lineage II/Animations/Fighter.ukx"

blender -b -P l2-unity/Assets/Editor/L2EffectGenerator/Blender/psk_psa_to_fbx.py -- \
  "$HOME/l2-export/Fighter/MFighter_m000_b.psk" \
  "$HOME/l2-export/Fighter/MFighter_anim.psa" \
  l2-unity/Assets/L2ImportTmp/MFighter_anim.fbx \
  l2-unity/Assets/Editor/L2EffectGenerator/Blender/io_import_scene_unreal_psa_psk_280.py

Unity -batchmode -nographics -quit -projectPath l2-unity \
  -executeMethod L2MFighterDuelistSpAtkImporter.RunCli
```

`RunCli` still exits 1 until that staged FBX is imported with the same skeleton remap as `L2MFighterSpAtk06Importer` (meter contract against `SpAtk01_1HS`). It does not invent a clip. Until the takes land, a missing Dual SpAtk logs `skillId`, race, weapon, `expectedClipStem`, and the Resources path, and does not play `SpAtk01_1HS`.

| AnimTween sequence | Expected Resources stem |
|--------------------|-------------------------|
| `spatk03_dual_MFighter` | `MFighter_m000_b.ao_spatk03_dual_MFighter` |
| `spatk07_dual_MFighter` | `MFighter_m000_b.ao_spatk07_dual_MFighter` |
| `spatk08_dual_MFighter` | `MFighter_m000_b.ao_spatk08_dual_MFighter` |
| `spatk11_dual_MFighter` | `MFighter_m000_b.ao_spatk11_dual_MFighter` |
| `spatk12_dual_MFighter` | `MFighter_m000_b.ao_spatk12_dual_MFighter` |
| `spatk13_dual_MFighter` | `MFighter_m000_b.ao_spatk13_dual_MFighter` |
| `spatk18_dual_MFighter` | `MFighter_m000_b.ao_spatk18_dual_MFighter` |
| `spatk19_dual_MFighter` | `MFighter_m000_b.ao_spatk19_dual_MFighter` |
| `spatk20_dual_MFighter` | `MFighter_m000_b.ao_spatk20_dual_MFighter` |
| `spatk21_dual_MFighter` | `MFighter_m000_b.ao_spatk21_dual_MFighter` |
| `spatk22_dual_MFighter` | `MFighter_m000_b.ao_spatk22_dual_MFighter` |

Takes listed in `L2MFighterDuelistSpAtkImporter.DualTakes` use Pascal `SpAtkNN_Dual_MFighter` FBX names; rebake should align Resources names with AnimTween ids (or add a row to `InterludeAnimResourceTake.tsv` via `scripts/interlude-anim-whitelist.py` after import).