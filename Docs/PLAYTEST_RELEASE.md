# Playtest client releases

Windows, macOS, and Linux players for the one shared public dev server. The client defaults to the Cloudflare tunnel:

| Name | Host | Port |
| --- | --- | --- |
| Landing (HTTP) | `fineage.evpoint.by` | 443 |
| Login (TCP) | `login.fineage.evpoint.by` | 2106 |
| Game (TCP) | `game.fineage.evpoint.by` | 7777 |

`FINEAGE_PUBLIC_HOST` is the landing name. It does not replace the login or game hosts. Secrets and `workflow_dispatch` inputs override any of the three.

PR CI (`.github/workflows/build-and-test.yml`) does not run Unity. The release workflow is `.github/workflows/release-clients.yml`.

## Where the client connects

Login defaults are `login.fineage.evpoint.by:2106` on `LoginClient` (prefab + `Connect()` applies `PlaytestEndpointRuntime`). The game address is whatever the login server puts in ServerList (`ServerSelectWindow` copies that IP and port onto `GameClient`), then `Connect()` may replace the host with `game.fineage.evpoint.by`. `AsynchronousClient` then dials TCP.

`PlaytestEndpointRuntime` runs at the start of `Connect()`:

- Login dials `login.fineage.evpoint.by:2106` unless `loginHost` / `FINEAGE_LOGIN_HOST` overrides it.
- Game host replaces the ServerList address (`game.fineage.evpoint.by`), so a LAN IP in the server list does not win.
- Game port is **7777** unless `gamePort` / `FINEAGE_GAME_PORT` overrides it.

Zone v2 (`FineageWireConfig.ZoneHost`) is unchanged.

The socket accepts a DNS name, not only a dotted IP. IPv4 is preferred. The client opens a normal TCP connection to the login host on 2106 and the game host on 7777. `fineage.evpoint.by` is HTTP for the landing page and is not a game socket. Each TCP name has to resolve and accept TCP on that port. A tunnel route that only answers through `cloudflared access tcp` is not a socket this client can dial.

## Config file

```json
{
  "host": "fineage.evpoint.by",
  "loginHost": "login.fineage.evpoint.by",
  "gameHost": "game.fineage.evpoint.by",
  "loginPort": 2106,
  "gamePort": 7777
}
```

`host` is the landing hostname. `loginHost` and `gameHost` are what the sockets dial. The same JSON is committed in:

- `l2-unity/Assets/StreamingAssets/fineage-endpoint.json` (shipped, editable in the player)
- `l2-unity/Assets/Resources/FineagePlaytest.json` (baked fallback)

`scripts/bake-playtest-endpoint.py` rewrites both before a release build. Empty environment variables keep these defaults. A set variable replaces that one field.

## Override order

Highest priority first. An empty field does not clear a lower layer.

1. Command line: `-fineageHost` / `--fineage-host` (landing only), `-fineageLoginHost`, `-fineageGameHost`, `-fineageLoginPort`, `-fineageGamePort` (space or `=`).
2. `fineage-endpoint.json` in Unity's `persistentDataPath` (survives updates). For company `DefaultCompany` and product `L2-Unity`:
   - Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\L2-Unity\fineage-endpoint.json`
   - macOS: `~/Library/Application Support/DefaultCompany/L2-Unity/fineage-endpoint.json`
   - Linux: `~/.config/unity3d/DefaultCompany/L2-Unity/fineage-endpoint.json`
3. `fineage-endpoint.json` next to the executable (Windows/Linux), or next to `Fineage.app` (macOS). Also checked in the folder that contains `Fineage_Data`.
4. Environment: `FINEAGE_PUBLIC_HOST`, `FINEAGE_LOGIN_HOST`, `FINEAGE_GAME_HOST`, `FINEAGE_LOGIN_PORT`, `FINEAGE_GAME_PORT`.
5. `StreamingAssets/fineage-endpoint.json` inside the build.
6. Resources asset `FineagePlaytest`.

Local loopback (the prefab still says `127.0.0.1`; the playtest config replaces it unless you override):

```bash
Fineage.exe -fineageLoginHost 127.0.0.1 -fineageGameHost 127.0.0.1 -fineageLoginPort 2106 -fineageGamePort 7777
```

A one-off public host:

```bash
FINEAGE_LOGIN_HOST=login.example FINEAGE_GAME_HOST=game.example python3 scripts/bake-playtest-endpoint.py
```

Revert a local bake with `git checkout -- l2-unity/Assets/StreamingAssets/fineage-endpoint.json l2-unity/Assets/Resources/FineagePlaytest.json`.

## GitHub Actions

| Trigger | Result |
| --- | --- |
| Push to `main` | Build all three players. Prerelease tag `playtest-<version>`. |
| Tag `v*` | Build all three. Full release on that tag. `releases/latest` points here. |
| `workflow_dispatch` | Build the selected ref. Release is on unless `create_release` is false. |

Version comes from the tag (`v1.2.3` → `1.2.3`, `playtest-0.0.5` → `0.0.5`). On an untagged `main` push, the script finds the highest existing `vX.Y.Z` or `playtest-X.Y.Z` tag and uses the next patch (`0.0.1` when none exist). Unity's bundle version is the same `X.Y.Z`. `version.json` keeps the full version, commit, host, ports, and asset names. GitHub release title is `Fineage X.Y.Z`; the playtest git tag is `playtest-X.Y.Z`. `CHANGELOG.md` lists up to 100 commits since the previous tag.

Each zip has the player plus those two files at the top. The same two files are separate release assets. Workflow artifacts are kept for 90 days even if the GitHub Release step is skipped.

Builds are serialized (`max-parallel: 1`) so a personal Unity seat is only activated once per job matrix within a run. The Windows player is Mono and is cross-compiled on Linux. Switching Standalone to IL2CPP means moving that job to `windows-latest`.

Workflow concurrency uses group `release-clients` with `cancel-in-progress: false`. A started run is allowed to finish attaching Windows, macOS, and Linux even if another push to `main` starts a newer run. That avoids Windows-only prereleases when `main` moves quickly; the tradeoff is that two overlapping runs can briefly hold two Personal seats until the older run completes.

`Assets/` is about 2.4 GB. The Linux jobs delete preinstalled SDKs first. If a hosted runner still runs out of disk, change that matrix `runsOn` to a larger runner or a self-hosted machine with Unity **6000.3.27f1** (the version in `l2-unity/ProjectSettings/ProjectVersion.txt`). `unityVersion: auto` reads that file. game-ci must have published an editor image for it; a missing image fails the build job when the container is pulled.

A zip over 2 GiB cannot be a GitHub Release asset. The workflow artifact is still uploaded. `scripts/pack-client.py` prints a warning.

## Secrets

Repository secrets (Settings → Secrets and variables → Actions). The prepare job exits before Unity starts when these are missing, and the log points at this file. Values are not printed.

Personal ([game-ci activation](https://game.ci/docs/github/activation)) can use `UNITY_EMAIL` + `UNITY_PASSWORD` alone. game-ci/unity-builder signs in and takes a floating Personal license. Unity blocks Personal offline activation on license.unity3d.com, so a `.ulf` is often unavailable. `UNITY_LICENSE` (base64 `.ulf`) stays optional and is preferred when you already have one.

| Secret | Value |
| --- | --- |
| `UNITY_EMAIL` | Unity account email. Required. |
| `UNITY_PASSWORD` | Unity account password. Required. game-ci has seen failures when the password contains special characters; use a mixed-case alphanumeric password. |
| `UNITY_LICENSE` | Optional, preferred when available. Base64 of a `.ulf`. `base64 -w 0 Unity_lic.ulf` on Linux, `base64 -i Unity_lic.ulf \| tr -d '\n'` on macOS. Personal accounts cannot download this file from license.unity3d.com. |

Pro/Plus can use `UNITY_SERIAL` + `UNITY_EMAIL` + `UNITY_PASSWORD` instead of a Personal login or `UNITY_LICENSE`. Do not commit the `.ulf`.

Playtest host secrets are optional. Unset means the Cloudflare defaults above.

| Secret | Override for |
| --- | --- |
| `FINEAGE_PUBLIC_HOST` | Landing host. Default `fineage.evpoint.by`. Does not change login or game. |
| `FINEAGE_LOGIN_HOST` | Login TCP host. Default `login.fineage.evpoint.by`. |
| `FINEAGE_GAME_HOST` | Game TCP host. Default `game.fineage.evpoint.by`. |
| `FINEAGE_LOGIN_PORT` | Login port. Default 2106. |
| `FINEAGE_GAME_PORT` | Game port. Default 7777. |

`workflow_dispatch` inputs override the matching secrets for that run.

## Path from `dev`

1. Merge the workflow to `dev` (this PR). That does not build clients.
2. Merge `dev` → `main`. The push to `main` runs `release-clients`.
3. GitHub only lists `workflow_dispatch` after the workflow file exists on the default branch. After the first merge to `main`, Actions → release-clients → Run workflow can target `dev` and build that branch without another merge. Secrets are repository secrets, so every ref sees them.

Cutting a stable download later is a `v*` tag on the commit you want (`v0.2.0`). Pushes to `main` stay prereleases and do not replace `releases/latest`.

## How landing consumes the release

Asset names are stable:

| File | |
| --- | --- |
| `Fineage-Windows-x64.zip` | Windows |
| `Fineage-macOS.zip` | macOS |
| `Fineage-Linux-x64.zip` | Linux |
| `CHANGELOG.md` | Notes, also the release body |
| `version.json` | Version, commit, host, ports, asset names |

Stable tag release:

```http
GET https://api.github.com/repos/FioraSolutions/Fineage/releases/latest
```

Playtest from `main` (prerelease, tag `playtest-<version>`):

```http
GET https://api.github.com/repos/FioraSolutions/Fineage/releases?per_page=20
```

Use the newest release whose `tag_name` starts with `playtest-`. Download:

```text
https://github.com/FioraSolutions/Fineage/releases/download/<tag>/<file>
```

The landing page is `https://fineage.evpoint.by`. It should link the zips from the GitHub Release, not proxy the game TCP ports.

`version.json` fields: `version`, `unityVersion`, `commit`, `ref`, `releaseTag`, `prerelease`, `host`, `landingUrl`, `loginHost`, `gameHost`, `loginPort`, `gamePort`, `gamePortMeaning` (`override` when the client forces the game port, which the default 7777 does), `assets[]` (`id`, `file`).

Read `version.json` before linking the zips so the page can show the build version, changelog, and which login and game hosts that client was baked for.
