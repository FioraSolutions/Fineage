#!/usr/bin/env python3
"""Zip a Unity player directory with CHANGELOG.md and version.json at the root."""

from __future__ import annotations

import argparse
import os
import stat
import sys
import tempfile
import time
import zipfile
from pathlib import Path

PLAYER_NAMES = ("Fineage.exe", "Fineage.x86_64", "Fineage.app")
NOTES = ("CHANGELOG.md", "version.json")


def find_player_dir(build_dir: Path) -> Path:
    for name in PLAYER_NAMES:
        matches = sorted(build_dir.rglob(name))
        if matches:
            return matches[0].parent
    top = ", ".join(child.name for child in sorted(build_dir.iterdir())) if build_dir.is_dir() else "(missing)"
    raise SystemExit(f"No Fineage player under {build_dir}. Top entries: {top}")


def zip_tree(source: Path, output: Path) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    if output.exists():
        output.unlink()

    with zipfile.ZipFile(
        output,
        "w",
        compression=zipfile.ZIP_DEFLATED,
        compresslevel=3,
        allowZip64=True,
    ) as archive:
        paths = sorted(source.rglob("*"), key=lambda item: item.as_posix())
        for path in paths:
            arcname = path.relative_to(source).as_posix()
            if path.is_symlink():
                info = zipfile.ZipInfo(arcname)
                info.create_system = 3
                info.external_attr = 0o120777 << 16
                info.date_time = time.localtime(path.lstat().st_mtime)[:6]
                archive.writestr(info, os.readlink(path))
                continue
            if path.is_dir():
                info = zipfile.ZipInfo(arcname + "/")
                info.create_system = 3
                info.external_attr = (0o755 << 16) | 0x10
                archive.writestr(info, b"")
                continue
            info = zipfile.ZipInfo.from_file(path, arcname)
            mode = stat.S_IMODE(path.stat().st_mode)
            info.create_system = 3
            info.external_attr = (mode & 0xFFFF) << 16
            # ZipInfo.from_file defaults to ZIP_STORED; archive.open(info) honours the
            # entry's compress_type, not the archive default. Without this the Windows
            # zip exceeded GitHub's 2 GiB release-asset limit.
            info.compress_type = zipfile.ZIP_DEFLATED
            with archive.open(info, "w") as dest, path.open("rb") as src:
                while True:
                    chunk = src.read(1024 * 1024)
                    if not chunk:
                        break
                    dest.write(chunk)


def pack(build_dir: Path, notes_dir: Path, output: Path) -> Path:
    player_dir = find_player_dir(build_dir)
    for name in NOTES:
        if not (notes_dir / name).is_file():
            raise SystemExit(f"missing {notes_dir / name}")

    with tempfile.TemporaryDirectory() as tmp:
        stage = Path(tmp) / "stage"
        copy_tree(player_dir, stage)
        for name in NOTES:
            (stage / name).write_bytes((notes_dir / name).read_bytes())
        zip_tree(stage, output)

    size = output.stat().st_size
    print(f"packed {output} ({size} bytes) from {player_dir}")
    if size > 2 * 1024 * 1024 * 1024:
        print(
            "warning: zip is over 2 GiB. GitHub Release assets reject files that large; "
            "the workflow artifact is still uploaded.",
            file=sys.stderr,
        )
    return output


def copy_tree(source: Path, dest: Path) -> None:
    dest.mkdir(parents=True, exist_ok=True)
    for path in sorted(source.rglob("*"), key=lambda item: item.as_posix()):
        target = dest / path.relative_to(source)
        if path.is_symlink():
            target.symlink_to(os.readlink(path))
        elif path.is_dir():
            target.mkdir(parents=True, exist_ok=True)
        else:
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(path.read_bytes())
            target.chmod(path.stat().st_mode & 0o777)


def self_test() -> int:
    with tempfile.TemporaryDirectory() as tmp:
        root = Path(tmp)
        player = root / "build" / "Fineage"
        data = player / "Fineage_Data"
        data.mkdir(parents=True)
        exe = player / "Fineage.x86_64"
        exe.write_text("player\n", encoding="utf-8")
        exe.chmod(0o755)
        (data / "dummy.txt").write_text("data\n", encoding="utf-8")
        (data / "link.txt").symlink_to("dummy.txt")
        notes = root / "notes"
        notes.mkdir()
        (notes / "CHANGELOG.md").write_text("# Fineage test\n", encoding="utf-8")
        (notes / "version.json").write_text('{"version":"0.0.0-test"}\n', encoding="utf-8")
        output = root / "dist" / "Fineage-Linux-x64.zip"
        pack(root / "build", notes, output)

        with zipfile.ZipFile(output) as archive:
            names = set(archive.namelist())
            assert "Fineage.x86_64" in names
            assert "Fineage_Data/dummy.txt" in names
            assert "CHANGELOG.md" in names
            assert "version.json" in names
            assert "Fineage_Data/link.txt" in names
            info = archive.getinfo("Fineage_Data/link.txt")
            assert stat.S_ISLNK(info.external_attr >> 16)
            assert archive.read("CHANGELOG.md").startswith(b"# Fineage")

    print("pack-client self-test OK")
    return 0


def main(argv: list[str]) -> int:
    if "--self-test" in argv:
        return self_test()

    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--build", type=Path, required=True)
    parser.add_argument("--notes", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args(argv)
    pack(args.build, args.notes, args.output)
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
