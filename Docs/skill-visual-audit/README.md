# Skill visual audit (WIP)

Source: official Interlude `animations/*.ukx` -> `umodel -export -psk` -> PSA take list
(`source-takes.tsv`: race, take, frames, rate, imported-in-project 0/1).

## Findings so far

1. **Animator template has no SpAtk03..SpAtk28 states.** `Player_Basic.controller` only has
   `SpAtk01_1HS/2HS/bow/pole`, `SpAtk02_1HS`, `SpAtk06_hand`. Playback of SpAtk03+ is the race clip
   from `InterludeAnimResourceTake.tsv`, not a fallthrough to `SpAtk01_1HS`. Mix tags still name the
   swings (Sonic Blaster Mix06=`SpAtk18`, Double Sonic Slash Mix04=`SpAtk07`/`SpAtk12`/`SpAtk21`,
   Triple Slash Mix05=`SpAtk08`/`SpAtk13`/`SpAtk22`, Triple Sonic Slash Mix03=`SpAtk07`/`SpAtk13`/`SpAtk20`).
2. **MFighter Dual SpAtk takes for those skills are in the tree** (`SpAtk03/07/08/11/12/13/18/19/20/21/22_Dual_MFighter`,
   imported in #1622). `source-takes.tsv` marks a take imported only when that Resources clip exists.
   `missing-assets.tsv` lists official takes that are still absent. See `Docs/MISSING_MFighter_DUAL_SPATK.md`.
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
- `Docs/skill-visual-audit/missing-assets.tsv` — official takes whose Resources clip is absent
- `Docs/skill-visual-audit/audit-summary.txt` — resolution counts
- `Docs/MISSING_MFighter_DUAL_SPATK.md` — MFighter Dual SpAtk rows from the same run

Latest summary (after script): 3116 `playable_clip`, 13516 `missing_clip`.

## Next

- Rumble/sound binding pass (skillgrp `rumble_self` → FMOD)
