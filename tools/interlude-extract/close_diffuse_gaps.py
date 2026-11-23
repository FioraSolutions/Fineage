#!/usr/bin/env python3
"""Close the lobby material gap: export what committed props name but the tree lacks.

Two row sources, one pipeline:

  - lobby_diffuse_closure.missing: Diffuse textures with no PNG.
  - ops rows: TexPanner / TexOscillator / TexEnvMap / Combiner / ... references
    with no props file (the same scan material-ops-gap.md reports, resolved
    here to a package).

A row's class is never assumed from the reference text: export_faces
classifies every object against the client package (DescribeExports), so a
Combiner row renders props while a Texture row exports a PNG, and the chain
walk pulls in whatever those name. This replaces step 6's old per-row
ExportNamedTextures loop, which failed loudly on the first material row
(de_fire_glow_s is a Combiner).
"""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

import lobby_diffuse_closure as closure
from build_ue2_material_gap import MODIFIER, REF
from export_brush_textures import describe_package, export_faces, find_utx

REPO = Path(__file__).resolve().parents[2]
ZONE_CATALOG = REPO / "l2-unity/Assets/StreamingAssets/Data/Meta/World/zone-render.tsv"

Row = tuple[str, str, str, str]  # want package, own package, leaf, reference class


def flare_rows(catalog: Path, map_name: str, tex: Path) -> list[Row]:
    """SkyZoneInfo lens-flare textures the zone catalog names for this map.

    L2ZoneRender.ReportFlares loads each as Resources Data/Textures/<Pkg>/<name>;
    a missing PNG is a [ZoneFog] error at runtime. The refs are fully qualified
    (Engine.Flare01), so want == own: a flare absent from its named package is
    an error, not a fallback candidate. A PNG already in the package folder is
    satisfied.
    """
    if not catalog.is_file():
        return []
    rows: list[Row] = []
    seen: set[str] = set()
    for line in catalog.read_text(encoding="utf-8").splitlines()[1:]:
        cells = line.rstrip("\r").split("\t")
        if len(cells) < 15 or cells[0] != map_name:
            continue
        for ref in cells[14].split("|"):
            ref = ref.strip()
            if not ref or "." not in ref or ref.lower() in seen:
                continue
            seen.add(ref.lower())
            package, leaf = ref.rsplit(".", 1)
            if closure.png_in_package(package, leaf, tex):
                continue
            rows.append((package, package, leaf, "LensFlare"))
    return sorted(rows)


def ops_rows(tex: Path) -> list[Row]:
    """Modifier references with no props file, resolved to a package.

    Mirrors the gap scan in build_ue2_material_gap (a short name with props
    anywhere is satisfied), keeping the referring file's package: a bare
    TexPanner'X' is an export of the referring material's own package, a
    qualified one resolves like a diffuse reference.
    """
    props_files = sorted(tex.rglob("*.props.txt"))
    known = {p.name[: -len(".props.txt")].lower() for p in props_files}
    rows: list[Row] = []
    seen: set[tuple[str, str, str]] = set()
    for path in props_files:
        own = closure.package_of(path, tex)
        text = path.read_text(encoding="utf-8", errors="replace")
        for cls, full in REF.findall(text):
            if cls not in MODIFIER:
                continue
            leaf = full.split(".")[-1]
            if not leaf or leaf.lower() in known:
                continue
            want = closure.want_package(full, own) if "." in full else own
            key = (want.lower(), own.lower(), leaf.lower())
            if key in seen:
                continue
            seen.add(key)
            rows.append((want, own, leaf, cls))
    rows.sort()
    return rows


def resolve_refs(rows: list[Row], live_names) -> tuple[list[str], list[str]]:
    """Rows -> 'Package.leaf' refs for export_faces.

    live_names(package) returns the object's names in that package's .utx,
    or None when the file does not exist. The package that actually contains
    the object wins: the named package first, then the referring material's
    own package (a Group.Object reference is an export of it). An object in
    neither is an error, not a guess.
    """
    refs: list[str] = []
    errors: list[str] = []
    for want, own, leaf, cls in rows:
        names = live_names(want)
        if names is not None and leaf.lower() in names:
            refs.append(f"{want}.{leaf}")
            continue
        if own.lower() != want.lower():
            own_names = live_names(own)
            if own_names is not None and leaf.lower() in own_names:
                refs.append(f"{own}.{leaf}")
                continue
        detail = "utx missing" if names is None else "object not in it"
        errors.append(f"package={want} own={own} name={leaf} ref-class={cls}: {detail}")
    return refs, errors


def live_resolver(client: Path, jar: Path):
    """live_names backed by DescribeExports, one describe per package."""
    cache: dict[str, set[str] | None] = {}

    def live_names(package: str) -> set[str] | None:
        key = package.lower()
        if key not in cache:
            utx = find_utx(client, package)
            cache[key] = (
                None
                if utx is None
                else {name.lower() for name, _cls, _grp in describe_package(jar, utx)}
            )
        return cache[key]

    return live_names


def gather_rows(
    packages: set[str] | None,
    tex: Path,
    catalog: Path | None = None,
    map_name: str = "Lobby",
) -> list[Row]:
    diffuse = [(want, own, leaf, "Texture") for want, own, leaf in closure.missing(packages, tex=tex)]
    seen = {(want.lower(), own.lower(), leaf.lower()) for want, own, leaf, _ in diffuse}
    rows = list(diffuse)
    for row in ops_rows(tex):
        if (row[0].lower(), row[1].lower(), row[2].lower()) not in seen:
            rows.append(row)
    if catalog is not None:
        for row in flare_rows(catalog, map_name, tex):
            if (row[0].lower(), row[1].lower(), row[2].lower()) not in seen:
                rows.append(row)
    return rows


def main(argv: list[str]) -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--client", type=Path, help="Interlude client root (textures/, systextures/)")
    ap.add_argument("--jar", type=Path, help="L2TerrainExtractor fat jar with DescribeExports")
    ap.add_argument("--out", type=Path, help="texture export work dir")
    ap.add_argument("--props-out", type=Path, help="props render work dir")
    ap.add_argument("--packages", help="comma-separated lobby packages; default is Lobby.unr imports")
    ap.add_argument("--map", default="Lobby", help="map whose SkyZoneInfo lens flares are closed (default Lobby)")
    ap.add_argument("--flare-catalog", type=Path, default=ZONE_CATALOG, help=argparse.SUPPRESS)
    ap.add_argument("--tex", type=Path, default=closure.TEX, help=argparse.SUPPRESS)
    ap.add_argument("--list", action="store_true", help="print rows and exit; no client needed")
    args = ap.parse_args(argv)
    packages = (
        {p.strip().lower() for p in args.packages.split(",") if p.strip()}
        if args.packages
        else closure.lobby_packages()
    )
    rows = gather_rows(packages, args.tex, args.flare_catalog, args.map)
    if args.list:
        for want, own, leaf, cls in rows:
            print(f"package={want} own={own} name={leaf} ref-class={cls}")
        print(f"lobby material gap: {len(rows)} rows", file=sys.stderr)
        return 0
    if not rows:
        print("lobby diffuse closure and material ops satisfied — nothing to export")
        return 0
    for flag in ("client", "jar", "out", "props_out"):
        if getattr(args, flag) is None:
            ap.error(f"--{flag.replace('_', '-')} is required without --list")
    refs, errors = resolve_refs(rows, live_resolver(args.client, args.jar))
    if errors:
        raise SystemExit("lobby material gap unresolved:\n" + "\n".join(errors))
    refs = list(dict.fromkeys(refs))
    print(f"lobby material gap: {len(refs)} objects to export or render ({len(rows)} rows)")
    export_faces(refs, args.client, args.jar, args.out, args.props_out)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
