# Missing NPC / monster models — full spawn audit

> Snapshot taken before Batch 4 (Aden) import. After this PR, Aden's 45 official meshes are present; the five Classic-only Aden misses listed in `missing.md` remain. Re-run `/tmp/npc_audit_all.py` after later batches to refresh ranks.


Audit of every backend spawn file against `Npcgrp_Classic.txt` and Unity `Data/Animations/{package}/{asset}` prefabs.
Batches 1–3 already covered start areas, Gludin, and unique Gludio/Dion meshes.
Ranking below is by unique **missing meshes** (`missing` / `no_prefab`), not texture-only or no-npcgrp ids.

## Ranked remaining regions (unique missing meshes)

| rank | region | unique ids | present | spawn rows | missing meshes | new missing (not in start/Gludin/Gludio/Dion) | only in this region | missing_textures | other |
|-----:|--------|-----------:|--------:|-----------:|---------------:|-----------------------------------------------:|--------------------:|-----------------:|------:|
| 1 | field_tiles | 596 | 245 | 6893 | 56 | 56 | 17 | 7 | 217 |
| 2 | aden | 208 | 88 | 2887 | 50 | 50 | 19 | 13 | 19 |
| 3 | giran | 209 | 90 | 2439 | 47 | 47 | 6 | 12 | 20 |
| 4 | catacombs | 212 | 14 | 2722 | 34 | 34 | 11 | 0 | 102 |
| 5 | heine | 112 | 42 | 1433 | 28 | 28 | 18 | 12 | 15 |
| 6 | oren | 145 | 72 | 1630 | 20 | 20 | 4 | 15 | 20 |
| 7 | goddard | 329 | 20 | 3283 | 12 | 12 | 3 | 6 | 282 |
| 8 | rune | 248 | 22 | 2410 | 6 | 5 | 1 | 2 | 210 |
| 9 | schuttgart | 151 | 12 | 907 | 6 | 5 | 0 | 2 | 123 |
| 10 | hunters | 74 | 41 | 451 | 2 | 2 | 1 | 9 | 14 |
| 11 | dwarven_mines | 13 | 12 | 511 | 1 | 1 | 0 | 0 | 0 |
| 12 | script_npcs | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 |

## Previously audited regions (after batches 1–3)

| region | unique ids | present | spawn rows | missing meshes | missing_textures | other |
|--------|-----------:|--------:|-----------:|---------------:|-----------------:|------:|
| start_talking_island | 89 | 72 | 634 | 0 | 9 | 2 |
| start_dark_elf | 80 | 59 | 510 | 0 | 5 | 6 |
| start_elf | 88 | 78 | 1085 | 0 | 3 | 7 |
| start_orc | 80 | 67 | 1343 | 0 | 1 | 10 |
| start_dwarf | 83 | 74 | 725 | 0 | 2 | 7 |
| gludin | 194 | 150 | 1904 | 0 | 11 | 27 |
| gludio | 111 | 81 | 894 | 0 | 10 | 17 |
| dion | 153 | 115 | 1487 | 1 | 11 | 19 |

## Still missing from earlier regions

- `LineageNPCs2.agit_maid_a_m00` regions=dion, rune, schuttgart names=Borna, Patty, Alice

## Remaining unique missing meshes (new regions): 148

| package | asset | regions | sample names |
|---------|-------|---------|--------------|
| `LineageMonsters` | `banshee_m00` | aden, catacombs, field_tiles, giran | Erin Ediunce, Bloody Banshee, Martyrium Priest, Mausoleum Inquisitor, Barrow Priest |
| `LineageMonsters` | `batur_orc_archer_m00` | field_tiles, giran, oren | Breka Orc Archer, Timak Orc Archer, Tamlin Orc Archer |
| `LineageMonsters` | `batur_orc_overlord_m00` | giran, oren | Breka Orc Prefect, Voltar, Timak Orc Prefect, Timak Orc Troop Captain |
| `LineageMonsters` | `batur_orc_shaman_a_m00` | field_tiles, giran, oren | Breka Orc Shaman, Timak Orc Shaman, Driko |
| `LineageMonsters` | `batur_orc_warrior_m00` | giran, oren | Breka Orc Warrior, Timak Orc Warrior |
| `LineageMonsters` | `binding_m00` | catacombs, giran | Fettered Soul, Martyrium Watchman, Martyrium Monk, Barrow Monk |
| `LineageMonsters` | `blade_stakato_drone_m00` | catacombs, heine | Stakato Drone, Ancient Holy Ground's Totem |
| `LineageMonsters` | `blade_stakato_m00` | heine | Stakato |
| `LineageMonsters` | `blade_stakato_soldier_m00` | catacombs, heine | Stakato Warrior, Catacomb Stakato Soldier |
| `LineageMonsters` | `blade_stakato_worker_m00` | heine | Stakato Worker |
| `LineageMonsters` | `bloody_queen_m00` | aden, catacombs, giran | Hallate's Maid, Limal Karinness, Pytan, Bloody Lady, Sepulcher Inquisitor |
| `LineageMonsters` | `blue_eye_m00` | aden | Kaleidos the Unicorn |
| `LineageMonsters` | `cave_maiden_m00` | aden, giran | Palibati, Cave Maiden, Stenoa Gorgon Queen |
| `LineageMonsters` | `crimson_doll_m00` | catacombs, giran, heine | Hell Keeper Crimson Doll, Wandering Spirit of the Dungeon |
| `LineageMonsters` | `crocodile_m00` | heine | Alligator |
| `LineageMonsters` | `crokian_m00` | heine | Crokian, Crokian Warrior, Crokian Lad, Crokian Lad Warrior |
| `LineageMonsters` | `cutie_cat_m00` | field_tiles | Mimi the Cat |
| `LineageMonsters` | `dailaon_m00` | heine | Dailaon, Dailaon Lad |
| `LineageMonsters` | `death_blader_m00` | aden, catacombs, field_tiles, giran, heine | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord |
| `LineageMonsters` | `death_knight_m00` | aden, catacombs, field_tiles, giran, heine | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian |
| `LineageMonsters` | `death_lord_m00` | catacombs | Ancient Holy Ground's Guardian Spirit |
| `LineageMonsters` | `devastator_m00` | aden | Kadios, Graveyard Predator, Swamp Predator |
| `LineageMonsters` | `doll_blader_m00` | giran, heine | Maiden Doll, Sairon, Wandering Dead of the Dungeon |
| `LineageMonsters` | `doll_master_m00` | catacombs, giran, heine | Netherworld Prophet, Doll Master, Garden Guard Captain, Wandering Evil Thoughts of the Dungeon, Sepulcher Sage |
| `LineageMonsters` | `drake_m00` | aden, field_tiles, giran, oren | Crimson Drake, Drake, Exarion, Weird Drake, Marsh Drake |
| `LineageMonsters` | `dwarf_ghost_m00` | field_tiles | Forest of Mirrors Ghost, Gatekeeper of Hell |
| `LineageMonsters` | `elf_ghost_m00` | field_tiles | Forest of Mirrors Ghost, Biggerstaff |
| `LineageMonsters` | `farhite_m00` | heine | Farhite, Farhite Lad, Swamp Tribe |
| `LineageMonsters` | `field_stalker_m00` | aden, field_tiles, giran, heine | Deprive, Hallate's Watchman, Musveren, Eva's Watchman, Ossiud |
| `LineageMonsters` | `fiend_archer_m00` | aden, field_tiles, giran | Archer of Abyss, Fiend Archer, Bloody Sniper, Flame Drake |
| `LineageMonsters` | `gamlin_m00` | aden | Gamlin |
| `LineageMonsters` | `gargoyle_m00` | aden, catacombs, field_tiles, oren | Grave Guard, Manashen Gargoyle, Reinforced Gargoyle, Catacomb Gargoyle, Purgatory Gargoyle |
| `LineageMonsters` | `great_white_shark_m00` | heine | Giant White Shark |
| `LineageMonsters` | `harpy_m00` | giran | Harpy |
| `LineageMonsters` | `headless_knight_m00` | field_tiles, giran | Headless Knight, Dark Guard |
| `LineageMonsters` | `heltor_silenos_m00` | aden | Vanor Silenos, Vanor Silenos Soldier, Vanor Silenos Scout |
| `LineageMonsters` | `heltor_silenos_shaman_m00` | aden | Vanor Silenos Shaman |
| `LineageMonsters` | `human_ghost_m00` | aden, field_tiles | Ghost of the Tower, Soul of Ruins, Ghost Guardian, Ghost, Forest of Mirrors Ghost |
| `LineageMonsters` | `hunter_gargoyle_m00` | field_tiles, giran, oren | Hunter Gargoyle, Dustwind Gargoyle, Ancient Gargoyle |
| `LineageMonsters` | `iron_golem_m00` | dwarven_mines, field_tiles, oren | Reinforced Steel Golem, Iron Ore Golem, Perum |
| `LineageMonsters` | `kaim_vanul_m00` | aden, giran | Hallate's Guardian, Kaim Vanul |
| `LineageMonsters` | `karik_m00` | aden, giran | Hallate's Inspector, Karik, Bloody Lord |
| `LineageMonsters` | `kel_mahum_archer_m00` | aden | Platinum Tribe Archer, Platinum Tribe Guardian Archer |
| `LineageMonsters` | `kel_mahum_champion_m00` | aden | Platinum Tribe Prefect, Binder, Platinum Tribe Guardian Prefect |
| `LineageMonsters` | `kel_mahum_m00` | aden | Platinum Tribe Soldier |
| `LineageMonsters` | `kel_mahum_warrior_m00` | aden | Platinum Tribe Warrior, Platinum Tribe Guardian Warrior |
| `LineageMonsters` | `kel_mahum_witch_doctor_m00` | aden | Platinum Tribe Shaman, Platinum Tribe Guardian Shaman, Platinum Tribe Guardian Chief |
| `LineageMonsters` | `ketra_orc_archer_a_m00` | aden, field_tiles | Hames Orc Sniper, Taik Orc Archer, Fallen Orc Archer |
| `LineageMonsters` | `ketra_orc_m00` | aden, field_tiles | Taik Orc Supply, Taik Orc Watchman, Hames Orc Scout, Hames Orc Foot Soldier, Taik Orc |
| `LineageMonsters` | `ketra_orc_shaman_m00` | aden, field_tiles | Hames Orc Shaman, Taik Orc Shaman, Fallen Orc Shaman |
| `LineageMonsters` | `ketra_orc_warrior_a_m00` | aden, field_tiles | Taik Orc Supply Officer , Hames Orc Prefect, Taik Orc Warrior, Taik Orc Captain, Fallen Orc Captain |
| `LineageMonsters` | `king_bugbear_m00` | oren | Tarlk Bugbear Captain, Innersen |
| `LineageMonsters` | `leogul_m00` | aden | Leogul, Mutated Creation |
| `LineageMonsters` | `lesser_giant_m00` | aden | Lesser Giant Soldier, Lesser Giant Warrior |
| `LineageMonsters` | `lesser_giant_mage_m00` | aden | Lesser Giant Wizard, Lesser Giant Elder, Forgotten Ancient People |
| `LineageMonsters` | `lesser_giant_shooter_m00` | aden | Giant's Shadow |
| `LineageMonsters` | `lienrik_m00` | heine | Lienrik, Lienrik Lad |
| `LineageMonsters` | `light_bringer_m00` | giran | Light Bringer |
| `LineageMonsters` | `light_worm_m00` | heine | Light Worm |
| `LineageMonsters` | `liviona_m00` | aden, catacombs, giran, heine | Crendion, Graveyard Lich, Zaken's Watchman, Bloody Liviona, Wandering Ghost of the Dungeon |
| `LineageMonsters` | `mana_vampire_m00` | field_tiles | Excuro, Carinkain |
| `LineageMonsters` | `medusa_m00` | aden, catacombs, field_tiles, giran | Hatar Hanishee, Medusa, Hell Keeper Medusa, Ancient Holy Ground's Hermit, Shyslassys |
| `LineageMonsters` | `mimic1_m00` | field_tiles | Treasure Chest |
| `LineageMonsters` | `nos_m00` | heine | Nos, Nos Lad |
| `LineageMonsters` | `oel_mahum_m00` | field_tiles, oren | Oel Mahum, Inner Castle Gatekeeper, Outer Castle Gatekeeper |
| `LineageMonsters` | `oel_mahum_warrior_m00` | oren | Oel Mahum Warrior |
| `LineageMonsters` | `oel_mahum_witch_doctor_m00` | oren | Oel Mahum Shaman |
| `LineageMonsters` | `ogre_m00` | heine | Ogre |
| `LineageMonsters` | `pan_m00` | hunters | Satyr, Satyr Elder |
| `LineageMonsters` | `pirates_zombie_captain_m00` | giran | Pirate Zombie Captain |
| `LineageMonsters` | `pirates_zombie_m00` | giran | Pirate Zombie |
| `LineageMonsters` | `raikel_m00` | field_tiles | Laikel |
| `LineageMonsters` | `rakul_m00` | heine | Rakul |
| `LineageMonsters` | `riba_m00` | field_tiles | Riba, Riba Iren |
| `LineageMonsters` | `saber_toothed_tiger_m00` | aden, field_tiles | Beast Lord, Sharp Talon Tiger, Fiend of Cold |
| `LineageMonsters` | `serpent_slave_m00` | aden, catacombs, giran | Trives, Cave Keeper, Catacomb Serpent, Purgatory Serpent |
| `LineageMonsters` | `shadeless_m00` | catacombs, giran | Ancient Holy Ground's Vision |
| `LineageMonsters` | `shadow_m00` | catacombs, field_tiles | Catacomb Shadow, Purgatory Shadow, Shadow Turen |
| `LineageMonsters` | `silhouette_m00` | catacombs, giran | Silhouette Tilfo, Ancient Holy Ground's Wing |
| `LineageMonsters` | `skeleton_knight_m00` | aden, catacombs, field_tiles, giran | Hallate's Warrior, Doom Warrior, Maluk Knight, Gilmore, Theodric |
| `LineageMonsters` | `skeleton_pikeman_m00` | aden, catacombs, giran | Doom Trooper, Zaken's Pikeman, Decayed Ancient Pikeman |
| `LineageMonsters` | `skeleton_royal_guard_m00` | aden, catacombs, giran | Hallate's Royal Guard, Spiteful Soul Leader, Doom Scout, Royal Cave Servant, Maluk Soldier |
| `LineageMonsters` | `susceptor_m00` | field_tiles | Susceptor |
| `LineageMonsters` | `theeder_m00` | heine | Piker, Mage |
| `LineageMonsters` | `troll_m00` | field_tiles, giran, oren | Cave Beast, Formor, Formor Elder, Premo |
| `LineageMonsters` | `twin_headed_giant_m00` | field_tiles, giran | Grandis, Cave Howler, Lakin |
| `LineageMonsters` | `unicorn_boxer_m00` | field_tiles, goddard | Aurora the Unicorn, Unicorn |
| `LineageMonsters` | `unicorn_m00` | field_tiles, hunters | Unicorn, Elder the Unicorn, Eva's Unicorn |
| `LineageMonsters` | `unicorn_mirage_m00` | field_tiles | Phantasm the Unicorn |
| `LineageMonsters` | `unpleasant_humming_m00` | aden, giran | Ghastly Warrior, Soldier of Grief, Dismal Humming |
| `LineageMonsters` | `vale_master_m00` | aden, catacombs, field_tiles, giran | Corrupt Sage, Magus Valac, Veil Master, Graveyard Sage, Icarus |
| `LineageMonsters` | `virud_lizardman_shaman_a_m00` | field_tiles, heine | Tasaba Lizardman Shaman, Delu Lizardman Shaman, Maille Lizardman Shaman, Langk Lizardman Shaman |
| `LineageMonsters` | `water_giant_m00` | heine | Water Giant |
| `LineageMonsters` | `will_o_wisp_m00` | aden, catacombs, field_tiles, giran | Paliote, Tairim, Glow Wisp, Ghost Fire, Death Wave |
| `LineageMonsters` | `wyrm_m00` | aden, field_tiles, giran, goddard, oren | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai |
| `LineageMonsters` | `zenta_lizardman_m00` | field_tiles, oren | Leto Lizardman, Leto Lizardman Soldier, Harit Lizardman, Harit Lizardman Soldier |
| `LineageMonsters` | `zenta_lizardman_matriarch_m00` | field_tiles, oren | Leto Lizardman Overlord, Lord of the Plains, Harit Lizardman Matriarch |
| `LineageMonsters` | `zenta_lizardman_scout_m00` | field_tiles, oren | Leto Lizardman Archer, Harit Lizardman Archer |
| `LineageMonsters` | `zenta_lizardman_shaman_m00` | field_tiles, goddard, oren | Raido, Leto Lizardman Shaman, Harit Lizardman Shaman |
| `LineageMonsters` | `zenta_lizardman_warrior_m00` | field_tiles, oren | Leto Lizardman Warrior, Harit Lizardman Warrior |
| `LineageMonsters14` | `ancient_giant_bowgun_m00` | aden | Lesser Giant Shooter |
| `LineageMonsters14` | `ancient_giant_warrior_m00` | aden | Barif |
| `LineageMonsters2` | `apostle_warrior_m00` | field_tiles, rune | Guardian of Forbidden Knowledge, Keeper of the Holy Edict, Watcher of the Ancient Pledge |
| `LineageMonsters2` | `bandersnatch_m00` | aden | Halingka |
| `LineageMonsters2` | `brain_buster_m00` | aden | Yintzu |
| `LineageMonsters2` | `ifrit_m00` | goddard | Fiery Ifrit |
| `LineageMonsters2` | `lilim_assassin_m00` | catacombs | Lilim Butcher, Lilim Marauder, Lilim Assassin, Lilim Slayer |
| `LineageMonsters2` | `lilim_knight_m00` | catacombs | Lilim Knight-Errant, Lilim Knight, Lilim Knight Captain, Lilim Royal Knight |
| `LineageMonsters2` | `lilim_wizard_m00` | catacombs | Lilim Magus, Lilim Priest, Lilim Soldier, Lilim Great Magus |
| `LineageMonsters2` | `nephilim_lord_m00` | catacombs | Nephilim Swordsman, Nephilim Centurion, Nephilim Praetorian, Nephilim Commander |
| `LineageMonsters2` | `nephilim_priest_m00` | catacombs, giran | Nephilim Cardinal, Nephilim Archbishop, Nephilim Priest, Nephilim Bishop |
| `LineageMonsters2` | `nephilim_slayer_m00` | catacombs, giran | Nephilim Guard, Nephilim Scout, Nephilim Sentinel |
| `LineageMonsters2` | `temple_guard_a_m00` | catacombs | Lith Scout, Lith Guard, Lith Patrolman |
| `LineageMonsters2` | `temple_guard_b_m00` | catacombs | Lith Warrior, Lith Prefect, Lith Commander |
| `LineageMonsters2` | `temple_savant_a_m00` | catacombs | Lith Witch, Lith Medium, Lith Shaman |
| `LineageMonsters2` | `tomb_guard_a_m00` | catacombs | Gigant Slave, Gigant Footman, Gigant Raider |
| `LineageMonsters2` | `tomb_guard_b_m00` | catacombs | Gigant Overseer, Gigant Officer |
| `LineageMonsters2` | `tomb_savant_a_m00` | catacombs | Gigant Acolyte, Gigant Cleric, Gigant Confessor |
| `LineageMonsters2` | `vampire_soldier_m00` | giran | Mordred |
| `LineageMonsters5` | `ice_elemental_bomb_m00` | field_tiles | Spirit of Cold |
| `LineageMonsters5` | `ice_knight_m00` | field_tiles | Ice Monster |
| `LineageMonsters5` | `scapegoat_m00` | heine | Condemned of Infinity |
| `LineageMonsters9` | `minotaur_fanatic_m00` | field_tiles | Swampland Watchman |
| `LineageNPCs` | `a_common_peopleC_Mhuman_m00` | goddard, rune, schuttgart | Hadley, Rowell, Gompus, Gallic, Kendrew |
| `LineageNPCs` | `a_hardin_MHuman_m00` | giran | Hardin |
| `LineageNPCs` | `a_hardins_pupil_FHuman_m00` | field_tiles, giran | Ritasha, Cema, Kalis, Cleo |
| `LineageNPCs` | `a_hardins_pupil_MHuman_m00` | giran, goddard | Orim, Kaspar, Pilgrim of Darkness |
| `LineageNPCs` | `a_maidA_FHuman_m00` | aden, giran, schuttgart | Dianne, Adrienne, Gladys, Serena |
| `LineageNPCs` | `a_traderC_Fhuman_m00` | goddard | Branhillde |
| `LineageNPCs` | `codex_stone_m00` | field_tiles, goddard, rune | Tablet Of Vision |
| `LineageNPCs` | `coffer_b_m00` | aden, field_tiles | Holy Ark of Secrecy 2, Box Of Titan |
| `LineageNPCs` | `coffer_c_m00` | field_tiles | Holy Ark of Secrecy 3 |
| `LineageNPCs` | `e_elmore_guard_Mhuman_m00` | goddard, rune, schuttgart | Mass Gatekeeper, Outer Castle Gatekeeper, Inner Castle Gatekeeper, Gatekeeper |
| `LineageNPCs` | `e_mageguild_teacher_FOrc_m00` | heine | Takina |
| `LineageNPCs` | `godad_holything_m00` | goddard, schuttgart | Altar of Fire, Altar of Water, Altar of Earth |
| `LineageNPCs` | `heart_of_warding_m00` | field_tiles | Heart of Warding |
| `LineageNPCs` | `human_tree_m00` | field_tiles | Fairy Tree of Wind, Fairy Tree of Star, Twilight Fairy Tree, Fairy Tree of Abyss, Tree |
| `LineageNPCs` | `pot_m00` | heine | Medium Jar |
| `LineageNPCs` | `yeti_cute_m00` | goddard | Green |
| `LineageNPCs` | `yetis_dinner_table_m00` | field_tiles | Summer Feast |
| `LineageNPCs2` | `agit_maid_c_m00` | goddard, schuttgart | Aida, Neria |
| `LineageNPCs2` | `gustaf_athebaldt_m00` | oren | Sir Gustav Athebaldt |
| `LineageNPCs2` | `iason_haine_m00` | heine | Iason Heine |
| `LineageNPCs2` | `madam_claudia_m00` | aden | Claudia Athebaldt |
| `LineageNPCs4` | `a_traderB_Mhuman_pet_m00` | aden, giran | Lemper, Cooper |
| `LineageNPCs4` | `castle_warrior_m00` | field_tiles | Ghost of Solitude |
| `LineageNPCsEV` | `rune_holything_m00` | rune | Divine Altar |
| `LineageNpcsEV` | `n_event_stakato_egg_m00` | aden, giran | ? |

## field_tiles — 56 new missing meshes

Spawn files: `others_18_19.json`, `others_18_24.json`, `others_19_16.json`, `others_19_16_pagan_altar.json`, `others_19_18.json`, `others_19_19.json`, `others_19_20.json`, `others_19_21.json`, `others_19_22.json`, `others_19_23.json`, `others_19_24.json`, `others_20_14.json`, `others_20_15.json`, `others_20_17.json`, `others_20_20.json`, `others_20_21.json`, `others_20_22.json`, `others_20_23.json`, `others_21_14.json`, `others_21_16.json`, `others_21_18.json`, `others_21_19.json`, `others_21_20.json`, `others_21_21.json`, `others_21_22.json`, `others_21_23.json`, `others_21_24.json`, `others_21_25.json`, `others_22_13.json`, `others_22_14.json`, `others_22_15.json`, `others_22_17.json`, `others_22_18.json`, `others_22_19.json`, `others_22_23.json`, `others_23_12.json`, `others_23_13.json`, `others_23_14.json`, `others_23_15.json`, `others_23_16.json`, `others_23_17.json`, `others_23_18.json`, `others_23_19.json`, `others_23_20.json`, `others_23_22.json`, `others_23_24.json`, `others_24_12.json`, `others_24_15.json`, `others_24_17.json`, `others_24_18.json`, `others_24_19.json`, `others_24_20.json`, `others_24_21.json`, `others_25_14.json`, `others_25_15.json`, `others_25_16.json`, `others_25_17.json`, `others_25_19.json`, `others_25_20.json`, `others_border_outpost_npcs.json`, `others_daimons_altar.json`, `others_monster_race_track_npcs.json`, `others_treasure_boxes.json`, `others_yetis_table.json`

| mesh | names |
|------|-------|
| `LineageMonsters.banshee_m00` | Erin Ediunce, Bloody Banshee, Martyrium Priest, Mausoleum Inquisitor, Barrow Priest, Graveyard Inquisitor |
| `LineageMonsters.batur_orc_archer_m00` | Breka Orc Archer, Timak Orc Archer, Tamlin Orc Archer |
| `LineageMonsters.batur_orc_shaman_a_m00` | Breka Orc Shaman, Timak Orc Shaman, Driko |
| `LineageMonsters.cutie_cat_m00` | Mimi the Cat |
| `LineageMonsters.death_blader_m00` | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord, Bloody Guardian |
| `LineageMonsters.death_knight_m00` | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian, Wandering Spirit of Abyss |
| `LineageMonsters.drake_m00` | Crimson Drake, Drake, Exarion, Weird Drake, Marsh Drake, Zwov |
| `LineageMonsters.dwarf_ghost_m00` | Forest of Mirrors Ghost, Gatekeeper of Hell |
| `LineageMonsters.elf_ghost_m00` | Forest of Mirrors Ghost, Biggerstaff |
| `LineageMonsters.field_stalker_m00` | Deprive, Hallate's Watchman, Musveren, Eva's Watchman, Ossiud |
| `LineageMonsters.fiend_archer_m00` | Archer of Abyss, Fiend Archer, Bloody Sniper, Flame Drake |
| `LineageMonsters.gargoyle_m00` | Grave Guard, Manashen Gargoyle, Reinforced Gargoyle, Catacomb Gargoyle, Purgatory Gargoyle, Marsh Stalker |
| `LineageMonsters.headless_knight_m00` | Headless Knight, Dark Guard |
| `LineageMonsters.human_ghost_m00` | Ghost of the Tower, Soul of Ruins, Ghost Guardian, Ghost, Forest of Mirrors Ghost, Hephaeston |
| `LineageMonsters.hunter_gargoyle_m00` | Hunter Gargoyle, Dustwind Gargoyle, Ancient Gargoyle |
| `LineageMonsters.iron_golem_m00` | Reinforced Steel Golem, Iron Ore Golem, Perum |
| `LineageMonsters.ketra_orc_archer_a_m00` | Hames Orc Sniper, Taik Orc Archer, Fallen Orc Archer |
| `LineageMonsters.ketra_orc_m00` | Taik Orc Supply, Taik Orc Watchman, Hames Orc Scout, Hames Orc Foot Soldier, Taik Orc, Fallen Orc |
| `LineageMonsters.ketra_orc_shaman_m00` | Hames Orc Shaman, Taik Orc Shaman, Fallen Orc Shaman |
| `LineageMonsters.ketra_orc_warrior_a_m00` | Taik Orc Supply Officer , Hames Orc Prefect, Taik Orc Warrior, Taik Orc Captain, Fallen Orc Captain |
| `LineageMonsters.mana_vampire_m00` | Excuro, Carinkain |
| `LineageMonsters.medusa_m00` | Hatar Hanishee, Medusa, Hell Keeper Medusa, Ancient Holy Ground's Hermit, Shyslassys, Gorr |
| `LineageMonsters.mimic1_m00` | Treasure Chest |
| `LineageMonsters.oel_mahum_m00` | Oel Mahum, Inner Castle Gatekeeper, Outer Castle Gatekeeper, Herald |
| `LineageMonsters.raikel_m00` | Laikel |
| `LineageMonsters.riba_m00` | Riba, Riba Iren |
| `LineageMonsters.saber_toothed_tiger_m00` | Beast Lord, Sharp Talon Tiger, Fiend of Cold |
| `LineageMonsters.shadow_m00` | Catacomb Shadow, Purgatory Shadow, Shadow Turen |
| `LineageMonsters.skeleton_knight_m00` | Hallate's Warrior, Doom Warrior, Maluk Knight, Gilmore, Theodric, Decayed Ancient Soldier |
| `LineageMonsters.susceptor_m00` | Susceptor |
| `LineageMonsters.troll_m00` | Cave Beast, Formor, Formor Elder, Premo |
| `LineageMonsters.twin_headed_giant_m00` | Grandis, Cave Howler, Lakin |
| `LineageMonsters.unicorn_boxer_m00` | Aurora the Unicorn, Unicorn |
| `LineageMonsters.unicorn_m00` | Unicorn, Elder the Unicorn, Eva's Unicorn |
| `LineageMonsters.unicorn_mirage_m00` | Phantasm the Unicorn |
| `LineageMonsters.vale_master_m00` | Corrupt Sage, Magus Valac, Veil Master, Graveyard Sage, Icarus, Mausoleum Sage |
| `LineageMonsters.virud_lizardman_shaman_a_m00` | Tasaba Lizardman Shaman, Delu Lizardman Shaman, Maille Lizardman Shaman, Langk Lizardman Shaman |
| `LineageMonsters.will_o_wisp_m00` | Paliote, Tairim, Glow Wisp, Ghost Fire, Death Wave, Catacomb Wisp |
| `LineageMonsters.wyrm_m00` | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai, Dire Wyrm |
| `LineageMonsters.zenta_lizardman_m00` | Leto Lizardman, Leto Lizardman Soldier, Harit Lizardman, Harit Lizardman Soldier |
| `LineageMonsters.zenta_lizardman_matriarch_m00` | Leto Lizardman Overlord, Lord of the Plains, Harit Lizardman Matriarch |
| `LineageMonsters.zenta_lizardman_scout_m00` | Leto Lizardman Archer, Harit Lizardman Archer |
| `LineageMonsters.zenta_lizardman_shaman_m00` | Raido, Leto Lizardman Shaman, Harit Lizardman Shaman |
| `LineageMonsters.zenta_lizardman_warrior_m00` | Leto Lizardman Warrior, Harit Lizardman Warrior |
| `LineageMonsters2.apostle_warrior_m00` | Guardian of Forbidden Knowledge, Keeper of the Holy Edict, Watcher of the Ancient Pledge |
| `LineageMonsters5.ice_elemental_bomb_m00` | Spirit of Cold |
| `LineageMonsters5.ice_knight_m00` | Ice Monster |
| `LineageMonsters9.minotaur_fanatic_m00` | Swampland Watchman |
| `LineageNPCs.a_hardins_pupil_FHuman_m00` | Ritasha, Cema, Kalis, Cleo |
| `LineageNPCs.codex_stone_m00` | Tablet Of Vision |
| `LineageNPCs.coffer_b_m00` | Holy Ark of Secrecy 2, Box Of Titan |
| `LineageNPCs.coffer_c_m00` | Holy Ark of Secrecy 3 |
| `LineageNPCs.heart_of_warding_m00` | Heart of Warding |
| `LineageNPCs.human_tree_m00` | Fairy Tree of Wind, Fairy Tree of Star, Twilight Fairy Tree, Fairy Tree of Abyss, Tree |
| `LineageNPCs.yetis_dinner_table_m00` | Summer Feast |
| `LineageNPCs4.castle_warrior_m00` | Ghost of Solitude |

## aden — 50 new missing meshes

Spawn files: `aden.json`, `aden_blazing_swamp.json`, `aden_cemetery.json`, `aden_fields_of_massacre.json`, `aden_giants_cave.json`, `aden_plains_of_glory.json`, `aden_silent_valley.json`, `aden_the_forbidden_gateway.json`, `aden_tower_of_insolence.json`, `aden_war_torn_plains.json`, `castles_aden.json`

| mesh | names |
|------|-------|
| `LineageMonsters.banshee_m00` | Erin Ediunce, Bloody Banshee, Martyrium Priest, Mausoleum Inquisitor, Barrow Priest, Graveyard Inquisitor |
| `LineageMonsters.bloody_queen_m00` | Hallate's Maid, Limal Karinness, Pytan, Bloody Lady, Sepulcher Inquisitor, Crypt Priest |
| `LineageMonsters.blue_eye_m00` | Kaleidos the Unicorn |
| `LineageMonsters.cave_maiden_m00` | Palibati, Cave Maiden, Stenoa Gorgon Queen |
| `LineageMonsters.death_blader_m00` | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord, Bloody Guardian |
| `LineageMonsters.death_knight_m00` | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian, Wandering Spirit of Abyss |
| `LineageMonsters.devastator_m00` | Kadios, Graveyard Predator, Swamp Predator |
| `LineageMonsters.drake_m00` | Crimson Drake, Drake, Exarion, Weird Drake, Marsh Drake, Zwov |
| `LineageMonsters.field_stalker_m00` | Deprive, Hallate's Watchman, Musveren, Eva's Watchman, Ossiud |
| `LineageMonsters.fiend_archer_m00` | Archer of Abyss, Fiend Archer, Bloody Sniper, Flame Drake |
| `LineageMonsters.gamlin_m00` | Gamlin |
| `LineageMonsters.gargoyle_m00` | Grave Guard, Manashen Gargoyle, Reinforced Gargoyle, Catacomb Gargoyle, Purgatory Gargoyle, Marsh Stalker |
| `LineageMonsters.heltor_silenos_m00` | Vanor Silenos, Vanor Silenos Soldier, Vanor Silenos Scout |
| `LineageMonsters.heltor_silenos_shaman_m00` | Vanor Silenos Shaman |
| `LineageMonsters.human_ghost_m00` | Ghost of the Tower, Soul of Ruins, Ghost Guardian, Ghost, Forest of Mirrors Ghost, Hephaeston |
| `LineageMonsters.kaim_vanul_m00` | Hallate's Guardian, Kaim Vanul |
| `LineageMonsters.karik_m00` | Hallate's Inspector, Karik, Bloody Lord |
| `LineageMonsters.kel_mahum_archer_m00` | Platinum Tribe Archer, Platinum Tribe Guardian Archer |
| `LineageMonsters.kel_mahum_champion_m00` | Platinum Tribe Prefect, Binder, Platinum Tribe Guardian Prefect |
| `LineageMonsters.kel_mahum_m00` | Platinum Tribe Soldier |
| `LineageMonsters.kel_mahum_warrior_m00` | Platinum Tribe Warrior, Platinum Tribe Guardian Warrior |
| `LineageMonsters.kel_mahum_witch_doctor_m00` | Platinum Tribe Shaman, Platinum Tribe Guardian Shaman, Platinum Tribe Guardian Chief |
| `LineageMonsters.ketra_orc_archer_a_m00` | Hames Orc Sniper, Taik Orc Archer, Fallen Orc Archer |
| `LineageMonsters.ketra_orc_m00` | Taik Orc Supply, Taik Orc Watchman, Hames Orc Scout, Hames Orc Foot Soldier, Taik Orc, Fallen Orc |
| `LineageMonsters.ketra_orc_shaman_m00` | Hames Orc Shaman, Taik Orc Shaman, Fallen Orc Shaman |
| `LineageMonsters.ketra_orc_warrior_a_m00` | Taik Orc Supply Officer , Hames Orc Prefect, Taik Orc Warrior, Taik Orc Captain, Fallen Orc Captain |
| `LineageMonsters.leogul_m00` | Leogul, Mutated Creation |
| `LineageMonsters.lesser_giant_m00` | Lesser Giant Soldier, Lesser Giant Warrior |
| `LineageMonsters.lesser_giant_mage_m00` | Lesser Giant Wizard, Lesser Giant Elder, Forgotten Ancient People |
| `LineageMonsters.lesser_giant_shooter_m00` | Giant's Shadow |
| `LineageMonsters.liviona_m00` | Crendion, Graveyard Lich, Zaken's Watchman, Bloody Liviona, Wandering Ghost of the Dungeon, Catacomb Liviona |
| `LineageMonsters.medusa_m00` | Hatar Hanishee, Medusa, Hell Keeper Medusa, Ancient Holy Ground's Hermit, Shyslassys, Gorr |
| `LineageMonsters.saber_toothed_tiger_m00` | Beast Lord, Sharp Talon Tiger, Fiend of Cold |
| `LineageMonsters.serpent_slave_m00` | Trives, Cave Keeper, Catacomb Serpent, Purgatory Serpent |
| `LineageMonsters.skeleton_knight_m00` | Hallate's Warrior, Doom Warrior, Maluk Knight, Gilmore, Theodric, Decayed Ancient Soldier |
| `LineageMonsters.skeleton_pikeman_m00` | Doom Trooper, Zaken's Pikeman, Decayed Ancient Pikeman |
| `LineageMonsters.skeleton_royal_guard_m00` | Hallate's Royal Guard, Spiteful Soul Leader, Doom Scout, Royal Cave Servant, Maluk Soldier, Zaken's Royal Guard |
| `LineageMonsters.unpleasant_humming_m00` | Ghastly Warrior, Soldier of Grief, Dismal Humming |
| `LineageMonsters.vale_master_m00` | Corrupt Sage, Magus Valac, Veil Master, Graveyard Sage, Icarus, Mausoleum Sage |
| `LineageMonsters.will_o_wisp_m00` | Paliote, Tairim, Glow Wisp, Ghost Fire, Death Wave, Catacomb Wisp |
| `LineageMonsters.wyrm_m00` | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai, Dire Wyrm |
| `LineageMonsters14.ancient_giant_bowgun_m00` | Lesser Giant Shooter |
| `LineageMonsters14.ancient_giant_warrior_m00` | Barif |
| `LineageMonsters2.bandersnatch_m00` | Halingka |
| `LineageMonsters2.brain_buster_m00` | Yintzu |
| `LineageNPCs.a_maidA_FHuman_m00` | Dianne, Adrienne, Gladys, Serena |
| `LineageNPCs.coffer_b_m00` | Holy Ark of Secrecy 2, Box Of Titan |
| `LineageNPCs2.madam_claudia_m00` | Claudia Athebaldt |
| `LineageNPCs4.a_traderB_Mhuman_pet_m00` | Lemper, Cooper |
| `LineageNpcsEV.n_event_stakato_egg_m00` | ? |

## giran — 47 new missing meshes

Spawn files: `castles_giran.json`, `giran.json`, `giran_antharas_lair.json`, `giran_brekas_stronghold.json`, `giran_death_pass.json`, `giran_devils_isle.json`, `giran_dragon_valley.json`, `giran_dragon_valley_monsters.json`, `giran_gorgon_flower_garden.json`, `giran_hardins_academy.json`

| mesh | names |
|------|-------|
| `LineageMonsters.banshee_m00` | Erin Ediunce, Bloody Banshee, Martyrium Priest, Mausoleum Inquisitor, Barrow Priest, Graveyard Inquisitor |
| `LineageMonsters.batur_orc_archer_m00` | Breka Orc Archer, Timak Orc Archer, Tamlin Orc Archer |
| `LineageMonsters.batur_orc_overlord_m00` | Breka Orc Prefect, Voltar, Timak Orc Prefect, Timak Orc Troop Captain |
| `LineageMonsters.batur_orc_shaman_a_m00` | Breka Orc Shaman, Timak Orc Shaman, Driko |
| `LineageMonsters.batur_orc_warrior_m00` | Breka Orc Warrior, Timak Orc Warrior |
| `LineageMonsters.binding_m00` | Fettered Soul, Martyrium Watchman, Martyrium Monk, Barrow Monk |
| `LineageMonsters.bloody_queen_m00` | Hallate's Maid, Limal Karinness, Pytan, Bloody Lady, Sepulcher Inquisitor, Crypt Priest |
| `LineageMonsters.cave_maiden_m00` | Palibati, Cave Maiden, Stenoa Gorgon Queen |
| `LineageMonsters.crimson_doll_m00` | Hell Keeper Crimson Doll, Wandering Spirit of the Dungeon |
| `LineageMonsters.death_blader_m00` | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord, Bloody Guardian |
| `LineageMonsters.death_knight_m00` | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian, Wandering Spirit of Abyss |
| `LineageMonsters.doll_blader_m00` | Maiden Doll, Sairon, Wandering Dead of the Dungeon |
| `LineageMonsters.doll_master_m00` | Netherworld Prophet, Doll Master, Garden Guard Captain, Wandering Evil Thoughts of the Dungeon, Sepulcher Sage, Sepulcher Prophet |
| `LineageMonsters.drake_m00` | Crimson Drake, Drake, Exarion, Weird Drake, Marsh Drake, Zwov |
| `LineageMonsters.field_stalker_m00` | Deprive, Hallate's Watchman, Musveren, Eva's Watchman, Ossiud |
| `LineageMonsters.fiend_archer_m00` | Archer of Abyss, Fiend Archer, Bloody Sniper, Flame Drake |
| `LineageMonsters.harpy_m00` | Harpy |
| `LineageMonsters.headless_knight_m00` | Headless Knight, Dark Guard |
| `LineageMonsters.hunter_gargoyle_m00` | Hunter Gargoyle, Dustwind Gargoyle, Ancient Gargoyle |
| `LineageMonsters.kaim_vanul_m00` | Hallate's Guardian, Kaim Vanul |
| `LineageMonsters.karik_m00` | Hallate's Inspector, Karik, Bloody Lord |
| `LineageMonsters.light_bringer_m00` | Light Bringer |
| `LineageMonsters.liviona_m00` | Crendion, Graveyard Lich, Zaken's Watchman, Bloody Liviona, Wandering Ghost of the Dungeon, Catacomb Liviona |
| `LineageMonsters.medusa_m00` | Hatar Hanishee, Medusa, Hell Keeper Medusa, Ancient Holy Ground's Hermit, Shyslassys, Gorr |
| `LineageMonsters.pirates_zombie_captain_m00` | Pirate Zombie Captain |
| `LineageMonsters.pirates_zombie_m00` | Pirate Zombie |
| `LineageMonsters.serpent_slave_m00` | Trives, Cave Keeper, Catacomb Serpent, Purgatory Serpent |
| `LineageMonsters.shadeless_m00` | Ancient Holy Ground's Vision |
| `LineageMonsters.silhouette_m00` | Silhouette Tilfo, Ancient Holy Ground's Wing |
| `LineageMonsters.skeleton_knight_m00` | Hallate's Warrior, Doom Warrior, Maluk Knight, Gilmore, Theodric, Decayed Ancient Soldier |
| `LineageMonsters.skeleton_pikeman_m00` | Doom Trooper, Zaken's Pikeman, Decayed Ancient Pikeman |
| `LineageMonsters.skeleton_royal_guard_m00` | Hallate's Royal Guard, Spiteful Soul Leader, Doom Scout, Royal Cave Servant, Maluk Soldier, Zaken's Royal Guard |
| `LineageMonsters.troll_m00` | Cave Beast, Formor, Formor Elder, Premo |
| `LineageMonsters.twin_headed_giant_m00` | Grandis, Cave Howler, Lakin |
| `LineageMonsters.unpleasant_humming_m00` | Ghastly Warrior, Soldier of Grief, Dismal Humming |
| `LineageMonsters.vale_master_m00` | Corrupt Sage, Magus Valac, Veil Master, Graveyard Sage, Icarus, Mausoleum Sage |
| `LineageMonsters.will_o_wisp_m00` | Paliote, Tairim, Glow Wisp, Ghost Fire, Death Wave, Catacomb Wisp |
| `LineageMonsters.wyrm_m00` | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai, Dire Wyrm |
| `LineageMonsters2.nephilim_priest_m00` | Nephilim Cardinal, Nephilim Archbishop, Nephilim Priest, Nephilim Bishop |
| `LineageMonsters2.nephilim_slayer_m00` | Nephilim Guard, Nephilim Scout, Nephilim Sentinel, Nephilim Royal Guard |
| `LineageMonsters2.vampire_soldier_m00` | Mordred |
| `LineageNPCs.a_hardin_MHuman_m00` | Hardin |
| `LineageNPCs.a_hardins_pupil_FHuman_m00` | Ritasha, Cema, Kalis, Cleo |
| `LineageNPCs.a_hardins_pupil_MHuman_m00` | Orim, Kaspar, Pilgrim of Darkness |
| `LineageNPCs.a_maidA_FHuman_m00` | Dianne, Adrienne, Gladys, Serena |
| `LineageNPCs4.a_traderB_Mhuman_pet_m00` | Lemper, Cooper |
| `LineageNpcsEV.n_event_stakato_egg_m00` | ? |

## catacombs — 34 new missing meshes

Spawn files: `catacombs_catacomb_of_dark_omens.json`, `catacombs_catacomb_of_the_apostate.json`, `catacombs_catacomb_of_the_branded.json`, `catacombs_catacomb_of_the_forbidden_path.json`, `catacombs_catacomb_of_the_heretic.json`, `catacombs_catacomb_of_the_witch.json`, `catacombs_necropolis_of_devotion.json`, `catacombs_necropolis_of_martyrdom.json`, `catacombs_necropolis_of_sacrifice.json`, `catacombs_necropolis_of_the_disciples.json`, `catacombs_necropolis_of_the_patriots.json`, `catacombs_necropolis_of_the_pilgrims.json`, `catacombs_necropolis_of_the_saints.json`, `catacombs_necropolis_of_worship.json`, `seven_signs_catacombs_npcs.json`, `seven_signs_necropolis_npcs.json`, `seven_signs_rift_npcs.json`

| mesh | names |
|------|-------|
| `LineageMonsters.banshee_m00` | Erin Ediunce, Bloody Banshee, Martyrium Priest, Mausoleum Inquisitor, Barrow Priest, Graveyard Inquisitor |
| `LineageMonsters.binding_m00` | Fettered Soul, Martyrium Watchman, Martyrium Monk, Barrow Monk |
| `LineageMonsters.blade_stakato_drone_m00` | Stakato Drone, Ancient Holy Ground's Totem |
| `LineageMonsters.blade_stakato_soldier_m00` | Stakato Warrior, Catacomb Stakato Soldier |
| `LineageMonsters.bloody_queen_m00` | Hallate's Maid, Limal Karinness, Pytan, Bloody Lady, Sepulcher Inquisitor, Crypt Priest |
| `LineageMonsters.crimson_doll_m00` | Hell Keeper Crimson Doll, Wandering Spirit of the Dungeon |
| `LineageMonsters.death_blader_m00` | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord, Bloody Guardian |
| `LineageMonsters.death_knight_m00` | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian, Wandering Spirit of Abyss |
| `LineageMonsters.death_lord_m00` | Ancient Holy Ground's Guardian Spirit |
| `LineageMonsters.doll_master_m00` | Netherworld Prophet, Doll Master, Garden Guard Captain, Wandering Evil Thoughts of the Dungeon, Sepulcher Sage, Sepulcher Prophet |
| `LineageMonsters.gargoyle_m00` | Grave Guard, Manashen Gargoyle, Reinforced Gargoyle, Catacomb Gargoyle, Purgatory Gargoyle, Marsh Stalker |
| `LineageMonsters.liviona_m00` | Crendion, Graveyard Lich, Zaken's Watchman, Bloody Liviona, Wandering Ghost of the Dungeon, Catacomb Liviona |
| `LineageMonsters.medusa_m00` | Hatar Hanishee, Medusa, Hell Keeper Medusa, Ancient Holy Ground's Hermit, Shyslassys, Gorr |
| `LineageMonsters.serpent_slave_m00` | Trives, Cave Keeper, Catacomb Serpent, Purgatory Serpent |
| `LineageMonsters.shadeless_m00` | Ancient Holy Ground's Vision |
| `LineageMonsters.shadow_m00` | Catacomb Shadow, Purgatory Shadow, Shadow Turen |
| `LineageMonsters.silhouette_m00` | Silhouette Tilfo, Ancient Holy Ground's Wing |
| `LineageMonsters.skeleton_knight_m00` | Hallate's Warrior, Doom Warrior, Maluk Knight, Gilmore, Theodric, Decayed Ancient Soldier |
| `LineageMonsters.skeleton_pikeman_m00` | Doom Trooper, Zaken's Pikeman, Decayed Ancient Pikeman |
| `LineageMonsters.skeleton_royal_guard_m00` | Hallate's Royal Guard, Spiteful Soul Leader, Doom Scout, Royal Cave Servant, Maluk Soldier, Zaken's Royal Guard |
| `LineageMonsters.vale_master_m00` | Corrupt Sage, Magus Valac, Veil Master, Graveyard Sage, Icarus, Mausoleum Sage |
| `LineageMonsters.will_o_wisp_m00` | Paliote, Tairim, Glow Wisp, Ghost Fire, Death Wave, Catacomb Wisp |
| `LineageMonsters2.lilim_assassin_m00` | Lilim Butcher, Lilim Marauder, Lilim Assassin, Lilim Slayer |
| `LineageMonsters2.lilim_knight_m00` | Lilim Knight-Errant, Lilim Knight, Lilim Knight Captain, Lilim Royal Knight |
| `LineageMonsters2.lilim_wizard_m00` | Lilim Magus, Lilim Priest, Lilim Soldier, Lilim Great Magus |
| `LineageMonsters2.nephilim_lord_m00` | Nephilim Swordsman, Nephilim Centurion, Nephilim Praetorian, Nephilim Commander |
| `LineageMonsters2.nephilim_priest_m00` | Nephilim Cardinal, Nephilim Archbishop, Nephilim Priest, Nephilim Bishop |
| `LineageMonsters2.nephilim_slayer_m00` | Nephilim Guard, Nephilim Scout, Nephilim Sentinel, Nephilim Royal Guard |
| `LineageMonsters2.temple_guard_a_m00` | Lith Scout, Lith Guard, Lith Patrolman |
| `LineageMonsters2.temple_guard_b_m00` | Lith Warrior, Lith Prefect, Lith Commander |
| `LineageMonsters2.temple_savant_a_m00` | Lith Witch, Lith Medium, Lith Shaman |
| `LineageMonsters2.tomb_guard_a_m00` | Gigant Slave, Gigant Footman, Gigant Raider |
| `LineageMonsters2.tomb_guard_b_m00` | Gigant Overseer, Gigant Officer |
| `LineageMonsters2.tomb_savant_a_m00` | Gigant Acolyte, Gigant Cleric, Gigant Confessor |

## heine — 28 new missing meshes

Spawn files: `castles_innadril.json`, `heine.json`, `innadril_alligator_beach.json`, `innadril_alligator_island.json`, `innadril_field_of_silence.json`, `innadril_field_of_whispers.json`, `innadril_garden_of_eva.json`

| mesh | names |
|------|-------|
| `LineageMonsters.blade_stakato_drone_m00` | Stakato Drone, Ancient Holy Ground's Totem |
| `LineageMonsters.blade_stakato_m00` | Stakato |
| `LineageMonsters.blade_stakato_soldier_m00` | Stakato Warrior, Catacomb Stakato Soldier |
| `LineageMonsters.blade_stakato_worker_m00` | Stakato Worker |
| `LineageMonsters.crimson_doll_m00` | Hell Keeper Crimson Doll, Wandering Spirit of the Dungeon |
| `LineageMonsters.crocodile_m00` | Alligator |
| `LineageMonsters.crokian_m00` | Crokian, Crokian Warrior, Crokian Lad, Crokian Lad Warrior |
| `LineageMonsters.dailaon_m00` | Dailaon, Dailaon Lad |
| `LineageMonsters.death_blader_m00` | Doom Knight, Hallate's Commander, Cursed Guardian, Imperial Gravekeeper, Maluk Lord, Bloody Guardian |
| `LineageMonsters.death_knight_m00` | Hallate's Knight, Slaughter Bathin, Maluk Berserker, Bloody Knight, Sepulcher Royal Guardian, Wandering Spirit of Abyss |
| `LineageMonsters.doll_blader_m00` | Maiden Doll, Sairon, Wandering Dead of the Dungeon |
| `LineageMonsters.doll_master_m00` | Netherworld Prophet, Doll Master, Garden Guard Captain, Wandering Evil Thoughts of the Dungeon, Sepulcher Sage, Sepulcher Prophet |
| `LineageMonsters.farhite_m00` | Farhite, Farhite Lad, Swamp Tribe |
| `LineageMonsters.field_stalker_m00` | Deprive, Hallate's Watchman, Musveren, Eva's Watchman, Ossiud |
| `LineageMonsters.great_white_shark_m00` | Giant White Shark |
| `LineageMonsters.lienrik_m00` | Lienrik, Lienrik Lad |
| `LineageMonsters.light_worm_m00` | Light Worm |
| `LineageMonsters.liviona_m00` | Crendion, Graveyard Lich, Zaken's Watchman, Bloody Liviona, Wandering Ghost of the Dungeon, Catacomb Liviona |
| `LineageMonsters.nos_m00` | Nos, Nos Lad |
| `LineageMonsters.ogre_m00` | Ogre |
| `LineageMonsters.rakul_m00` | Rakul |
| `LineageMonsters.theeder_m00` | Piker, Mage |
| `LineageMonsters.virud_lizardman_shaman_a_m00` | Tasaba Lizardman Shaman, Delu Lizardman Shaman, Maille Lizardman Shaman, Langk Lizardman Shaman |
| `LineageMonsters.water_giant_m00` | Water Giant |
| `LineageMonsters5.scapegoat_m00` | Condemned of Infinity |
| `LineageNPCs.e_mageguild_teacher_FOrc_m00` | Takina |
| `LineageNPCs.pot_m00` | Medium Jar |
| `LineageNPCs2.iason_haine_m00` | Iason Heine |

## oren — 20 new missing meshes

Spawn files: `castles_oren.json`, `oren.json`, `oren_ivory_tower.json`, `oren_ivory_tower_npcs.json`, `oren_outlaw_forest.json`, `oren_plains_of_the_lizardmen.json`, `oren_sea_of_spores.json`, `oren_skyshadow_meadow.json`, `oren_timak_outpost.json`

| mesh | names |
|------|-------|
| `LineageMonsters.batur_orc_archer_m00` | Breka Orc Archer, Timak Orc Archer, Tamlin Orc Archer |
| `LineageMonsters.batur_orc_overlord_m00` | Breka Orc Prefect, Voltar, Timak Orc Prefect, Timak Orc Troop Captain |
| `LineageMonsters.batur_orc_shaman_a_m00` | Breka Orc Shaman, Timak Orc Shaman, Driko |
| `LineageMonsters.batur_orc_warrior_m00` | Breka Orc Warrior, Timak Orc Warrior |
| `LineageMonsters.drake_m00` | Crimson Drake, Drake, Exarion, Weird Drake, Marsh Drake, Zwov |
| `LineageMonsters.gargoyle_m00` | Grave Guard, Manashen Gargoyle, Reinforced Gargoyle, Catacomb Gargoyle, Purgatory Gargoyle, Marsh Stalker |
| `LineageMonsters.hunter_gargoyle_m00` | Hunter Gargoyle, Dustwind Gargoyle, Ancient Gargoyle |
| `LineageMonsters.iron_golem_m00` | Reinforced Steel Golem, Iron Ore Golem, Perum |
| `LineageMonsters.king_bugbear_m00` | Tarlk Bugbear Captain, Innersen |
| `LineageMonsters.oel_mahum_m00` | Oel Mahum, Inner Castle Gatekeeper, Outer Castle Gatekeeper, Herald |
| `LineageMonsters.oel_mahum_warrior_m00` | Oel Mahum Warrior |
| `LineageMonsters.oel_mahum_witch_doctor_m00` | Oel Mahum Shaman |
| `LineageMonsters.troll_m00` | Cave Beast, Formor, Formor Elder, Premo |
| `LineageMonsters.wyrm_m00` | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai, Dire Wyrm |
| `LineageMonsters.zenta_lizardman_m00` | Leto Lizardman, Leto Lizardman Soldier, Harit Lizardman, Harit Lizardman Soldier |
| `LineageMonsters.zenta_lizardman_matriarch_m00` | Leto Lizardman Overlord, Lord of the Plains, Harit Lizardman Matriarch |
| `LineageMonsters.zenta_lizardman_scout_m00` | Leto Lizardman Archer, Harit Lizardman Archer |
| `LineageMonsters.zenta_lizardman_shaman_m00` | Raido, Leto Lizardman Shaman, Harit Lizardman Shaman |
| `LineageMonsters.zenta_lizardman_warrior_m00` | Leto Lizardman Warrior, Harit Lizardman Warrior |
| `LineageNPCs2.gustaf_athebaldt_m00` | Sir Gustav Athebaldt |

## goddard — 12 new missing meshes

Spawn files: `castles_goddard.json`, `goddard.json`, `goddard_forge_of_gods.json`, `goddard_four_sepulchers_managers.json`, `goddard_garden_of_beasts.json`, `goddard_hot_springs.json`, `goddard_imperial_tomb.json`, `goddard_ketra_orc_outpost.json`, `goddard_monastery_of_silence.json`, `goddard_varka_slenos_outpost.json`, `goddard_wall_of_argos.json`

| mesh | names |
|------|-------|
| `LineageMonsters.unicorn_boxer_m00` | Aurora the Unicorn, Unicorn |
| `LineageMonsters.wyrm_m00` | Lava Wyrm, Wyrm, Thunder Wyrm, Shamhai, Dire Wyrm |
| `LineageMonsters.zenta_lizardman_shaman_m00` | Raido, Leto Lizardman Shaman, Harit Lizardman Shaman |
| `LineageMonsters2.ifrit_m00` | Fiery Ifrit |
| `LineageNPCs.a_common_peopleC_Mhuman_m00` | Hadley, Rowell, Gompus, Gallic, Kendrew |
| `LineageNPCs.a_hardins_pupil_MHuman_m00` | Orim, Kaspar, Pilgrim of Darkness |
| `LineageNPCs.a_traderC_Fhuman_m00` | Branhillde |
| `LineageNPCs.codex_stone_m00` | Tablet Of Vision |
| `LineageNPCs.e_elmore_guard_Mhuman_m00` | Mass Gatekeeper, Outer Castle Gatekeeper, Inner Castle Gatekeeper, Gatekeeper |
| `LineageNPCs.godad_holything_m00` | Altar of Fire, Altar of Water, Altar of Earth |
| `LineageNPCs.yeti_cute_m00` | Green |
| `LineageNPCs2.agit_maid_c_m00` | Aida, Neria |

## rune — 5 new missing meshes

Spawn files: `castles_rune.json`, `rune.json`, `rune_beast_farm.json`, `rune_chapel_guards.json`, `rune_fortress_of_the_dead.json`, `rune_primeval_isle.json`, `rune_stakato_nest.json`, `rune_swamp_of_screams.json`, `rune_valley_of_saints.json`, `rune_wild_beast_reserve.json`

| mesh | names |
|------|-------|
| `LineageMonsters2.apostle_warrior_m00` | Guardian of Forbidden Knowledge, Keeper of the Holy Edict, Watcher of the Ancient Pledge |
| `LineageNPCs.a_common_peopleC_Mhuman_m00` | Hadley, Rowell, Gompus, Gallic, Kendrew |
| `LineageNPCs.codex_stone_m00` | Tablet Of Vision |
| `LineageNPCs.e_elmore_guard_Mhuman_m00` | Mass Gatekeeper, Outer Castle Gatekeeper, Inner Castle Gatekeeper, Gatekeeper |
| `LineageNPCsEV.rune_holything_m00` | Divine Altar |

## schuttgart — 5 new missing meshes

Spawn files: `castles_schuttgart.json`, `schuttgart.json`, `schuttgart_crypts_of_discrace.json`, `schuttgart_den_of_evil.json`, `schuttgart_frozen_labyrinth.json`, `schuttgart_pavel_archaic.json`

| mesh | names |
|------|-------|
| `LineageNPCs.a_common_peopleC_Mhuman_m00` | Hadley, Rowell, Gompus, Gallic, Kendrew |
| `LineageNPCs.a_maidA_FHuman_m00` | Dianne, Adrienne, Gladys, Serena |
| `LineageNPCs.e_elmore_guard_Mhuman_m00` | Mass Gatekeeper, Outer Castle Gatekeeper, Inner Castle Gatekeeper, Gatekeeper |
| `LineageNPCs.godad_holything_m00` | Altar of Fire, Altar of Water, Altar of Earth |
| `LineageNPCs2.agit_maid_c_m00` | Aida, Neria |

## hunters — 2 new missing meshes

Spawn files: `hunters.json`, `hunters_enchanted_valley.json`

| mesh | names |
|------|-------|
| `LineageMonsters.pan_m00` | Satyr, Satyr Elder |
| `LineageMonsters.unicorn_m00` | Unicorn, Elder the Unicorn, Eva's Unicorn |

## dwarven_mines — 1 new missing meshes

Spawn files: `dwarven_mithril_mines.json`

| mesh | names |
|------|-------|
| `LineageMonsters.iron_golem_m00` | Reinforced Steel Golem, Iron Ore Golem, Perum |

