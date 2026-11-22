# UE2 world material ops gap

Source: committed `*.props.txt` under `l2-unity/Assets/Resources/Data/Textures`.
Shared path: `L2Ue2MaterialChain` executes Shader slots (Diffuse, Opacity, Specular,
SpecularityMask, SelfIllumination, SelfIlluminationMask, Detail), Combiner, FinalBlend,
TexPanner, TexOscillator, TexEnvMap, and TexScaler.

A referenced op with no props file is not given a guessed PanRate or combine mode.
The runtime logs `[Ue2Material] material <name> op <class> '<object>' missing props`.

## Modifier references whose property dump is missing

| Op | Object | References |
|---|---|---:|
| `TexPanner` | `evil_fallwater01_pan` | 3 |
| `TexPanner` | `statue_pan` | 3 |
| `TexPanner` | `TexPanner1` | 2 |
| `Combiner` | `de_fire_glow_s` | 1 |
| `Combiner` | `reflection` | 1 |
| `TexEnvMap` | `HighlightEnv01` | 1 |
| `TexEnvMap` | `spenv` | 1 |
| `TexEnvMap` | `statue_sp` | 1 |
| `TexOscillator` | `ssq_water_01_tex` | 1 |
| `TexPanner` | `DE_panner_01` | 1 |
| `TexPanner` | `FX_E_under_pan04` | 1 |
| `TexPanner` | `Orc_Dgn_light002_pan` | 1 |
| `TexPanner` | `elf_water_01` | 1 |
| `TexPanner` | `evil_fallwater02_pan` | 1 |
| `TexPanner` | `pagrio_fx_01_sh` | 1 |
| `TexPanner` | `sp2f_fire_pan` | 1 |

Missing objects: **16**. References by op (including ones that resolve): Combiner 2, TexEnvMap 3, TexOscillator 1, TexPanner 15.

TexScaler and FinalBlend are part of the same evaluator. Committed world props do not
reference them yet; a chain that does is executed when its props are present.

The original `.utx` packages are not in this tree, so this pass cannot extend the
catalog with PanRate / PanDirection / CombineOperation / EnvMapType / oscillation
fields. Those fields have to be dumped losslessly from the packages before the
referenced ops can move.

Some short shader names exist in more than one package with different slot text
(`z_water01` in `interior_aa_t` and `interior_B_t`, plus other `z_*` / grass pairs).
The runtime keys props by that short name. When the two bodies differ ignoring case
it logs `[Ue2Material] material <name> props ambiguous` and does not pick one.
