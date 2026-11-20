# Classic vs Interlude skillgrp audit

- Interlude rows: 29748 (2630 skill ids)
- Classic rows: 34698 (8274 skill ids)
- Overlap rows: 14448
- Only in Interlude: 15300 rows, 462 skills
- Only in Classic (later-chronicle / missing): 20250 rows, 6265 skills
- Overlap rows with any visual/anim/hit_time/range/is_magic/operate_type mismatch: 5544

- Unique skill ids with visual mismatch: 604
- Unique skill ids with animation mismatch: 64
- Unique skill ids with hit_time mismatch: 96
- Unique skill ids with cast_range mismatch: 21
- Unique skill ids with is_magic mismatch: 80
- Unique skill ids with operate_type mismatch: 62
- Unique skill ids with visual/anim/hit_time/cast_range mismatch: 665

## Highlighted Adventurer / reported skills (level 1)
- 358: Classic vis=358 ani=X hit=1.0 range=40 | Interlude vis=358 ani=X hit=1.0 range=40 desc=skill.at.358
- 361: Classic vis=361 ani=f hit=2.5 range=500 | Interlude vis=361 ani=f hit=4.0 range=500 desc=skill.at.361
- 445: Classic vis=445 ani=X hit=2.0 range=-1 | Interlude vis=361 ani=X hit=2.0 range=-1 desc=skill.at.361
- 453: Classic vis=1231 ani=X hit=1.0 range=-1 | Interlude vis=1231 ani=D hit=4.0 range=600 desc=skill.el.1231
- 454: only Interlude
- 456: only Interlude
- 460: Classic vis=454 ani=i hit=5.0 range=-1 | Interlude vis=454 ani=i hit=5.0 range=-1 desc=skill.white.454
- 1231: Classic vis=1231 ani=K hit=1.5 range=150 | Interlude vis=1231 ani=K hit=1.5 range=150 desc=skill.el.1231

## Example mismatches (up to 12 per field)
### visual
- 3 lvl 1: Classic 3 → Interlude ∅
- 3 lvl 2: Classic 3 → Interlude ∅
- 3 lvl 3: Classic 3 → Interlude ∅
- 3 lvl 4: Classic 3 → Interlude ∅
- 3 lvl 5: Classic 3 → Interlude ∅
- 3 lvl 6: Classic 3 → Interlude ∅
- 3 lvl 7: Classic 3 → Interlude ∅
- 3 lvl 8: Classic 3 → Interlude ∅
- 3 lvl 9: Classic 3 → Interlude ∅
- 16 lvl 1: Classic 16 → Interlude ∅
- 16 lvl 2: Classic 16 → Interlude ∅
- 16 lvl 3: Classic 16 → Interlude ∅

### animation
- 21 lvl 1: Classic X → Interlude D
- 21 lvl 2: Classic X → Interlude D
- 21 lvl 3: Classic X → Interlude D
- 61 lvl 1: Classic X → Interlude D
- 61 lvl 2: Classic X → Interlude D
- 61 lvl 3: Classic X → Interlude D
- 69 lvl 1: Classic SME03 → Interlude X
- 69 lvl 2: Classic SME03 → Interlude X
- 69 lvl 3: Classic SME03 → Interlude X
- 69 lvl 4: Classic SME03 → Interlude X
- 69 lvl 5: Classic SME03 → Interlude X
- 69 lvl 6: Classic SME03 → Interlude X

### hit_time
- 21 lvl 1: Classic 1.0 → Interlude 4.0
- 21 lvl 2: Classic 1.0 → Interlude 4.0
- 21 lvl 3: Classic 1.0 → Interlude 4.0
- 28 lvl 1: Classic 0.7 → Interlude 1.0
- 28 lvl 2: Classic 0.7 → Interlude 1.0
- 28 lvl 3: Classic 0.7 → Interlude 1.0
- 28 lvl 4: Classic 0.7 → Interlude 1.0
- 28 lvl 5: Classic 0.7 → Interlude 1.0
- 28 lvl 6: Classic 0.7 → Interlude 1.0
- 28 lvl 7: Classic 0.7 → Interlude 1.0
- 28 lvl 8: Classic 0.7 → Interlude 1.0
- 28 lvl 9: Classic 0.7 → Interlude 1.0

### operate_type
- 28 lvl 1: Classic 1 → Interlude 0
- 28 lvl 2: Classic 1 → Interlude 0
- 28 lvl 3: Classic 1 → Interlude 0
- 28 lvl 4: Classic 1 → Interlude 0
- 28 lvl 5: Classic 1 → Interlude 0
- 28 lvl 6: Classic 1 → Interlude 0
- 28 lvl 7: Classic 1 → Interlude 0
- 28 lvl 8: Classic 1 → Interlude 0
- 28 lvl 9: Classic 1 → Interlude 0
- 28 lvl 10: Classic 1 → Interlude 0
- 28 lvl 11: Classic 1 → Interlude 0
- 28 lvl 12: Classic 1 → Interlude 0

### is_magic
- 1016 lvl 1: Classic 4 → Interlude 1
- 1016 lvl 2: Classic 4 → Interlude 1
- 1016 lvl 3: Classic 4 → Interlude 1
- 1016 lvl 4: Classic 4 → Interlude 1
- 1016 lvl 5: Classic 4 → Interlude 1
- 1016 lvl 6: Classic 4 → Interlude 1
- 1016 lvl 7: Classic 4 → Interlude 1
- 1016 lvl 8: Classic 4 → Interlude 1
- 1016 lvl 9: Classic 4 → Interlude 1
- 69 lvl 1: Classic 0 → Interlude 1
- 69 lvl 2: Classic 0 → Interlude 1
- 69 lvl 3: Classic 0 → Interlude 1

### cast_range
- 1073 lvl 1: Classic -1 → Interlude 400
- 1073 lvl 2: Classic -1 → Interlude 400
- 98 lvl 1: Classic 40 → Interlude -1
- 98 lvl 2: Classic 40 → Interlude -1
- 98 lvl 3: Classic 40 → Interlude -1
- 98 lvl 4: Classic 40 → Interlude -1
- 98 lvl 5: Classic 40 → Interlude -1
- 1129 lvl 1: Classic -1 → Interlude 40
- 1129 lvl 2: Classic -1 → Interlude 40
- 1129 lvl 3: Classic -1 → Interlude 40
- 1129 lvl 4: Classic -1 → Interlude 40
- 1129 lvl 5: Classic -1 → Interlude 40

## Only-Classic skill id samples (first 30 of 6265)
1, 5, 6, 7, 8, 9, 15, 17, 18, 19, 24, 25, 28, 30, 35, 36, 46, 48, 50, 54, 65, 69, 70, 78, 84, 92, 94, 95, 96, 97
## Only-Interlude skill id samples (first 30 of 462)
1, 5, 6, 7, 9, 13, 17, 18, 19, 24, 25, 28, 30, 35, 36, 42, 46, 48, 49, 51, 54, 58, 69, 70, 72, 81, 84, 86, 92, 95
