# Interlude whitelist

The client may only ship player animations, skill visuals and effect classes that exist in the original
Interlude client. These files are the reference list; they are generated, not hand-edited.

Regenerate:

```
python3 tools/interlude-whitelist/build_whitelist.py \
  --client "<Interlude client>/Lineage II" \
  --psa-dir <umodel PSA export of animations/*.ukx> \
  --l2encdec tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec
```

The PSA export is `umodel -game=l2 -export -nomesh -notex -nostat -out=<psa-dir> animations/<Race>.ukx`
for Fighter, Magic, Elf, DarkElf, Dwarf, Orc, Shaman. Only the ANIMINFO chunk is read.

| file | source | content |
|---|---|---|
| `player-anim-takes.tsv` | `animations/<Race>.ukx` (PSA ANIMINFO) | every player animation sequence, frames, rate (2053 rows, 14 race/sex sets + skirt sets) |
| `engine-anim-names.txt` | `system/Engine.u` | animation names the engine requests (`Get<Name>AnimName` natives) |
| `lineageeffect-classes.tsv` | `system/lineageeffect.u` | 863 effect classes |
| `script-classes-all-packages.tsv` | all `system/*.u` | every script class, per package |
| `skill-visual-effects.tsv` | `animations/skill.usk` | 243 SkillVisualEffect objects, 522 effect actions (class, attach method, bone, delay, scale) |
| `skillgrp-visual.tsv` | `system/skillgrp.dat` (Ver413) | 2630 skill ids: animation tag, visual reference, whether skill.usk has it |
| `removed.tsv` | diff of the client against the files above | what was removed from the Unity client and why |

Decoding: `Lineage2Ver111/121` packages are a 28-byte header followed by a body XORed with
`body[0] ^ 0xC1`; `skillgrp.dat` (Ver413) is decoded with l2encdec.

## Known gaps

- About 1730 Interlude skills have no `skill.usk` entry; their visuals are a table compiled into
  `engine.dll`, which is packed and cannot be read. The existing GlobalEffect rows for those skills
  were kept: their composites use only classes from `lineageeffect-classes.tsv`, but the
  skill-to-visual mapping itself is not verifiable from Interlude data.
- Interlude data has no animation-tag -> clip table. `AnimationCombo_Interlude.txt` is the previous
  combo table filtered to the 31 tags Interlude skillgrp uses; every clip name in it exists in
  `player-anim-takes.tsv`/`engine-anim-names.txt`. Tags `MS01` and empty have no row.
- Soulshot/spiritshot glow (`soul_N_stick`, `spirit_N_stick`, `shot_N_atk_v1`) is not Interlude but
  is still shipped: removing it would leave shots without any visual. The Interlude classes
  `it_soul_shot_d_ca` / `it_spirit_shot_d_ca` must be generated first.
- MMagic and FMagic have no Interlude SpAtk01 1HS/2HS/Bow takes; those override entries are cleared and
  the states play the template clip (see `removed.tsv`, category `animator-controller`).
- Only player animations are whitelisted. NPC/monster meshes and their `AnimTween.txt` rows were not
  checked against Interlude `.ukx` packages.
- GlobalEffect rows added by #1623 through `player_vfx_manifest.json` aliases keep their composites
  (all built from Interlude classes); manifest entries whose classes are not in `lineageeffect.u`
  were removed from the manifest.
