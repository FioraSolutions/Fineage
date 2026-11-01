# Fineage — agent entry

Default workspace: **`/Users/nikitarakitin/Projects/Fineage`** (client) and **`/Users/nikitarakitin/Projects/Fineage-Backend`** (server). Work in these primary folders. Do not invent sibling worktrees unless the user explicitly asks for parallel agents.

Do not push to `main`/`dev` directly — land via PR into **`dev`** (client) / **`main`** (backend, until a `dev` branch exists).

## Model selection

| Work | Model |
|------|--------|
| Architecture, complex design, epic planning, protocol/boundary decisions | **Powerful** — Claude Opus / high-thinking |
| Implementation, tests, CI after the design is settled | **Composer** (default) |

Do not burn Opus on routine edits. Do not vibe-code architecture on Composer alone when boundaries are at stake.

## Delivery

1. **Refactor before Gremlin features.** Epic **#131** (client architecture) before heavy epic **#129** (Gremlin smoke) on the client.
2. **Green CI** on the PR branch before merge.
3. **Small PRs.** Prefer narrow diffs; rebase onto `origin/dev` (client) before review.

## GitHub Project

https://github.com/orgs/FioraSolutions/projects/7

Epics: [#131](https://github.com/FioraSolutions/Fineage/issues/131) (arch), [#129](https://github.com/FioraSolutions/Fineage/issues/129) (Gremlin smoke).

## Optional: parallel agents

Only if several agents run at once: use git worktrees so they do not share one dirty tree. See [`Docs/AGENT_BRANCH_PROTOCOL.md`](Docs/AGENT_BRANCH_PROTOCOL.md). Single-agent default is the primary checkout above.
