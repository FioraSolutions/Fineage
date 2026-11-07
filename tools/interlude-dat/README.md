# Interlude DAT tooling (skillsoundgrp)

Decode Interlude `system/skillsoundgrp.dat` (Lineage2Ver413) and emit the cast/shot/voice table Fineage loads at runtime.

## Pipeline

1. Decrypt (open-l2encdec 1.5.0 Darwin, protocol 413, legacy RSA):

```bash
./l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec -c decode -p 413 -l \
  ../../ref/l2-interlude-client/system/skillsoundgrp.dat \
  -o dec-skillsoundgrp.dat
```

2. Parse (schema = `skillsoundgrp.ddf` from L2FileEdit Interlude defs):

```bash
python3 parse_skillsoundgrp.py
```

Writes:

- `skillsoundgrp.json` / `skillsoundgrp.tsv` — human dumps
- `../../l2-unity/Assets/Resources/Data/SkillSoundGrp.json` — runtime SoT for `SkillCastSound`

## Runtime

`SkillCastSound` phases from skillsoundgrp:

| Slot | When | Wind Strike |
|------|------|-------------|
| `cast` + sub voice | MagicSkillUse start | `wind_strike_cast` + `Element` |
| `shot` + throw voice | HitTime − flight (projectile leave) | `wind_strike_shot` + `Shot` |
| `explode` | projectile impact (~HitTime) | `wind_strike_explotion` |

Cast/shot stems may differ (e.g. Acumen: `empower_cast` + `acumen_shot`).
