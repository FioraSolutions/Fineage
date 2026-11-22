# Fineage — agent entry

Default workspace: **`/Users/nikitarakitin/Projects/Fineage`** (client) and **`/Users/nikitarakitin/Projects/Fineage-Backend`** (server). Work in these primary folders. Do not invent sibling worktrees unless the user explicitly asks for parallel agents.

Do not push to `main`/`dev` directly — land via PR into **`dev`** (client) / **`dev`** (backend; `main` is protected deploy). Doc triage: [`Docs/DOC_CANON.md`](Docs/DOC_CANON.md).

## Parity with originals (hard rule)

Fineage **server business logic**, **client behavior**, and **client look** must match the Interlude originals unless a ticket explicitly documents a deliberate divergence. Do not invent UI layouts, HUD chrome, or gameplay rules when the original exists.

| SoT | Local path (gitignored under `ref/`) |
|-----|--------------------------------------|
| L2J Mobius CT0 Interlude (server) | `Fineage/ref/mobius-ct0-interlude/` and `Fineage-Backend/ref/mobius-ct0-interlude/` |
| L2 Interlude client (UI / assets) | `Fineage/ref/l2-interlude-client/` |

Before changing HUD, MenuWnd, SystemMsgWnd, skillbar, chat, combat formulas, spawns, etc.: **open the matching original** (Interface / Mobius Java / datapack) and match it. That covers art, how it is drawn, and the business rule. If the local source does not have it or the draw path is unclear, search the web before inventing a screen or a formula. See [`ref/README.md`](ref/README.md), [`Docs/DOC_CANON.md`](Docs/DOC_CANON.md), and `.cursor/rules/interlude-sources-first.mdc`.

## UI: original layout, original textures, pixel-perfect (hard rule)

Every client UI element (lobby / char select, login, HUD, status, target, chat, radar/compass, world map, buffs, skillbar, menus, dialogs, tooltips) is built **only from the Interlude original**:

1. **Layout** comes from the original window definitions (Interface.u scripts + window XML/xdat, unpacked from `ref/l2-interlude-client/`): position, size, anchors, child order, fonts, font colors, text alignment, tab geometry. Copy the numbers; do not eyeball or "make it look similar".
2. **Art** is only the original textures (L2UI*.utx and friends, imported through the bake pipeline), with the original slicing/frame pieces. No hand-drawn, recolored, generated, or stand-in sprites, no CSS-like gradients replacing a texture.
3. **Pixel-perfect:** at the original reference resolution the Unity element must match an original-client screenshot pixel for pixel (same frame, same offsets, same text baseline/centering). Every UI PR attaches an original-vs-Fineage side-by-side screenshot of each touched window.
4. Do not add elements that do not exist in the original (e.g. no map inside the radar) and do not drop ones that do.
5. Missing layout data or texture → log a concrete error naming the window/control/texture and stop; never substitute a default look. If the agent cannot access the originals (cloud VM without `ref/`), use the committed extracted layout specs/screenshots; if those are missing, extract them first (on the Mac) instead of guessing.

Agents must use [`l2-unity/Docs/interlude-ref`](l2-unity/Docs/interlude-ref) before re-extracting anything from the official client.


## Model selection

| Work | Model |
|------|--------|
| Architecture, complex design, epic planning, protocol/boundary decisions | **Powerful** — Claude Opus / high-thinking |
| Implementation, tests, CI after the design is settled | **Composer** (default) |

Do not burn Opus on routine edits. Do not vibe-code architecture on Composer alone when boundaries are at stake.

## Delivery

1. **Primary focus:** gameplay debug, graphics/import, Interlude parity. Stabilize A–C bar is **met** — do not block play/assets on LifetimeScope→`Fineage.Di`.
2. **#131 residual** is opportunistic only (when a concrete change hurts). Do not grind DAT/World seams as a finish-line before features.
3. **#129 Gremlin smoke** is fine when useful; do not dump large feature layers onto ACS when a thin seam clearly belongs first.
4. **Merge when `build-and-test` is green** (branch up to date with `dev`). Do **not** wait for `unity-editmode` — it is not a required status check on `dev` (~20–25 min game-ci). See [`Docs/CI_AND_TESTING.md`](Docs/CI_AND_TESTING.md).
5. **Small PRs.** Prefer narrow diffs; rebase onto `origin/dev` (client) before review.

## Where to start

Read **“Agent start here”** in [`Docs/REFACTOR_PLAN_2026-11.md`](Docs/REFACTOR_PLAN_2026-11.md) and **Status Now** in [`l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md`](l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md). Primary: gameplay debug / graphics import. #131 LifetimeScope residual is background. Backend: [`Fineage-Backend/docs/ZONE_MODULARIZATION.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/ZONE_MODULARIZATION.md) + epic [#353](https://github.com/FioraSolutions/Fineage-Backend/issues/353).

## GitHub Project

https://github.com/orgs/FioraSolutions/projects/7

Epics: [#131](https://github.com/FioraSolutions/Fineage/issues/131) (arch), [#129](https://github.com/FioraSolutions/Fineage/issues/129) (Gremlin smoke).

## Optional: parallel agents

Only if several agents run at once: use git worktrees so they do not share one dirty tree. See [`Docs/AGENT_BRANCH_PROTOCOL.md`](Docs/AGENT_BRANCH_PROTOCOL.md). Single-agent default is the primary checkout above.
