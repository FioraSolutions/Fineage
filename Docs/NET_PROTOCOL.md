# Fineage Net (client)

Parallel client path for PacketEnvelope Net — **no** `ZoneV2DomainMap` / Interlude DTO bridge.

## Mode

- `FineageWireConfig.Mode.Net` via `EnableLocalNetBackend()` or env `FINEAGE_NET_CLIENT=1` (see `GameManager.Awake`).
- Login **9106**, Zone **9777** (forwarded / loopback). Legacy V2 uses 9206/9877.

## Types

| Path | Role |
|------|------|
| `Assets/Scripts/Networking/Net/Contract/**` | Symlinked sources from Backend `Fineage.Net.Contract` |
| `NetTransport.cs` | TCP + `NetFraming` |
| `NetSession.cs` | Lobby + zone session → `IWorldDomainSink` / spawn / move |

## Flow

1. Login → `NetSession.BeginLobby` (Hello → Auth → Roster UI).
2. CharSelect Start → `SelectCharacter` → `Handoff` → zone Hello/Auth/EnterWorld.
3. `EntitySpawn` (self) + `PlayerStats` + `ItemList` / `SkillList` → local enter; remote player `EntitySpawn` → CharInfo; NPC `EntitySpawn` (`Kind=Npc`) → NpcInfo.
4. Click move → `FineageZoneTransport.SendMove` → `MoveInput` → `MoveState` (self + peers). `EntityDespawn` removes forgotten peers/NPCs. NPC chase also sends `MoveState`.
5. Click NPC → `SelectTarget` / `AttackRequest` → `TargetSelected` / `AttackState` / `StatusUpdate`.
6. Kill → `EntityDie` + `DropItem`; corpse decay / leave → `EntityDespawn`.
7. Click ground loot → `PickupRequest` → `InventoryUpdate` + `EntityDespawn`.
8. AOI on the zone server: `FINEAGE_ZONE_AOI_CELL` (grid) or whole-zone SingleCell by default.

Editor Play defaults to local Net. V2 MessagePack adapters stay in `Adapters/V2` as reference; enable only with scripting define `FINEAGE_V2_WIRE`.

Backend durable store: `FINEAGE_STORE=postgres` + `FINEAGE_DATABASE_URL` (see `docker-compose.yml` / `scripts/run-local-net.sh`).
