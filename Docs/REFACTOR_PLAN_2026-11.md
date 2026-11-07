# Fineage refactor plan (2026-11)

Unified plan from the GPT + DeepSeek architecture reviews, reconciled with epics **#131** (client arch) and **#112** (net scale), `AGENTS.md`, and current code (2026-11-07).

**Do not** chase 20k CCU / Gateway / Redis yet. Stabilize **one Zone** + client apply path first. Prefer #131 before packing more #129 Gremlin surface onto the Unity monolith.

## North star (keep)

- Login + Zone processes, Fineage Net only, server-authoritative simulation
- AOI + presence hub, PostgreSQL durable store, Mobius Interlude parity
- Small PRs into `dev`; green CI; no inventing HUD when `ref/` originals exist

## Phase map

| Phase | Goal | When |
|-------|------|------|
| **A — Harden** | Auth, dedupe, delivery classes, contract CI truth | Now (before public playtest) |
| **B — Simulate cleanly** | DB out of world lock; main-thread budget; NetSession shrink | Next |
| **C — Client domain** | Dissolve `NetDomainMap`; DI / asmdef / Instance drain (#131 residual) | Parallel with B, epic #131 |
| **D — Scale** | Simulation actor, multi-zone, Gateway+TLS, Redis sessions (#112–#114) | After A–C prove one Zone |

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

### B3. Client main-thread budget — done
- Cap `/frame` work from `NetSession` `_main` queue; coalesce MoveState; never drop EntityDespawn / inventory.

### B4. NetSession decomposition — partial
- [x] `NetCommandSender` + zone `Send*` return `Task` (async void wrappers removed).
- [x] Lobby flow extracted to `NetLobbyFlow` (`BeginLobby`, roster CRUD, lobby dispatch, handoff enter orchestration).
- [x] `NetSession` split into partials (`NetSession.ZoneConnect`, `NetSession.ZoneDispatch`, `NetSession.ZoneApply`, `NetSession.Commands`).
- Transport / Session / shared outbound pump as separate types: later (when measured).

### B5. State recovery — later
- EnterWorld snapshot + revision; resync / reconnect path when Critical outbound was dropped.

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
- [x] First real asmdef content: `Fineage.Networking` (NetPaperdoll **#684** + engine-free Net domain batch).
- [ ] Further asmdef slices for `Fineage.Game` / `UI` / `Controller` / `Di` when types are cycle-safe (stubs remain).

### C3. Doc sync
- [x] Strip “Zone v2 / MessagePack” wording from `NetDomainMap`, `IWorldDomainSink`, stale comments.

---

## Phase D — Scale (#112) — later

Gateway + TLS, multi-zone ownership, Redis session directory / pub-sub, distributed presence, load bots + AOI profiles. Only after Phase A–B metrics say one Zone is healthy.

---

## Execution order for autonomous agents (this pass)

| Track | Owner | Deliverable |
|-------|-------|-------------|
| Plan + canon link | Primary | This doc + `DOC_CANON` pointer |
| Backend A1 + A3 | Cloud / local branch | PR → Backend `dev` |
| Backend A4 outbound wiring + reject sanitize | Follow-up PR | Small |
| Client C1 slice 1 | Cloud | Split NetDomainMap → adapters, delete facade if empty |
| Client A5 | Cloud / local | Workflow checks out Backend for contract sync |
| A2 one-time handoff | Next PR after A1 | Needs store design |

**Out of scope this pass:** TLS Gateway, Redis, multi-zone, full NetSession rewrite, 20k claims.

## Success metrics

- External bind without `FINEAGE_HANDOFF_SECRET` fails closed
- Deduper rejects pruned-window replays
- Client CI fails if Net contract drifts vs Backend
- `NetDomainMap.cs` and frozen `Net*Map` adapters gone (`check-frozen-net-domain-maps.py`)
- No new world DB I/O under `_gate` on enter (Phase B)
