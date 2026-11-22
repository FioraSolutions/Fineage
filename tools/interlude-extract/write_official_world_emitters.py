#!/usr/bin/env python3
"""Build official world-map Emitter placements and L2EffectGenerator .uc folders.

Reads lossless .unr dumps (maps + env). One unique particle definition (hash of
child emitter properties) becomes Data/Effects/world/we_<hash>/we_<hash>.uc.
Placement rows go to StreamingAssets official-world-emitters.json.gz.

Does not invent children for empty Emitters slots. Logs those actors.
WetTexture and other unresolved object refs stay as errors in the report — they
are not replaced with a stand-in texture.
"""
from __future__ import annotations

import gzip
import hashlib
import json
import sys
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMP_ROOT = ROOT / "l2-unity/docs/interlude-ref/world"
EFFECT_ROOT = ROOT / "l2-unity/Assets/Resources/Data/Effects/world"
JSON_OUT = ROOT / "l2-unity/Assets/StreamingAssets/Data/Meta/Effects/official-world-emitters.json.gz"
DOCS_REPORT = ROOT / "l2-unity/docs/interlude-ref/world/official-world-emitters-report.md"

# Reuse the LineageEffect UC formatter (same tagged property shape).
sys.path.insert(0, str(Path(__file__).resolve().parent))
from write_lineageeffect_uc import Writer  # noqa: E402

PARENT_SKIP = {
    "Emitters",
    "Level",
    "Region",
    "Tag",
    "PhysicsVolume",
    "Location",
    "Rotation",
    "SwayRotationOrig",
    "DrawScale",
    "DrawScale3D",
    "TexModifyInfo",
    "ForcedRegionTag",
    "bSelected",
    "bDynamicActorFilterState",
    "bLightChanged",
}


def props(e: dict) -> dict:
    return {p.get("name"): p for p in (e.get("properties") or [])}


def vec(p: dict | None) -> dict | None:
    if not p:
        return None
    if "vector" in p:
        v = p["vector"]
        return {"x": v.get("x", 0), "y": v.get("y", 0), "z": v.get("z", 0)}
    if "rotator" in p:
        v = p["rotator"]
        return {
            "x": v.get("pitch", v.get("x", 0)),
            "y": v.get("yaw", v.get("y", 0)),
            "z": v.get("roll", v.get("z", 0)),
        }
    return None


def range_min_max(p: dict | None) -> tuple[float, float] | None:
    if not p:
        return None
    lo = hi = None
    for ip in _flat(p.get("subproperties")):
        name = ip.get("name")
        if name == "Min":
            lo = float(ip.get("value") or 0)
        elif name == "Max":
            hi = float(ip.get("value") or 0)
    if lo is None and hi is None:
        return None
    return (lo or 0.0, hi or 0.0)


def _flat(node):
    if not node:
        return []
    if isinstance(node, list):
        out = []
        for item in node:
            if isinstance(item, list):
                out.extend(_flat(item))
            else:
                out.append(item)
        return out
    return [node]


def spawn_sounds(p: dict | None) -> list[str]:
    if not p:
        return []
    out = []
    for obj in p.get("object_array") or []:
        ref = obj.get("ref") or ""
        if not ref or ref == "none":
            continue
        # import:Engine:Sound:rock_1
        parts = ref.split(":")
        if parts[0] == "import" and len(parts) >= 4:
            out.append(parts[1] + "." + parts[-1])
        else:
            out.append(ref)
    return out


def def_hash(child_blobs: list[tuple[str, str]]) -> str:
    payload = json.dumps(child_blobs, separators=(",", ":"), ensure_ascii=False)
    return hashlib.sha1(payload.encode("utf-8")).hexdigest()[:16]


def iter_dumps() -> list[Path]:
    return sorted((DUMP_ROOT / "maps").glob("*.unr.json.gz")) + sorted(
        (DUMP_ROOT / "env").glob("*.unr.json.gz")
    )


def tile_xy(package: str) -> tuple[int, int]:
    parts = package.split("_")
    if len(parts) == 2 and parts[0].isdigit() and parts[1].isdigit():
        return int(parts[0]), int(parts[1])
    return 0, 0


def write_uc(class_name: str, children: list[dict], parent_extra_lines: list[str]) -> str:
    body: list[str] = []
    # Writer still needs imports/index for Texture refs; callers pass a Writer
    # via closure... we format in the caller.
    raise NotImplementedError


def main() -> int:
    placements: list[dict] = []
    unique_uc: dict[str, str] = {}
    unique_source: dict[str, str] = {}
    errors: list[str] = []
    unresolved: list[str] = []
    empty_children = 0
    maps = 0
    child_counts: Counter[str] = Counter()

    EFFECT_ROOT.mkdir(parents=True, exist_ok=True)
    JSON_OUT.parent.mkdir(parents=True, exist_ok=True)

    for fp in iter_dumps():
        with gzip.open(fp, "rt", encoding="utf-8") as f:
            data = json.load(f)
        package = data.get("package") or fp.name.split(".")[0]
        exports = data.get("exports") or []
        imports = data.get("imports") or []
        by_index = {e["export_index"]: e for e in exports}
        by_name: dict[str, list] = {}
        for e in exports:
            by_name.setdefault((e.get("name") or "").lower(), []).append(e)
        writer = Writer(imports, by_index, by_name)
        writers_map = package
        had = False
        tile_x, tile_y = tile_xy(package)

        for e in exports:
            if e.get("class") != "Emitter":
                continue
            had = True
            pm = props(e)
            loc = vec(pm.get("Location"))
            if loc is None:
                errors.append(f"{package}:{e.get('name')}: official Emitter has no Location")
                continue
            rot = vec(pm.get("Rotation")) or {"x": 0, "y": 0, "z": 0}
            ds = 1.0
            if "DrawScale" in pm:
                ds = float(pm["DrawScale"].get("value") or 1.0)
            ds3 = vec(pm.get("DrawScale3D"))
            sounds = spawn_sounds(pm.get("SpawnSound"))
            sound_r = pm.get("SoundRadius", {}).get("value")
            sound_v = pm.get("SoundVolume", {}).get("value")
            vlim = range_min_max(pm.get("VisibleLimit"))
            move = (pm.get("L2MoveEvent") or {}).get("value") or ""
            has_game = "L2GameEvent" in pm
            if has_game:
                ev = pm["L2GameEvent"]
                # The dump sometimes has array_size without values.
                has_vals = bool(ev.get("object_array") or ev.get("subproperties") or ev.get("value"))
                if not has_vals:
                    errors.append(
                        f"{package}:{e.get('name')}: L2GameEvent is present but the dump exported no event values"
                    )

            emit_prop = pm.get("Emitters") or {}
            refs = []
            for item in emit_prop.get("object_array") or []:
                ref = item.get("ref") or ""
                if not ref or ref == "none":
                    continue
                refs.append(ref)

            if not refs:
                empty_children += 1
                errors.append(
                    f"{package}:{e.get('name')}: official Emitters array has no resolved particle streams"
                )
                placements.append(
                    {
                        "map": package,
                        "name": e.get("name") or "",
                        "tileX": tile_x,
                        "tileY": tile_y,
                        "location": loc,
                        "rotation": rot,
                        "drawScale": ds,
                        "drawScale3D": ds3 or {"x": 1, "y": 1, "z": 1},
                        "hasDrawScale3D": ds3 is not None,
                        "prefabClass": "",
                        "spawnSounds": sounds,
                        "soundRadius": float(sound_r or 0),
                        "soundVolume": float(sound_v or 0),
                        "visibleLimitMin": (vlim or (0, 0))[0],
                        "visibleLimitMax": (vlim or (0, 0))[1],
                        "hasVisibleLimit": vlim is not None,
                        "moveEvent": move,
                        "hasGameEvent": has_game,
                    }
                )
                continue

            children = []
            child_blobs = []
            missing_child = False
            for i, ref in enumerate(refs):
                parts = ref.split(":")
                try:
                    idx = int(parts[2])
                except (IndexError, ValueError):
                    errors.append(f"{package}:{e.get('name')}: Emitters[{i}] bad ref {ref}")
                    missing_child = True
                    continue
                child = by_index.get(idx)
                if child is None:
                    errors.append(
                        f"{package}:{e.get('name')}: Emitters[{i}] export {idx} missing ({ref})"
                    )
                    missing_child = True
                    continue
                children.append(child)
                child_classes = child.get("class") or ""
                child_counts[child_classes] += 1
                blob = json.dumps(child.get("properties") or [], sort_keys=True, separators=(",", ":"))
                child_blobs.append((child_classes, blob))

            if missing_child or not children:
                continue

            writer.unresolved_refs = []
            writer.unknown_bytes = []
            body: list[str] = []
            for i, child in enumerate(children):
                # Stable object names so shared defs can be reused across maps.
                child = dict(child)
                cls = child.get("class") or "ParticleEmitter"
                child["name"] = f"{cls}_{i}"
                body.extend(writer.emitter_lines(child, "     "))
                body.append(f"     Emitters({i})={cls}'{child['name']}'")

            if writer.unresolved_refs:
                for r in writer.unresolved_refs:
                    unresolved.append(f"{package}:{e.get('name')}: {r}")

            class_name = "we_" + def_hash(child_blobs)
            text = (
                f"class {class_name} extends Emitter;\n\n"
                f"defaultproperties\n{{\n"
                + "\n".join(body)
                + "\n}\n"
            )
            if class_name not in unique_uc:
                unique_uc[class_name] = text
                unique_source[class_name] = f"{package}:{e.get('name')}"
            elif unique_uc[class_name] != text:
                errors.append(
                    f"{package}:{e.get('name')}: definition {class_name} collided with a different UC body "
                    f"(first seen at {unique_source[class_name]})"
                )

            placements.append(
                {
                    "map": package,
                    "name": e.get("name") or "",
                    "tileX": tile_x,
                    "tileY": tile_y,
                    "location": loc,
                    "rotation": rot,
                    "drawScale": ds,
                    "drawScale3D": ds3 or {"x": 1, "y": 1, "z": 1},
                    "hasDrawScale3D": ds3 is not None,
                    "prefabClass": class_name,
                    "spawnSounds": sounds,
                    "soundRadius": float(sound_r or 0),
                    "soundVolume": float(sound_v or 0),
                    "visibleLimitMin": (vlim or (0, 0))[0],
                    "visibleLimitMax": (vlim or (0, 0))[1],
                    "hasVisibleLimit": vlim is not None,
                    "moveEvent": move,
                    "hasGameEvent": has_game,
                }
            )

        if had:
            maps += 1

    # Write unique UC folders (and a DefaultImporter-less raw .uc; Unity will import).
    written = 0
    for class_name, text in unique_uc.items():
        folder = EFFECT_ROOT / class_name
        folder.mkdir(parents=True, exist_ok=True)
        (folder / f"{class_name}.uc").write_text(text, encoding="utf-8")
        written += 1

    payload = {
        "emitterCount": len(placements),
        "prefabClasses": len(unique_uc),
        "emitters": placements,
    }
    with gzip.open(JSON_OUT, "wt", encoding="utf-8") as f:
        json.dump(payload, f, separators=(",", ":"), ensure_ascii=False)

    with_prefab = sum(1 for p in placements if p["prefabClass"])
    report = [
        "# Official world map emitters",
        "",
        f"- Official `Emitter` actors: **{len(placements)}** on **{maps}** maps/levels.",
        f"- Unique particle definitions (`.uc` / prefab classes): **{len(unique_uc)}**.",
        f"- Actors with resolved particle streams: **{with_prefab}**.",
        f"- Actors whose official `Emitters` array has no resolved streams: **{empty_children}**.",
        f"- Child stream classes: {dict(child_counts)}.",
        "",
        "Runtime places `Resources/Data/Effects/world/<class>/<class>` at official Location/Rotation/DrawScale.",
        "`2SP_PRO_000` is a `WetTexture` (not a `Texture` export) and is logged when referenced; it is not replaced.",
        "",
    ]
    if errors:
        report.append(f"## Dump notes ({len(errors)})")
        report.append("")
        for line in errors:
            report.append(f"- {line}")
        report.append("")
    if unresolved:
        report.append(f"## Unresolved object refs ({len(unresolved)})")
        report.append("")
        for line in unresolved:
            report.append(f"- {line}")
        report.append("")
    DOCS_REPORT.write_text("\n".join(report) + "\n", encoding="utf-8")

    print(
        f"placements={len(placements)} with_prefab={with_prefab} unique={len(unique_uc)} "
        f"empty={empty_children} errors={len(errors)} unresolved={len(unresolved)} "
        f"uc_written={written} json={JSON_OUT}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
