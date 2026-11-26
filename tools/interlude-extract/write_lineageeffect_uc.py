#!/usr/bin/env python3
"""Write complete LineageEffect .uc defaultproperties from the lossless usk dump.

UScriptDump leaves Emitters=/* Array type was not detected. */. The tagged
emitter objects are in lineageeffect.u.part*.json. This writer emits the
Begin Object blocks the L2EffectGenerator already parses, without inventing
properties or silently dropping unresolved refs.
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMP = ROOT / "l2-unity/docs/interlude-ref/packages/usk-dump"

BYTE_ENUMS: dict[str, tuple[str, ...]] = {
    "DrawStyle": (
        "PTDS_Regular", "PTDS_AlphaBlend", "PTDS_Modulated", "PTDS_Translucent",
        "PTDS_AlphaModulate_MightNotFogCorrectly", "PTDS_Darken", "PTDS_Brighten",
    ),
    "CoordinateSystem": (
        "PTCS_Independent", "PTCS_Relative", "PTCS_Absolute",
        "PTCS_RelativeRotation", "PTCS_Spray",
    ),
    "StartLocationShape": ("PTLS_Box", "PTLS_Sphere", "PTLS_Polar", "PTLS_All"),
    "UseDirectionAs": (
        "PTDU_None", "PTDU_Up", "PTDU_Right", "PTDU_Forward", "PTDU_Normal",
        "PTDU_UpAndNormal", "PTDU_RightAndNormal", "PTDU_Scale",
    ),
    "UseRotationFrom": ("PTRS_None", "PTRS_Actor", "PTRS_Offset", "PTRS_Normal"),
    "GetVelocityDirectionFrom": (
        "PTVD_None", "PTVD_StartPositionAndOwner", "PTVD_OwnerAndStartPosition",
        "PTVD_AddRadial",
    ),
    "DetermineEndPointBy": (
        "PTEP_Velocity", "PTEP_Distance", "PTEP_Offset", "PTEP_Actor",
        "PTEP_TraceOffset", "PTEP_OffsetAsAbsolute",
    ),
    "MeshSpawning": ("PTMS_None", "PTMS_Linear", "PTMS_Random"),
    "Physics": (
        "PHYS_None", "PHYS_Walking", "PHYS_Falling", "PHYS_Swimming", "PHYS_Flying",
        "PHYS_Rotating", "PHYS_Projectile", "PHYS_Interpolating", "PHYS_MovingBrush",
        "PHYS_Spider", "PHYS_Trailer", "PHYS_Ladder", "PHYS_RootMotion", "PHYS_Karma",
        "PHYS_KarmaRagDoll", "PHYS_MovingTrailer", "PHYS_EffectTrailer",
        "PHYS_NProjectile", "PHYS_NMover", "PHYS_L2Movement",
    ),
    "DrawType": (
        "DT_None", "DT_Sprite", "DT_Mesh", "DT_Brush", "DT_RopeSprite",
        "DT_VerticalSprite", "DT_Terraform", "DT_SpriteAnimOnce", "DT_StaticMesh",
        "DT_DrawType", "DT_Particle", "DT_AntiPortal", "DT_FluidSurface", "DT_Sun",
        "DT_MusicVolume", "DT_Custom",
    ),
    "CollisionSound": ("PTSC_None", "PTSC_LinearGlobal", "PTSC_LinearLocal", "PTSC_Random"),
    "EffectAxis": ("PTEA_NegativeX", "PTEA_PositiveZ"),
    "SkelLocationUpdate": ("PTSU_None", "PTSU_SpawnOffset", "PTSU_Location"),
    "ParticlesPerSecondChoice": ("PTWC_MaxParticle", "PTWC_ParticlePerSecond"),
}


def load_dump() -> tuple[list, dict, dict]:
    man = json.loads((DUMP / "lineageeffect.u.manifest.json").read_text(encoding="utf-8"))
    imports = man["imports"]
    exports = []
    for part in man["parts"]:
        data = json.loads((DUMP / part["file"]).read_text(encoding="utf-8"))
        exports.extend(data["exports"])
    by_index = {e["export_index"]: e for e in exports}
    by_name: dict[str, list] = {}
    for e in exports:
        by_name.setdefault((e.get("name") or "").lower(), []).append(e)
    return imports, by_index, by_name


def parent_import(imp: dict, by_imp: dict) -> dict | None:
    idx = imp.get("package_index") or 0
    if not idx:
        return None
    if idx < 0:
        return by_imp.get(-idx - 1)
    return by_imp.get(idx)


def import_token(imp: dict, by_imp: dict) -> str:
    names = [imp.get("object_name") or ""]
    cur = parent_import(imp, by_imp)
    seen = set()
    while cur is not None:
        key = cur.get("import_index")
        if key in seen:
            break
        seen.add(key)
        names.append(cur.get("object_name") or "")
        cur = parent_import(cur, by_imp)
    names = [n for n in reversed(names) if n]
    return ".".join(names)


def resolve_ref(ref: str, imports: list, by_index: dict, by_imp: dict) -> tuple[str, str] | None:
    """Return (type_name, object_path) or None."""
    if not ref:
        return None
    parts = ref.split(":")
    if parts[0] == "import" and len(parts) >= 4:
        class_name = parts[2]
        obj_name = parts[-1]
        imp = None
        for i in imports:
            if i.get("class_name") == class_name and i.get("object_name") == obj_name:
                imp = i
                break
        if imp is None:
            for i in imports:
                if i.get("object_name") == obj_name:
                    imp = i
                    class_name = i.get("class_name") or class_name
                    break
        if imp is None:
            return class_name, obj_name
        return class_name, import_token(imp, by_imp)
    if parts[0] == "export" and len(parts) >= 4:
        try:
            idx = int(parts[2])
        except ValueError:
            return None
        exp = by_index.get(idx)
        if exp is None:
            return None
        class_name = parts[3] if len(parts) > 3 and parts[3] != "None" else (exp.get("class") or "Object")
        return class_name, exp.get("name") or ""
    return None


def fmt_float(v: float) -> str:
    s = format(float(v), ".6f")
    if "." in s:
        s = s.rstrip("0")
        if s.endswith("."):
            s += "000000"
        # Unreal defaultproperties typically keep 6 decimals for non-integers
        if s.endswith("000000"):
            return s
        # pad/truncate to 6 for parser regex \d+(?:\.\d+)?
        whole, frac = s.split(".", 1)
        frac = (frac + "000000")[:6]
        return whole + "." + frac
    return s + ".000000"


def fmt_int_or_float(v) -> str:
    if isinstance(v, bool):
        return "True" if v else "False"
    if isinstance(v, int) and not isinstance(v, bool):
        return str(v)
    try:
        f = float(v)
    except (TypeError, ValueError):
        return str(v)
    if f == int(f) and abs(f) < 1e12:
        # MaxParticles is int; many ranges are 1.000000
        return fmt_float(f)
    return fmt_float(f)


def u8(v) -> int:
    n = int(v)
    if n < 0:
        n = n & 0xFF
    return max(0, min(255, n))


def flatten_props(node) -> list[dict]:
    """subproperties may be a list of props or a list containing one list of props."""
    if node is None:
        return []
    if isinstance(node, dict) and node.get("name"):
        return [node]
    if isinstance(node, list):
        if node and all(isinstance(x, dict) and x.get("name") for x in node):
            return node
        out = []
        for x in node:
            out.extend(flatten_props(x))
        return out
    return []


class Writer:
    def __init__(self, imports, by_index, by_name):
        self.imports = imports
        self.by_index = by_index
        self.by_name = by_name
        self.by_imp = {i["import_index"]: i for i in imports}
        self.unknown_bytes: list[str] = []
        self.unresolved_refs: list[str] = []

    def class_export(self, class_name: str) -> dict | None:
        key = class_name.lower()
        cands = self.by_name.get(key) or []
        for e in cands:
            src = e.get("properties_source") or ""
            cn = e.get("class") or ""
            if src == "class_defaults" or cn in ("", "None", "Class"):
                return e
        return cands[0] if cands else None

    def fmt_prop(self, prop: dict, owner: str) -> str | None:
        name = prop.get("name")
        typ = prop.get("type")
        if not name:
            return None
        # Ignore extractor leak
        if typ == "Bool":
            return f"{name}={'True' if prop.get('value') else 'False'}"
        if typ in ("Float", "Int"):
            if typ == "Int":
                return f"{name}={int(prop.get('value') or 0)}"
            return f"{name}={fmt_float(prop.get('value') or 0)}"
        if typ == "Str" or typ == "Name":
            val = prop.get("value") or prop.get("name_value") or ""
            if typ == "Name" or name == "Name":
                return f'{name}="{val}"'
            return f'{name}="{val}"'
        if typ == "Byte":
            val = int(prop.get("value") or 0)
            names = BYTE_ENUMS.get(name)
            if names is None:
                self.unknown_bytes.append(f"{owner}.{name}={val}")
                return f"{name}={val}"
            if val < 0 or val >= len(names):
                self.unknown_bytes.append(f"{owner}.{name}={val} (out of range)")
                return f"{name}={val}"
            return f"{name}={names[val]}"
        if typ == "Object":
            ref = prop.get("ref") or ""
            resolved = resolve_ref(ref, self.imports, self.by_index, self.by_imp)
            if resolved is None:
                if not ref or ref.lower() == "none" or ref.lower().endswith(":none"):
                    return f"{name}=None"
                self.unresolved_refs.append(f"{owner}.{name} ref={ref}")
                return None
            cls, path = resolved
            if not path or path == "None":
                return f"{name}=None"
            return f"{name}={cls}'{path}'"
        if typ == "Struct":
            inner = self.fmt_struct(prop, owner + "." + name)
            if inner is None:
                return None
            return f"{name}={inner}"
        if typ == "Array":
            return None  # handled by caller as indexed entries
        return None

    def fmt_struct(self, prop: dict, owner: str) -> str | None:
        struct = prop.get("struct") or ""
        if struct == "Color" or "color" in prop:
            c = prop.get("color") or {}
            if not c and prop.get("subproperties"):
                # unusual
                c = {}
            if c:
                return (
                    f"(B={u8(c.get('b', 0))},G={u8(c.get('g', 0))},"
                    f"R={u8(c.get('r', 0))},A={u8(c.get('a', 255))})"
                )
        if struct == "Vector" or "vector" in prop:
            v = prop.get("vector")
            if isinstance(v, dict):
                parts = []
                for axis, key in (("X", "x"), ("Y", "y"), ("Z", "z")):
                    if key in v:
                        parts.append(f"{axis}={fmt_float(v[key])}")
                return "(" + ",".join(parts) + ")" if parts else None
        if struct == "Rotator" or "rotator" in prop:
            v = prop.get("rotator") or prop.get("vector")
            if isinstance(v, dict):
                parts = []
                mapping = (("Pitch", "pitch"), ("Yaw", "yaw"), ("Roll", "roll"),
                           ("Pitch", "x"), ("Yaw", "y"), ("Roll", "z"))
                used = set()
                for axis, key in mapping:
                    if key in v and axis not in used:
                        parts.append(f"{axis}={int(v[key])}")
                        used.add(axis)
                return "(" + ",".join(parts) + ")" if parts else None
        inner_props = flatten_props(prop.get("subproperties"))
        if not inner_props:
            return None
        parts = []
        for ip in inner_props:
            text = self.fmt_prop(ip, owner)
            if text:
                # nested structs already include Name=...
                parts.append(text)
        if not parts:
            return None
        return "(" + ",".join(parts) + ")"

    def fmt_array(self, prop: dict, owner: str) -> list[str]:
        name = prop.get("name")
        lines = []
        objs = prop.get("object_array")
        if objs:
            for i, obj in enumerate(objs):
                ref = obj.get("ref") or ""
                resolved = resolve_ref(ref, self.imports, self.by_index, self.by_imp)
                if resolved is None:
                    self.unresolved_refs.append(f"{owner}.{name}[{i}] ref={ref}")
                    continue
                cls, path = resolved
                lines.append(f"{name}({i})={cls}'{path}'")
            return lines
        subs = prop.get("subproperties") or []
        # array of structs: list of list-of-props
        for i, item in enumerate(subs):
            fake = {"type": "Struct", "struct": "", "subproperties": item}
            inner = self.fmt_struct(fake, f"{owner}.{name}[{i}]")
            if inner is None:
                # maybe a list of already-flat props? or object?
                if isinstance(item, dict) and item.get("ref"):
                    resolved = resolve_ref(item.get("ref"), self.imports, self.by_index, self.by_imp)
                    if resolved:
                        cls, path = resolved
                        lines.append(f"{name}({i})={cls}'{path}'")
                    else:
                        self.unresolved_refs.append(f"{owner}.{name}[{i}] ref={item.get('ref')}")
                continue
            lines.append(f"{name}({i})={inner}")
        return lines

    def emitter_lines(self, exp: dict, indent: str) -> list[str]:
        cls = exp.get("class") or "ParticleEmitter"
        name = exp.get("name") or "Emitter"
        lines = [f"{indent}Begin Object Class={cls} Name={name}"]
        for prop in exp.get("properties") or []:
            typ = prop.get("type")
            if typ == "Array":
                for item in self.fmt_array(prop, name):
                    lines.append(f"{indent}    {item}")
                continue
            text = self.fmt_prop(prop, name)
            if text:
                lines.append(f"{indent}    {text}")
        lines.append(f"{indent}End Object")
        return lines

    def write_class(self, class_name: str) -> tuple[str | None, str | None]:
        """Return (uc_text, error)."""
        self.unresolved_refs = []
        exp = self.class_export(class_name)
        if exp is None:
            return None, f"effect class {class_name}: not found in lineageeffect dump"
        super_name = exp.get("super") or "Emitter"
        if super_name in ("", "None"):
            super_name = "Emitter"
        emitters = []
        other_lines = []
        emit_prop = None
        for prop in exp.get("properties") or []:
            if prop.get("name") == "Emitters" and prop.get("type") == "Array":
                emit_prop = prop
                continue
            if prop.get("type") == "Array":
                other_lines.extend(self.fmt_array(prop, class_name))
                continue
            text = self.fmt_prop(prop, class_name)
            if text:
                other_lines.append(text)
        if emit_prop is None:
            return None, (
                f"effect class {class_name}: no Emitters property in lineageeffect class defaults "
                f"(export {exp.get('export_index')})"
            )
        objs = emit_prop.get("object_array") or []
        if emit_prop.get("array_size", len(objs)) and not objs:
            return None, (
                f"effect class {class_name}: Emitters array is empty or unresolved in the dump "
                f"(array_size={emit_prop.get('array_size')})"
            )
        if not objs:
            return None, (
                f"effect class {class_name}: Emitters array has no object refs in the dump"
            )
        body = []
        for i, obj in enumerate(objs):
            ref = obj.get("ref") or ""
            parts = ref.split(":")
            if not (len(parts) >= 3 and parts[0] == "export"):
                return None, (
                    f"effect class {class_name}: Emitters[{i}] is not an export ref ({ref or 'empty'})"
                )
            try:
                idx = int(parts[2])
            except ValueError:
                return None, f"effect class {class_name}: Emitters[{i}] has a bad export index ({ref})"
            child = self.by_index.get(idx)
            if child is None:
                return None, (
                    f"effect class {class_name}: Emitters[{i}] export {idx} is missing from the dump ({ref})"
                )
            body.extend(self.emitter_lines(child, "     "))
            cls = child.get("class") or "ParticleEmitter"
            name = child.get("name") or f"Emitter{i}"
            body.append(f"     Emitters({i})={cls}'{name}'")
        if self.unresolved_refs:
            return None, (
                f"effect class {class_name}: unresolved object refs: "
                + "; ".join(self.unresolved_refs)
            )
        body.extend("     " + line for line in other_lines)
        text = (
            f"class {class_name} extends {super_name};\n\n"
            f"defaultproperties\n{{\n"
            + "\n".join(body)
            + "\n}\n"
        )
        return text, None


def write_folder(writer: Writer, class_name: str, folder: Path) -> str | None:
    text, err = writer.write_class(class_name)
    if err:
        return err
    dest_dir = folder / class_name
    dest_dir.mkdir(parents=True, exist_ok=True)
    dest = dest_dir / f"{class_name}.uc"
    dest.write_text(text, encoding="utf-8")
    return None


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--class", dest="classes", action="append", default=[])
    ap.add_argument("--list", type=Path, help="Text file with one Assets/.../class-folder path per line")
    ap.add_argument("--stdout", action="store_true")
    args = ap.parse_args()
    imports, by_index, by_name = load_dump()
    writer = Writer(imports, by_index, by_name)
    targets: list[tuple[str, Path | None]] = []
    for name in args.classes:
        targets.append((name, None))
    if args.list:
        for line in args.list.read_text(encoding="utf-8").splitlines():
            line = line.strip()
            if not line or line.startswith("#"):
                continue
            folder = ROOT / "l2-unity" / line if not line.startswith("/") else Path(line)
            # list entries are like Assets/Resources/Data/Effects/imported-missing/ab_bleeding
            if not folder.is_absolute():
                # line is relative to l2-unity or repo?
                p1 = ROOT / "l2-unity" / line
                p2 = ROOT / line
                folder = p1 if p1.exists() or not p2.exists() else p2
                if line.startswith("Assets/"):
                    folder = ROOT / "l2-unity" / line
            class_name = folder.name
            targets.append((class_name, folder))
    if not targets:
        print("no classes", file=sys.stderr)
        return 2
    ok = 0
    fail = 0
    for class_name, folder in targets:
        if args.stdout and folder is None:
            text, err = writer.write_class(class_name)
            if err:
                print(err, file=sys.stderr)
                fail += 1
                continue
            sys.stdout.write(text)
            ok += 1
            continue
        if folder is None:
            text, err = writer.write_class(class_name)
            if err:
                print(err, file=sys.stderr)
                fail += 1
            else:
                sys.stdout.write(text)
                ok += 1
            continue
        err = write_folder(writer, class_name, folder)
        if err:
            print("[write_lineageeffect_uc] FAIL " + err, file=sys.stderr)
            fail += 1
        else:
            print("[write_lineageeffect_uc] OK " + class_name)
            ok += 1
    if writer.unknown_bytes:
        uniq = sorted(set(writer.unknown_bytes))
        print(f"[write_lineageeffect_uc] unknown byte enums ({len(uniq)}): " + "; ".join(uniq[:20]), file=sys.stderr)
    print(f"[write_lineageeffect_uc] done ok={ok} fail={fail}")
    return 0 if fail == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
