# Fineage Net (client)

Unity path for `PacketEnvelope` Net. MessagePack Zone V2 is retired; Fineage Net is the only live wire.

Server wire SoT: [Fineage-Backend `docs/net-protocol.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/net-protocol.md). Doc triage: [DOC_CANON.md](DOC_CANON.md).

## Wire

- Fineage Net is the only wire. There is no protocol mode switch: `FineageWireConfig` only carries
  the login/zone hosts (`Networking/Net/Domain/FineageWireConfig.cs`).
- Editor Play defaults to loopback (`EnableLocalNetBackend()`); a build uses the public
  tunnel (`EnablePlaytestBackend`). `FINEAGE_NET_CLIENT=0` pins a public-playtest run
  while developing locally. See `GameManager.ConfigureBackend`.
- Login **9106**, Zone **9777** (forwarded / loopback). Public Pi remap: **2106** / **7777**.

## Types

| Path | Role |
|------|------|
| `Assets/Scripts/Networking/Net/Contract/**` | Verbatim copy of Backend `Fineage.Net.Contract`, kept in sync by `scripts/sync-net-contract.py` |
| `NetTransport.cs` | TCP + `NetFraming` |
| `NetSession.cs` | Lobby + zone session → domain sink / spawn / move |
| `FineageZoneTransport` / `IZoneTransport` | Sole zone command path (DI + `ZoneCommands.Transport` fallback) |
| `Net/Domain/Net{Entity,Combat,Gear,Social}Map.cs` | Maps Net contract payloads → Interlude-shaped apply DTOs (`PlayerInfoInterlude`, move/chat/combat DTOs) for `IWorldDomainSink` / `WorldPacketApply` (ex-`NetDomainMap`) |

The presentation layer is still Interlude-era (client protocol identity **746**, `ClientLibrary` DTOs). Net is a custom envelope into that apply path — not a second wire and not “no DTO bridge.”

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

Backend durable store: PostgreSQL via `FINEAGE_DATABASE_URL` (see Backend `docker-compose.yml` / `scripts/run-local-net.sh`). There is no file-backed account store.
