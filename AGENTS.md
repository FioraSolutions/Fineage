# Fineage — agent entry

Default workspace: **`/Users/nikitarakitin/Projects/Fineage`** (client) and **`/Users/nikitarakitin/Projects/Fineage-Backend`** (server). Work in these primary folders. Do not invent sibling worktrees unless the user explicitly asks for parallel agents.

Do not push to `main`/`dev` directly — land via PR into **`dev`** (client) / **`dev`** (backend; `main` is protected deploy). Doc triage: [`Docs/DOC_CANON.md`](Docs/DOC_CANON.md).

## Parity with originals (hard rule)

Fineage **server business logic**, **client behavior**, and **client look** must match the Interlude originals unless a ticket explicitly documents a deliberate divergence. Do not invent UI layouts, HUD chrome, or gameplay rules when the original exists.

| SoT | Local path (gitignored under `ref/`) |
|-----|--------------------------------------|
| L2J Mobius CT0 Interlude (server) | `Fineage/ref/mobius-ct0-interlude/` and `Fineage-Backend/ref/mobius-ct0-interlude/` |
| L2 Interlude client (UI / assets) | `Fineage/ref/l2-interlude-client/` |

Before changing HUD, MenuWnd, SystemMsgWnd, skillbar, chat, combat formulas, spawns, etc.: **open the matching original** (Interface / Mobius Java / datapack) and match it. See [`ref/README.md`](ref/README.md) and [`Docs/DOC_CANON.md`](Docs/DOC_CANON.md).

## Model selection

| Work | Model |
|------|--------|
| Architecture, complex design, epic planning, protocol/boundary decisions | **Powerful** — Claude Opus / high-thinking |
| Implementation, tests, CI after the design is settled | **Composer** (default) |

Do not burn Opus on routine edits. Do not vibe-code architecture on Composer alone when boundaries are at stake.

## Delivery

1. **Refactor before Gremlin features.** Epic **#131** (client architecture) before heavy epic **#129** (Gremlin smoke) on the client.
2. **Merge when `build-and-test` is green** (branch up to date with `dev`). Do **not** wait for `unity-editmode` — it is not a required status check on `dev` (~20–25 min game-ci). See [`Docs/CI_AND_TESTING.md`](Docs/CI_AND_TESTING.md).
3. **Small PRs.** Prefer narrow diffs; rebase onto `origin/dev` (client) before review.

## GitHub Project

https://github.com/orgs/FioraSolutions/projects/7

Epics: [#131](https://github.com/FioraSolutions/Fineage/issues/131) (arch), [#129](https://github.com/FioraSolutions/Fineage/issues/129) (Gremlin smoke).

## Optional: parallel agents

Only if several agents run at once: use git worktrees so they do not share one dirty tree. See [`Docs/AGENT_BRANCH_PROTOCOL.md`](Docs/AGENT_BRANCH_PROTOCOL.md). Single-agent default is the primary checkout above.
