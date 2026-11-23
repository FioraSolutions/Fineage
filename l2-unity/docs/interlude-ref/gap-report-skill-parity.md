# Skill parity — server trees × Interlude client

Learnable player skills come from Mobius CT0 class skill trees. Motion names come from
`docs/interlude-ref/dats/skillgrp.tsv` `ani_char` and `AnimationCombo_Interlude.txt`.
A take is required only when that body's `*_anim` MeshAnimation in the Interlude `.ukx`
dump contains it (interlude ukx MeshAnimation sequences). Cape, cloak, and helmet anim sets are not the skill resolver.

## Headline

- Player skills checked: **622**
- Skill × race-body combos checked: **1520**
- Skills with an animation tag: **514**
- Missing motion (import or silent fallback): **0**
- Silent fallback (different clip than the original): **0**
- Original ukx has no take and no verified int alias: **8**
- SpAtk phase×weapon rows resolved through LineageWarrior.int (clip already shipped, literal sequence absent): **1080**
- Missing effect: **0**
- Missing sound: **0** (2734 skillsoundgrp tokens, 512 skills have a row)

Skill trees: Fineage-Backend data/skill-trees.json (Mobius CT0 class trees). Slim copy: `skills/player-skill-trees.json`.

Full rows: `skill-parity.csv`, `skill-parity.json`, `skill-parity-motions.tsv`.
EditMode allowlist: `skill-parity-known-gaps.txt`.

## Per class

| class | race | skills | motion % | effect % | sound % |
|---|---|---:|---:|---:|---:|
| 0 Human Fighter | Human | 11 | 100.0 | 100.0 | 100.0 |
| 1 Warrior | Human | 16 | 100.0 | 100.0 | 100.0 |
| 2 Gladiator | Human | 25 | 100.0 | 100.0 | 100.0 |
| 3 Warlord | Human | 25 | 100.0 | 100.0 | 100.0 |
| 4 Human Knight | Human | 12 | 100.0 | 100.0 | 100.0 |
| 5 Paladin | Human | 27 | 100.0 | 100.0 | 100.0 |
| 6 Dark Avenger | Human | 24 | 100.0 | 100.0 | 100.0 |
| 7 Rogue | Human | 24 | 100.0 | 100.0 | 100.0 |
| 8 Treasure Hunter | Human | 30 | 100.0 | 100.0 | 100.0 |
| 9 Hawkeye | Human | 21 | 100.0 | 100.0 | 100.0 |
| 10 Human Mystic | Human | 23 | 100.0 | 100.0 | 100.0 |
| 11 Human Wizard | Human | 30 | 100.0 | 100.0 | 100.0 |
| 12 Sorcerer | Human | 30 | 100.0 | 100.0 | 100.0 |
| 13 Necromancer | Human | 35 | 100.0 | 100.0 | 100.0 |
| 14 Warlock | Human | 31 | 100.0 | 100.0 | 100.0 |
| 15 Cleric | Human | 30 | 100.0 | 100.0 | 100.0 |
| 16 Bishop | Human | 37 | 100.0 | 100.0 | 100.0 |
| 17 Prophet | Human | 41 | 100.0 | 100.0 | 100.0 |
| 18 Elven Fighter | Elf | 13 | 100.0 | 100.0 | 100.0 |
| 19 Elven Knight | Elf | 16 | 100.0 | 100.0 | 100.0 |
| 20 Temple Knight | Elf | 28 | 96.4 | 100.0 | 100.0 |
| 21 Sword Singer | Elf | 25 | 100.0 | 100.0 | 100.0 |
| 22 Elven Scout | Elf | 28 | 100.0 | 100.0 | 100.0 |
| 23 Plains Walker | Elf | 32 | 100.0 | 100.0 | 100.0 |
| 24 Silver Ranger | Elf | 24 | 100.0 | 100.0 | 100.0 |
| 25 Elven Mystic | Elf | 22 | 100.0 | 100.0 | 100.0 |
| 26 Elven Wizard | Elf | 26 | 100.0 | 100.0 | 100.0 |
| 27 Spellsinger | Elf | 34 | 100.0 | 100.0 | 100.0 |
| 28 Elemental Summoner | Elf | 31 | 100.0 | 100.0 | 100.0 |
| 29 Elven Oracle | Elf | 30 | 100.0 | 100.0 | 100.0 |
| 30 Elven Elder | Elf | 41 | 100.0 | 100.0 | 100.0 |
| 31 Dark Fighter | DarkElf | 14 | 100.0 | 100.0 | 100.0 |
| 32 Palus Knight | DarkElf | 16 | 100.0 | 100.0 | 100.0 |
| 33 Shillien Knight | DarkElf | 30 | 100.0 | 100.0 | 100.0 |
| 34 Bladedancer | DarkElf | 24 | 95.8 | 100.0 | 100.0 |
| 35 Assassin | DarkElf | 28 | 100.0 | 100.0 | 100.0 |
| 36 Abyss Walker | DarkElf | 30 | 100.0 | 100.0 | 100.0 |
| 37 Phantom Ranger | DarkElf | 24 | 100.0 | 100.0 | 100.0 |
| 38 Dark Mystic | DarkElf | 23 | 100.0 | 100.0 | 100.0 |
| 39 Dark Wizard | DarkElf | 30 | 100.0 | 100.0 | 100.0 |
| 40 Spellhowler | DarkElf | 32 | 100.0 | 100.0 | 100.0 |
| 41 Phantom Summoner | DarkElf | 31 | 100.0 | 100.0 | 100.0 |
| 42 Shillien Oracle | DarkElf | 29 | 100.0 | 100.0 | 100.0 |
| 43 Shillien Elder | DarkElf | 32 | 100.0 | 100.0 | 100.0 |
| 44 Orc Fighter | Orc | 12 | 100.0 | 100.0 | 100.0 |
| 45 Orc Raider | Orc | 19 | 100.0 | 100.0 | 100.0 |
| 46 Destroyer | Orc | 24 | 100.0 | 100.0 | 100.0 |
| 47 Monk | Orc | 12 | 100.0 | 100.0 | 100.0 |
| 48 Tyrant | Orc | 23 | 100.0 | 100.0 | 100.0 |
| 49 Orc Mystic | Orc | 22 | 100.0 | 100.0 | 100.0 |
| 50 Orc Shaman | Orc | 32 | 100.0 | 100.0 | 100.0 |
| 51 Overlord | Orc | 41 | 97.6 | 100.0 | 100.0 |
| 52 Warcryer | Orc | 34 | 97.1 | 100.0 | 100.0 |
| 53 Dwarf Fighter | Dwarf | 13 | 100.0 | 100.0 | 100.0 |
| 54 Scavenger | Dwarf | 13 | 100.0 | 100.0 | 100.0 |
| 55 Bounty Hunter | Dwarf | 17 | 100.0 | 100.0 | 100.0 |
| 56 Artisan | Dwarf | 14 | 100.0 | 100.0 | 100.0 |
| 57 Warsmith | Dwarf | 21 | 100.0 | 100.0 | 100.0 |
| 88 Duelist | Human | 12 | 100.0 | 100.0 | 100.0 |
| 89 Dreadnought | Human | 12 | 100.0 | 100.0 | 100.0 |
| 90 Phoenix Knight | Human | 11 | 100.0 | 100.0 | 100.0 |
| 91 Hell Knight | Human | 11 | 100.0 | 100.0 | 100.0 |
| 92 Sagittarius | Human | 8 | 100.0 | 100.0 | 100.0 |
| 93 Adventurer | Human | 11 | 100.0 | 100.0 | 100.0 |
| 94 Archmage | Human | 8 | 100.0 | 100.0 | 100.0 |
| 95 Soultaker | Human | 13 | 100.0 | 100.0 | 100.0 |
| 96 Arcana Lord | Human | 11 | 100.0 | 100.0 | 100.0 |
| 97 Cardinal | Human | 14 | 100.0 | 100.0 | 100.0 |
| 98 Hierophant | Human | 12 | 100.0 | 100.0 | 100.0 |
| 99 Eva's Templar | Elf | 10 | 100.0 | 100.0 | 100.0 |
| 100 Sword Muse | Elf | 9 | 100.0 | 100.0 | 100.0 |
| 101 Wind Rider | Elf | 11 | 100.0 | 100.0 | 100.0 |
| 102 Moonlight Sentinel | Elf | 9 | 100.0 | 100.0 | 100.0 |
| 103 Mystic Muse | Elf | 9 | 100.0 | 100.0 | 100.0 |
| 104 Elemental Master | Elf | 10 | 100.0 | 100.0 | 100.0 |
| 105 Eva's Saint | Elf | 11 | 100.0 | 100.0 | 100.0 |
| 106 Shillien Templar | DarkElf | 10 | 100.0 | 100.0 | 100.0 |
| 107 Spectral Dancer | DarkElf | 8 | 100.0 | 100.0 | 100.0 |
| 108 Ghost Hunter | DarkElf | 11 | 100.0 | 100.0 | 100.0 |
| 109 Ghost Sentinel | DarkElf | 9 | 100.0 | 100.0 | 100.0 |
| 110 Storm Screamer | DarkElf | 10 | 100.0 | 100.0 | 100.0 |
| 111 Spectral Master | DarkElf | 11 | 100.0 | 100.0 | 100.0 |
| 112 Shillien Saint | DarkElf | 10 | 100.0 | 100.0 | 100.0 |
| 113 Titan | Orc | 11 | 100.0 | 100.0 | 100.0 |
| 114 Grand Khavatari | Orc | 11 | 100.0 | 100.0 | 100.0 |
| 115 Dominator | Orc | 14 | 100.0 | 100.0 | 100.0 |
| 116 Doom Cryer | Orc | 10 | 100.0 | 100.0 | 100.0 |
| 117 Fortune Seeker | Dwarf | 12 | 100.0 | 100.0 | 100.0 |
| 118 Maestro | Dwarf | 11 | 100.0 | 100.0 | 100.0 |

## Motion

`SkillAnimationResolver.ResolvePlayableCombatState` does not substitute `SpAtk01` for
`SpAtk03+`. A missing official take logs `skillId`, race, sex, body, and the expected
clip, then leaves the requested state unplayed (it does not crossfade `SpAtk01_1HS`).
A missing weapon variant that still has a `1HS` animator state logs the same fields and
names the substitute. `GetOverrideClip` still completes a cast on the shared mid/shot clip
when CastLong, CastShort, MagicNoTarget, or MagicThrow is absent; that path logs skill id,
race, sex, and the expected clip, and is counted as a silent fallback above.
A `SpAtkNN` token is a pawn variable (`GetSpAtkNNAnimName` → `SpAtkNNAnimName[CurWeaponType]`),
filled from `system/LineageWarrior.int`, not a sequence literally named `spatkNN`.
`LineageWarriorAnimAlias.tsv` applies that table when the body's ukx has no literal take
and the substituted sequence is in the ukx and in `AnimTween.txt`. The clip is logged
(`[SkillAnimation] LineageWarrior.int alias`) and is not an SpAtk01 fallback.
Rows marked `no_original_take` are slots with no literal sequence and no verified int alias.

8 motion rows. First 80:

| skill | name | body | sex | kind | expected |
|---:|---|---|---|---|---|
| 84 | Poison Blade Dance | MDarkElf | M | no_original_take | `spatk09_*_MDarkElf` |
| 84 | Poison Blade Dance | MDarkElf | M | no_original_take | `spatk26_*_MDarkElf` |
| 84 | Poison Blade Dance | FDarkElf | F | no_original_take | `spatk09_*_FDarkElf` |
| 84 | Poison Blade Dance | FDarkElf | F | no_original_take | `spatk26_*_FDarkElf` |
| 260 | Hammer Crush | MShaman | M | no_original_take | `spatk03_*_MShaman` |
| 260 | Hammer Crush | FShaman | F | no_original_take | `spatk03_*_FShaman` |
| 400 | Tribunal | MElf | M | no_original_take | `spatk03_*_MElf` |
| 400 | Tribunal | FElf | F | no_original_take | `spatk03_*_FElf` |

Evidence for each remaining row:

- `84` Poison Blade Dance MDarkElf `spatk09_*_MDarkElf`: MDarkElf_anim has no SpAtk09 sequence. Every dumped ukx was scanned; spatk09 exists only as SpAtk09_Hand on Orc.ukx (FOrc/MOrc/Skirt_FOrc). LineageWarrior.int [MFighter] does not set this slot for dual (weapon index 3), and substituting the body suffix on the slots it does set does not name a sequence in DarkElf.ukx.
- `84` Poison Blade Dance MDarkElf `spatk26_*_MDarkElf`: MDarkElf_anim has no SpAtk26 sequence. Every dumped ukx was scanned; spatk26 exists only as SpAtk26_Hand on Orc.ukx (FOrc/MOrc/Skirt_FOrc). LineageWarrior.int [MFighter] does not set this slot for dual (weapon index 3), and substituting the body suffix on the slots it does set does not name a sequence in DarkElf.ukx.
- `84` Poison Blade Dance FDarkElf `spatk09_*_FDarkElf`: FDarkElf_anim has no SpAtk09 sequence. Every dumped ukx was scanned; spatk09 exists only as SpAtk09_Hand on Orc.ukx (FOrc/MOrc/Skirt_FOrc). LineageWarrior.int [MFighter] does not set this slot for dual (weapon index 3), and substituting the body suffix on the slots it does set does not name a sequence in DarkElf.ukx.
- `84` Poison Blade Dance FDarkElf `spatk26_*_FDarkElf`: FDarkElf_anim has no SpAtk26 sequence. Every dumped ukx was scanned; spatk26 exists only as SpAtk26_Hand on Orc.ukx (FOrc/MOrc/Skirt_FOrc). LineageWarrior.int [MFighter] does not set this slot for dual (weapon index 3), and substituting the body suffix on the slots it does set does not name a sequence in DarkElf.ukx.
- `260` Hammer Crush MShaman `spatk03_*_MShaman`: MShaman_anim has no SpAtk03 sequence (Elf.ukx and Shaman.ukx have none; spatk03 in the other dumps is Fighter/DarkElf/Dwarf/Orc and raid meshes). LineageWarrior.int [MFighter] names SpAtk03_*_MFighter for 1HS/2HS/dual/pole; that name is not in this body's ukx, so the suffix copy is not applied. The per-race int section is not in the local client dump.
- `260` Hammer Crush FShaman `spatk03_*_FShaman`: FShaman_anim has no SpAtk03 sequence (Elf.ukx and Shaman.ukx have none; spatk03 in the other dumps is Fighter/DarkElf/Dwarf/Orc and raid meshes). LineageWarrior.int [MFighter] names SpAtk03_*_MFighter for 1HS/2HS/dual/pole; that name is not in this body's ukx, so the suffix copy is not applied. The per-race int section is not in the local client dump.
- `400` Tribunal MElf `spatk03_*_MElf`: MElf_anim has no SpAtk03 sequence (Elf.ukx and Shaman.ukx have none; spatk03 in the other dumps is Fighter/DarkElf/Dwarf/Orc and raid meshes). LineageWarrior.int [MFighter] names SpAtk03_*_MFighter for 1HS/2HS/dual/pole; that name is not in this body's ukx, so the suffix copy is not applied. The per-race int section is not in the local client dump.
- `400` Tribunal FElf `spatk03_*_FElf`: FElf_anim has no SpAtk03 sequence (Elf.ukx and Shaman.ukx have none; spatk03 in the other dumps is Fighter/DarkElf/Dwarf/Orc and raid meshes). LineageWarrior.int [MFighter] names SpAtk03_*_MFighter for 1HS/2HS/dual/pole; that name is not in this body's ukx, so the suffix copy is not applied. The per-race int section is not in the local client dump.

## Effects

Cast / flying / target phases are `skill-effect-placement.tsv` (Interlude `skill.usk`
`CastingActions` / `ShotActions` / `ExplosionActions`). `skillgrp.desc` `skill.<group>.<id>`
follows that skill's phases. A class is wired when `effect-prefabs.txt` names it and the
prefab file exists. `extra_eff` is a numeric flag, not an effect class.
A skill-name guess (`effect_class_by_name` in `skill-visuals.tsv`) is not a binding.

`at_power_strike_cs` (lineageeffect.u export 24, Emitter, DrawScale 0.05, two MeshEmitters
and one SpriteEmitter) and `wh_bless_shield_ta` (export 1546, Emitter, DrawScale 0.25,
two MeshEmitters and three SpriteEmitters) are real classes. `skill.usk` has 238
`SkillVisualEffect` objects (ids 4–5204) and neither id 3 nor id 1243. The package
does not reference either class name or export. `skillgrp` desc is empty and `extra_eff`
is 0 for both skills. They are not the retail visual for Power Strike or Bless Shield,
so they are not imported as those skills' prefabs. GlobalEffect id 3 points at
`it_power_striker_composite` and id 1243 at `m_u044_a_v1243_composite`; those are
separate catalog entries, not these emitters.

No effect gaps for learnable skills that name an effect class or a skill.usk phase.

## Sounds

Every skillsoundgrp cast, shot, explode, and race voice token for a learnable skill has a clip under `Resources/Data/Sounds`.

## Mac rebake

No player `*_anim` skill take is missing from `Resources`, so no anim rebake is required
for this report. Body importers (`L2MElfAnimImporter`, `L2FemaleAnimImporter`,
`L2MOrcAnimImporter`) already copy every sequence in the FBX; they do not whitelist
SpAtk numbers. If a later AnimTween row is added, rebake that race with:

```bash
export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client"
./tools/skill-parity/rebake-player-anims.sh MFighter
```

Re-run: `python3 tools/skill-parity/generate_skill_parity.py`.
