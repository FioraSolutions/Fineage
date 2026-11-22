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

## Skill-effect placement (skill.usk lossless dump)

`Animations/skill.usk` holds one `SkillVisualEffect` per skill that has a unique
effect, plus the `SkillAction_LocateEffect` objects those visuals reference.
Emitter and effect-actor defaults live in `system/lineageeffect.u`.

The committed dump is a full property tree, not the short Cruma `listVisuals()`
view. Regeneration (official Interlude client + Cruma 1.2.0):

```bash
tools/interlude-extract/export_package_properties.sh
python3 tools/interlude-extract/build_skill_effect_placement.py
```

| Path | Contents |
|------|----------|
| `packages/usk-dump/skill.usk.json` | Every export and import in `skill.usk`, every tagged property |
| `packages/usk-dump/lineageeffect.u.manifest.json` | Index of the sharded `lineageeffect.u` property dump (class defaults + emitters) |
| `packages/usk-dump/skill.usk.references.json` | Packages and class names `skill.usk` references |
| `skills/skill-effect-placement.tsv` | Derived placement/timing table (one row per LocateEffect action) |
| `skills/skill-effect-placement.json` | Same rows plus enum maps and declared defaults |
| `skills/skill-effect-sounds.tsv` | Every skill visual / effect sound (skillsoundgrp + LineageEffect SpawnSound) |
| `skills/skill-effect-sounds.json` | Same sound rows, including volume, radius, loop and whether Fineage plays them |
| `skills/gap-report.md` | Original fields and sounds the client does not yet use, with skill counts |

### Placement columns

All names below exist in the original data (or are a documented remap / declared default).

**From SkillVisualEffect:** `skill_id` (leading digits of the object name; five Interlude visuals use a suffix such as `1012_sec` or `4641_a`, stored in `object_name` / `visual_key`), `package` (outer Package, so the key is `skill.el.1177`), `desc` (Name), `flying_time` (`FlyingTime`), `phase` (`CastingActions`, `ShotActions`, `ExplosionActions`, `ChannelingActions`, `PreshotActions`), `action_index`, `specific_stage` (`SkillActionInfo.SpecificStage`).

**From SkillAction_LocateEffect:** `effect_class` (`EffectClass`), `attach_on_raw` / `attach_on_name` (`AttachOn`, Interlude `EAttachMethod` in Engine.u: none, rh, lh, bone, alias, trail, rf, lf), `attach_bone_name`, `offset_*`, `spawn_delay` (`SpawnDelay`), `b_absolute`, `b_on_multi_target`, `b_relative_to_cylinder` (default **true** in Engine.u when the flag is not serialized), `b_size_scale`, `b_spawn_on_target`, `b_use_character_rotation`.

**Cruma remap (not a new field):** `attach_on_visual` is `SkillEffects.VISUAL_ATTACH` applied to the Interlude enum (`[0, 2, 3, 7, 8, 9, 101, 102]`). The short `skill-effects-engine.tsv` writes this remapped value.

**From the referenced LineageEffect class defaults (joined, empty when the property is absent):** `draw_scale`, `rotation_*`, `rot_per_second_*`, `b_rot_emitter`, `b_self_rotation`, `b_directional`, `speed`, `acc_speed`.

Cruma's typed Action also exposes `scale`, `turn`, `curve`, `forceLife`, `adjustLife`, `chaining` and Action-level `channeling`. A full pass over Interlude `listVisuals()` found no non-default values for those fields, and they are not properties of `SkillAction_LocateEffect`, so they are not in the table. Duration / loop live on emitters inside `lineageeffect.u`, not on the LocateEffect.

The runtime copy is `Assets/StreamingAssets/Data/Meta/SkillVisual/skill-effect-placement.txt`.

UScriptDump `.uc` files drop the `Emitters` arrays (`/* Array type was not detected. */`). `tools/interlude-extract/write_lineageeffect_uc.py` rewrites complete defaultproperties (including `Begin Object` emitter blocks) from the lossless `lineageeffect.u` dump so L2EffectGenerator can import them without a second parser.

Sound rows are a second derived view: `skills/skill-effect-sounds.tsv`. Each row is one official cue — `skillsoundgrp` `spelleffect_sound_*` (cast / launch / impact), `shoteffect_*`, `expeffect_*`, hero voice (`*_sub` / `*_throw`), and every `Emitter.SpawnSound` (or other Sound object) reached from a skill visual's EffectClass. Columns include the sound package, cue name, the phase it plays in, `fineage_plays` / `fineage_play_path` (whether SkillCastSound or SkillEffectSpawnSound has a code path), and whether a clip exists under `Resources/Data/Sounds`. World, ambient and footstep audio is issue #1670 and is not listed here.
 The resolver may place, orient and time an effect only from this table, through one shared path. A missing required field logs the skill id, phase and field name. See `skills/gap-report.md`.

