# Fineage

Unity client for Fineage. This repository is [FioraSolutions/Fineage](https://github.com/FioraSolutions/Fineage).

The client started from an earlier Unity L2 project (the [shnok](https://gitlab.com/shnok) / L2-Unity line; last sync with that source was 25 September 2024). Fineage then reworked networking and continues the Interlude-compatible client.

The game server is a separate repository: our own **C# (.NET) Login + Zone** in [Fineage-Backend](https://github.com/FioraSolutions/Fineage-Backend). **V2 MessagePack** is the primary protocol. An **Interlude** bridge remains for wire compatibility.

`tools/login-stub` and `tools/zone-server` in this repo are local bridge helpers for compatibility checks. The product Login and Zone processes are [Fineage-Backend](https://github.com/FioraSolutions/Fineage-Backend).

## Links

- Client: https://github.com/FioraSolutions/Fineage
- Product server: https://github.com/FioraSolutions/Fineage-Backend
- Delivery board: https://github.com/orgs/FioraSolutions/projects/7
- Upstream client line: https://gitlab.com/shnok
- Packet and layout notes: https://github.com/gawric/Guide-L2Unity
- Discussion: https://t.me/l2unityForL2j

Pull requests target the `dev` branch. Branch and worktree rules are in [AGENTS.md](AGENTS.md).

## Open and build the client

The Unity project is the `l2-unity/` folder. In Unity Hub, add that folder as the project.

1. Install Unity **6000.3.27f1** (the version in `l2-unity/ProjectSettings/ProjectVersion.txt`). The project uses URP.
2. Open `l2-unity`. The first import resolves packages and can take a long time. `Assets/` is on the order of 2.4 GB.
3. Open `Assets/Resources/Scenes/Menu.unity` and enter Play to reach the login screen. The walkthrough is [RUN.md](RUN.md).
4. Login and game hosts come from `l2-unity/Assets/StreamingAssets/fineage-endpoint.json`. The file in git points at the public dev server: login `login.fineage.evpoint.by:2106`, game `game.fineage.evpoint.by:7777`. Override those hosts when you want a process on your own machine. See [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md).
5. Standalone players: **File → Build Settings** in the editor (Windows, macOS, or Linux). Automated playtest builds are in [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md).

Local helpers are for compatibility checks on your machine. The product Login and Zone processes are Fineage-Backend.

| Tool | Protocol | Ports |
| --- | --- | --- |
| [`tools/login-stub`](tools/login-stub/README.md) | Interlude | 2106 login, 7777 game |
| [`tools/zone-server`](tools/zone-server/README.md) | V2 MessagePack | 9777 |

Pull-request CI does not start Unity. It builds and tests `tools/Fineage.Tools.sln` (**.NET 10**):

```bash
dotnet test tools/Fineage.Tools.sln
```

## Documentation

| Path | Contents |
| --- | --- |
| [Docs/ARCHITECTURE_NET.md](Docs/ARCHITECTURE_NET.md) | Target login, gateway, and zone architecture |
| [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md) | Playtest players, public hosts, endpoint overrides |
| [Docs/CI_AND_TESTING.md](Docs/CI_AND_TESTING.md) | GitHub Actions and `dotnet test` |
| [RUN.md](RUN.md) | Local path from the editor to the login screen |
| [ROADMAP.md](ROADMAP.md) | Protocol and content phases |
| [AGENTS.md](AGENTS.md) | Contributor and agent branch rules |
| [Docs/Ui.md](Docs/Ui.md), [Docs/Ui_Interface.md](Docs/Ui_Interface.md) | Inventory and interface captures |
| [l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md](l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md) | Client module boundaries |
| [l2-unity/docs/LOGGING.md](l2-unity/docs/LOGGING.md) | Client and helper logs |
| [l2-unity/docs/CONTENT_PREVIEW.md](l2-unity/docs/CONTENT_PREVIEW.md) | Offline character and NPC preview |
| [tools/login-stub/README.md](tools/login-stub/README.md), [tools/zone-server/README.md](tools/zone-server/README.md) | Local bridge helpers |

Shader and VFX notes live under `l2-unity/docs/`.

## Client notes

Early checks of this client ran against the Java servers Mobius and Acis on the Interlude wire. The product Login + Zone is now [Fineage-Backend](https://github.com/FioraSolutions/Fineage-Backend). The notes below are client capabilities from that work and from the 27 August 2026 update.

**Movement, combat, and UI (earlier pass)**

- Character, monster, and NPC movement in the style of the original client, with server sync (including a 3-vs-1 movement check).
- Physical-attack combat, a basic skill path and item use, shops, server HTML, nick colors, and tooltips.
- Login encryption compatible with the L2J-style handshake, and a mode without encryption.
- Animator rework. The previous clip set struggled past roughly 50 monsters.
- About 150 new character-part models, about 10 test effects, about 900 textures, about 100 server packets, and fighter and mage animations for humans and elves.
- Inventory and interface captures: [Docs/Ui.md](Docs/Ui.md), [Docs/Ui_Interface.md](Docs/Ui_Interface.md).

Videos: [bow animation and projectiles](https://disk.yandex.ru/i/PFP7tHJV4AlTUg), [basic flow](https://jmp.sh/70qYH2gU), [movement sync](https://jmp.sh/GX3m6XIs).

**Update, 27 August 2026**

Farming: [video](https://jumpshare.com/share/ps2GNwqncomBju1oNrLD).

- Effect library covering many skills, built from scripts, including effects from later chronicles.
- Animation manager with four states; other clips are substituted onto those states.
- [VContainer](https://github.com/hadashiA/VContainer) for composition.
- UI Toolkit for the local user. Two further libraries were planned so the interface can follow later chronicles.
- Packet codecs kept separate from the Unity world, so packets from other chronicles can be attached.
- About 400 new textures and meshes (weapons, armor, and similar).
- Combat sync while two players farm.
- World available at startup. Map load around 400 ms. Only the squares around the local player are loaded.
- Caches for meshes, NPCs, monsters, and dropped items.
- Nameplates in a single draw call.
- Particles that share one material are drawn on the GPU in one draw call (`ParticlesGroup`).
- `MFighter` and `MMagic` as bases for other classes: buffs, magic, soulshots, bows, SP skills, and seeing and dropping items.

<img src="https://i.ibb.co/zW8hVgnH/farm-1-2.png" alt="Two players farming" width="300">

<img src="https://i.ibb.co/fVPrb7m2/base-walpepr-2.png" alt="Character in the world" width="480">
<img src="https://i.ibb.co/js319yZ/sword-glow.png" alt="Weapon glow" width="480">
<img src="https://i.postimg.cc/RhzrQjzS/buy-sell.png" alt="Buy and sell window" width="480">

An asset thread in the Telegram group is bookmarked here (models and textures were posted; whether that pack is still needed was not sorted): https://t.me/c/l2unityForL2j/1

## По-русски

Этот репозиторий — Unity-клиент Fineage. Основа — более ранняя Unity-работа по L2 (линия [shnok](https://gitlab.com/shnok) / L2-Unity, последняя синхронизация 25.09.2024). Дальше Fineage переработал сеть и продолжает Interlude-совместимый клиент.

Игровой сервер — свой **C# (.NET) Login + Zone**: [Fineage-Backend](https://github.com/FioraSolutions/Fineage-Backend). Основной протокол — **V2 MessagePack**, мост **Interlude** — для совместимости.

`tools/login-stub` и `tools/zone-server` — локальные мосты. Как открыть клиент — в разделе **Open and build the client**. Подробности — в `Docs/`, `RUN.md` и `ROADMAP.md`.
