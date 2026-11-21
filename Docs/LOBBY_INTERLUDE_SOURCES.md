# Account lobby (char select) — Interlude sources

Fineage account lobby = retail **Interlude** `maps/Lobby.unr` (Talking Island hall + race towers), not later-chronicle `Lobby01.unr` / `Lobby02.unr` swaps used from Gracia/Freya/H5 onward.

## Map / scene (in repo)

| SoT | Fineage path |
| --- | --- |
| Retail map name | `Lobby.unr` (`LobbyMapInterludeContract.RetailMapName`) |
| Additive Unity scene | `l2-unity/Assets/Resources/Scenes/l2_lobby.unity` (`SceneLoader._lobbyScene` = `l2_lobby`) |
| Forest login vista meta | `l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/LoginForestActors.json` (note: full Interlude `Lobby.unr` forest login) |
| Char-create hall camera | `CharSelect_` — see `Docs/agent-tips/create-hall-textures.md` |
| Render profile (data-driven) | `Resources/Settings/LobbyAccountEnvironmentProfile.asset` → `LobbyAccountRenderSettings` |

### External references (retail file sets)

- [MaxCheaters — transfer Interlude login to later client](https://maxcheaters.com/topic/243457-change-login-screen-gf-to-interlude/) — Interlude uses `Lobby.unr` + `L2_Lobby.usx` + `L2_Lobby_T.utx` + `Lobby_terrain_t.utx` (later chronicles use `Lobby01.unr` / `Lobby02.unr`).
- [Zone-Game — login window file list C3/C4/Interlude vs Gracia/H5](https://forum.zone-game.info/showthread.php?tid=41974) — same split; brightness/fog live in map `ZoneInfo` (L2PE), not only `env.int`.
- [RageZone — `env.int` `StaticLightingAdjust`](https://forum.ragezone.com/threads/enchant-color-change.107275/) — global static-lighting scale (Fineage bakes scene ambient from `l2_lobby.unity` RenderSettings instead of hardcoding flat ambient in code).

## UI (L2UI_CH3, not Gracia `*_DF_*`)

| Control | Interlude art in Fineage |
| --- | --- |
| Char info panel back | `Data/UI/Status/Interlude/Status_Interlude_MyInfo_Back` (USS `.il-char-select .char-select-detail`) |
| HP/MP/EXP bars | `Status_Interlude_Ps_*` in `CharSelectWindow.uxml` (same family as in-game status Ps gauges) |
| Create/Delete/Re-Login/Start | `Data/UI/Login/Interlude/bigbutton` (+ over/down), `.il-char-select-*-btn` in `L2StyleSheet.uss` |
| SP/Karma readouts | `Data/UI/Login/Interlude/inputbox2` via `.il-char-select .login-input-bg` |

Login screen parity pattern: `LoginWindow.uxml` + `.il-login` in `L2StyleSheet.uss`.

## Audio

- Intro: `event:/Music/l2_lobby/intro` (`AudioMusicIds.LoginIntroFmodPath`) — retail `ALAUDIO.INT` `[LobbySound] LogInSound` family (see MaxCheaters topic above).