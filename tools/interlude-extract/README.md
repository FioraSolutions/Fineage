# Interlude extract

One-shot catalog of the official Interlude client: DAT tables, Unreal package indexes, PE packer notes, and skill-visual resolution.

```bash
python3 tools/interlude-extract/extract_all.py \
  --client "$HOME/Downloads/L2_Interlude_Client/Lineage II" \
  --extract-root "$HOME/Projects/Fineage-ref-extracted" \
  --docs l2-unity/Docs/interlude-ref
```

Committed output is text only (`l2-unity/Docs/interlude-ref`). Decrypted packages, PNG, and WAV stay under `--extract-root` (outside git).

| Script | Role |
|--------|------|
| `extract_all.py` | Entry point: DAT decrypt/parse, package decrypt/index, PE inspect, skill visuals, README |
| `export_skill_effects.sh` | Dump Interlude `skill.usk` skill-effect table via Cruma 1.2.0 `listVisuals()` |
| `ExportSkillEffects.java` | Helper used by that script (prebuilt `.class` is Java 26, matching Cruma 1.2.0) |
| `ddf_parser.py` | l2asm DDF reader (ASCF/CNTR, UNICODE, MTX/MAT, ENBBY) |
| `decrypt.py` | Lineage2Ver413 DAT via l2encdec; Ver111/121 XOR packages |
| `package_index.py` | UE2 name/import/export index |
| `inspect_binaries.py` | PE section/entropy packer ID and string dump |
| `UScriptDump` | UELib decompiler for `.u` scripts (Interface, Engine, …) |
| `ddf/Interlude/*.ddf` | Community DAT schemas (two Interlude corrections: huntingzone `ASCF extra`, logongrp floats) |

`l2encdec` is not committed; use the Darwin binary already under `tools/interlude-dat/l2encdec-bin/`.

| `export_package_properties.sh` | Lossless JSON dump of every property on every object in `skill.usk` plus `lineageeffect.u` |
| `build_skill_effect_placement.py` | Placement TSV/JSON, skill-effect-sounds TSV/JSON, gap report, and sharded dump parts from that JSON |
| `write_lineageeffect_uc.py` | Writes complete LineageEffect `.uc` defaultproperties (including `Begin Object` emitter blocks) from the lossless JSON dump. UScriptDump drops the Emitters arrays; the generator still parses `.uc`, so this is the single path that keeps emitters lossless. |
| `build_skill_visual_runtime.py` | StreamingAssets skill-visual tables plus the missing-assets TSV (leftovers only). |
| `build_skill_effect_sounds.py` | Derived sound table used by the placement gap report. |

| `MeshAnimationNotifyReader.java` | Reads every AnimNotify binding (time + object) from MeshAnimation sequences. Cruma keeps only AttackShot/PreShot/Channeling times; this reader is what the ukx dump and `anim-sequence-notifies` table use. |
| `ExtractUaxWaves.java` | Writes official Sound wave payloads from a `.uax` to WAV/OGG under an extract-root, grouped by the export outer (no package binary). |
| `import_official_item_mon_chr_waves.py` | Copies extracted item/monster/character waves into Resources and writes `official-waves.tsv`. |
| `export_client_packages.sh` | Lossless JSON dump of named packages (maps `.unr`, sounds `.uax`, animations `.ukx`, env/UI `.u`). Uses Cruma `UObjectLoader.exportProperties` so map actors (Emitter, Light, MusicVolume, ZoneInfo, …) keep every property. |
| `build_world_sound_gap.py` | Derived world/sound tables and `docs/interlude-ref/world/gap-report.md` from those dumps vs the Fineage tree. |

