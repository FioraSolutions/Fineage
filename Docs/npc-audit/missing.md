# Missing NPC / monster models

**Batch 1 imported (this PR):** 43 unique `LineageMonsters*` meshes used in start areas (Talking Island, Dark Elf, Elf, Orc, Dwarf) and Gludin hunting grounds, including all local bears (`grizzly_m00` — Red/Brown/Grizzly/Kasha/Hunter Bear).

Meshes were exported from the official Interlude `LineageMonsters.ukx` / `LineageMonsters2.ukx` and `lineagemonsterstex*.utx` with umodel, converted PSK+PSA → FBX with the existing `psk_psa_to_fbx.py` pipeline (wolf.fbx scale reference), then wired like keltir via `L2MonsterImportBatch` (clips, `Monster_Basic` override, click_area, capsule). No per-mob gameplay branches.

Npcgrp package names for these spawn ids already match official Interlude UKX (no mapping fixes in this batch). Village/town NPC bodies and Gludio/Dion-only meshes are still outstanding — see later sections.

---

Audit of backend spawn `npcId`s against `Npcgrp_Classic.txt` and Unity Resources meshes under `Assets/Resources/Data/Animations/{package}/{asset}/{asset}.prefab`.

Load path is `ModelTable.GetOrLoadNpc(package.asset)` → `Data/Animations/{package}/{asset}`.
Monster package typo is fatal: FallbackCreate only retries `LineageNPC*` packages, not monsters.

Regions: start areas (Talking Island, Dark Elf, Elf, Orc, Dwarf), then Gludin (incl. bears), Gludio, Dion.

## Currently imported meshes

- `animator`: 5 folders (_template, avatar, monster, npc, player)
- `darkelf`: 2 folders (fdarkelf, mdarkelf)
- `dwarf`: 2 folders (fdwarf, mdwarf)
- `elf`: 2 folders (felf, melf)
- `fighter`: 2 folders (ffighter, mfighter)
- `human`: 4 folders (ffighter, fmagic, mfighter, mmagic)
- `lineageeffect`: 1 folders (lineageeffectsstaticmeshes)
- `lineagemonsters`: 21 folders (aracne_m00, cat_the_cat_m00, elpy_m00, fox_m00, giant_spider_m00, goblin_m00, gremlin_m00, hatchling_m00…)
- `lineagemonsters2`: 4 folders (baby_buffalo_m00, baby_ostrich_m00, baby_tiger_m00, queen_of_cat_m00)
- `lineagenpcs`: 42 folders (a_casino_fdarkelf_m00, a_casino_mhuman_m00, a_chamberlain_mhuman_m00, a_clergyman_fdarkelf_m00, a_clergyman_mdarkelf_m00, a_common_people_fdarkelf_m00, a_common_people_felf_m00, a_common_people_melf_m00…)
- `lineagenpcs4`: 8 folders (a_chamberlain_mhuman_m00, a_common_people_mdarkelf_m00, a_fighterguild_teacher_fdarkelf_m00, a_fighterguild_teacher_mhuman_m00, a_mageguild_teacher_fdarkelf_m00, a_teleporter_fhuman_m00, a_traderb_fhuman_m00, a_traderb_mhuman_m00)
- `lineageweapons`: 1 folders (models)
- `magic`: 2 folders (fmagic, mmagic)
- `orc`: 5 folders (forc, fshaman, models, morc, mshaman)
- `shaman`: 3 folders (fshaman, models, mshaman)

## start_talking_island

Spawn files: `ti_cedric_training_hall.json`, `ti_village.json`, `ti_village_road.json`, `ti_monsters.json`, `ti_quest_monsters.json`, `training_hall_gremlins.json`, `others_17_25.json`

Unique npcIds: **89**. Present: **70**. Missing/wrong: **19**. Spawn rows: 634.

### Missing / wrong (13 unique meshes, 19 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageNPCs.a_sanctuary_teacher_MHuman_m00` | missing_textures | 30032 Yohanes ×1, 30036 Petron ×1, 30598 Newbie Guide ×1 | LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_h |  |
| `LineageNPCs4.a_fighterguild_teacher_MHuman_m00` | missing_textures | 30009 Newbie Helper ×1, 30027 Gwinter ×1, 30028 Pintage ×1 | LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t01 |  |
| `LineageNPCs4.a_teleporter_FHuman_m00` | missing_textures | 30006 Roxxy ×1, 30427 Siff ×1, 30429 Tiramisa ×1 | LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t00, LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t01 |  |
| `LineageNPCs.a_mageguild_master_MHuman_m00` | missing_textures | 30017 Gallint ×1 | LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_f, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_h |  |
| `LineageNPCs.a_sanctuary_master_MHuman_m00` | missing_textures | 30031 Biotin ×1 | LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b00, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b01, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_f, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_h |  |
| `LineageNPCs.a_sanctuary_teacher_FHuman_m00` | missing | 31776 Iris ×1 | LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_h_ori |  |
| `LineageNPCs.a_traderA_MHuman_m00` | missing_textures | 30001 Lector ×1 | LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_f, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_h |  |
| `LineageNPCs.heroes_obelisk_human_m00` | missing | 31690 Monument of Heroes ×1 | LineageNPCsTex.heroes_obelisk_human_t00, LineageNPCsTex.heroes_obelisk_human_t01 |  |
| `LineageNPCs4.a_chamberlain_MHuman_m00` | missing_textures | 35103 Manor Manager ×1 | LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t00, LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FHuman_m00` | other_package | 30029 Minia ×1 | LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t01 | lineagenpcs |
| `LineageNPCs4.a_traderB_FHuman_m00` | missing_textures | 30004 Katerina ×1 | LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t00, LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t01 |  |
| `LineageNPCs4.a_wharfinger_MHuman_m00` | other_package | 30047 Firon ×1 | LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t00, LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t01 | lineagenpcs |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |

## start_dark_elf

Spawn files: `dark_elf_starting.json`, `dark_elven_village.json`

Unique npcIds: **80**. Present: **47**. Missing/wrong: **33**. Spawn rows: 510.

### Missing / wrong (20 unique meshes, 33 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.walking_fungus_m00` | missing | 20007 Green Fungus ×40, 20400 Blood Fungus ×27 | LineageMonstersTex.walking_fungus_t00 |  |
| `LineageMonsters.imp_m00` | missing | 20004 Imp ×19, 20005 Imp Elder ×18 | LineageMonstersTex.imp_t00, LineageMonstersTex.imp_t01 |  |
| `LineageMonsters.pixy_m00` | missing | 31845 Bloody Pixy ×21 | LineageMonstersTex.pixy_t00, LineageMonstersTex.pixy_t01 |  |
| `LineageMonsters.ti_mi_riran_m00` | missing | 31850 Blight Treant ×17 | LineageMonstersTex.ti_mi_riran_t00, LineageMonstersTex.ti_mi_riran_t01, LineageMonstersTex.ti_mi_riran_t02 |  |
| `LineageNPCs.a_common_people_FDarkElf_m00` | missing_textures | 30352 Karina ×1, 30354 Jewel ×1, 30418 Annika ×1, 30652 Uruha ×1, 30668 Katari ×1 | LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_b00, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_b01, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_f, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_h |  |
| `LineageNPCs4.a_common_people_MDarkelf_m00` | missing_textures | 30307 Karrod ×1, 30351 Astaron ×1, 30353 Jughead ×1, 30415 Charkeren ×1, 35231 Manor Manager ×1 | LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t00, LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t01 |  |
| `LineageNPCs.a_trader_FDarkelf_m00` | missing_textures | 30135 Iria ×1, 30138 Minaless ×1, 30420 Tyra ×1 | LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_b00, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_b01, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_f, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_h |  |
| `LineageNPCs.a_guard_MElf_m00` | missing | 30663 Slein Shining Blade ×1, 30664 Cain Flying Knife ×1 |  |  |
| `LineageMonsters.angel_m00` | missing | 30980 Ark Guardian's Corpse ×1 | LineageMonstersTex.angel_t00, LineageMonstersTex.angel_t01 |  |
| `LineageNPCs.a_fighterguild_teacher_FElf_m00` | missing | 30662 Piper Longbow ×1 |  |  |
| `LineageNPCs.coffer_a_m00` | missing | 30977 Holy Ark of Secrecy 1 ×1 | LineageNPCsTex.coffer_a_t00 |  |
| `LineageNPCs4.a_fighterguild_teacher_FDarkElf_m00` | missing_textures | 30143 Trudy ×1 | LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t01 |  |
| `LineageNPCs4.a_mageguild_teacher_FDarkElf_m00` | missing_textures | 30144 Harne ×1 | LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `NO_GRP:31360` | no_npcgrp | 31360 ? ×1 |  |  |
| `NO_GRP:31361` | no_npcgrp | 31361 ? ×1 |  |  |
| `NO_GRP:31415` | no_npcgrp | 31415 ? ×1 |  |  |
| `NO_GRP:31564` | no_npcgrp | 31564 ? ×1 |  |  |
| `NO_GRP:31770` | no_npcgrp | 31770 ? ×1 |  |  |
| `NO_GRP:31779` | no_npcgrp | 31779 ? ×1 |  |  |

## start_elf

Spawn files: `elven_starting.json`, `elven_village.json`, `elven_village_quest_monsters.json`, `elven_fortress.json`

Unique npcIds: **88**. Present: **32**. Missing/wrong: **56**. Spawn rows: 1085.

### Missing / wrong (27 unique meshes, 56 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.dryad_m00` | missing | 20013 Dryad ×48, 20019 Dryad Elder ×48, 20336 Green Dryad ×30 | LineageMonstersTex.dryad_t00, LineageMonstersTex.dryad_t01 |  |
| `LineageMonsters.walking_fungus_m00` | missing | 20509 Spore Fungus ×81 | LineageMonstersTex.walking_fungus_t00 |  |
| `LineageMonsters.sylph_m00` | missing | 20036 Lirein ×40, 20044 Lirein Elder ×28 | LineageMonstersTex.sylph_t00, LineageMonstersTex.sylph_t01 |  |
| `LineageMonsters.dark_horror_m00` | missing | 20033 Shade Horror ×18, 20345 Dark Terror ×17, 20371 Mist Terror ×10 | LineageMonstersTex.dark_horror_t00 |  |
| `LineageMonsters.skeleton_m00` | missing | 20464 Dungeon Skeleton Soldier ×14, 20504 Dread Soldier ×16 | LineageMonstersTex.skeleton_t00, LineageMonstersTex.skeleton_t01 |  |
| `LineageMonsters.wererat_m00` | missing | 20040 Sukar Ratman ×15, 20047 Sukar Ratman Captain ×11, 27020 Varool Foulclaw ×1 | LineageMonstersTex.wererat_t00, LineageMonstersTex.wererat_t01 |  |
| `LineageNPCs.a_guard_MElf_m00` | missing | 30216 Wheeler ×1, 30217 Berros ×1, 30218 Kendell ×1, 30219 Veltress ×1, 30220 Starden ×1, 30221 Rayen ×1, 30284 Alberius ×1, 30285 Gartrandell ×1, 30400 Newbie Helper ×1, 30430 Trionell ×1, 30431 Eriel ×1, 30599 Newbie Guide ×1, 31033 Sentinel ×15 |  |  |
| `LineageMonsters.dre_vanul_m00` | missing | 20351 Dre Vanul Tracker ×18, 20353 Dre Vanul Beholder ×5, 27021 Kirunak ×1 | LineageMonstersTex.dre_vanul_t00, LineageMonstersTex.dre_vanul_t01 |  |
| `LineageMonsters.ti_mi_tran_m00` | missing | 31851 Treant ×11, 31853 Treant Bremec ×1 | LineageMonstersTex.ti_mi_tran_t00, LineageMonstersTex.ti_mi_tran_t01, LineageMonstersTex.ti_mi_tran_t02 |  |
| `LineageMonsters.pixy_m00` | missing | 31848 Fairy ×9, 31852 Pixy Murika ×1 | LineageMonstersTex.pixy_t00, LineageMonstersTex.pixy_t01 |  |
| `LineageMonsters.skeleton_archer_m00` | missing | 20463 Dungeon Skeleton Archer ×7 | LineageMonstersTex.skeleton_archer_t00, LineageMonstersTex.skeleton_archer_t01, LineageMonstersTex.skeleton_archer_t02 |  |
| `LineageNPCs.a_trader_MElf_m00` | missing | 30147 Unoren ×1, 30149 Creamees ×1, 30150 Herbiel ×1, 30151 Chad ×1, 30991 Lottery Ticket Seller ×1, 35230 Manor Manager ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_FElf_m00` | missing | 30155 Ellenia ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_MElf_m00` | missing | 30156 Cobendell ×1 |  |  |
| `LineageNPCs.a_mageguild_teacher_MElf_m00` | missing | 30158 Esrandell ×1 |  |  |
| `LineageNPCs.a_traderC_Mhuman_m00` | missing | 31046 Marsden ×1 | LineageNPCsTex.a_traderC_Mhuman_m00_t00_b00, LineageNPCsTex.a_traderC_Mhuman_m00_t00_b01, LineageNPCsTex.a_traderC_Mhuman_m00_t00_f, LineageNPCsTex.a_traderC_Mhuman_m00_t00_h |  |
| `LineageNPCs4.a_mageguild_teacher_FElf_m00` | missing | 30157 Greenis ×1 | LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t01 |  |
| `LineageNPCs4.a_patriarch_MElf_m00` | missing | 30154 Asterios ×1 | LineageNpcsTex3.a_patriarch_Melf_re.a_patriarch_Melf_re_t00, LineageNpcsTex3.a_patriarch_Melf_re.a_patriarch_Melf_re_t01 |  |
| `LineageNPCs4.a_trader_FElf_m00` | missing | 30148 Ariel ×1 | LineageNpcsTex3.a_trader_FElf_re.a_trader_FElf_re_t00, LineageNpcsTex3.a_trader_FElf_re.a_trader_FElf_re_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `NO_GRP:31358` | no_npcgrp | 31358 ? ×1 |  |  |
| `NO_GRP:31359` | no_npcgrp | 31359 ? ×1 |  |  |
| `NO_GRP:31414` | no_npcgrp | 31414 ? ×1 |  |  |
| `NO_GRP:31563` | no_npcgrp | 31563 ? ×1 |  |  |
| `NO_GRP:31769` | no_npcgrp | 31769 ? ×1 |  |  |
| `NO_GRP:31780` | no_npcgrp | 31780 ? ×1 |  |  |
| `NO_GRP:31781` | no_npcgrp | 31781 ? ×1 |  |  |

## start_orc

Spawn files: `orc_starting.json`, `orc_village.json`, `orc_village_quest_monsters.json`

Unique npcIds: **80**. Present: **19**. Missing/wrong: **61**. Spawn rows: 1343.

### Bears in this region

| npcId | name | status | mesh | spawns | files |
|------:|------|--------|------|-------:|-------|
| 20143 | Grizzly Bear | missing | `LineageMonsters.grizzly_m00` | 40 | orc_starting.json |
| 20479 | Kasha Bear | missing | `LineageMonsters.grizzly_m00` | 74 | orc_starting.json |

### Missing / wrong (28 unique meshes, 61 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.imp_m00` | missing | 20311 Rakeclaw Imp ×25, 20312 Rakeclaw Imp Hunter ×37, 20313 Rakeclaw Imp Leader ×8, 21117 Kasha Imp ×52, 21120 Kasha Imp Turen ×40 | LineageMonstersTex.imp_t00, LineageMonstersTex.imp_t01 |  |
| `LineageMonsters.monster_eye_m00` | missing | 20426 Eye of Seer ×50, 20428 Evil Eye Patrol ×52, 21257 Evil Eye Seer ×47 | LineageMonstersTex.monster_eye_t00 |  |
| `LineageMonsters.grizzly_m00` | missing | 20143 Grizzly Bear ×40, 20479 Kasha Bear ×74, 30592 Hestui Totem Spirit ×1 | LineageMonstersTex.grizzly_t00 |  |
| `LineageMonsters.dre_vanul_m00` | missing | 21118 Baar Dre Vanul ×47, 21123 Baar Dre Vanul Destroyer ×50 | LineageMonstersTex.dre_vanul_t00, LineageMonstersTex.dre_vanul_t01 |  |
| `LineageMonsters.dire_wolf_m00` | missing | 21121 Kasha Dire Wolf ×87 |  |  |
| `LineageMonsters.bugbear_m00` | missing | 21122 Kasha Bugbear ×52 | LineageMonstersTex.bugbear_t00, LineageMonstersTex.bugbear_t01 |  |
| `LineageMonsters.hobgoblin_m00` | missing | 21119 Kasha Hobgoblin ×47 | LineageMonstersTex.hobgoblin_t00, LineageMonstersTex.hobgoblin_t01 |  |
| `LineageMonsters.walking_fungus_m00` | missing | 20365 Mountain Fungus ×47 | LineageMonstersTex.walking_fungus_t00 |  |
| `LineageMonsters.vampire_bat_m00` | missing | 20316 Blackwing Bat ×45 | LineageMonstersTex.vampire_bat_t00, LineageMonstersTex.vampire_bat_t01 |  |
| `LineageNPCs.e_guard_MOrc_m00` | missing | 30577 Rukain ×1, 30578 Nakusin ×1, 30579 Tamai ×1, 30580 Parugon ×1, 30581 Orinak ×1, 30582 Tiku ×1, 30583 Petukai ×1, 30584 Vapook ×1, 30602 Newbie Guide ×1, 30672 Dubabah ×1, 31036 Centurion ×24 | LineageNPCsTex.e_guard_MOrc_m00_t00_b00, LineageNPCsTex.e_guard_MOrc_m00_t00_b01, LineageNPCsTex.e_guard_MOrc_m00_t00_f, LineageNPCsTex.e_guard_MOrc_m00_t00_h |  |
| `LineageNPCs.e_trader_MOrc_m00` | missing | 30558 Jakal ×1, 30560 Uska ×1, 30561 Papuma ×1, 30562 Grookin ×1, 30563 Imantu ×1, 30564 Sumari ×1, 30675 Kamuru's Corpse ×1, 30994 Lottery Ticket Seller ×1 | LineageNPCsTex.e_trader_MOrc_m00_t00_b00, LineageNPCsTex.e_trader_MOrc_m00_t00_b01, LineageNPCsTex.e_trader_MOrc_m00_t00_f, LineageNPCsTex.e_trader_MOrc_m00_t00_h |  |
| `LineageNPCs.e_fighterguild_teacher_MOrc_m00` | missing | 30566 Varkees ×1, 30567 Tantus ×1, 30568 Hatos ×1, 30569 Brukurse ×1, 30590 Rosheek ×1 | LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b00, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b01, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_f, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_h |  |
| `LineageNPCs4.e_fighterguild_teacher_FOrc_m00` | missing | 30559 Kunai ×1, 30570 Karukia ×1, 30669 Kakan ×1 | LineageNpcsTex3.e_fighterguild_teacher_Forc_re.e_fighterguild_teacher_Forc_re_t00, LineageNpcsTex3.e_fighterguild_teacher_Forc_re.e_fighterguild_teacher_Forc_re_t01 |  |
| `LineageNPCs.e_common_people_MOrc_m00` | missing | 30585 Tataru Zu Hestui ×1, 30587 Gantaki Zu Urutu ×1 | LineageNPCsTex.e_common_people_MOrc_m00_t00_b00, LineageNPCsTex.e_common_people_MOrc_m00_t00_b01, LineageNPCsTex.e_common_people_MOrc_m00_t00_f, LineageNPCsTex.e_common_people_MOrc_m00_t00_h |  |
| `LineageNPCs.e_patriarch_MOrc_m00` | missing | 30565 Kakai ×1 | LineageNPCsTex.e_patriarch_MOrc_m00_t00_b00, LineageNPCsTex.e_patriarch_MOrc_m00_t00_b01, LineageNPCsTex.e_patriarch_MOrc_m00_t00_f, LineageNPCsTex.e_patriarch_MOrc_m00_t00_h |  |
| `LineageNPCs.e_teleporter_FOrc_m00` | missing | 30576 Tamil ×1 | LineageNPCsTex.e_teleporter_FOrc_m00_t00_b00, LineageNPCsTex.e_teleporter_FOrc_m00_t00_b01, LineageNPCsTex.e_teleporter_FOrc_m00_t00_f, LineageNPCsTex.e_teleporter_FOrc_m00_t00_h |  |
| `LineageNPCs.orc_head_m00` | missing | 30649 Ancestor Martankus ×1 | LineageNPCsTex.orc_head_t00, LineageNPCsTex.orc_head_t01, LineageNPCsTex.orc_head_t02 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `NO_GRP:31364` | no_npcgrp | 31364 ? ×1 |  |  |
| `NO_GRP:31365` | no_npcgrp | 31365 ? ×1 |  |  |
| `NO_GRP:31424` | no_npcgrp | 31424 ? ×1 |  |  |
| `NO_GRP:31565` | no_npcgrp | 31565 ? ×1 |  |  |
| `NO_GRP:31771` | no_npcgrp | 31771 ? ×1 |  |  |
| `NO_GRP:31784` | no_npcgrp | 31784 ? ×1 |  |  |
| `NO_GRP:31785` | no_npcgrp | 31785 ? ×1 |  |  |
| `NO_GRP:31786` | no_npcgrp | 31786 ? ×1 |  |  |
| `NO_GRP:32056` | no_npcgrp | 32056 ? ×1 |  |  |
| `NO_GRP:35645` | no_npcgrp | 35645 ? ×1 |  |  |

## start_dwarf

Spawn files: `dwarven_starting.json`, `dwarven_village.json`

Unique npcIds: **83**. Present: **24**. Missing/wrong: **59**. Spawn rows: 725.

### Bears in this region

| npcId | name | status | mesh | spawns | files |
|------:|------|--------|------|-------:|-------|
| 20777 | Hunter Bear | missing | `LineageMonsters.grizzly_m00` | 20 | dwarven_starting.json |

### Missing / wrong (36 unique meshes, 59 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters2.northern_goblin_m00` | missing | 21127 Northern Goblin ×29, 21130 Northern Goblin Captain ×43 | LineageMonstersTex2.northern_goblin_t00, LineageMonstersTex2.northern_goblin_t01 |  |
| `LineageMonsters.vampire_bat_m00` | missing | 20370 Barbed Bat ×10, 20480 Blade Bat ×13, 21124 Red Eye Barbed Bat ×30 | LineageMonstersTex.vampire_bat_t00, LineageMonstersTex.vampire_bat_t01 |  |
| `LineageMonsters.onyx_beast_m00` | missing | 20502 Twink Puma ×26, 20510 Crystal Puma ×10 | LineageMonstersTex.onyx_beast_t00 |  |
| `LineageMonsters.poison_spider_m00` | missing | 21125 Northern Trimden ×32 | LineageMonstersTex.poison_spider_t00, LineageMonstersTex.poison_spider_t01 |  |
| `LineageNPCs.e_guard_MDwarf_m00` | missing | 30528 Laferon ×1, 30541 Paion ×1, 30542 Runant ×1, 30543 Ethan ×1, 30544 Cromwell ×1, 30545 Proton ×1, 30546 Dinkey ×1, 30547 Tardyon ×1, 30548 Nathan ×1, 30601 Newbie Guide ×1, 31035 Protector ×19 | LineageNPCsTex.e_guard_MDwarf_m00_t00_b00, LineageNPCsTex.e_guard_MDwarf_m00_t00_b01 |  |
| `LineageMonsters.magical_weaver_m00` | missing | 20153 Magical Weaver ×21 | LineageMonstersTex.magical_weaver_t00, LineageMonstersTex.magical_weaver_t01 |  |
| `LineageMonsters.grizzly_m00` | missing | 20777 Hunter Bear ×20 | LineageMonstersTex.grizzly_t00 |  |
| `LineageNPCs.e_warehouse_keeper_FDwarf_m00` | missing | 30522 Airy ×1, 30539 Chichirin ×1, 30550 Gauri Twinklerock ×1, 30553 Maryse Redbonnet ×1 | LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_h |  |
| `LineageNPCs.a_traderA_MDwarf_m00` | missing | 30516 Reep ×1, 30555 Torocco ×1, 30993 Lottery Ticket Seller ×1 | LineageNPCsTex.a_traderA_MDwarf_m00_t00_b00, LineageNPCsTex.a_traderA_MDwarf_m00_t00_b01, LineageNPCsTex.a_traderA_MDwarf_m00_t00_f, LineageNPCsTex.a_traderA_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_clergyman_MDwarf_m00` | missing | 30538 Zimenf ×1, 30650 Gerald ×1 | LineageNPCsTex.e_clergyman_MDwarf_m00_t00_b00, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_b01, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_f, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_mine_teacher_MDwarf_m00` | missing | 30529 Maron ×1, 30554 Bolter ×1 | LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_b00, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_b01, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_f, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_smith_master_MDwarf_m00` | missing | 30525 Bronk ×1, 30536 Arin ×1 | LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_f, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_warehouse_master_MDwarf_m00` | missing | 30520 Reed ×1, 30531 Lockirin ×1 | LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_celebrant_MDwarf_m00` | missing | 30537 Daichir ×1 | LineageNPCsTex.e_celebrant_MDwarf_m00_t00_b00, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_b01, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_f, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_collector_master_MDwarf_m00` | missing | 30534 Keef ×1 | LineageNPCsTex.e_collector_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_collector_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_collector_master_MDwarf_m00_t00_f |  |
| `LineageNPCs.e_collector_teacher_FDwarf_m00` | missing | 30524 Pippi ×1 | LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_b00, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_b01, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_f, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_common_people_MDwarf_m00` | missing | 30651 Dorf ×1 | LineageNPCsTex.e_common_people_MDwarf_m00_t00_b00, LineageNPCsTex.e_common_people_MDwarf_m00_t00_b01, LineageNPCsTex.e_common_people_MDwarf_m00_t00_f, LineageNPCsTex.e_common_people_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_guard_MOrc_m00` | missing | 30671 Croto ×1 | LineageNPCsTex.e_guard_MOrc_m00_t00_b00, LineageNPCsTex.e_guard_MOrc_m00_t00_b01, LineageNPCsTex.e_guard_MOrc_m00_t00_f, LineageNPCsTex.e_guard_MOrc_m00_t00_h |  |
| `LineageNPCs.e_mine_master_MDwarf_m00` | missing | 30535 Filaur ×1 | LineageNPCsTex.e_mine_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_f, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_mine_teacher_FDwarf_m00` | missing | 30530 Newbie Helper ×1 | LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_b00, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_b01, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_f, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_smith_Fdwarf_m00` | missing | 30527 Silvera ×1 | LineageNPCsTex.e_smith_Fdwarf_m00_t00_b00, LineageNPCsTex.e_smith_Fdwarf_m00_t00_b01, LineageNPCsTex.e_smith_Fdwarf_m00_t00_f, LineageNPCsTex.e_smith_Fdwarf_m00_t00_h |  |
| `LineageNPCs.e_teleporter_FDwarf_m00` | missing | 30540 Wirphy ×1 | LineageNPCsTex.e_teleporter_FDwarf_m00_t00_b00, LineageNPCsTex.e_teleporter_FDwarf_m00_t00_b01 |  |
| `LineageNPCs.e_traderA_FDwarf_m00` | missing | 30517 Shari ×1 | LineageNPCsTex.e_traderA_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_FDwarf_m00_t00_f, LineageNPCsTex.e_traderA_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_traderA_master_MDwarf_m00` | missing | 30532 Spiron ×1 | LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_f, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_traderB_FDwarf_m00` | missing | 30519 Mion ×1 | LineageNPCsTex.e_traderB_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_FDwarf_m00_t00_f, LineageNPCsTex.e_traderB_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_traderB_MDwarf_m00` | missing | 30518 Garita ×1 | LineageNPCsTex.e_traderB_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_MDwarf_m00_t00_f, LineageNPCsTex.e_traderB_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_traderB_master_MDwarf_m00` | missing | 30533 Balanki ×1 | LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_f, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_h |  |
| `LineageNPCs4.e_collector_teacher_MDwarf_m00` | missing | 30523 Gouph ×1 | LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t00, LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `NO_GRP:31362` | no_npcgrp | 31362 ? ×1 |  |  |
| `NO_GRP:31363` | no_npcgrp | 31363 ? ×1 |  |  |
| `NO_GRP:31566` | no_npcgrp | 31566 ? ×1 |  |  |
| `NO_GRP:31772` | no_npcgrp | 31772 ? ×1 |  |  |
| `NO_GRP:31782` | no_npcgrp | 31782 ? ×1 |  |  |
| `NO_GRP:31783` | no_npcgrp | 31783 ? ×1 |  |  |
| `NO_GRP:35644` | no_npcgrp | 35644 ? ×1 |  |  |

## gludin

Spawn files: `gludin.json`, `gludin_turek_orcs.json`, `others_17_20.json`, `others_17_21.json`, `others_17_22.json`, `others_17_23.json`, `others_18_20.json`, `others_18_21.json`, `others_18_22.json`, `others_18_23.json`

Unique npcIds: **194**. Present: **49**. Missing/wrong: **145**. Spawn rows: 1904.

### Bears in this region

| npcId | name | status | mesh | spawns | files |
|------:|------|--------|------|-------:|-------|
| 20021 | Red Bear | missing | `LineageMonsters.grizzly_m00` | 12 | others_17_20.json, others_17_21.json, others_18_20.json, others_18_21.json |
| 20310 | Brown Bear | missing | `LineageMonsters.grizzly_m00` | 21 | others_17_22.json, others_17_23.json, others_18_22.json, others_18_23.json |
| 20335 | Grizzly Bear | missing | `LineageMonsters.grizzly_m00` | 6 | others_17_20.json, others_17_21.json |

### Missing / wrong (81 unique meshes, 145 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.poison_spider_m00` | missing | 20038 Giant Venomous Spider ×81, 20043 Arachnid Tracker ×70, 20050 Poisonous Thornleg ×53, 20926 Arachnid Predator ×4, 21097 Young Araneid ×22 | LineageMonstersTex.poison_spider_t00, LineageMonstersTex.poison_spider_t01 |  |
| `LineageMonsters.sylph_m00` | missing | 20078 Whispering Wind ×14, 21023 Sobbing Wind ×25, 21024 Babbling Wind ×42, 21025 Giggling Wind ×46, 21026 Singing Wind ×31, 30412 Wind Sylph ×1 | LineageMonstersTex.sylph_t00, LineageMonstersTex.sylph_t01 |  |
| `LineageMonsters.ol_mahum_m00` | missing | 20058 Ol Mahum Guard ×29, 20061 Ol Mahum Straggler ×8, 20436 Ol Mahum Supplier ×15, 20437 Ol Mahum Recruit ×15, 20439 Ol Mahum Sergeant ×28, 20548 Ol Mahum Deserter ×9, 20782 Ol Mahum Novice ×10, 21011 Ol Mahum Van Soldier ×18, 27038 Cat's Eye Bandit ×9 | LineageMonstersTex.ol_mahum_t00, LineageMonstersTex.ol_mahum_t01 |  |
| `LineageMonsters.skeleton_m00` | missing | 20035 Skeleton Tracker ×31, 20042 Skeleton Tracker Captain ×54 | LineageMonstersTex.skeleton_t00, LineageMonstersTex.skeleton_t01 |  |
| `LineageMonsters.spartoi_m00` | missing | 20054 Ruin Spartoi ×43, 20060 Raging Spartoi ×23, 20615 Ritmal Swordsman ×13 | LineageMonstersTex.spartoi_t00, LineageMonstersTex.spartoi_t01 |  |
| `LineageMonsters.orc_shaman_m00` | missing | 20501 Turek Orc Priest ×50, 20546 Turek Orc Elder ×15, 30617 Burai ×1 | LineageMonstersTex.orc_shaman_t00 |  |
| `LineageMonsters.skeleton_archer_m00` | missing | 20045 Skeleton Scout ×45, 20051 Skeleton Bowman ×12, 20608 Patin Archer ×8 | LineageMonstersTex.skeleton_archer_t00, LineageMonstersTex.skeleton_archer_t01, LineageMonstersTex.skeleton_archer_t02 |  |
| `LineageMonsters.grizzly_m00` | missing | 20021 Red Bear ×12, 20310 Brown Bear ×21, 20335 Grizzly Bear ×6, 21098 Plain Grizzly ×25 | LineageMonstersTex.grizzly_t00 |  |
| `LineageMonsters.onyx_beast_m00` | missing | 20418 Crystalline Beast ×27, 21096 Cloudy Beast ×21, 21099 Cloudy Beast Turen ×13 | LineageMonstersTex.onyx_beast_t00 |  |
| `LineageMonsters.monster_eye_m00` | missing | 20055 Monster Eye Watchman ×17, 20059 Monster Eye Patrol ×25, 20610 Watchman Of Fire ×12, 27030 Water Seer ×6 | LineageMonstersTex.monster_eye_t00 |  |
| `LineageMonsters.virud_lizardman_m00` | missing | 20008 Felim Lizardman ×6, 20010 Felim Lizardman Scout ×9, 20027 Langk Lizardman Scout ×1, 20030 Langk Lizardman ×22, 20613 Lafi Lizardman ×7, 20921 Maille Lizardman Guard ×2, 30410 Wasteland Lizardman ×1 | LineageMonstersTex.virud_lizardman_t00, LineageMonstersTex.virud_lizardman_t01 |  |
| `LineageMonsters.wererat_m00` | missing | 20358 Ratman Hunter ×21, 20359 Ratman Warrior ×21 | LineageMonstersTex.wererat_t00, LineageMonstersTex.wererat_t01 |  |
| `LineageMonsters.succubus_m00` | missing | 20618 Kanil Succubus ×13, 20747 Roxide ×5, 20776 Dark Succubus ×17 | LineageMonstersTex.succubus_t00, LineageMonstersTex.succubus_t01 |  |
| `LineageMonsters.zombie_m00` | missing | 20026 Ruin Zombie ×8, 20029 Ruin Zombie Leader ×27 | LineageMonstersTex.zombie_t00, LineageMonstersTex.zombie_t01 |  |
| `LineageMonsters.magical_weaver_m00` | missing | 20617 Watchman Of Water ×8, 21012 Magical Eye ×26 | LineageMonstersTex.magical_weaver_t00, LineageMonstersTex.magical_weaver_t01 |  |
| `LineageMonsters.ol_mahum_archer_m00` | missing | 20053 Ol Mahum Patrol ×13, 20063 Ol Mahum Shooter ×7, 21013 Ol Mahum Van Archer ×12 | LineageMonstersTex.ol_mahum_archer_t00, LineageMonstersTex.ol_mahum_archer_t01 |  |
| `LineageMonsters.orc_champion_m00` | missing | 20495 Turek Orc Prefect ×28 | LineageMonstersTex.orc_champion_t00 |  |
| `LineageMonsters.salamander_m00` | missing | 20609 Salamander Lakin ×15, 20612 Salamander Rowin ×9, 27116 Lava Salamander ×4 | LineageMonstersTex.salamander_t00, LineageMonstersTex.salamander_t01 |  |
| `LineageMonsters.ol_mahum_champion_m00` | missing | 20066 Ol Mahum Officer ×11, 20076 Ol Mahum Commander ×6, 20438 Ol Mahum General ×8 | LineageMonstersTex.ol_mahum_champion_t00, LineageMonstersTex.ol_mahum_champion_t01 |  |
| `LineageMonsters.giant_bee_m00` | missing | 21095 Giant Poison Bee ×24 | LineageMonstersTex.giant_bee_t00 |  |
| `LineageMonsters.undine_m00` | missing | 20616 Undine Lakin ×12, 20619 Undine Rowin ×8, 30413 Water Undine ×1 | LineageMonstersTex.undine_t00, LineageMonstersTex.undine_t01 |  |
| `LineageMonsters.bugbear_m00` | missing | 20062 Tumran Bugbear ×12, 20064 Tumran Bugbear Warrior ×7 | LineageMonstersTex.bugbear_t00, LineageMonstersTex.bugbear_t01 |  |
| `LineageMonsters.dre_vanul_m00` | missing | 20611 Dre Vanul Warrior ×14, 20749 Death Fire ×5 | LineageMonstersTex.dre_vanul_t00, LineageMonstersTex.dre_vanul_t01 |  |
| `LineageMonsters.virud_lizardman_warrior_m00` | missing | 20014 Felim Lizardman Warrior ×16, 20922 Maille Lizardman Warrior ×2 | LineageMonstersTex.virud_lizardman_warrior_t00, LineageMonstersTex.virud_lizardman_warrior_t01 |  |
| `LineageMonsters.kobold_m00` | missing | 20606 Kuran Kobold ×7, 20607 Kuran Kobold Warrior ×4 | LineageMonstersTex.kobold_t00, LineageMonstersTex.kobold_t01 |  |
| `LineageMonsters.batur_orc_m00` | missing | 27054 Umbar Orc ×10 | LineageMonstersTex.batur_orc_t00 |  |
| `LineageMonsters.dire_wolf_m00` | missing | 20494 Turek War Hound ×10 |  |  |
| `LineageMonsters.virud_lizardman_scout_m00` | missing | 20614 Lafi Lizardman Scout ×10 | LineageMonstersTex.virud_lizardman_scout_t00, LineageMonstersTex.virud_lizardman_scout_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×5 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `LineageMonsters.mandragora_b_m00` | missing | 27095 Apparition Singing Flower ×3 | LineageMonstersTex.mandragora_b_t00, LineageMonstersTex.mandragora_b_t01 |  |
| `LineageNPCs.a_sanctuary_teacher_FHuman_m00` | missing | 30037 Levian ×1, 30424 Allana ×1, 30665 Kristina ×1 | LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_h_ori |  |
| `LineageNPCs4.a_traderB_MHuman_m00` | missing_textures | 30208 Damion ×1, 30314 Nestle ×1, 30731 Martin ×1 | LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t00, LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t01 |  |
| `LineageNPCs.a_traderA_MHuman_m00` | missing_textures | 30207 Arodin ×1, 30648 Santiago ×1 | LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_f, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_h |  |
| `LineageNPCs.e_common_people_MDwarf_m00` | missing | 30611 Triff ×1, 30674 Daurin Hammercrush ×1 | LineageNPCsTex.e_common_people_MDwarf_m00_t00_b00, LineageNPCsTex.e_common_people_MDwarf_m00_t00_b01, LineageNPCsTex.e_common_people_MDwarf_m00_t00_f, LineageNPCsTex.e_common_people_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_fighterguild_teacher_MOrc_m00` | missing | 30501 Kasman ×1, 30591 Toruku ×1 | LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b00, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b01, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_f, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_h |  |
| `LineageNPCs4.a_chamberlain_MHuman_m00` | missing_textures | 35103 Manor Manager ×2 | LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t00, LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_MHuman_m00` | missing_textures | 30010 Auron ×1, 30435 Leopold ×1 | LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t01 |  |
| `None` | no_mesh_name | 31076 Gatekeeper Ziggurat ×1, 31078 Gatekeeper Ziggurat ×1 | None |  |
| `LineageMonsters.lesser_basilisk_m00` | missing | 30409 Earth Snake ×1 | LineageMonstersTex.lesser_basilisk_t00, LineageMonstersTex.lesser_basilisk_t01 |  |
| `LineageMonsters.mirror_m00` | missing | 27165 Abyssal Jewel 1 ×1 | LineageMonstersTex.mirror_t00, LineageMonstersTex.mirror_t01 |  |
| `LineageMonsters.virud_lizardman_matriarch_m00` | missing | 20924 Maille Lizardman Matriarch ×1 | LineageMonstersTex.virud_lizardman_matriarch_t00, LineageMonstersTex.virud_lizardman_matriarch_t01 |  |
| `LineageNPCs.a_fighterguild_teacher_FElf_m00` | missing | 30374 Rhodiell ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_MElf_m00` | missing | 30369 Baenedes ×1 |  |  |
| `LineageNPCs.a_mageguild_master_FHuman_m00` | missing | 30634 Galatea ×1 |  |  |
| `LineageNPCs.a_mageguild_teacher_MElf_m00` | missing | 30375 Adonius ×1 |  |  |
| `LineageNPCs.a_maidB_FHuman_m00` | missing | 30767 Auctioneer ×1 | LineageNPCsTex.a_maidB_FHuman_m00_t00_b00, LineageNPCsTex.a_maidB_FHuman_m00_t00_b01, LineageNPCsTex.a_maidB_FHuman_m00_t00_f, LineageNPCsTex.a_maidB_FHuman_m00_t00_h |  |
| `LineageNPCs.a_sanctuary_teacher_MHuman_m00` | missing_textures | 30022 Zigaunt ×1 | LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_h |  |
| `LineageNPCs.a_traderC_Mhuman_m00` | missing | 31046 Marsden ×1 | LineageNPCsTex.a_traderC_Mhuman_m00_t00_b00, LineageNPCsTex.a_traderC_Mhuman_m00_t00_b01, LineageNPCsTex.a_traderC_Mhuman_m00_t00_f, LineageNPCsTex.a_traderC_Mhuman_m00_t00_h |  |
| `LineageNPCs.e_patriarch_MOrc_m00` | missing | 30500 Osborn ×1 | LineageNPCsTex.e_patriarch_MOrc_m00_t00_b00, LineageNPCsTex.e_patriarch_MOrc_m00_t00_b01, LineageNPCsTex.e_patriarch_MOrc_m00_t00_f, LineageNPCsTex.e_patriarch_MOrc_m00_t00_h |  |
| `LineageNPCs.e_smith_master_MDwarf_m00` | missing | 30499 Tapoy ×1 | LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_f, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_warehouse_master_MDwarf_m00` | missing | 30498 Moke ×1 | LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.heroes_obelisk_human_m00` | missing | 31690 Monument of Heroes ×1 | LineageNPCsTex.heroes_obelisk_human_t00, LineageNPCsTex.heroes_obelisk_human_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FDarkElf_m00` | missing_textures | 30378 Estella ×1 | LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FHuman_m00` | other_package | 30014 Celma ×1 | LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t01 | lineagenpcs |
| `LineageNPCs4.a_mageguild_teacher_FElf_m00` | missing | 30376 Nell ×1 | LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t01 |  |
| `LineageNPCs4.a_teleporter_FHuman_m00` | missing_textures | 30320 Richlin ×1 | LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t00, LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t01 |  |
| `LineageNPCs4.a_traderB_FHuman_m00` | missing_textures | 30209 Colleen ×1 | LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t00, LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t01 |  |
| `LineageNPCs4.a_wharfinger_MHuman_m00` | other_package | 30387 Clancy ×1 | LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t00, LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t01 | lineagenpcs |
| `NO_GRP:31085` | no_npcgrp | 31085 ? ×1 |  |  |
| `NO_GRP:31092` | no_npcgrp | 31092 ? ×1 |  |  |
| `NO_GRP:31127` | no_npcgrp | 31127 ? ×1 |  |  |
| `NO_GRP:31128` | no_npcgrp | 31128 ? ×1 |  |  |
| `NO_GRP:31129` | no_npcgrp | 31129 ? ×1 |  |  |
| `NO_GRP:31130` | no_npcgrp | 31130 ? ×1 |  |  |
| `NO_GRP:31131` | no_npcgrp | 31131 ? ×1 |  |  |
| `NO_GRP:31137` | no_npcgrp | 31137 ? ×1 |  |  |
| `NO_GRP:31138` | no_npcgrp | 31138 ? ×1 |  |  |
| `NO_GRP:31139` | no_npcgrp | 31139 ? ×1 |  |  |
| `NO_GRP:31140` | no_npcgrp | 31140 ? ×1 |  |  |
| `NO_GRP:31141` | no_npcgrp | 31141 ? ×1 |  |  |
| `NO_GRP:31202` | no_npcgrp | 31202 ? ×1 |  |  |
| `NO_GRP:31416` | no_npcgrp | 31416 ? ×1 |  |  |
| `NO_GRP:31425` | no_npcgrp | 31425 ? ×1 |  |  |
| `NO_GRP:31439` | no_npcgrp | 31439 ? ×1 |  |  |
| `NO_GRP:31567` | no_npcgrp | 31567 ? ×1 |  |  |
| `NO_GRP:31569` | no_npcgrp | 31569 ? ×1 |  |  |
| `NO_GRP:31787` | no_npcgrp | 31787 ? ×1 |  |  |
| `NO_GRP:31788` | no_npcgrp | 31788 ? ×1 |  |  |
| `NO_GRP:31789` | no_npcgrp | 31789 ? ×1 |  |  |
| `NO_GRP:31790` | no_npcgrp | 31790 ? ×1 |  |  |
| `NO_GRP:31791` | no_npcgrp | 31791 ? ×1 |  |  |

## gludio

Spawn files: `gludio.json`, `gludio_queen_ant_guards.json`, `gludio_wasteland.json`, `castles_gludio.json`

Unique npcIds: **111**. Present: **39**. Missing/wrong: **72**. Spawn rows: 894.

### Missing / wrong (53 unique meshes, 72 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.ant_soldier_m00` | missing | 20082 Ant Recruit ×15, 20084 Ant Patrol ×20, 20086 Ant Guard ×21, 20087 Ant Soldier ×23, 20088 Ant Warrior Captain ×23, 20745 Gigantiops ×33 | LineageMonstersTex.ant_soldier_t00, LineageMonstersTex.ant_soldier_t01 |  |
| `LineageMonsters.lesser_basilisk_m00` | missing | 20070 Lesser Basilisk ×48, 20072 Basilisk ×58 | LineageMonstersTex.lesser_basilisk_t00, LineageMonstersTex.lesser_basilisk_t01 |  |
| `LineageMonsters.grave_m00` | missing | 20192 Tyrant ×44, 20193 Elder Tyrant ×18, 21014 Lesser Warlike Tyrant ×24, 21016 Warlike Tyrant ×14 | LineageMonstersTex.grave_t00, LineageMonstersTex.grave_t01 |  |
| `LineageMonsters.ant_m00` | missing | 20079 Ant ×26, 20080 Ant Captain ×26, 20081 Ant Overseer ×16, 29004 Escort Ant ×25 | LineageMonstersTex.ant_t00 |  |
| `LineageMonsters.onyx_beast_m00` | missing | 20074 Androscorpio ×33, 20077 Androscorpio Hunter ×37 | LineageMonstersTex.onyx_beast_t00 |  |
| `LineageMonsters.monster_eye_m00` | missing | 20067 Monster Eye Watcher ×33, 20068 Monster Eye Destroyer ×19 | LineageMonstersTex.monster_eye_t00 |  |
| `LineageMonsters.skeleton_m00` | missing | 20191 Skeleton Raider ×34 | LineageMonstersTex.skeleton_t00, LineageMonstersTex.skeleton_t01 |  |
| `LineageMonsters.ol_mahum_champion_m00` | missing | 21015 Ol Mahum Van Captain ×29 | LineageMonstersTex.ol_mahum_champion_t00, LineageMonstersTex.ol_mahum_champion_t01 |  |
| `LineageMonsters.noble_ant_m00` | missing | 20089 Noble Ant ×19, 20090 Noble Ant Captain ×9 | LineageMonstersTex.noble_ant_t00, LineageMonstersTex.noble_ant_t01 |  |
| `LineageMonsters.ant_larva_m00` | missing | 20075 Ant Larva ×14 | LineageMonstersTex.ant_larva_t00 |  |
| `LineageMonsters.skeleton_archer_m00` | missing | 20190 Skeleton Marauder ×14 | LineageMonstersTex.skeleton_archer_t00, LineageMonstersTex.skeleton_archer_t01, LineageMonstersTex.skeleton_archer_t02 |  |
| `LineageNPCs.e_traderA_FDwarf_m00` | missing | 30659 Orphan Girl ×9 | LineageNPCsTex.e_traderA_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_FDwarf_m00_t00_f, LineageNPCsTex.e_traderA_FDwarf_m00_t00_h |  |
| `LineageMonsters.magical_weaver_m00` | missing | 21012 Magical Eye ×7 | LineageMonstersTex.magical_weaver_t00, LineageMonstersTex.magical_weaver_t01 |  |
| `LineageMonsters.ol_mahum_archer_m00` | missing | 21013 Ol Mahum Van Archer ×3 | LineageMonstersTex.ol_mahum_archer_t00, LineageMonstersTex.ol_mahum_archer_t01 |  |
| `LineageNPCs4.a_traderB_MHuman_m00` | missing_textures | 30294 Varan ×1, 30827 Lundy ×1, 35101 Grad ×1 | LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t00, LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t01 |  |
| `LineageNPCs4.a_chamberlain_MHuman_m00` | missing_textures | 35103 Manor Manager ×2 | LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t00, LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t01 |  |
| `LineageNPCs4.a_wharfinger_MHuman_m00` | other_package | 30434 Samed ×1, 30729 Borys ×1 | LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t00, LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t01 | lineagenpcs |
| `None` | no_mesh_name | 31077 Gatekeeper Ziggurat ×1, 31079 Gatekeeper Ziggurat ×1 | None |  |
| `LineageNPCs.a_fighterguild_teacher_FElf_m00` | missing | 30328 Reisa ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_MElf_m00` | missing | 30327 Sorius ×1 |  |  |
| `LineageNPCs.a_mageguild_teacher_MElf_m00` | missing | 30293 Manuel ×1 |  |  |
| `LineageNPCs.a_maidB_FHuman_m00` | missing | 30767 Auctioneer ×1 | LineageNPCsTex.a_maidB_FHuman_m00_t00_b00, LineageNPCsTex.a_maidB_FHuman_m00_t00_b01, LineageNPCsTex.a_maidB_FHuman_m00_t00_f, LineageNPCsTex.a_maidB_FHuman_m00_t00_h |  |
| `LineageNPCs.a_sanctuary_master_MHuman_m00` | missing_textures | 30289 Raymond ×1 | LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b00, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b01, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_f, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_h |  |
| `LineageNPCs.a_sanctuary_teacher_FHuman_m00` | missing | 30030 Vivyan ×1 | LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_h_ori |  |
| `LineageNPCs.a_traderA_MDwarf_m00` | missing | 30658 Gupu ×1 | LineageNPCsTex.a_traderA_MDwarf_m00_t00_b00, LineageNPCsTex.a_traderA_MDwarf_m00_t00_b01, LineageNPCsTex.a_traderA_MDwarf_m00_t00_f, LineageNPCsTex.a_traderA_MDwarf_m00_t00_h |  |
| `LineageNPCs.a_traderA_MHuman_m00` | missing_textures | 30253 Simplon ×1 | LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_f, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_h |  |
| `LineageNPCs.a_traderC_Mhuman_m00` | missing | 31047 Kell ×1 | LineageNPCsTex.a_traderC_Mhuman_m00_t00_b00, LineageNPCsTex.a_traderC_Mhuman_m00_t00_b01, LineageNPCsTex.a_traderC_Mhuman_m00_t00_f, LineageNPCsTex.a_traderC_Mhuman_m00_t00_h |  |
| `LineageNPCs.e_fighterguild_teacher_MOrc_m00` | missing | 30506 Buka ×1 | LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b00, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b01, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_f, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_h |  |
| `LineageNPCs.e_patriarch_MOrc_m00` | missing | 30505 Drikus ×1 | LineageNPCsTex.e_patriarch_MOrc_m00_t00_b00, LineageNPCsTex.e_patriarch_MOrc_m00_t00_b01, LineageNPCsTex.e_patriarch_MOrc_m00_t00_f, LineageNPCsTex.e_patriarch_MOrc_m00_t00_h |  |
| `LineageNPCs.e_smith_master_MDwarf_m00` | missing | 30504 Mendio ×1 | LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_f, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_warehouse_master_MDwarf_m00` | missing | 30503 Rikadio ×1 | LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.heroes_obelisk_human_m00` | missing | 31690 Monument of Heroes ×1 | LineageNPCsTex.heroes_obelisk_human_t00, LineageNPCsTex.heroes_obelisk_human_t01 |  |
| `LineageNPCs4.a_common_people_MDarkelf_m00` | missing_textures | 30416 Triskel ×1 | LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t00, LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FHuman_m00` | other_package | 30326 Leona ×1 | LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t01 | lineagenpcs |
| `LineageNPCs4.a_fighterguild_teacher_MHuman_m00` | missing_textures | 30325 Audiberti ×1 | LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t01 |  |
| `LineageNPCs4.a_mageguild_teacher_FDarkElf_m00` | missing_textures | 30330 Sidra ×1 | LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t01 |  |
| `LineageNPCs4.a_mageguild_teacher_FElf_m00` | missing | 30345 Ramoniell ×1 | LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t01 |  |
| `LineageNPCs4.a_teleporter_FHuman_m00` | missing_textures | 30256 Bella ×1 | LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t00, LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `LineageNpcs.castle_kent_statue_jewel_m00` | missing | 35063 Gludio Holy Artifact ×1 | LineageNPCsTex.castle_kent_statue_jewel_t00 |  |
| `NO_GRP:31086` | no_npcgrp | 31086 ? ×1 |  |  |
| `NO_GRP:31092` | no_npcgrp | 31092 ? ×1 |  |  |
| `NO_GRP:31203` | no_npcgrp | 31203 ? ×1 |  |  |
| `NO_GRP:31417` | no_npcgrp | 31417 ? ×1 |  |  |
| `NO_GRP:31426` | no_npcgrp | 31426 ? ×1 |  |  |
| `NO_GRP:31440` | no_npcgrp | 31440 ? ×1 |  |  |
| `NO_GRP:31568` | no_npcgrp | 31568 ? ×1 |  |  |
| `NO_GRP:31792` | no_npcgrp | 31792 ? ×1 |  |  |
| `NO_GRP:31793` | no_npcgrp | 31793 ? ×1 |  |  |
| `NO_GRP:31794` | no_npcgrp | 31794 ? ×1 |  |  |
| `NO_GRP:31795` | no_npcgrp | 31795 ? ×1 |  |  |
| `NO_GRP:31796` | no_npcgrp | 31796 ? ×1 |  |  |
| `NO_GRP:35648` | no_npcgrp | 35648 ? ×1 |  |  |

## dion

Spawn files: `dion.json`, `dion_bee_hive.json`, `dion_cruma_marshlands.json`, `dion_execution_grounds.json`, `dion_plains_of_dion.json`, `dion_tanor_canyon.json`, `castles_dion.json`, `floran.json`

Unique npcIds: **153**. Present: **53**. Missing/wrong: **100**. Spawn rows: 1487.

### Bears in this region

| npcId | name | status | mesh | spawns | files |
|------:|------|--------|------|-------:|-------|
| 20930 | Hatu Brown Bear | missing | `LineageMonsters.grizzly_m00` | 57 | dion_bee_hive.json |
| 20932 | Hatu Crimson Bear | missing | `LineageMonsters.crimson_bear_m00` | 34 | dion_bee_hive.json |

### Missing / wrong (82 unique meshes, 100 npcIds)

| mesh | status | npcIds (name × count) | textures missing | alt package |
|------|--------|----------------------|------------------|-------------|
| `LineageMonsters.giant_bee_m00` | missing | 20229 Weird Wasp ×45, 20928 Hatu Weird Wasp ×49, 20934 Wasp Worker ×59, 20935 Wasp Captain ×67 | LineageMonstersTex.giant_bee_t00 |  |
| `LineageMonsters.dire_wolf_m00` | missing | 20205 Dire Wolf ×69, 20929 Hatu Dire Wolf ×51 |  |  |
| `LineageMonsters.monster_eye_m00` | missing | 20068 Monster Eye Destroyer ×42, 20266 Monster Eye Overseer ×26 | LineageMonstersTex.monster_eye_t00 |  |
| `LineageMonsters.dark_horror_m00` | missing | 20171 Specter ×20, 20227 Mist Horror Ripper ×42 | LineageMonstersTex.dark_horror_t00 |  |
| `LineageMonsters.marsh_stakato_worker_m00` | missing | 20230 Marsh Stakato Worker ×59 | LineageMonstersTex.marsh_stakato_worker_t00, LineageMonstersTex.marsh_stakato_worker_t01 |  |
| `LineageMonsters.grizzly_m00` | missing | 20930 Hatu Brown Bear ×57 | LineageMonstersTex.grizzly_t00 |  |
| `LineageMonsters.onyx_beast_m00` | missing | 20250 Glass Jaguar ×22, 20931 Hatu Onyx Beast ×34 | LineageMonstersTex.onyx_beast_t00 |  |
| `LineageMonsters.succubus_m00` | missing | 20944 Nightmare Lord ×48 | LineageMonstersTex.succubus_t00, LineageMonstersTex.succubus_t01 |  |
| `LineageMonsters.marsh_stakato_soldier_m00` | missing | 20232 Marsh Stakato Soldier ×46 | LineageMonstersTex.marsh_stakato_soldier_t00, LineageMonstersTex.marsh_stakato_soldier_t01 |  |
| `LineageMonsters.heltor_silenos_warrior_m00` | missing | 20939 Tanor Silenos Warrior ×43 | LineageMonstersTex.heltor_silenos_warrior_t00, LineageMonstersTex.heltor_silenos_warrior_t01 |  |
| `LineageMonsters.poison_spider_m00` | missing | 20233 Marsh Spider ×39 | LineageMonstersTex.poison_spider_t00, LineageMonstersTex.poison_spider_t01 |  |
| `LineageMonsters.marsh_stakato_m00` | missing | 20157 Marsh Stakato ×36 | LineageMonstersTex.marsh_stakato_t00, LineageMonstersTex.marsh_stakato_t01 |  |
| `LineageMonsters.crimson_bear_m00` | missing | 20932 Hatu Crimson Bear ×34 | LineageMonstersTex.crimson_bear_t00 |  |
| `LineageMonsters.neer_crawler_m00` | missing | 20160 Neer Crawler ×19, 20198 Neer Crawler Berserker ×15 | LineageMonstersTex.neer_crawler_t00, LineageMonstersTex.neer_crawler_t01 |  |
| `LineageMonsters.giant_wild_hog_m00` | missing | 20933 Hatu Windsus ×33 | LineageMonstersTex.giant_wild_hog_t00 |  |
| `LineageMonsters.virud_lizardman_m00` | missing | 20251 Delu Lizardman ×19, 20252 Delu Lizardman Scout ×13 | LineageMonstersTex.virud_lizardman_t00, LineageMonstersTex.virud_lizardman_t01 |  |
| `LineageMonsters.marsh_stakato_drone_m00` | missing | 20234 Marsh Stakato Drone ×29 | LineageMonstersTex.marsh_stakato_drone_t00, LineageMonstersTex.marsh_stakato_drone_t01 |  |
| `LineageMonsters.ant_soldier_m00` | missing | 20228 Giant Crimson Ant ×26 | LineageMonstersTex.ant_soldier_t00, LineageMonstersTex.ant_soldier_t01 |  |
| `LineageMonsters.vampire_bat_m00` | missing | 20202 Corpse Tracker ×26 | LineageMonstersTex.vampire_bat_t00, LineageMonstersTex.vampire_bat_t01 |  |
| `LineageMonsters.giant_leech_m00` | missing | 20225 Giant Mist Leech ×24 | LineageMonstersTex.giant_leech_t00, LineageMonstersTex.giant_leech_t01 |  |
| `LineageMonsters.heltor_silenos_chieftain_m00` | missing | 20941 Tanor Silenos Chieftain ×22 | LineageMonstersTex.heltor_silenos_chieftain_t00, LineageMonstersTex.heltor_silenos_chieftain_t01 |  |
| `LineageMonsters.hungry_spirit_m00` | missing | 20200 Strain ×22 | LineageMonstersTex.hungry_spirit_t00, LineageMonstersTex.hungry_spirit_t01 |  |
| `LineageMonsters.hanged_man_ripper_m00` | missing | 20144 Hangman Tree ×21 | LineageMonstersTex.hanged_man_ripper_t00, LineageMonstersTex.hanged_man_ripper_t01 |  |
| `LineageMonsters.ant_m00` | missing | 20226 Gray Ant ×20 | LineageMonstersTex.ant_t00 |  |
| `LineageMonsters.zombie_m00` | missing | 20201 Ghoul ×16 | LineageMonstersTex.zombie_t00, LineageMonstersTex.zombie_t01 |  |
| `LineageMonsters.silenos_m00` | missing | 20168 Silenos ×15 | LineageMonstersTex.silenos_t00, LineageMonstersTex.silenos_t01 |  |
| `LineageMonsters.sylph_m00` | missing | 20197 Sorrow Maiden ×15 | LineageMonstersTex.sylph_t00, LineageMonstersTex.sylph_t01 |  |
| `LineageMonsters.manadragora_m00` | missing | 20154 Mandragora ×8, 20223 Mandragora ×4 | LineageMonstersTex.manadragora_t00 |  |
| `LineageMonsters.mandragora_b_m00` | missing | 20156 Mandragora Blossom ×9 | LineageMonstersTex.mandragora_b_t00, LineageMonstersTex.mandragora_b_t01 |  |
| `LineageMonsters.virud_lizardman_warrior_m00` | missing | 20253 Delu Lizardman Warrior ×9 | LineageMonstersTex.virud_lizardman_warrior_t00, LineageMonstersTex.virud_lizardman_warrior_t01 |  |
| `LineageMonsters.lesser_basilisk_m00` | missing | 20199 Amber Basilisk ×8 | LineageMonstersTex.lesser_basilisk_t00, LineageMonstersTex.lesser_basilisk_t01 |  |
| `LineageMonsters.skeleton_m00` | missing | 20254 Skeleton Axeman ×8 | LineageMonstersTex.skeleton_t00, LineageMonstersTex.skeleton_t01 |  |
| `LineageMonsters.skeleton_archer_m00` | missing | 20190 Skeleton Marauder ×6 | LineageMonstersTex.skeleton_archer_t00, LineageMonstersTex.skeleton_archer_t01, LineageMonstersTex.skeleton_archer_t02 |  |
| `LineageMonsters.mandragora_a_m00` | missing | 20155 Mandragora Sapling ×5 | LineageMonstersTex.mandragora_a_t00, LineageMonstersTex.mandragora_a_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_MHuman_m00` | missing_textures | 30064 Terry ×1, 30291 Alex  ×1, 30626 Hamil ×1 | LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t01 |  |
| `LineageNPCs4.a_traderB_MHuman_m00` | missing_textures | 30061 Casey ×1, 30828 Waters ×1, 35143 Hodler ×1 | LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t00, LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t01 |  |
| `LineageNPCs.a_traderA_MHuman_m00` | missing_textures | 30060 Sabrin ×1, 30737 Morgan ×1 | LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_f, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_h |  |
| `LineageNPCs.e_fighterguild_teacher_MOrc_m00` | missing | 30509 Dowki ×1, 30642 Chianta ×1 | LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b00, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b01, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_f, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_h |  |
| `LineageNPCs4.a_chamberlain_MHuman_m00` | missing_textures | 35145 Manor Manager ×2 | LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t00, LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FDarkElf_m00` | missing_textures | 30463 Ixia ×1, 30956 Nanarin ×1 | LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t01 |  |
| `LineageNPCs4.a_fighterguild_teacher_FHuman_m00` | other_package | 30065 Arnelle ×1, 30728 Leirynn ×1 | LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t01 | lineagenpcs |
| `LineageNPCs.a_fighterguild_master_FHuman_m00` | missing | 30735 Sophya ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_FElf_m00` | missing | 30460 Cardien ×1 |  |  |
| `LineageNPCs.a_fighterguild_teacher_MElf_m00` | missing | 30459 Wandius ×1 |  |  |
| `LineageNPCs.a_mageguild_teacher_MElf_m00` | missing | 30068 Shegfield ×1 |  |  |
| `LineageNPCs.a_maidB_FHuman_m00` | missing | 30767 Auctioneer ×1 | LineageNPCsTex.a_maidB_FHuman_m00_t00_b00, LineageNPCsTex.a_maidB_FHuman_m00_t00_b01, LineageNPCsTex.a_maidB_FHuman_m00_t00_f, LineageNPCsTex.a_maidB_FHuman_m00_t00_h |  |
| `LineageNPCs.a_sanctuary_master_MHuman_m00` | missing_textures | 30070 Sylvain ×1 | LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b00, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b01, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_f, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_h |  |
| `LineageNPCs.a_sanctuary_teacher_FHuman_m00` | missing | 30067 Glyvka ×1 | LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_h_ori |  |
| `LineageNPCs.a_traderC_Mhuman_m00` | missing | 31048 McDermott ×1 | LineageNPCsTex.a_traderC_Mhuman_m00_t00_b00, LineageNPCsTex.a_traderC_Mhuman_m00_t00_b01, LineageNPCsTex.a_traderC_Mhuman_m00_t00_f, LineageNPCsTex.a_traderC_Mhuman_m00_t00_h |  |
| `LineageNPCs.box_m00` | missing | 30960 Beer Chest ×1 | LineageNPCsTex.box_t00, LineageNPCsTex.box_t01 |  |
| `LineageNPCs.coffer_d_m00` | missing | 30961 Clothes Chest ×1 | LineageNPCsTex.coffer_d_t00 |  |
| `LineageNPCs.e_patriarch_MOrc_m00` | missing | 30508 Castor ×1 | LineageNPCsTex.e_patriarch_MOrc_m00_t00_b00, LineageNPCsTex.e_patriarch_MOrc_m00_t00_b01, LineageNPCsTex.e_patriarch_MOrc_m00_t00_f, LineageNPCsTex.e_patriarch_MOrc_m00_t00_h |  |
| `LineageNPCs.e_smith_master_MDwarf_m00` | missing | 30595 Opix ×1 | LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_f, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.e_traderA_FDwarf_m00` | missing | 31043 Octavia ×1 | LineageNPCsTex.e_traderA_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_FDwarf_m00_t00_f, LineageNPCsTex.e_traderA_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_warehouse_keeper_FDwarf_m00` | missing | 31773 Sonin ×1 | LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_h |  |
| `LineageNPCs.e_warehouse_master_MDwarf_m00` | missing | 30594 Ranspo ×1 | LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_h |  |
| `LineageNPCs.heroes_obelisk_human_m00` | missing | 31690 Monument of Heroes ×1 | LineageNPCsTex.heroes_obelisk_human_t00, LineageNPCsTex.heroes_obelisk_human_t01 |  |
| `LineageNPCs2.agit_maid_a_m00` | missing | 35402 Borna ×1 | LineageNPCsTex2.agit_maid_a_t00, LineageNPCsTex2.agit_maid_a_t01 |  |
| `LineageNPCs4.a_mageguild_teacher_FElf_m00` | missing | 30461 Mirien ×1 | LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t01 |  |
| `LineageNPCs4.a_teleporter_FHuman_m00` | missing_textures | 30059 Trisha ×1 | LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t00, LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t01 |  |
| `LineageNPCs4.a_traderB_FHuman_m00` | missing_textures | 30063 Lars ×1 | LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t00, LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t01 |  |
| `LineageNPCs4.a_wharfinger_MHuman_m00` | other_package | 30625 Mason ×1 | LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t00, LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t01 | lineagenpcs |
| `LineageNPCs4.e_collector_teacher_MDwarf_m00` | missing | 30959 Barbado ×1 | LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t00, LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t01 |  |
| `LineageNpcs.castle_aden_controltower_jewel_m00` | missing | 31031 Broadcasting Tower ×1 | LineageNpcsTex.castle_aden_controltower_jewel_t00 |  |
| `LineageNpcs.castle_kent_statue_jewel_m00` | missing | 35105 Dion Holy Artifact ×1 | LineageNPCsTex.castle_kent_statue_jewel_t00 |  |
| `NO_GRP:31087` | no_npcgrp | 31087 ? ×1 |  |  |
| `NO_GRP:31092` | no_npcgrp | 31092 ? ×1 |  |  |
| `NO_GRP:31204` | no_npcgrp | 31204 ? ×1 |  |  |
| `NO_GRP:31418` | no_npcgrp | 31418 ? ×1 |  |  |
| `NO_GRP:31434` | no_npcgrp | 31434 ? ×1 |  |  |
| `NO_GRP:31441` | no_npcgrp | 31441 ? ×1 |  |  |
| `NO_GRP:31570` | no_npcgrp | 31570 ? ×1 |  |  |
| `NO_GRP:31571` | no_npcgrp | 31571 ? ×1 |  |  |
| `NO_GRP:31797` | no_npcgrp | 31797 ? ×1 |  |  |
| `NO_GRP:31798` | no_npcgrp | 31798 ? ×1 |  |  |
| `NO_GRP:31799` | no_npcgrp | 31799 ? ×1 |  |  |
| `NO_GRP:31800` | no_npcgrp | 31800 ? ×1 |  |  |
| `NO_GRP:31801` | no_npcgrp | 31801 ? ×1 |  |  |
| `NO_GRP:31802` | no_npcgrp | 31802 ? ×1 |  |  |
| `NO_GRP:31803` | no_npcgrp | 31803 ? ×1 |  |  |
| `NO_GRP:35649` | no_npcgrp | 35649 ? ×1 |  |  |
| `None` | no_mesh_name | 31080 Gatekeeper Ziggurat ×1 | None |  |

## Unique missing meshes (all audited regions)

| package | asset | status | regions | used by (sample) | textures |
|---------|-------|--------|---------|------------------|----------|
| `LineageMonsters` | `angel_m00` | missing | start_dark_elf | 30980 Ark Guardian's Corpse | LineageMonstersTex.angel_t00, LineageMonstersTex.angel_t01 |
| `LineageMonsters` | `batur_orc_m00` | missing | gludin | 27054 Umbar Orc | LineageMonstersTex.batur_orc_t00 |
| `LineageMonsters` | `bugbear_m00` | missing | gludin, start_orc | 21122 Kasha Bugbear, 20062 Tumran Bugbear, 20064 Tumran Bugbear Warrior | LineageMonstersTex.bugbear_t00, LineageMonstersTex.bugbear_t01 |
| `LineageMonsters` | `dark_horror_m00` | missing | dion, start_elf | 20033 Shade Horror, 20345 Dark Terror, 20371 Mist Terror, 20171 Specter | LineageMonstersTex.dark_horror_t00 |
| `LineageMonsters` | `dire_wolf_m00` | missing | dion, gludin, start_orc | 21121 Kasha Dire Wolf, 20494 Turek War Hound, 20205 Dire Wolf, 20929 Hatu Dire Wolf | LineageMonstersTex.dire_wolf_t00 |
| `LineageMonsters` | `dre_vanul_m00` | missing | gludin, start_elf, start_orc | 20351 Dre Vanul Tracker, 20353 Dre Vanul Beholder, 27021 Kirunak, 21118 Baar Dre Vanul | LineageMonstersTex.dre_vanul_t00, LineageMonstersTex.dre_vanul_t01 |
| `LineageMonsters` | `dryad_m00` | missing | start_elf | 20013 Dryad, 20019 Dryad Elder, 20336 Green Dryad | LineageMonstersTex.dryad_t00, LineageMonstersTex.dryad_t01 |
| `LineageMonsters` | `giant_bee_m00` | missing | dion, gludin | 21095 Giant Poison Bee, 20229 Weird Wasp, 20928 Hatu Weird Wasp, 20934 Wasp Worker | LineageMonstersTex.giant_bee_t00 |
| `LineageMonsters` | `grizzly_m00` | missing | dion, gludin, start_dwarf, start_orc | 20143 Grizzly Bear, 20479 Kasha Bear, 30592 Hestui Totem Spirit, 20777 Hunter Bear | LineageMonstersTex.grizzly_t00 |
| `LineageMonsters` | `hobgoblin_m00` | missing | start_orc | 21119 Kasha Hobgoblin | LineageMonstersTex.hobgoblin_t00, LineageMonstersTex.hobgoblin_t01 |
| `LineageMonsters` | `imp_m00` | missing | start_dark_elf, start_orc | 20004 Imp, 20005 Imp Elder, 20311 Rakeclaw Imp, 20312 Rakeclaw Imp Hunter | LineageMonstersTex.imp_t00, LineageMonstersTex.imp_t01 |
| `LineageMonsters` | `kobold_m00` | missing | gludin | 20606 Kuran Kobold, 20607 Kuran Kobold Warrior | LineageMonstersTex.kobold_t00, LineageMonstersTex.kobold_t01 |
| `LineageMonsters` | `lesser_basilisk_m00` | missing | dion, gludin, gludio | 30409 Earth Snake, 20070 Lesser Basilisk, 20072 Basilisk, 20199 Amber Basilisk | LineageMonstersTex.lesser_basilisk_t00, LineageMonstersTex.lesser_basilisk_t01 |
| `LineageMonsters` | `magical_weaver_m00` | missing | gludin, gludio, start_dwarf | 20153 Magical Weaver, 20617 Watchman Of Water, 21012 Magical Eye, 21012 Magical Eye | LineageMonstersTex.magical_weaver_t00, LineageMonstersTex.magical_weaver_t01 |
| `LineageMonsters` | `mandragora_b_m00` | missing | dion, gludin | 27095 Apparition Singing Flower, 20156 Mandragora Blossom | LineageMonstersTex.mandragora_b_t00, LineageMonstersTex.mandragora_b_t01 |
| `LineageMonsters` | `mirror_m00` | missing | gludin | 27165 Abyssal Jewel 1 | LineageMonstersTex.mirror_t00, LineageMonstersTex.mirror_t01 |
| `LineageMonsters` | `monster_eye_m00` | missing | dion, gludin, gludio, start_orc | 20426 Eye of Seer, 20428 Evil Eye Patrol, 21257 Evil Eye Seer, 20055 Monster Eye Watchman | LineageMonstersTex.monster_eye_t00 |
| `LineageMonsters` | `ol_mahum_archer_m00` | missing | gludin, gludio | 20053 Ol Mahum Patrol, 20063 Ol Mahum Shooter, 21013 Ol Mahum Van Archer, 21013 Ol Mahum Van Archer | LineageMonstersTex.ol_mahum_archer_t00, LineageMonstersTex.ol_mahum_archer_t01 |
| `LineageMonsters` | `ol_mahum_champion_m00` | missing | gludin, gludio | 20066 Ol Mahum Officer, 20076 Ol Mahum Commander, 20438 Ol Mahum General, 21015 Ol Mahum Van Captain | LineageMonstersTex.ol_mahum_champion_t00, LineageMonstersTex.ol_mahum_champion_t01 |
| `LineageMonsters` | `ol_mahum_m00` | missing | gludin | 20058 Ol Mahum Guard, 20061 Ol Mahum Straggler, 20436 Ol Mahum Supplier, 20437 Ol Mahum Recruit | LineageMonstersTex.ol_mahum_t00, LineageMonstersTex.ol_mahum_t01 |
| `LineageMonsters` | `onyx_beast_m00` | missing | dion, gludin, gludio, start_dwarf | 20502 Twink Puma, 20510 Crystal Puma, 20418 Crystalline Beast, 21096 Cloudy Beast | LineageMonstersTex.onyx_beast_t00 |
| `LineageMonsters` | `orc_champion_m00` | missing | gludin | 20495 Turek Orc Prefect | LineageMonstersTex.orc_champion_t00 |
| `LineageMonsters` | `orc_shaman_m00` | missing | gludin | 20501 Turek Orc Priest, 20546 Turek Orc Elder, 30617 Burai | LineageMonstersTex.orc_shaman_t00 |
| `LineageMonsters` | `pixy_m00` | missing | start_dark_elf, start_elf | 31845 Bloody Pixy, 31848 Fairy, 31852 Pixy Murika | LineageMonstersTex.pixy_t00, LineageMonstersTex.pixy_t01 |
| `LineageMonsters` | `poison_spider_m00` | missing | dion, gludin, start_dwarf | 21125 Northern Trimden, 20038 Giant Venomous Spider, 20043 Arachnid Tracker, 20050 Poisonous Thornleg | LineageMonstersTex.poison_spider_t00, LineageMonstersTex.poison_spider_t01 |
| `LineageMonsters` | `salamander_m00` | missing | gludin | 20609 Salamander Lakin, 20612 Salamander Rowin, 27116 Lava Salamander | LineageMonstersTex.salamander_t00, LineageMonstersTex.salamander_t01 |
| `LineageMonsters` | `skeleton_archer_m00` | missing | dion, gludin, gludio, start_elf | 20463 Dungeon Skeleton Archer, 20045 Skeleton Scout, 20051 Skeleton Bowman, 20608 Patin Archer | LineageMonstersTex.skeleton_archer_t00, LineageMonstersTex.skeleton_archer_t01, LineageMonstersTex.skeleton_archer_t02 |
| `LineageMonsters` | `skeleton_m00` | missing | dion, gludin, gludio, start_elf | 20464 Dungeon Skeleton Soldier, 20504 Dread Soldier, 20035 Skeleton Tracker, 20042 Skeleton Tracker Captain | LineageMonstersTex.skeleton_t00, LineageMonstersTex.skeleton_t01 |
| `LineageMonsters` | `spartoi_m00` | missing | gludin | 20054 Ruin Spartoi, 20060 Raging Spartoi, 20615 Ritmal Swordsman | LineageMonstersTex.spartoi_t00, LineageMonstersTex.spartoi_t01 |
| `LineageMonsters` | `succubus_m00` | missing | dion, gludin | 20618 Kanil Succubus, 20747 Roxide, 20776 Dark Succubus, 20944 Nightmare Lord | LineageMonstersTex.succubus_t00, LineageMonstersTex.succubus_t01 |
| `LineageMonsters` | `sylph_m00` | missing | dion, gludin, start_elf | 20036 Lirein, 20044 Lirein Elder, 20078 Whispering Wind, 21023 Sobbing Wind | LineageMonstersTex.sylph_t00, LineageMonstersTex.sylph_t01 |
| `LineageMonsters` | `ti_mi_riran_m00` | missing | start_dark_elf | 31850 Blight Treant | LineageMonstersTex.ti_mi_riran_t00, LineageMonstersTex.ti_mi_riran_t01, LineageMonstersTex.ti_mi_riran_t02 |
| `LineageMonsters` | `ti_mi_tran_m00` | missing | start_elf | 31851 Treant, 31853 Treant Bremec | LineageMonstersTex.ti_mi_tran_t00, LineageMonstersTex.ti_mi_tran_t01, LineageMonstersTex.ti_mi_tran_t02 |
| `LineageMonsters` | `undine_m00` | missing | gludin | 20616 Undine Lakin, 20619 Undine Rowin, 30413 Water Undine | LineageMonstersTex.undine_t00, LineageMonstersTex.undine_t01 |
| `LineageMonsters` | `vampire_bat_m00` | missing | dion, start_dwarf, start_orc | 20316 Blackwing Bat, 20370 Barbed Bat, 20480 Blade Bat, 21124 Red Eye Barbed Bat | LineageMonstersTex.vampire_bat_t00, LineageMonstersTex.vampire_bat_t01 |
| `LineageMonsters` | `virud_lizardman_m00` | missing | dion, gludin | 20008 Felim Lizardman, 20010 Felim Lizardman Scout, 20027 Langk Lizardman Scout, 20030 Langk Lizardman | LineageMonstersTex.virud_lizardman_t00, LineageMonstersTex.virud_lizardman_t01 |
| `LineageMonsters` | `virud_lizardman_matriarch_m00` | missing | gludin | 20924 Maille Lizardman Matriarch | LineageMonstersTex.virud_lizardman_matriarch_t00, LineageMonstersTex.virud_lizardman_matriarch_t01 |
| `LineageMonsters` | `virud_lizardman_scout_m00` | missing | gludin | 20614 Lafi Lizardman Scout | LineageMonstersTex.virud_lizardman_scout_t00, LineageMonstersTex.virud_lizardman_scout_t01 |
| `LineageMonsters` | `virud_lizardman_warrior_m00` | missing | dion, gludin | 20014 Felim Lizardman Warrior, 20922 Maille Lizardman Warrior, 20253 Delu Lizardman Warrior | LineageMonstersTex.virud_lizardman_warrior_t00, LineageMonstersTex.virud_lizardman_warrior_t01 |
| `LineageMonsters` | `walking_fungus_m00` | missing | start_dark_elf, start_elf, start_orc | 20007 Green Fungus, 20400 Blood Fungus, 20509 Spore Fungus, 20365 Mountain Fungus | LineageMonstersTex.walking_fungus_t00 |
| `LineageMonsters` | `wererat_m00` | missing | gludin, start_elf | 20040 Sukar Ratman, 20047 Sukar Ratman Captain, 27020 Varool Foulclaw, 20358 Ratman Hunter | LineageMonstersTex.wererat_t00, LineageMonstersTex.wererat_t01 |
| `LineageMonsters` | `zombie_m00` | missing | dion, gludin | 20026 Ruin Zombie, 20029 Ruin Zombie Leader, 20201 Ghoul | LineageMonstersTex.zombie_t00, LineageMonstersTex.zombie_t01 |
| `LineageMonsters2` | `northern_goblin_m00` | missing | start_dwarf | 21127 Northern Goblin, 21130 Northern Goblin Captain | LineageMonstersTex2.northern_goblin_t00, LineageMonstersTex2.northern_goblin_t01 |
| `LineageNPCs` | `a_common_people_FDarkElf_m00` | missing_textures | start_dark_elf | 30352 Karina, 30354 Jewel, 30418 Annika, 30652 Uruha | LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_b00, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_b01, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_f, LineageNpcsTex3.a_common_people_FDarkElf_re.a_common_people_FDarkElf_re_m00_t00_h |
| `LineageNPCs` | `a_fighterguild_teacher_FElf_m00` | missing | dion, gludin, gludio, start_dark_elf, start_elf | 30662 Piper Longbow, 30155 Ellenia, 30374 Rhodiell, 30328 Reisa | LineageNPCsTex.a_fighterguild_teacher_FElf_m00_t00_b00, LineageNPCsTex.a_fighterguild_teacher_FElf_m00_t00_b01, LineageNPCsTex.a_fighterguild_teacher_FElf_m00_t00_f, LineageNPCsTex.a_fighterguild_teacher_FElf_m00_t00_h |
| `LineageNPCs` | `a_fighterguild_teacher_MElf_m00` | missing | dion, gludin, gludio, start_elf | 30156 Cobendell, 30369 Baenedes, 30327 Sorius, 30459 Wandius | LineageNPCsTex.a_fighterguild_teacher_MElf_m00_t00_b00, LineageNPCsTex.a_fighterguild_teacher_MElf_m00_t00_b01, LineageNPCsTex.a_fighterguild_teacher_MElf_m00_t00_f, LineageNPCsTex.a_fighterguild_teacher_MElf_m00_t00_h |
| `LineageNPCs` | `a_guard_MElf_m00` | missing | start_dark_elf, start_elf | 30663 Slein Shining Blade, 30664 Cain Flying Knife, 30216 Wheeler, 30217 Berros | LineageNPCsTex.a_guard_MElf_m00_t00_b00, LineageNPCsTex.a_guard_MElf_m00_t00_b01, LineageNPCsTex.a_guard_MElf_m00_t00_h |
| `LineageNPCs` | `a_mageguild_master_FHuman_m00` | missing | gludin | 30634 Galatea | LineageNPCsTex.a_mageguild_master_FHuman_m00_t00_b00, LineageNPCsTex.a_mageguild_master_FHuman_m00_t00_b01, LineageNPCsTex.a_mageguild_master_FHuman_m00_t00_f, LineageNPCsTex.a_mageguild_master_FHuman_m00_t00_h |
| `LineageNPCs` | `a_mageguild_master_MHuman_m00` | missing_textures | start_talking_island | 30017 Gallint | LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_f, LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_h |
| `LineageNPCs` | `a_mageguild_teacher_MElf_m00` | missing | dion, gludin, gludio, start_elf | 30158 Esrandell, 30375 Adonius, 30293 Manuel, 30068 Shegfield | LineageNPCsTex.a_mageguild_teacher_MElf_m00_t00_b00, LineageNPCsTex.a_mageguild_teacher_MElf_m00_t00_b01, LineageNPCsTex.a_mageguild_teacher_MElf_m00_t00_f, LineageNPCsTex.a_mageguild_teacher_MElf_m00_t00_h |
| `LineageNPCs` | `a_maidB_FHuman_m00` | missing | dion, gludin, gludio | 30767 Auctioneer, 30767 Auctioneer, 30767 Auctioneer | LineageNPCsTex.a_maidB_FHuman_m00_t00_b00, LineageNPCsTex.a_maidB_FHuman_m00_t00_b01, LineageNPCsTex.a_maidB_FHuman_m00_t00_f, LineageNPCsTex.a_maidB_FHuman_m00_t00_h |
| `LineageNPCs` | `a_sanctuary_master_MHuman_m00` | missing_textures | dion, gludio, start_talking_island | 30031 Biotin, 30289 Raymond, 30070 Sylvain | LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b00, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_b01, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_f, LineageNpcsTex3.a_sanctuary_master_MHuman_re.a_sanctuary_master_MHuman_re_t00_h |
| `LineageNPCs` | `a_sanctuary_teacher_FHuman_m00` | missing | dion, gludin, gludio, start_talking_island | 31776 Iris, 30037 Levian, 30424 Allana, 30665 Kristina | LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_FHuman_re.a_sanctuary_teacher_FHuman_re_m00_t00_h_ori |
| `LineageNPCs` | `a_sanctuary_teacher_MHuman_m00` | missing_textures | gludin, start_talking_island | 30032 Yohanes, 30036 Petron, 30598 Newbie Guide, 30022 Zigaunt | LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_f, LineageNpcsTex3.a_sanctuary_teacher_MHuman_re.a_sanctuary_teacher_MHuman_re_m00_t00_h |
| `LineageNPCs` | `a_traderA_MDwarf_m00` | missing | gludio, start_dwarf | 30516 Reep, 30555 Torocco, 30993 Lottery Ticket Seller, 30658 Gupu | LineageNPCsTex.a_traderA_MDwarf_m00_t00_b00, LineageNPCsTex.a_traderA_MDwarf_m00_t00_b01, LineageNPCsTex.a_traderA_MDwarf_m00_t00_f, LineageNPCsTex.a_traderA_MDwarf_m00_t00_h |
| `LineageNPCs` | `a_traderA_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island | 30001 Lector, 30207 Arodin, 30648 Santiago, 30253 Simplon | LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b00, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_b01, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_f, LineageNpcsTex3.a_traderA_MHuman_re.a_traderA_MHuman_re_m00_t00_h |
| `LineageNPCs` | `a_traderC_Mhuman_m00` | missing | dion, gludin, gludio, start_elf | 31046 Marsden, 31046 Marsden, 31047 Kell, 31048 McDermott | LineageNPCsTex.a_traderC_Mhuman_m00_t00_b00, LineageNPCsTex.a_traderC_Mhuman_m00_t00_b01, LineageNPCsTex.a_traderC_Mhuman_m00_t00_f, LineageNPCsTex.a_traderC_Mhuman_m00_t00_h |
| `LineageNPCs` | `a_trader_FDarkelf_m00` | missing_textures | start_dark_elf | 30135 Iria, 30138 Minaless, 30420 Tyra | LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_b00, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_b01, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_f, LineageNpcsTex3.a_trader_FDarkElf_re.a_trader_FDarkElf_re_m00_t00_h |
| `LineageNPCs` | `a_trader_MElf_m00` | missing | start_elf | 30147 Unoren, 30149 Creamees, 30150 Herbiel, 30151 Chad | LineageNPCsTex.a_trader_MElf_m00_t00_b00, LineageNPCsTex.a_trader_MElf_m00_t00_b01, LineageNPCsTex.a_trader_MElf_m00_t00_f, LineageNPCsTex.a_trader_MElf_m00_t00_h |
| `LineageNPCs` | `coffer_a_m00` | missing | start_dark_elf | 30977 Holy Ark of Secrecy 1 | LineageNPCsTex.coffer_a_t00 |
| `LineageNPCs` | `e_celebrant_MDwarf_m00` | missing | start_dwarf | 30537 Daichir | LineageNPCsTex.e_celebrant_MDwarf_m00_t00_b00, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_b01, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_f, LineageNPCsTex.e_celebrant_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_clergyman_MDwarf_m00` | missing | start_dwarf | 30538 Zimenf, 30650 Gerald | LineageNPCsTex.e_clergyman_MDwarf_m00_t00_b00, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_b01, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_f, LineageNPCsTex.e_clergyman_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_collector_master_MDwarf_m00` | missing | start_dwarf | 30534 Keef | LineageNPCsTex.e_collector_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_collector_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_collector_master_MDwarf_m00_t00_f |
| `LineageNPCs` | `e_collector_teacher_FDwarf_m00` | missing | start_dwarf | 30524 Pippi | LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_b00, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_b01, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_f, LineageNPCsTex.e_collector_teacher_FDwarf_m00_t00_h |
| `LineageNPCs` | `e_common_people_MDwarf_m00` | missing | gludin, start_dwarf | 30651 Dorf, 30611 Triff, 30674 Daurin Hammercrush | LineageNPCsTex.e_common_people_MDwarf_m00_t00_b00, LineageNPCsTex.e_common_people_MDwarf_m00_t00_b01, LineageNPCsTex.e_common_people_MDwarf_m00_t00_f, LineageNPCsTex.e_common_people_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_common_people_MOrc_m00` | missing | start_orc | 30585 Tataru Zu Hestui, 30587 Gantaki Zu Urutu | LineageNPCsTex.e_common_people_MOrc_m00_t00_b00, LineageNPCsTex.e_common_people_MOrc_m00_t00_b01, LineageNPCsTex.e_common_people_MOrc_m00_t00_f, LineageNPCsTex.e_common_people_MOrc_m00_t00_h |
| `LineageNPCs` | `e_fighterguild_teacher_MOrc_m00` | missing | dion, gludin, gludio, start_orc | 30566 Varkees, 30567 Tantus, 30568 Hatos, 30569 Brukurse | LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b00, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_b01, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_f, LineageNPCsTex.e_fighterguild_teacher_MOrc_m00_t00_h |
| `LineageNPCs` | `e_guard_MDwarf_m00` | missing | start_dwarf | 30528 Laferon, 30541 Paion, 30542 Runant, 30543 Ethan | LineageNPCsTex.e_guard_MDwarf_m00_t00_b00, LineageNPCsTex.e_guard_MDwarf_m00_t00_b01 |
| `LineageNPCs` | `e_guard_MOrc_m00` | missing | start_dwarf, start_orc | 30577 Rukain, 30578 Nakusin, 30579 Tamai, 30580 Parugon | LineageNPCsTex.e_guard_MOrc_m00_t00_b00, LineageNPCsTex.e_guard_MOrc_m00_t00_b01, LineageNPCsTex.e_guard_MOrc_m00_t00_f, LineageNPCsTex.e_guard_MOrc_m00_t00_h |
| `LineageNPCs` | `e_mine_master_MDwarf_m00` | missing | start_dwarf | 30535 Filaur | LineageNPCsTex.e_mine_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_f, LineageNPCsTex.e_mine_master_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_mine_teacher_FDwarf_m00` | missing | start_dwarf | 30530 Newbie Helper | LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_b00, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_b01, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_f, LineageNPCsTex.e_mine_teacher_FDwarf_m00_t00_h |
| `LineageNPCs` | `e_mine_teacher_MDwarf_m00` | missing | start_dwarf | 30529 Maron, 30554 Bolter | LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_b00, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_b01, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_f, LineageNPCsTex.e_mine_teacher_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_patriarch_MOrc_m00` | missing | dion, gludin, gludio, start_orc | 30565 Kakai, 30500 Osborn, 30505 Drikus, 30508 Castor | LineageNPCsTex.e_patriarch_MOrc_m00_t00_b00, LineageNPCsTex.e_patriarch_MOrc_m00_t00_b01, LineageNPCsTex.e_patriarch_MOrc_m00_t00_f, LineageNPCsTex.e_patriarch_MOrc_m00_t00_h |
| `LineageNPCs` | `e_smith_Fdwarf_m00` | missing | start_dwarf | 30527 Silvera | LineageNPCsTex.e_smith_Fdwarf_m00_t00_b00, LineageNPCsTex.e_smith_Fdwarf_m00_t00_b01, LineageNPCsTex.e_smith_Fdwarf_m00_t00_f, LineageNPCsTex.e_smith_Fdwarf_m00_t00_h |
| `LineageNPCs` | `e_smith_master_MDwarf_m00` | missing | dion, gludin, gludio, start_dwarf | 30525 Bronk, 30536 Arin, 30499 Tapoy, 30504 Mendio | LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_f, LineageNPCsTex.e_smith_master_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_teleporter_FDwarf_m00` | missing | start_dwarf | 30540 Wirphy | LineageNPCsTex.e_teleporter_FDwarf_m00_t00_b00, LineageNPCsTex.e_teleporter_FDwarf_m00_t00_b01 |
| `LineageNPCs` | `e_teleporter_FOrc_m00` | missing | start_orc | 30576 Tamil | LineageNPCsTex.e_teleporter_FOrc_m00_t00_b00, LineageNPCsTex.e_teleporter_FOrc_m00_t00_b01, LineageNPCsTex.e_teleporter_FOrc_m00_t00_f, LineageNPCsTex.e_teleporter_FOrc_m00_t00_h |
| `LineageNPCs` | `e_traderA_FDwarf_m00` | missing | dion, gludio, start_dwarf | 30517 Shari, 30659 Orphan Girl, 31043 Octavia | LineageNPCsTex.e_traderA_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_FDwarf_m00_t00_f, LineageNPCsTex.e_traderA_FDwarf_m00_t00_h |
| `LineageNPCs` | `e_traderA_master_MDwarf_m00` | missing | start_dwarf | 30532 Spiron | LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_f, LineageNPCsTex.e_traderA_master_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_traderB_FDwarf_m00` | missing | start_dwarf | 30519 Mion | LineageNPCsTex.e_traderB_FDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_FDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_FDwarf_m00_t00_f, LineageNPCsTex.e_traderB_FDwarf_m00_t00_h |
| `LineageNPCs` | `e_traderB_MDwarf_m00` | missing | start_dwarf | 30518 Garita | LineageNPCsTex.e_traderB_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_MDwarf_m00_t00_f, LineageNPCsTex.e_traderB_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_traderB_master_MDwarf_m00` | missing | start_dwarf | 30533 Balanki | LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_f, LineageNPCsTex.e_traderB_master_MDwarf_m00_t00_h |
| `LineageNPCs` | `e_trader_MOrc_m00` | missing | start_orc | 30558 Jakal, 30560 Uska, 30561 Papuma, 30562 Grookin | LineageNPCsTex.e_trader_MOrc_m00_t00_b00, LineageNPCsTex.e_trader_MOrc_m00_t00_b01, LineageNPCsTex.e_trader_MOrc_m00_t00_f, LineageNPCsTex.e_trader_MOrc_m00_t00_h |
| `LineageNPCs` | `e_warehouse_keeper_FDwarf_m00` | missing | dion, start_dwarf | 30522 Airy, 30539 Chichirin, 30550 Gauri Twinklerock, 30553 Maryse Redbonnet | LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_h |
| `LineageNPCs` | `e_warehouse_master_MDwarf_m00` | missing | dion, gludin, gludio, start_dwarf | 30520 Reed, 30531 Lockirin, 30498 Moke, 30503 Rikadio | LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b00, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_b01, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_f, LineageNPCsTex.e_warehouse_master_MDwarf_m00_t00_h |
| `LineageNPCs` | `heroes_obelisk_human_m00` | missing | dion, gludin, gludio, start_talking_island | 31690 Monument of Heroes, 31690 Monument of Heroes, 31690 Monument of Heroes, 31690 Monument of Heroes | LineageNPCsTex.heroes_obelisk_human_t00, LineageNPCsTex.heroes_obelisk_human_t01 |
| `LineageNPCs` | `orc_head_m00` | missing | start_orc | 30649 Ancestor Martankus | LineageNPCsTex.orc_head_t00, LineageNPCsTex.orc_head_t01, LineageNPCsTex.orc_head_t02 |
| `LineageNPCs4` | `a_chamberlain_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island | 35103 Manor Manager, 35103 Manor Manager, 35103 Manor Manager, 35145 Manor Manager | LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t00, LineageNpcsTex3.a_chamberlain_mhuman_re.a_chamberlain_mhuman_re_t01 |
| `LineageNPCs4` | `a_common_people_MDarkelf_m00` | missing_textures | gludio, start_dark_elf | 30307 Karrod, 30351 Astaron, 30353 Jughead, 30415 Charkeren | LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t00, LineageNpcsTex3.a_common_people_MDarkElf_re.a_common_people_MDarkElf_re_t01 |
| `LineageNPCs4` | `a_fighterguild_teacher_FDarkElf_m00` | missing_textures | dion, gludin, start_dark_elf | 30143 Trudy, 30378 Estella, 30463 Ixia, 30956 Nanarin | LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FDarkelf_re.a_fighterguild_teacher_FDarkelf_re_t01 |
| `LineageNPCs4` | `a_fighterguild_teacher_FHuman_m00` | other_package | dion, gludin, gludio, start_talking_island | 30029 Minia, 30014 Celma, 30326 Leona, 30065 Arnelle | LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_FHuman_re.a_fighterguild_teacher_FHuman_re_t01 |
| `LineageNPCs4` | `a_fighterguild_teacher_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island | 30009 Newbie Helper, 30027 Gwinter, 30028 Pintage, 30010 Auron | LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t00, LineageNpcsTex3.a_fighterguild_teacher_MHuman_re.a_fighterguild_teacher_MHuman_re_t01 |
| `LineageNPCs4` | `a_mageguild_teacher_FDarkElf_m00` | missing_textures | gludio, start_dark_elf | 30144 Harne, 30330 Sidra | LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FDarkElf_re.a_mageguild_teacher_FDarkElf_re_t01 |
| `LineageNPCs4` | `a_mageguild_teacher_FElf_m00` | missing | dion, gludin, gludio, start_elf | 30157 Greenis, 30376 Nell, 30345 Ramoniell, 30461 Mirien | LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t00, LineageNpcsTex3.a_mageguild_teacher_FELf_re.a_mageguild_teacher_FELf_re_t01 |
| `LineageNPCs4` | `a_patriarch_MElf_m00` | missing | start_elf | 30154 Asterios | LineageNpcsTex3.a_patriarch_Melf_re.a_patriarch_Melf_re_t00, LineageNpcsTex3.a_patriarch_Melf_re.a_patriarch_Melf_re_t01 |
| `LineageNPCs4` | `a_teleporter_FHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island | 30006 Roxxy, 30427 Siff, 30429 Tiramisa, 30320 Richlin | LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t00, LineageNpcsTex3.a_Teleporter_Fhuman_re.a_Teleporter_Fhuman_re_t01 |
| `LineageNPCs4` | `a_traderB_FHuman_m00` | missing_textures | dion, gludin, start_talking_island | 30004 Katerina, 30209 Colleen, 30063 Lars | LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t00, LineageNpcsTex3.a_traderB_FHuman_re.a_traderB_FHuman_re_t01 |
| `LineageNPCs4` | `a_traderB_MHuman_m00` | missing_textures | dion, gludin, gludio | 30208 Damion, 30314 Nestle, 30731 Martin, 30294 Varan | LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t00, LineageNpcsTex3.a_traderB_MHuman_re.a_traderB_MHuman_re_t01 |
| `LineageNPCs4` | `a_trader_FElf_m00` | missing | start_elf | 30148 Ariel | LineageNpcsTex3.a_trader_FElf_re.a_trader_FElf_re_t00, LineageNpcsTex3.a_trader_FElf_re.a_trader_FElf_re_t01 |
| `LineageNPCs4` | `a_wharfinger_MHuman_m00` | other_package | dion, gludin, gludio, start_talking_island | 30047 Firon, 30387 Clancy, 30434 Samed, 30729 Borys | LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t00, LineageNpcsTex3.a_wharfinger_mhuman_re.a_wharfinger_mhuman_re_t01 |
| `LineageNPCs4` | `e_collector_teacher_MDwarf_m00` | missing | dion, start_dwarf | 30523 Gouph, 30959 Barbado | LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t00, LineageNpcsTex3.e_collector_teacher_MDwarf_re_a.e_collector_teacher_MDwarf_re_a_t01 |
| `LineageNPCs4` | `e_fighterguild_teacher_FOrc_m00` | missing | start_orc | 30559 Kunai, 30570 Karukia, 30669 Kakan | LineageNpcsTex3.e_fighterguild_teacher_Forc_re.e_fighterguild_teacher_Forc_re_t00, LineageNpcsTex3.e_fighterguild_teacher_Forc_re.e_fighterguild_teacher_Forc_re_t01 |
| `LineageNpcs` | `castle_aden_controltower_jewel_m00` | missing | dion, gludin, gludio, start_dark_elf, start_dwarf, start_elf, start_orc, start_talking_island | 31031 Broadcasting Tower, 31031 Broadcasting Tower, 31031 Broadcasting Tower, 31031 Broadcasting Tower | LineageNpcsTex.castle_aden_controltower_jewel_t00 |
| `LineageMonsters` | `ant_larva_m00` | missing | gludio | 20075 Ant Larva | LineageMonstersTex.ant_larva_t00 |
| `LineageMonsters` | `ant_m00` | missing | dion, gludio | 20079 Ant, 20080 Ant Captain, 20081 Ant Overseer, 29004 Escort Ant | LineageMonstersTex.ant_t00 |
| `LineageMonsters` | `ant_soldier_m00` | missing | dion, gludio | 20082 Ant Recruit, 20084 Ant Patrol, 20086 Ant Guard, 20087 Ant Soldier | LineageMonstersTex.ant_soldier_t00, LineageMonstersTex.ant_soldier_t01 |
| `LineageMonsters` | `crimson_bear_m00` | missing | dion | 20932 Hatu Crimson Bear | LineageMonstersTex.crimson_bear_t00 |
| `LineageMonsters` | `giant_leech_m00` | missing | dion | 20225 Giant Mist Leech | LineageMonstersTex.giant_leech_t00, LineageMonstersTex.giant_leech_t01 |
| `LineageMonsters` | `giant_wild_hog_m00` | missing | dion | 20933 Hatu Windsus | LineageMonstersTex.giant_wild_hog_t00 |
| `LineageMonsters` | `grave_m00` | missing | gludio | 20192 Tyrant, 20193 Elder Tyrant, 21014 Lesser Warlike Tyrant, 21016 Warlike Tyrant | LineageMonstersTex.grave_t00, LineageMonstersTex.grave_t01 |
| `LineageMonsters` | `hanged_man_ripper_m00` | missing | dion | 20144 Hangman Tree | LineageMonstersTex.hanged_man_ripper_t00, LineageMonstersTex.hanged_man_ripper_t01 |
| `LineageMonsters` | `heltor_silenos_chieftain_m00` | missing | dion | 20941 Tanor Silenos Chieftain | LineageMonstersTex.heltor_silenos_chieftain_t00, LineageMonstersTex.heltor_silenos_chieftain_t01 |
| `LineageMonsters` | `heltor_silenos_warrior_m00` | missing | dion | 20939 Tanor Silenos Warrior | LineageMonstersTex.heltor_silenos_warrior_t00, LineageMonstersTex.heltor_silenos_warrior_t01 |
| `LineageMonsters` | `hungry_spirit_m00` | missing | dion | 20200 Strain | LineageMonstersTex.hungry_spirit_t00, LineageMonstersTex.hungry_spirit_t01 |
| `LineageMonsters` | `manadragora_m00` | missing | dion | 20154 Mandragora, 20223 Mandragora | LineageMonstersTex.manadragora_t00 |
| `LineageMonsters` | `mandragora_a_m00` | missing | dion | 20155 Mandragora Sapling | LineageMonstersTex.mandragora_a_t00, LineageMonstersTex.mandragora_a_t01 |
| `LineageMonsters` | `marsh_stakato_drone_m00` | missing | dion | 20234 Marsh Stakato Drone | LineageMonstersTex.marsh_stakato_drone_t00, LineageMonstersTex.marsh_stakato_drone_t01 |
| `LineageMonsters` | `marsh_stakato_m00` | missing | dion | 20157 Marsh Stakato | LineageMonstersTex.marsh_stakato_t00, LineageMonstersTex.marsh_stakato_t01 |
| `LineageMonsters` | `marsh_stakato_soldier_m00` | missing | dion | 20232 Marsh Stakato Soldier | LineageMonstersTex.marsh_stakato_soldier_t00, LineageMonstersTex.marsh_stakato_soldier_t01 |
| `LineageMonsters` | `marsh_stakato_worker_m00` | missing | dion | 20230 Marsh Stakato Worker | LineageMonstersTex.marsh_stakato_worker_t00, LineageMonstersTex.marsh_stakato_worker_t01 |
| `LineageMonsters` | `neer_crawler_m00` | missing | dion | 20160 Neer Crawler, 20198 Neer Crawler Berserker | LineageMonstersTex.neer_crawler_t00, LineageMonstersTex.neer_crawler_t01 |
| `LineageMonsters` | `noble_ant_m00` | missing | gludio | 20089 Noble Ant, 20090 Noble Ant Captain | LineageMonstersTex.noble_ant_t00, LineageMonstersTex.noble_ant_t01 |
| `LineageMonsters` | `silenos_m00` | missing | dion | 20168 Silenos | LineageMonstersTex.silenos_t00, LineageMonstersTex.silenos_t01 |
| `LineageNPCs` | `a_fighterguild_master_FHuman_m00` | missing | dion | 30735 Sophya | LineageNPCsTex.a_fighterguild_master_FHuman_m00_t00_b00, LineageNPCsTex.a_fighterguild_master_FHuman_m00_t00_b01, LineageNPCsTex.a_fighterguild_master_FHuman_m00_t00_f, LineageNPCsTex.a_fighterguild_master_FHuman_m00_t00_h |
| `LineageNPCs` | `box_m00` | missing | dion | 30960 Beer Chest | LineageNPCsTex.box_t00, LineageNPCsTex.box_t01 |
| `LineageNPCs` | `coffer_d_m00` | missing | dion | 30961 Clothes Chest | LineageNPCsTex.coffer_d_t00 |
| `LineageNPCs2` | `agit_maid_a_m00` | missing | dion | 35402 Borna | LineageNPCsTex2.agit_maid_a_t00, LineageNPCsTex2.agit_maid_a_t01 |
| `LineageNpcs` | `castle_kent_statue_jewel_m00` | missing | dion, gludio | 35063 Gludio Holy Artifact, 35105 Dion Holy Artifact | LineageNPCsTex.castle_kent_statue_jewel_t00 |

## Export batch 1 (start areas + Gludin)

103 unique meshes to export/fix.

| package | asset | status | regions |
|---------|-------|--------|---------|
| `LineageMonsters` | `angel_m00` | missing | start_dark_elf |
| `LineageMonsters` | `batur_orc_m00` | missing | gludin |
| `LineageMonsters` | `bugbear_m00` | missing | gludin, start_orc |
| `LineageMonsters` | `dark_horror_m00` | missing | dion, start_elf |
| `LineageMonsters` | `dire_wolf_m00` | missing | dion, gludin, start_orc |
| `LineageMonsters` | `dre_vanul_m00` | missing | gludin, start_elf, start_orc |
| `LineageMonsters` | `dryad_m00` | missing | start_elf |
| `LineageMonsters` | `giant_bee_m00` | missing | dion, gludin |
| `LineageMonsters` | `grizzly_m00` | missing | dion, gludin, start_dwarf, start_orc |
| `LineageMonsters` | `hobgoblin_m00` | missing | start_orc |
| `LineageMonsters` | `imp_m00` | missing | start_dark_elf, start_orc |
| `LineageMonsters` | `kobold_m00` | missing | gludin |
| `LineageMonsters` | `lesser_basilisk_m00` | missing | dion, gludin, gludio |
| `LineageMonsters` | `magical_weaver_m00` | missing | gludin, gludio, start_dwarf |
| `LineageMonsters` | `mandragora_b_m00` | missing | dion, gludin |
| `LineageMonsters` | `mirror_m00` | missing | gludin |
| `LineageMonsters` | `monster_eye_m00` | missing | dion, gludin, gludio, start_orc |
| `LineageMonsters` | `ol_mahum_archer_m00` | missing | gludin, gludio |
| `LineageMonsters` | `ol_mahum_champion_m00` | missing | gludin, gludio |
| `LineageMonsters` | `ol_mahum_m00` | missing | gludin |
| `LineageMonsters` | `onyx_beast_m00` | missing | dion, gludin, gludio, start_dwarf |
| `LineageMonsters` | `orc_champion_m00` | missing | gludin |
| `LineageMonsters` | `orc_shaman_m00` | missing | gludin |
| `LineageMonsters` | `pixy_m00` | missing | start_dark_elf, start_elf |
| `LineageMonsters` | `poison_spider_m00` | missing | dion, gludin, start_dwarf |
| `LineageMonsters` | `salamander_m00` | missing | gludin |
| `LineageMonsters` | `skeleton_archer_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageMonsters` | `skeleton_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageMonsters` | `spartoi_m00` | missing | gludin |
| `LineageMonsters` | `succubus_m00` | missing | dion, gludin |
| `LineageMonsters` | `sylph_m00` | missing | dion, gludin, start_elf |
| `LineageMonsters` | `ti_mi_riran_m00` | missing | start_dark_elf |
| `LineageMonsters` | `ti_mi_tran_m00` | missing | start_elf |
| `LineageMonsters` | `undine_m00` | missing | gludin |
| `LineageMonsters` | `vampire_bat_m00` | missing | dion, start_dwarf, start_orc |
| `LineageMonsters` | `virud_lizardman_m00` | missing | dion, gludin |
| `LineageMonsters` | `virud_lizardman_matriarch_m00` | missing | gludin |
| `LineageMonsters` | `virud_lizardman_scout_m00` | missing | gludin |
| `LineageMonsters` | `virud_lizardman_warrior_m00` | missing | dion, gludin |
| `LineageMonsters` | `walking_fungus_m00` | missing | start_dark_elf, start_elf, start_orc |
| `LineageMonsters` | `wererat_m00` | missing | gludin, start_elf |
| `LineageMonsters` | `zombie_m00` | missing | dion, gludin |
| `LineageMonsters2` | `northern_goblin_m00` | missing | start_dwarf |
| `LineageNPCs` | `a_common_people_FDarkElf_m00` | missing_textures | start_dark_elf |
| `LineageNPCs` | `a_fighterguild_teacher_FElf_m00` | missing | dion, gludin, gludio, start_dark_elf, start_elf |
| `LineageNPCs` | `a_fighterguild_teacher_MElf_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageNPCs` | `a_guard_MElf_m00` | missing | start_dark_elf, start_elf |
| `LineageNPCs` | `a_mageguild_master_FHuman_m00` | missing | gludin |
| `LineageNPCs` | `a_mageguild_master_MHuman_m00` | missing_textures | start_talking_island |
| `LineageNPCs` | `a_mageguild_teacher_MElf_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageNPCs` | `a_maidB_FHuman_m00` | missing | dion, gludin, gludio |
| `LineageNPCs` | `a_sanctuary_master_MHuman_m00` | missing_textures | dion, gludio, start_talking_island |
| `LineageNPCs` | `a_sanctuary_teacher_FHuman_m00` | missing | dion, gludin, gludio, start_talking_island |
| `LineageNPCs` | `a_sanctuary_teacher_MHuman_m00` | missing_textures | gludin, start_talking_island |
| `LineageNPCs` | `a_traderA_MDwarf_m00` | missing | gludio, start_dwarf |
| `LineageNPCs` | `a_traderA_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island |
| `LineageNPCs` | `a_traderC_Mhuman_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageNPCs` | `a_trader_FDarkelf_m00` | missing_textures | start_dark_elf |
| `LineageNPCs` | `a_trader_MElf_m00` | missing | start_elf |
| `LineageNPCs` | `coffer_a_m00` | missing | start_dark_elf |
| `LineageNPCs` | `e_celebrant_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_clergyman_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_collector_master_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_collector_teacher_FDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_common_people_MDwarf_m00` | missing | gludin, start_dwarf |
| `LineageNPCs` | `e_common_people_MOrc_m00` | missing | start_orc |
| `LineageNPCs` | `e_fighterguild_teacher_MOrc_m00` | missing | dion, gludin, gludio, start_orc |
| `LineageNPCs` | `e_guard_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_guard_MOrc_m00` | missing | start_dwarf, start_orc |
| `LineageNPCs` | `e_mine_master_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_mine_teacher_FDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_mine_teacher_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_patriarch_MOrc_m00` | missing | dion, gludin, gludio, start_orc |
| `LineageNPCs` | `e_smith_Fdwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_smith_master_MDwarf_m00` | missing | dion, gludin, gludio, start_dwarf |
| `LineageNPCs` | `e_teleporter_FDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_teleporter_FOrc_m00` | missing | start_orc |
| `LineageNPCs` | `e_traderA_FDwarf_m00` | missing | dion, gludio, start_dwarf |
| `LineageNPCs` | `e_traderA_master_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_traderB_FDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_traderB_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_traderB_master_MDwarf_m00` | missing | start_dwarf |
| `LineageNPCs` | `e_trader_MOrc_m00` | missing | start_orc |
| `LineageNPCs` | `e_warehouse_keeper_FDwarf_m00` | missing | dion, start_dwarf |
| `LineageNPCs` | `e_warehouse_master_MDwarf_m00` | missing | dion, gludin, gludio, start_dwarf |
| `LineageNPCs` | `heroes_obelisk_human_m00` | missing | dion, gludin, gludio, start_talking_island |
| `LineageNPCs` | `orc_head_m00` | missing | start_orc |
| `LineageNPCs4` | `a_chamberlain_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island |
| `LineageNPCs4` | `a_common_people_MDarkelf_m00` | missing_textures | gludio, start_dark_elf |
| `LineageNPCs4` | `a_fighterguild_teacher_FDarkElf_m00` | missing_textures | dion, gludin, start_dark_elf |
| `LineageNPCs4` | `a_fighterguild_teacher_FHuman_m00` | other_package | dion, gludin, gludio, start_talking_island |
| `LineageNPCs4` | `a_fighterguild_teacher_MHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island |
| `LineageNPCs4` | `a_mageguild_teacher_FDarkElf_m00` | missing_textures | gludio, start_dark_elf |
| `LineageNPCs4` | `a_mageguild_teacher_FElf_m00` | missing | dion, gludin, gludio, start_elf |
| `LineageNPCs4` | `a_patriarch_MElf_m00` | missing | start_elf |
| `LineageNPCs4` | `a_teleporter_FHuman_m00` | missing_textures | dion, gludin, gludio, start_talking_island |
| `LineageNPCs4` | `a_traderB_FHuman_m00` | missing_textures | dion, gludin, start_talking_island |
| `LineageNPCs4` | `a_traderB_MHuman_m00` | missing_textures | dion, gludin, gludio |
| `LineageNPCs4` | `a_trader_FElf_m00` | missing | start_elf |
| `LineageNPCs4` | `a_wharfinger_MHuman_m00` | other_package | dion, gludin, gludio, start_talking_island |
| `LineageNPCs4` | `e_collector_teacher_MDwarf_m00` | missing | dion, start_dwarf |
| `LineageNPCs4` | `e_fighterguild_teacher_FOrc_m00` | missing | start_orc |
| `LineageNpcs` | `castle_aden_controltower_jewel_m00` | missing | dion, gludin, gludio, start_dark_elf, start_dwarf, start_elf, start_orc, start_talking_island |

## Export batch 2+ (Gludio, Dion) — later

25 additional unique meshes not already in batch 1.

## Region totals

| region | unique ids | present | missing/wrong | spawn rows |
|--------|-----------:|--------:|--------------:|-----------:|
| start_talking_island | 89 | 70 | 19 | 634 |
| start_dark_elf | 80 | 47 | 33 | 510 |
| start_elf | 88 | 32 | 56 | 1085 |
| start_orc | 80 | 19 | 61 | 1343 |
| start_dwarf | 83 | 24 | 59 | 725 |
| gludin | 194 | 49 | 145 | 1904 |
| gludio | 111 | 39 | 72 | 894 |
| dion | 153 | 53 | 100 | 1487 |

