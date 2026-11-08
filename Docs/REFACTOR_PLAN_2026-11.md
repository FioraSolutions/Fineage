# Fineage refactor plan (2026-11)

Unified plan from architecture reviews (GPT + DeepSeek), reconciled with epics **#131** (client arch) and **#112** (net scale), `AGENTS.md`, and code on `dev` as of **2026-11-08**.

**Do not** chase 20k CCU / Gateway / Redis yet. Stabilize **one Zone** + client apply path first. Prefer #131 before packing more #129 Gremlin surface onto the Unity monolith.

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

## GPT review verification (2026-11-08)

Static re-check of the long GPT “client + server” architecture review against both `dev` tips. Verdict: **direction mostly right; several P0/P1 claims are already fixed or overstated.**

### Keep (still true)

| Claim | Evidence |
|-------|----------|
| Fineage Net (not L2 wire / not MsgPack V2) is the right port strategy | Backend README + `net-protocol.md`; client Net-only |
| Login + Zone + HMAC handoff + Postgres SoT | Live |
| `ZoneWorld` / `NetZoneServer` / `NetCodec` are oversized hubs | ~1.4k + many partials; NetZoneServer ~2.9k; NetCodec ~3.5k |
| Global `_gate` on `ZoneWorld` is a future bottleneck; prefer single-writer sim later | Still `lock (_gate)` across world methods |
| `ZoneEntity` is a player+NPC+ground god bag | Defined in `ZoneWorld.cs` (~400+ fields mixed by kind) |
| `ZoneWorld` → `INetPresenceBridge` (in `Fineage.Zone.Net`) couples domain to net | Still set from `NetZoneServer` |
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
| `#34` Unity CI closed ⇒ Unity PR gate exists | **No:** issue closed; PR still does not batchmode-compile Unity |

### Priority remap after verification

1. **Before public internet:** TLS / Gateway (#17 residual, #113) + rate-limit login brute force (still open).
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
| `NetZoneServer` handlers | Opcode groups → `*PacketHandler` (decode → command → world); keep projection/fanout separate |
| Domain events | Replace `INetPresenceBridge` dependency on world with `IWorldEventPublisher` (or move interface into Game/Zone.World) |
| `ZoneWorld` systems | New gameplay goes into focused types (`*System` / `*Rules` / `*Service`); stop growing the partial bag when avoidable |
| `ZoneEntity` composition | Split player / NPC / ground payloads over time (composition, not deep Java-style inheritance) |
| `NetCodec` | Split by domain **or** keep monolith + mandatory round-trip tests per message (generation optional later) |

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

### B5. State recovery — later
- EnterWorld snapshot + revision; resync / reconnect path when Critical outbound was dropped.

### B6. Economy invariants — open (tests)
- Trade / inventory / adena: atomic apply + persistence tests (dup-item / double-spend). Not claiming a live bug; GPT correctly flagged the invariant set.

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
- [ ] Further asmdef slices for remaining MonoBehaviours / LifetimeScope when types are cycle-safe.

### C3. Doc sync
- [x] Strip “Zone v2 / MessagePack” wording from `NetDomainMap`, `IWorldDomainSink`, stale comments.

---

## Phase D — Scale (#112) — later

Gateway + TLS, multi-zone ownership, Redis session directory / pub-sub, distributed presence, load bots + AOI profiles. Only after Phase A–B metrics say one Zone is healthy.

---

## Execution order for autonomous agents (next)

| Track | Deliverable |
|-------|-------------|
| Docs | This plan + `DOC_CANON` / `ARCHITECTURE_NET` / `CI_AND_TESTING` stay honest about “done vs later” |
| Backend B2b | Continue handler/system extracts; domain events (`IWorldEventPublisher`); epic [Fineage-Backend #353](https://github.com/FioraSolutions/Fineage-Backend/issues/353). Opcode groups largely extracted — residual = session lifecycle (Hello/Auth/Enter) + knownlist fanout on `NetZoneServer` |
| Client #131 | More asmdef moves; LifetimeScope into `Fineage.Di` only when cycle-safe (no big-bang Instance purge) |
| A6 | TLS before non-VPN public auth (#17 / #113) |
| CI follow-up | Unity PR compile/EditMode — [#737](https://github.com/FioraSolutions/Fineage/issues/737) (replaces closed #34) |

**Out of scope now:** Redis, multi-zone, QUIC, 20k claims, ECS rewrite, microservices.

## Success metrics

- [x] External bind without `FINEAGE_HANDOFF_SECRET` fails closed
- [x] Deduper rejects pruned-window replays
- [x] Client CI fails if Net contract drifts vs Backend
- [x] `NetDomainMap.cs` and frozen `Net*Map` adapters gone (`check-frozen-net-domain-maps.py`)
- [x] No new world DB I/O under `_gate` on enter (Phase B1)
- [ ] TLS (or equivalent) before public auth
- [ ] New Zone features land outside `NetZoneServer` / `ZoneWorld` god files when practical
- [x] Real `Fineage.*` asmdef content for migrated types; `Fineage.Di` more than a stub (`ILateBound`)
- [ ] LifetimeScope / gameplay MonoBehaviours in `Fineage.*` assemblies when cycle-safe
