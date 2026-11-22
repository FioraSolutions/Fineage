# Skill-effect placement gap report

Built from the lossless skill.usk / lineageeffect.u dump and compared to the
client resolver (SkillVisualCatalog + SkillVisualPlayback) as of this change.
Field names are the original Unreal properties, not invented aliases.

- SkillVisualEffect entries with actions: 238
- Action rows (LocateEffect): 522

## Features the original data has that the client does not fully use

| Feature | Actions | Skills | Notes |
|---|---:|---:|---|
| `b_use_character_rotation` | 13 | 13 | Serialized on the LocateEffect. Playback now sets CompositePart.inheritRotation from this flag. |
| `b_size_scale` | 107 | 91 | Serialized on the LocateEffect. Playback applies LineageEffect DrawScale as CompositePart.scale, but still does not scale the effect with the pawn when this flag is set (Notify() is packed). |
| `attach_on_raw rf/lf (feet)` | 0 | 0 | Interlude EAttachMethod EAM_RF/EAM_LF. Cruma remaps them to AM_RIGHT_FOOT=101 and AM_LEFT_FOOT=102. TryMapAttach now accepts 101/102 as named bones; it logs attach_on=N without bone when AttachBoneName is empty. Interlude skill.usk has zero such rows. |
| `b_relative_to_cylinder effective true` | 491 | 234 | Default in Engine.u is true. Playback now treats relative offsets with max \|component\| <= 5 as cylinder units from the attach point (Unity X = UE Y * radius, Unity Y = UE Z * half-height, Unity Z = UE X * radius) so (0,0,-1) from CasterCenter lands at the feet. Larger relative offsets convert as UU and are logged. This convention is inferred: the native Notify() formula is in packed Engine.dll and was not dumped. |
| `b_relative_to_cylinder explicitly false` | 31 | 10 | These actions overrode the default and want a non-cylinder offset. |
| `b_absolute` | 16 | 13 | Serialized on the LocateEffect. The catalog only uses this to force Immediate spawn timing when a delay is also set; it does not change the position/orientation space. |
| `spawn_delay != 0` | 34 | 17 | Maps to CompositePart.spawnDelaySeconds. Already applied when delay > 0. |
| `specific_stage != 0` | 0 | 0 | SkillActionInfo.SpecificStage from SkillVisualEffect. The client has no stage filter. |
| `channeling phase` | 6 | 3 | ChannelingActions exist in Interlude skill.usk (a few skills). The catalog has a CHANNELING phase but playback does not keep the effect for the channel duration. |
| `draw_scale present on effect class` | 454 | 221 | LineageEffect class default DrawScale (for example Wind Strike ca=0.25, pr/ta=0.1). Playback now uses it as CompositePart.scale when the value is greater than 0. |
| `Rotation present on effect class` | 63 | 55 | LineageEffect default Rotation (Unreal rotator units). Playback now applies it as CompositePart.spawnEuler via VectorUtils.ConvertRotToUnity. |
| `bRotEmitter` | 88 | 70 | LineageEffect default. Not applied by playback. |
| `bSelfRotation` | 5 | 5 | LineageEffect default. Not applied by playback. |
| `bDirectional` | 353 | 193 | LineageEffect default. Not applied by playback. |
| `projectile Speed/AccSpeed` | 17 | 17 | LineageEffect projectile defaults (Wind Strike and Prominence flying shots use Speed=1000, AccSpeed=3000). Playback uses distance/FlyingTime when FlyingTime > 0, otherwise Speed converted from UU; AccSpeed is still not applied. |
| `effect class defaults missing` | 2 | 2 | The LocateEffect.EffectClass was not found in the lineageeffect.u class-default dump. |
| `attach bone specified` | 22 | 18 | AttachBoneName is used when attach_on is bone or alias. Already parsed into the catalog Bone field. |
| `RotPerSecond present` | 63 | 44 | LineageEffect default RotPerSecond. Not applied by playback. |

## Fields confirmed absent from Interlude skill.usk actions

Cruma's SkillEffects.Action exposes scale, turn, curve, forceLife,
adjustLife, chaining and channeling as Action fields. A full pass over the
243 Interlude listVisuals() entries found no non-default values for those
fields. They are not properties of SkillAction_LocateEffect in Engine.u and
are not written into this placement table. Duration and loop flags also do not
exist on the LocateEffect; lifetime lives on the emitter (AutoDestroy, emitter
Lifetime ranges) inside lineageeffect.u.

PreshotActions is declared on SkillVisualEffect but has zero rows in this
client. UnionTarget is a Cruma phase that Interlude's SkillVisualEffect does
not have.

## Wind Strike, Prominence, Vortex

### Wind Strike

| phase | effect | attach raw | offset | flags | actor |
|---|---|---|---|---|---|
| casting | `el_wind_strike_ca` | 5 `trail` | (0, 0, -1) | rel_cyl size_scale | DrawScale=0.25 |
| casting | `el_wind_strike_pr` | 5 `trail` | (1, 0, 0) | rel_cyl use_char_rot | DrawScale=0.100000001 |
| shot | `el_wind_strike_fl` | 0 `none` | (1, 0, 0) | rel_cyl | DrawScale=0.0500000007, Speed=1000.0, AccSpeed=3000.0, bDirectional |
| explosion | `el_wind_strike_ta` | 5 `trail` | (0, 0, 0) | rel_cyl on_target | DrawScale=0.100000001, bDirectional |

### Prominence

| phase | effect | attach raw | offset | flags | actor |
|---|---|---|---|---|---|
| casting | `el_prominence_ca` | 5 `trail` | (1, 0, 0) | rel_cyl | bRotEmitter, bDirectional |
| shot | `el_prominence_fl` | 0 `none` | (0, 0, 0) | rel_cyl | Speed=1000.0, AccSpeed=3000.0, bRotEmitter, bDirectional |
| shot | `el_prominence_cs` | 5 `trail` | (1, 0, 0) | rel_cyl | bRotEmitter, bDirectional |
| explosion | `el_prominence_ta` | 5 `trail` | (0, 0, 0) | rel_cyl on_target | bRotEmitter, bDirectional |

### Dryad Root (skill.sp.1201, used by Vortex 1339-1343 via skillgrp desc)

| phase | effect | attach raw | offset | flags | actor |
|---|---|---|---|---|---|
| casting | `sp_agility_ca` | 5 `trail` | (0, 0, -1) | rel_cyl | DrawScale=0.100000001, bRotEmitter, bDirectional |
| shot | `sp_dryad_root_ta` | 5 `trail` | (0, 0, 0) | rel_cyl on_target multi | DrawScale=0.100000001, bDirectional |

Vortex skills 1339-1343 are not in skill.usk. Their skillgrp desc is
skill.sp.1201 (Dryad Root), so they share that visual: a casting effect at
offset (0,0,-1) with bRelativeToCylinder and an on-target shot/impact.

## Skill sounds

Every skillsoundgrp token (spell, shot, explosion, and hero voice) and every
LineageEffect SpawnSound / Sound object ref is in
`skills/skill-effect-sounds.tsv` (and `.json`). Nothing is dropped.
skillsoundgrp `spelleffect_sound_1/2/3` are the Interlude cast / launch /
impact cues (Wind Strike: `SkillSound.wind_strike_cast` / `_shot` /
`_explotion`). `shoteffect_*` and `expeffect_*` are additional official
slots. Hero voice is the `*_sub` (cast) and `*_throw` (release) columns.
World, ambient and footstep audio is tracked separately in Fineage #1670
and is not part of this table.

- Sound rows: 12740
- From skillsoundgrp: 12718
- From LineageEffect SpawnSound: 22
- Other lineageeffect Sound refs not reached from skill.usk: 0

| Feature | Rows | Skills | Notes |
|---|---:|---:|---|
| `skillsoundgrp shoteffect_* not played` | 0 | 0 | Official shoteffect_sound_1/2/3 cues. SkillCastSound now plays every non-empty shoteffect slot at release with the table volume and radius. |
| `skillsoundgrp expeffect_* not played` | 0 | 0 | Official expeffect_sound_1/2/3 cues. SkillCastSound now plays every non-empty expeffect slot at impact with the table volume and radius. |
| `lineageeffect SpawnSound not played` | 0 | 0 | Emitter.SpawnSound is wired into CompositeEffectV2 spawn via SkillEffectSpawnSound, using the table volume and radius. Missing clips log package+name. |
| `SpawnSound flight loop not played` | 0 | 0 | SpawnSound with SoundLooping on a shot-phase effect. None of the Interlude class defaults use a looping flight cue. |
| `skillsoundgrp spell/shot/explode played but clip missing` | 13 | 11 | SkillCastSound tries to play the stem under Resources/Data/Sounds/SkillSound (or the matching package folder) and logs the exact package and name when the clip is absent. The 11 rows are listed below. |
| `hero voice mapped but clip missing` | 0 | 0 | SkillCastSound.PlayVoiceKind will look up Data/Sounds/SkillVoice/{racePrefix}_{kind} and miss. |
| `skillsoundgrp hero-voice volume/radius unused` | 10108 | 379 | Hero voice still uses SkillCastSound.VoiceVolume=1. Effect cues (cast/shot/flight/impact/exp, including shoteffect, expeffect, and SpawnSound) now use the table volume (vol/255) and radius (UU/52.5 meters). |
| `SpawnSound clip missing` | 8 | 0 | SkillEffectSpawnSound logs the exact package and name when the clip is not in Resources. ItemSound2 firework cues are the known leftovers. |
| `lineageeffect SpawnSound not referenced by skill.usk` | 22 | 0 | These Emitter.SpawnSound cues exist on LineageEffect class defaults (teleport, Baium, Frintezza drops, fireworks, meteor, bubbles) but no skill.usk LocateEffect points at that class. They are still in the sound table so the dump is lossless. |

### Missing skill-sound clips

SkillCastSound logs `package` and `name` when a table cue has no clip.
These 13 skillsoundgrp effect rows are played from the
table but have no imported .ogg/.wav:

| skill_id | level | slot | package | name |
|---:|---:|---|---|---|
| 323 | 1 | `spelleffect_sound_2` | `ItemSound` | `itemdrop_wood` |
| 324 | 1 | `spelleffect_sound_2` | `ItemSound` | `itemdrop_wood` |
| 419 | 1 | `spelleffect_sound_2` | `ItemSound2` | `broken_key` |
| 1387 | 1 | `spelleffect_sound_2` | `ItemSound` | `itemdrop_bone` |
| 2187 | 1 | `spelleffect_sound_1` | `EtcSound` | `battle` |
| 2272 | 1 | `spelleffect_sound_1` | `EtcSound` | `VVKOREA` |
| 4110 | 1 | `shoteffect_sound_2` | `SkillSound2` | `antaras_creak` |
| 4110 | 1 | `spelleffect_sound_2` | `SkillSound2` | `antaras_creak` |
| 4141 | 1 | `spelleffect_sound_3` | `SkillSound4` | `doublewind_explotion` |
| 4142 | 1 | `spelleffect_sound_3` | `SkillSound4` | `doublewind_explotion` |
| 4231 | 1 | `spelleffect_sound_3` | `MonSound` | `Hit_Wet_5` |
| 4231 | 1 | `spelleffect_sound_2` | `MonSound` | `Hit_Wet_5` |
| 4606 | 1 | `shoteffect_sound_2` | `MonSound3` | `antars_ground_wave` |

### Wind Strike, Prominence, Vortex sounds

#### Wind Strike

| phase | slot | token | Fineage plays | asset |
|---|---|---|---|---|
| casting | `spelleffect_sound_1` | `SkillSound.wind_strike_cast` | yes | yes |
| explosion | `spelleffect_sound_3` | `SkillSound.wind_strike_explotion` | yes | yes |
| shot | `spelleffect_sound_2` | `SkillSound.wind_strike_shot` | yes | yes |
| voice_cast | `*_sub` | 14 race slots (`chrsound.f_darkelf_element`, `chrsound.f_dwarf_type_b`, `chrsound.f_elf_element`, `chrsound.f_hfighter_element`, `chrsound.f_hmagician_element`, `chrsound.f_orc_fighter_type_b`, `chrsound.f_orc_magician_type_b`, `chrsound.m_darkelf_element`, `chrsound.m_dwarf_type_b`, `chrsound.m_elf_element`, `chrsound.m_hfighter_element`, `chrsound.m_hmagician_element`, `chrsound.m_orc_fighter_type_b`, `chrsound.m_orc_magician_type_b`) | yes | — |
| voice_shot | `*_throw` | 14 race slots (`chrsound.f_darkelf_shot`, `chrsound.f_dwarf_shot`, `chrsound.f_elf_shot`, `chrsound.f_hfighter_shot`, `chrsound.f_hmagician_shot`, `chrsound.f_orc_fighter_shot`, `chrsound.f_orc_magician_shot`, `chrsound.m_darkelf_shot`, `chrsound.m_dwarf_shot`, `chrsound.m_elf_shot`, `chrsound.m_hfighter_shot`, `chrsound.m_hmagician_shot`, `chrsound.m_orc_fighter_shot`, `chrsound.m_orc_magician_shot`) | yes | — |

#### Prominence

| phase | slot | token | Fineage plays | asset |
|---|---|---|---|---|
| casting | `spelleffect_sound_1` | `SkillSound.blaze_cast` | yes | yes |
| explosion | `spelleffect_sound_3` | `SkillSound.blaze_explotion` | yes | yes |
| shot | `spelleffect_sound_2` | `SkillSound.blaze_shot` | yes | yes |
| voice_cast | `*_sub` | 14 race slots (`chrsound.f_darkelf_element`, `chrsound.f_dwarf_type_b`, `chrsound.f_elf_element`, `chrsound.f_hfighter_element`, `chrsound.f_hmagician_element`, `chrsound.f_orc_fighter_type_b`, `chrsound.f_orc_magician_type_b`, `chrsound.m_darkelf_element`, `chrsound.m_dwarf_type_b`, `chrsound.m_elf_element`, `chrsound.m_hfighter_element`, `chrsound.m_hmagician_element`, `chrsound.m_orc_fighter_type_b`, `chrsound.m_orc_magician_type_b`) | yes | — |
| voice_shot | `*_throw` | 14 race slots (`chrsound.f_darkelf_throw`, `chrsound.f_dwarf_throw`, `chrsound.f_elf_throw`, `chrsound.f_hfighter_throw`, `chrsound.f_hmagician_throw`, `chrsound.f_orc_fighter_throw`, `chrsound.f_orc_magician_throw`, `chrsound.m_darkelf_throw`, `chrsound.m_dwarf_throw`, `chrsound.m_elf_throw`, `chrsound.m_hfighter_throw`, `chrsound.m_hmagician_throw`, `chrsound.m_orc_fighter_throw`, `chrsound.m_orc_magician_throw`) | yes | — |

#### Dryad Root (Vortex 1339-1343 share the visual, not this sound row)

| phase | slot | token | Fineage plays | asset |
|---|---|---|---|---|
| casting | `spelleffect_sound_1` | `SkillSound4.dryad_root_cast` | yes | yes |
| shot | `spelleffect_sound_2` | `SkillSound4.dryad_root_shot` | yes | yes |
| voice_cast | `*_sub` | 14 race slots (`chrsound.f_darkelf_element`, `chrsound.f_dwarf_type_b`, `chrsound.f_elf_element`, `chrsound.f_hfighter_element`, `chrsound.f_hmagician_element`, `chrsound.f_orc_fighter_type_b`, `chrsound.f_orc_magician_type_b`, `chrsound.m_darkelf_element`, `chrsound.m_dwarf_type_b`, `chrsound.m_elf_element`, `chrsound.m_hfighter_element`, `chrsound.m_hmagician_element`, `chrsound.m_orc_fighter_type_b`, `chrsound.m_orc_magician_type_b`) | yes | — |
| voice_shot | `*_throw` | 14 race slots (`chrsound.f_darkelf_throw`, `chrsound.f_dwarf_throw`, `chrsound.f_elf_throw`, `chrsound.f_hfighter_throw`, `chrsound.f_hmagician_throw`, `chrsound.f_orc_fighter_throw`, `chrsound.f_orc_magician_throw`, `chrsound.m_darkelf_throw`, `chrsound.m_dwarf_throw`, `chrsound.m_elf_throw`, `chrsound.m_hfighter_throw`, `chrsound.m_hmagician_throw`, `chrsound.m_orc_fighter_throw`, `chrsound.m_orc_magician_throw`) | yes | — |

#### Vortex (1341)

| phase | slot | token | Fineage plays | asset |
|---|---|---|---|---|
| casting | `spelleffect_sound_1` | `SkillSound5.clear_vision_cast` | yes | yes |
| shot | `spelleffect_sound_2` | `SkillSound5.clear_vision_shot` | yes | yes |

Vortex 1339-1343 have their own skillsoundgrp rows
(`SkillSound5.clear_vision_cast` / `_shot`), not Dryad Root's sounds,
even though they reuse the skill.sp.1201 visual.

## How to regenerate

```bash
tools/interlude-extract/export_package_properties.sh
python3 tools/interlude-extract/build_skill_effect_placement.py
```

