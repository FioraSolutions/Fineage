# Agent branch protocol (Fineage)

**Default (one agent):** work in the primary checkouts:

- Client: `/Users/nikitarakitin/Projects/Fineage`
- Backend: `/Users/nikitarakitin/Projects/Fineage-Backend`

Feature branch in-place (`feat/…`). No sibling worktree required.

## Always

1. **Never push directly to `main` or `dev`.** Land via PR.
2. Do not amend, rebase, or force-push commits you did not create (unless the user asks).

## Only when several agents run in parallel

Then use **one agent → one branch → one worktree** so checkouts do not fight:

```bash
git fetch origin
git worktree add ../Fineage-<slice> -b feat/<slice> origin/dev
```

Do not `git checkout` another agent’s in-flight branch in a shared dirty tree.

Live notes (if any): [`Docs/PARALLEL_AGENTS.md`](PARALLEL_AGENTS.md).
