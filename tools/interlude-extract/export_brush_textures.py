#!/usr/bin/env python3
"""Export every brush/BSP face the bake classifies.

Texture (and Texture subclasses) go through ExportNamedTextures into
{out}/{package}/Texture/{leaf}.png. Any other Material subclass is rendered
from an ExportPackageProperties dump (material_props.py) into
{props-out}/{package}/{class}/{name}.props.txt. umodel is not used: its
supported-class list does not include ColorModifier or the other Modifier
subclasses. A props file or PNG that is already on disk is satisfied and is
not written again. An object that is not in the package fails naming
package, group, name, and class. The class is the package export's class
(export table, confirmed against the live utx when a client is passed).
"""
from __future__ import annotations

import argparse
import os
import subprocess
import sys
from collections import defaultdict
from pathlib import Path

from material_props import (
    Dump,
    find_dump_file,
    find_named,
    load_dump,
    props_dest,
    render_face,
    write_rendered,
)

from brush_face_class import (
    KIND_ABSENT,
    KIND_AMBIGUOUS,
    KIND_MATERIAL,
    KIND_OTHER,
    KIND_TEXTURE,
    Face,
    classify,
    confirm_live,
    index,
    props_tokens,
    resolve_props_token,
)

REPO = Path(__file__).resolve().parents[2]
EXPORT_JAVA = REPO / "tools" / "interlude" / "ExportNamedTextures.java"
DESCRIBE_JAVA = REPO / "tools" / "interlude" / "DescribeExports.java"
TEX_ASSETS = REPO / "l2-unity" / "Assets" / "Resources" / "Data" / "Textures"
DUMP_SCRIPT = REPO / "tools" / "interlude-extract" / "export_client_packages.sh"


def collect_refs(paths: list[Path]) -> list[str]:
    seen: dict[str, str] = {}
    for path in paths:
        if not path.is_file():
            raise SystemExit(f"brush source missing: {path}")
        for full in refs_in_file(path):
            seen.setdefault(full.lower(), full)
    if not paths:
        raise SystemExit("no brush source (Brushes.json or brushes.t3d)")
    return [seen[k] for k in sorted(seen)]


def refs_in_file(path: Path) -> list[str]:
    if path.suffix.lower() == ".json":
        import json
        data = json.loads(path.read_text(encoding="utf-8"))
        brushes = data.get("data")
        if not isinstance(brushes, list):
            raise SystemExit(f"Brushes.json has no brush array: {path}")
        out: list[str] = []
        for brush in brushes:
            model = (brush or {}).get("model") or {}
            poly = model.get("poly") or {}
            for face in poly.get("polyData") or []:
                ref = (face or {}).get("texture") or ""
                if ref:
                    out.append(ref)
        return out
    import re
    text = path.read_text(encoding="utf-8", errors="replace")
    return re.findall(r"Texture=(\S+)", text)


def find_utx(client: Path, package: str) -> Path | None:
    want = package.lower() + ".utx"
    for folder in ("textures", "systextures", "Textures", "SysTextures"):
        root = client / folder
        if not root.is_dir():
            continue
        for child in root.iterdir():
            if child.is_file() and child.name.lower() == want:
                return child
    return None


def describe_package(jar: Path, utx: Path) -> list[tuple[str, str, str]]:
    proc = subprocess.run(
        ["java", "-cp", str(jar), str(DESCRIBE_JAVA), str(utx)],
        text=True,
        capture_output=True,
    )
    if proc.returncode != 0:
        raise SystemExit(
            f"DescribeExports failed for {utx.name} (exit {proc.returncode})\n"
            + proc.stdout + proc.stderr
        )
    rows: list[tuple[str, str, str]] = []
    for line in proc.stdout.splitlines():
        parts = line.split("\t")
        if len(parts) < 2:
            continue
        group = parts[2] if len(parts) > 2 else ""
        if group == "None":
            group = ""
        rows.append((parts[0], parts[1], group))
    return rows


def find_props(face: Face, roots: list[Path]) -> Path | None:
    return find_named(roots, face.package, face.name + ".props.txt")


def find_png(face: Face, roots: list[Path]) -> Path | None:
    return find_named(roots, face.package, face.name + ".png")


def dump_packages(client: Path, packages: list[str], dest: Path) -> None:
    """Run ExportClientPackages for packages that have no dump yet."""
    utxs: list[str] = []
    for package in packages:
        utx = find_utx(client, package)
        if utx is None:
            raise SystemExit(
                f"package={package} group= name= class=absent utx missing under textures/ and systextures/"
            )
        utxs.append(str(utx))
    dest.mkdir(parents=True, exist_ok=True)
    env = os.environ.copy()
    env["EXPORT_OUT"] = str(dest)
    env["CRUMA_CLIENT"] = str(client)
    proc = subprocess.run(["bash", str(DUMP_SCRIPT), "--", *utxs], env=env, text=True)
    if proc.returncode != 0:
        raise SystemExit(
            "property dump failed for " + ", ".join(packages) + f" (exit {proc.returncode})"
        )


def expand_chain(
    roots: list[Face],
    read_props,
) -> tuple[list[Face], list[Face], list[str]]:
    """Split a face closure into textures and materials.

    read_props(face) returns the props text of a material, or None when it
    has not been exported yet. Materials with no props stay in `materials`
    so the caller can export them and call again.
    """
    textures: list[Face] = []
    materials: list[Face] = []
    errors: list[str] = []
    seen: set[tuple[str, str, str]] = set()
    queue = list(roots)
    while queue:
        face = queue.pop(0)
        if face.key() in seen:
            continue
        seen.add(face.key())
        if face.kind in (KIND_ABSENT, KIND_AMBIGUOUS, KIND_OTHER):
            errors.append(face.format())
            continue
        if face.kind == KIND_TEXTURE:
            textures.append(face)
            continue
        if face.kind != KIND_MATERIAL:
            errors.append(face.format())
            continue
        materials.append(face)
        text = read_props(face)
        if text is None:
            continue
        for token in props_tokens(text):
            queue.append(resolve_props_token(token, face.package))
    return textures, materials, errors


def dest_png(out_root: Path, face: Face) -> Path:
    return out_root / face.package / "Texture" / f"{face.name}.png"


def export_textures(
    client: Path | None,
    jar: Path | None,
    faces: list[Face],
    out_root: Path,
    present_roots: list[Path],
) -> list[str]:
    pending = [face for face in faces if find_png(face, present_roots) is None]
    by_pkg: dict[str, list[Face]] = defaultdict(list)
    for face in pending:
        by_pkg[face.package].append(face)
    errors: list[str] = []
    utx_of: dict[str, Path | None] = {}
    for package, group in sorted(by_pkg.items()):
        if client is None or jar is None:
            for face in group:
                errors.append(f"{face.format()} texture png missing and no client was given")
            continue
        utx = find_utx(client, package)
        utx_of[package] = utx
        if utx is None:
            continue
        out_dir = out_root / package / "Texture"
        out_dir.mkdir(parents=True, exist_ok=True)
        names = sorted({face.name for face in group})
        if any("," in name for name in names):
            raise SystemExit(
                "texture name contains a comma; ExportNamedTextures splits one argument on commas: "
                + ", ".join(face.format() for face in group if "," in face.name)
            )
        proc = subprocess.run(
            ["java", "-cp", str(jar), str(EXPORT_JAVA), str(utx), str(out_dir), ",".join(names)],
            text=True,
            capture_output=True,
        )
        sys.stderr.write(proc.stdout)
        sys.stderr.write(proc.stderr)
        if proc.returncode != 0:
            sys.stderr.write(
                f"ExportNamedTextures failed for package={package} (exit {proc.returncode})\n"
            )
    for face in pending:
        if find_png(face, present_roots + [out_root]) is not None:
            continue
        utx = utx_of.get(face.package)
        if client is None:
            continue
        if utx is None:
            errors.append(f"{face.format()} utx missing under textures/ and systextures/")
        else:
            errors.append(f"{face.format()} texture export wrote no {face.name}.png")
    return errors


def _dumps_for(packages: list[str], extra_dirs: list[Path]) -> dict[str, Dump]:
    found: dict[str, Dump] = {}
    for package in packages:
        dump = load_dump(package, extra_dirs)
        if dump is not None:
            found[package.lower()] = dump
    return found


def _rendered(face: Face, dumps: dict[str, Dump]) -> str | None:
    dump = dumps.get(face.package.lower())
    if dump is None:
        return None
    return render_face(dump, face)


def material_status(face: Face, props_roots: list[Path], dumps: dict[str, Dump]) -> str:
    if find_props(face, props_roots) is not None:
        return "satisfied"
    if _rendered(face, dumps) is not None:
        return "from-dump"
    if face.package.lower() in dumps or find_dump_file(face.package) is not None:
        return "not-in-dump"
    return "needs-property-dump"


def material_plan(
    refs: list[str],
    props_roots: list[Path],
    dump_dirs: list[Path] | None = None,
) -> list[tuple[Face, str]]:
    """One row per material in the brush closure. No umodel commands.

    Props already on disk are satisfied. A material whose dump can render it
    is from-dump. A package with no dump at all is needs-property-dump.
    """
    idx = index()
    roots = [classify(ref, idx) for ref in refs]
    errors = [
        face.format() for face in roots
        if face.kind in (KIND_ABSENT, KIND_AMBIGUOUS, KIND_OTHER)
    ]
    if errors:
        raise SystemExit("brush face class unresolved:\n" + "\n".join(errors))

    extra = list(dump_dirs or [])
    packages = sorted({face.package for face in roots})
    dumps = _dumps_for(packages, extra)

    def read_props(face: Face) -> str | None:
        path = find_props(face, props_roots)
        if path is not None:
            return path.read_text(encoding="utf-8", errors="replace")
        text = _rendered(face, dumps)
        if text is not None:
            return text
        # A child package may have a dump even when the root package did not.
        child = load_dump(face.package, extra)
        if child is None:
            return None
        dumps[face.package.lower()] = child
        return render_face(child, face)

    _textures, materials, chain_errors = expand_chain(roots, read_props)
    if chain_errors:
        raise SystemExit("brush material chain missing:\n" + "\n".join(chain_errors))
    return [(face, material_status(face, props_roots, dumps)) for face in materials]


def format_plan(rows: list[tuple[Face, str]]) -> str:
    return "\n".join(f"{status} {face.format()}" for face, status in rows)


def export_faces(
    refs: list[str],
    client: Path | None,
    jar: Path | None,
    out_root: Path,
    props_out: Path,
    dump_dir: Path | None = None,
) -> None:
    idx = index()
    roots = [classify(ref, idx) for ref in refs]
    errors = [face.format() for face in roots if face.kind in (KIND_ABSENT, KIND_AMBIGUOUS, KIND_OTHER)]
    if errors:
        raise SystemExit("brush face class unresolved:\n" + "\n".join(errors))

    if client is not None and jar is not None:
        live: dict[str, list[tuple[str, str, str]]] = {}
        for package in sorted({face.package for face in roots}):
            utx = find_utx(client, package)
            if utx is None:
                raise SystemExit(
                    f"package={package} group= name= class=absent utx missing under textures/ and systextures/"
                )
            live[package.lower()] = describe_package(jar, utx)
        for face in roots:
            problem = confirm_live(face, live[face.package.lower()])
            if problem:
                errors.append(problem)
        if errors:
            raise SystemExit("brush face class unresolved:\n" + "\n".join(errors))

    work_dumps = dump_dir or (props_out.parent / "property-dumps")
    extra = [work_dumps]
    props_roots = [props_out, TEX_ASSETS]
    dumps = _dumps_for(sorted({face.package for face in roots}), extra)
    written: list[Path] = []
    dumped: set[str] = set()

    def read_props(face: Face) -> str | None:
        path = find_props(face, props_roots)
        if path is not None:
            return path.read_text(encoding="utf-8", errors="replace")
        text = _rendered(face, dumps)
        if text is not None:
            return text
        child = load_dump(face.package, extra)
        if child is None:
            return None
        dumps[face.package.lower()] = child
        return render_face(child, face)

    materials: list[Face] = []
    all_textures: dict[tuple[str, str, str], Face] = {}
    missing: list[Face] = []
    while True:
        textures, materials, chain_errors = expand_chain(roots, read_props)
        if chain_errors:
            raise SystemExit("brush material chain missing:\n" + "\n".join(chain_errors))
        for face in textures:
            all_textures.setdefault(face.key(), face)
        missing = [face for face in materials if read_props(face) is None]
        if not missing:
            break
        packages = sorted({
            face.package for face in missing
            if face.package.lower() not in dumped and find_dump_file(face.package, extra) is None
        })
        if client is not None and packages:
            dump_packages(client, packages, work_dumps)
            dumped.update(package.lower() for package in packages)
            for package in packages:
                dump = load_dump(package, extra)
                if dump is not None:
                    dumps[package.lower()] = dump
            continue
        break

    for face in materials:
        if find_props(face, props_roots) is not None:
            continue
        text = _rendered(face, dumps)
        if text is None:
            continue
        dest = props_dest(props_out, face.package, face.ue_class, face.name)
        before = dest.is_file()
        write_rendered(dest, text)
        if not before:
            written.append(dest)

    if written:
        packages_with_props = sorted({path.relative_to(props_out).parts[0] for path in written})
        proc = subprocess.run(
            [
                sys.executable,
                str(REPO / "tools" / "interlude-extract" / "place_login_assets.py"),
                "props", "--src", str(props_out),
                "--packages", ",".join(packages_with_props),
            ],
        )
        if proc.returncode != 0:
            raise SystemExit("placing material props failed")

    if missing:
        lines = []
        for face in missing:
            if face.package.lower() in dumps or find_dump_file(face.package, extra) is not None:
                lines.append(f"{face.format()} not in the property dump")
            else:
                lines.append(f"{face.format()} property dump missing")
        raise SystemExit("property dump missing:\n" + "\n".join(lines))

    tex_errors = export_textures(client, jar, list(all_textures.values()), out_root, [TEX_ASSETS, out_root])
    if tex_errors:
        raise SystemExit("brush texture missing:\n" + "\n".join(tex_errors))
    print(
        f"brush faces: {len(all_textures)} textures, "
        f"{len(written)} material props from the property dump"
    )


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--brushes", action="append", required=True, type=Path)
    parser.add_argument("--client", type=Path)
    parser.add_argument("--out", type=Path)
    parser.add_argument("--props-out", type=Path)
    parser.add_argument("--jar", type=Path)
    parser.add_argument("--list", action="store_true")
    parser.add_argument(
        "--dry-run", action="store_true",
        help="print satisfied / from-dump / needs-property-dump for every material face",
    )
    args = parser.parse_args()
    refs = collect_refs(args.brushes)
    if args.list:
        for ref in refs:
            face = classify(ref)
            print(f"{face.kind}\t{face.format()}\t{face.full}")
        return
    if args.dry_run:
        rows = material_plan(refs, [TEX_ASSETS])
        print(format_plan(rows))
        counts: dict[str, int] = defaultdict(int)
        for _face, status in rows:
            counts[status] += 1
        summary = ", ".join(f"{n} {status}" for status, n in sorted(counts.items()))
        print(f"dry-run: {summary or 'no material faces'}", file=sys.stderr)
        return
    out_root = args.out or (REPO / ".work" / "lobby-extract" / "mesh-export")
    props_out = args.props_out or (out_root.parent / "props-export")
    export_faces(refs, args.client, args.jar, out_root, props_out)


if __name__ == "__main__":
    main()
