# L2TerrainExtractor (Fineage vendored fork)

Upstream: [leeleatherwood/L2TerrainExtractor](https://github.com/leeleatherwood/L2TerrainExtractor) (MIT).

Fineage patches (see git history in Fineage repo):

- Always write `staticmeshes.json` per tile (`[]` when a map has zero StaticMeshActors).
- Resolve ground textures from regional `T_*.utx` packages, then from per-tile `t_XX_YY.utx` for still-missing splat layers.
- Log every missing referenced ground texture by name (non-verbose).

Build fat jar (Java 17+):

```bash
cd tools/interlude/L2TerrainExtractor
# If gradle-wrapper.jar is absent, install Gradle 8.5+ and run:
gradle fatJar
```

`run_mac_tile_bake.sh` uses `build/libs/*-all.jar` by default (`L2_TERRAIN_EXTRACTOR` overrides).