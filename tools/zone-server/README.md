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
dotnet run -- --selftest   # Hello→Auth→EnterWorld→Move smoke
```

## Frame

```
uint16 LE totalLength (includes 4-byte header)
uint16 LE msgType
MessagePack body
```

See `Wire/MsgType.cs` and `Wire/Messages.cs`.

## Next

- TLS on the channel
- Separate login listener
- Client `Adapters/V2` → `IWorldDomainSink`
- Interest / tick loop
