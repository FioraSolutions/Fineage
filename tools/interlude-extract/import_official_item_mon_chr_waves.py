#!/usr/bin/env python3
"""Copy official item/monster/character waves into Resources and write official-waves.tsv.

Does not copy original .uax packages. Existing Resources clips are left in place.
Same group+name from a later official package replaces an earlier one only when
the destination was also created by this import (not a pre-existing clip).
"""
from __future__ import annotations

import csv
import shutil
import sys
import uuid
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
ASSETS = ROOT / "l2-unity" / "Assets"
SOUNDS_RES = ASSETS / "Resources" / "Data" / "Sounds"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "sounds"
STREAM = ASSETS / "StreamingAssets" / "Data" / "Meta" / "Sounds"
CATALOG = DOCS / "sound-catalog.tsv"

ITEM_PACKS = ("itemsound", "ItemSound2", "ItemSound3", "EtcSound")
MON_PACKS = ("monsound", "monsound2", "monsound3", "monsound4", "monsound5", "MonSound6", "monsound7")
CHR_PACKS = ("chrsound", "chrsound2")
SCOPE = set(ITEM_PACKS + MON_PACKS + CHR_PACKS)

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

DEFAULT_META = """fileFormatVersion: 2
guid: {guid}
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def resources_path(rel_under_sounds: Path) -> str:
    # Data/Sounds/... without extension
    return "Data/Sounds/" + rel_under_sounds.with_suffix("").as_posix()


def ensure_folder_meta(dirpath: Path, created_dirs: set[Path]) -> None:
    if not dirpath.is_dir():
        dirpath.mkdir(parents=True, exist_ok=True)
    meta = dirpath.with_name(dirpath.name + ".meta")
    if not meta.exists():
        meta.write_text(FOLDER_META.format(guid=uuid.uuid4().hex), encoding="utf-8")
        created_dirs.add(dirpath)


def existing_clip_index() -> dict[str, Path]:
    """stem.lower() -> path, first wins; also record relative resources stems."""
    idx: dict[str, Path] = {}
    for p in SOUNDS_RES.rglob("*"):
        if p.suffix.lower() in {".wav", ".ogg", ".mp3"} and p.is_file():
            idx.setdefault(p.stem.lower(), p)
    return idx


def find_extracted(extracted_root: Path, package: str, group: str, name: str) -> Path | None:
    pkg_dir = extracted_root / package
    # case-accurate package dir first, then case-insensitive
    if not pkg_dir.is_dir():
        matches = [p for p in extracted_root.iterdir() if p.is_dir() and p.name.lower() == package.lower()]
        pkg_dir = matches[0] if matches else pkg_dir
    candidates = []
    if group:
        candidates.append(pkg_dir / group / f"{name}.wav")
    candidates.append(pkg_dir / f"{name}.wav")
    for c in candidates:
        if c.is_file():
            return c
    # case-insensitive walk within package (rare typo groups)
    if pkg_dir.is_dir():
        low = name.lower()
        group_low = (group or "").lower()
        for p in pkg_dir.rglob("*.wav"):
            if p.stem.lower() != low:
                continue
            if not group_low or p.parent.name.lower() == group_low or p.parent == pkg_dir:
                return p
    return None


def existing_resources_for(name: str, kind: str, group: str, clip_idx: dict[str, Path]) -> Path | None:
    """Prefer already-imported official layout; do not move existing clips."""
    if kind == "item":
        p = SOUNDS_RES / "ItemSound" / f"{name}.wav"
        if p.is_file():
            return p
        return None
    if kind == "mon":
        p = SOUNDS_RES / "MonSound" / group / f"{name}.wav"
        if p.is_file():
            return p
        # already present under this group with different case?
        folder = SOUNDS_RES / "MonSound" / group
        if folder.is_dir():
            for q in folder.iterdir():
                if q.is_file() and q.suffix.lower() == ".wav" and q.stem.lower() == name.lower():
                    return q
        return None
    if kind == "chr":
        # Keep clips that already live under SkillVoice (100 official spell voices).
        sv = SOUNDS_RES / "SkillVoice"
        for q in sv.glob("*"):
            if q.is_file() and q.suffix.lower() in {".wav", ".ogg"} and q.stem.lower() == name.lower():
                return q
        voice = SOUNDS_RES / "Voice" / f"{name}.wav"
        if voice.is_file():
            return voice
        return None
    return None


def dest_for_new(kind: str, group: str, name: str, src: Path) -> Path:
    ext = src.suffix
    if kind == "item":
        return SOUNDS_RES / "ItemSound" / f"{name}{ext}"
    if kind == "mon":
        return SOUNDS_RES / "MonSound" / group / f"{name}{ext}"
    return SOUNDS_RES / "Voice" / f"{name}{ext}"


def write_audio_meta(audio: Path) -> None:
    meta = Path(str(audio) + ".meta")
    if meta.exists():
        return
    meta.write_text(AUDIO_META.format(guid=uuid.uuid4().hex), encoding="utf-8")


def kind_of(package: str) -> str:
    if package in ITEM_PACKS:
        return "item"
    if package in MON_PACKS:
        return "mon"
    return "chr"


def later_package(a: str, b: str) -> str:
    order = {p.lower(): i for i, p in enumerate(ITEM_PACKS + MON_PACKS + CHR_PACKS)}
    return b if order.get(b.lower(), -1) >= order.get(a.lower(), -1) else a


def main() -> int:
    if len(sys.argv) < 2:
        print("Usage: import_official_item_mon_chr_waves.py <extracted-grouped-root>", file=sys.stderr)
        return 2
    extracted = Path(sys.argv[1]).resolve()
    if not extracted.is_dir():
        print(f"missing extract root: {extracted}", file=sys.stderr)
        return 1

    rows = []
    with CATALOG.open(encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")
        fieldnames = reader.fieldnames
        for rec in reader:
            rows.append(rec)

    clip_idx = existing_clip_index()
    table = []  # official-waves rows
    # dest_key -> (package, src, dest, kind, rec)
    planned: dict[str, tuple] = {}
    missing_src = []
    skipped_existing = 0
    copied = 0
    replaced = 0
    created_dirs: set[Path] = set()
    logs = []

    for rec in rows:
        package = rec["package"]
        if package not in SCOPE:
            continue
        name = rec["name"]
        group = rec.get("group") or ""
        kind = kind_of(package)
        src = find_extracted(extracted, package, group, name)
        if src is None:
            missing_src.append((package, group, name))
            continue
        existing = existing_resources_for(name, kind, group, clip_idx)
        if existing is not None:
            dest = existing
            skipped_existing += 1
        else:
            dest = dest_for_new(kind, group, name, src)
            key = str(dest.relative_to(SOUNDS_RES)).lower()
            if key in planned:
                prev_pkg, prev_src, prev_dest, prev_kind, prev_rec = planned[key]
                winner = later_package(prev_pkg, package)
                if winner == package:
                    if prev_src.read_bytes() != src.read_bytes():
                        logs.append(
                            f"later package {package} replaces {prev_pkg} for {key} "
                            f"({prev_src.stat().st_size} vs {src.stat().st_size} bytes)"
                        )
                    planned[key] = (package, src, dest, kind, rec)
                    replaced += 1
                else:
                    if prev_src.read_bytes() != src.read_bytes():
                        logs.append(
                            f"kept {prev_pkg} over {package} for {key} "
                            f"({prev_src.stat().st_size} vs {src.stat().st_size} bytes)"
                        )
                continue
            planned[key] = (package, src, dest, kind, rec)

        rel = dest.relative_to(SOUNDS_RES)
        table.append(
            {
                "package": package,
                "group": group,
                "name": name,
                "resources_path": resources_path(rel),
            }
        )

    ensure_folder_meta(SOUNDS_RES / "ItemSound", created_dirs)
    ensure_folder_meta(SOUNDS_RES / "MonSound", created_dirs)
    ensure_folder_meta(SOUNDS_RES / "Voice", created_dirs)
    ensure_folder_meta(STREAM, created_dirs)

    for package, src, dest, kind, rec in planned.values():
        dest.parent.mkdir(parents=True, exist_ok=True)
        if kind == "mon":
            ensure_folder_meta(dest.parent, created_dirs)
        if dest.exists():
            # should be a planned new file; don't clobber pre-existing (those were skipped)
            continue
        shutil.copy2(src, dest)
        write_audio_meta(dest)
        copied += 1

    # official-waves.tsv includes both existing and newly copied
    STREAM.mkdir(parents=True, exist_ok=True)
    tsv = STREAM / "official-waves.tsv"
    with tsv.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=["package", "group", "name", "resources_path"], delimiter="\t", lineterminator="\n")
        w.writeheader()
        for row in sorted(table, key=lambda r: (r["package"].lower(), r["group"].lower(), r["name"].lower())):
            w.writerow(row)
    tsv_meta = Path(str(tsv) + ".meta")
    if not tsv_meta.exists():
        tsv_meta.write_text(DEFAULT_META.format(guid=uuid.uuid4().hex), encoding="utf-8")

    # docs copy
    docs_tsv = DOCS / "official-waves.tsv"
    shutil.copy2(tsv, docs_tsv)

    # refresh catalog fineage for imported names (resources_clip if dest exists)
    present = set()
    for row in table:
        present.add((row["package"].lower(), row["name"].lower(), (row["group"] or "").lower()))
    updated = 0
    for rec in rows:
        if rec["package"] not in SCOPE:
            continue
        key = (rec["package"].lower(), rec["name"].lower(), (rec.get("group") or "").lower())
        if key in present and rec.get("fineage") != "resources_clip":
            rec["fineage"] = "resources_clip"
            updated += 1
    with CATALOG.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=fieldnames, delimiter="\t", lineterminator="\n")
        w.writeheader()
        for rec in rows:
            w.writerow(rec)

    print(f"copied={copied} kept_existing={skipped_existing} later_pkg_replaced={replaced} table={len(table)}")
    print(f"catalog_updated={updated} missing_extract={len(missing_src)}")
    if missing_src:
        print("MISSING EXTRACT:")
        for row in missing_src[:30]:
            print(" ", row)
    for line in logs:
        print("NOTE:", line)
    return 0 if not missing_src else 1


if __name__ == "__main__":
    raise SystemExit(main())
