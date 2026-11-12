# Login forest vista (Interlude Camera25)

Retail Lobby.unr stack — **no screen-space path/card hacks**:

1. **Camera25** FOV 35 (`LoginForestActors.json`)
2. **`lobby_back`** static mesh + **Jungle03** (Unlit Geometry) — SoT mist plate from Lobby.unr
3. Additive **l2_lobby terrain** kept (ships/FX hidden); dirt = real `field_deco_T/beachstone1` Unlit + mud tint
4. Lobby.unr static meshes near Camera25 + foliage sway
5. BGM: `event:/Music/l2_lobby/intro` (`music/intro.ogg`)

## Do

- Place `lobby_back` from actors JSON with Jungle03 — do not invent a camera-child Jungle quad.
- Keep Terrain/Brush; hide additive ships/sky/FX only.
- `lobby_terrain_t/D_M_*` are near-black modulate leftovers — do not use raw as albedo.
- Proof: `LoginForestShot.RunCli` + `-shotOut` (Editor must not hold the project lock).

## Don’t

- Screen-space khaki/dirt slabs (`LoginScreenGround` / `LoginForestPath`).
- Skybox/Panoramic Jungle03 (cyan warp).
- Skipping `lobby_back` because an earlier black mat bug — fix the material instead.
