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
| `skill-effects-engine.tsv` | SkillID → cast/shot/hit actions from `Animations/skill.usk` (Cruma `listVisuals`) |
| `skills/skill-effects-coverage.json` | Union of skillgrp `desc` paths + this table vs 2630 skill IDs |
| `skills/skills-without-unique-effect.tsv` | Skill IDs with no `desc` path and no skill.usk entry |
| `skills/skill-visual-missing-assets.tsv` | Catalog skills whose desc or usk effect class is not an imported prefab |
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
   plus `LineageEffect` class names from the package index.
7. The C3–Interlude skill-effect table (SkillID → emitters, attach points, phases, timing) is in
   `Animations/skill.usk` (`SkillVisualEffect` objects), not in packed `engine.dll`. Cruma 1.2.0
   reads it with `SkillEffects.listVisuals()`. Export:
   `tools/interlude-extract/export_skill_effects.sh` (needs a Cruma 1.2.0 install, e.g.
   `~/Tools/Cruma/Cruma.app`, pointed at the official Interlude client). C1/C2 still keep the
   equivalent table only inside `Engine.dll`, which Cruma cannot read.

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


## Skill-effect table (`skill.usk`)

Interlude (and Chronicle 3) store per-skill visual actions in `Animations/skill.usk`, one
`SkillVisualEffect` object per entry, named by skill ID (for example `skill.el.1177` for
Wind Strike). Cruma 1.2.0 dumps this table with `SkillEffects.listVisuals()`.

The table was exported from the official Interlude client with Cruma 1.2.0
(`ExportSkillEffects`, same `listVisuals()` call the viewer uses). Re-run:

```bash
tools/interlude-extract/export_skill_effects.sh
# optional:
#   CRUMA_CLIENT="/path/to/Lineage II" \
#   CRUMA_JAR="$HOME/Tools/Cruma/Cruma.app/Contents/app/cruma.jar" \
#   tools/interlude-extract/export_skill_effects.sh l2-unity/Docs/interlude-ref/skill-effects-engine.tsv
```

Coverage (unique skill IDs in Interlude `skillgrp` / `skillname`):

- 2630 skill IDs in the catalog
- 900 have a `skillgrp` `desc` effect path (`skill.wh.1062` and the like)
- 243 `skill.usk` entries / 238 unique IDs (three IDs — 831, 832, 1058 — are not in the 2630)
- 28 catalog IDs are new from this table (including Wind Strike 1177); 207 already had a `desc` path
- Union of `desc` paths + this table: 928 IDs
- Remaining 1702 IDs have no unique effect in `skillgrp.desc` or `skill.usk`; the original client plays only the character animation (`ani_char`) and sound for those, if any. See `skills/skills-without-unique-effect.tsv`.

Cruma does not unpack Themida-packed `engine.dll`. For Interlude that is unnecessary: the
viewer already loads this table from `skill.usk`. The leftover packed map is a C1/C2 issue.

The client resolver (`SkillVisualCatalog`) uses this table after the skillgrp `desc` path.
Runtime copies live in `Assets/StreamingAssets/Data/Meta/SkillVisual/` and are rebuilt by
`tools/interlude-extract/build_skill_visual_runtime.py`. A skill in the 928 whose effect
prefab is not under `Assets/Resources/Data/Effects` is listed in
`skills/skill-visual-missing-assets.tsv` (skill id, level, source, visual key, asset).
The 1702 IDs in `skills-without-unique-effect.tsv` resolve to no effect. Regenerating the
missing report needs the official client packages and the Unity effect importer; this
workspace does not have them, so those classes were not imported.

## Notes

- `hairaccessarygrp.dat` and `helmetgrp.dat` have no community DDF; they are decrypted only.
- `orc.dll` is also Themida-packed (besides `engine.dll`).
- `servername-e.dat` header count is 127; 73 extra trailing records were parsed up to id 199.
