"""Move extracted Unity .anim clips onto the meter contract, alongside fbx_meter_patch.py.

Clips extracted from a legacy-unit FBX (UnitScaleFactor 1, node Lcl Scaling 100) that Unity
imported with globalScale 1 + useFileScale carry:
  * bone m_LocalPosition keys in file units x 0.01 (the useFileScale factor), and
  * a x100 m_LocalScale carrier on the bare `.ao` / `.mo` node.
After fbx_meter_patch.py the rig has node scale 1 and translations x1.9, so every position
key grows by 100 x 1.9 = 190 and the carrier is pinned to 1. World poses stay identical
relative to the patched mesh.

Idempotent: a clip whose `.ao`/`.mo` scale is not ~100 is already meter and is left alone.

Usage: python3 tools/interlude/anim_meter_patch.py [--factor 190] <file.anim|dir> [...]
"""
from __future__ import annotations

import os
import re
import sys

NODE_SCALE = 100.0
DEFAULT_FACTOR = NODE_SCALE * 1.9

_SECTION = re.compile(r"^  (m_\w+):", re.M)
_VEC = re.compile(r"^(\s+(?:value|inSlope|outSlope): )\{x: ([^,]+), y: ([^,]+), z: ([^,}]+)\}$", re.M)


def _fmt(v: float) -> str:
    if v == 0:
        return "0"
    return repr(float(f"{v:.9g}"))


def _sections(text):
    marks = [(m.start(), m.group(1)) for m in _SECTION.finditer(text)]
    for i, (start, name) in enumerate(marks):
        end = marks[i + 1][0] if i + 1 < len(marks) else len(text)
        yield name, start, end


def _blocks(section):
    parts = section.split("\n  - curve:")
    return parts[0], parts[1:]


def _block_path(block):
    m = re.search(r"^    path: (.*)$", block, re.M)
    return m.group(1).strip() if m else ""


def _is_carrier(path):
    return "/" not in path and (path.endswith(".ao") or path.endswith(".mo"))


def _scale_block(block, factor):
    def repl(m):
        x, y, z = (float(m.group(i)) * factor for i in (2, 3, 4))
        return f"{m.group(1)}{{x: {_fmt(x)}, y: {_fmt(y)}, z: {_fmt(z)}}}"
    return _VEC.sub(repl, block)


def _unit_block(block):
    def repl(m):
        if m.group(1).strip().startswith("value"):
            return f"{m.group(1)}{{x: 1, y: 1, z: 1}}"
        return f"{m.group(1)}{{x: 0, y: 0, z: 0}}"
    return _VEC.sub(repl, block)


_EDITOR_SPLIT = "\n  - serializedVersion: 2\n    curve:"
_SCALAR = re.compile(r"^(\s+(value|inSlope|outSlope): )(\S+)$", re.M)


def _patch_editor_curves(section, factor):
    """Editor float curves (`m_LocalPosition.x` ...) mirror the vector curves Unity bakes."""
    parts = section.split(_EDITOR_SPLIT)
    out = [parts[0]]
    for block in parts[1:]:
        attr = re.search(r"^    attribute: (\S+)$", block, re.M)
        path = re.search(r"^    path: (.*)$", block, re.M)
        attr = attr.group(1) if attr else ""
        path = path.group(1).strip() if path else ""
        if attr.startswith("m_LocalPosition"):
            block = _SCALAR.sub(lambda m: m.group(1) + _fmt(float(m.group(3)) * factor), block)
        elif attr.startswith("m_LocalScale") and _is_carrier(path):
            block = _SCALAR.sub(lambda m: m.group(1) + ("1" if m.group(2) == "value" else "0"), block)
        out.append(block)
    return _EDITOR_SPLIT.join(out)


def is_legacy(text):
    for name, start, end in _sections(text):
        if name != "m_ScaleCurves":
            continue
        _, blocks = _blocks(text[start:end])
        for b in blocks:
            if not _is_carrier(_block_path(b)):
                continue
            m = re.search(r"value: \{x: ([^,]+),", b)
            if m and abs(abs(float(m.group(1))) - NODE_SCALE) < 1.0:
                return True
    return False


def patch_text(text, factor=DEFAULT_FACTOR):
    if not is_legacy(text):
        return text, False
    if re.search(r"^  m_Compressed: 1", text, re.M):
        raise ValueError("compressed clips are not handled; convert in Unity instead")
    out = []
    pos = 0
    for name, start, end in _sections(text):
        out.append(text[pos:start])
        section = text[start:end]
        if name == "m_PositionCurves":
            head, blocks = _blocks(section)
            section = head + "".join("\n  - curve:" + _scale_block(b, factor) for b in blocks)
        elif name == "m_ScaleCurves":
            head, blocks = _blocks(section)
            section = head + "".join(
                "\n  - curve:" + (_unit_block(b) if _is_carrier(_block_path(b)) else b) for b in blocks)
        elif name == "m_EditorCurves":
            section = _patch_editor_curves(section, factor)
        out.append(section)
        pos = end
    out.append(text[pos:])
    return "".join(out), True


def iter_files(args):
    for a in args:
        if os.path.isdir(a):
            for folder, _d, files in os.walk(a):
                for f in sorted(files):
                    if f.endswith(".anim"):
                        yield os.path.join(folder, f)
        elif a.endswith(".anim"):
            yield a


def main():
    args = sys.argv[1:]
    factor = DEFAULT_FACTOR
    if "--factor" in args:
        i = args.index("--factor")
        factor = float(args[i + 1])
        del args[i:i + 2]
    changed = skipped = 0
    for path in iter_files(args):
        with open(path, "r", encoding="utf-8", newline="") as fh:
            text = fh.read()
        new, did = patch_text(text, factor)
        if not did:
            skipped += 1
            continue
        tmp = path + ".tmp"
        with open(tmp, "w", encoding="utf-8", newline="") as fh:
            fh.write(new)
        os.replace(tmp, path)
        changed += 1
    print(f"clips patched={changed} skipped={skipped}")


if __name__ == "__main__":
    main()
