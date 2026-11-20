# Skill visual audit (WIP)

Source: official Interlude `animations/*.ukx` -> `umodel -export -psk` -> PSA take list
(`source-takes.tsv`: race, take, frames, rate, imported-in-project 0/1).

## Findings so far

1. **Animator template has no SpAtk03..SpAtk28 states.** `Player_Basic.controller` only has
   `SpAtk01_1HS/2HS/bow/pole`, `SpAtk02_1HS`, `SpAtk06_hand`. `PlayerSpAtkAnim` prefixes stop at 09/27/28,
   and `AnimationManager.ResolvePlayableCombatState` falls every missing state back to
   `SpAtk01_1HS`. So every skill whose skillgrp `animation` tag maps (via `AnimationCombo_Classic.txt`)
   to SpAtk03+ (Mix01..Mix09 = SpAtk07..26, e.g. Duelist Sonic Blaster Mix06=SpAtk18,
   Sonic Storm Mix07=SpAtk19, Double Sonic Slash Mix04=SpAtk07/12/21, Triple Sonic Slash Mix03)
   plays the generic SpAtk01 swing (the "dagger leftover" look) for every race.
2. **MFighter is missing 59 of 114 official takes** (29 skill takes: SpAtk03/04/07/08/11/12/13/18/19/20/21/22,
   ShieldAtk_1HS, CastLong, CastShort, MagicNotarget, SpAtk02_Bow). All other races have every take imported.
3. PSA -> project clip transform verified against committed `SpAtk01_1HS_MFighter`:
   rot = (-x, y, z, w) (root bone: (-x, y, z, -w)), pos = (-x, y, z) * 0.0189975, frame/24 s.
   Exact at every committed key (`compare_psa_clip.py`).

## Next

- Generate missing MFighter takes from PSA (no Unity needed), runtime path that plays the race's
  `SpAtkNN_<Weapon>` clip when the template has no state, full per-skill table (anim/sound/effect/attach).
