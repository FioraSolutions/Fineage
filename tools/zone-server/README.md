# Fineage ZoneServer (wire v2 skeleton)

Minimal .NET zone process for the modern protocol path. Speaks **length-prefix + MessagePack**, not Interlude Blowfish/XOR.

Maps to client domain events from `#3` (`PlayerSpawned`, `MoveTo`, …). Unity dual-adapter is **not** wired yet — use `--selftest` or a future v2 client flag.

## Ports

| Role | Port | Notes |
|------|------|--------|
| Interlude stub | 2106 / 7777 | `tools/login-stub` (unchanged) |
| ZoneServer v2 | **9777** (game) | Auth on same socket for MVP; 9106 reserved |

## Run

```bash
cd tools/zone-server
dotnet run                 # listen 127.0.0.1:9777
dotnet run -- --verbose    # Debug lines (FINEAGE_ZONE_DEBUG=1 also works)
dotnet run -- --selftest   # Hello→Auth→EnterWorld→Move smoke
```

## Logs

Milestone lines go to the console (`[ZONE]`) and `logs/zone.log` (rotate ~3MB). Info covers Hello → Auth → EnterWorld → Move echoes; `--verbose` / `FINEAGE_ZONE_DEBUG=1` enables Debug. See `l2-unity/docs/LOGGING.md`.

## Frame

```
uint16 LE totalLength (includes 4-byte header)
uint16 LE msgType
MessagePack body
```

See `Wire/MsgType.cs` and `Wire/Messages.cs`.

## Account-local characters (Interlude stub)

`Accounts/AccountCharacterStore` holds per-account characters for the **login-stub** Interlude game port (no DB). Human Fighter create path only; wired from `tools/login-stub`.

## v0.4 presence (thin)

On `EnterWorld`, the server sends `NpcInfo` / `CharInfo` for entities in grid AOI (3×3 cells, 3000 L2 units). `MoveTo` updates AOI and emits `DeleteObject` when entities leave visibility. Static TI NPCs are hardcoded in `ZoneWorld.SeedTalkingIslandNpcs()`.

## v0.5 combat (thin)

`SelectTarget` → `MyTargetSelected` (Interlude color hint). `AttackRequest` applies a flat melee hit when in range to the nearby sword mob (`objectId=20003`, Gremlin template). Server echoes `Attack` + `StatusUpdate` with `CUR_HP` / `MAX_HP` (ids `0x09` / `0x0a`). No skills, inventory, or full formulas.

## Next

- TLS on the channel
- Separate login listener
- Client `Adapters/V2` → `IWorldDomainSink`
- Tick loop / AI
