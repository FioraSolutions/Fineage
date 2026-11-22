# Missing MFighter dual SpAtk clips (Mac re-bake)

These AnimTween sequences exist in `AnimTween.txt` but no shipped `Resources` clip was found under `Data/Animations/Fighter/MFighter/Clips/`. Import via `L2MFighterDuelistSpAtkImporter` after staging `MFighter_anim.fbx` (see editor class).

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