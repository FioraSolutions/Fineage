# Fineage login+game stub (Interlude wire)

Local Blowfish login on **2106** and game on **7777** for the Unity client.

## Run

```bash
cd tools/login-stub
dotnet run                 # listen 127.0.0.1:2106 + :7777
dotnet run -- --verbose    # Debug (opcode/len); FINEAGE_STUB_DEBUG=1 also works
dotnet run -- --selftest   # crypto/packet unit checks
dotnet run -- --clienttest # needs stub already listening in another process
```

## Logs

Console `[STUB]` + `logs/stub.log` (~3MB rotate). Info milestones cover the full login → EnterWorld → Move path; verbose adds opcode/len and short hex. Details: `l2-unity/docs/LOGGING.md`.

## Character create (stub)

Account-local roster lives in `Fineage.ZoneServer.Accounts.AccountCharacterStore` (in-memory, no DB). Game opcodes:

- `NewCharacter` (0x0E) → `CharTemplate` (0x17) with **Human Fighter** only
- `CharacterCreate` (0x0B) → `CharCreateOk` (0x19) + refreshed `CharSelectionInfo` (0x13)

Fresh accounts start with an empty char list; use the client Create flow or let `--clienttest` create `Test` automatically.
