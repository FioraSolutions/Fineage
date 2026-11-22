# Interlude world and sound dumps

Lossless property dumps of the official Interlude client, for
[Fineage #1670](https://github.com/FioraSolutions/Fineage/issues/1670).

## Layout

- `maps/` — every `maps/*.unr` grid tile (gzipped JSON). Every export, every property.
- `env/` — `Lobby`, `entry`, `skylevel`, `ship_position`, plus `lineageenv.u`, `interface.u`, `nwindow.u`, `LineageDeco.u`, `Fire.u`.
- `anims/` — every `animations/*.ukx` (notifies + MeshAnimation sequence timing; keyframe tracks omitted).
- `../sounds/packages/` — every `sounds/*.uax` (Sound names and byte lengths; wave payloads omitted).
- `package-dump-index.json` — inventory of the dump itself.
- Derived tables and `gap-report.md` in this folder and in `../sounds/`.
- `ti-shore-tiles-heightmaps.png` — official heightmaps for the TI shoreline tiles `16_26`, `17_26`, `18_25`, `18_26` next to already-imported neighbors (no Unity render).
- `render-parity.md` plus `render-parity-tiles.tsv`, `render-perf.tsv`, and `render-parity-holes.tsv` — world render parity and static perf audit for [#1673](https://github.com/FioraSolutions/Fineage/issues/1673).

## How to regenerate

On a machine with the official client and Cruma 1.2.0:

```bash
tools/interlude-extract/export_client_packages.sh maps/17_25.unr sounds/ambsound.uax
# or pass every maps/*.unr, sounds/*.uax, animations/*.ukx
python3 tools/interlude-extract/build_world_sound_gap.py
```

JSON over 256 KiB is stored gzipped. Do not commit original `.unr` / `.uax` / `.ukx` binaries.

Render parity (no Unity required for the static audit):

```bash
# Official hole bitmaps (Cruma typed TerrainInfo; needs the official client):
# javac --release 25 -cp "$CRUMA_JAR:tools/interlude-extract" tools/interlude-extract/DumpTerrainHoles.java
# java -cp ".:$CRUMA_JAR:tools/interlude-extract" DumpTerrainHoles "$CLIENT_ROOT" l2-unity/docs/interlude-ref/world/render-parity-holes.tsv

python3 tools/interlude-extract/build_render_parity.py
```

Do not run Unity batchmode for this audit if the editor may already be open on another checkout.
Frame-time steps (not run here) are in `render-parity.md`.
