"""Minimal binary-FBX reader for import-contract measurements (no Blender, no Unity).

Usage: python3 tools/interlude/fbx_units.py <file.fbx> [...] [--json]

Reports, per file:
  * GlobalSettings UnitScaleFactor / OriginalUnitScaleFactor
  * every Model node, its type, and its Lcl Scaling / Lcl Translation
  * node names of interest (bip01, Bip01_Pelvis, Weapon_R_Bone) with local translations

The FBX binary layout is: 27-byte header ("Kaydara FBX Binary  \\x00" + u32 version),
then a sequence of node records. Each record is
  u32 endOffset, u32 numProperties, u32 propertyListLen, u8 nameLen, name,
  properties..., nested records..., and a null terminator whose size depends on the
  version (13 bytes below 7500, 25 bytes from 7500).
"""
from __future__ import annotations

import json
import struct
import sys

MAGIC = b"Kaydara FBX Binary  \x00\x1a\x00"

INTEREST = {"bip01", "Bip01_Pelvis", "Weapon_R_Bone", "Bip01", "Root"}


class Node:
    __slots__ = ("name", "props", "children")

    def __init__(self, name, props, children):
        self.name = name
        self.props = props
        self.children = children

    def find(self, name):
        return [c for c in self.children if c.name == name]

    def first(self, name):
        found = self.find(name)
        return found[0] if found else None


def _array(code, data, offset, length):
    count, encoding, compressed_len = struct.unpack_from("<III", data, offset)
    offset += 12
    raw = data[offset:offset + compressed_len]
    offset += compressed_len
    import zlib
    if encoding == 1:
        raw = zlib.decompress(raw)
    fmt = {"f": "<f", "d": "<d", "i": "<i", "l": "<q", "b": "<?"}[code]
    size = struct.calcsize(fmt)
    values = list(struct.unpack_from(f"<{len(raw) // size}{code}", raw))
    del count, length
    return values, offset


def _read_properties(data, offset, count):
    props = []
    for _ in range(count):
        code = chr(data[offset])
        offset += 1
        if code == "Y":
            props.append(struct.unpack_from("<h", data, offset)[0]); offset += 2
        elif code == "C":
            props.append(bool(data[offset])); offset += 1
        elif code == "I":
            props.append(struct.unpack_from("<i", data, offset)[0]); offset += 4
        elif code == "F":
            props.append(struct.unpack_from("<f", data, offset)[0]); offset += 4
        elif code == "D":
            props.append(struct.unpack_from("<d", data, offset)[0]); offset += 8
        elif code == "L":
            props.append(struct.unpack_from("<q", data, offset)[0]); offset += 8
        elif code in "fdilb":
            values, offset = _array(code, data, offset, None)
            props.append(values)
        elif code in "SR":
            length = struct.unpack_from("<I", data, offset)[0]
            offset += 4
            chunk = data[offset:offset + length]
            offset += length
            props.append(chunk.decode("utf-8", "replace") if code == "S" else chunk)
        else:
            raise ValueError(f"unknown property code {code!r} at {offset}")
    return props, offset


def _read_node(data, offset, wide):
    if offset + 13 > len(data):
        return None, offset
    end, num_props, _plen, name_len = struct.unpack_from("<IIIB", data, offset)
    offset += 13
    if end == 0:
        return None, offset
    raw_name = data[offset:offset + name_len]
    offset += name_len
    if raw_name.startswith(b"\x12") and raw_name.endswith(b"\x00\x01"):
        # FBX 7.4+ "PRE" compression marker: 0x12, 16-byte id, 0x00 0x01.
        name = raw_name[17:-2].decode("ascii", "replace")
        num_props = max(0, num_props - 1)
    else:
        name = raw_name.decode("ascii", "replace")
    props, offset = _read_properties(data, offset, num_props)
    children = []
    while offset < end:
        child, offset = _read_node(data, offset, wide)
        if child is None:
            break
        children.append(child)
    terminator = 25 if wide else 13
    offset = end
    return Node(name, props, children), offset


def parse(path):
    data = open(path, "rb").read()
    if not data.startswith(MAGIC):
        raise SystemExit(f"{path}: not a binary FBX")
    version = struct.unpack_from("<I", data, len(MAGIC))[0]
    wide = version >= 7500
    offset = len(MAGIC) + 4
    nodes = []
    while True:
        node, offset = _read_node(data, offset, wide)
        if node is None:
            break
        nodes.append(node)
    return version, nodes


def _p70(model, key):
    props = model.first("Properties70")
    if not props:
        return None
    for p in props.children:
        if p.name == "P" and p.props and p.props[0] == key:
            return p.props[4:] if len(p.props) > 4 else None
    return None


def report(path):
    version, nodes = parse(path)
    root = Node("", [], nodes)
    out = {"file": path, "version": version, "unit": {}, "models": [], "interest": {}}

    globals_ = root.first("GlobalSettings")
    if globals_:
        props = globals_.first("Properties70")
        if props:
            for p in props.children:
                if p.name == "P" and p.props and p.props[0] in ("UnitScaleFactor", "OriginalUnitScaleFactor"):
                    out["unit"][p.props[0]] = p.props[4] if len(p.props) > 4 else None

    objects = root.first("Objects")
    for model in (objects.children if objects else []):
        if model.name != "Model" or len(model.props) < 3:
            continue
        name = model.props[1].split("\x00")[0]
        kind = model.props[2]
        entry = {"name": name, "kind": kind}
        scale = _p70(model, "Lcl Scaling")
        if scale:
            entry["lclScaling"] = [round(float(v), 6) for v in scale[:3]]
        out["models"].append(entry)
        if name in INTEREST or name.endswith(".ao") or name.endswith(".mo"):
            trans = _p70(model, "Lcl Translation")
            out["interest"][name] = {
                "lclScaling": entry.get("lclScaling"),
                "lclTranslation": [round(float(v), 6) for v in trans[:3]] if trans else None,
            }
    return out


def main():
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    as_json = "--json" in sys.argv
    reports = [report(a) for a in args]
    if as_json:
        print(json.dumps(reports, indent=1))
        return
    for r in reports:
        print(f"=== {r['file']}  (FBX {r['version']})")
        print(f"  unit {r['unit']}")
        ao = [m for m in r["models"] if m["name"].endswith(".ao") or m["name"].endswith(".mo")]
        for m in ao[:3]:
            print(f"  model {m['name']} ({m['kind']}) lclScaling={m.get('lclScaling')}")
        for name, info in list(r["interest"].items())[:6]:
            if info in ao:
                continue
            print(f"  {name}: lclScaling={info['lclScaling']} lclTranslation={info['lclTranslation']}")
        print(f"  models={len(r['models'])}")


if __name__ == "__main__":
    main()
