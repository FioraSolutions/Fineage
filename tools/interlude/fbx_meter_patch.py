"""Rewrite a shipping character FBX onto the meter contract (Docs/METER_SCALE_MIGRATION.md §3)
without a Blender round-trip.

Why numeric instead of Blender: Blender rebuilds the rig from the FBX BindPose, but Unity
builds the hierarchy from `Lcl Translation`. In `MFighter_anim.fbx` those two disagree by
4% on `bip01`, so a Blender re-bake silently moves the pelvis. This tool edits the units
in place and preserves bone names, the hierarchy, skin clusters, UVs, normals and
materials byte-for-byte.

Contract transformation (legacy -> meter), all values stay the same size:

  UnitScaleFactor         1   -> 100      (Unity: useFileScale=true -> import scale 1)
  Model Lcl Scaling       100 -> 1        (.ao / .mo / root)
  Model Lcl Translation   x1.9            (100 * 0.019, the legacy world factor)
  Geometry Vertices       x1.9
  BindPose Matrix xyz     x1.9

After it, Unity with `globalScale = 1`, `useFileScale = true` reproduces exactly the same
world-space bones, vertices and renderer bounds as the legacy import, but every bone
reports `lossyScale = 1`.

Usage:
  python3 tools/interlude/fbx_meter_patch.py <src.fbx> <dst.fbx> [--json]
  python3 tools/interlude/fbx_meter_patch.py --batch <src_dir> <dst_dir> [--json]
"""
from __future__ import annotations

import json
import os
import struct
import sys
import zlib

MAGIC = b"Kaydara FBX Binary  \x00\x1a\x00"
LEGACY_FACTOR = 1.9  # 100 (node) * 0.019 (importer)
NODE_SCALE = 100.0   # legacy Lcl Scaling on .ao/.mo, folded into every global matrix
METER_UNIT = 100.0

ARRAY_CODES = set("fdilb")
SCALAR_CODES = set("YCIFDLSR")


class Prop:
    """One FBX property. Arrays keep their encoding so re-serialisation stays valid."""

    __slots__ = ("code", "value", "encoding")

    def __init__(self, code, value, encoding=0):
        self.code = code
        self.value = value
        self.encoding = encoding

    def __repr__(self):
        return f"Prop({self.code!r}, {self.value!r})"


class Node:
    __slots__ = ("name", "props", "children", "wide")

    def __init__(self, name, props=None, children=None, wide=False):
        self.name = name
        self.props = props if props is not None else []
        self.children = children if children is not None else []
        self.wide = wide

    def find(self, name):
        return [c for c in self.children if c.name == name]

    def first(self, name):
        found = self.find(name)
        return found[0] if found else None

    def walk(self):
        yield self
        for child in self.children:
            yield from child.walk()


# --------------------------------------------------------------------------- reading
def _read_properties(data, offset, count):
    props = []
    for _ in range(count):
        code = chr(data[offset])
        offset += 1
        if code == "Y":
            props.append(Prop(code, struct.unpack_from("<h", data, offset)[0])); offset += 2
        elif code == "C":
            props.append(Prop(code, bool(data[offset]))); offset += 1
        elif code == "I":
            props.append(Prop(code, struct.unpack_from("<i", data, offset)[0])); offset += 4
        elif code == "F":
            props.append(Prop(code, struct.unpack_from("<f", data, offset)[0])); offset += 4
        elif code == "D":
            props.append(Prop(code, struct.unpack_from("<d", data, offset)[0])); offset += 8
        elif code == "L":
            props.append(Prop(code, struct.unpack_from("<q", data, offset)[0])); offset += 8
        elif code in ARRAY_CODES:
            count_elems, encoding, compressed_len = struct.unpack_from("<III", data, offset)
            offset += 12
            raw = data[offset:offset + compressed_len]
            offset += compressed_len
            if encoding == 1:
                raw = zlib.decompress(raw)
            fmt = {"f": "f", "d": "d", "i": "i", "l": "q", "b": "b"}[code]
            size = struct.calcsize("<" + fmt)
            values = list(struct.unpack_from(f"<{count_elems}{fmt}", raw))
            props.append(Prop(code, values, encoding))
        elif code in ("S", "R"):
            length = struct.unpack_from("<I", data, offset)[0]
            offset += 4
            chunk = data[offset:offset + length]
            offset += length
            props.append(Prop(code, chunk.decode("utf-8", "replace") if code == "S" else chunk))
        else:
            raise ValueError(f"unknown property code {code!r} at {offset}")
    return props, offset


def _read_node(data, offset, wide):
    if offset + 13 > len(data):
        return None, offset
    end, num_props, _plen, name_len = struct.unpack_from("<IIIB", data, offset)
    if end == 0:
        return None, offset
    offset += 13
    raw_name = data[offset:offset + name_len]
    offset += name_len
    if raw_name.startswith(b"\x12") and raw_name.endswith(b"\x00\x01"):
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
    return Node(name, props, children, wide), end


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
    # The footer after the last top-level node is copied verbatim.
    return version, nodes, data, offset


# --------------------------------------------------------------------------- writing
def _write_properties(buf, props):
    start = len(buf)
    for prop in props:
        code = prop.code
        buf += code.encode("ascii")
        if code == "Y":
            buf += struct.pack("<h", prop.value)
        elif code == "C":
            buf += struct.pack("<B", 1 if prop.value else 0)
        elif code == "I":
            buf += struct.pack("<i", prop.value)
        elif code == "F":
            buf += struct.pack("<f", prop.value)
        elif code == "D":
            buf += struct.pack("<d", prop.value)
        elif code == "L":
            buf += struct.pack("<q", prop.value)
        elif code in ARRAY_CODES:
            fmt = {"f": "f", "d": "d", "i": "i", "l": "q", "b": "b"}[code]
            raw = struct.pack(f"<{len(prop.value)}{fmt}", *prop.value)
            if prop.encoding == 1:
                packed = zlib.compress(raw)
                buf += struct.pack("<III", len(prop.value), 1, len(packed))
                buf += packed
            else:
                buf += struct.pack("<III", len(prop.value), 0, len(raw))
                buf += raw
        elif code in ("S", "R"):
            chunk = prop.value.encode("utf-8") if code == "S" else prop.value
            buf += struct.pack("<I", len(chunk))
            buf += chunk
        else:
            raise ValueError(f"cannot write property code {code!r}")
    return len(buf) - start


def _write_node(buf, node):
    start = len(buf)
    # Record header (13 bytes on FBX 7400): endOffset, numProperties, propertyListLen, nameLen.
    # endOffset and propertyListLen are patched once their values are known.
    buf += struct.pack("<IIIB", 0, len(node.props), 0, len(node.name))
    buf += node.name.encode("ascii")
    prop_len = _write_properties(buf, node.props)
    buf[start + 8:start + 12] = struct.pack("<I", prop_len)
    for child in node.children:
        _write_node(buf, child)
    buf += b"\x00" * (25 if node.wide else 13)
    buf[start:start + 4] = struct.pack("<I", len(buf))


def serialize(version, nodes, footer):
    buf = bytearray()
    buf += MAGIC
    buf += struct.pack("<I", version)
    for node in nodes:
        _write_node(buf, node)
    buf += footer
    return bytes(buf)


# --------------------------------------------------------------------------- transform
def _p70(node, key):
    props = node.first("Properties70")
    if not props:
        return None
    for p in props.children:
        if p.name == "P" and p.props and p.props[0].value == key:
            return p
    return None


def _scale_translation(node, factor):
    """Scale the xyz of a Properties70 `P` record (props 4..6)."""
    for i in (4, 5, 6):
        if i < len(node.props) and node.props[i].code in SCALAR_CODES:
            node.props[i].value = float(node.props[i].value) * factor


def _matrix_array(node):
    """Return the float array holding a 4x4 matrix, wherever the node keeps it."""
    if node.props and node.props[-1].code in ARRAY_CODES and len(node.props[-1].value) == 16:
        return node.props[-1].value
    child = node.first("Matrix") or node.first("Transform") or node.first("TransformLink")
    if child and child.props and child.props[0].code in ARRAY_CODES:
        return child.props[0].value
    return None


def _retarget_global_matrix(values, linear_div, translation_mul):
    """Scale an absolute (bind) matrix from legacy raw space to meter raw space.

    Legacy raw space has the x100 node scale inside every global matrix, so the linear
    part divides by that scale while the translation keeps the world offset, which grows
    by the legacy 1.9 factor relative to the raw chain.
    """
    for i in range(12):
        if i % 4 != 3:
            values[i] = values[i] * (1.0 / linear_div)
    for i in (12, 13, 14):
        values[i] = values[i] * translation_mul


def detect_contract(root):
    """Read the asset's current unit contract before patching.

    Returns (legacy, reason):
      legacy=True  -> node Lcl Scaling is still 100 and/or UnitScaleFactor is 1, so the
                      asset is in the old contract and must be multiplied by 1.9.
      legacy=False -> already on the meter contract; patching again would grow it by
                      another 1.9x, so the caller must skip it.

    The decision is per file, never per folder: the shared weapon folder holds 382
    legacy assets next to 8 that are already meter.
    """
    unit = None
    node_scale = None
    globals_ = root.first("GlobalSettings")
    if globals_:
        props = globals_.first("Properties70")
        for p in (props.children if props else []):
            if p.name == "P" and p.props and p.props[0].value == "UnitScaleFactor":
                if len(p.props) > 4:
                    unit = float(p.props[4].value)
    objects = root.first("Objects")
    for obj in (objects.children if objects else []):
        if obj.name != "Model":
            continue
        scaling = _p70(obj, "Lcl Scaling")
        if scaling and len(scaling.props) > 6:
            value = abs(float(scaling.props[4].value))
            if value > 1.5:
                node_scale = value
                break
    if node_scale is not None:
        return True, f"node-scale={node_scale:g},unit={unit}"
    if unit is not None and abs(unit - METER_UNIT) > 0.5:
        return True, f"node-scale=1,unit={unit:g}"
    return False, f"node-scale=1,unit={unit}"


def patch(path, dst, force=False):
    version, nodes, original, footer_offset = parse(path)
    root = Node("", [], nodes, wide=version >= 7500)
    stats = {"unit": 0, "models": 0, "vertices": 0, "bindPose": 0, "clusters": 0, "scaledModels": 0}

    legacy, reason = detect_contract(root)
    stats["contract"] = reason
    if not legacy and not force:
        # Already meter: copy through untouched so a batch run stays idempotent.
        stats["skipped"] = True
        os.makedirs(os.path.dirname(os.path.abspath(dst)) or ".", exist_ok=True)
        with open(path, "rb") as src, open(dst, "wb") as fh:
            fh.write(src.read())
        return stats
    stats["skipped"] = False

    globals_ = root.first("GlobalSettings")
    if globals_:
        props = globals_.first("Properties70")
        for p in (props.children if props else []):
            if p.name != "P" or not p.props:
                continue
            key = p.props[0].value
            if key in ("UnitScaleFactor", "OriginalUnitScaleFactor"):
                target = p.props[4] if len(p.props) > 4 else None
                if target is not None:
                    target.value = (int(METER_UNIT) if target.code in "il"
                                    else METER_UNIT)
                    stats["unit"] += 1

    objects = root.first("Objects")
    for obj in (objects.children if objects else []):
        if obj.name == "Model":
            scaling = _p70(obj, "Lcl Scaling")
            if scaling and len(scaling.props) > 4:
                for i in (4, 5, 6):
                    scaling.props[i].value = 1.0
            translation = _p70(obj, "Lcl Translation")
            if translation and len(translation.props) > 4:
                _scale_translation(translation, LEGACY_FACTOR)
                stats["scaledModels"] += 1
            stats["models"] += 1
        elif obj.name == "Geometry":
            vertices = obj.first("Vertices")
            if vertices and vertices.props and vertices.props[0].code in ARRAY_CODES:
                prop = vertices.props[0]
                prop.value = [v * LEGACY_FACTOR for v in prop.value]
                stats["vertices"] += 1
        elif obj.name == "Pose":
            for pose_node in obj.find("PoseNode"):
                values = _matrix_array(pose_node)
                if values:
                    _retarget_global_matrix(values, NODE_SCALE, LEGACY_FACTOR * 0.01)
                    stats["bindPose"] += 1
        elif obj.name == "Deformer" and len(obj.props) > 2 and obj.props[2].value == "Cluster":
            link = obj.first("TransformLink")
            if link is not None:
                values = _matrix_array(link)
                if values:
                    _retarget_global_matrix(values, NODE_SCALE, LEGACY_FACTOR * 0.01)
            local = obj.first("Transform")
            if local is not None:
                values = _matrix_array(local)
                if values:
                    for i in (12, 13, 14):
                        values[i] = values[i] * LEGACY_FACTOR
            stats["clusters"] += 1

    os.makedirs(os.path.dirname(os.path.abspath(dst)), exist_ok=True)
    with open(dst, "wb") as fh:
        fh.write(serialize(version, nodes, original[footer_offset:]))
    return stats


def main():
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    as_json = "--json" in sys.argv
    if "--batch" in sys.argv:
        src_dir, dst_dir = args[0], args[1]
        os.makedirs(dst_dir, exist_ok=True)
        report = {}
        for name in sorted(f for f in os.listdir(src_dir) if f.lower().endswith(".fbx")):
            report[name] = patch(os.path.join(src_dir, name), os.path.join(dst_dir, name))
            print("PATCHED", name, report[name])
        skipped = sum(1 for s in report.values() if s.get("skipped"))
        print("TOTAL", len(report), "SKIPPED", skipped)
        return
    for i in range(0, len(args), 2):
        stats = patch(args[i], args[i + 1])
        if as_json:
            print(json.dumps({"src": args[i], "dst": args[i + 1], "stats": stats}, indent=1))
        else:
            print("PATCHED", args[i], "->", args[i + 1], stats)


if __name__ == "__main__":
    main()
