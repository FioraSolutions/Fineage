# Fineage refactor plan (2026-11)

Unified plan from architecture reviews (GPT + DeepSeek + Claude 2026-11-09), reconciled with epics **#131** (client arch) and **#112** (net scale), `AGENTS.md`, and code on `dev` as of **2026-11-09**.

**Do not** chase 20k CCU / Gateway / Redis yet. Stabilize **one Zone** + client apply path first — **that bar is now MET for gameplay debug + graphics import**. #131 LifetimeScope residual is opportunistic; primary work is play/parity/assets (and #129 smoke when useful).

## North star (keep)

- Login + Zone processes, Fineage Net only, server-authoritative simulation
- AOI + presence hub, PostgreSQL durable store, Mobius Interlude parity
- Small PRs into `dev`; green CI; no inventing HUD when `ref/` originals exist

## Phase map

| Phase | Goal | When |
|-------|------|------|
| **A — Harden** | Auth, dedupe, delivery classes, contract CI truth | Mostly **done**; TLS still open before public internet |
| **B — Simulate cleanly** | DB out of world lock; main-thread budget; NetSession shrink; Zone modularization | Next (server + residual client) |
| **C — Client domain** | Dissolve `NetDomainMap`; DI / asmdef / Instance drain (#131 residual) | Parallel with B, epic #131 |
| **D — Scale** | Simulation actor, multi-zone, Gateway+TLS, Redis sessions (#112–#114) | After A–C prove one Zone |

---

## Claude review verification (2026-11-09)

Static re-check of Claude’s architecture note (server `b94fee9` / client `d7189d6` era → current `dev`). Verdict: **several P0/P1 bugs are real and actionable; persistence “memory-only enter” claim is outdated.**

### Keep (still true / still open)

| Claim | Evidence on 2026-11-09 |
|-------|------------------------|
| One ZoneWorld per process; server-authoritative move/combat | Live |
| Sim + combat share `lock (_gate)` | `StartSimulation` → regen + `CombatLoopAsync` both take `_gate` |
| Dual Login/Zone writers of `store_snapshot` id=1 | `PostgresAccountStore.RunSilent` / `PersistSnapshot` both upsert the same row; Zone export can clobber Login’s backup JSON |
| Inventory/shop/trade paths call `SaveInventory` while world work often still holds `_gate` | `SaveInventory` → `ReplaceItemsFromInventory` → `RunSilent` opens Postgres synchronously |
| Live combat loop skips proactive AI | `CombatLoopAsync` → `RunPlayerCombatTickAsync` only; `TickAggression` / `TickLeash` live in dead `RunCombatTickAsync` (tests only) |
| Zone drop leaves client in-world | `FineageZoneTransport.OnDisconnected` never raised; `Disconnect()` only destroys `NetSession`; `GameManager.OnDisconnect` unsubscribed |
| `NetZoneDispatch` silent holes | No cases for `StopMove`, `SiegeInfo`, `OlympiadStatus`, `SevenSignsStatus`, `DungeonEntered`, `PrivateStoreInfo`, `FriendList`; no `default` |
| `TickSpawns` scans all `_spawnSlots` every sim tick | `ZoneWorldSpawns.TickSpawns` full foreach; due-check is cheap but cost grows with catalog |

### Wrong / outdated in that Claude pass

| Claim | Reality on 2026-11-09 |
|-------|------------------------|
| `FindById` reads only memory from `Open()` snapshot | **Outdated for Postgres:** `PostgresAccountStore.FindById` → `NormalizedStoreReader.TryLoadById` (+ hydrate). Enter path already loads off-lock (B1) |
| Comment that enter is a Postgres round-trip is a lie | **Half-true:** async off-lock `FindById` *is* SQL now; the old “memory only” reading is wrong |
| Inventory always full-wipe on pickup | **Wrong for hot path:** `RunSilent` + `ReplaceInventoryOnly`; full wipe remains on `PersistSnapshot` / process start / full `Save()` |
| Enter still loads character under `_gate` | **Fixed (B1)** |

### Priority remap from Claude (do next, small PRs)

1. **B7 — Live AI tick:** fold `TickAggression` + `TickLeash` (+ `TickAttackStance` if still missing) into `RunPlayerCombatTickAsync`.
2. **C4 — Zone disconnect UX:** raise `OnDisconnected` from remote close + intentional `Disconnect`; wire to `GameManager.OnDisconnect`.
3. **C5 — Dispatch holes:** add missing inbound opcodes (apply or log) + `default` warning.
4. **B8 — Persist off `_gate`:** after world mutation under lock, queue dirty inventory and run `SaveInventory` / `RunSilent` outside the lock (same pattern as enter/unlink persist).
5. **B9 — Snapshot ownership:** stop dual full-JSON writers — Zone must not rewrite `store_snapshot` from a partial in-memory view; prefer normalized tables as SoT (already true for `FindById`) and treat snapshot as backup/export only.
6. **B10 — Spawn due index:** due SortedSet for respawns (Backend #498).

---

## GPT review verification (2026-11-08)

Static re-check of the long GPT “client + server” architecture review against both `dev` tips. Verdict: **direction mostly right; several P0/P1 claims are already fixed or overstated.**

### Keep (still true)

| Claim | Evidence |
|-------|----------|
| Fineage Net (not L2 wire / not MsgPack V2) is the right port strategy | Backend README + `net-protocol.md`; client Net-only |
| Login + Zone + HMAC handoff + Postgres SoT | Live |
| `ZoneWorld` / `NetZoneServer` / `NetCodec` are oversized hubs | ~1.4k + many partials; NetZoneServer ~2.9k; NetCodec ~3.5k |
| Global `_gate` on `ZoneWorld` is a future bottleneck; prefer single-writer sim later | Still `lock (_gate)` across world methods |
| `ZoneEntity` is a player+NPC+ground god bag | **Partial:** `ZoneGroundItemPayload` composed; player/NPC payloads still flat |
| `ZoneWorld` → presence fanout couples domain to net encoding | **Partial:** `INetPresenceBridge` now in `Fineage.Zone.World`; still set from `NetZoneServer` and still carries Net.Contract DTOs |
| Auth password bytes on the wire; TLS not on client TCP yet | `EncodeAuth` writes plaintext password; TLS = Gateway/#17/#113 / Phase A6 |
| Keep TCP for now; QUIC later | Matches `ARCHITECTURE_NET` + #123 |
| Default AOI grid cell 3000; do not flood full world | `ZoneAoiSettings.DefaultAoiCell = 3000` → `GridAoi` |
| Unity PR CI does not compile/test the Unity project | `build-and-test` = tools + guards; Player builds only on `release-clients` |
| Client asmdef / `Fineage.Di` incomplete; gameplay still mostly `Assembly-CSharp` | Status Now in `CLIENT_ARCHITECTURE_REVIEW.md` |
| Hybrid tick (not naïvely update everything at 20 Hz) is the right long-term model | Aligns with #116 + regen/AI timers already in world |
| Do **not** microservices-split combat/inventory/chat now | Agrees with project principles |

### Wrong / outdated in that GPT pass

| Claim | Reality on 2026-11-08 |
|-------|------------------------|
| Contract sync “skipped in standard client CI” | **Fixed (A5):** workflow checks out Backend `dev`, runs `FINEAGE_REQUIRE_CONTRACT_SYNC=1 … --check` |
| Handoff secret “dev-default risk” as unfixed P0 | **Fixed (A1):** non-loopback refuse without `FINEAGE_HANDOFF_SECRET`; DefaultSecret tests/loopback only |
| Handoff only validates signature | **Fixed (A2):** `IHandoffStore.TryConsume` one-time consume |
| Skills “only AutoSkills, no table” | **Wrong:** `character_skills` + `LearnedSkills`; AutoSkills merge on enter |
| Quests/shortcuts “need tables” as greenfield | Already have normalized quest/shortcut tables; gaps are elsewhere (clan JSONB, world sidecars) |
| IncomingPacketActions / “DI barely started” as current P0 | **Historical:** IPA deleted (#142); PacketServices drain largely done; residual = asmdef moves + `Fineage.Di` |
| NetDomainMap as live apply path | **Gone (C1)** + CI freeze guard |
| Client “no main-thread net pump / coalesce” | **Done (B3/B4 partial):** `NetMainThreadPump`, lobby extract, zone partials |
| Metrics / backpressure entirely missing | **Partial:** Prometheus `/metrics` + tick/online; inbound Wait (not DropOldest) for critical; outbound critical path — #117/#119 still open for full AC |
| `#34` Unity CI closed ⇒ Unity PR gate exists | **Superseded:** [#737](https://github.com/FioraSolutions/Fineage/issues/737) EditMode/compile gate landed (#874) |

### Priority remap after verification

1. **Before public internet:** TLS / Gateway (#17 residual, #113). Login password brute-force rate limit landed (Backend `AuthAttemptLimiter`).
2. **Before more Zone surface area:** Backend modularization epic (handlers / systems / entity composition / domain event publisher) — see Phase B below and Backend tracker.
3. **Client residual (#131):** real asmdef moves + `Fineage.Di`; optional Unity EditMode/PR compile follow-up (reopen/replace #34).
4. **Defer:** 20k / Redis / multi-zone / QUIC until one Zone is measured.

---

## Phase A — Harden (P0 / early P1)

### A1. Handoff secret fail-closed — done
- `HandoffTokens.FromEnvironment()` must **not** silently use `DefaultSecret` when binding non-loopback.
- Login + Zone: missing `FINEAGE_HANDOFF_SECRET` on external bind → **refuse start**.
- Keep `DefaultSecret` only for explicit tests / loopback / `--selftest`.
- Test: production-path startup without secret fails.

### A2. One-time handoff consume — done
- `IHandoffStore` already exists but only wraps HMAC validate.
- Add atomic consume (nonce / jti): Postgres OK for single Zone; Redis later for multi-zone.
- Zone Auth must **consume**, not only verify signature + TTL.

### A3. CommandDeduper sliding window — done
- After prune, replay of purged sequences is accepted today — fix.
- Reject `sequence <= watermark` even if not in `_seen` (or keep a contiguous bit window).
- Extend `FramingAndDedupTests`.

### A4. Inbox / outbound delivery classes — done (coalesce + TrySendCritical + outbound queue env)
- **Inbox:** do not `DropOldest` for economy / trade / auth; coalesce movement; backpressure or disconnect on critical overflow.
- **Outbound:** Critical (spawn/despawn, inventory, stats) vs Replaceable (MoveState) vs Transient (VFX). Reliable path must not die in `TrySend` before TCP.
- Wire `FINEAGE_ZONE_OUTBOUND_QUEUE` into `NetTcpServer.RunAsync` (today logged but ignored → default 256).

### A5. Contract CI truth (client) — done
- `sync-net-contract.py --check` skips when Backend missing → false green.
- CI: checkout Backend at pinned SHA (or `dev` tip) and fail on drift.
- Optional later: generate client contract from Backend in CI.

### A6. TLS (playtest gate) — deferred (VPN/LAN playtest)
- Either transport TLS or Gateway termination before non-local public auth.
- Can land after A1–A3 if playtest stays VPN/LAN.

### A7. Stop leaking handler exceptions — done (Zone + Login)
- `RejectAsync(..., ex.Message)` → normalized client reason; full stack only in server logs.

---

## Phase B — Simulate cleanly (P1)

### B1. ZoneWorld enter path — done
- Load character / inventory / quests / skills **async outside** `_gate`.
- Under lock: validate session + apply snapshot only.
- Make `TryEnterAsync` genuinely async.

### B2. Toward Zone Simulation Actor — later
- Longer term: single owner of world mutation; net handlers enqueue intents.
- Do not invent microservices; keep one process until measured pain.
- Tracker: Fineage **#116** (20 Hz / hybrid tick) + Backend modularization epic.

### B2b. Zone / Net modularization (open — GPT P1, still valid)
Incremental extractions only (small PRs). Do **not** rewrite the world for purity.

| Slice | Intent |
|-------|--------|
| `NetZoneServer` handlers | [x] Opcode groups → `*PacketHandler`; fanout encode in `NetWorldEventPublisher`; listener is dispatch + lifecycle |
| Domain events | [x] `IWorldEventPublisher` in `Fineage.Zone.World` + Net encode in `NetWorldEventPublisher`. All fanout DTOs Protocol/primitives; ShortCut/admin World entry Protocol/primitives. World has no `Fineage.Net.Contract` references |
| `ZoneWorld` systems | Prefer focused `*Rules` / `*Service` over growing the partial bag. Landed extracts include Trade/Party/Ground/PrivateStore/Friend/Craft/Manufacture/ShortCut/SkillUse/Consumable/Shot/ShopSession/Gear/EnchantSession/WarehouseSession/Move (+ prior Clan/Siege/Olympiad/SS/Say/Death). Residual: opportunistic Rules only; simulation actor is B2 later |
| `ZoneEntity` composition | [x] ground + spoil + aggro + player store/social/status/skills/identity + NPC template/merchant + shared move/vitals/combat payloads composed; leftover refs (Session/Inventory/Warehouse/Shop/Quests/Knownlist/DungeonId/InstanceId) stay on entity |
| `NetCodec` | Round-trip tests mandatory; domain partials: session (`NetCodec.cs`), `PresenceCombat`, `WorldEvents`, `Clan`, `Friends`, `Craft`, `PrivateStore`, `Trade`, `Party`, `Quest`, `Shop` |

### B3. Client main-thread budget — done
- Cap `/frame` work from `NetSession` `_main` queue; coalesce MoveState; never drop EntityDespawn / inventory.

### B4. NetSession decomposition — partial
- [x] `NetCommandSender` + zone `Send*` return `Task` (async void wrappers removed).
- [x] Lobby flow extracted to `NetLobbyFlow` (`BeginLobby`, roster CRUD, lobby dispatch, handoff enter orchestration).
- [x] `NetSession` split into partials (`NetSession.ZoneConnect`, `NetSession.ZoneDispatch`, `NetSession.ZoneApply`, `NetSession.Commands`).
- [x] Main-thread apply pump extracted to `NetMainThreadPump` in `Fineage.Networking` (critical + budgeted normal queues).
- [x] Shared outbound send pump: `NetOutboundSendPump` on `NetTransport`.
- [x] Zone inbound switch extracted to `NetZoneDispatch` (apply bodies stay on `NetSession` / receivers).
- Transport already in `Fineage.Networking`; further Session shrink when cycle-safe.
- [x] `NetSession.ZoneApply` split into Combat/Social/Shop/Gear partials (#131 residual)

### B5. State recovery — partial (client disconnect invalidate + epoch)
- EnterWorld snapshot + revision; full resync / reconnect path when Critical outbound was dropped — still later.
- **Landed (client):** on zone TCP close, dispose transport + clear Connecting/Hello flags; clear `PlayerInfo` on `NotifyDisconnected`; `OnDisconnect` clears `WorldSpawnReady` and cancels `ObjectIdWaitMap` waiters; `ZoneCommands.Send` drops TX when `!Connected`.
- **Landed (client epoch):** `NetSession.ZoneEpoch` bumps on connect-start / TCP-close / destroy; queued zone applies no-op when epoch drifted (stale post-disconnect packets).

### B6. Economy invariants — partial (tests)
- Trade / inventory / adena: atomic apply + persistence tests (dup-item / double-spend). Not claiming a live bug; GPT correctly flagged the invariant set.
- [x] Trade: over-offer count refused; post-finish reconfirm cannot invent a second transfer (`NetTradeTests`).
- [x] Warehouse: over-count / duplicate-line deposit refused; stale objectId deposit/withdraw cannot invent a second move (`NetWarehouseEconomyTests`).
- [x] Inventory destroy: stale objectId after full remove cannot invent another removal (`NetGroundItemTests`).
- [x] Shop limited-stock second buy refused without taking adena (`NetShopTests`).

### B7. Live AI tick — done (Backend #490)
- Fold `TickAggression` + `TickLeash` (+ `TickAttackStance`) into `RunPlayerCombatTickAsync`.
- Keep `RunCombatTickAsync` as the test/helper entry that already includes them, or make it call the live tick body.

### B8. Persist off `_gate` — largely done (#492–#497)
- World mutation under lock; `SaveInventory` / warehouse Postgres after release via `WithDeferredInventoryPersist`.
- Covered: ground, combat, shop/gear/trade/private-store, warehouse/enchant/craft, quest/shots, manufacture/sweep/seals, dungeon/olympiad/bypass, skill use/learn, teleport.
- Residual: niche admin/test paths; consumables already ride `TryUseItemAsync`.

### B9. Snapshot ownership — done (#491 + #493)
- Stop dual full-JSON writers of `store_snapshot` id=1 from Login + Zone partial memory.
- Normalized tables remain SoT for `FindById`; snapshot = backup/export only.
- Login `AfterSaveSnapshot` writes backup JSON only (no `UpsertFromSnapshot`); `TryAddCharacter` upserts one character row.

### B10. Spawn due index — done (Backend #498)
- `_spawnDue` SortedSet; `TickSpawns` pops due entries only.

---

## Phase C — Client domain (#131 residual)

### C1. Dissolve `NetDomainMap` — tactics 1–4 done
Legacy path (deleted):

```
Fineage Net → NetSession → NetDomainMap → Interlude DTO → PacketServices / WorldPacketApply → World
```

Current path:

```
Fineage Net → NetSession / NetGearSession / PacketServices → Interlude DTO → WorldPacketApply → World
```

**Tactics (small PRs, no big-bang):**

1. [x] **Split** `NetDomainMap` into domain adapters; delete `NetCombatMap`, `NetSocialMap`, `NetGearMap`, `NetEntityMap`.
2. [x] **Expand** apply colocation — DTO construction lives on `NetSession` / `NetGearSession` / packet services; paperdoll overlay on `NetPaperdollApply` (slot constants on `NetPaperdoll`).
3. [x] **Kill fabricated defaults** — EntitySpawn carries Level/MaxHp/PAtkSpd; dead NetNpcInfo stubs removed.
4. [x] `NetDomainMap` and split `Net*Map` types deleted; CI guard `scripts/check-frozen-net-domain-maps.py`.

Call-site inventory (2026-11-07): DTO/apply logic on `NetSession` (spawn, move, social, combat tails), `NetGearSession` (UserInfo, inventory, skills), party/HUD packet services — no `Net*Map` imports.

### C2. DI / Instance drain — largely done
- [x] Non-UI manager / table / combat / login UI Instance drain via `*PacketServices` (through PR **#726**).
- [x] No new `*.Instance` for frozen seams (`World` / `GameManager` / `EventProcessor` / `PlayerController`).
- [x] First real asmdef content: `Fineage.Networking` (NetPaperdoll **#684** + engine-free Net domain batch + `NetTransport` / `NetCommandSender`).
- [x] First `Fineage.Game` content: cycle-safe item / action / entity enums + drop/effect deco constants.
- [x] First `Fineage.Controller` / `Fineage.UI` content: state/intention/action enums + chat/nameplate enums.
- [x] `Fineage.Di` seeded with `ILateBound` / `LateBoundMono` (not LifetimeScope yet — those still touch `Assembly-CSharp`).
- [x] `GameState` enum moved into `Fineage.Controller` (cycle-safe).
- [x] `MonsterSoundEvent` / `AmbientSoundType` moved into `Fineage.Game` (cycle-safe).
- [x] `EquipmentCategory` / `SkillEventType` moved into `Fineage.Game` (cycle-safe).
- [x] `HitType` / `ProjectileImpactType` / `SpecialSkillType` moved into `Fineage.Game` (cycle-safe).
- [x] `ObjectType` → `Fineage.Game`; `MagicPhase` → `Fineage.Controller`; `TypeToolTips` → `Fineage.UI`.
- [x] `WhType` moved into `Fineage.Game` (cycle-safe).
- [x] `InventoryChange` moved into `Fineage.Game` (cycle-safe).
- [x] `SlotType` moved into `Fineage.UI` (cycle-safe).
- [x] `SMParamType` moved into `Fineage.Networking` (cycle-safe).
- [x] `EnchantResult` moved into `Fineage.Networking` (cycle-safe; was nested `EnumResult`).
- [x] `LightAttachSubject` moved into `Fineage.Game` (cycle-safe).
- [x] `EnumType2` moved into `Fineage.Game` (cycle-safe).
- [x] `FineageLogLevel` / `FineageLogCategory` moved into `Fineage.Networking` (cycle-safe).
- [x] `MagicSkillUseKind` moved into `Fineage.Game` (cycle-safe; was nested `MagicSkillPresentation.Kind`).
- [x] `EffectDriftDirection` moved into `Fineage.Game` (cycle-safe; was nested `EffectPrefabStaggeredDrift.DriftDirection`).
- [x] `TimeUtils` moved into `Fineage.Game` (cycle-safe).
- [x] `RandomUtils` / `StringUtils` / `ArrayUtils` moved into `Fineage.Game` (cycle-safe).
- [x] `NumberUtils` moved into `Fineage.Game` (cycle-safe).
- [x] `TransformUtils` moved into `Fineage.Game` (cycle-safe).
- [x] `ByteUtils` moved into `Fineage.Game` (cycle-safe; dropped unused UIElements import).
- [x] `L2DiagPaths` moved into `Fineage.Networking` (cycle-safe).
- [x] `FineageLog` / `LobbyFlowLog` moved into `Fineage.Networking` (cycle-safe).
- [x] `CalcBaseParam` / `ProjectileFlightTimeCalculator` moved into `Fineage.Game` (cycle-safe).
- [x] `PlayerLocomotionAnim` moved into `Fineage.Controller` (cycle-safe; `LocomotionCrossFadeSettings` stays Assembly-CSharp).
- [x] `PlayerStriderAnim` moved into `Fineage.Controller` (cycle-safe).
- [x] `CharTemplateRegistry` / `GenderStats` / `BaseStatModel` moved into `Fineage.Controller` (cycle-safe).
- [x] `AnimationDataCache` moved into `Fineage.Controller` (cycle-safe).
- [x] `IAnimationController` / `AnimPlayback` moved into `Fineage.Controller` (cycle-safe).
- [x] `AnimationExitCrossFade` / `LocomotionCrossFadeDefaults` moved into `Fineage.Controller` (cycle-safe; table resolve stays on `LocomotionCrossFadeSettings`).
- [x] V2 particle enums / `L2ParticleCoordinateSystemUtil` moved into `Fineage.Game` (cycle-safe; `EffectPlaybackContext` stays Assembly-CSharp).
- [x] `DatUtils` string/JSON helpers moved into `Fineage.Game` (cycle-safe; `AbstractgrpDatParser` stays Assembly-CSharp).
- [x] `MapUtils` moved into `Fineage.UI` (cycle-safe UIElements helpers).
- [x] `DropdownFieldUtils` moved into `Fineage.UI` (cycle-safe).
- [x] `ToolTipsUtils` moved into `Fineage.UI` (cycle-safe; SlotType already in Fineage.UI).
- [x] `VectorUtils` moved into `Fineage.Game` (cycle-safe; Entity/World aim/gravity stay on `VectorWorldUtils`).
- [x] `L2PawnWalk` moved into `Fineage.Controller` (cycle-safe; `L2PawnWalkEntity.IsCharInfoPawn` stays Assembly-CSharp).
- [x] `L2NameplateAnchor` moved into `Fineage.Game` (cycle-safe capsule math; OverheadAnchor stays Assembly-CSharp).
- [x] `ArmorBodySlots` moved into `Fineage.Game` (cycle-safe; uses `ItemSlot` already in Fineage.Game).
- [x] `AnimatorBoolSet` moved into `Fineage.Controller` (cycle-safe Animator parameter helpers).
- [x] `L2YebisPostRuntime` moved into `Fineage.Game` (cycle-safe nameplate flush handshake flag).
- [x] `L2FxCompositorLayers` moved into `Fineage.Game` (cycle-safe Unity layer helpers).
- [x] `CharacterAnimPaths` moved into `Fineage.Game` (cycle-safe race clip path helpers).
- [x] `Appearance` moved into `Fineage.Game` (cycle-safe collision/hand appearance DTO).
- [x] `SysStringData` moved into `Fineage.Game` (cycle-safe id/name DTO).
- [x] `L2AppRand` moved into `Fineage.Game` (cycle-safe L2 AppRand float helpers).
- [x] `BoneModification` moved into `Fineage.Game` (cycle-safe spine offset DTO).
- [x] `VariableItem` moved into `Fineage.Game` (cycle-safe name/objId DTO).
- [x] `NpcName` moved into `Fineage.Game` (cycle-safe npc name/title DTO).
- [x] `Logongrp` moved into `Fineage.Game` (cycle-safe lobby race pose DTO).
- [x] `PetVisuals` moved into `Fineage.Game` (cycle-safe summon id helpers; `IsPet(owned,npcId)`).
- [x] `WeaponChargeSkill` id helpers moved into `Fineage.Game` (playback stays `WeaponChargeSkillPlayback`).
- [x] `Abstractgrp` moved into `Fineage.Game` (cycle-safe item grp base; ItemMaterial/ItemGrade already there).
- [x] `EtcItemgrp` moved into `Fineage.Game` (cycle-safe etc item grp row).
- [x] Database model DTOs moved into `Fineage.Game`: ActionData / AnimationCombo / ItemStatData / NpcDecoEffect / RecipeMaterials / SkillNameData / SystemMessageDat / RecipeData / Skillgrp / Armorgrp / Weapongrp.
- [x] `PollInput` moved into `Fineage.Game` (Unity.InputSystem asmdef ref + layout guard updated).
- [x] Manor `Seed` / `SeedProduction` / `CropProcure` moved into `Fineage.Domain` (shared by Networking wire DTOs + Game; `CastleCrop` stays ACS via ItemInstance).
- [x] `LODAdjustment` moved into `Fineage.Game` (cycle-safe shadow LOD tuning DTO).
- [x] Cycle-safe enum/util/DTO/helper slices largely exhausted (through #864).
- [x] `PacketReader` / `PacketWriter` / `IWireDto` moved into `Fineage.Networking` (ByteUtils calls inlined to avoid Game↔Networking cycle).
- [x] Cycle-safe Interlude wire DTOs + `IOutgoingDto` moved into `Fineage.Networking` (ItemInstance/ItemSlot/Entity/UI-tied DTOs stay ACS; #737 clean compile bounced Product/Entity/SystemMessage-tied DTOs back).
- [x] `Hit` / `SMParam` moved into `Fineage.Networking`; `VectorUtils` / `NumberUtils` / `ChatTypeData` into `Fineage.Domain` (Networking must not reference Game).
- [x] `GameRuntime` Input/PositionValidation/Bus fallbacks use `ControllerPacketServices` (no direct `.Instance`).
- [x] UI/Database/Audio/Icon `Instance` getters marked `[Obsolete]` (PacketServices / Inject preferred).
- [x] Remaining non-Fineage `Instance` getters marked `[Obsolete]` (PacketServices / Inject preferred; includes managers/controllers round 2).
- [x] Cycle-safe leaf MonoBehaviours wave 2 into `Fineage.Game` (`L2ActorLocalLight`, `L2CloudLayers`, `ItemPickupMotion`, `SwordSetup`, `IRotation`/`RotationService`, `DropMeshLifetimeTracker`) + delete unused empty MB stubs.
- [x] First cycle-safe gameplay MonoBehaviours into `Fineage.Game` (Windmill / BoatSway / DestroyAfter / FpsLimiter / ParticleTimerReset / LayerCullDistance / RandomizeChildYawOnEnable / L2FireNoWorldLight / EffectPrefabRotator / DontDestroyOnLoad).
- [x] Cycle-safe settings/audio/terrain into `Fineage.Game` (`GameSettings`, `InterludeSfx`, `L2TerrainLayerStack`/`L2TerrainOverlayRenderPass` + URP asmdef refs).
- [x] Cycle-safe billboard + instanced prop MonoBehaviours into `Fineage.Game` (`IRegisteredBillboard`, `RegisteredBillboards`, `CameraPreRenderUpdater`, `L2InstancedPropSet`/`L2InstancedPropRenderer`).
- [x] Client ACS hub file-splits continued (PlayerController Move/Rotate/Query)
- [x] Cycle-safe L2 beam builders into `Fineage.Game` (`L2BeamEmitterStripBuilder`, `L2BeamStripMeshBuilder`, `L2MultiLayerBeamBuilder`).
- [x] Cycle-safe L2 beam builders into `Fineage.Game`.
- [x] Client ACS hub file-splits continued (CharacterSelector Lobby/Spawn/Select)
- [x] Client ACS hub file-splits continued (EffectManager Registry/Play/Impact)
- [x] Client ACS hub file-splits continued (EntitySpawnShared Fallback/Entity)
- [x] Client ACS hub file-splits continued (MagicCastData Timing/Clips)
- [x] Cycle-safe `L2MaterialPropertyCopier` / `NpcDecoKeepWorldUp` / debug `DrawLine` into `Fineage.Game`.
- [x] Overnight ACS hub file-splits (CharacterSelector/EffectManager/EntityAction/UI windows/zone commands/ItemDrop FX).
- [ ] Remaining asmdef work = LifetimeScope / more gameplay MonoBehaviours when types are cycle-safe (hard residual).
- [x] `LifetimeScopeMonoRegistration` in `Fineage.Di` (VContainer ref) — App/Login/Game scopes share RegisterIfFound/LateBound/LiveMono; ACS PacketServices fallbacks stay in GameLifetimeScope.
- [x] Cycle-safe preview/debug MonoBehaviours into `Fineage.Game` (`PreviewOrbitCamera`, `PlayerLoopDebugger`).
- [x] Client ACS hub file-splits continued (SkillCastSound.Helpers; MovementData.Step)
- [x] Cycle-safe `NpcDecoKeepWorldUp` into `Fineage.Game`.
- [x] Cycle-safe `NpcDecoKeepWorldUp` + `NpcDecoUpright` into `Fineage.Game` (ACS `NpcDecoAttachment` delegates upright yaw).
- [x] Cycle-safe `L2MaterialPropertyCopier` into `Fineage.Game`.
- [x] Cycle-safe `L2MaterialPropertyCopier` (+ AppRand partial) into `Fineage.Game`.
- [x] Cycle-safe debug `DrawLine` into `Fineage.Game`.
- [ ] Remaining asmdef work = LifetimeScope types themselves / more gameplay MonoBehaviours when cycle-safe (hard residual).

### C3. Doc sync
- [x] Strip “Zone v2 / MessagePack” wording from `NetDomainMap`, `IWorldDomainSink`, stale comments.

### C4. Zone disconnect UX — done (#878)
- Raise `FineageZoneTransport.OnDisconnected` on remote close and intentional `Disconnect`.
- Wire AppLifetimeScope → `GameManager.OnDisconnect` so zone drop returns to menu.

### C4b. Zone connection semantics — done (A01)
- GPT deep-audit finding: `FineageZoneTransport.Connected` was `NetworkingPacketServices.Session != null`.
- **Fixed:** `Connected` = zone TCP open (`NetSession.IsZoneSocketConnected`); added `HasSession`, `ZoneConnectionState` / resolver (+ tools tests).
- Residual: full reconnect/resync state machine still B5.

### C4d. NetSession composition owner (GPT A02) — done (partial)
- AppLifetimeScope creates/binds the single host via `NetSession.CreateForCompositionRoot`.
- `FineageZoneTransport` no longer calls `Ensure()`; login/action call sites use `TryGetBound`.
- `Ensure()` marked obsolete with warning. Residual: injectable `IZoneSessionHost`.

### C4e. ZoneCommands / AppLifetimeScope lifecycle (GPT A03 / A06) — done (partial)
- `ZoneCommands.Unbind` on AppLifetimeScope destroy; Transport is bind-only (no silent App.Resolve).
- Disconnect handler unsubscribed; App.Container cleared when this scope owned it.
- DAT table + `AnimationManager` static Bind seams cleared via `Unbind` on AppLifetimeScope destroy.
- Residual: tables still Bind in BuildCallback (not fully DI-owned yet).

### C4f. LoginRuntime / LoginLifetimeScope diagnostics (GPT A07) — done (partial)
- Resolve failures and PacketServices fallbacks log warnings; LoginLifetimeScope no longer empty-catch.
- Residual: still optional PacketServices fallback until login inject is mandatory everywhere.

### C4c. World entity wait (GPT A04) — done
- Replace `World.GetEntityAsync` `Task.Delay(150)` poll with `ObjectIdWaitMap` (notify on Register*, fail on unregister, cancel on clear/destroy).
- Timeout remains 150ms for parity; early complete when spawn registers. Tools tests cover the wait map.

### C4g. World entity registry (GPT A05) — done (partial)
- Extract `WorldEntityRegistry<T>` (all / players / npcs maps) under `Fineage.Domain/World`.
- `World` Register*/Remove/Query/Clear delegates to registry; spawn/presentation stays on World.
- Residual: further World splits (spawn vs apply vs combat) still opportunistic.

### C5. NetZoneDispatch holes — done (#879)
- Add inbound cases for `StopMove`, `SiegeInfo`, `OlympiadStatus`, `SevenSignsStatus`, `DungeonEntered`, `PrivateStoreInfo`, `FriendList` (apply or log).
- Add `default` warning so unknown opcodes are visible.

### C5b. Opcode inbound coverage guard — done
- CI `scripts/check-net-zone-inbound-opcodes.py`: every Zone→client `NetOpcode` must have a `NetZoneDispatch` case (or explicit Client→zone / lobby outbound).
- Closed remaining decode-only gaps (BossStatus, Olympiad match/countdown/result, craft/store lists, friend/block updates, SevenSignsSky).

---

## Phase D — Scale (#112) — later

Gateway + TLS, multi-zone ownership, Redis session directory / pub-sub, distributed presence, load bots + AOI profiles. Only after Phase A–B metrics say one Zone is healthy.

---

## Agent start here (as of 2026-11-11 — gameplay-debug bar)

**Status Now:** Stabilize A–C bar for **gameplay debug + graphics import is MET** (~80–85%). Di seams through `IGameManager` / `ISceneLoader` / `ILobbySession` / `IWorldEntityLookup` / `ISkillNameTable` / `ISkillgrpLookup` / `IZoneTransport` / `PacketApplyQueue` (+ leaf waves ~#1334–#1341). LifetimeScope shells still ACS — **ongoing residual, not the gate**. See `CLIENT_ARCHITECTURE_REVIEW` Status Now.

**Primary focus now:** debug gameplay, import graphics / Interlude assets, parity fixes. Backend #353 acceptance largely met. Phase D scale still later.

| Track | Do next | Do **not** |
|-------|---------|------------|
| Client primary | Gameplay debug, graphics/import, Interlude parity; #129 smoke when useful | Wait on LifetimeScope→`Fineage.Di` before play/assets; invent HUD |
| Client [#131](https://github.com/FioraSolutions/Fineage/issues/131) residual | Opportunistic seams only when they unblock concrete pain | DAT/World grind as finish-line; empty hub splits; fat `IDatRegistry` / ACS `ItemName` |
| Backend [#353](https://github.com/FioraSolutions/Fineage-Backend/issues/353) | Opportunistic hub residual only; sim actor / retire `_gate` later when measured | Microservices; rewrite ZoneWorld; invent non-Mobius rules |
| A6 | TLS / Gateway before public internet (#17 / #113) | Treat Phase D scale as current work |
| Docs | Keep this plan + epic bodies honest after each wave | Trust unchecked boxes in old review prose without Status Now |

**Out of scope now:** Redis, multi-zone, QUIC, 20k claims, ECS rewrite, microservices.

Living client Status Now: [`l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md`](../l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md). Backend Status Now: [Fineage-Backend `docs/ZONE_MODULARIZATION.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/ZONE_MODULARIZATION.md).

## Success metrics

- [x] External bind without `FINEAGE_HANDOFF_SECRET` fails closed
- [x] Deduper rejects pruned-window replays
- [x] Client CI fails if Net contract drifts vs Backend
- [x] `NetDomainMap.cs` and frozen `Net*Map` adapters gone (`check-frozen-net-domain-maps.py`)
- [x] No new world DB I/O under `_gate` on enter (Phase B1)
- [x] Unity PR EditMode/compile gate (#737) via `unity-compile.yml`
- [ ] TLS (or equivalent) before public auth
- [x] New Zone opcodes land in `*PacketHandler` (session + knownlist extracted); ZoneWorld hub file-split (primaries mostly <500 lines)
- [x] Real `Fineage.*` asmdef content for migrated types; `Fineage.Di` more than a stub (`ILateBound`)
- [ ] LifetimeScope / remaining gameplay MonoBehaviours in `Fineage.*` assemblies when cycle-safe (first leaf MB wave landed)
- [x] Live combat loop runs `TickAggression` / `TickLeash` (B7, Backend #490)
- [x] Zone disconnect raises UI path (C4, #878)
- [x] `NetZoneDispatch` covers server-sent world/event opcodes or logs `default` (C5, #879)
- [x] Inventory persist deferred off `_gate` on hot inventory paths (B8, Backend #492–#497)
- [x] Hot-path / Open / Login snapshot no longer clobber normalized SoT (B9, Backend #491 + #493)
- [x] `GameRuntime` windows split (#881)
- [x] Leftover table/spawn Instance drains via PacketServices (#888–#889)
- [x] `LoginRuntime` windows resolver-only; Instance fallbacks in LoginPacketServices (#890)
- [x] BufferPanel SkillGrps via TablePacketServices (#897)
- [x] Backend hub splits continued (#513–#522: ClassCatalog/QuestCatalog/GameData/AccountStore/ZoneWorld.Members/PostgresAccountStore/ZoneEntity.Members/NormalizedStoreWriter/NetCodec.Presence/WorldMessages)
- [x] Backend hub splits continued (#524–#543: NetCodec.Session/LoginResult/NpcWarehouse/WorldEvents/ZoneCombat/Skills/QuestsKills/SiegeBoard/NetZoneHandle/NetLoginHandle/BossSchedule/SevenSigns/ShopTrade/ClassCatalog.Load/GeoEngine.Los+Region/Olympiad.Actions/VillageMasterModels/PlayerInventory.Bag/QuestMachine.Ops.Helpers)
- [x] Client HitManager Remote/Impact (#902); World Spawn/Query (#903)
- [x] Backend hub splits continued (#551–#561: NetLogin Handle.Roster / Cli.Listen / ZoneEntity Identity+Status / WorldEventPacketHandler / Abnormals.Tick / ToNet.Social / Skills.Members.Support / SessionLifecycle.Enter / Warehouse.Helpers / ZoneWorld.More.Enter)
- [x] Client ACS hub file-splits continued (#911–#927 + wave3: TargetWindow / CharacterInfoWindow / ProjectileManager / FineageZoneTransport.Commands; prior: EntitySpawnShared / PlayerInventory / CreatorTableWindows / NetworkCommands / ClanWindow / SceneLoader / EnchantWindow / Entity / EntityActionSkill / DealerWindow / BaseAnimationController / NetGearSession.Map / L2SlotManager / NetCommandSender / ItemDropPresentation / DayNightCycle / ParticleStreamRuntime)
- [x] Client ACS hub file-splits continued (TargetWindow Properties/Effects; CharacterInfoWindow Build/Chrome/Tabs/Values; ProjectileManager Impact/Update; FineageZoneTransport Commands/Helpers)
- [x] Client ACS hub file-splits continued (#911–#927: EntitySpawnShared / PlayerInventory / CreatorTableWindows / NetworkCommands / ClanWindow / SceneLoader / EnchantWindow / Entity / EntityActionSkill / DealerWindow / BaseAnimationController / NetGearSession.Map / L2SlotManager / NetCommandSender / ItemDropPresentation / DayNightCycle / ParticleStreamRuntime)
- [x] Client ACS hub file-splits continued (CharSelectWindow Lobby/Interlude/Actions; WorldPacketApply Combat/Movement; EntityActionVisual Combat/Helpers; SkillbarWindow Dock/Layout/Shortcuts)
- [ ] LifetimeScope / gameplay MB asmdef remains #131 residual — **opportunistic only; not the gameplay-debug gate**
