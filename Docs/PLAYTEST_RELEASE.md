# Playtest client releases

Windows, macOS, and Linux players for the one shared public dev server. The client in git still defaults to `127.0.0.1`. Release builds bake a hostname from `FINEAGE_PUBLIC_HOST` (a GitHub Actions secret or a `workflow_dispatch` input). Nothing in the repo hardcodes that hostname.

PR CI (`.github/workflows/build-and-test.yml`) does not run Unity. The release workflow is `.github/workflows/release-clients.yml`.

## Where the client connects

Login is the serialized `_serverIp` / `_serverPort` on `LoginClient` in `l2-unity/Assets/Resources/Prefab/Networking.prefab` (`127.0.0.1:2106`). The game address is whatever the login server puts in ServerList (`ServerSelectWindow` copies that IP and port onto `GameClient`). `AsynchronousClient` then dials TCP.

`PlaytestEndpointRuntime` runs at the start of `Connect()`:

- Login uses the public host and port (default **2106** when a host is set and no port is).
- Game host replaces the ServerList address when a public host is set, so a LAN IP in the server list does not win.
- Game port stays the ServerList port unless `gamePort` / `FINEAGE_GAME_PORT` is set.

Zone v2 (`FineageWireConfig.ZoneHost`) is unchanged.

The socket accepts a DNS name, not only a dotted IP. IPv4 is preferred. The client opens a normal TCP connection to `host:port`. A Cloudflare Tunnel TCP route that only answers through `cloudflared access tcp` is not a socket this client can dial. The hostname has to resolve and accept TCP (DNS plus forwarded ports, Cloudflare Spectrum, or two real TCP listeners). When login and game are different names, set `FINEAGE_LOGIN_HOST` and `FINEAGE_GAME_HOST`. When the public game port is not the port inside ServerList, set `FINEAGE_GAME_PORT`.

## Config file

```json
{
  "host": "",
  "loginHost": "",
  "gameHost": "",
  "loginPort": 0,
  "gamePort": 0
}
```

`host` fills login and game when the specific host is empty. `loginPort` / `gamePort` of `0` means unset. The copies in git are empty on purpose:

- `l2-unity/Assets/StreamingAssets/fineage-endpoint.json` (shipped, editable in the player)
- `l2-unity/Assets/Resources/FineagePlaytest.json` (baked fallback)

`scripts/bake-playtest-endpoint.py` overwrites both from the environment before a release build. It refuses to write when every host variable is empty.

## Override order

Highest priority first. An empty field does not clear a lower layer.

1. Command line: `-fineageHost` / `--fineage-host`, `-fineageLoginHost`, `-fineageGameHost`, `-fineageLoginPort`, `-fineageGamePort` (space or `=`).
2. `fineage-endpoint.json` in Unity's `persistentDataPath` (survives updates). For company `DefaultCompany` and product `L2-Unity`:
   - Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\L2-Unity\fineage-endpoint.json`
   - macOS: `~/Library/Application Support/DefaultCompany/L2-Unity/fineage-endpoint.json`
   - Linux: `~/.config/unity3d/DefaultCompany/L2-Unity/fineage-endpoint.json`
3. `fineage-endpoint.json` next to the executable (Windows/Linux), or next to `Fineage.app` (macOS). Also checked in the folder that contains `Fineage_Data`.
4. Environment: `FINEAGE_PUBLIC_HOST`, `FINEAGE_LOGIN_HOST`, `FINEAGE_GAME_HOST`, `FINEAGE_LOGIN_PORT`, `FINEAGE_GAME_PORT`.
5. `StreamingAssets/fineage-endpoint.json` inside the build.
6. Resources asset `FineagePlaytest`.

Examples (replace the hostname; do not commit it):

```bash
FINEAGE_PUBLIC_HOST=<your-tunnel-hostname> python3 scripts/bake-playtest-endpoint.py
Fineage.exe -fineageHost <your-tunnel-hostname> -fineageGamePort 7777
FINEAGE_PUBLIC_HOST=<your-tunnel-hostname> ./Fineage.x86_64
```

Revert a local bake with `git checkout -- l2-unity/Assets/StreamingAssets/fineage-endpoint.json l2-unity/Assets/Resources/FineagePlaytest.json`.

## GitHub Actions

| Trigger | Result |
| --- | --- |
| Push to `main` | Build all three players. Prerelease tag `playtest-<version>`. |
| Tag `v*` | Build all three. Full release on that tag. `releases/latest` points here. |
| `workflow_dispatch` | Build the selected ref. Release is on unless `create_release` is false. |

Version comes from the tag, otherwise `git describe` (`0.4.1-2-gabcdef`), otherwise `0.0.0-<sha>`. Unity's bundle version is the numeric `major.minor.patch` only. `version.json` keeps the full version, commit, host, ports, and asset names. `CHANGELOG.md` lists up to 100 commits since the previous tag.

Each zip has the player plus those two files at the top. The same two files are separate release assets. Workflow artifacts are kept for 90 days even if the GitHub Release step is skipped.

Builds are serialized (`max-parallel: 1`) so a personal Unity seat is only activated once. The Windows player is Mono and is cross-compiled on Linux. Switching Standalone to IL2CPP means moving that job to `windows-latest`.

`Assets/` is about 2.4 GB. The Linux jobs delete preinstalled SDKs first. If a hosted runner still runs out of disk, change that matrix `runsOn` to a larger runner or a self-hosted machine with Unity **6000.3.27f1** (the version in `l2-unity/ProjectSettings/ProjectVersion.txt`). `unityVersion: auto` reads that file. game-ci must have published an editor image for it; a missing image fails the build job when the container is pulled.

A zip over 2 GiB cannot be a GitHub Release asset. The workflow artifact is still uploaded. `scripts/pack-client.py` prints a warning.

## Secrets

Repository secrets (Settings → Secrets and variables → Actions). The prepare job exits before Unity starts when these are missing, and the log points at this file. Values are not printed.

Personal license ([game-ci activation](https://game.ci/docs/github/activation)):

| Secret | Value |
| --- | --- |
| `UNITY_LICENSE` | Base64 of the `.ulf` from activation. `base64 -w 0 Unity_lic.ulf` on Linux, `base64 -i Unity_lic.ulf \| tr -d '\n'` on macOS. |
| `UNITY_EMAIL` | Unity account email. |
| `UNITY_PASSWORD` | Unity account password. game-ci has seen failures when the password contains special characters; use a mixed-case alphanumeric password. |

Pro/Plus can use `UNITY_SERIAL` + `UNITY_EMAIL` + `UNITY_PASSWORD` instead of `UNITY_LICENSE`. Do not commit the `.ulf`.

Playtest host:

| Secret | Value |
| --- | --- |
| `FINEAGE_PUBLIC_HOST` | Hostname or IP of the shared dev server. Required unless `FINEAGE_LOGIN_HOST` or `FINEAGE_GAME_HOST` is set. |
| `FINEAGE_LOGIN_HOST` | Optional. Overrides the login host only. |
| `FINEAGE_GAME_HOST` | Optional. Overrides the game host only. |
| `FINEAGE_LOGIN_PORT` | Optional. Default 2106. |
| `FINEAGE_GAME_PORT` | Optional. Unset keeps the ServerList port. |

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

`version.json` fields: `version`, `unityVersion`, `commit`, `ref`, `releaseTag`, `prerelease`, `host`, `loginHost`, `gameHost`, `loginPort`, `gamePort` (`0` means the client kept the server-list port), `gamePortMeaning`, `assets[]` (`id`, `file`).

Read `version.json` before linking the zips so the page can show the build version, changelog, and which host that client was baked for.
