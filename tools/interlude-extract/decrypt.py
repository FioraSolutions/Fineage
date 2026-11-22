#!/usr/bin/env python3
"""Decrypt Interlude DAT (via l2encdec) and XOR-packed Unreal packages (ver 111/121)."""

from __future__ import annotations

import struct
import subprocess
from pathlib import Path

HEADER_SIZE = 28  # UTF-16LE "Lineage2VerXXX"
VER_111_KEY = 0xAC
U_SIGNATURE = 0x9E2A83C1


def read_l2_header(data: bytes) -> str | None:
    if len(data) < HEADER_SIZE:
        return None
    try:
        header = data[:HEADER_SIZE].decode("utf-16le")
    except UnicodeDecodeError:
        return None
    if header.startswith("Lineage2Ver") and header[11:14].isdigit():
        return header
    return None


def protocol_of(data: bytes) -> int | None:
    h = read_l2_header(data)
    if not h:
        return None
    return int(h[11:14])


def derive_xor_key(filename: str) -> int:
    return sum(ord(c) for c in filename.lower()) & 0xFF


def decrypt_xor_package(data: bytes, filename: str) -> bytes:
    """Return payload without the 28-byte header, XOR-decrypted."""
    ver = protocol_of(data)
    if ver is None:
        # already a UPackage?
        return data
    key = VER_111_KEY if ver == 111 else derive_xor_key(filename)
    payload = bytearray(data[HEADER_SIZE:])
    for i in range(len(payload)):
        payload[i] ^= key
    return bytes(payload)


def is_upackage(data: bytes) -> bool:
    return len(data) >= 4 and struct.unpack_from("<I", data, 0)[0] == U_SIGNATURE


def find_l2encdec(repo_root: Path | None = None) -> Path:
    candidates: list[Path] = []
    if repo_root:
        candidates.extend(repo_root.glob("tools/interlude-dat/l2encdec-bin/*/l2encdec"))
    home = Path.home()
    candidates.extend(
        [
            home / "Projects/Fineage/tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec",
            home / "Projects/Fineage-wt/ref-catalog/tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec",
        ]
    )
    for p in candidates:
        if p.is_file():
            return p
    raise FileNotFoundError(
        "l2encdec not found. Place the Darwin binary under "
        "tools/interlude-dat/l2encdec-bin/<build>/l2encdec"
    )


def decrypt_dat_file(src: Path, dest: Path, l2encdec: Path, protocol: int = 413) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    cmd = [
        str(l2encdec),
        "-c",
        "decode",
        "-p",
        str(protocol),
        "-l",
        "-o",
        str(dest),
        str(src),
    ]
    proc = subprocess.run(cmd, capture_output=True, text=True)
    if proc.returncode != 0 or not dest.is_file():
        raise RuntimeError(
            f"l2encdec failed for {src.name}: rc={proc.returncode} "
            f"stdout={proc.stdout!r} stderr={proc.stderr!r}"
        )


def decrypt_package_file(src: Path, dest: Path) -> dict:
    data = src.read_bytes()
    header = read_l2_header(data)
    dest.parent.mkdir(parents=True, exist_ok=True)
    if header is None:
        dest.write_bytes(data)
        return {
            "file": src.name,
            "protocol": None,
            "already_plain": True,
            "upackage": is_upackage(data),
            "size": dest.stat().st_size,
        }
    proto = int(header[11:14])
    out = decrypt_xor_package(data, src.name)
    dest.write_bytes(out)
    return {
        "file": src.name,
        "protocol": proto,
        "already_plain": False,
        "upackage": is_upackage(out),
        "size": len(out),
    }
