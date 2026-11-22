#!/usr/bin/env python3
"""Export every brush/BSP face the bake classifies.

Texture (and Texture subclasses) go through ExportNamedTextures into
{out}/{package}/Texture/{leaf}.png. Any other Material subclass goes through
the step-9 umodel path: props for that object, then the same classification
on every reference in the props, down to base textures. An object that is
not in the package fails naming package, group, name, and class. The class
is the package export's class (export table, confirmed against the live utx).
"""
from __future__ import annotations

import argparse
import shlex
import subprocess
import sys
from collections import defaultdict
from pathlib import Path

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
    want = face.name.lower() + ".props.txt"
    package = face.package.lower()
    for root in roots:
        if not root.is_dir():
            continue
        for path in root.rglob("*.props.txt"):
            if path.name.lower() != want:
                continue
            if package in {part.lower() for part in path.parts}:
                return path
    return None


def umodel_export(client: Path, umodel: Path, out_dir: Path, utx: Path, names: list[str]) -> None:
    rel = None
    for folder in ("textures", "systextures"):
        candidate = client / folder / utx.name
        if candidate.is_file() or utx.parent.name.lower() == folder:
            rel = f"{utx.parent.name}/{utx.name}"
            break
    if rel is None:
        rel = f"textures/{utx.name}"
    quoted = " ".join(shlex.quote(n) for n in names)
    inner = (
        "apt-get update -qq && apt-get install -y -qq libpng16-16 libsdl2-2.0-0 >/dev/null && "
        f"/umodel -path=/client -game=l2 -export -out=/export {shlex.quote(rel)} {quoted}"
    )
    out_dir.mkdir(parents=True, exist_ok=True)
    proc = subprocess.run(
        [
            "docker", "run", "--rm", "--platform", "linux/amd64",
            "-v", f"{client}:/client:ro",
            "-v", f"{out_dir}:/export",
            "-v", f"{umodel}:/umodel:ro",
            "ubuntu:22.04", "bash", "-lc", inner,
        ],
        text=True,
    )
    if proc.returncode != 0:
        raise SystemExit(
            f"umodel material export failed package={utx.stem} objects={','.join(names)} "
            f"(exit {proc.returncode})"
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


def export_textures(client: Path, jar: Path, faces: list[Face], out_root: Path) -> list[str]:
    by_pkg: dict[str, list[Face]] = defaultdict(list)
    for face in faces:
        by_pkg[face.package].append(face)
    errors: list[str] = []
    utx_of: dict[str, Path | None] = {}
    for package, group in sorted(by_pkg.items()):
        utx = find_utx(client, package)
        utx_of[package] = utx
        if utx is None:
            continue
        out_dir = out_root / package / "Texture"
        out_dir.mkdir(parents=True, exist_ok=True)
        names = ",".join(sorted({face.name for face in group}))
        proc = subprocess.run(
            ["java", "-cp", str(jar), str(EXPORT_JAVA), str(utx), str(out_dir), names],
            text=True,
            capture_output=True,
        )
        sys.stderr.write(proc.stdout)
        sys.stderr.write(proc.stderr)
        if proc.returncode != 0:
            sys.stderr.write(
                f"ExportNamedTextures failed for package={package} (exit {proc.returncode})\n"
            )
    for face in faces:
        png = dest_png(out_root, face)
        if png.is_file():
            continue
        utx = utx_of.get(face.package)
        if utx is None:
            errors.append(f"{face.format()} utx missing under textures/ and systextures/")
        else:
            errors.append(f"{face.format()} texture export wrote no {png.name}")
    return errors


def export_faces(
    refs: list[str],
    client: Path,
    jar: Path,
    out_root: Path,
    props_out: Path,
    umodel: Path,
) -> None:
    idx = index()
    roots = [classify(ref, idx) for ref in refs]
    errors = [face.format() for face in roots if face.kind in (KIND_ABSENT, KIND_AMBIGUOUS, KIND_OTHER)]
    if errors:
        raise SystemExit("brush face class unresolved:\n" + "\n".join(errors))

    packages = sorted({face.package for face in roots})
    live: dict[str, list[tuple[str, str, str]]] = {}
    for package in packages:
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

    props_roots = [props_out, TEX_ASSETS]
    exported_materials: set[tuple[str, str, str]] = set()

    def read_props(face: Face) -> str | None:
        path = find_props(face, props_roots)
        if path is None:
            return None
        return path.read_text(encoding="utf-8", errors="replace")

    all_textures: dict[tuple[str, str, str], Face] = {}
    materials: list[Face] = []
    while True:
        textures, materials, chain_errors = expand_chain(roots, read_props)
        if chain_errors:
            raise SystemExit("brush material chain missing:\n" + "\n".join(chain_errors))
        for face in textures:
            all_textures.setdefault(face.key(), face)
            if face.package.lower() not in live:
                utx = find_utx(client, face.package)
                if utx is None:
                    errors.append(f"{face.format()} utx missing")
                    continue
                live[face.package.lower()] = describe_package(jar, utx)
            problem = confirm_live(face, live[face.package.lower()])
            if problem:
                errors.append(problem)
        for face in materials:
            if face.package.lower() not in live:
                utx = find_utx(client, face.package)
                if utx is None:
                    errors.append(f"{face.format()} utx missing")
                    continue
                live[face.package.lower()] = describe_package(jar, utx)
            problem = confirm_live(face, live[face.package.lower()])
            if problem:
                errors.append(problem)
        if errors:
            raise SystemExit("brush material chain missing:\n" + "\n".join(dict.fromkeys(errors)))
        need = [
            face for face in materials
            if read_props(face) is None and face.key() not in exported_materials
        ]
        if not need:
            break
        if not umodel.is_file():
            raise SystemExit(
                "umodel linux binary missing at "
                + str(umodel)
                + " — material faces need the step-9 props export:\n"
                + "\n".join(face.format() for face in need)
            )
        if subprocess.run(["docker", "version"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode != 0:
            raise SystemExit(
                "docker is required to export brush material chains:\n"
                + "\n".join(face.format() for face in need)
            )
        by_pkg: dict[str, list[Face]] = defaultdict(list)
        for face in need:
            by_pkg[face.package].append(face)
        for package, group in sorted(by_pkg.items()):
            utx = find_utx(client, package)
            if utx is None:
                raise SystemExit(f"{group[0].format()} utx missing")
            names = sorted({face.name for face in group})
            umodel_export(client, umodel, props_out / package, utx, names)
            for face in group:
                exported_materials.add(face.key())
                if read_props(face) is None:
                    errors.append(f"{face.format()} umodel wrote no props.txt")
        if errors:
            raise SystemExit("brush material chain missing:\n" + "\n".join(errors))

    tex_errors = export_textures(client, jar, list(all_textures.values()), out_root)
    if tex_errors:
        raise SystemExit("brush texture missing:\n" + "\n".join(tex_errors))

    packages_with_props = sorted({
        package for package in {face.package for face in materials}
        if (props_out / package).is_dir()
    })
    if packages_with_props:
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
    print(
        f"brush faces: {len(all_textures)} textures, "
        f"{len(exported_materials)} material props exported"
    )


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--brushes", action="append", required=True, type=Path)
    parser.add_argument("--client", type=Path)
    parser.add_argument("--out", type=Path)
    parser.add_argument("--props-out", type=Path)
    parser.add_argument("--jar", type=Path)
    parser.add_argument("--umodel", type=Path)
    parser.add_argument("--list", action="store_true")
    args = parser.parse_args()
    refs = collect_refs(args.brushes)
    if args.list:
        for ref in refs:
            face = classify(ref)
            print(f"{face.kind}\t{face.format()}\t{face.full}")
        return
    if args.client is None or args.out is None or args.jar is None or args.umodel is None:
        raise SystemExit("--client, --out, --jar, and --umodel are required to export")
    props_out = args.props_out or (args.out.parent / "props-export")
    export_faces(refs, args.client, args.jar, args.out, props_out, args.umodel)


if __name__ == "__main__":
    main()
