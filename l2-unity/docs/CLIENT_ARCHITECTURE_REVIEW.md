# Fineage Unity Client — Architecture Review

> **Date:** 2026-10-27
> **Scope:** `Assets/Scripts/` — Di, Networking (Domain, Adapters, Interlude, V2),
> Game, Controller, UI, and cross-cutting concerns.
> **Codebase size:** ~1 329 C# files, 145 MonoBehaviours, ~140 singleton instances.

---

## Executive Summary

The client is a working L2 Interlude-era MMO client being extended with a parallel
Fineage ZoneServer v2 protocol path. It already uses **VContainer** for DI and has a
nascent **Domain event layer** (`IWorldDomainSink`). These are strong foundations.

However, the transition from the upstream singleton-based design is incomplete.
A massive **static service locator** (`IncomingPacketActions`, 420 lines) stitches
everything together, ~140 classes expose `static Instance` singletons, and the
DI-injected path always falls back to those singletons. This creates hidden
coupling, makes isolated testing impossible, and will cause race conditions when
parallel agents touch overlapping files.

Below are the findings grouped by severity.

---

## 1. Findings

### P0 — Must fix before parallel work scales

#### P0-1: `IncomingPacketActions` is a 420-line global mutable static service locator

| Aspect | Detail |
|--------|--------|
| **File** | `Networking/ClientLibrary/Packet/Incoming/IncomingPacketActions.cs` |
| **Problem** | Every incoming packet handler, every UI window, every controller accesses services via `IncomingPacketActions.XYZ`. It holds ~40 static mutable fields bound at different lifecycle stages (`BindApp`, `BindLogin`, `BindGame`). Any change to the bind order, or any new field, risks breaking existing code silently. Used in **200+ call sites** across the project. |
| **Parallel risk** | Two agents adding different features will both add new static properties/accessors — merge conflicts on every other line. |
| **Recommendation** | Phase 1: freeze the public surface (no new members). Phase 2: replace usage site-by-site with `[Inject]` from VContainer scopes. Phase 3: delete. |

#### P0-2: 140+ singleton `static Instance` pattern coexists with DI

| Aspect | Detail |
|--------|--------|
| **Problem** | Nearly every MonoBehaviour (`World`, `GameManager`, `EventProcessor`, `PlayerController`, 20+ UI windows, 20+ DAT tables, etc.) has `static Instance { get; }` set in `Awake()`. VContainer registers them too, but every consumer falls back to the singleton: `_field != null ? _field : Foo.Instance`. |
| **Impact** | DI becomes cosmetic — nothing breaks if the container is removed. Testing with a mock container still hits global singletons. Scene load order dictates correctness. |
| **Recommendation** | Keep singletons as a migration bridge but add `[Obsolete]` to `Instance` properties. New code must not use `XXX.Instance`. |

#### P0-3: `GameManager` is a god object

| Aspect | Detail |
|--------|--------|
| **File** | `Game/Manager/GameManager.cs` (310 lines, 18 `[Inject]` fields) |
| **Problem** | Owns game state machine, lobby flow callbacks, table initialization, protocol toggle, scene loading orchestration. Every new lobby state or networking event adds more methods here. |
| **Parallel risk** | Central merge-conflict magnet. |
| **Recommendation** | Extract: (1) `TableInitializer` — the `LoadTables()` block, (2) `LobbyFlowController` — the 12 `On...()` callbacks, (3) leave `GameManager` as a thin lifecycle owner. See section 3 for target module map. |

---

### P1 — Should fix soon (blocks quality, not basic function)

#### P1-1: No assembly definitions for project scripts

| Aspect | Detail |
|--------|--------|
| **Problem** | All 1 329 scripts compile into a single `Assembly-CSharp.dll`. Only FMOD and Atmospheric Fog have `.asmdef` files. |
| **Impact** | (a) Every change recompiles everything. (b) No compile-time boundary enforcement — UI code can `new MonsterSpawner()` or reference `GameClient` directly. (c) Parallel agents trigger full recompiles on every touch. |
| **Recommendation** | Introduce `Fineage.Domain.asmdef`, `Fineage.Networking.asmdef`, `Fineage.Game.asmdef`, `Fineage.UI.asmdef`, `Fineage.Controller.asmdef`, `Fineage.Di.asmdef`. |

#### P1-2: Dual protocol check (`FineageWireConfig.IsZoneV2`) is inline `if` spaghetti

| Aspect | Detail |
|--------|--------|
| **Files** | `GameManager.cs` (3 checks), `GameClient.cs` (2 checks), `LoginWindow.cs` (1 check) |
| **Problem** | Protocol-specific branches are scattered as `if (FineageWireConfig.IsZoneV2)` inside methods that otherwise handle Interlude logic. Adding a third protocol or a feature flag will double the branch count. |
| **Recommendation** | `GameClient.Send()` should delegate to an `ITransport` strategy injected by DI, selected by `FineageWireConfig.Current`. The inline checks collapse. |
| **Progress** | PR #111 introduced `IZoneTransport` interface (`Networking/Adapters/V2/IZoneTransport.cs`) — the right abstraction for V2 transport. Next step: wire `GameClient.Send()` to delegate through it and remove inline `IsZoneV2` branches. |

#### P1-3: `GameRuntime` and `LoginRuntime` are hand-rolled service locators

| Aspect | Detail |
|--------|--------|
| **Files** | `Di/GameRuntime.cs` (130 lines, 48 properties), `Di/LoginRuntime.cs` (40 lines) |
| **Problem** | Every new UI window or manager must be added as a field + property + fallback-to-singleton. This is a second service locator in addition to `IncomingPacketActions`. |
| **Recommendation** | These should become simple `IObjectResolver` wrappers or be removed as consumers migrate to `[Inject]`. |

#### P1-4: `FineageWireConfig` uses mutable statics (`ZoneHost`, `ZonePort`, `_override`)

| Aspect | Detail |
|--------|--------|
| **File** | `Networking/Adapters/V2/FineageWireConfig.cs` |
| **Problem** | `public static string ZoneHost`, `public static int ZonePort` are shared mutable state. Parallel test harnesses or multiple login attempts will clobber each other. |
| **Recommendation** | Wrap in a `WireSettings` record injected via DI. Inspector toggle stays on `GameManager` but writes to the injected instance. |

#### P1-5: DAT tables are singleton ScriptableObjects registered as DI instances

| Aspect | Detail |
|--------|--------|
| **File** | `AppLifetimeScope.RegisterDatTables()` |
| **Problem** | `builder.RegisterInstance(ItemTable.Instance)` — the DI container holds the exact same object as the static singleton. No isolation possible. Tables like `NpcgrpTable` are initialized with side-effects (`Initialize(decoEffects)`) in `GameManager.LoadTables()`, not in DI lifecycle. |
| **Recommendation** | Move `Initialize()` calls into a DI `IStartable` or `RegisterBuildCallback`. Register tables as `Lifetime.Singleton` created via factory, not `RegisterInstance`. |

---

### P2 — Improve when convenient

#### P2-1: `EventBus` covers only 3 events, everything else is direct calls

| Aspect | Detail |
|--------|--------|
| **File** | `Game/Manager/GameEvent/EventBus.cs` |
| **Problem** | Only `OnEquipped`, `OnUnEquipped`, `OnLevelUp`. All other game events (spawn, die, move, skill cast, chat message) go through direct method calls on `WorldPacketApply`, `World`, or `IncomingPacketActions`. No unified event pipeline exists. |
| **Recommendation** | Expand `EventBus` into a typed pub/sub or use `MessagePipe` (VContainer ecosystem). New features should publish domain events, not add methods to `WorldPacketApply`. |

#### P2-2: Async/threading model uses `Task.Delay(150)` as entity-wait retry

| Aspect | Detail |
|--------|--------|
| **File** | `World.GetEntityAsync()` |
| **Problem** | Single 150ms retry with `Task.Delay` inside a method called from `async Task` chains. `lock(_objects)` inside an `async` method can cause subtle threading issues. Entity lookups from incoming packets should queue to main-thread, not poll. |
| **Recommendation** | Replace with `EventProcessor.QueueEvent` callback pattern (already used elsewhere). Eliminate `lock(_objects)` — dictionary is only accessed on main thread after queue dispatch. |

#### P2-3: `World.cs` is 840 lines — spawn/despawn/lookup all in one

| Aspect | Detail |
|--------|--------|
| **Problem** | `World` handles entity registration, spawning (delegates to spawners), deletion, fade-out, pooling, teleport, ground snapping, combat delegation, user info updates. |
| **Recommendation** | Extract `EntityRegistry` (register/unregister/lookup) and `EntityLifecycleManager` (spawn/despawn/fade/pool). Keep `World` as the scene-level coordinator. |

#### P2-4: `DiBootstrap.EnsureGameScope()` does `DestroyImmediate` + re-create

| Aspect | Detail |
|--------|--------|
| **File** | `Di/DiBootstrap.cs` |
| **Problem** | If the existing scope is "not ready", it destroys and rebuilds. This can orphan resolved references. |
| **Recommendation** | Add a `Dispose()` path or make scope creation idempotent. |

#### P2-5: Editor-only namespace leak in runtime code

| Aspect | Detail |
|--------|--------|
| **File** | `Game/Manager/GameEvent/EventBus.cs` line 3 |
| **Problem** | `using static UnityEditor.Progress;` — will fail in a standalone build. Currently benign because unused, but indicates lack of build verification. |
| **Recommendation** | Remove. Add a CI step that runs player build. |

---

### P3 — Minor / cosmetic

#### P3-1: Commented-out debug logging everywhere

Nearly every file has 5-20 commented-out `Debug.Log` calls. This clutters diffs and makes it hard to see real logic. Use a `FineageLog` conditional logger instead (partially introduced).

#### P3-2: Dead-code methods in `World.cs`

`UpdateObjectRotation` and `UpdateObjectMoveDirection` do effectively nothing in their action callbacks.

#### P3-3: Inconsistent naming

`EffectSkillsmanager` (lowercase 'm'), `AnimLeghtTable` (typo for 'Length'), `InventoryPapderDoll` (typo for 'Paperdoll'). Low priority but confusing.

---

## 2. Layering & Boundaries Analysis

### Current (implicit) layers

```
┌─────────────────────────────────────────────────────────┐
│ UI  (L2LoginUI, L2GameUI, ChatWindow, InventoryWindow…) │
├─────────────────────────────────────────────────────────┤
│ Controller (PlayerController, StateMachine, ClickMgr)    │
├─────────────────────────────────────────────────────────┤
│ Game (World, GameManager, Spawners, PlayerInventory…)    │
├─────────────────────────────────────────────────────────┤
│ Networking (GameClient, LoginClient, Packet handlers)    │
├─────────────────────────────────────────────────────────┤
│ Domain (IWorldDomainSink — nascent, 8 files)             │
├─────────────────────────────────────────────────────────┤
│ Database (DAT tables — ItemTable, NpcgrpTable, …)        │
└─────────────────────────────────────────────────────────┘
```

### Violations

1. **Networking → UI direct**: Incoming packet handlers (e.g. `PlayOkIncoming`, `CharSelectionInfoIncoming`) call `IncomingPacketActions.LoginUi.ShowXYZ()` — networking reaches directly into UI layer.
2. **UI → Networking direct**: `LoginWindow` calls `_loginClient.Connect()` and `_gameClient.Send(...)` — UI sends raw network commands.
3. **Game → Networking**: `World.SendValidatePosition()` calls `_gameClient.Send(...)` bypassing any transport abstraction.
4. **No domain model**: There is no `Player` domain entity separate from `PlayerEntity : MonoBehaviour`. Game state lives on GameObjects.

### Target layering (recommendation)

```
UI  ──→  Controller  ──→  Game/Domain  ──→  Networking
  (events only)     (commands)      (ITransport)
                                        ↓
                                   Protocol adapter
                               (Interlude / ZoneV2)
```

---

## 3. Dual Protocol (Interlude vs ZoneV2) Analysis

### Current state

| Concern | Interlude path | ZoneV2 path |
|---------|---------------|-------------|
| Login | `LoginClient` → `AsynchronousClient` → Blowfish + RSA | `ZoneV2Client.BeginSession()` → TCP raw |
| Auth | Full handshake (Init, RSA, PlayOk) | Hello → Auth → AuthOk |
| Send | `GameClient.Send()` → encrypt → queue | `ZoneV2Client.SendMove()` |
| Receive | `GameServerPacketHandler` → `IProtocol.TryParseGame` → `NetworkDispatcher` → `IncomingPacketAutoRegistry.Handle()` | `ZoneV2Client.RunSessionAsync()` → inline switch |
| World | `WorldPacketApply` methods | → `IWorldDomainSink.ApplyMoveTo()` → `WorldPacketApply` |

### Good

- `IWorldDomainSink` is the right abstraction — both protocols converge here.
- `DomainMoveTo`, `DomainPlayerSpawned`, etc. are proper domain DTOs.
- **PR #111** added `IZoneTransport` interface and `Docs/ARCHITECTURE_NET.md` — the V2 transport abstraction is in place.

### Problems

- `GameClient.Send()` has an inline `if (FineageWireConfig.IsZoneV2)` that silently drops all non-Move commands.
- `GameManager.OnWorldSceneLoaded()` has 3 `IsZoneV2` checks that skip EnterWorld/EndLoadWorld — these are lobby-flow branches, not transport details.
- `ZoneV2Client` directly touches `GameClient.Instance.PlayerInfo` and `IncomingPacketActions.Manager` — it should publish a domain event.
- `IZoneTransport` (from #111) exists but is not yet wired into `GameClient.Send()` — the inline branches remain.

### Recommendation

Wire `GameClient.Send()` to delegate through `IZoneTransport` (already defined)
for V2 commands. For Interlude, the existing send-queue path stays as-is.
The 6 inline `IsZoneV2` checks in GameManager/GameClient/LoginWindow should
collapse once the transport strategy is injected.

`GameManager` should never check `IsZoneV2` — transport details belong in the
adapter layer.

---

## 4. DI (VContainer) Analysis

### Scope hierarchy

```
AppLifetimeScope (DontDestroyOnLoad)
├── LoginLifetimeScope (Login scene)
└── GameLifetimeScope (Game scene)
```

### What's registered

| Scope | Registered via | Count | Style |
|-------|---------------|-------|-------|
| App | `RegisterComponent` (find-on-scene) | 4 | MonoBehaviour singletons |
| App | `Register<T>(Singleton)` | 12 | Pure C# singletons |
| App | `RegisterInstance` | 18 | DAT tables (singleton pass-through) |
| Game | `RegisterComponent` (find-on-scene) | ~45 | MonoBehaviour singletons |
| Game | `Register<T>(Singleton)` | ~25 | Pure C# services |
| Login | `RegisterComponent` (find-on-scene) | 9 | MonoBehaviour singletons |

### Problems

1. **No interfaces for most registrations** — `RegisterComponent<World>(builder)` registers the concrete type. Cannot substitute for tests.
2. **`RegisterBuildCallback` contains bind logic** — `IncomingPacketActions.BindApp(...)` in the callback means DI construction has side effects on a global static.
3. **`RegisterIfFound<T>` silently skips missing components** — if a prefab is misconfigured, no error; the inject field is just null, and the code falls back to `Instance`. Failures are invisible.
4. **Cross-scope resolution via `App.GameContainer`** — `App.InjectGameObject()` searches for `GameLifetimeScope` at runtime. This is a DI anti-pattern (containers should not be passed around).

### Testability score: 2/10

No tests exist. The DI structure would not support them without extracting interfaces for at least: `IGameClient`, `ILoginClient`, `IWorld`, `IGameManager`.

---

## 5. Parallel Agent Safety

### God objects (merge-conflict magnets)

| File | Lines | Inject fields | Static refs | Risk |
|------|-------|--------------|-------------|------|
| `IncomingPacketActions.cs` | 420 | 0 (all static) | 40 | **Critical** |
| `GameRuntime.cs` | 130 | 0 (ctor inject) | 48 props | High |
| `GameManager.cs` | 310 | 18 | self | High |
| `World.cs` | 840 | 21 | self | High |
| `GameLifetimeScope.cs` | 140 | 0 | — | Medium |

### Shared mutable statics

| Static | File | Mutated at | Risk |
|--------|------|-----------|------|
| `FineageWireConfig._override` | Adapters/V2 | `GameManager.Awake` | Medium |
| `FineageWireConfig.ZoneHost/Port` | Adapters/V2 | Any caller | Medium |
| `FineageDev.AutoAdvanceLobby` | Utils | Inspector / code | Low |
| `IncomingPacketActions.*` | Networking | 3 Bind methods | **Critical** |
| `140× Foo.Instance` | Everywhere | Each Awake | High |

### Scene/prefab coupling

- `World.Awake()` does `GameObject.Find("Npcs")`, `Find("Monsters")`, etc. — fragile name-based lookup.
- `DiBootstrap.EnsureGameScope()` does `FindFirstObjectByType<World>()` to decide whether to create scope — scene structure dictates DI lifetime.

---

## 6. Async / Threading

### Current model

```
Network thread (AsynchronousClient)
  ↓ ConcurrentQueue (IncomingGameQueue)
  ↓ EventProcessor.Update() [main thread, double-buffered]
  ↓ PacketApplyQueue.QueueApply()
  ↓ handler.ApplyModel() [main thread]
```

### Good

- Double-buffered queue in `EventProcessor` prevents starvation.
- `PacketApplyQueue` handles re-entrant apply with `_applyDepth`.
- `ZoneV2Client` uses `ConcurrentQueue<Action>` with `Update()` pump.

### Problems

- `World.GetEntityAsync()` uses `lock(_objects)` + `Task.Delay(150)`. The dictionary is only mutated on the main thread, so the lock is unnecessary. The delay is a poll hack.
- `GameClient.Connect()` is `async void` — exceptions are swallowed.
- `DefaultClient.Connect()` catches exceptions from `Task.Run(_client.Connect)` but logging may fail if `EventProcessor` is null.

---

## 7. Target Module Map

Recommended assembly / namespace boundaries after refactoring:

```
Fineage.Domain/              (no Unity deps except Vector3)
  ├── Events/                DomainMoveTo, DomainPlayerSpawned, ...
  ├── Models/                EntityIdentity, PlayerStats, ItemInstance, ...
  └── Ports/                 ITransport, IWorldDomainSink

Fineage.Networking/          (depends on Domain)
  ├── Protocol/              IProtocol, InterludeProtocol
  ├── Adapters/
  │   ├── Interlude/         GameClient, LoginClient, packet handlers
  │   └── V2/               ZoneV2Client, ZoneV2FrameIO
  ├── AutoRegistry/          IncomingPacketAutoRegistry
  └── Queue/                 IncomingGameQueue, SendGameDataQueue, ...

Fineage.Game/                (depends on Domain, Networking)
  ├── Manager/               World, EntityRegistry, EntityLifecycle
  ├── Spawn/                 PlayerSpawner, NpcSpawner, ...
  ├── Combat/                WorldCombat, HitManager, ...
  ├── Apply/                 WorldPacketApply, MessagePacketApply
  └── Table/                 ItemTable, NpcgrpTable, ... (or Fineage.Data)

Fineage.Controller/          (depends on Game)
  ├── Player/                PlayerController, PlayerStateMachine
  ├── Entity/                EntityActionMachine
  └── Input/                 InputManager, ClickManager

Fineage.UI/                  (depends on Game, Controller)
  ├── Login/                 L2LoginUI, LoginWindow, ...
  ├── Game/                  L2GameUI, ChatWindow, InventoryWindow, ...
  └── Shared/                L2Slot, L2Tooltip, Manipulators

Fineage.Di/                  (depends on all above)
  ├── AppLifetimeScope
  ├── GameLifetimeScope
  └── LoginLifetimeScope
```

---

## 8. Refactoring Roadmap

### Phase 1 — Guardrails (this PR + next sprint)

- [x] Document architecture boundaries (this review)
- [x] Extract `IGameManager` interface for networking entry points *(PR #127)*
- [x] Remove `using static UnityEditor.Progress` from `EventBus.cs` *(PR #127)*
- [x] Add boundary comments / `#region` markers to `GameManager.cs` *(PR #127)*
- [ ] Freeze `IncomingPacketActions` surface → [#137](https://github.com/FioraSolutions/Fineage/issues/137)

### Phase 2 — Assembly definitions (1–2 sprints)

→ [#139](https://github.com/FioraSolutions/Fineage/issues/139)

- [ ] Create `.asmdef` files for Domain, Networking, Game, UI, Controller, Di
- [ ] Fix resulting compile errors (these reveal real dependency violations)
- [ ] Gate PRs on assembly-level compile check

### Phase 3 — Protocol abstraction (2 sprints)

→ [#140](https://github.com/FioraSolutions/Fineage/issues/140)

- [x] Define `IZoneTransport` interface *(PR #111)*
- [ ] Wire `GameClient.Send()` to delegate V2 commands through `IZoneTransport`
- [ ] Move `ZoneV2Client` receive loop to use `IWorldDomainSink` exclusively
- [ ] Remove all `FineageWireConfig.IsZoneV2` inline checks from GameManager/GameClient/LoginWindow

### Phase 4 — Service locator elimination (ongoing)

→ [#141](https://github.com/FioraSolutions/Fineage/issues/141) (GameManager decompose), [#142](https://github.com/FioraSolutions/Fineage/issues/142) (IncomingPacketActions migration)

- [x] Extract GameManager → TableInitializer + LobbyFlowController
- [ ] Convert `IncomingPacketActions` usages to `[Inject]` one subsystem at a time
- [ ] Remove `GameRuntime` / `LoginRuntime` as consumers migrate
- [ ] Remove `static Instance` from classes as last consumer migrates

### Phase 5 — Domain events (ongoing)

→ [#143](https://github.com/FioraSolutions/Fineage/issues/143)

- [ ] Introduce `MessagePipe` or expand `EventBus`
- [ ] New features use domain events, not direct calls
- [ ] Existing features migrate opportunistically

**Epic issue:** [#131](https://github.com/FioraSolutions/Fineage/issues/131)

---

## Appendix A — Singleton Instance Count

Total classes with `static … Instance`: **~140**

Top categories:
- UI Windows: ~35
- DAT Tables: ~20
- Managers (World, GameManager, EffectManager, ...): ~15
- Controllers: ~10
- Networking: ~5
- Misc: ~55

---

## Appendix B — `IncomingPacketActions` usage hotspots

| File | References |
|------|-----------|
| `L2LoginUI.cs` | 34 |
| `GameManager.cs` | 32 |
| `CharCreationWindow.cs` | 30 |
| `EntityActionVisual.cs` | 26 |
| `EntityActionSkill.cs` | 17 |
| `PlayOkIncoming.cs` | 11 |
| `RunningState.cs` | 10 |
| `UserInfoIncoming.cs` | 9 |
