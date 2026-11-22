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

## How to regenerate

On a machine with the official client and Cruma 1.2.0:

```bash
tools/interlude-extract/export_client_packages.sh maps/17_25.unr sounds/ambsound.uax
# or pass every maps/*.unr, sounds/*.uax, animations/*.ukx
python3 tools/interlude-extract/build_world_sound_gap.py
```

JSON over 256 KiB is stored gzipped. Do not commit original `.unr` / `.uax` / `.ukx` binaries.
