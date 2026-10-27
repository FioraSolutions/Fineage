# Fineage — agent coordination

Entry point for Cursor agents and vibe-coding on this repo. Prefer **worktrees + small PRs into `dev`**. Do not push to `main`/`dev` directly.

## Model selection (mandatory)

| Work | Model |
|------|--------|
| Architecture, complex design, epic planning, non-trivial refactor strategy, protocol/boundary decisions | **Powerful** — Claude Opus / high-thinking (or equivalent strong reasoning model) |
| Implementation, mechanical edits, tests, CI wiring, “code hammering” after the design is settled | **Composer** (default coding agent) — omit heavy models |

Do **not** burn Opus on routine file edits. Do **not** vibe-code architecture on Composer alone when boundaries, merge order, or cross-cutting design are at stake.

## Delivery rules (refactor-first)

1. **Refactor before Gremlin features.** Epic **#131** (client architecture) gates meaningful work on epic **#129** (Gremlin vertical smoke). Land architecture/guardrails first; do not pile smoke features onto a still-coupled client.
2. **PRs to `dev` need green CI.** GitHub Actions `build-and-test` (or the repo’s current required check) must pass before merge. Fix red CI on your branch; do not merge “and fix later.”
3. **One agent → one branch → one worktree.** Follow [`Docs/AGENT_BRANCH_PROTOCOL.md`](Docs/AGENT_BRANCH_PROTOCOL.md). Live ownership: [`Docs/PARALLEL_AGENTS.md`](Docs/PARALLEL_AGENTS.md).
4. **Epic order:** progress **#131** (and child issues) before treating **#129** smoke as unblocked for client-heavy paths. Datapack/zone smoke slices may proceed only where they do not fight the arch workstream — still coordinate via worktrees.
5. **Small PRs.** Prefer narrow diffs; rebase onto `origin/dev` before asking for review.

## GitHub Project

Track delivery on **Fineage Project #7**:

https://github.com/orgs/FioraSolutions/projects/7

Link issues/PRs to that board. Epics of note: [#131](https://github.com/FioraSolutions/Fineage/issues/131) (arch), [#129](https://github.com/FioraSolutions/Fineage/issues/129) (Gremlin smoke).

## Cursor Project (manual — no API)

Cursor **Projects** are not creatable via `cursor-app-control` today (`open_resource` cannot open `command:` / product-protocol URIs; `cursor_dialog` only manages **user rules**). Create one in the UI:

1. Open **Cursor → Projects** (or Command Palette → “Projects”).
2. **New Project** → point it at this repo root:  
   `/Users/nikitarakitin/Projects/Fineage`  
   (or the worktree you use as primary coordination checkout).
3. Attach or note GitHub Project **#7** in the Project description/instructions:  
   https://github.com/orgs/FioraSolutions/projects/7
4. Ensure project/agent instructions pick up this `AGENTS.md` and `.cursor/rules/` (repo rules apply automatically when the folder is the workspace).

Optional: keep the primary checkout on `chore/agent-coordination` or `dev` and put feature work in sibling worktrees (`../Fineage-net`, `../Fineage-arch`, etc.).

## Suggested worktree layout

```text
Fineage/                 # primary — coordination; keep clean
Fineage-net/             # feat/net-foundation (or follow-ups)
Fineage-arch/            # chore/client-arch-review / #131 work
Fineage-gremlin-preview/ # smoke / content-preview follow-ups
```

```bash
git fetch origin
git worktree add ../Fineage-arch -b chore/client-arch-review origin/dev   # if missing
```

## Conflict hotspots

See `Docs/AGENT_BRANCH_PROTOCOL.md`. Serialize or rebase carefully around `GameManager.cs`, `Adapters/V2/*`, `tools/zone-server/**`, and architecture docs.
