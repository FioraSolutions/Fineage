# Original sources (local only — gitignored)

These trees are **not** committed. Agents and humans use them as the Interlude source of truth.

| Path | Origin | Use |
|------|--------|-----|
| `ref/mobius-ct0-interlude/` | `~/Downloads/L2J_Mobius_CT_0_Interlude_src` | Server gameplay / formulas / XML datapack (L2J Mobius CT0) |
| `ref/l2-interlude-client/` | `~/Downloads/L2_Interlude_Client` | Client look & feel, UI (`system`, textures), DAT reference |

Same Mobius tree also lives at **Fineage-Backend** `ref/mobius-ct0-interlude/` (gitignored there too).

**Rule:** Fineage server logic, client behavior, and client UI must match these originals unless a Fineage ticket explicitly documents a deliberate divergence.

Refresh from Downloads:

```bash
rsync -a --delete --exclude '.git' ~/Downloads/L2J_Mobius_CT_0_Interlude_src/ ref/mobius-ct0-interlude/
rsync -a --delete ~/Downloads/L2_Interlude_Client/ ref/l2-interlude-client/
```
