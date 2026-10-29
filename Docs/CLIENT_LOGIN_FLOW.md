# Client login / connection flow

How the Unity client gets from the login screen into the world, and what was removed in the
connection-path cleanup.

## Now (one path)

```
GameManager.Awake
  └─ FineageConnection.Initialize()            ← decides the backend ONCE
       ├─ FineageLocalBackend.TryGetEndpoints  ← only with an explicit opt-in (see below)
       └─ FineageEndpoints.Public(PlaytestEndpointRuntime.Current)
            → FineageWireConfig.Use(endpoints)  (read-only afterwards; mode = ZoneV2)

Log In button (LoginWindow) / local auto-login (GameManager)
  └─ ZoneV2Client.BeginSession(account, password)
       └─ FineageLobbySession.ConnectAsync(Endpoints.Login)       login TCP via FineageTcp
            Hello → HelloAck → Auth(account, password) → LoginOk → ListCharacters → Roster
  Roster #1 → LobbyFlowController.OnLobbyReady(servers)  → License window
  License "Agree"  → OnLicenseAccepted → ServerSelect window (LobbyServer rows)
  Server "Confirm" → ZoneV2Client.ConfirmServer → OnAuthAllowed → Character select
  Character "Start" → SelectCharacter → Handoff{zoneHost, zonePort, token}
  └─ Endpoints.ZoneForHandoff(advertised)        ← never loopback on the public RP
       └─ RunSessionAsync(zone)                    zone TCP via FineageTcp
            Hello → HelloAck → ZoneAuth(handoff token) → AuthOk → EnterWorld → PlayerSpawned
```

* **Endpoints**: `FineageEndpoints` is the single source of truth (`Backend`, `Login`, `Zone`).
  Public default: `login.fineage.evpoint.by:2106`, `game.fineage.evpoint.by:7777`.
  Overrides (baked JSON, StreamingAssets, env `FINEAGE_*_HOST/PORT`, sidecar / user
  `fineage-endpoint.json`, `-fineageLoginHost/-fineageGameHost/...` args) go through
  `PlaytestEndpoint`; any loopback override is ignored on the public backend.
* **Local backend** (`127.0.0.1:9106` / `:9777`) only when explicitly asked:
  `-fineageLocal`, `FINEAGE_LOCAL_BACKEND=1`, PlayerPrefs `Fineage.LocalBackend=1`, or a player built
  with the `FINEAGE_LOCAL_BACKEND` define. `-fineagePublic` / `FINEAGE_LOCAL_BACKEND=0` always win.
  Local-only extras: `-fineageChar` auto-pick, `-fineageUser/-fineagePass` auto-login.
* **TCP**: `FineageTcp.ConnectAsync` is the only dialer for login and zone: IPv4-first DNS,
  10 s connect timeout, cancellation, and a readable `Cannot reach login server host:port: reason`.
* **Server list**: Fineage-Backend has no server-list message yet, so `LobbyServer.FromEndpoints`
  builds one row (the world behind the configured zone; online because login just succeeded,
  character count from the roster). When the backend adds a list, decode it into `LobbyServer` rows.
* Editor Play, `Fineage/Play RP` and shipped players all take the same public path.

## Before (what was removed)

| Crutch | Why it existed | Now |
|---|---|---|
| Interlude login: `LoginClient` (Blowfish/RSA TCP), `RequestAuthLogin`, `RequestServerList`, `RequestServerLogin`, `PlayOk`, `InterludeKey` → `GameClient` `AuthLogin` with play/session keys, login send/receive queues, login packet registry, `ServerNameDAO` | L2J stub era | Deleted. The only login is Fineage-Backend lobby + handoff. |
| `FineageWireConfig.UseLoginHandoff` + mutable static `LoginHost/LoginPort/ZoneHost/ZonePort` | Two login paths selected at runtime | Deleted. Read-only `FineageWireConfig.Endpoints`. |
| `GameManager.Awake` endpoint logic, `_useZoneServerV2` inspector toggle, `#if UNITY_EDITOR` public gate | Editor defaulted to Interlude/localhost | `FineageConnection.Initialize()` |
| `FineageLocalBackend.PublicPlaytestRequested()`, `FINEAGE_PUBLIC_PLAYTEST` code gate, PlayerPrefs `Fineage.PublicPlaytest` | Opt-in to public RP | Public is the default; nothing to opt into. |
| `DefaultClient.ApplyFineageLoginEndpointIfNeeded`, `IsPublicLoginHost` guards, prefab `_serverIp` fallbacks | Prefab 127.0.0.1 leaked into builds | Gone with `LoginClient`; endpoints never come from prefabs. |
| `PlaytestEndpointRuntime.ApplyLogin/ApplyGame` | Second endpoint system, unused | Deleted; runtime only reads overrides. |
| Handoff host from the server used verbatim (`127.0.0.1` behind the tunnel) | — | `ZoneForHandoff` falls back to the configured public zone. |
| Three `new TcpClient().ConnectAsync` without timeout | — | `FineageTcp`. |
| `LoginClient` component on `Resources/Prefab/Networking.prefab` | — | Removed. |

## Verify in the Editor

1. `Fineage/Play RP` (or plain Play on the menu scene) → console shows
   `[Fineage] connection Public login=login.fineage.evpoint.by:2106 zone=game.fineage.evpoint.by:7777`.
2. Log In → License → Server select (one row "Fineage", Normal, character count) → Character select.
3. Start → `[Fineage] handoff ... zone=game.fineage.evpoint.by:7777` → `[ZoneV2] connected ...`.
4. Local backend: run with `-fineageLocal` (or `FINEAGE_LOCAL_BACKEND=1`) → `connection Local login=127.0.0.1:9106`.
