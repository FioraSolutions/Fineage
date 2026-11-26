#!/usr/bin/env python3
"""Identify packers and extract string/table dumps from Interlude PE binaries."""

from __future__ import annotations

import collections
import math
import re
import struct
from pathlib import Path


def entropy(data: bytes) -> float:
    if not data:
        return 0.0
    counts = collections.Counter(data)
    n = len(data)
    return -sum((c / n) * math.log2(c / n) for c in counts.values())


def parse_pe(data: bytes) -> dict | None:
    if data[:2] != b"MZ":
        return None
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    if data[e_lfanew : e_lfanew + 4] != b"PE\0\0":
        return {"error": "MZ but no PE signature", "e_lfanew": e_lfanew}
    coff = e_lfanew + 4
    machine, nsec, ts, _, _, optsz, chars = struct.unpack_from("<HHIIIHH", data, coff)
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    entry = struct.unpack_from("<I", data, opt + 16)[0]
    imagebase = (
        struct.unpack_from("<I", data, opt + 28)[0]
        if magic == 0x10B
        else struct.unpack_from("<Q", data, opt + 24)[0]
    )
    sec_off = opt + optsz
    sections = []
    for i in range(nsec):
        off = sec_off + i * 40
        sname = data[off : off + 8].split(b"\0", 1)[0]
        vsize, va, rsize, raw = struct.unpack_from("<IIII", data, off + 8)
        chunk = data[raw : raw + min(rsize, 2_000_000)] if rsize and raw < len(data) else b""
        sections.append(
            {
                "name": sname.decode("latin-1", errors="replace"),
                "name_bytes": sname.hex(),
                "va": va,
                "vsize": vsize,
                "raw": raw,
                "rsize": rsize,
                "entropy": round(entropy(chunk), 3) if chunk else 0.0,
            }
        )
    return {
        "machine": hex(machine),
        "sections_n": nsec,
        "pe32": magic == 0x10B,
        "entry": entry,
        "imagebase": imagebase,
        "sections": sections,
        "overall_entropy": round(entropy(data[: min(len(data), 2_000_000)]), 3),
    }


def identify_packer(name: str, data: bytes, pe: dict | None) -> dict:
    result = {
        "file": name,
        "size": len(data),
        "packer": "unknown",
        "unpacked": False,
        "notes": [],
    }
    if pe is None:
        result["packer"] = "not-mz"
        result["notes"].append("No MZ header")
        return result
    if "error" in pe:
        result["notes"].append(pe["error"])
        return result
    result["pe"] = {k: pe[k] for k in ("machine", "sections_n", "pe32", "entry", "imagebase", "overall_entropy")}
    result["sections"] = pe["sections"]
    names = [s["name"] for s in pe["sections"]]
    entry = pe["entry"]
    in_themida = False
    for s in pe["sections"]:
        if "themida" in s["name"].lower() or "winlice" in s["name"].lower() or s["name"].lower() == "oreans":
            in_themida = True
        va, ve = s["va"], s["va"] + max(s["vsize"], s["rsize"])
        if va <= entry < ve and "themida" in s["name"].lower():
            in_themida = True
            result["notes"].append(f"entry 0x{entry:x} is inside section {s['name']!r}")
    if in_themida or b"Themida" in data or b"Oreans" in data:
        result["packer"] = "Themida"
        result["unpacked"] = False
        if b"Themida" in data:
            result["notes"].append("Contains literal 'Themida' string")
        return result
    if any(n.lower().startswith("upx") for n in names) or data[:4] == b"UPX!" or b"UPX0" in data[:0x400]:
        result["packer"] = "UPX"
        result["unpacked"] = False
        result["notes"].append("UPX section names present")
        return result
    # Standard PE section set and moderate entropy ⇒ not packed
    standard = all(s["name"] in {".text", ".rdata", ".data", ".idata", ".rsrc", ".reloc", ".bss"} for s in pe["sections"])
    high = any(s["entropy"] >= 7.5 and s["rsize"] > 64_000 for s in pe["sections"])
    if standard and not high:
        result["packer"] = "none"
        result["unpacked"] = True
        result["notes"].append("Standard PE sections, no high-entropy packed stub")
    elif high:
        result["packer"] = "unknown-high-entropy"
        result["unpacked"] = False
        result["notes"].append("High-entropy section without a known stub name")
    else:
        result["packer"] = "none"
        result["unpacked"] = True
        result["notes"].append("No known packer signature")
    return result


def extract_strings(data: bytes, minlen: int = 5) -> dict[str, list[str]]:
    ascii_s = [m.group().decode("ascii") for m in re.finditer(rb"[\x20-\x7e]{%d,}" % minlen, data)]
    utf16: list[str] = []
    i = 0
    n = len(data) - 1
    while i < n:
        if data[i + 1] == 0 and 32 <= data[i] < 127:
            j = i
            chars = []
            while j < n and data[j + 1] == 0 and 32 <= data[j] < 127:
                chars.append(chr(data[j]))
                j += 2
            if len(chars) >= minlen:
                utf16.append("".join(chars))
            i = j
        else:
            i += 1
    # unique preserve order, cap
    def uniq(xs):
        seen = set()
        out = []
        for x in xs:
            if x in seen:
                continue
            seen.add(x)
            out.append(x)
        return out

    return {"ascii": uniq(ascii_s), "utf16": uniq(utf16)}


INTERESTING = re.compile(
    r"(skill|effect|anim|Lineage|emitter|FX_|cast|weapon|NSkill|MagicSkill|class)",
    re.I,
)


def inspect_file(path: Path) -> dict:
    data = path.read_bytes()
    pe = parse_pe(data)
    info = identify_packer(path.name, data, pe)
    strings = extract_strings(data, 6)
    info["string_counts"] = {k: len(v) for k, v in strings.items()}
    interesting = [s for s in strings["ascii"] if INTERESTING.search(s)]
    interesting += [s for s in strings["utf16"] if INTERESTING.search(s)]
    info["interesting_strings"] = interesting[:400]
    info["all_strings"] = strings
    return info


def write_string_tsv(strings: dict[str, list[str]], dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    with dest.open("w", encoding="utf-8") as f:
        f.write("encoding\tstring\n")
        for enc, xs in strings.items():
            for s in xs:
                f.write(f"{enc}\t{s.replace(chr(9), ' ').replace(chr(10), ' ')}\n")
