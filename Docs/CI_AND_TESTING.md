# CI & Testing Guide

## GitHub Actions CI

### Workflow: `build-and-test`

**File:** `.github/workflows/build-and-test.yml`

**Triggers:**
- Pull request → `dev`
- Push to `feat/**`, `fix/**`, `chore/**` branches

**What it does:**
1. Restores, builds, and tests `tools/Fineage.Tools.sln` (Release configuration)
2. Uploads test results as artifacts (`.trx` format, 14 day retention)

**Required status check name:** `build-and-test`

### Workflow: `format-check`

Optional formatting verification using `dotnet format --verify-no-changes`.
Not required for merge (advisory only).

### Workflow: `release-clients`

**File:** `.github/workflows/release-clients.yml`

Builds the playtest clients, plus `version.json` and `CHANGELOG.md`. It does not run on pull requests. A `playtest-*` or `v*` tag on a commit that contains the workflow publishes a GitHub Release without waiting for `main`. Windows is published first; macOS and Linux are attached after. Setup, secrets, and how a landing page reads the release assets are in [PLAYTEST_RELEASE.md](PLAYTEST_RELEASE.md).

`build-and-test` also runs the playtest helper self-tests (`scripts/check-release-secrets.py`, `scripts/bake-playtest-endpoint.py`, `scripts/release-notes.py`, `scripts/pack-client.py`) and `tools/playtest-endpoint.Tests`. Those do not start Unity.

---

## Branch Protection Setup (Manual)

After this PR merges, enable branch protection on `dev` in GitHub:

1. Go to **Settings → Branches → Add branch protection rule**
2. Branch name pattern: `dev`
3. Enable:
   - ☑ Require status checks to pass before merging
   - ☑ Require branches to be up to date before merging
   - Status check: **`build-and-test`**
4. Optionally enable:
   - ☑ Require pull request reviews before merging
   - ☑ Do not allow bypassing the above settings

---

## Test Projects

### `tools/zone-server.Tests` (xUnit)

| Test class | Covers |
|------------|--------|
| `FrameIOTests` | V2 frame send/read round-trips over TCP, empty body, MessagePack payloads, disconnect handling, ordering |
| `WireMessagesTests` | MessagePack serialize/deserialize for all Wire message types, opcode enum values |
| `MessageRegistryTests` | Opcode→CLR type resolution, protocol version gating, overwrite semantics |
| `MemorySessionStoreTests` | Add/remove/lookup by ID and account, duplicate handling, case-insensitive account lookup |
| `MemoryCacheTests` | Get/set/remove, TTL expiry, overwrite, count tracking |
| `SingleCellAoiTests` | Enter/leave/move, self-exclusion from visibility, count |

### `tools/login-stub.Tests` (xUnit)

| Test class | Covers |
|------------|--------|
| `PacketBuilderTests` | Byte/Int16/Int32/Int64 LE encoding, UTF-16LE strings, raw bytes, chaining |
| `LoginCryptTests` | Blowfish encrypt/decrypt round-trip, XOR pass, checksum append/verify, padding, Init packet round-trip, modulus scramble |
| `FramedTests` | Interlude-style framed send/read round-trip, LE length header, disconnect handling |

### Running tests locally

```bash
# From repo root
dotnet test tools/Fineage.Tools.sln --verbosity normal

# Single project
dotnet test tools/zone-server.Tests/

# With coverage (requires coverlet)
dotnet test tools/Fineage.Tools.sln --collect:"XPlat Code Coverage"
```

---

## Unity EditMode Tests (Future)

Unity C# code (`l2-unity/Assets/Scripts/`) cannot be built or tested in GitHub Actions without a Unity license. Here's the plan for adding tests later:

### Option 1: Unity Test Framework (EditMode)

1. Create `l2-unity/Assets/Tests/EditMode/` with a `.asmdef` referencing the game assemblies
2. Write tests using `NUnit` (Unity's test runner)
3. Run locally via Unity Test Runner window
4. For CI: requires [GameCI](https://game.ci/) with a Unity license file (`.ulf`)
   - Free for personal licenses
   - Needs `UNITY_LICENSE` secret in GitHub repo settings

### Option 2: Extract pure C# to shared library

Extract Unity-independent logic (domain models, DTOs, pure computation) into a `netstandard2.1` class library:
- `Fineage.Domain/` — domain events, models
- Can be referenced by both Unity (as a DLL) and xUnit tests
- Builds and tests in CI without Unity

### What's testable without Unity right now

- All `tools/` code (zone-server, login-stub) — ✅ covered
- Wire protocol encoding/decoding — ✅ covered
- Crypto (Blowfish, XOR) — ✅ covered
- Domain DTOs (DomainMoveTo, DomainPlayerSpawned, etc.) — needs extraction from Unity project

### What requires Unity

- MonoBehaviour lifecycle (Awake, Update, etc.)
- Scene loading and DI scopes
- Rendering, animation, physics
- UI (UITK, UGUI)
