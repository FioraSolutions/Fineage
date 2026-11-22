#!/usr/bin/env python3
"""UE2 (L2 Interlude) package name/export/import indexer."""

from __future__ import annotations

import struct
from pathlib import Path

U_SIGNATURE = 0x9E2A83C1


class PackageError(Exception):
    pass


def _u32(data: bytes, o: int) -> tuple[int, int]:
    if o + 4 > len(data):
        raise PackageError(f"EOF u32 at {o}")
    return struct.unpack_from("<I", data, o)[0], o + 4


def _i32(data: bytes, o: int) -> tuple[int, int]:
    if o + 4 > len(data):
        raise PackageError(f"EOF i32 at {o}")
    return struct.unpack_from("<i", data, o)[0], o + 4


def _u16(data: bytes, o: int) -> tuple[int, int]:
    if o + 2 > len(data):
        raise PackageError(f"EOF u16 at {o}")
    return struct.unpack_from("<H", data, o)[0], o + 2


def read_index(data: bytes, o: int, version: int) -> tuple[int, int]:
    """Unreal compact index (NAME/export object refs)."""
    if o >= len(data):
        raise PackageError("EOF compact index")
    b = data[o]
    o += 1
    negative = bool(b & 0x80)
    value = b & 0x3F
    if b & 0x40:
        shift = 6
        while True:
            if o >= len(data):
                raise PackageError("EOF compact index cont")
            c = data[o]
            o += 1
            value |= (c & 0x7F) << shift
            if not (c & 0x80):
                break
            shift += 7
            if shift > 35:
                raise PackageError("compact index overflow")
    return (-value if negative else value), o


def _read_name(data: bytes, o: int, version: int) -> tuple[str, int, int]:
    # UE2: compact length or dword length depending on version.
    # L2 Interlude packages are typically version 0x69 (105) / licensee custom.
    # Try: compact index length, latin-1 chars, optional trailing flags (DWORD).
    if version >= 64:
        # length-prefixed ANSI: compact index includes trailing NUL
        length, o = read_index(data, o, version)
        if length < 0:
            # unicode
            n = -length
            raw = data[o : o + n * 2]
            o += n * 2
            s = raw.decode("utf-16le", errors="replace").rstrip("\x00")
        else:
            raw = data[o : o + length]
            o += length
            s = raw.split(b"\x00", 1)[0].decode("latin-1", errors="replace")
        flags, o = _u32(data, o)
        return s, flags, o
    # very old: 64-byte padded
    raw = data[o : o + 64]
    o += 64
    s = raw.split(b"\x00", 1)[0].decode("latin-1", errors="replace")
    flags, o = _u32(data, o)
    return s, flags, o


def index_package(data: bytes, filename: str = "") -> dict:
    if len(data) < 16 or struct.unpack_from("<I", data, 0)[0] != U_SIGNATURE:
        raise PackageError(f"{filename}: not a UPackage")
    version, o = _u16(data, 4)
    licensee, o = _u16(data, 6)
    # Interlude Engine.u is package version 123? we'll detect.
    # Standard UE2 (v68+): DWORD flags, then counts/offsets.
    flags, o = _u32(data, o)
    name_count, o = _u32(data, o)
    name_offset, o = _u32(data, o)
    export_count, o = _u32(data, o)
    export_offset, o = _u32(data, o)
    import_count, o = _u32(data, o)
    import_offset, o = _u32(data, o)
    names: list[str] = []
    oo = name_offset
    for i in range(name_count):
        s, _flags, oo = _read_name(data, oo, version)
        names.append(s)

    def name_at(idx: int) -> str:
        if 0 <= idx < len(names):
            return names[idx]
        return f"#NAME_{idx}"

    def obj_ref_name(ref: int, exports: list[dict], imports: list[dict]) -> str:
        if ref > 0:
            i = ref - 1
            if 0 <= i < len(exports):
                return exports[i]["object"]
            return f"#EXP_{ref}"
        if ref < 0:
            i = -ref - 1
            if 0 <= i < len(imports):
                return imports[i]["object"]
            return f"#IMP_{ref}"
        # UE2: class index 0 means the export itself is a UClass.
        return "Class"

    # Imports first (needed to resolve export classes). Layout (UE2):
    # compact class package, compact class name, compact package, compact object name
    imports: list[dict] = []
    oo = import_offset
    for i in range(import_count):
        class_pkg, oo = read_index(data, oo, version)
        class_name, oo = read_index(data, oo, version)
        pkg, oo = _i32(data, oo)
        obj_name, oo = read_index(data, oo, version)
        imports.append(
            {
                "object": name_at(obj_name),
                "class": name_at(class_name),
                "class_package": name_at(class_pkg),
                "package_ref": pkg,
            }
        )

    exports: list[dict] = []
    oo = export_offset
    for i in range(export_count):
        class_ref, oo = read_index(data, oo, version)
        super_ref, oo = read_index(data, oo, version)
        package_ref, oo = _i32(data, oo)
        obj_name, oo = read_index(data, oo, version)
        flags, oo = _u32(data, oo)
        serial_size, oo = read_index(data, oo, version)
        serial_offset = 0
        if serial_size > 0:
            serial_offset, oo = read_index(data, oo, version)
        # some versions have extra fields (GUID, export flags) — leave unread if
        # the next export still lines up; we only need the header we already have.
        exports.append(
            {
                "object": name_at(obj_name),
                "class_ref": class_ref,
                "super_ref": super_ref,
                "package_ref": package_ref,
                "flags": flags,
                "serial_size": serial_size,
                "serial_offset": serial_offset,
            }
        )

    # Resolve class names after both tables exist.
    for exp in exports:
        exp["class"] = obj_ref_name(exp["class_ref"], exports, imports)
        exp["super"] = obj_ref_name(exp["super_ref"], exports, imports)
        del exp["class_ref"]
        del exp["super_ref"]

    return {
        "file": filename,
        "version": version,
        "licensee": licensee,
        "flags": flags,
        "name_count": name_count,
        "export_count": export_count,
        "import_count": import_count,
        "names": names,
        "exports": exports,
        "imports": imports,
    }
