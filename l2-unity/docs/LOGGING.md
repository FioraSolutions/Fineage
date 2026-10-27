# Fineage logging

## Where logs live

| Log | Path | What |
|---|---|---|
| Lobby / login handshake | `l2-unity/logs/diag/lobby-flow.txt` (+ `.1` backup) | Confirm → PlayOk → AuthLogin → CharSelectionInfo |
| Game net | `l2-unity/logs/diag/game-net.txt` | Optional GameNet category |
| Login stub (server) | `tools/login-stub/logs/stub.log` | Init → LoginOk → PlayOk → EnterWorld → Move |
| Zone server v2 | `tools/zone-server/logs/zone.log` | Hello → Auth → EnterWorld → MoveTo |
| Unity Editor | `~/Library/Logs/Unity/Editor.log` (macOS) | Engine + URP; can grow huge |

Console filters: `[LOBBY]`, `[LoginNet]`, `[GameNet]`, `[ZoneV2]`, `[World]`, `[STUB]`, `[ZONE]`.

Files rotate at ~3MB (one `.1` backup kept). `logs/` dirs are gitignored.

## Levels

`FineageLog` / `LobbyFlowLog` / `StubLog` / `ZoneLog`: **Error**, **Warn**, **Info**, **Debug**.

- Info/Warn/Error: always on for milestones.
- Debug: off by default.

### Enable Debug

**Client**

1. **Runtime:** `FineageLog.DebugEnabled = true;` (e.g. from a debug menu / console).
2. **Compile:** add scripting define `FINEAGE_LOG_DEBUG` in Player Settings.

**Servers**

```bash
# login-stub — opcode/len + hex snippets
cd tools/login-stub
dotnet run -- --verbose
# or: FINEAGE_STUB_DEBUG=1 dotnet run

# zone-server
cd tools/zone-server
dotnet run -- --verbose
# or: FINEAGE_ZONE_DEBUG=1 dotnet run
```

## What we log (milestones only)

**Client:** Connect/disconnect, RequestAuthLogin, RequestServerLogin, PlayOk, InterludeKey/AuthLogin, CharSelectionInfo, EnterWorld, failures with reason — **not** per-opcode every frame.

**login-stub (`[STUB]`):** listen bind, login/game connect+disconnect (remote endpoint), Init → AuthGameGuard → RequestAuthLogin → LoginOk → ServerList → RequestServerLogin → PlayOk, then game AuthLogin / CharSelectionInfo / CharacterSelect / EnterWorld / Move echoes. Session id on login lines; account when known on game lines. No hex unless `--verbose`.

**zone-server (`[ZONE]`):** bind, connect/disconnect, Hello / Auth / EnterWorld / MoveTo ↔ MoveToAck (with session id + account).

## URP / Editor.log spam

Custom L2 passes implement `RecordRenderGraph` (URP 17 Render Graph). Compatibility Mode is **off** in Graphics settings.

If `Editor.log` is already huge after old spam:

```bash
: > ~/Library/Logs/Unity/Editor.log
# or
rm ~/Library/Logs/Unity/Editor.log
```

(Unity recreates it on next run.)

## See also

`RUN.md` — local login stub flow.
