# Fineage Net (client)

Unity path for `PacketEnvelope` Net. MessagePack Zone V2 is retired; Fineage Net is the only live wire.

Server wire SoT: [Fineage-Backend `docs/net-protocol.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/net-protocol.md). Doc triage: [DOC_CANON.md](DOC_CANON.md).

## Wire

- Fineage Net is the only wire. There is no protocol mode switch: `FineageWireConfig` only carries
  the login/zone hosts (`Networking/Net/Domain/FineageWireConfig.cs`).
- Two environments, one switch: `FineageServerProfile` / `FineageBackendSelector`.
  - **Local** — Fineage-Backend on this machine, `127.0.0.1:9106` / `:9777`
    (`EnableLocalNetBackend()`). The **Editor default**, so Play reaches the server you are
    debugging.
  - **Remote** — the shared playtest tunnel (`EnablePlaytestBackend`). The **player-build
    default**, and what GitHub Actions ships.
  - `Fineage/Play Remote (playtest)` / `Fineage/Play Local` switch Play; the choice is
    remembered in EditorPrefs and a tick marks the active one.
- Overrides, highest first: `-fineageBackend local|remote` / `--fineage-backend=…`,
  `FINEAGE_BACKEND=local|remote`, legacy `FINEAGE_NET_CLIENT=1|0`, stored Editor choice,
  host default (Local in the Editor, Remote in a player). Local is only honoured by the
  Editor or a build compiled with `FINEAGE_LOCAL_CAPABLE` (Fineage → Build Dev Mac); a
  release-clients player ignores all of them and stays remote. See
  `GameManager.ConfigureBackend`.
- Login **9106**, Zone **9777** (loopback / local Fineage-Backend). Public playtest on the Debian homeserver (DDNS + router forward): **2106** / **7777** — see [`PLAYTEST_RELEASE.md`](PLAYTEST_RELEASE.md).

## Types

| Path | Role |
|------|------|
| `Assets/Scripts/Networking/Net/Contract/**` | Verbatim copy of Backend `Fineage.Net.Contract`, kept in sync by `scripts/sync-net-contract.py` |
| `NetTransport.cs` | TCP + `NetFraming` |
| `NetSession.cs` | Lobby + zone session → domain sink / spawn / move |
| `FineageZoneTransport` / `IZoneTransport` | Sole zone command path (DI + `ZoneCommands.Transport` fallback) |
| `NetSession.cs`, `Net/Domain/NetGearSession.cs` | Net contract → Interlude-shaped apply DTOs (`PlayerInfoInterlude`, CharInfo/NpcInfo, inventory, skills) via colocated helpers; paperdoll overlay via `NetPaperdollApply` + slot indexes on `NetPaperdoll` (`NetDomainStructs.cs`) |

The presentation layer is still Interlude-era (client protocol identity **746**, `ClientLibrary` DTOs). Net is a custom envelope into that apply path — not a second wire and not “no DTO bridge.”

## Flow

1. Login → `NetSession.BeginLobby` (Hello → Auth → Roster UI).
2. CharSelect Start → `SelectCharacter` → `Handoff` → zone Hello/Auth/EnterWorld.
3. `EntitySpawn` (self) + `PlayerStats` + `ItemList` / `SkillList` → local enter; remote player `EntitySpawn` → CharInfo; NPC `EntitySpawn` (`Kind=Npc`) → NpcInfo.
4. Click move → `FineageZoneTransport.SendMove` → `MoveInput` → `MoveState` (self + peers). `EntityDespawn` removes forgotten peers/NPCs. NPC chase also sends `MoveState`.
5. Click NPC → `SelectTarget` / `AttackRequest` → `TargetSelected` / `AttackState` / `StatusUpdate`.
6. Kill → `EntityDie` + `DropItem`; corpse decay / leave → `EntityDespawn`.
7. Click ground loot → `PickupRequest` → `InventoryUpdate` + `EntityDespawn`.
8. Bag drop / equip / unequip → `DropItemRequest` / `UseItem` / `UnequipItem` → `InventoryUpdate` (+ `DropItem` for drops). The client keeps one inventory book (`CharacterInventory`), keyed by objectId. `ItemList` replaces it. `InventoryUpdate` applies add (0) / modify (1) / remove (2) in order. The bag, paperdoll, shortcut lookup, adena, and slot count are projections of that book. Weight stays the server load on `PlayerStats` (`StatusUpdate` / `UserInfo`). `ItemList` and `InventoryUpdate` are applied unversioned until the Net contract defines an inventory version (`Fineage-Backend` `docs/inventory.md`). A little-endian int32 appended after either body is left unread: it is not a row and does not change object ids, counts, or slots. A gap or an unknown change does not change the book; the client sends Interlude `RequestItemList` (opcode `0x0F`, empty body) and waits for a new `ItemList`.
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
