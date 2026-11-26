# Official QuadVisibilityBitmap words

2048 signed 32-bit words per in-repo Interlude tile (256×256 = 65536 bits).
Bit 0 of word 0 is official quad 0; a 0-bit hides the quad.

Dumped with Cruma's typed `UTerrainInfoActor.getQuadVisibilityBitmap()` via
`tools/interlude-extract/DumpTerrainHoles.java`. The #1670 JSON only stored
`array_size=2048` with no payload.

Apply into Meta / t3d:

```bash
python3 tools/interlude-extract/apply_quad_visibility.py \
  --words-dir l2-unity/docs/interlude-ref/world/quadvisibility \
  --maps-dir l2-unity/Assets/Resources/Data/Maps
```

Counts are in `../render-parity-holes.tsv`.
