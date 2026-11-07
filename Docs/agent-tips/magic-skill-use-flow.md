# MagicSkillUse client flow (Interlude SoT)

## What we can / cannot extract from the original client

| Source | Available | Role |
|--------|-----------|------|
| Mobius CT0 `Creature.beginCast` / `doCast` | Yes (`ref/mobius-ct0-interlude`) | Packet order: `MagicSkillUse` (+ gauge) at cast **start**; hit later on timer |
| Mobius `AttackStanceTaskManager` | Yes | `AutoAttackStart` / `AutoAttackStop` = **combat stance flag only** (enter combat / 15 s idle). Not tied to cast anim |
| Mobius `abortAttack` | Yes | Sends `ActionFailed` if mid-swing — **does not** send `AutoAttackStop` |
| Interlude `system/skillsoundgrp.dat` | Decoded (Ver413 / open-l2encdec) | `*_cast` @ start, `*_shot` @ leave (HitTime−flight), `*_explotion` @ impact → `SkillSoundGrp.json` |
| Interlude UE client C++ / UnrealScript | **Not** in repo | Full client presentation source is closed; do not invent from vibes |

So: server/packet semantics from Mobius; SFX table from skillsoundgrp; body anim from skillgrp + our CastClipPlayable. There is no checkable “full UE cast state machine” under `ref/`.

## Canonical client channels (do not cross-wire)

```
MagicSkillUse ──► MagicSkillPresentation
                    ├─ charge / potion early-out
                    ├─ stop move (LocalCastStop) + CastMoveLock
                    ├─ SkillCastSound (cast+voice now; shot @ HitTime−flight; explode @ impact)
                    ├─ CastBar (local player)
                    └─ Intention MAGIC / PHYSICAL → SkillExecutor / CastClipPlayable

AutoAttackStart/Stop ──► PlayerEntity.isAutoAttack only
                         Idle pose reads flag (wait vs atkwait)
                         NEVER ChangeIntention / CrossFade / WAIT_RETURN

SocialAction ──► LevelUp bus (id 15) + social01 gesture
                 Blocked while AnimChannel.Cast is held
```

## Why the old “ifs” felt like hacks

`AutoAttackStop` was wrongly used to force idle CrossFade. Zone can fan stance next to `MagicSkillUse`; that **must not** cancel CastMid. Stance and cast are independent in Mobius — keep them independent here.

## skillsoundgrp decode

```
# from tools/interlude-dat/
./l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec -c decode -p 413 -l \
  ../../ref/l2-interlude-client/system/skillsoundgrp.dat -o dec-skillsoundgrp.dat
python3 parse_skillsoundgrp.py   # writes Resources/Data/SkillSoundGrp.json
```

Schema: `ellysh/L2FileEdit` `DAT_defs/Interlude/skillsoundgrp.ddf` (local copy: `skillsoundgrp.ddf`).
