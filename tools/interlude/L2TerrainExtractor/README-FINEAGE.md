# L2TerrainExtractor (Fineage vendored fork)

Upstream: [leeleatherwood/L2TerrainExtractor](https://github.com/leeleatherwood/L2TerrainExtractor) (MIT).

Fineage patches (see git history in Fineage repo):

- Always write `staticmeshes.json` per tile (`[]` when a map has zero StaticMeshActors).
- Ground textures: index every client `textures/*.utx`, resolve by TerrainInfo `Package.TextureName` hint first, then by name across all packages; log source `.utx`; exit non-zero when any referenced name is still missing.
- Metadata splat lines use `Package.TextureName` when the cache has the package ref.

Build fat jar (Java 17+):

```bash
cd tools/interlude/L2TerrainExtractor
# If gradle-wrapper.jar is absent, install Gradle 8.5+ and run:
gradle fatJar
```

`run_mac_tile_bake.sh` uses `build/libs/*-all.jar` by default (`L2_TERRAIN_EXTRACTOR` overrides).