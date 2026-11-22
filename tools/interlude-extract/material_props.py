#!/usr/bin/env python3
"""Material *.props.txt from the lossless UE2 property dump.

umodel only registers the classes in REGISTER_MATERIAL_CLASSES
(UE Viewer UnMaterial2.h): ConstantColor, BitmapMaterial, Palette, Shader,
Combiner, Texture, Cubemap, FinalBlend, TexEnvMap, TexOscillator, TexPanner,
TexRotator, TexScaler. ColorModifier and the other Modifier subclasses are
not in that list, so umodel reports "Unknown class" and writes no props.

Every Material subclass is rendered from an ExportPackageProperties dump
(the same reader that writes docs/interlude-ref). Object references use the
committed props form Class'Group.Name'. umodel is not called. Base Texture
pixels stay on ExportNamedTextures.
"""
from __future__ import annotations

import argparse
import json
import re
import sys
from collections import Counter
from pathlib import Path

from brush_face_class import KIND_MATERIAL, ExportIndex, Face, index

REPO = Path(__file__).resolve().parents[2]
ENV_DUMPS = REPO / "l2-unity" / "docs" / "interlude-ref" / "world" / "env"
TEX_ASSETS = REPO / "l2-unity" / "Assets" / "Resources" / "Data" / "Textures"
STATIC_MESHES = REPO / "l2-unity" / "Assets" / "Resources" / "Data" / "StaticMeshes"
LOBBY_JSON = ENV_DUMPS / "Lobby.json"
BRUSHES_JSON = (
    REPO / "l2-unity" / "Assets" / "Resources" / "Data" / "Maps" / "l2_lobby" / "Meta" / "Brushes.json"
)

# gildor2/UEViewer Unreal/UnrealMaterial/UnMaterial2.h REGISTER_MATERIAL_CLASSES,
# with the leading U stripped. This is the set umodel can export. It is the
# audit comparison, not a filter on what this module will render.
UMODEL_SUPPORTED_MATERIAL_CLASSES = frozenset({
    "ConstantColor",
    "BitmapMaterial",
    "Palette",
    "Shader",
    "Combiner",
    "Texture",
    "Cubemap",
    "FinalBlend",
    "TexEnvMap",
    "TexOscillator",
    "TexPanner",
    "TexRotator",
    "TexScaler",
})

_EXPORT_REF = re.compile(r"^export:([^:]+):(\d+):([^:]+):(.*)$")
_IMPORT_REF = re.compile(r"^import:([^:]*):([^:]+):(.*)$")
_SKIN_REF = re.compile(r"([A-Za-z_][A-Za-z0-9_]*)'[^']+'")


def ubyte(value: int) -> int:
    return int(value) & 0xFF


def fmt_float(value: float) -> str:
    return format(float(value), ".9g")


class Dump:
    """One ExportPackageProperties JSON document."""

    def __init__(self, doc: dict, path: Path) -> None:
        self.path = path
        self.package = str(doc.get("package") or path.stem)
        self.exports: list[dict] = list(doc.get("exports") or [])
        self.imports: list[dict] = list(doc.get("imports") or [])
        self.by_index: dict[int, dict] = {}
        for exp in self.exports:
            self.by_index[int(exp["export_index"])] = exp

    @classmethod
    def load(cls, path: Path) -> "Dump":
        return cls(json.loads(path.read_text(encoding="utf-8")), path)

    def group_of(self, exp: dict) -> str:
        """Immediate outer Package name. That is the UE2 group in Class'Group.Name'."""
        seen: set[int] = set()
        current = exp
        while current is not None:
            idx = int(current["export_index"])
            if idx in seen:
                break
            seen.add(idx)
            outer = current.get("outer") or "none"
            match = _EXPORT_REF.match(outer)
            if not match:
                return ""
            parent = self.by_index.get(int(match.group(2)))
            if parent is None:
                return ""
            if parent.get("class") == "Package":
                return str(parent.get("name") or "")
            current = parent
        return ""

    def find(self, name: str, ue_class: str, group: str) -> dict | None:
        hits = [
            exp for exp in self.exports
            if str(exp.get("name", "")).lower() == name.lower()
            and str(exp.get("class", "")).lower() == ue_class.lower()
        ]
        if group:
            hits = [exp for exp in hits if self.group_of(exp).lower() == group.lower()]
        if len(hits) != 1:
            return None
        return hits[0]

    def object_token(self, ref: str) -> str:
        if not ref or ref == "none":
            return "None"
        export = _EXPORT_REF.match(ref)
        if export:
            _pkg, idx, cls, name = export.groups()
            target = self.by_index.get(int(idx))
            group = self.group_of(target) if target is not None else ""
            if group:
                return f"{cls}'{group}.{name}'"
            return f"{cls}'{name}'"
        imported = _IMPORT_REF.match(ref)
        if imported:
            class_package, cls, name = imported.groups()
            group = self._import_group(class_package, cls, name)
            if group:
                return f"{cls}'{group}.{name}'"
            return f"{cls}'{name}'"
        raise ValueError(f"unparsed object ref {ref}")

    def _import_group(self, class_package: str, cls: str, name: str) -> str:
        hits = [
            imp for imp in self.imports
            if imp.get("class_package") == class_package
            and imp.get("class_name") == cls
            and imp.get("object_name") == name
        ]
        if len(hits) != 1:
            return ""
        outer = str(hits[0].get("package") or "")
        match = _IMPORT_REF.match(outer)
        if match and match.group(2) == "Package":
            return match.group(3)
        return ""

    def format_value(self, prop: dict) -> str:
        kind = prop.get("type")
        if kind == "Bool":
            return "true" if prop.get("value") else "false"
        if kind == "Float":
            return fmt_float(prop["value"])
        if kind in ("Int", "Byte"):
            return str(int(prop["value"]))
        if kind in ("Name", "Str", "Class"):
            value = prop.get("value")
            if value is None:
                raise ValueError(f"property {prop.get('name')} has no value")
            return str(value)
        if kind == "Object":
            return self.object_token(str(prop.get("ref") or "none"))
        if kind == "Vector" or (kind == "Struct" and prop.get("vector")):
            vec = prop.get("vector") or prop.get("value") or {}
            return "(X={x},Y={y},Z={z})".format(**{k: fmt_float(vec[k]) for k in ("x", "y", "z")})
        if kind == "Rotator" or (kind == "Struct" and prop.get("rotator")):
            rot = prop.get("rotator") or prop.get("value") or {}
            return "{{ Yaw={yaw}, Pitch={pitch}, Roll={roll} }}".format(
                yaw=int(rot["yaw"]), pitch=int(rot["pitch"]), roll=int(rot["roll"]),
            )
        if kind == "Struct" and prop.get("color"):
            color = prop["color"]
            return "(B={b},G={g},R={r},A={a})".format(
                b=ubyte(color["b"]),
                g=ubyte(color["g"]),
                r=ubyte(color["r"]),
                a=ubyte(color["a"]),
            )
        if kind == "Struct":
            struct = prop.get("struct") or "Struct"
            return f"(struct {struct})"
        if kind in ("Array", "FixedArray"):
            return self._format_array(prop)
        raise ValueError(
            f"property {prop.get('name')} type {kind} is not rendered "
            "(the property dump has no value this writer knows)"
        )

    def _format_array(self, prop: dict) -> str:
        objects = prop.get("object_array") or []
        ints = prop.get("int_array") or []
        if objects:
            tokens = [self.object_token(str(item.get("ref") or "none")) for item in objects]
            return "( " + ", ".join(tokens) + " )"
        if ints:
            return "( " + ", ".join(str(int(v)) for v in ints) + " )"
        size = prop.get("array_size", 0)
        return f"(array size={size}; elements not stored in the property dump)"

    def render(self, exp: dict) -> str:
        lines: list[str] = []
        for prop in exp.get("properties") or []:
            name = str(prop["name"])
            if "array_index" in prop:
                name = f"{name}[{int(prop['array_index'])}]"
            lines.append(f"{name} = {self.format_value(prop)}")
        return "\n".join(lines) + ("\n" if lines else "")


def dump_filename_matches(path: Path, package: str) -> bool:
    stem = package.lower()
    name = path.name.lower()
    return name in (f"{stem}.json", f"{stem}.utx.json")


def find_dump_file(package: str, extra_dirs: list[Path] | None = None) -> Path | None:
    dirs = list(extra_dirs or [])
    if ENV_DUMPS not in dirs:
        dirs.append(ENV_DUMPS)
    for folder in dirs:
        if not folder.is_dir():
            continue
        for path in folder.iterdir():
            if path.is_file() and path.suffix.lower() == ".json" and dump_filename_matches(path, package):
                return path
    return None


def load_dump(package: str, extra_dirs: list[Path] | None = None) -> Dump | None:
    path = find_dump_file(package, extra_dirs)
    if path is None:
        return None
    return Dump.load(path)


def asset_package_name(package: str, root: Path = TEX_ASSETS) -> str:
    """Folder name already on disk for this package, preserving its case."""
    if root.is_dir():
        for child in root.iterdir():
            if child.is_dir() and child.name.lower() == package.lower():
                return child.name
    return package


def props_dest(root: Path, package: str, ue_class: str, name: str) -> Path:
    # Match the folder case already committed under Textures/, so a
    # case-sensitive checkout does not grow a second L2_Skies next to l2_skies.
    return root / asset_package_name(package) / ue_class / f"{name}.props.txt"


def find_named(roots: list[Path], package: str, filename: str) -> Path | None:
    want = filename.lower()
    pkg = package.lower()
    for root in roots:
        if not root.is_dir():
            continue
        for path in root.rglob("*"):
            if not path.is_file() or path.name.lower() != want:
                continue
            if pkg in {part.lower() for part in path.parts}:
                return path
    return None


def render_face(dump: Dump, face: Face) -> str | None:
    exp = dump.find(face.name, face.ue_class, face.group)
    if exp is None:
        return None
    return dump.render(exp)


def write_rendered(path: Path, text: str) -> None:
    if path.is_file():
        existing = path.read_text(encoding="utf-8")
        if existing != text:
            raise SystemExit(
                f"{path} already exists and differs from the property dump; not overwritten"
            )
        return
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def material_exports(dump: Dump, idx: ExportIndex | None = None) -> list[dict]:
    idx = idx or index()
    out: list[dict] = []
    for exp in dump.exports:
        ue_class = str(exp.get("class") or "")
        if idx.kind_of(ue_class) == KIND_MATERIAL:
            out.append(exp)
    return out


def write_package(dump: Dump, props_out: Path, existing_roots: list[Path]) -> list[Path]:
    """Write props for every Material subclass that is not already on disk."""
    written: list[Path] = []
    rendered_at: dict[Path, str] = {}
    incomplete: list[str] = []
    for exp in material_exports(dump):
        name = str(exp["name"])
        ue_class = str(exp["class"])
        group = dump.group_of(exp)
        if find_named(existing_roots, dump.package, name + ".props.txt") is not None:
            continue
        text = dump.render(exp)
        if "elements not stored in the property dump" in text:
            incomplete.append(
                f"package={dump.package} group={group} name={name} class={ue_class} "
                "property dump has an array size and no elements"
            )
            continue
        dest = props_dest(props_out, dump.package, ue_class, name)
        prior = rendered_at.get(dest)
        if prior is not None and prior != text:
            raise SystemExit(
                f"package={dump.package} group={group} name={name} class={ue_class} "
                f"shares {dest.name} with another export whose props differ; not overwritten"
            )
        rendered_at[dest] = text
        before = dest.is_file()
        write_rendered(dest, text)
        if not before:
            written.append(dest)
    if incomplete:
        raise SystemExit("property dump incomplete:\n" + "\n".join(incomplete))
    return written


def lobby_brush_classes() -> Counter:
    """Classes of lobby brush faces, from Brushes.json via the export table."""
    from export_brush_textures import collect_refs
    import brush_face_class as faces

    counts: Counter = Counter()
    if not BRUSHES_JSON.is_file():
        return counts
    for ref in collect_refs([BRUSHES_JSON]):
        face = faces.classify(ref)
        counts[face.ue_class or "absent"] += 1
    return counts


def lobby_mesh_packages() -> set[str]:
    if not LOBBY_JSON.is_file():
        return set()
    doc = json.loads(LOBBY_JSON.read_text(encoding="utf-8"))
    packages: set[str] = set()
    for imp in doc.get("imports") or []:
        if imp.get("class_name") != "StaticMesh":
            continue
        match = _IMPORT_REF.match(str(imp.get("package") or ""))
        if match and match.group(2) == "Package":
            packages.add(match.group(3))
    return packages


def committed_mesh_skin_classes() -> Counter:
    """Classes named in every committed StaticMesh *.props.txt, lobby or not."""
    counts: Counter = Counter()
    if not STATIC_MESHES.is_dir():
        return counts
    for path in STATIC_MESHES.rglob("*.props.txt"):
        for match in _SKIN_REF.finditer(path.read_text(encoding="utf-8", errors="replace")):
            counts[match.group(1)] += 1
    return counts


def lobby_skin_classes() -> tuple[Counter, list[str]]:
    """Classes named by Material = Class'...' in lobby static-mesh props.

    Skins live in the mesh package. Only packages Lobby imports, and only
    when a committed *.props.txt exists, are counted. Packages with no props
    are returned as the second value.
    """
    counts: Counter = Counter()
    missing: list[str] = []
    packages = lobby_mesh_packages()
    if not STATIC_MESHES.is_dir():
        return counts, sorted(packages)
    on_disk = {child.name.lower(): child for child in STATIC_MESHES.iterdir() if child.is_dir()}
    for package in sorted(packages):
        folder = on_disk.get(package.lower())
        if folder is None:
            missing.append(package)
            continue
        props = list(folder.rglob("*.props.txt"))
        if not props:
            missing.append(package)
            continue
        for path in props:
            for match in _SKIN_REF.finditer(path.read_text(encoding="utf-8", errors="replace")):
                counts[match.group(1)] += 1
    return counts, missing


def audit_lines() -> list[str]:
    brushes = lobby_brush_classes()
    skins, missing_pkgs = lobby_skin_classes()
    all_skins = committed_mesh_skin_classes()
    brush_set = set(brushes)
    skin_set = set(skins) | set(all_skins)
    used = brush_set | skin_set
    unsupported = sorted(used - UMODEL_SUPPORTED_MATERIAL_CLASSES - {""})
    supported = sorted(used & UMODEL_SUPPORTED_MATERIAL_CLASSES)
    lines = [
        "umodel REGISTER_MATERIAL_CLASSES: " + ", ".join(sorted(UMODEL_SUPPORTED_MATERIAL_CLASSES)),
        "lobby brush face classes:",
    ]
    for cls, n in sorted(brushes.items()):
        mark = "umodel-unsupported" if cls not in UMODEL_SUPPORTED_MATERIAL_CLASSES else "umodel-supported"
        lines.append(f"  brush {cls}: {n} ({mark})")
    lines.append("lobby static-mesh skin classes (from committed StaticMesh *.props.txt):")
    if not skins:
        lines.append("  (none — no committed props for lobby mesh packages)")
    for cls, n in sorted(skins.items()):
        mark = "umodel-unsupported" if cls not in UMODEL_SUPPORTED_MATERIAL_CLASSES else "umodel-supported"
        lines.append(f"  skin {cls}: {n} ({mark})")
    lines.append("classes in brushes or skins that umodel does not export: " + (", ".join(unsupported) or "(none)"))
    lines.append("classes umodel can export that also appear: " + (", ".join(supported) or "(none)"))
    lines.append(
        f"lobby mesh packages with no committed props: {len(missing_pkgs)} "
        "(their skin classes are not in this list)"
    )
    lines.append("all committed StaticMesh props (not only lobby):")
    if not all_skins:
        lines.append("  (none)")
    for cls, n in sorted(all_skins.items()):
        mark = "umodel-unsupported" if cls not in UMODEL_SUPPORTED_MATERIAL_CLASSES else "umodel-supported"
        lines.append(f"  mesh {cls}: {n} ({mark})")
    return lines


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dump", type=Path, help="ExportPackageProperties JSON")
    parser.add_argument("--package", help="package whose env dump is l2_skies.json and the like")
    parser.add_argument("--props-out", type=Path)
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("--audit", action="store_true")
    args = parser.parse_args()
    if args.audit:
        print("\n".join(audit_lines()))
        return
    path = args.dump
    if path is None and args.package:
        path = find_dump_file(args.package)
        if path is None:
            raise SystemExit(f"property dump missing for package={args.package}")
    if path is None:
        raise SystemExit("--dump or --package is required")
    dump = Dump.load(path)
    existing = [TEX_ASSETS]
    if args.dry_run:
        for exp in material_exports(dump):
            name = str(exp["name"])
            ue_class = str(exp["class"])
            group = dump.group_of(exp)
            text = dump.render(exp)
            if find_named(existing, dump.package, name + ".props.txt"):
                status = "satisfied"
            elif "elements not stored in the property dump" in text:
                status = "dump-incomplete"
            else:
                status = "from-dump"
            print(
                f"{status} package={dump.package} group={group} name={name} class={ue_class}"
            )
        return
    if args.props_out is None:
        raise SystemExit("--props-out is required to write")
    written = write_package(dump, args.props_out, existing)
    for path in written:
        print(path)
    print(f"wrote {len(written)} material props from {dump.path.name}", file=sys.stderr)


if __name__ == "__main__":
    main()
