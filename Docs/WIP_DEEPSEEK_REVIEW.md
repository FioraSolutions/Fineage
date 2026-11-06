# WIP review — uncommitted DeepSeek piles (2026-11-06)

Not committed. Stashed off `dev` so doc-sync PRs stay clean:

- Client: `git stash list` → `wip/deepseek-client-autoshot-skillbar`
- Backend: `git stash list` → `wip/deepseek-backend-consumables-regions`

Restore with `git stash pop` onto a feature branch. Verdict before merge/split:

## Client (`Fineage` dirty)

| Area | Files | Verdict |
|------|-------|---------|
| Auto soulshot | `NetAutoShot.cs` (new), `FineageZoneTransport`, `NetSession`, `NetworkCommands`, skillbar slots/window | **Promising.** Matches Mobius RequestAutoSoulShot / ExAutoSoulShot intent; uses existing `NetOpcode.RequestAutoSoulShot`. Static `NetAutoShot` + `ZoneCommands` fits current patterns but is another locator — fine for MVP, track under #142 later. |
| Skillbar / inventory UX | `SkillbarSlot`, `AbstractSkillbar`, inventory slots, `SlotDragManipulator`, `ToolTipSimple`, `PlayerShortcuts` | **Coupled to auto-shot.** Null-safe inventory before skillbar init is good. Right-click = left-click for shots is a Fineage UX choice — OK if intentional. |
| Spawn | `EntitySpawnShared.cs` (~+100) | **Needs focused review** — large hunk; confirm AOI/spawn budget interaction with #601, no new `*.Instance` seams for frozen types. |

**Client risks:** optimistic toggle before zone ack (Apply on S→C is correct; UI RefreshMarks on click may flash wrong if zone rejects); `async void` send methods (pre-existing NetSession style); ensure `EtcItemgrpTable` classification covers spiritshots as claimed.

**Recommend:** one PR for auto-shot + skillbar marks; separate PR if `EntitySpawnShared` is unrelated.

## Backend (`Fineage-Backend` dirty)

| Area | Files | Verdict |
|------|-------|---------|
| Consumables | `ZoneWorldConsumables.cs`, `ItemSkillCatalog.cs`, `data/item-skills.json`, `gen_item_skills.py`, `ConsumableUseTests.cs` | **Solid direction.** Mobius ItemSkills flattened to JSON effects; tests for mana/heal/SoE/compressed shots. HoT collapsed to instant heal — document as known parity gap. |
| Map regions | `MapRegionCatalog.cs`, `data/map-regions.json`, `gen_map_regions.py`, `RegionSpawnTests` | **Useful for SoE / town respawn.** Confirm Scroll of Escape uses catalog, not hardcoded TI. |
| Abnormals | `ZoneAbnormals` slot + dispel-by-slot stub, `ZoneAbnormalEntry` | **Partial.** Comments admit `abnormalType` not imported yet — dispel may no-op; OK if tests assert that. |
| Combat / gear / ground tests | small diffs in `ZoneCombat`, `ZoneWorldGear`, `NetGroundItemTests` | Spot-check for drive-by; keep with consumable PR only if required. |
| Elven spawns | `elven_starting.json` huge rewrite + `gen_elven_spawns.py` | **Separate PR.** Content churn should not ride consumables. |
| `.gitignore` | +2 lines | Confirm only intended ignores. |

**Backend risks:** wire `UseItem` path must route non-equippable through `TryUseConsumable` (verify call site in gear/use handler); catalog LoadDefault path in Zone csproj; generated JSON size in git is fine if MobiusImport-aligned.

**Recommend:** PR A consumables + item-skills + tests; PR B map-regions; PR C elven spawns; do not squash with doc-sync.

## Do next

1. Land doc-sync PRs (client + backend) with **only** docs/README/AGENTS/ROADMAP.
2. Stash or branch the DeepSeek code onto `feat/…` branches off `dev`.
3. Run Backend `dotnet test` on consumable/map-region PRs before review.
4. Client: playtest shot toggle + skillbar mark; confirm opcode already on Net contract both sides.
