# Interlude reference catalog

Text tables and indexes extracted once from the official Interlude client,
so agents do not have to unpack the same files on every task.

Re-run (Mac, official client present):

```bash
python3 tools/interlude-extract/extract_all.py \
  --client "$HOME/Downloads/L2_Interlude_Client/Lineage II" \
  --extract-root "$HOME/Projects/Fineage-ref-extracted" \
  --docs l2-unity/Docs/interlude-ref
```

The entry script lives at `tools/interlude-extract/extract_all.py`.
DAT field schemas are in `tools/interlude-extract/ddf/Interlude/` (community l2asm DDF set,
with two Interlude-specific corrections: `huntingzone-e` uses `ASCF extra` like the C5 layout,
`logongrp` positions are IEEE floats).

Raw decrypted binaries, PNG, and WAV are **not** committed. They go to
`~/Projects/Fineage-ref-extracted/` on the machine that ran the extractor.
Single committed files are split if they would exceed ~20 MB.

## Layout

| Path | Contents |
|------|----------|
| `dats/*.tsv` | Decrypted `system/*.dat` tables (Lineage2Ver413) |
| `dats/summary.json` | Row counts and parse status per table |
| `packages/exports.tsv` | Every Unreal export: package, object, class, size |
| `packages/packages.json` | Per-package name/export/import counts |
| `binaries/packers.tsv` | PE packer identification for client binaries |
| `binaries/*.interesting-strings.txt` | Keyword-filtered strings from each PE |
| `skills/skill-visuals.tsv` | skill_id × animation / desc / icon / effect resolution |
| `skills/skill-visuals-summary.json` | Resolution counts |
| `scripts/` | Decompiled UnrealScript from `Interface.u` and other `.u` with scripts (if produced) |

## How it is produced

1. `system/*.dat` are Lineage2Ver413; decrypted with open-l2encdec (`-p 413 -l`).
2. Tables are parsed with the DDF schemas (ASCF/CNTR packed counters, UNICODE, MTX/MAT, ENBBY).
3. Unreal packages (`.u` `.ukx` `.utx` `.usx` `.unr` `.uax` `.usk`) are Lineage2Ver111 (XOR 0xAC)
   or Lineage2Ver121 (XOR from the lowercase filename). The 28-byte header is stripped.
4. Package export indexes are built from the UE2 name/import/export tables.
5. PE binaries are classified from section names, entry point, and entropy. Only `engine.dll`
   is packed (Themida). The others are ordinary PE and can be string-scanned directly.
6. Skill visuals are resolved from Interlude `skillgrp` (`ani_char`, `desc`, `icon_name`, `extra_eff`)
   plus `LineageEffect` class names from the package index. Hardcoded C++ maps inside packed
   `engine.dll` are recorded as unresolved until that binary can be dumped.

## DAT tables (0 parsed)

| File | Rows | Status |
|------|------|--------|

## Packages

Indexed 0 of 0 packages; 0 index failures (see `packages/index-failures.json`).

## Binaries

| File | Packer | Unpacked |
|------|--------|----------|

## Skill visuals

```json
{
  "skill_rows": 29748,
  "unique_skill_ids": 2630,
  "rows_with_ani": 19926,
  "rows_with_desc": 12161,
  "rows_with_icon": 29748,
  "rows_desc_is_effect_key": 8967,
  "rows_matched_effect_class": 4598,
  "rows_unresolved": 6044,
  "unique_id_resolution": {
    "effect_class_by_name": 218,
    "skillgrp_desc_key": 779,
    "skillgrp_ani_only": 200,
    "skillgrp_extra_eff": 196,
    "unresolved": 1237
  },
  "effect_classes_indexed": 863
}
```

## Scripts

Decompiled UnrealScript for `Interface.u` is under `scripts/Interface.u/`.
Class name lists for Engine, GamePlay, Core, LineageEffect, nwindow, UWindow, Fire, and Editor are in `scripts/*.classes.txt`.
Full `.uc` dumps (including LineageEffect defaultproperties, which may be incomplete where emitter arrays do not deserialize) live in `~/Projects/Fineage-ref-extracted/scripts/`.

## Local extract root

On the machine that ran the extractor:

`/Users/nikitarakitin/Projects/Fineage-ref-extracted/`

- `dat/decrypted/` — raw decrypted DAT
- `packages/decrypted/` — 862 Unreal packages with the 28-byte header removed
- `scripts/` — full UnrealScript dumps
- `binaries/` — complete string dumps
- `logs/` — extract run logs

PNG texture export and WAV dump are optional follow-up passes against the decrypted packages (see `packages/classes.tsv` / `exports.part*.tsv` for Texture and Sound object names). `engine.dll` is Themida-packed; no dump of its code section is stored here.

## Notes

- `hairaccessarygrp.dat` and `helmetgrp.dat` have no community DDF; they are decrypted only.
- `orc.dll` is also Themida-packed (besides `engine.dll`).
- `servername-e.dat` header count is 127; 73 extra trailing records were parsed up to id 199.
