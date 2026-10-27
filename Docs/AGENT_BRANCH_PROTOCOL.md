# Agent branch protocol (Fineage)

Parallel Cursor agents share **one primary checkout** unless they use git worktrees. Branch wars happen when two agents `git checkout` each other’s branches or edit the same dirty tree.

## Rules (mandatory)

1. **One agent → one branch → one worktree.** Never reuse another agent’s branch mid-flight.
2. **Do not `git checkout` a branch that another agent owns** while that agent is running. Prefer `git worktree add`.
3. **Commit and push only from your branch.** Do not amend, rebase, or force-push anyone else’s commits.
4. **Never push directly to `main` or `dev`.** Land only via PR into `dev`, then sync `main` via PR when intentional.
5. **If the shared checkout is dirty and not yours:** stop. Stash only your own WIP with a clear message (`AGENT <id>: …`), or move to your worktree. Do not discard others’ files.
6. **Prefer worktrees for parallel edits:**
   - Net: `../Fineage-net` → `feat/net-foundation`
   - Arch: `../Fineage-arch` → `chore/client-arch-review`
   - Primary: `/Users/nikitarakitin/Projects/Fineage` → coordination / idle on `chore/agent-coordination` or `dev`

## Suggested layout

```text
Fineage/           # primary — leave clean; coordination docs only
Fineage-net/       # feat/net-foundation (PR #111)
Fineage-arch/      # chore/client-arch-review
```

Create if missing (from repo root, after `git fetch`):

```bash
git worktree add ../Fineage-net feat/net-foundation
git worktree add ../Fineage-arch chore/client-arch-review   # or -b from origin/dev
```

## Conflict hotspots (serialize or rebase carefully)

| Path | Why |
|------|-----|
| `l2-unity/Assets/Scripts/Game/Manager/GameManager.cs` | Arch refactor + net transport decoupling |
| `l2-unity/Assets/Scripts/Networking/Adapters/V2/*` | Net foundations |
| `tools/zone-server/**` | Net session/AOI/cache |
| `Docs/ARCHITECTURE_*.md` / `docs/**` / `ROADMAP*` | Doc collisions |

## Merge order

See `docs/PARALLEL_AGENTS.md`. Default: **content-preview (done) → net-foundation → client-arch-review**.
