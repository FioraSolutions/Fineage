# Special-case branches & silent fallbacks audit

**Scope:** `l2-unity/Assets/Scripts` (Unity Interlude client). **Method:** `rg` sweep for `WeaponType.*`, `CharacterRace*`, class/race/sex gates, `SkillId` / `npcId` literals, `"bow"` anim hacks, `??` / `catch { }` / `Fallback` / default clip lengths. **Date:** 2026-11-20.

**Architecture target (owner rules):**

1. One shared mechanism per pipeline concern; per-race/weapon/skeleton differences come from data profiles resolved at load/spawn (resolver/factory), not scattered `if (type == pole)` in gameplay code. Central parsers (`WeaponClassifier`, `WeaponTypeParser`, `CharacterRaceAnimationResolve`, `MapClassId` data tables) are allowed **once** at the boundary — violations are **downstream** branches that should read profile fields instead.
2. Missing required data must log via project logger (`FineageLog` / `EquipmentMeshLog` / etc.) and leave a visible failure — not swap in Human bodies, `1f` clip lengths, potion meshes, or empty tables.

**Parallel work:** `WeaponAttachProfile` refactor is **in progress** (separate agent; not merged in this tree yet). All weapon-attach / bone-slot / mirror / equip-refresh rows below are tagged **WIP**.

---

## Ordered batch plan (reviewable PRs)

| Batch | Theme | Player-visible impact | Primary files / profiles |
|------:|-------|----------------------|---------------------------|
| **1** | Weapon attach + dual `wp_mesh` | Wrong/missing off-hand mesh, fist mirror, bow on wrong bone | `WeaponAttachProfile` **(WIP)**, `WeapongrpTable.Lookup`, `AbstractMeshManager`, `Gear.*` |
| **2** | Combat style from suffix, not `WeaponType` / `"bow"` string | Bow facing, hit timing, arrow nock, melee vs ranged hits | `CombatStyleProfile` / cached `AnimSuffix` on `Gear` |
| **3** | Locomotion state names (`wait_` + suffix) | Run/walk/atk wrong weapon stance for remotes | `LocomotionProfile`, entity `AnimSuffix` |
| **4** | Meta table bootstrap | Silent empty DAT → wrong stats, icons, weapons everywhere | All `*Table.Instance` `catch { }` |
| **5** | Spawn / identity apply | Wrong race body, HP/speed, mage flag | `NetSession.ZoneApply`, `MapClassId.GetCharacterRace` |
| **6** | Animation duration fallbacks | Desynced P.Atk / skill cast / pickup | `AnimationManager.MonsterHelpers*`, `AnimPlayback`, `SetupDurationHelper` |
| **7** | NPC / pet mesh substitution | Wrong NPC appearance, debug strider ids | `EntitySpawnShared.Fallback*`, `PetVisuals` |
| **8** | Skill presentation by id | Wind Strike / single-skill hacks | `SkillPresentationProfile` rows (4247, 2046, …) |
| **9** | Drops, shop grouping, enchant glow ids | Wrong ground mesh, shop tabs | `ItemDropGrpCatalog`, `ItemDropPrefabLoader`, naked item ids |
| **10** | UI / audio / icon fallbacks | `??` class label, missing cast voice | `CharSelectWindow`, `SkillCastSound`, `IconManager` |

---

## A) Special-case branches

Columns: **Location** · **Branches on** · **Pipeline** · **Fold into (proposed field)** · **Effort** · **Notes**

Effort: **S** = thin profile read; **M** = new profile slice + DAT; **L** = table-driven art fallbacks.

### Weapon attach / gear mesh **(many WIP → WeaponAttachProfile)**

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Combat/Gear/Gear.Equip.Core.cs:48–50,102–106,203–206` | `pole`, `staff`, `bigword`, `bow` | Equip → anim refresh | `WeaponAttachProfile.triggersEquipAnimRefresh` | S | **WIP** |
| `Combat/Gear/Gear.Equip.Core.cs:66` | `WeaponType.bow` → force `leftSlot` | Weapon attach | `WeaponAttachProfile.equipUsesLeftHand` | S | **WIP** |
| `Combat/Gear/Gear.Equip.Core.cs:142` | `WeaponType.arrow` | Arrow mesh | `WeaponAttachProfile.etcArrowSlot` | S | **WIP** |
| `Combat/Gear/Gear.Equip.Helpers.cs:39–42` | `WeaponType` → `GetWeaponAnim` / `WeaponRange` | Locomotion suffix | `Weapongrp.AnimSuffix`, `AttackRange` at load | M | Parser OK at boundary |
| `Combat/Gear/Gear.EquipApply.Core.cs:21–25` | Hard-coded `"Bow Bone"` | Attach bone | `SkeletonAttachProfile.boneNames[]` | S | **WIP** |
| `Combat/Gear/Manager/AbstractMeshManager.Mesh.Core.cs:17–30` | `none`/`shield`/`bow`/`arrow`/L/R | Parent bone index | `WeaponAttachProfile.boneSlot` enum | M | **WIP** |
| `Combat/Gear/Manager/AbstractMeshManager.Mesh.Core.cs:43–55,58–60` | `fist`/`dualfist`, `shield` | Mirror X, double-sided | `WeaponAttachProfile.mirrorLeft`, `forceDoubleSided` | M | **WIP**; see `Docs/FIST_WEAPON_MOUNT_INVESTIGATION.md` |
| `Combat/Gear/Gear.Sync.Core.cs:23–27,37–66` | `dual`, `dualfist`, `fist`, `bow`, `shield` | Hand layout sync | `WeaponAttachProfile.handLayout` | M | **WIP** |
| `Combat/Gear/Gear.Sync.Core.cs:85–91` | 2H types incl. pole/staff/bow | Two-hand detection | `BodyPart` + `occupiesBothHands` | S | **WIP** |
| `Combat/Gear/Gear.SyncQuery.Helpers.cs:51–54` | bigword/pole/staff/bigblunt | 2H query | `isTwoHandVisual` | S | **WIP** |
| `Combat/Gear/Gear.SyncUnequip.Core.cs:57–60` | `bow` vs `hand` on unequip | Unequip anim | `AnimSuffix` from prior state | S | **WIP** |
| `Database/WeapongrpTable.Lookup.Helpers.cs:81–85` | `wp_mesh[0]` only | Mesh import | `Weapongrp.MeshParts[]` (dual) | M | Data gap for dual swords |
| `Game/Entity/Entity.Lifecycle.cs:20–24` | `WeaponType.bow` | FX attach point | `WeaponAttachProfile.chargeBone` | S | **WIP** |
| `Combat/Gear/EquipGear.Apply.Core.cs:20` | `GearSlotByIsBow` | Paperdoll slots | `handLayout` | S | **WIP** |
| `Combat/Gear/EquipGear.Apply.Helpers.cs:8–14` | `IsBow()` | L/R paperdoll | same | S | **WIP** |
| `Game/Entity/Entity.Gear.Core.cs:59–60` | `IsBow()`, `IsDual()` | Unequip hand pick | `handLayout` | S | **WIP** |

### Locomotion / movement

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Animation/User/UserStateBase.cs:34–35` | `Gear.WeaponType`, `WeaponAnim` | Remote user locomotion | Cached `AnimSuffix` on entity | S | |
| `Animation/User/UserStateWait.cs` (+ Run/Jump/Stand/Atk*) | `"wait_" + _weaponAnim` | Animator bool params | `AnimSuffix` only | M | Many sibling states |
| `Game/Character/SelectableCharacterEntity.Core.cs:61–69` | Fighter/magic race subset | Lobby walk speed | `LocomotionProfile.lobbyWalkMultiplier` | S | |
| `Fineage.Game/Anim/CharacterAnimPaths.cs:12–35` | Per `CharacterRaceAnimation` | Clip root path | Keep single resolver | S | OK if sole resolver |
| `Game/Character/SelectableCharacterEntity.Helpers.cs:12,43` | `RaceId` folders | Clip load path | `AnimationRootProfile` | S | |
| `Service/CombatFacingService.Tick.Helpers.cs:26–27,46–47,57–58` | `"bow"` in name / `WeaponType.bow` | Combat facing | `CombatStyleProfile.isRanged` | S | |
| `Controller/EntityAction/L2PawnRange.Core.cs:15–19` | `WeaponAnim == "bow"` | CharInfo snap skip | `CombatStyleProfile.isRanged` | S | |
| `Game/Entity/Entity.Gear.Core.cs:31` | anim name contains `"bow"` | `IsUseBow()` | `Gear.AnimSuffix` flag | S | |
| `Game/Manager/MonsterManager/Move/Model/MovementData.*` | `walk_`/`run_` + `WeaponAnim` | Monster gait | `CreatureLocomotionProfile.suffix` | M | |
| `Networking/Protocol/MagicGaitTrace.Capture.cs:15–16` | `MMagic`/`FMagic` | Debug trace | Optional appearance flag | S | Dev/diag |

### Animation / combat visuals

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Controller/EntityAction/EntityActionVisual.CombatFx.Core.cs:33` | `trigger + WeaponAnim` | Skill state name | `AnimSuffix` on cast row | S | |
| `Controller/EntityAction/EntityActionVisual.CombatHit.Helpers.cs:18,77` | `WeaponAnim` suffix | Hit states | Profile suffix | S | |
| `Controller/EntityAction/CreatureWeaponAnimApply.cs:58–68` | `Gear.WeaponAnim` else type→parser | NPC weapon clips | `NpcWeaponProfile.animSuffix` | M | |
| `Controller/StateMachine/State/Events/BowArrowVisual.Helpers.cs:8–14` | `"bow"` / `WeaponAnim` | Arrow visual | `CombatStyleProfile.isBow` | S | |
| `Controller/StateMachine/State/Events/AbstractAttackEvents.Callbacks.Helpers.cs:168` | `"bow"` in anim | Melee vs bow callback | `CombatStyleProfile` | S | |
| `Controller/StateMachine/State/Helpers/AttackTimingHelper.Core.cs:65` | lacks `"bow"` | Timing branch | `isRanged` | S | |
| `Controller/StateMachine/State/Helpers/AttackTimingHelper.Helpers.cs:70` | `"bow"` in anim | Hit fraction 0.82 | `WeaponTimingProfile.hitFraction` | S | |
| `Animation/Player/PlayerStateJAtk.*` | `"dual"` in name / `IsDual()` | Dual jatk timing | `AnimSetProfile.isDual` | S | |
| `Database/WeaponShotTable.Helpers.cs:53–74` | Many `WeaponType` pairs | Soul/spirit shot section | `WeaponShotProfile.sectionKey` | M | |
| `Game/Manager/HitManager/HitManager.Remote*.cs` | bow checks | Remote hit path | `CombatStyleProfile` | S | |
| `Controller/EntityAction/EntityActionSkill.*` | `SkillId == 4247` | Wind Strike | `SkillPresentationProfile` | M | |
| `Controller/StateMachine/State/NewPhysicalSkillsState.Helpers.cs:46` | `SkillId == 2046` | Physical skill | Skill profile row | M | |
| `Controller/EntityAction/EntityActionMachine.cs:61` | `SkillId == 4247` | Action gate | Skill profile | M | |
| `Game/WorldApply/MonsterWorldApply.Helpers.cs:15` | `SkillId == 4247` | Monster FX apply | Skill profile | M | |

### Combat / items / drops

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Game/Item/AbstractServerItem.Ops.Core.cs:60,67` | `shield`, `bow` via grp | Inventory semantics | `Weapongrp` capability flags | S | |
| `Game/Item/AbstractServerItem.Ops.Helpers.cs:5,9–10` | arrow/dual/fist types | Slot rules | Etc/weapon flags | S | |
| `Game/Item/DropVisual/ItemDropGrpCatalog.Helpers.cs:15–27` | `WeaponType` switch | Drop stick mesh | `DropVisualProfile.isStickWeapon` | S | |
| `Game/Item/DropVisual/ItemDropGrpCatalog.Helpers.cs:47–49` | Herb id ranges / name `"Herb"` | Drop classification | Item grp / `drop_model` | M | |
| `Combat/Gear/UserGear.EnchantGlow.cs:15–16` | Naked **itemId** constants | Enchant glow off | Item metadata flag | S | |
| `Combat/Gear/ArmorVisualResolver.Helpers.cs:17–18` | `CharacterRaceAnimation` | Armor mesh pick | Armor row keyed by `RaceId` | S | Importer-adjacent |
| `UI/Game/ToolTip/*Helpers.cs:34,44` | `WeaponType.shield` | Tooltip label | Display kind from grp | S | |

### Sound

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Audio/SkillCastSound.Voice.Core.cs:58–74` | Each `CharacterRaceAnimation` | Cast voice prefix | `VoiceProfile.prefix` (logongrp) | M | |
| `Audio/SkillCastSound.Voice.Helpers.cs:19–20,34–39,59` | Race/orc/dwarf, `"bow"`/`"spear"` in skill name | Voice selection | `SkillPresentationProfile.voiceCategory` | M | |
| `Audio/SkillCastSound.cs:15–30` | Race anim literals | Prewarm list | Generate from dat | S | |
| `Animation/User/UserStateAtk.cs:24` | `Atk_1H` only | Remote atk SFX | `WeaponSoundProfile.swingEvent` | M | |

### NPC rendering / spawn

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Game/Spawn/EntitySpawnShared.FallbackSetup.cs:9–18` | Race **strings** in mesh names | NPC fallback rules | `NpcArtFallbackTable` | L | |
| `Game/Spawn/EntitySpawnShared.FallbackCreate.Core.cs:26–70` | Race/outfit string rewrite | NPC mesh substitute | Same table | L | |
| `Game/Spawn/EntitySpawnShared.Npc.Core.cs:110–111,149–150` | npcId 12526–12528 | Strider special case | `NpcTemplateProfile` flags | S | |
| `Fineage.Game/Spawn/PetVisuals.cs:8` | npcId 12077 → wolf 20120 | Pet visual | Npc template row | S | |
| `Game/Manager/World.SpawnPlace.Helpers.cs:31` | npcId 20481 | Spawn placement | Template flag | S | |
| `Rendering/L2ActorLightCapture.Capture.Core.cs:12` | npcId constant | Dev lighting | Dev-only | S | |

### Networking / appearance apply

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Networking/Net/NetSession.ZoneApply.Core.cs:60–68,115–121` | `Race`, `ClassId`, `IsMage` | Spawn appearance DTO | Single `AppearanceProfile` at codec | M | |
| `Networking/Net/Domain/NetGearSession.Apply.Helpers.cs:24–28` | Race/sex/mage | Gear session patch | Same DTO | S | |
| `Networking/ClientLibrary/.../CharInfoDto.ReadSocial.cs:53–54` | `IsMage(class)` | Mage inference | Server sends `BaseClass` | S | |
| `Game/Spawn/PlayerSpawner.Helpers.cs:18–20` | `PlayerClass` → `ResolveRaceAnimation` | Player spawn mesh | `AppearanceProfile` once | S | |
| `Game/Character/CharacterSelector.Lobby.Helpers.cs:74–80` | Race/sex/class/mage | Lobby `RaceId` | Resolved on server or single resolver | M | |

### Database load (runtime consumers)

| Location | Branches on | Pipeline | Proposed profile / field | Effort | Notes |
|----------|-------------|----------|--------------------------|--------|-------|
| `Database/ArmorgrpTable.Load.Helpers.cs:55–94` | Per-race column keys | Armor texture bind | `ArmorRaceMeshProfile` rows | M | Importer |
| `Database/ModelTable.Load.Core.cs:18–20` | `RaceId` → folder | Body mesh lookup | `RaceMeshProfile` | S | |
| `Fineage.Game/Entity/CharacterRaceAnimation.cs:33–108` | Race/sex/class switches | `RaceId` resolve | **Keep** as sole resolver | S | Not a violation at boundary |
| `Fineage.Game/Entity/Interlude/CharacterHeight.cs:6–10` | Dwarf/DarkElf race anim | Collision height | `AppearanceProfile.heightClass` | S | |

**Excluded (boundary, not duplicated fixes):** `Fineage.Game/Item/WeaponClassifier.cs`, `WeaponTypeParser` tables in `Fineage.Game/Item/Enums/WeaponType.cs`, `MapClassId` **registration** in `MapClassId.Core.cs:10–28`.

**Rough counts:** ~81 gameplay `WeaponType` literal sites (excl. classifier/enum); **~70** catalogued above as violations or WIP attach debt.

---

## B) Silent fallbacks

Columns: **Location** · **Falls back to** · **Should log** · **Original data** (import gap vs optional)

### Meta / DAT tables

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Database/WeapongrpTable.cs:18–24` | Empty table after `catch` | `LogError` DI fail + empty `_weaponGrps` | **Import/init** — retail `Weapongrp_*` |
| `Database/NpcgrpTable.cs:19–23` | Same pattern | Same | **Import/init** |
| `Database/ArmorgrpTable.cs`, `EtcItemgrpTable.cs`, `ItemNameTable.cs`, `NpcNameTable.cs`, `LogongrpTable.cs`, `ItemStatDataTable.cs` | Same `catch` → `new *Table()` | Same per table name | **Import/init** |
| `Database/WeapongrpTable.Lookup.Helpers.cs:13–15` | Missing file → empty dict, return | `LogWarning` path `Weapongrp_Classic.txt` | **Import** |
| `Database/AnimTweenTable.Lookup.cs:9–13` | `30f` rate | `LogWarning` key once | Mixed — some NPCs lack row |
| `Database/Model/Abstract/AbstractGetCache.Load.Helpers.cs:50–52` | `null` prefab | `LogWarning` model path + item id | **Import** |
| `Database/Model/Abstract/AbstractGetCache.Npc.Helpers.cs:92–93` | Materials on largest SMR | `LogWarning` npc + wanted mesh | **Import** |
| `Database/LogongrpTable.Helpers.cs:42` | Default `FFighter` | `LogWarning` missing slot row | **Datapack** |

### Network → spawn apply

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Networking/Net/NetSession.ZoneApply.Core.cs:40–55` | `maxHp=100`, run/walk 120/80 | `LogWarning` entity id + zero fields | **Packet** |
| `Networking/Net/NetSession.ZoneApply.Core.cs:60–62,115–121` | `MapClassId.GetCharacterRace(classId)` if race out of range | `LogWarning` invalid `spawn.Race` | **Optional** if server omits race |
| `Networking/ClientLibrary/.../CharInfoDto.ReadIdentity.cs:20` | `MapClassId.GetRace(race)` | `LogWarning` unknown race byte | **Packet** |
| `Networking/ClientLibrary/.../UserInfoDto.ReadIdentity.cs:22` | Same | Same on UserInfo | **Packet** |
| `Fineage.Game/Entity/Interlude/MapClassId.Core.cs:54–55` | `(CharacterRace)0` (Human) unknown race id | `LogError` race byte (commented today) | **Packet** invalid |
| `Fineage.Game/Entity/Interlude/MapClassId.Core.cs:81` | `CharacterRace.Human` unknown `classId` | `LogWarning` classId | **Design** — wrong body on bad class |
| `Networking/Net/Domain/NetNpcWindows.Shop.cs:188` | `TYPE2_OTHER` if no grp | `LogWarning` itemId | **Import** |
| `Game/Spawn/PlayerSpawner.Helpers.cs:19–21` | `playerClass : 0` | `LogWarning` when class 0 at spawn | **Timing** CharSelected quirk |
| `Game/Spawn/UserSpawner.Lifecycle.Helpers.cs:17–20` | Same | Same | **Timing** |

### Entity spawn / models

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Game/Spawn/EntitySpawnShared.Core.cs:40–54` | `TryResolveFallbackNpcPrefab` | Ensure `ReportSubstituted` visible in prod | **Import** — documented stand-ins |
| `Game/Spawn/EntitySpawnShared.FallbackCreate.Core.cs:26–70` | Human trader / outfit swaps | `LogWarning` npcId wanted vs used | **Import** art gap |
| `Game/Spawn/EntitySpawnShared.FallbackCreate.Helpers.cs:50–51` | Procedural treasure chest | `LogWarning` npcId | **Import** |
| `Fineage.Game/Spawn/PetVisuals.cs:8–9` | Pet 12077 → wolf mesh 20120 | `LogWarning` once per pet id | **Import** summon mesh |
| `Controller/Validate/GroundSnapHelper.cs:11` | `FallbackOriginY=200f` | `LogWarning` when used | **Optional** terrain edge |
| `Combat/Gear/Manager/AbstractMeshManager.Mesh.Core.cs:100–114` | First non-null material for missing submesh slots | `LogWarning` weapon id + submesh count | **Import** shield dual-slot |

### Character / lobby

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Fineage.Game/Entity/CharacterRaceAnimationResolve.cs:17–22` | `BaseClass` default Fighter when 0 | `LogWarning` with real `playerClass` | **Timing** Interlude packet order |
| `Game/Character/CharacterCreator.Build.Helpers.cs:29–30` | `FFighter` naked on failure | Outer catch logs in `Create.cs` | **Import** |
| `Fineage.Game/Entity/Interlude/MapClassId.Helpers.cs:79` | `null` from name lookup | `LogError` before CharacterCreate wire | **UI** |
| `Fineage.Game/Combat/Stats/PlayerTemplates.cs:19` | `_templateClassId` null | `LogWarning` classId | **Datapack** |
| `UI/Login/CharSelectWindow.Interlude.cs:34,80` | Class label `"??"` | N/A (empty slot) | **Optional** UX |
| `UI/Login/CharSelectWindow.Interlude.cs:44` | Wrong list `_characters` vs Interlude | Fix + `LogError` OOB | **Bug** — not optional |
| `Fineage.Networking/LobbyCharacterText.cs:29` | `"Class " + classId` | `LogWarning` unknown classId | **Datapack** |
| `UI/Game/CharacterInfoWindow.Values.Core.cs:37,44` | `"No Clan"`, `"Vagabond"` | Log when packet fields missing | **Incomplete UI** |

### Animation / combat timing

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Controller/StateMachine/AnimationSystem/AnimationManager.MonsterHelpers.Core.cs:8–23` | Attack clip length `1f` | `LogWarning` objectId + state | **Import** |
| `AnimationManager.MonsterHelpers.Helpers.cs:12,22–23` | No controller → `1.6f`; pickup fallback | `LogWarning` | **Import/timing** |
| `AnimationManager.MonsterHelpers.Helpers.cs:61–66` | Missing override clip → duration `0` | `LogWarning` cycle + controller | **Import** |
| `AnimationManager.MonsterCore.Helpers.cs:43–44` | No controller → PAtk mult `1f` | `LogWarning` objectId | **State** |
| `Fineage.Controller/Anim/AnimPlayback.cs:47` | `PickupFallbackSeconds` (0.2s) | `LogWarning` clip + objectId | **Import** |
| `Fineage.Domain/Anim/LocomotionGait.cs:30` | `FallbackStrideMeters=1.5f` | `LogWarning` clip + skeleton | **Import** |
| `Fineage.Controller/Anim/AnimationDataCache.Core.cs:61` | Missing clip length `1.0f` | `LogWarning` motion name | **Import** |
| `SkillAnimationDatabase.Lookup.cs:44` | `null` cache unknown race token | `LogError` controller name | **Import** |
| `SkillAnimationDatabase.Load.Helpers.cs:39–41` | Alias e.g. MagicNoTarget→MagicShot | `LogWarning` when alias used | **Documented optional** — missing packs |
| `EntityActionSkill.CastHelpers.Core.cs:28–37` | NPC `spatk*` → `Atk01` | Enable commented `LogWarning` | **Import** |
| `SetupDurationHelper.Helpers.cs:100–126` | Magic flight `1000ms` no target | `LogWarning` skillId | **Timing** — retail uses distance |
| `Rendering/Effects/Composite/CompositePrefabEffect.PlaySpawnRoutines.Core.cs:29–41` | `ShootFallbackRoutine` | `LogWarning` skillId in prod | **Optional** safety notify |
| `DefaultEffectAttachmentResolver.Points.Helpers.cs:133` | `FirstOrDefault()` weapon bone | `LogWarning` entity + hand | **Import** rig names |

### Items / drops / shortcuts

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Game/Item/AbstractServerItem.Ops.Helpers.cs:9` | `WeaponType.none` | `LogWarning` itemId | **Import** weapongrp |
| `ItemDropPrefabLoader.Core.cs:23–41` | Coin → potion static → `null` | `LogWarning` itemId + dropModel | **Import** |
| `ItemDropPrefabLoader.Helpers.cs:41` | `PotionFallbackModel` | `LogWarning` dropModel | **Import** intentional stand-in |
| `ItemInstance.Accessors.Helpers.cs:48` | Empty display name | `LogWarning` itemId | **Import** ItemName |
| `SkillInstance.Helpers.cs:26–36` | Empty icon/name/desc | `LogWarning` skillId/level | **Import** |
| `ItemDropGrpCatalog.Core.cs:26` | `null` grp | Caller `LogWarning` itemId | **Import** |
| `ObjectPoolManager.Get.Core.cs:11–12` | `null` unknown pool tag | Uncomment `LogWarning` | **Config** |
| `Combat/Gear/Gear.Equip.Helpers.cs:32–35` | Commented `Debug.LogWarning` on missing weapon | `EquipmentMeshLog` pattern | **Import** |

### UI / icons / messages / audio

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Icon/IconManager.Lookup.Core.cs:33–35` | `_noImageIcon` | Enable missing texture warning | **Import** |
| `UI/L2SlotIconView.Helpers.cs:37` | `GetInvetoryDefaultBackground()` | `LogWarning` if default BG fails | **Optional** empty-slot chrome |
| `CooldownAnimationService.Core.cs:22–24` | Solid darken if cooltime frames missing | `LogWarning` atlas count | **Import** sysTextures |
| `UI/Game/Nameplate/Logic/L2OverheadAnchor.Helpers.cs:25,74` | `FallbackLocalTopY` | `LogWarning` no head bone | **Optional** — Interlude uses collision height |
| `Fineage.Networking/NetSystemLines.cs:65–71` | Hardcoded lines if `text` empty | `LogWarning` message id | **Server** normally sends text |
| `Game/MessageApply/MessagePacketApply.Helpers.cs:22` | `new UnhandledMessage()` | `LogWarning` null system message | **Packet** |
| `Audio/SkillCastSound.VoiceLoad.Helpers.cs:35` | `catch { }` on skill name lookup | `LogWarning` skillId + exception | **Import** skillname |

### DI / infra (gameplay-adjacent)

| Location | Falls back to | Should log | Original |
|----------|---------------|------------|----------|
| `Game/Dispatcher/DispatcherPacketServices.cs:39` | `null` on resolve `catch` | `LogWarning` type name | **DI** — not game data |
| `Game/Manager/GamePacketServices.Helpers.cs:6` | Legacy `IAnimationManager` | Error if both missing | **DI** |
| `Game/Manager/TablePacketServices.Accessors.cs:40–41` | `WeapongrpTable.Instance` (see swallow) | Central warn on empty table | **Init order** |

---

## Search patterns used

```text
WeaponType\.|== WeaponType\.|CharacterRace\.|CharacterRaceAnimation\.|PlayerClass|IsMage\(
SkillId\s*==\s*\d+|npcId\s*==\s*\d+|IndexOf\(\"bow\"|Contains\(\"bow\"|Contains\(\"dual\"
\?\?|catch\s*\{|Fallback|TryGet|DefaultBackground|GetWeaponAnim|wp_mesh
```

## Follow-ups (not in this PR)

- Implement batches 1–3 behind `WeaponAttachProfile` / `AnimSuffix` / `CombatStyleProfile`.
- Add CI grep guard (optional): fail on new `== WeaponType.` outside `WeaponClassifier` / `WeaponTypeParser` / tests.
- Cross-link: `Docs/FIST_WEAPON_MOUNT_INVESTIGATION.md`, `Docs/REFACTOR_PLAN_2026-11.md`.