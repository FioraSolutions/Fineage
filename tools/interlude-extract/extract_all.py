#!/usr/bin/env python3
"""One entry point: decrypt, parse, index, and publish Interlude reference tables.

Usage (from the Fineage repo root or this directory):

  python3 tools/interlude-extract/extract_all.py \\
      --client "$HOME/Downloads/L2_Interlude_Client/Lineage II" \\
      --extract-root "$HOME/Projects/Fineage-ref-extracted" \\
      --docs l2-unity/Docs/interlude-ref

Raw binaries, PNG, and WAV stay under --extract-root (outside git).
Only TSV/JSON/text indexes are copied into --docs for the commit.
"""

from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))

from ddf_parser import parse_ddf, parse_dat, rows_to_tsv, schema_columns, ParseError  # noqa: E402
from decrypt import (  # noqa: E402
    decrypt_dat_file,
    decrypt_package_file,
    find_l2encdec,
    protocol_of,
)
from inspect_binaries import inspect_file, write_string_tsv  # noqa: E402
from package_index import index_package, PackageError  # noqa: E402

PACKAGE_EXTS = {".u", ".ukx", ".utx", ".usx", ".unr", ".uax", ".usk"}
MAX_COMMIT_BYTES = 20 * 1024 * 1024
KEY_PE = ("engine.dll", "core.dll", "nwindow.dll", "Window.dll", "L2.exe", "WinDrv.dll")


def default_client() -> Path:
    home = Path.home()
    for p in (
        home / "Downloads/L2_Interlude_Client/Lineage II",
        home / "Projects/Fineage/ref/l2-interlude-client/Lineage II",
    ):
        if p.is_dir():
            return p
    return home / "Downloads/L2_Interlude_Client/Lineage II"


def repo_root_from_here() -> Path:
    # tools/interlude-extract/ -> repo root
    return HERE.parents[1]


def write_text(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def split_if_huge(dest: Path, text: str, max_bytes: int = MAX_COMMIT_BYTES) -> list[Path]:
    raw = text.encode("utf-8")
    if len(raw) <= max_bytes:
        write_text(dest, text)
        return [dest]
    lines = text.splitlines(keepends=True)
    header = lines[0] if lines else ""
    out = []
    part = [header]
    size = len(header.encode())
    idx = 1
    body = lines[1:] if header else lines
    for line in body:
        b = len(line.encode())
        if part and size + b > max_bytes:
            p = dest.with_name(f"{dest.stem}.part{idx:02d}{dest.suffix}")
            write_text(p, "".join(part))
            out.append(p)
            idx += 1
            part = [header, line]
            size = len(header.encode()) + b
        else:
            part.append(line)
            size += b
    p = dest.with_name(f"{dest.stem}.part{idx:02d}{dest.suffix}")
    write_text(p, "".join(part))
    out.append(p)
    return out


def collect_packages(client: Path, extra_ref: Path | None) -> list[tuple[Path, str]]:
    files: list[tuple[Path, str]] = []
    seen = set()
    roots = [client]
    if extra_ref and extra_ref.is_dir() and extra_ref.resolve() != client.resolve():
        roots.append(extra_ref)
    for root in roots:
        for p in root.rglob("*"):
            if not p.is_file() or p.suffix.lower() not in PACKAGE_EXTS:
                continue
            try:
                rel = str(p.relative_to(root))
            except ValueError:
                rel = p.name
            key = rel.lower()
            if key in seen:
                continue
            seen.add(key)
            files.append((p, rel))
    return files


def step_dats(client: Path, extract: Path, docs: Path, log: dict) -> None:
    sysdir = client / "system"
    dec_dir = extract / "dat" / "decrypted"
    ddf_dir = HERE / "ddf" / "Interlude"
    tsv_dir = docs / "dats"
    raw_tsv = extract / "dat" / "tsv"
    tsv_dir.mkdir(parents=True, exist_ok=True)
    raw_tsv.mkdir(parents=True, exist_ok=True)
    l2encdec = find_l2encdec(repo_root_from_here())
    summary = []
    for src in sorted(sysdir.glob("*.dat")):
        dest = dec_dir / src.name
        proto = protocol_of(src.read_bytes()) or 413
        if not dest.exists():
            decrypt_dat_file(src, dest, l2encdec, protocol=proto)
        ddf = ddf_dir / (src.stem + ".ddf")
        row = {
            "file": src.name,
            "protocol": proto,
            "decrypted_bytes": dest.stat().st_size,
        }
        if not ddf.exists():
            row["status"] = "no-ddf"
            row["note"] = "No community DDF for this table; decrypted only."
            summary.append(row)
            continue
        try:
            schema = parse_ddf(ddf)
            rows, leftover = parse_dat(schema, dest.read_bytes())
            cols = schema_columns(schema)
            text = rows_to_tsv(rows, cols)
            raw_path = raw_tsv / (src.stem + ".tsv")
            write_text(raw_path, text)
            published = split_if_huge(tsv_dir / (src.stem + ".tsv"), text)
            row.update(
                {
                    "status": "ok",
                    "rows": len(rows),
                    "leftover": leftover,
                    "columns": len(cols),
                    "published": [str(p.relative_to(docs)) for p in published],
                }
            )
        except Exception as e:
            row["status"] = "parse-failed"
            row["error"] = str(e)
        summary.append(row)
    write_text(docs / "dats" / "summary.json", json.dumps(summary, indent=2, ensure_ascii=False))
    log["dats"] = summary


def step_packages(client: Path, extract: Path, docs: Path, extra_ref: Path | None, log: dict) -> None:
    dec_root = extract / "packages" / "decrypted"
    files = collect_packages(client, extra_ref)
    dec_info = []
    for src, rel in files:
        dest = dec_root / rel
        if dest.exists() and dest.stat().st_size > 0:
            from decrypt import is_upackage

            dec_info.append(
                {
                    "rel": rel,
                    "skipped_existing": True,
                    "upackage": is_upackage(dest.read_bytes()[:8] + dest.read_bytes()[0:0] or dest.read_bytes()),
                    "size": dest.stat().st_size,
                }
            )
            # cheaper upackage check
            dec_info[-1]["upackage"] = dest.read_bytes()[:4] == b"\xc1\x83\x2a\x9e"
            continue
        try:
            info = decrypt_package_file(src, dest)
            info["rel"] = rel
            dec_info.append(info)
        except Exception as e:
            dec_info.append({"rel": rel, "error": str(e)})
    # index
    index_rows = ["folder\tfile\text\tversion\tlicensee\tobject\tclass\tsuper\tserial_size"]
    pkg_summary = []
    failed = []
    for item in dec_info:
        rel = item.get("rel")
        if not rel or item.get("error"):
            if rel:
                failed.append(item)
            continue
        dest = dec_root / rel
        try:
            idx = index_package(dest.read_bytes(), rel)
        except Exception as e:
            failed.append({"rel": rel, "index_error": str(e)})
            continue
        folder = str(Path(rel).parent)
        fname = Path(rel).name
        ext = Path(rel).suffix.lower()
        pkg_summary.append(
            {
                "rel": rel,
                "version": idx["version"],
                "licensee": idx["licensee"],
                "names": idx["name_count"],
                "exports": idx["export_count"],
                "imports": idx["import_count"],
            }
        )
        for exp in idx["exports"]:
            index_rows.append(
                "\t".join(
                    [
                        folder,
                        fname,
                        ext,
                        str(idx["version"]),
                        str(idx["licensee"]),
                        exp["object"].replace("\t", " "),
                        exp["class"].replace("\t", " "),
                        exp["super"].replace("\t", " "),
                        str(exp["serial_size"]),
                    ]
                )
            )
        # per-package names list (small, keep raw only)
        names_path = extract / "packages" / "names" / (rel + ".names.txt")
        names_path.parent.mkdir(parents=True, exist_ok=True)
        names_path.write_text("\n".join(idx["names"]) + "\n", encoding="utf-8")

    docs_pkg = docs / "packages"
    docs_pkg.mkdir(parents=True, exist_ok=True)
    split_if_huge(docs_pkg / "exports.tsv", "\n".join(index_rows) + "\n")
    write_text(docs_pkg / "packages.json", json.dumps(pkg_summary, indent=2))
    write_text(docs_pkg / "index-failures.json", json.dumps(failed, indent=2))
    # Compact class index (one row per UClass export) for skill/UI lookups.
    class_rows = ["folder\tfile\tobject\tsuper\tserial_size"]
    # rebuild from exports.tsv parts if present
    exp_parts = sorted(docs_pkg.glob("exports*.tsv"))
    for pth in exp_parts:
        lines = pth.read_text(encoding="utf-8").splitlines()
        if not lines:
            continue
        cols = lines[0].split("\t")
        for line in lines[1:]:
            r = dict(zip(cols, line.split("\t")))
            if r.get("class") == "Class":
                class_rows.append("\t".join([
                    r.get("folder",""), r.get("file",""), r.get("object",""),
                    r.get("super",""), r.get("serial_size",""),
                ]))
    write_text(docs_pkg / "classes.tsv", "\n".join(class_rows) + "\n")
    log["packages"] = {
        "count": len(files),
        "indexed": len(pkg_summary),
        "failed": len(failed),
        "failures": failed[:40],
    }


def step_binaries(client: Path, extract: Path, docs: Path, log: dict) -> None:
    sysdir = client / "system"
    # case-insensitive names on macOS: pick unique inodes
    seen = set()
    files = []
    for name in KEY_PE:
        p = sysdir / name
        if not p.exists():
            continue
        ino = p.stat().st_ino
        if ino in seen:
            continue
        seen.add(ino)
        files.append(p)
    # also any other PE in system
    for p in sysdir.iterdir():
        if p.suffix.lower() in {".dll", ".exe"} and p.stat().st_ino not in seen:
            seen.add(p.stat().st_ino)
            files.append(p)

    bin_docs = docs / "binaries"
    bin_docs.mkdir(parents=True, exist_ok=True)
    raw = extract / "binaries"
    raw.mkdir(parents=True, exist_ok=True)
    packer_rows = ["file\tsize\tpacker\tunpacked\tentry\timagebase\tsections\tnotes"]
    reports = []
    for p in files:
        info = inspect_file(p)
        strings = info.pop("all_strings")
        write_string_tsv(strings, raw / f"{p.name}.strings.tsv")
        # commit interesting strings only (keep size down)
        interesting = info.get("interesting_strings") or []
        write_text(
            bin_docs / f"{p.name}.interesting-strings.txt",
            "\n".join(interesting) + ("\n" if interesting else ""),
        )
        pe = info.get("pe") or {}
        packer_rows.append(
            "\t".join(
                [
                    p.name,
                    str(info["size"]),
                    str(info["packer"]),
                    "yes" if info["unpacked"] else "no",
                    hex(pe.get("entry", 0)),
                    hex(pe.get("imagebase", 0)),
                    str(pe.get("sections_n", "")),
                    " | ".join(info.get("notes") or []),
                ]
            )
        )
        # drop huge fields before json
        slim = {k: v for k, v in info.items() if k != "interesting_strings"}
        slim["interesting_string_count"] = len(interesting)
        reports.append(slim)
    write_text(bin_docs / "packers.tsv", "\n".join(packer_rows) + "\n")
    write_text(bin_docs / "packers.json", json.dumps(reports, indent=2))
    log["binaries"] = reports



def _norm(s: str) -> str:
    return "".join(ch if ch.isalnum() else "_" for ch in (s or "").lower()).strip("_")


def _match_effect_classes(name: str, desc: str, icon: str, effect_names: list[str]) -> list[str]:
    stems = []
    for raw in (name, desc, icon):
        n = _norm(raw)
        if n:
            stems.append(n)
        # icon often like skill.power_strike or icon.skill0003
        if "." in (raw or ""):
            stems.append(_norm(raw.split(".")[-1]))
    stems = [s for s in stems if len(s) >= 4]
    hits = []
    lower_map = [(e, e.lower()) for e in effect_names]
    for e, el in lower_map:
        eln = _norm(e)
        for st in stems:
            if st and (st in eln or eln.endswith(st) or f"_{st}_" in f"_{eln}_"):
                hits.append(e)
                break
    # unique preserve
    seen=set(); out=[]
    for h in hits:
        if h not in seen:
            seen.add(h); out.append(h)
    return out


def step_skill_visuals(docs: Path, log: dict) -> None:
    """Combine skillgrp + package effect class names into a skill visual catalog."""
    skillgrp = docs / "dats" / "skillgrp.tsv"
    skillname = docs / "dats" / "skillname-e.tsv"
    exports = docs / "packages" / "exports.tsv"
    if not skillgrp.exists():
        log["skill_visuals"] = {"status": "skipped", "reason": "skillgrp.tsv missing"}
        return

    def load_tsv(path: Path) -> list[dict]:
        parts = []
        if path.exists():
            parts = [path]
        else:
            parts = sorted(path.parent.glob(path.stem + ".part*" + path.suffix))
            if not parts:
                # also accept stem.part01.tsv when dest was split
                parts = sorted(path.parent.glob(path.name.replace(path.suffix, ".part*" + path.suffix)))
        rows = []
        cols = None
        for pth in parts:
            lines = pth.read_text(encoding="utf-8").splitlines()
            if not lines:
                continue
            cols = lines[0].split("\t")
            for line in lines[1:]:
                if not line.strip():
                    continue
                fields = line.split("\t")
                rows.append({cols[i]: fields[i] if i < len(fields) else "" for i in range(len(cols))})
        return rows

    grp = load_tsv(skillgrp)
    names = { (r.get("id"), r.get("level")): r for r in (load_tsv(skillname) if skillname.exists() else []) }

    effect_classes: list[str] = []
    classes_path = docs / "packages" / "classes.tsv"
    if classes_path.exists():
        for r in load_tsv(classes_path):
            if r.get("file", "").lower() == "lineageeffect.u" and r.get("object"):
                effect_classes.append(r["object"])
    if not effect_classes and (exports.exists() or list((docs / "packages").glob("exports*.tsv"))):
        for r in load_tsv(exports):
            if r.get("file", "").lower() in {"lineageeffect.u", "lineageeffect.u"} or r.get("file", "").lower() == "lineageeffect.u":
                if r.get("class") in {"Class", "None"} or r.get("class") == "Class":
                    effect_classes.append(r.get("object", ""))
                # UE export of a class object often has class=Class
                if r.get("class") == "Class":
                    pass
        # also any export whose class ends with Emitter or Effect
        for r in load_tsv(exports):
            if "effect" in r.get("file", "").lower() and r.get("class") == "Class":
                if r.get("object") and r.get("object") not in effect_classes:
                    effect_classes.append(r["object"])

    effect_set = {c.lower(): c for c in effect_classes if c}
    out_rows = [
        "skill_id\tlevel\tname\tani_char\tdesc\ticon_name\textra_eff\teffect_class\tresolution"
    ]
    stats = {
        "skill_rows": 0,
        "unique_ids": set(),
        "has_ani": 0,
        "has_desc": 0,
        "has_icon": 0,
        "desc_is_effect_key": 0,
        "matched_effect_class": 0,
        "unresolved": 0,
    }
    unique_res = {}
    for r in grp:
        sid = r.get("skill_id", "")
        lvl = r.get("skill_level") or r.get("level") or ""
        ani = r.get("ani_char", "")
        desc = r.get("desc", "")
        icon = r.get("icon_name", "")
        extra = r.get("extra_eff", "")
        nm = (names.get((sid, lvl)) or {}).get("name", "")
        stats["skill_rows"] += 1
        stats["unique_ids"].add(sid)
        if ani:
            stats["has_ani"] += 1
        if desc:
            stats["has_desc"] += 1
        if icon:
            stats["has_icon"] += 1
        effect = ""
        resolution = "unresolved"
        key = desc.strip()
        matched = _match_effect_classes(nm, desc, icon, list(effect_set.values()))
        if key.lower() in effect_set:
            effect = effect_set[key.lower()]
            resolution = "effect_class_from_desc"
            stats["matched_effect_class"] += 1
        elif matched:
            effect = ",".join(matched[:8])
            resolution = "effect_class_by_name"
            stats["matched_effect_class"] += 1
        elif key.startswith("skill.") or key.startswith("Skill"):
            stats["desc_is_effect_key"] += 1
            resolution = "skillgrp_desc_key"
            effect = key
        elif extra not in ("", "0"):
            resolution = "skillgrp_extra_eff"
            effect = extra
        elif ani:
            resolution = "skillgrp_ani_only"
        if resolution == "unresolved":
            stats["unresolved"] += 1
        unique_res.setdefault(sid, resolution)
        out_rows.append(
            "\t".join([sid, lvl, nm, ani, desc, icon, extra, effect, resolution])
        )

    dest = docs / "skills" / "skill-visuals.tsv"
    split_if_huge(dest, "\n".join(out_rows) + "\n")
    # unique-id rollup: best resolution per id (prefer effect class / desc key)
    rank = {
        "effect_class_from_desc": 0,
        "effect_class_by_name": 1,
        "skillgrp_desc_key": 2,
        "skillgrp_extra_eff": 2,
        "skillgrp_ani_only": 3,
        "unresolved": 4,
    }
    id_best = {}
    for r in grp:
        sid = r.get("skill_id", "")
        # last written in unique_res is last level; recompute properly below
    id_best = {}
    # re-read out_rows
    for line in out_rows[1:]:
        parts = line.split("\t")
        sid, res = parts[0], parts[-1]
        prev = id_best.get(sid)
        if prev is None or rank.get(res, 9) < rank.get(prev, 9):
            id_best[sid] = res
    from collections import Counter

    id_counts = Counter(id_best.values())
    summary = {
        "skill_rows": stats["skill_rows"],
        "unique_skill_ids": len(stats["unique_ids"]),
        "rows_with_ani": stats["has_ani"],
        "rows_with_desc": stats["has_desc"],
        "rows_with_icon": stats["has_icon"],
        "rows_desc_is_effect_key": stats["desc_is_effect_key"],
        "rows_matched_effect_class": stats["matched_effect_class"],
        "rows_unresolved": stats["unresolved"],
        "unique_id_resolution": dict(id_counts),
        "effect_classes_indexed": len(effect_set),
    }
    write_text(docs / "skills" / "skill-visuals-summary.json", json.dumps(summary, indent=2))
    log["skill_visuals"] = summary


def write_readme(docs: Path, log: dict) -> None:
    dats = log.get("dats") or []
    ok = [d for d in dats if d.get("status") == "ok"]
    no = [d for d in dats if d.get("status") != "ok"]
    pkg = log.get("packages") or {}
    bins = log.get("binaries") or []
    vis = log.get("skill_visuals") or {}
    lines = [
        "# Interlude reference catalog",
        "",
        "Text tables and indexes extracted once from the official Interlude client,",
        "so agents do not have to unpack the same files on every task.",
        "",
        "Re-run (Mac, official client present):",
        "",
        "```bash",
        "python3 tools/interlude-extract/extract_all.py \\",
        "  --client \"$HOME/Downloads/L2_Interlude_Client/Lineage II\" \\",
        "  --extract-root \"$HOME/Projects/Fineage-ref-extracted\" \\",
        "  --docs l2-unity/Docs/interlude-ref",
        "```",
        "",
        "The entry script lives at `tools/interlude-extract/extract_all.py`.",
        "DAT field schemas are in `tools/interlude-extract/ddf/Interlude/` (community l2asm DDF set,",
        "with two Interlude-specific corrections: `huntingzone-e` uses `ASCF extra` like the C5 layout,",
        "`logongrp` positions are IEEE floats).",
        "",
        "Raw decrypted binaries, PNG, and WAV are **not** committed. They go to",
        "`~/Projects/Fineage-ref-extracted/` on the machine that ran the extractor.",
        "Single committed files are split if they would exceed ~20 MB.",
        "",
        "## Layout",
        "",
        "| Path | Contents |",
        "|------|----------|",
        "| `dats/*.tsv` | Decrypted `system/*.dat` tables (Lineage2Ver413) |",
        "| `dats/summary.json` | Row counts and parse status per table |",
        "| `packages/exports.tsv` | Every Unreal export: package, object, class, size |",
        "| `packages/packages.json` | Per-package name/export/import counts |",
        "| `binaries/packers.tsv` | PE packer identification for client binaries |",
        "| `binaries/*.interesting-strings.txt` | Keyword-filtered strings from each PE |",
        "| `skills/skill-visuals.tsv` | skill_id × animation / desc / icon / effect resolution |",
        "| `skills/skill-visuals-summary.json` | Resolution counts |",
        "| `scripts/` | Decompiled UnrealScript from `Interface.u` and other `.u` with scripts (if produced) |",
        "",
        "## How it is produced",
        "",
        "1. `system/*.dat` are Lineage2Ver413; decrypted with open-l2encdec (`-p 413 -l`).",
        "2. Tables are parsed with the DDF schemas (ASCF/CNTR packed counters, UNICODE, MTX/MAT, ENBBY).",
        "3. Unreal packages (`.u` `.ukx` `.utx` `.usx` `.unr` `.uax` `.usk`) are Lineage2Ver111 (XOR 0xAC)",
        "   or Lineage2Ver121 (XOR from the lowercase filename). The 28-byte header is stripped.",
        "4. Package export indexes are built from the UE2 name/import/export tables.",
        "5. PE binaries are classified from section names, entry point, and entropy. Only `engine.dll`",
        "   is packed (Themida). The others are ordinary PE and can be string-scanned directly.",
        "6. Skill visuals are resolved from Interlude `skillgrp` (`ani_char`, `desc`, `icon_name`, `extra_eff`)",
        "   plus `LineageEffect` class names from the package index. Hardcoded C++ maps inside packed",
        "   `engine.dll` are recorded as unresolved until that binary can be dumped.",
        "",
        f"## DAT tables ({len(ok)} parsed)",
        "",
        "| File | Rows | Status |",
        "|------|------|--------|",
    ]
    for d in dats:
        lines.append(f"| {d.get('file','')} | {d.get('rows','')} | {d.get('status','')} |")
    if no:
        lines += ["", "Tables without a DDF or with a parse error stay as decrypted binaries in the extract root only."]
    lines += [
        "",
        f"## Packages",
        "",
        f"Indexed {pkg.get('indexed', 0)} of {pkg.get('count', 0)} packages;"
        f" {pkg.get('failed', 0)} index failures (see `packages/index-failures.json`).",
        "",
        "## Binaries",
        "",
        "| File | Packer | Unpacked |",
        "|------|--------|----------|",
    ]
    for b in bins:
        if b.get("file", "").lower() in {n.lower() for n in ("engine.dll", "core.dll", "nwindow.dll", "Window.dll", "L2.exe")}:
            lines.append(f"| {b.get('file')} | {b.get('packer')} | {'yes' if b.get('unpacked') else 'no'} |")
    lines += [
        "",
        "## Skill visuals",
        "",
        "```json",
        json.dumps(vis, indent=2, default=list),
        "```",
        "",
    ]
    write_text(docs / "README.md", "\n".join(lines) + "\n")


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--client", type=Path, default=default_client())
    ap.add_argument("--extract-root", type=Path, default=Path.home() / "Projects/Fineage-ref-extracted")
    ap.add_argument("--docs", type=Path, default=None)
    ap.add_argument("--extra-ref", type=Path, default=Path.home() / "Projects/Fineage/ref/l2-interlude-client/Lineage II")
    ap.add_argument(
        "--steps",
        default="dats,packages,binaries,skills,readme",
        help="Comma-separated: dats,packages,binaries,skills,readme",
    )
    args = ap.parse_args()
    docs = args.docs or (repo_root_from_here() / "l2-unity/Docs/interlude-ref")
    if not docs.is_absolute():
        docs = (Path.cwd() / docs).resolve()
    extract = args.extract_root.resolve()
    client = args.client.resolve()
    extract.mkdir(parents=True, exist_ok=True)
    docs.mkdir(parents=True, exist_ok=True)
    steps = {s.strip() for s in args.steps.split(",") if s.strip()}
    log: dict = {"client": str(client), "extract_root": str(extract), "docs": str(docs)}
    if "dats" in steps:
        print("== dats ==", flush=True)
        step_dats(client, extract, docs, log)
    if "binaries" in steps:
        print("== binaries ==", flush=True)
        step_binaries(client, extract, docs, log)
    if "packages" in steps:
        print("== packages ==", flush=True)
        step_packages(client, extract, docs, args.extra_ref, log)
    if "skills" in steps:
        print("== skills ==", flush=True)
        step_skill_visuals(docs, log)
    if "readme" in steps:
        print("== readme ==", flush=True)
        write_readme(docs, log)
    write_text(extract / "logs" / "extract_all.json", json.dumps(log, indent=2, default=list))
    print(json.dumps({k: (len(v) if isinstance(v, list) else v) for k, v in log.items() if k != "dats"}, indent=2, default=str))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
