# Parallel agents — live status

**Updated:** 2026-10-27 (coordination pass)  
**Risk:** Branch war **mitigated** (was YES on shared dirty checkout; worktrees isolated). Do not re-merge into one tree.

## Workstreams

| Stream | Agent (hint) | Branch | Worktree | Status |
|--------|--------------|--------|----------|--------|
| Content Preview | 95224829 / prior | `feat/content-preview` | — | **DONE** — PR [#126](https://github.com/FioraSolutions/Fineage/pull/126) merged into `dev` + `main`. Remote branch deleted. **Leave alone.** |
| Net foundation | 116c6d10 | `feat/net-foundation` | `../Fineage-net` | **OPEN** PR [#111](https://github.com/FioraSolutions/Fineage/pull/111) → `dev`. Tip `8221da55a`. Needs merge/rebase of `origin/dev` (includes #126) before land. |
| Client arch review | fc359f69 | `chore/client-arch-review` | `../Fineage-arch` | **Committed** `ee3ad774a` (IGameManager + review doc + boundary markers); remote `origin/chore/client-arch-review` exists. Based on post-#126 `dev`. No PR yet. |
| Coordination | this pass | `chore/agent-coordination` | primary `Fineage/` | Protocol + this STATUS. Keep primary clean. |

## Ownership (do not cross-edit)

| Owner | Owns |
|-------|------|
| **net** | `Docs/ARCHITECTURE_NET.md`, `tools/zone-server/Net/**`, `tools/zone-server/Interest/**`, `l2-unity/.../Adapters/V2/IZoneTransport.cs` |
| **arch** | `GameManager.cs`, `IGameManager.cs`, `EventBus.cs`, `l2-unity/docs/CLIENT_ARCHITECTURE_REVIEW.md` |
| **content-preview** | nothing further — already merged |

## Conflict hotspots

- **`GameManager.cs`** — arch is actively region/interface-refactoring; net wants consumers to depend on `IZoneTransport` (not editing GameManager in #111 yet, but follow-ups will). Serialize: finish/merge net first, then arch rebases.
- **`tools/zone-server/**`** — net only.
- **Roadmap / architecture docs** — net owns `ARCHITECTURE_NET`; arch owns client review doc under `l2-unity/docs/`.

## Merge order (recommended)

1. ~~**content-preview** (#126)~~ — merged.
2. **net-foundation** (#111) — rebase/merge `origin/dev`, CI green, merge to `dev`.
3. **client-arch-review** — open PR after #111 lands (or rebase onto post-#111 `dev` if overlapping).
4. Sync **main** ← **dev** only via intentional PR (as with #110).

**Do not touch `main`/`dev` except via PR.** No force-push. No resetting other agents’ commits.

## Shared-checkout incident (resolved)

Primary tree was on dead `feat/content-preview` with arch WIP mixed in (`GameManager` dirty; review files appearing/disappearing). Actions taken:

1. Stashed stray primary `GameManager` hunk (`stash`: “ARCH WIP: GameManager TableInitializer region…” — likely superseded by richer WIP in `Fineage-arch`; safe to drop after arch agent confirms).
2. Ensured worktrees: `Fineage-net`, `Fineage-arch`.
3. Synced `chore/client-arch-review` → `origin/dev` with WIP preserved.
4. Parked primary on `chore/agent-coordination` (tracks `origin/dev`), clean aside from these docs.

## Agent instructions (short)

See `docs/AGENT_BRANCH_PROTOCOL.md`. Work only in your worktree/branch. Interrupt of other agents is unavailable — read this file before checkout/push.
