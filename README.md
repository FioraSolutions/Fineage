# Fineage

Unity client for Fineage. This repository is [FioraSolutions/Fineage](https://github.com/FioraSolutions/Fineage).

The client started from an earlier Unity L2 project (the [shnok](https://gitlab.com/shnok) / L2-Unity line; last sync with that source was 25 September 2024). Fineage reworked networking and continues an **Interlude-era** client (Mobius CT0 content target).

The game server is a separate repository: **C# (.NET) Login + Zone** in [Fineage-Backend](https://github.com/FioraSolutions/Fineage-Backend). The **only** live client transport is **Fineage Net** (`PacketEnvelope` / `NetOpcode`). MessagePack Zone V2 is retired. The presentation layer is still Interlude-shaped (DTOs / DAT); Net maps into that apply path.

Local `tools/login-stub` and `tools/zone-server` are historical / diag helpers — **not** the play path. Product Login (**9106**, public remap **2106**) and Zone (**9777**, public remap **7777**) are Fineage-Backend.

## Links

- Client: https://github.com/FioraSolutions/Fineage
- Product server: https://github.com/FioraSolutions/Fineage-Backend
- Delivery board: https://github.com/orgs/FioraSolutions/projects/7
- Doc triage: [Docs/DOC_CANON.md](Docs/DOC_CANON.md)
- Upstream client line: https://gitlab.com/shnok
- Packet and layout notes: https://github.com/gawric/Guide-L2Unity
- Discussion: https://t.me/l2unityForL2j

Pull requests target the `dev` branch. Branch and worktree rules are in [AGENTS.md](AGENTS.md).

## Open and build the client

The Unity project is the `l2-unity/` folder. In Unity Hub, add that folder as the project.

1. Install Unity **6000.3.27f1** (the version in `l2-unity/ProjectSettings/ProjectVersion.txt`). The project uses URP.
2. Open `l2-unity`. The first import resolves packages and can take a long time. `Assets/` is on the order of 2.4 GB.
3. Open `Assets/Resources/Scenes/Menu.unity` and enter Play to reach the login screen. The walkthrough is [RUN.md](RUN.md).
4. Editor Play defaults to **local Fineage Net** (loopback Login/Zone). Public playtest hosts are in `l2-unity/Assets/StreamingAssets/fineage-endpoint.json` / `FINEAGE_NET_CLIENT=0`. See [Docs/NET_PROTOCOL.md](Docs/NET_PROTOCOL.md) and [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md).
5. Standalone players: **File → Build Settings** in the editor (Windows, macOS, or Linux). Automated playtest builds are in [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md).

| Tool | Role | Ports |
| --- | --- | --- |
| Fineage-Backend Login + Zone | **Live** Net play path | 9106 / 9777 (dev); 2106 / 7777 (public remap) |
| [`tools/login-stub`](tools/login-stub/README.md) | Legacy Interlude diag only | 2106 / 7777 |
| [`tools/zone-server`](tools/zone-server/README.md) | Legacy MessagePack diag only | 9777 |

Pull-request CI does not start Unity. It builds and tests `tools/Fineage.Tools.sln` (**.NET 10**):

```bash
dotnet test tools/Fineage.Tools.sln
```

## Documentation

| Path | Contents |
| --- | --- |
| [Docs/DOC_CANON.md](Docs/DOC_CANON.md) | What to trust vs historical |
| [Docs/NET_PROTOCOL.md](Docs/NET_PROTOCOL.md) | Client Fineage Net flow |
| [Docs/ARCHITECTURE_NET.md](Docs/ARCHITECTURE_NET.md) | Target login, gateway, and zone scale |
| [Docs/PLAYTEST_RELEASE.md](Docs/PLAYTEST_RELEASE.md) | Playtest players, public hosts, endpoint overrides |
| [Docs/CI_AND_TESTING.md](Docs/CI_AND_TESTING.md) | GitHub Actions and `dotnet test` |
| [RUN.md](RUN.md) | Local path from the editor to the login screen |
| [ROADMAP.md](ROADMAP.md) | Protocol and content phases |
| [AGENTS.md](AGENTS.md) | Contributor and agent branch rules |
| [Docs/Ui.md](Docs/Ui.md), [Docs/Ui_Interface.md](Docs/Ui_Interface.md) | Inventory and interface captures |
| [l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md](l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md) | Historical client review + Status Now |
| [l2-unity/docs/LOGGING.md](l2-unity/docs/LOGGING.md) | Client and helper logs |
| [l2-unity/docs/CONTENT_PREVIEW.md](l2-unity/docs/CONTENT_PREVIEW.md) | Offline character and NPC preview |

Shader and VFX notes live under `l2-unity/docs/`. Backend wire SoT: [Fineage-Backend `docs/net-protocol.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/net-protocol.md).

## Focus (near term)

**Finish and stabilize Interlude** (Mobius CT0 parity, Net smoke, client arch epic [#131](https://github.com/FioraSolutions/Fineage/issues/131)). High Five is out of scope until Interlude is solid.

## Client notes

Early checks of this client ran against the Java servers Mobius and Acis on the Interlude wire. The product Login + Zone is now Fineage-Backend over Fineage Net.

**Movement, combat, and UI (earlier pass)**

- Character, monster, and NPC movement with server sync.
- Physical-attack combat, basic skill path and item use, shops, server HTML, nick colors, and tooltips.
- Inventory and interface captures: [Docs/Ui.md](Docs/Ui.md), [Docs/Ui_Interface.md](Docs/Ui_Interface.md).

Videos: [bow animation and projectiles](https://disk.yandex.ru/i/PFP7tHJV4AlTUg), [basic flow](https://jmp.sh/70qYH2gU), [movement sync](https://jmp.sh/GX3m6XIs).

## License

Fineage is open source under the [Apache License 2.0](LICENSE).
