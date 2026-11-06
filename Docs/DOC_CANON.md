# Fineage — document canon

Which docs agents and humans should trust.

**Near-term focus:** finish and **stabilize Interlude** (Mobius CT0). High Five and other chronicles are **out of scope** until that lands. Tracker: [#602](https://github.com/FioraSolutions/Fineage/issues/602).

**Wire:** Fineage Net only (`PacketEnvelope` / `NetOpcode`).

## Read first

| Doc | Repo | Role |
|-----|------|------|
| [AGENTS.md](../AGENTS.md) | Client | Delivery, model choice, epic order |
| [Backend docs/net-protocol.md](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/net-protocol.md) | Backend | **Live wire SoT** (`NetOpcode`, ports, presence, Postgres) |
| [NET_PROTOCOL.md](NET_PROTOCOL.md) | Client | Client Net flow + `scripts/sync-net-contract.py` |
| [ARCHITECTURE_NET.md](ARCHITECTURE_NET.md) | Client | Target scale (gateway / AOI / 20k) — design, not “already shipped” |
| [Backend docs/MOBIUS_XML_REF.md](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/MOBIUS_XML_REF.md) | Backend | Mobius CT0 XML → `data/` |
| [Backend docs/combat-parity.md](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/combat-parity.md) | Backend | Combat checklist vs Mobius |
| [l2-unity/docs/LOGGING.md](../l2-unity/docs/LOGGING.md) | Client | Client logging |
| [WIP_DEEPSEEK_REVIEW.md](WIP_DEEPSEEK_REVIEW.md) | Client | Review of local uncommitted feature piles (not canon wire) |

Backend README must stay Net-accurate; prefer `docs/net-protocol.md` over any leftover MsgType tables elsewhere.

## Semantics-only (`*-v2-protocol.md`)

Backend `docs/*-v2-protocol.md` keep **payload semantics** and feature scope. Opcode numbers in those files are **legacy MessagePack `MsgType`** — not the on-wire Net opcodes. Live opcodes: `NetOpcode` + `docs/net-protocol.md`.

## Historical / snapshot — do not treat as current

| Doc | Stamp |
|-----|--------|
| [l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md](../l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md) | **HISTORICAL** (2026-10-27); see Status Now header in that file |
| [Backend docs/mobius-parity-audit.md](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/mobius-parity-audit.md) | **SNAPSHOT** pinned to old commit; re-run before trusting |

## External sources of truth (not in git)

| Source | Typical local path | Use |
|--------|-------------------|-----|
| L2J Mobius CT0 Interlude | `~/Downloads/L2J_Mobius_CT_0_Interlude_src` or Backend `ref/mobius-ct0-interlude` (gitignored) | Server formulas, XML datapack, AI |
| L2 Interlude client | `~/Downloads/L2_Interlude_Client` | Client assets / DAT reference |
| Fineage-Backend `data/*.json` | In Backend repo | Runtime catalog after MobiusImport |

Do not copy full Mobius trees into git. Sync Net contract with `python3 scripts/sync-net-contract.py` (client) against Backend `src/Fineage.Net.Contract`.

## Stabilize Interlude (current program)

1. Keep Fineage Net as the only client wire; do not revive MessagePack V2 or native L2 TCP as play path.
2. Prefer epic **#131** (client architecture) before packing more Gremlin/content onto the monolith (**#129**).
3. Treat Mobius CT0 as gameplay SoT; refresh parity docs against current `dev` when claiming gaps.
4. Land WIP in **small PRs**; do not mix doc sync with feature piles.
5. Chronicle / High Five versioning stays **deferred** — no HF datapack or DTO sprawl until Interlude is stable.

## Keep as-is (content / tooling)

`INTERLUDE_TILE_BAKE.md`, shader/VFX guides, UI tips, playtest/CI docs, agent branch protocol — not wire architecture.
