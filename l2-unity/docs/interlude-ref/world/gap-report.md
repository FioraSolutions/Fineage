# Interlude world / effects / sounds gap report

Derived from a lossless property dump of every official Interlude `.unr` map,
every `.uax` sound package, every `.ukx` animation package, and `lineageenv` /
`skylevel` / `interface` / `nwindow`. Compared against what is present in the
Fineage tree (map tiles, ambient prefabs/JSON, audio clips, FMOD banks, and
runtime scripts). No guessed asset lists.

Source issue: [#1670](https://github.com/FioraSolutions/Fineage/issues/1670).
Skill-visual leftovers stay in [skills/gap-report.md](../skills/gap-report.md) and #1674.

## Headline counts

- Map packages dumped: **153** grid tiles + env levels (`Lobby`, `entry`, `skylevel`, `ship_position`). Extract failures: **0**.
- Interesting placed actors (emitters, ambient sounds, music volumes, lights, zone/sky/celestial, volumes): **250158**.
- Sound waves in `.uax` packages: **5130** across 25 packages.
- Sound / music references discovered (maps, anim notifies, items, UI, events): **652154**.
- Animation notifies: **20801**. MeshAnimation sequences: **6633**.
- `musicinfo` tracks: **205** unique names in **358** ids.
- Skill-sound table rows already imported (see skills/gap-report.md): **12740**.

### Original sound clips vs Fineage playback assets

| Fineage status | Original `.uax` waves |
|---|---:|
| `resources_clip` | 5130 |
| `fmod_bank` | 0 |
| `missing` | 0 |
| **total** | 5130 |

Status is discovered, not assumed: a wave counts as present if a Resources/StreamingAssets
`.ogg`/`.wav` stem matches, or the wave name appears in the matching FMOD `.bank`
(`AmbSound` 6421472 bytes, `Music` 11142656,
`StepSound` 60416, `InterfaceSound` 411904,
`ChrSound` 686624, `MonSound` 1346080,
`ItemSound` 58976, `SkillSound` 18688).
Fineage also has **5148** loose clips under `Assets/Resources/Data/Sounds` (item, monster, character, skill, step, UI, and ambient waves).

This client install has **no loose `.ogg` music folder**; music lives only in `musicinfo.dat`
plus Fineage's `Music.bank`. Track files themselves were not in `~/Downloads/L2_Interlude_Client`.

### Interesting actor classes (all maps + env)

| Class | Count |
|---|---:|
| `AmbientSoundObject` | 185275 |
| `Light` | 47405 |
| `SpriteEmitter` | 6366 |
| `Emitter` | 4484 |
| `BlockingVolume` | 1870 |
| `ZoneInfo` | 930 |
| `L2FogInfo` | 604 |
| `MeshEmitter` | 575 |
| `WaterVolume` | 467 |
| `Mover` | 415 |
| `MusicVolume` | 410 |
| `PhysicsVolume` | 220 |
| `NMoon` | 161 |
| `LevelInfo` | 157 |
| `SkyZoneInfo` | 156 |
| `NSun` | 156 |
| `NMovableSunLight` | 155 |
| `TerrainInfo` | 154 |
| `BeamEmitter` | 81 |
| `DefaultPhysicsVolume` | 47 |
| `VertMeshEmitter` | 39 |
| `ConvexVolume` | 25 |
| `Spotlight` | 4 |
| `AmbientSound` | 2 |

### Animation notify classes

| Class | Count |
|---|---:|
| `AnimNotify_Sound` | 17906 |
| `AnimNotify_AttackShot` | 1472 |
| `AnimNotify_AttackItem` | 758 |
| `AnimNotify_AttackVoice` | 226 |
| `AnimNotify_Effect` | 223 |
| `AnimNotify_ViewShake` | 90 |
| `AnimNotify_AttackPreShot` | 78 |
| `AnimNotify_BoneScale` | 18 |
| `AnimNotify_SwimSound` | 14 |
| `AnimNotify_Channeling` | 14 |
| `AnimNotify_ScreenFade` | 2 |

### Reference kinds

| Kind | Count |
|---|---:|
| `anim_notify` | 441197 |
| `map_actor` | 185583 |
| `item` | 15509 |
| `npc_effect` | 6536 |
| `enter_event` | 2263 |
| `ui_systemmsg` | 545 |
| `map_music` | 409 |
| `package` | 112 |

## Fineage world coverage

- Official grid maps: **153**. Fineage map tiles under `Resources/Data/Maps`: **41**.
- Tiles with `*_AmbientSounds.prefab`: **32**.
- Tiles with committed `*_AmbientSounds.json`: **9** (plus Talking Island aggregate).
- Tiles with `MusicArea.prefab`: **2** ['16_25', '17_25'].
- Runtime: AmbientSoundEmitter=yes, interval L2Footsteps=no, AnimNotifyPlayback=yes, L2TownAmbience hardcoded beds=yes, DayNightCycle=yes, SkillCastSound=yes, CharacterAnimationAudioHandler=yes.

## Gaps by category

### 1. World effects (map Emitter / SpriteEmitter / xEmitter)
- Original placed emitter actors: **11545** on **70** maps/levels.
- Fineage `Resources/Data/Maps` contains **no** dedicated emitter/particle prefabs or Meta JSON for world emitters.
- `L2TownAmbience` hardcodes three FMOD loops (`fire_small_05`, `waterfall_big_02`, `stream_small_03`) 
  because, in its own comment, "the map export dropped" original beds. That is a silent substitute,
  not a dump-backed placement.
- Starting-zone emitter counts are listed below. Full table: `world/map-actors.tsv` (class contains Emitter).

### 2. Ambient sounds (AmbientSound / AmbientSoundObject)
- Original ambient actors: **185277**, all shipped in `StreamingAssets/Data/Meta/Sounds/official-ambient-objects.tsv.gz` (grid dumps plus `Lobby` from `map-actors.tsv.gz`).
- Runtime playback is `OfficialAmbientSoundPlayback`: official Resources clips, authored type / volume / radius / pitch / start, day/night `AmbientSoundType`. `L2TownAmbience` name-matching is disabled when the official table is present, and baked `AmbientSoundEmitter` FMOD events are not started.
- Official `ambsound` (393) and `AmbSound2` (34) waves are Resources clips. A missing wave logs package + name and is not replaced by an FMOD event.

### 3. Music (MusicVolume + musicinfo)
- Original `MusicVolume` actors: **410** using **356** distinct `nMusicID` values.
- `musicinfo` defines **205** unique track names.
- Fineage `MusicArea.prefab` exists only on: ['16_25', '17_25'].
- Talking Island village volumes use `nMusicID=172501` → `NT_Speaking`. That mapping is in `sounds/musicinfo-tracks.tsv`.
- No official loose `.ogg` music was present in this client tree; `Music.bank` (11 MB) is the only Fineage music container.

### 4. Footsteps and weapon-swing / hit notifies (`*.ukx`)
- `AnimNotify_Sound` (footsteps / draw / swing beds with per-surface arrays): **17906**.
- `AnimNotify_AttackShot` / `AttackItem` / `AttackPreShot` / `AttackVoice`: **2534**.
- Swim / channeling notifies: **28**.
- Sequence bindings (time + notify object on each MeshAnimation sequence): **21095** (dump: `sounds/anim-sequence-notifies.tsv.gz`). Times are sequence-normalized in 0..1.
- Fineage plays those bindings through `AnimNotifyPlayback` (one path, official times, volume, radius,
  and per-surface arrays when the notify's primary wave is in `stepsound`).
- `stepsound` waves in Fineage: all **93** official clips under `Resources/Data/Sounds/StepSound/`.
- MeshAnimation sequences also carry `shot_time` / `pre_shot_time` / `channel_times` (dump: `sounds/anim-sequences.tsv`).

### 5. UI sounds
- `interfacesound.uax` + `messengersound.uax` waves: **15**, Fineage-missing: **0**. Official clips are under `Data/Sounds/InterfaceSound/` and `Data/Sounds/MessengerSound/`. System-message `item_sound` tokens come from Interlude `systemmsg-e.tsv` (545 references).
- `systemmsg-e.dat` item_sound references: **545**.
- Fineage `InterfaceSound.bank` is 411904 bytes — far smaller than the original wave set.

### 6. Item, NPC, and character voice / monster sounds
- Item DAT sound fields (etc/armor/weapon): **15509** references.
- Enter-event sound/effect fields: **2263**.
- Monster and character waves live in `monsound*.uax` and `chrsound*.uax` (see catalog).
- Official `itemsound*` / `EtcSound` / `monsound*` / `chrsound*` waves (**4257**) are imported as Resources clips under `Data/Sounds/{ItemSound,MonSound,Voice,SkillVoice}`. The runtime index is `StreamingAssets/Data/Meta/Sounds/official-waves.tsv`. Official `ambsound` / `AmbSound2` waves are imported as Resources clips under `Data/Sounds/AmbSound/` and `Data/Sounds/AmbSound2/` (#1678).

### 7. Skill effects and skill sounds
- Already covered by the lossless `skill.usk` / `lineageeffect.u` dump and `skills/gap-report.md`.
- `12740` skill-sound rows are in the derived skill table. Leftovers: issue #1674.
- This report does not reopen that work except to count those rows in the overall inventory.

### 8. Sky, day/night, fog, and lights
- Lights: **47405**. Celestial actors (NSun/NMoon/NMovableSunLight): **472**. SkyZoneInfo: **156**. ZoneInfo+LevelInfo: **1087**.
- Fineage has `DayNightCycle`, `L2CelestialDiscs`, `L2CloudMyst`, and `L2WorldAmbientLUT.csv` / `TimeEnv0.int`.
- `skylevel.unr` is dumped under `world/env/` (sun, moons, lens flares, zone fog). Compare those properties to the
  LUT rather than assuming the LUT is complete. Map `ZoneInfo` fog and `NSun` radii are per-tile in `map-actors.tsv`.

### Original `.uax` waves missing from Fineage, by package

| Package | Waves | Missing in Fineage |
|---|---:|---:|
| `ambsound` | 393 | 0 |
| `AmbSound2` | 34 | 0 |
| `interfacesound` | 11 | 0 |
| `messengersound` | 4 | 0 |
| `monsound` | 1499 | 0 |
| `chrsound` | 1029 | 0 |
| `monsound5` | 406 | 0 |
| `itemsound` | 330 | 0 |
| `MonSound6` | 243 | 0 |
| `monsound2` | 193 | 0 |
| `monsound4` | 174 | 0 |
| `monsound3` | 122 | 0 |
| `monsound7` | 117 | 0 |
| `stepsound` | 93 | 0 |
| `ItemSound2` | 65 | 0 |
| `chrsound2` | 40 | 0 |
| `ItemSound3` | 33 | 0 |
| `EtcSound` | 6 | 0 |
| `skillsound` | 74 | 0 |
| `skillsound2` | 53 | 0 |
| `SkillSound3` | 65 | 0 |
| `skillsound4` | 62 | 0 |
| `SkillSound5` | 41 | 0 |
| `skillsound6` | 8 | 0 |
| `skillsound7` | 35 | 0 |

## Starting zones (play first)

Tiles listed here follow `zonename-e.dat` plus immediately adjacent shipped tiles.

### Talking Island

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `16_24` | Talking Island, Western Territory | 922 / 0 / 3 / 3 | yes | yes | 920 | no |
| `16_25` | Elven Ruins; Obelisk of Victory; Singing Waterfall; Talking Island, Western Territory | 1471 / 38 / 6 / 32 | yes | yes | 1465 | yes |
| `16_26` | Talking Island, Western Territory | 291 / 0 / 0 / 3 | yes | no | 291 | no |
| `17_24` | Talking Island, Eastern Territory | 291 / 0 / 0 / 3 | yes | yes | 291 | no |
| `17_25` | Talking Island Village; Cedric's Training Hall; Einhovant's School of Magic; Talking Island Harbor; Talking Island, Eastern Territory | 1484 / 12 / 12 / 61 | yes | yes | 1482 | yes |
| `17_26` | Talking Island, Eastern Territory | 291 / 0 / 0 / 3 | yes | no | 291 | no |
| `18_25` | Talking Island, Eastern Territory | 291 / 0 / 0 / 3 | yes | no | 291 | no |
| `18_26` | Talking Island, Eastern Territory | 291 / 0 / 0 / 3 | yes | no | 291 | no |

- Official totals on these tiles: ambient **5332**, emitters **50**, music volumes **21**, lights/celestials **111**.
- Emitter actors: `16_25` × 38, `17_25` × 12.
- Ambient waves used here and **missing** from Fineage clips/banks (3 unique):
  - `fire_01` (`17_25` AmbientSoundObject12347 at -85084.0,245120.0,-3682.0)
  - `dt_electric_ball` (`17_25` AmbientSoundObject12340 at -90978.0,248083.0,-3546.0)
  - `flame_big_loop` (`17_25` AmbientSoundObject12171 at -71343.0,258390.0,-3079.0)
- Ambient JSON row count differs from the official actor count:
  - `16_24` official=922 json=920 (delta -2)
  - `16_25` official=1471 json=1465 (delta -6)
  - `17_25` official=1484 json=1482 (delta -2)

### Elven Village

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `21_19` | Shadow of the Mother Tree; Elven Village; Starlight Waterfall | 1715 / 69 / 9 / 28 | yes | yes | — | no |
| `21_18` | Sea of Spores; Shilen's Garden | 1191 / 220 / 5 / 5 | yes | no | — | no |
| `21_20` | Iris Lake | 1588 / 0 / 1 / 3 | yes | yes | — | no |
| `20_19` |  | 1647 / 0 / 0 / 3 | yes | yes | — | no |

- Official totals on these tiles: ambient **6141**, emitters **289**, music volumes **15**, lights/celestials **39**.
- Emitter actors: `21_19` × 69, `21_18` × 220.
- Ambient waves used here and **missing** from Fineage clips/banks (26 unique):
  - `lake_01` (`21_19` AmbientSoundObject1208 at 46064.0,49040.0,-3072.0)
  - `stream_small_05` (`21_19` AmbientSoundObject1214 at 45600.0,50592.0,-3072.0)
  - `stream_small_01` (`21_19` AmbientSoundObject1215 at 45152.0,50592.0,-3072.0)
  - `ev_bell_01` (`21_19` AmbientSoundObject1222 at 43728.0,48336.0,-2984.0)
  - `ev_bell_04` (`21_19` AmbientSoundObject1223 at 47856.0,50464.0,-2988.0)
  - `lake_03` (`21_19` AmbientSoundObject1233 at 43856.0,41024.0,-3504.0)
  - `ed_chimes_02` (`21_19` AmbientSoundObject1740 at 44840.0,41529.0,-3454.0)
  - `ed_chimes_04` (`21_19` AmbientSoundObject2740 at 45280.0,49376.0,-3049.0)
  - `wind_light_loop_2` (`21_19` AmbientSoundObject2769 at 46336.0,40464.0,-3376.0)
  - `axe_4` (`21_19` AmbientSoundObject2888 at 44112.0,47040.0,-2960.0)
  - `bow_small_2` (`21_19` AmbientSoundObject2889 at 44272.0,47232.0,-2976.0)
  - `spear_9` (`21_19` AmbientSoundObject2890 at 44480.0,47184.0,-2976.0)
  - `ed_chimes_05` (`21_19` AmbientSoundObject2919 at 44720.0,47202.0,-2942.0)
  - `ds_wind_01` (`21_18` AmbientSoundObject28331 at 53648.5586,18636.0254,-5366.0)
  - `cm_wind_01` (`21_18` AmbientSoundObject28309 at 50064.5586,23004.0254,-5286.0)
  - `an_beetle_01` (`21_18` AmbientSoundObject28279 at 47022.0039,20000.3652,-5024.0)
  - `an_drone_02` (`21_18` AmbientSoundObject28258 at 57950.0039,12912.3652,-4192.0)
  - `st_horror_01` (`21_18` AmbientSoundObject28032 at 55328.0,17472.0,-5520.0)
  - `st_drone_02` (`21_18` AmbientSoundObject28240 at 51934.0039,19456.3652,-5376.0)
  - `st_drone_01` (`21_18` AmbientSoundObject28220 at 50414.0039,16784.3652,-5472.0)
  - `ed_drone_02` (`21_18` AmbientSoundObject28210 at 52030.0039,16912.3652,-5488.0)
  - `ed_drone_01` (`21_18` AmbientSoundObject28209 at 56446.0039,18368.3652,-4960.0)
  - `sd_horror_01` (`21_18` AmbientSoundObject28171 at 50702.0039,14944.3652,-5344.0)
  - `ds_horn_01` (`21_18` AmbientSoundObject28130 at 56046.0039,27952.3652,-4704.0)
  - `an_bird_13` (`21_20` AmbientSoundObject8205 at 55747.293,76457.7266,-3535.35864)
  - … 1 more

### Dark Elf Village

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `20_18` | Dark Elf Village; The Shilen Temple; Shilen's Garden | 1205 / 28 / 6 / 137 | yes | yes | — | no |
| `20_17` | Primeval Plains; Lost Nest; Primeval Isle; Primeval Isle Wharf; Siren's Den | 739 / 151 / 4 / 13 | yes | yes | — | no |
| `19_17` |  | 0 / 0 / 1 / 3 | yes | yes | — | no |
| `19_18` | Catacomb of Dark Omens; The Dark Forest; Shilen's Garden | 2557 / 252 / 1 / 1758 | yes | yes | — | no |

- Official totals on these tiles: ambient **4501**, emitters **431**, music volumes **12**, lights/celestials **1911**.
- Emitter actors: `20_18` × 28, `20_17` × 151, `19_18` × 252.
- Ambient waves used here and **missing** from Fineage clips/banks (75 unique):
  - `d_wind_loop_11` (`20_18` AmbientSoundObject1146 at 26336.0,10912.0,-3695.0)
  - `d_wind_loop_10` (`20_18` AmbientSoundObject1145 at 27216.0,10880.0,-3951.0)
  - `dt_horror_02` (`20_18` AmbientSoundObject366 at 13907.0,16863.0,-4530.0)
  - `dt_horror_01` (`20_18` AmbientSoundObject365 at 10003.0,15149.0,-4284.0)
  - `dt_spark_01` (`20_18` AmbientSoundObject362 at 11387.0,16889.0,-4430.0)
  - `dt_spark_02` (`20_18` AmbientSoundObject360 at 11356.0,16879.0,-4585.0)
  - `dt_electric_ball` (`20_18` AmbientSoundObject359 at 11391.0,16889.0,-4043.0)
  - `dd_wind_01` (`20_18` AmbientSoundObject358 at 29595.0,11035.0,-4209.0)
  - `st_drone_01` (`20_18` AmbientSoundObject0 at 30095.0,10673.0,-4192.0)
  - `st_drone_02` (`20_18` AmbientSoundObject1 at 30127.0,11345.0,-4192.0)
  - `st_horror_01` (`20_18` AmbientSoundObject2 at 30303.0,11025.0,-4208.0)
  - `dd_wind_03` (`20_18` AmbientSoundObject5 at 28815.0,11329.0,-4208.0)
  - `d_water_drop_07` (`20_18` AmbientSoundObject70 at 11183.0,12321.0,-4398.0)
  - `d_water_drop_08` (`20_18` AmbientSoundObject71 at 9375.0,12465.0,-4510.0)
  - `d_water_drop_01` (`20_18` AmbientSoundObject72 at 6047.0,18657.0,-4110.0)
  - `d_water_drop_02` (`20_18` AmbientSoundObject73 at 6879.0,19761.0,-3710.0)
  - `d_water_drop_03` (`20_18` AmbientSoundObject74 at 16511.0,14753.0,-4030.0)
  - `d_water_drop_04` (`20_18` AmbientSoundObject75 at 16127.0,13729.0,-3710.0)
  - `d_horror_17` (`20_18` AmbientSoundObject76 at 10943.0,14529.0,-4216.0)
  - `d_horror_21` (`20_18` AmbientSoundObject77 at 10911.0,19089.0,-4524.0)
  - `dd_rattle_01` (`20_18` AmbientSoundObject78 at 9503.0,15457.0,-3744.0)
  - `dd_rattle_02` (`20_18` AmbientSoundObject79 at 12415.0,14769.0,-3792.0)
  - `dd_rattle_03` (`20_18` AmbientSoundObject80 at 13135.0,18641.0,-3792.0)
  - `dd_rattle_04` (`20_18` AmbientSoundObject81 at 10911.0,19153.0,-3792.0)
  - `d_drone_03` (`20_18` AmbientSoundObject85 at 13535.0,17329.0,-4552.0)
  - … 50 more

### Orc Village

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `18_14` | Orc Village; The Pa'agrio Temple; Valley of Heroes | 2175 / 7 / 9 / 113 | yes | yes | — | no |
| `18_13` |  | 0 / 0 / 0 / 3 | yes | no | — | no |
| `18_15` |  | 0 / 0 / 0 / 3 | yes | no | — | no |
| `19_14` | Immortal Plateau, Northern Region; Valley of Heroes | 3197 / 0 / 1 / 3 | yes | yes | — | no |
| `19_15` | Immortal Plateau, Northern Region; Immortal Plateau, Southern Region | 2723 / 0 / 0 / 3 | yes | yes | — | no |

- Official totals on these tiles: ambient **8095**, emitters **7**, music volumes **10**, lights/celestials **125**.
- Emitter actors: `18_14` × 7.
- Ambient waves used here and **missing** from Fineage clips/banks (44 unique):
  - `fire_01` (`18_14` AmbientSoundObject7833 at -49383.0,-113335.0,113.0)
  - `an_bird_06` (`18_14` AmbientSoundObject7900 at -50784.0,-105712.0,-2093.0)
  - `an_bird_13` (`18_14` AmbientSoundObject7907 at -35904.0,-115152.0,-1869.0)
  - `p_pipe_01` (`18_14` AmbientSoundObject8004 at -42409.4766,-111729.922,-1811.0)
  - `df_pipe_01` (`18_14` AmbientSoundObject8009 at -44796.6367,-117149.141,-83.0)
  - `dv_pipe_01` (`18_14` AmbientSoundObject8011 at -48975.3281,-110028.758,-163.0)
  - `df_percussion_01` (`18_14` AmbientSoundObject8012 at -49649.4805,-111810.164,-147.0)
  - `d_lava_01` (`18_14` AmbientSoundObject8022 at -56906.0,-113395.0,-677.0)
  - `d_lava_02` (`18_14` AmbientSoundObject8023 at -56895.0,-113838.0,-677.0)
  - `d_lava_04` (`18_14` AmbientSoundObject8024 at -57309.0,-113825.0,-677.0)
  - `d_lava_03` (`18_14` AmbientSoundObject8025 at -57309.0,-113399.0,-677.0)
  - `flame_big_loop` (`18_14` AmbientSoundObject8026 at -57080.0,-113619.0,-496.0)
  - `dt_wind_02` (`18_14` AmbientSoundObject8565 at -52560.0,-113776.0,-98.0)
  - `ms_wind_loop_01` (`18_14` AmbientSoundObject8673 at -53783.0,-109392.0,-922.0)
  - `ds_horn_01` (`18_14` AmbientSoundObject8777 at -35296.0,-120000.0,-1817.0)
  - `p_pipe_02` (`18_14` AmbientSoundObject8782 at -33792.0,-113616.0,-2089.0)
  - `an_bat_01` (`18_14` AmbientSoundObject8850 at -36048.0,-105184.0,-2227.0)
  - `sd_drone_02` (`18_14` AmbientSoundObject9638 at -54114.0,-113771.0,-354.0)
  - `cd_stonedoor_01` (`18_14` AmbientSoundObject9643 at -56552.0,-114500.0,-697.0)
  - `dd_wind_03` (`18_14` AmbientSoundObject9651 at -57466.0,-113629.0,-688.0)
  - `dd_wind_01` (`18_14` AmbientSoundObject9657 at -53173.0,-113693.0,-103.0)
  - `dt_horror_02` (`18_14` AmbientSoundObject9658 at -53599.0,-113515.0,-184.0)
  - `dt_horror_01` (`18_14` AmbientSoundObject9659 at -54296.0,-113511.0,-417.0)
  - `stone_rolling_loop` (`18_14` AmbientSoundObject9665 at -57088.8867,-113428.898,-663.0)
  - `dv_percussion_01` (`18_14` AmbientSoundObject9813 at -48074.0,-113831.0,-190.0)
  - … 19 more

### Dwarf Village

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `23_12` | Dwarven Village; Strip Mine; Frozen Valley; Western Mining Zone; Mining Zone Passage; Plunderous Plains | 1302 / 14 / 9 / 40 | yes | yes | — | no |
| `23_11` | Western Mining Zone | 611 / 0 / 1 / 3 | yes | yes | — | no |
| `23_13` | Plunderous Plains; Sky Wagon Relic; Railroad Station; Brigand Stronghold | 490 / 21 / 2 / 85 | yes | no | — | no |
| `24_11` | Western Mining Zone; Eastern Mining Zone | 787 / 0 / 0 / 3 | yes | yes | — | no |
| `24_12` | Abandoned Coal Mines; Schuttgart Territory; Western Mining Zone; Eastern Mining Zone | 1587 / 0 / 1 / 129 | yes | yes | — | no |

- Official totals on these tiles: ambient **4777**, emitters **35**, music volumes **13**, lights/celestials **260**.
- Emitter actors: `23_12` × 14, `23_13` × 21.
- Ambient waves used here and **missing** from Fineage clips/banks (43 unique):
  - `stream_big_02` (`23_12` AmbientSoundObject640 at 129812.25,-182508.906,-3518.0)
  - `an_wolf_05` (`23_12` AmbientSoundObject750 at 120126.0,-194332.0,-3169.0)
  - `dd_rattle_01` (`23_12` AmbientSoundObject1598 at 108125.0,-172743.0,-570.0)
  - `dd_rattle_03` (`23_12` AmbientSoundObject1602 at 108735.0,-175974.0,-420.0)
  - `dd_rattle_02` (`23_12` AmbientSoundObject1604 at 109422.0,-174794.0,-570.0)
  - `dd_rattle_04` (`23_12` AmbientSoundObject1607 at 106921.0,-176079.0,-420.0)
  - `an_dog_06` (`23_12` AmbientSoundObject1878 at 117560.0,-181286.0,-1193.0)
  - `lake_01` (`23_11` AmbientSoundObject5 at 114880.0,-202832.0,-3760.0)
  - `lake_03` (`23_11` AmbientSoundObject36 at 115378.0,-202192.0,-3776.0)
  - `df_pipe_01` (`23_11` AmbientSoundObject79 at 127264.0,-203888.0,-2377.0)
  - `gf_flute_01` (`23_11` AmbientSoundObject81 at 116256.0,-198176.0,-2869.0)
  - `gf_animal_crying_01` (`23_11` AmbientSoundObject82 at 115376.0,-205248.0,-2601.0)
  - `wl_rock_01` (`23_13` AmbientSoundObject214 at 128542.633,-160599.859,-1222.26086)
  - `wind_light_1` (`23_13` AmbientSoundObject117 at 109223.492,-161916.438,-1602.09375)
  - `wind_light_4` (`23_13` AmbientSoundObject120 at 106251.578,-157391.297,-2107.90625)
  - `wind_light_2` (`23_13` AmbientSoundObject126 at 118023.547,-159093.922,-1346.21875)
  - `wind_light_3` (`23_13` AmbientSoundObject106 at 112138.508,-147963.609,-3271.09375)
  - `stream_small_01` (`23_13` AmbientSoundObject227 at 128190.258,-147622.25,-3764.0)
  - `an_rock_01` (`23_13` AmbientSoundObject221 at 119653.617,-149394.031,-3275.74146)
  - `d_wind_loop_10` (`24_12` AmbientSoundObject3039 at 147764.0,-171652.0,2237.0)
  - `d_wind_loop_07` (`24_12` AmbientSoundObject3034 at 150160.0,-168929.0,1994.0)
  - `d_wind_loop_18` (`24_12` AmbientSoundObject3023 at 147125.0,-169937.0,2200.0)
  - `d_wind_loop_11` (`24_12` AmbientSoundObject2993 at 139201.0,-169202.0,-1578.0)
  - `d_wind_loop_16` (`24_12` AmbientSoundObject2941 at 152657.0,-175908.0,798.0)
  - `d_water_drop_04` (`24_12` AmbientSoundObject3993 at 139525.0,-170148.0,279.0)
  - … 18 more

### Gludin Village

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `17_22` | Giran Arena; Gludin Village; Gludin Harbor; Fellmere Lake; Windmill Hill | 1488 / 0 / 16 / 67 | yes | yes | — | no |
| `17_21` | Orc Barracks; Oracle of Dawn; Gludio Territory; Fellmere Harvesting Grounds | 2123 / 177 / 5 / 812 | yes | yes | — | no |

- Official totals on these tiles: ambient **3611**, emitters **177**, music volumes **21**, lights/celestials **879**.
- Emitter actors: `17_21` × 177.
- Ambient waves used here and **missing** from Fineage clips/banks (57 unique):
  - `an_wolf_05` (`17_22` AmbientSoundObject20066 at -76792.0,147618.0,-1253.0)
  - `d_horror_15` (`17_22` AmbientSoundObject20888 at -79616.0,151024.0,-2957.0)
  - `d_horror_05` (`17_22` AmbientSoundObject20887 at -79552.0,150672.0,-2989.0)
  - `fire_01` (`17_22` AmbientSoundObject20867 at -79216.0,150656.0,-2992.0)
  - `bow_small_5` (`17_22` AmbientSoundObject20861 at -80224.0,155328.0,-3120.0)
  - `sword_great_7` (`17_22` AmbientSoundObject20860 at -80496.0,155136.0,-3120.0)
  - `hammer_2` (`17_22` AmbientSoundObject20859 at -80848.0,155680.0,-3120.0)
  - `staff_1` (`17_22` AmbientSoundObject20857 at -80208.0,155024.0,-3120.0)
  - `r_rattle_03` (`17_22` AmbientSoundObject19960 at -79526.0,155466.0,-3040.0)
  - `r_rattle_01` (`17_22` AmbientSoundObject19959 at -77696.0,155522.0,-3058.0)
  - `an_bird_13` (`17_22` AmbientSoundObject19954 at -94257.0,147200.0,-3394.0)
  - `an_bird_06` (`17_22` AmbientSoundObject19952 at -88689.0,132896.0,-2690.0)
  - `an_dog_07` (`17_22` AmbientSoundObject19894 at -81808.0,152368.0,-2960.0)
  - `an_dog_06` (`17_22` AmbientSoundObject19893 at -84162.0,152369.0,-2912.0)
  - `fv_frog_01` (`17_22` AmbientSoundObject19888 at -67808.0,133136.0,-3136.0)
  - `fv_frog_02` (`17_22` AmbientSoundObject19887 at -67024.0,136752.0,-3152.0)
  - `lake_01` (`17_22` AmbientSoundObject19555 at -68013.0,131117.0,-3163.0)
  - `lake_03` (`17_22` AmbientSoundObject19554 at -67495.0,131669.0,-3163.0)
  - `under_water_2` (`17_21` AmbientSoundObject846 at -83839.1953,111495.789,-4838.81152)
  - `under_water_1` (`17_21` AmbientSoundObject847 at -83572.2188,111487.156,-4947.1792)
  - `d_wind_loop_18` (`17_21` AmbientSoundObject300 at -76215.1172,111957.117,-4862.0)
  - `d_horror_14` (`17_21` AmbientSoundObject302 at -75214.9063,111577.633,-4927.68652)
  - `d_horror_13` (`17_21` AmbientSoundObject303 at -75593.4766,111588.063,-4927.56055)
  - `dd_drone_01` (`17_21` AmbientSoundObject323 at -78604.7344,113199.445,-4896.0)
  - `dd_wind_01` (`17_21` AmbientSoundObject324 at -78602.6328,112630.883,-4896.0)
  - … 32 more

### Town of Gludio

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `19_21` | Gludio Castle; Town of Gludio; Maille Lizardmen Barracks; Evil Hunting Grounds | 1852 / 20 / 12 / 129 | yes | yes | — | no |
| `19_19` | Swampland; Undine Waterfall; Black Rock Hill | 2096 / 5 / 2 / 4 | yes | no | — | no |

- Official totals on these tiles: ambient **3948**, emitters **25**, music volumes **14**, lights/celestials **133**.
- Emitter actors: `19_21` × 20, `19_19` × 5.
- Ambient waves used here and **missing** from Fineage clips/banks (37 unique):
  - `stream_small_01` (`19_21` AmbientSoundObject202 at -21824.0,118992.0,-3760.0)
  - `fire_01` (`19_21` AmbientSoundObject569 at -17854.0,109820.0,-2422.0)
  - `gf_horn_01` (`19_21` AmbientSoundObject553 at -31584.0,119616.0,-2832.0)
  - `gf_flute_01` (`19_21` AmbientSoundObject549 at -31648.0,121840.0,-3040.0)
  - `an_dog_07` (`19_21` AmbientSoundObject542 at -14048.0,125568.0,-2768.0)
  - `d_wind_loop_18` (`19_21` AmbientSoundObject644 at -18112.0,110168.0,-2365.0)
  - `an_bird_13` (`19_21` AmbientSoundObject1571 at -23215.7227,125558.617,-3579.81152)
  - `an_bird_06` (`19_21` AmbientSoundObject1576 at -26228.1309,116297.039,-3555.021)
  - `d_horror_15` (`19_21` AmbientSoundObject1663 at -13552.0,121712.0,-2925.0)
  - `axe_2` (`19_21` AmbientSoundObject1679 at -15409.0,123260.0,-3108.0)
  - `sword_great_7` (`19_21` AmbientSoundObject1680 at -14882.0,123349.0,-3106.0)
  - `bow_small_5` (`19_21` AmbientSoundObject1682 at -15068.0,123595.0,-3093.0)
  - `d_horror_05` (`19_21` AmbientSoundObject1660 at -12704.0,121712.0,-2925.0)
  - `d_wind_loop_10` (`19_21` AmbientSoundObject1667 at -18384.0,110336.0,-2512.0)
  - `ds_bubble_01` (`19_19` AmbientSoundObject14088 at -13550.6777,41196.2773,-3721.0)
  - `cm_mud_03` (`19_19` AmbientSoundObject14089 at -13198.6777,38828.2773,-3721.0)
  - `cm_mud_05` (`19_19` AmbientSoundObject14091 at -26958.6777,49628.2773,-3737.0)
  - `cm_mud_01` (`19_19` AmbientSoundObject14093 at -17134.6777,47564.2773,-3737.0)
  - `cm_mud_02` (`19_19` AmbientSoundObject14094 at -15310.6777,48044.2773,-3737.0)
  - `cm_mud_04` (`19_19` AmbientSoundObject14116 at -11662.6777,47996.2773,-3753.0)
  - `dt_pipe_01` (`19_19` AmbientSoundObject13662 at -18624.0,43376.0,-3554.0)
  - `gf_animal_crying_01` (`19_19` AmbientSoundObject13680 at -23792.0,45168.0,-3506.0)
  - `cm_grunt_01` (`19_19` AmbientSoundObject13689 at -24992.0,49600.0,-3654.0)
  - `cm_pipe_01` (`19_19` AmbientSoundObject13693 at -10160.0,44480.0,-3622.0)
  - `ds_wind_01` (`19_19` AmbientSoundObject13782 at -26280.4141,48186.5117,-3739.0)
  - … 12 more

### Town of Dion

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `20_22` | Dion Castle; Town of Dion; Floran Agricultural Area; Dion Hills | 2113 / 20 / 10 / 177 | yes | yes | — | no |

- Official totals on these tiles: ambient **2113**, emitters **20**, music volumes **10**, lights/celestials **177**.
- Emitter actors: `20_22` × 20.
- Ambient waves used here and **missing** from Fineage clips/banks (17 unique):
  - `d_wind_loop_20` (`20_22` AmbientSoundObject466 at 16416.0,142112.0,-2704.0)
  - `d_horror_15` (`20_22` AmbientSoundObject470 at 15712.0,142352.0,-2650.0)
  - `d_horror_05` (`20_22` AmbientSoundObject471 at 16432.0,142128.0,-2666.0)
  - `sword_great_7` (`20_22` AmbientSoundObject477 at 17824.0,144672.0,-3023.0)
  - `bow_small_6` (`20_22` AmbientSoundObject478 at 18389.0,144442.0,-3023.0)
  - `hammer_2` (`20_22` AmbientSoundObject479 at 18101.0,144879.0,-3023.0)
  - `sword_great_5` (`20_22` AmbientSoundObject480 at 18307.0,144585.0,-3023.0)
  - `fire_01` (`20_22` AmbientSoundObject454 at 15957.0,142161.0,-2639.0)
  - `an_dog_06` (`20_22` AmbientSoundObject423 at 19042.0,146420.0,-3104.0)
  - `an_dog_07` (`20_22` AmbientSoundObject421 at 16505.0,146264.0,-3127.0)
  - `sv_cow_01` (`20_22` AmbientSoundObject417 at 16210.0,145807.0,-2835.0)
  - `fv_frog_01` (`20_22` AmbientSoundObject412 at 18001.0,143686.0,-2788.0)
  - `ad_frog_02` (`20_22` AmbientSoundObject404 at 20464.0,143024.0,-3087.0)
  - `fv_frog_02` (`20_22` AmbientSoundObject403 at 20352.0,142976.0,-3087.0)
  - `an_bird_06` (`20_22` AmbientSoundObject370 at 1936.0,137184.0,-3122.0)
  - `d_wind_loop_16` (`20_22` AmbientSoundObject2087 at 22496.0,160096.0,-2586.0)
  - `flag_02` (`20_22` AmbientSoundObject2112 at 5479.99365,151493.047,-2918.14941)

### Giran Castle Town

| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |
|---|---|---|---|---|---:|---|
| `22_22` | Giran Arena; Giran Castle Town; Tanor Canyon; Breka's Stronghold | 2301 / 0 / 16 / 94 | yes | yes | — | no |
| `22_21` | Death Pass; Breka's Stronghold | 1610 / 0 / 2 / 3 | yes | yes | — | no |
| `22_23` | Field of Silence | 2490 / 0 / 2 / 39 | yes | yes | — | no |
| `21_21` | Partisan's Hideaway; The Pilgrim's Necropolis; Death Pass; Fortress of Resistance | 3026 / 269 / 3 / 1752 | yes | yes | — | no |
| `21_22` | Execution Grounds; Tanor Canyon; Catacomb of the Heretic; Dion Hills | 2538 / 0 / 4 / 136 | yes | yes | — | no |
| `21_23` | Giran Harbor; Catacomb of the Branded; Tanor Canyon; Bee Hive; Giran Territory | 4271 / 300 / 4 / 2381 | yes | yes | — | no |

- Official totals on these tiles: ambient **16236**, emitters **569**, music volumes **31**, lights/celestials **4405**.
- Emitter actors: `21_21` × 269, `21_23` × 300.
- Ambient waves used here and **missing** from Fineage clips/banks (110 unique):
  - `an_wolf_05` (`22_22` AmbientSoundObject4512 at 79008.0,132448.0,-2646.0)
  - `fire_01` (`22_22` AmbientSoundObject2841 at 84777.0,146376.0,-3386.0)
  - `ms_drone_01` (`22_22` AmbientSoundObject2821 at 85826.4844,153151.25,-3463.0)
  - `lake_01` (`22_22` AmbientSoundObject2450 at 83838.0,155371.0,-3746.0)
  - `lake_03` (`22_22` AmbientSoundObject2413 at 89824.0,148416.0,-3746.0)
  - `sword_mid_8` (`22_22` AmbientSoundObject4066 at 79344.0,150704.0,-3488.0)
  - `bow_small_5` (`22_22` AmbientSoundObject4065 at 79344.0,150423.0,-3488.0)
  - `staff_1` (`22_22` AmbientSoundObject4061 at 79392.0,146496.0,-3488.0)
  - `sword_great_6` (`22_22` AmbientSoundObject4060 at 80134.0,146496.0,-3488.0)
  - `d_horror_15` (`22_22` AmbientSoundObject4056 at 85120.0,148352.0,-3364.0)
  - `d_horror_05` (`22_22` AmbientSoundObject4055 at 85088.0,148864.0,-3364.0)
  - `d_wind_loop_07` (`22_22` AmbientSoundObject4050 at 85824.0,148624.0,-3347.0)
  - `ed_drone_02` (`22_22` AmbientSoundObject4036 at 85392.0,153136.0,-3463.0)
  - `ed_drone_01` (`22_22` AmbientSoundObject4035 at 86256.0,153136.0,-3463.0)
  - `dv_percussion_01` (`22_21` AmbientSoundObject18909 at 74885.0,117695.0,-3617.0)
  - `ds_insect_01` (`22_21` AmbientSoundObject20504 at 80088.5781,113718.758,-3015.0)
  - `gf_animal_crying_01` (`22_21` AmbientSoundObject20486 at 91806.0,110346.0,-3656.0)
  - `df_pipe_01` (`22_21` AmbientSoundObject20483 at 89416.0,107994.0,-2910.0)
  - `dv_pipe_02` (`22_21` AmbientSoundObject20479 at 91523.0,114427.0,-2936.0)
  - `dv_pipe_01` (`22_21` AmbientSoundObject20478 at 91157.0,112442.0,-2782.0)
  - `ds_bird_02` (`22_21` AmbientSoundObject20233 at 73718.0,100115.0,-3174.0)
  - `ds_bird_01` (`22_21` AmbientSoundObject20232 at 75304.0,100715.0,-3181.0)
  - `wl_rock_01` (`22_21` AmbientSoundObject20175 at 95114.0,112990.0,-2973.0)
  - `dt_rock_01` (`22_21` AmbientSoundObject20141 at 85428.0,117246.0,-2985.0)
  - `p_pipe_02` (`22_21` AmbientSoundObject20482 at 83085.0,111117.0,-3543.0)
  - … 85 more

## Files

- Lossless dumps: `world/maps/*.unr.json.gz`, `world/env/`, `world/anims/*.ukx.json.gz`, `sounds/packages/*.uax.json`.
- `world/package-dump-index.json` — package list, export counts, stored sizes (0 extract failures).
- `world/map-actors.tsv` — every interesting placed actor and its authored properties.
- `world/map-class-counts.tsv`, `world/map-summary.tsv`.
- `sounds/sound-catalog.tsv` — every original wave + Fineage presence.
- `sounds/sound-references.tsv` — who references each sound.
- `sounds/anim-notifies.tsv`, `sounds/anim-sequences.tsv`, `sounds/anim-sequence-notifies.tsv.gz`, `sounds/musicinfo-tracks.tsv`.
- Tools: `tools/interlude-extract/ExportClientPackages.java`, `export_client_packages.sh`, `build_world_sound_gap.py`.

Original binaries are not committed. Dumps omit wave/texture payloads (`wave_bytes` is a length only).

