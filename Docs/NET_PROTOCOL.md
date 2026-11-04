# Fineage Net (client)

Parallel client path for PacketEnvelope Net — **no** `ZoneV2DomainMap` / Interlude DTO bridge.

## Mode

- `FineageWireConfig.Mode.Net` via `EnableLocalNetBackend()` or env `FINEAGE_NET_CLIENT=1` (see `GameManager.Awake`).
- Login **9106**, Zone **9777** (forwarded / loopback). Legacy V2 uses 9206/9877.

## Types

| Path | Role |
|------|------|
| `Assets/Scripts/Networking/Net/Contract/**` | Verbatim copy of Backend `Fineage.Net.Contract`, kept in sync by `scripts/sync-net-contract.py` |
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
8. Bag drop / equip / unequip → `DropItemRequest` / `UseItem` / `UnequipItem` → `InventoryUpdate` (+ `DropItem` for drops).
9. Chat → `Say` / `CreatureSay`; welcome + notices → `SystemMessage`.
10. Skills → `UseSkill` / `MagicSkillUse` (+ `StatusUpdate` for MP/heal).
11. Death → `ReviveRequest` / `Revive` / `TeleportToLocation` (village restart).
12. Kill reward → `SystemMessage` + `StatusUpdate` + `PlayerStats` (exp/sp).
13. Buffs → `AbnormalStatusUpdate`.
14. NPC dialog → `NpcTalk` / `BypassRequest` / `NpcHtml`.
15. Shop → `BuyList` / `SellList` / `MultiSellList` / `ShopBalance` + buy/sell/multisell requests.
16. Warehouse → deposit/withdraw lists + requests.
17. Quests → `RequestQuestList` / `RequestQuestAbort` / `QuestList` (dialog HTML still `NpcHtml`).
18. Party → invite/answer/leave/kick + `PartyAskJoin` / `PartyJoinResult` / `PartyMemberList`.
19. Trade → request/answer/add/done + ask/start/offer/confirm/finished.
20. AOI on the zone server: `FINEAGE_ZONE_AOI_CELL` (grid) or whole-zone SingleCell by default.

Editor Play defaults to local Net. The legacy V2 MessagePack codec has been retired; the payload rows it used to decode live in `Net/Domain/ZoneV2DomainStructs.cs` and only the Net path is live.

Backend durable store: `FINEAGE_STORE=postgres` + `FINEAGE_DATABASE_URL` (see `docker-compose.yml` / `scripts/run-local-net.sh`).
