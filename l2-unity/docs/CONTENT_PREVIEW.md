# Content Preview — offline visual iteration

## Problem

Every time you want to check how a character, NPC, or weapon looks in-game
you have to: start the login server → log in → select character → wait for
zone-server to confirm → load the world map → find the entity. This takes
30–90 seconds per iteration and requires two running Java processes.

## Solution

A dedicated **Preview scene** + **Editor window** that:

1. Loads data tables (items, npcgrp, models) at play-mode start — **no
   network, no login, no zone-server**.
2. Instantiates a character or NPC prefab with face/hair/weapon straight
   from the editor window.
3. Provides an orbit camera so you can inspect the model from any angle.

### Why this approach (vs. alternatives)

| Approach | Pros | Cons |
|----------|------|------|
| **Prefab Mode** (double-click .prefab) | Zero setup | No animations, no data tables, no weapon equip |
| **Standalone test scene with hard-coded GO** | Simple | Manual, no parametric control, stale fast |
| **Full login mock (fake server packets)** | Closest to prod | Huge effort, fragile, still slow |
| **→ Editor Window + Preview Scene** | Fast (<5 s), parametric, extensible | Tables load in editor memory |

## Quick start

1. **Menu → Fineage → Content Preview** (or `Ctrl+Alt+P` / `Cmd+Alt+P`).
2. Pick the **Character** tab or **NPC / Monster** tab.
3. Configure race, face, hair, weapon, or NPC id.
4. Click **▶ Open Preview Scene & Play**.
5. The tool creates `Assets/Resources/Scenes/Preview.unity` on first use,
   enters Play Mode, and spawns the entity.
6. Use **Spawn / Respawn** to iterate without leaving Play Mode.
7. **Mouse drag** = orbit, **scroll** = zoom, **middle-drag** = pan.
8. Click **■ Stop Preview** when done.

## What it loads

The bootstrap (`PreviewSceneBootstrap`) calls:

```
ItemTable.Instance.Initialize()  +  CacheItems()
NpcgrpTable.Instance.Initialize()
NpcNameTable.Instance.Initialize()
ModelTable.Instance.Initialize()
CharacterBuilder  (singleton)
```

No networking classes, no VContainer scopes, no `GameClient`.

## Character preview

- **Race / Sex**: dropdown for all 14 `CharacterRaceAnimation` values.
- **Face**: 0–5.
- **Hair Style / Color**: 0–5 / 0–3.
- **Weapon ID**: L2 item id (e.g. `2500` = Crystal Dagger). `0` = bare hands.
- **Shield ID**: L2 item id. Separate from weapon.
- The idle animation plays automatically (`wait` state).

## NPC / Monster preview

- **NPC ID**: integer from `Npcgrp_Classic.txt` (e.g. `20006` = Orc Archer).
- **Name filter**: type ≥2 chars to search by name in the loaded
  `NpcNameTable` (up to 50 results).
- Weapons from `npcgrp.Rhand` / `npcgrp.Lhand` are auto-equipped.

## File layout

```
Assets/
  Editor/ContentPreview/
    ContentPreviewWindow.cs   — EditorWindow (menu, UI, spawn logic)
  Scripts/ContentPreview/
    PreviewSceneBootstrap.cs  — data-table loader (MonoBehaviour)
    PreviewOrbitCamera.cs     — orbit/zoom/pan camera
  Resources/Scenes/
    Preview.unity             — auto-created on first use
```

## Limitations & follow-ups

- **Armor equip** is not wired yet (armor system is more complex — needs
  mesh swap per race).  See GitHub issue: *Armor equip preview*.
- **Skill VFX preview** requires the composite effect pipeline — separate
  tool.  See GitHub issue: *Skill VFX preview*.
- **NPC catalog picker** with thumbnails / npcgrp search is a future
  improvement.
- **Record / replay packets** for offline replays of combat sequences.

## Does it break the main game?

No.

- All new files are in `Editor/ContentPreview` (editor-only) and
  `Scripts/ContentPreview` (two tiny MonoBehaviours that are not referenced
  by any production scene or prefab).
- The Preview scene is auto-created and not linked to any build profile.
- No changes to existing spawner, gear, or table code.
