# CI & Testing Guide

## GitHub Actions CI

### Workflow: `build-and-test`

**File:** `.github/workflows/build-and-test.yml`

**Triggers:**
- Pull request → `dev`
- Push to `feat/**`, `fix/**`, `chore/**` branches

**What it does:**
1. Checks out this repo **and** `FioraSolutions/Fineage-Backend` at `dev`
2. Frozen-path guards (`IncomingPacketActions`, Net domain maps, asmdef layout)
3. **Net contract sync:** `FINEAGE_REQUIRE_CONTRACT_SYNC=1 python3 scripts/sync-net-contract.py --check` against the checked-out Backend (fails on drift; does not skip when Backend is present)
4. Restores, builds, and tests `tools/Fineage.Tools.sln` (Release configuration)
5. Uploads test results as artifacts (`.trx` format, 14 day retention)

**Not in PR CI:** Unity Editor/Player compile, EditMode/PlayMode tests. Player builds run on `release-clients` (not a PR gate). Follow-up: replace closed [#34](https://github.com/FioraSolutions/Fineage/issues/34) with an explicit Unity PR-compile issue when runners/license allow.

**Required status check name:** `build-and-test`

### Workflow: `format-check`

Optional formatting verification using `dotnet format --verify-no-changes`.
Not required for merge (advisory only).

### Workflow: `release-clients`

**File:** `.github/workflows/release-clients.yml`

Builds the Windows, macOS, and Linux playtest clients, plus `version.json` and `CHANGELOG.md`. It does not run on pull requests. Setup, secrets, the `dev` → `main` path, and how a landing page reads the release assets are in [PLAYTEST_RELEASE.md](PLAYTEST_RELEASE.md).

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

## Unity EditMode / PR compile (#737)

Workflow: `.github/workflows/unity-compile.yml` (path-filtered to `l2-unity/Assets/Scripts|Tests`).

**Merge policy:** `unity-editmode` is **not** a required status check on `dev`. Required gate remains **`build-and-test`** only. Agents/humans may merge once `build-and-test` is green; do not block ~20–25 min on game-ci. Treat a later red Unity run as a follow-up fix, not a merge blocker.

- Uses [game-ci/unity-test-runner](https://game.ci/) **v4.4.0** + `cliVersion: v0.1.72` with the same Personal seat as `release-clients`: `UNITY_EMAIL` + `UNITY_PASSWORD` (optional `UNITY_SERIAL`). Do not set `coverageEnabled: false` — that maps to a CLI flag v0.1.72 rejects.
- Runs EditMode tests under `l2-unity/Assets/Tests/EditMode/` — opening the project compiles `Assets/Scripts`; script errors fail the job.
- Smoke test covers `PacketReader` in `Fineage.Networking` so the job is not an empty suite.
- Do **not** rely on a stale `UNITY_LICENSE` `.ulf` for this Personal path (same note as playtest builds).
- Networking must not reference `Fineage.Game` (cycle). Shared wire helpers (`VectorUtils` / `NumberUtils` / manor seed models / `ChatTypeData`) live in `Fineage.Domain`.

### Option 1: more EditMode tests locally

1. Add tests under `l2-unity/Assets/Tests/EditMode/`
2. Run via Unity Test Runner window

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
