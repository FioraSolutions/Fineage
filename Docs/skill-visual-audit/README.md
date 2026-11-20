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

## Runtime fix (this branch)

`AnimationManager.TryPlayCombatCrossFade` plays imported `SpAtk03+` clips through
`CastClipPlayable.TryPlaySpAtkCombat` when `Player_Basic` has no matching state (instead of
falling back to `SpAtk01_1HS`). Clips resolve via `SkillAnimationDatabase.GetSpAtkCombatClip`.

## Audit

```bash
python3 scripts/skill-visual-audit/run_audit.py
```

Outputs:

- `Docs/skill-visual-audit/audit-table.tsv` — skill × race × weapon rows
- `Docs/skill-visual-audit/missing-assets.tsv` — official takes not imported (needs Mac export)
- `Docs/skill-visual-audit/audit-summary.txt` — resolution counts

Latest summary (after script): ~2904 `playable_clip`, ~13728 `missing_clip` (mostly MFighter gaps).

## Next

- Import missing MFighter takes from PSA on Mac (`source-takes.tsv` imported=0 rows)
- Rumble/sound binding pass (skillgrp `rumble_self` → FMOD)
