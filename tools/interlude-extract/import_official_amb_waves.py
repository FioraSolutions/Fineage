#!/usr/bin/env python3
"""Copy official ambsound / AmbSound2 waves into Resources and append official-waves.tsv.

Does not copy original .uax packages. Existing official-waves rows are kept.
"""
from __future__ import annotations

import csv
import shutil
import sys
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
ASSETS = ROOT / "l2-unity" / "Assets"
SOUNDS_RES = ASSETS / "Resources" / "Data" / "Sounds"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "sounds"
STREAM = ASSETS / "StreamingAssets" / "Data" / "Meta" / "Sounds"
CATALOG = DOCS / "sound-catalog.tsv"
SCOPE = {"ambsound", "AmbSound2"}

AUDIO_META = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1.0
    conversionMode: 0
    preloadAudioData: 0
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

FOLDER_META = """fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def resources_path(rel_under_sounds: Path) -> str:
    return "Data/Sounds/" + rel_under_sounds.with_suffix("").as_posix()


def ensure_folder_meta(dirpath: Path) -> None:
    dirpath.mkdir(parents=True, exist_ok=True)
    meta = dirpath.with_name(dirpath.name + ".meta")
    if not meta.exists():
        meta.write_text(FOLDER_META.format(guid=uuid.uuid4().hex), encoding="utf-8")


def find_extracted(extracted_root: Path, package: str, group: str, name: str) -> Path | None:
    pkg_dir = extracted_root / package
    if not pkg_dir.is_dir():
        matches = [p for p in extracted_root.iterdir() if p.is_dir() and p.name.lower() == package.lower()]
        pkg_dir = matches[0] if matches else pkg_dir
    candidates = []
    if group:
        candidates.append(pkg_dir / group / f"{name}.wav")
        candidates.append(pkg_dir / group / f"{name}.ogg")
    candidates.append(pkg_dir / f"{name}.wav")
    for c in candidates:
        if c.is_file():
            return c
    if pkg_dir.is_dir():
        low = name.lower()
        group_low = (group or "").lower()
        for p in pkg_dir.rglob("*"):
            if p.suffix.lower() not in {".wav", ".ogg"}:
                continue
            if p.stem.lower() != low:
                continue
            if not group_low or p.parent.name.lower() == group_low or p.parent == pkg_dir:
                return p
    return None


def dest_for(package: str, group: str, name: str, src: Path) -> Path:
    root = SOUNDS_RES / ("AmbSound2" if package.lower() == "ambsound2" else "AmbSound")
    if group:
        return root / group / f"{name}{src.suffix}"
    return root / f"{name}{src.suffix}"


def main() -> int:
    if len(sys.argv) < 2:
        print("Usage: import_official_amb_waves.py <extracted-grouped-root>", file=sys.stderr)
        return 2
    extracted = Path(sys.argv[1]).resolve()
    if not extracted.is_dir():
        print(f"missing extract root: {extracted}", file=sys.stderr)
        return 1

    catalog_rows = []
    with CATALOG.open(encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")
        fieldnames = reader.fieldnames
        for rec in reader:
            catalog_rows.append(rec)

    tsv_path = STREAM / "official-waves.tsv"
    existing = []
    if tsv_path.is_file():
        with tsv_path.open(encoding="utf-8", newline="") as f:
            existing = list(csv.DictReader(f, delimiter="\t"))
    kept = [r for r in existing if r.get("package") not in SCOPE]
    added = []
    missing_src = []
    copied = 0
    kept_existing = 0

    ensure_folder_meta(SOUNDS_RES / "AmbSound")
    ensure_folder_meta(SOUNDS_RES / "AmbSound2")

    for rec in catalog_rows:
        package = rec["package"]
        if package not in SCOPE:
            continue
        name = rec["name"]
        group = rec.get("group") or ""
        src = find_extracted(extracted, package, group, name)
        if src is None:
            missing_src.append((package, group, name))
            continue
        dest = dest_for(package, group, name, src)
        ensure_folder_meta(dest.parent)
        if dest.is_file():
            kept_existing += 1
        else:
            dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, dest)
            meta = Path(str(dest) + ".meta")
            if not meta.exists():
                meta.write_text(AUDIO_META.format(guid=uuid.uuid4().hex), encoding="utf-8")
            copied += 1
        added.append({
            "package": package,
            "group": group,
            "name": name,
            "resources_path": resources_path(dest.relative_to(SOUNDS_RES)),
        })

    table = kept + added
    STREAM.mkdir(parents=True, exist_ok=True)
    with tsv_path.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=["package", "group", "name", "resources_path"], delimiter="\t", lineterminator="\n")
        w.writeheader()
        for row in table:
            w.writerow(row)
    shutil.copy2(tsv_path, DOCS / "official-waves.tsv")

    present = {(r["package"].lower(), r["name"].lower(), (r["group"] or "").lower()) for r in added}
    updated = 0
    for rec in catalog_rows:
        if rec["package"] not in SCOPE:
            continue
        key = (rec["package"].lower(), rec["name"].lower(), (rec.get("group") or "").lower())
        if key in present and rec.get("fineage") != "resources_clip":
            rec["fineage"] = "resources_clip"
            updated += 1
    with CATALOG.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=fieldnames, delimiter="\t", lineterminator="\n")
        w.writeheader()
        for rec in catalog_rows:
            w.writerow(rec)

    print(f"copied={copied} kept_existing={kept_existing} amb_rows={len(added)} table={len(table)}")
    print(f"catalog_updated={updated} missing_extract={len(missing_src)}")
    for row in missing_src[:30]:
        print(" MISSING", row)
    return 0 if not missing_src else 1


if __name__ == "__main__":
    raise SystemExit(main())
