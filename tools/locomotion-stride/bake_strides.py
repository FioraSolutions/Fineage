#!/usr/bin/env python3
"""Bake locomotion stride sidecars from committed Unity .anim clips.

Interlude walk/run clips are in-place: the skeleton root (bip01) returns to
its start pose, so net root travel is ~0. Stride is then the distance the
root must advance per cycle for the feet to plant — the sum of each foot's
dominant planar swing in root space over one cycle. When a clip really
translates the root (net planar travel), that displacement is the stride.

Writes:
  <clip>.stride.json beside each walk/run .anim
  l2-unity/Assets/Resources/Data/Animations/locomotion_strides.json

Rebake (no Unity required; the .anim YAML is the source):

  python3 tools/locomotion-stride/bake_strides.py

Unity batch (same script):

  Unity -batchmode -quit -projectPath l2-unity -executeMethod L2LocomotionStrideBake.RunCli
"""
from __future__ import annotations

import json
import math
import os
import re
import sys
import uuid

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
ANIM_ROOT = os.path.join(ROOT, "l2-unity", "Assets", "Resources", "Data", "Animations")
INDEX_PATH = os.path.join(ANIM_ROOT, "locomotion_strides.json")
MIN_STRIDE = 0.05

PLAYER_DIRS = [
    "Fighter/MFighter/Clips",
    "Fighter/FFighter/Clips",
    "Magic/MMagic/Clips",
    "Magic/FMagic/Clips",
    "Elf/MElf/Clips",
    "Elf/FElf/Clips",
    "DarkElf/MDarkElf/Clips",
    "DarkElf/FDarkElf/Clips",
    "Dwarf/MDwarf/Clips",
    "Dwarf/FDwarf/Clips",
    "Orc/MOrc/Clips",
    "Orc/FOrc/Clips",
    "Orc/MShaman/Clips",
    "Orc/FShaman/Clips",
]

SKIP_TOKENS = (
    "atk", "wait", "social", "cast", "death", "sit", "stun", "damage",
    "jump", "fall", "swim", "turn", "stand",
)


def quat_mul(a, b):
    ax, ay, az, aw = a
    bx, by, bz, bw = b
    return (
        aw * bx + ax * bw + ay * bz - az * by,
        aw * by - ax * bz + ay * bw + az * bx,
        aw * bz + ax * by - ay * bx + az * bw,
        aw * bw - ax * bx - ay * by - az * bz,
    )


def quat_norm(q):
    x, y, z, w = q
    n = math.sqrt(x * x + y * y + z * z + w * w)
    if n < 1e-12:
        return (0.0, 0.0, 0.0, 1.0)
    return (x / n, y / n, z / n, w / n)


def quat_rotate(q, v):
    x, y, z, w = q
    vx, vy, vz = v
    tx = 2 * (y * vz - z * vy)
    ty = 2 * (z * vx - x * vz)
    tz = 2 * (x * vy - y * vx)
    return (
        vx + w * tx + (y * tz - z * ty),
        vy + w * ty + (z * tx - x * tz),
        vz + w * tz + (x * ty - y * tx),
    )


def parse_vec(s):
    s = s.strip().strip("{}")
    out = {}
    for part in s.split(","):
        k, v = part.split(":")
        out[k.strip()] = float(v.strip())
    if "w" in out:
        return (out["x"], out["y"], out["z"], out["w"])
    return (out["x"], out["y"], out["z"])


def unwrap_paths(lines):
    """Unity wraps long bone paths that contain spaces onto the next indented line."""
    out = []
    i = 0
    n = len(lines)
    while i < n:
        line = lines[i]
        if line.strip().startswith("path:"):
            while i + 1 < n:
                nxt = lines[i + 1]
                body = nxt.strip()
                if not body or ":" in body or not nxt.startswith(" "):
                    break
                line = line.rstrip("\n") + " " + body
                i += 1
            if not line.endswith("\n"):
                line += "\n"
        out.append(line)
        i += 1
    return out


def parse_clip(path):
    pos = {}
    rot = {}
    section = None
    length = 0.0
    pending = []
    cur_time = None
    cur_value = None

    def flush_key():
        nonlocal cur_time, cur_value
        if cur_time is not None and cur_value is not None:
            pending.append((cur_time, cur_value))
        cur_time = None
        cur_value = None

    with open(path, "r", errors="replace") as f:
        raw_lines = unwrap_paths(f.readlines())
    for line in raw_lines:
            if line.startswith("  m_RotationCurves:"):
                section = "rot"
                pending.clear()
                cur_time = None
                cur_value = None
                continue
            if line.startswith("  m_PositionCurves:"):
                section = "pos"
                pending.clear()
                cur_time = None
                cur_value = None
                continue
            if section in ("pos", "rot") and line.startswith("  m_") and not line.startswith("  m_Curve"):
                if not line.startswith("  m_PreInfinity") and not line.startswith("  m_PostInfinity") and not line.startswith("  m_RotationOrder"):
                    section = None
                    continue
            if section not in ("pos", "rot"):
                if line.strip().startswith("m_StopTime:"):
                    try:
                        length = max(length, float(line.split(":", 1)[1].strip()))
                    except ValueError:
                        pass
                continue
            stripped = line.strip()
            if stripped.startswith("- curve:"):
                flush_key()
                pending.clear()
                continue
            if stripped.startswith("path:"):
                flush_key()
                cur_path = stripped.split(":", 1)[1].strip()
                bucket = pos if section == "pos" else rot
                bucket[cur_path] = list(pending)
                for t, _v in pending:
                    if t > length:
                        length = t
                pending.clear()
                continue
            if stripped.startswith("time:"):
                flush_key()
                cur_time = float(stripped.split(":", 1)[1].strip())
                continue
            if stripped.startswith("value:"):
                cur_value = parse_vec(stripped.split(":", 1)[1])
                continue
    return pos, rot, length


def sample(keys, t):
    if not keys:
        return None
    if t <= keys[0][0]:
        return keys[0][1]
    if t >= keys[-1][0]:
        return keys[-1][1]
    lo, hi = 0, len(keys) - 1
    while hi - lo > 1:
        mid = (lo + hi) // 2
        if keys[mid][0] <= t:
            lo = mid
        else:
            hi = mid
    t0, v0 = keys[lo]
    t1, v1 = keys[hi]
    if t1 <= t0:
        return v0
    u = (t - t0) / (t1 - t0)
    if len(v0) == 4:
        dot = v0[0] * v1[0] + v0[1] * v1[1] + v0[2] * v1[2] + v0[3] * v1[3]
        if dot < 0:
            v1 = (-v1[0], -v1[1], -v1[2], -v1[3])
        return quat_norm(tuple(v0[i] + (v1[i] - v0[i]) * u for i in range(4)))
    return tuple(v0[i] + (v1[i] - v0[i]) * u for i in range(3))


def world_of(path, t, pos, rot, cache):
    key = (path, t)
    if key in cache:
        return cache[key]
    if not path:
        cache[key] = ((0.0, 0.0, 0.0), (0.0, 0.0, 0.0, 1.0))
        return cache[key]
    parent = path.rsplit("/", 1)[0] if "/" in path else ""
    pp, pr = world_of(parent, t, pos, rot, cache) if parent else ((0.0, 0.0, 0.0), (0.0, 0.0, 0.0, 1.0))
    lp = sample(pos.get(path, []), t) or (0.0, 0.0, 0.0)
    lr = sample(rot.get(path, []), t) or (0.0, 0.0, 0.0, 1.0)
    wr = quat_mul(pr, lr)
    delta = quat_rotate(pr, lp)
    wp = (pp[0] + delta[0], pp[1] + delta[1], pp[2] + delta[2])
    cache[key] = (wp, wr)
    return cache[key]


def planar(a, b):
    return math.hypot(a[0] - b[0], a[2] - b[2])


def gait_of(filename):
    n = filename.lower()
    if any(tok in n for tok in SKIP_TOKENS):
        return None
    if re.search(r"(^|[_.])walk([_.]|$)", n) or ".ao_walk" in n:
        return "walk"
    if re.search(r"(^|[_.])run([_.]|$)", n) or ".ao_run" in n:
        return "run"
    return None


def is_foot(path):
    parts = path.split("/")
    leaf = parts[-1].lower()
    parent = parts[-2].lower() if len(parts) >= 2 else ""
    if any(x in leaf for x in ("toe", "nub", "finger", "hand", "dummy")):
        return False
    if "foot" in leaf or leaf.endswith("ankle") or " ankle" in leaf:
        return True
    # L2 creatures often name the foot bone "Bip01" directly under the calf.
    if "calf" in parent:
        return True
    return False


def dominant_range(pts):
    if len(pts) < 2:
        return 0.0
    xs = [p[0] for p in pts]
    zs = [p[2] for p in pts]
    return max(max(xs) - min(xs), max(zs) - min(zs))


def measure(path):
    pos, rot, length = parse_clip(path)
    all_paths = list(pos.keys()) + list(rot.keys())
    roots = [p for p in all_paths if p.lower().endswith("/bip01") or p.lower() == "bip01"]
    if not roots:
        # Non-biped rigs: the skeleton root is the shallowest bone under the mesh.
        candidates = [p for p in all_paths if p.count("/") == 1]
        roots = candidates
    if not roots:
        return None
    root = sorted(roots, key=len)[0]
    feet = []
    seen = set()
    for p in list(pos.keys()) + list(rot.keys()):
        if p in seen or not is_foot(p):
            continue
        seen.add(p)
        feet.append(p)
    times = sorted({k[0] for k in pos.get(root, [])})
    if len(times) < 2 and length > 0:
        times = [i * length / 16.0 for i in range(17)]
    if len(times) < 2:
        return None
    root_pts = []
    foot_rel = {f: [] for f in feet}
    for t in times:
        cache = {}
        rp, rr = world_of(root, t, pos, rot, cache)
        root_pts.append(rp)
        iq = (-rr[0], -rr[1], -rr[2], rr[3])
        for f in feet:
            fp, _fr = world_of(f, t, pos, rot, cache)
            delta = (fp[0] - rp[0], fp[1] - rp[1], fp[2] - rp[2])
            foot_rel[f].append(quat_rotate(iq, delta))
    root_net = planar(root_pts[-1], root_pts[0])
    amps = sorted((dominant_range(pts) for pts in foot_rel.values()), reverse=True)
    # Two largest swings: both feet of a biped, or the leading pair on a quadruped.
    if len(amps) >= 2:
        foot_sum = amps[0] + amps[1]
    elif len(amps) == 1:
        foot_sum = amps[0]
    else:
        foot_sum = 0.0
    if root_net >= MIN_STRIDE and foot_sum < MIN_STRIDE:
        stride = root_net
        method = "root-net"
    else:
        stride = foot_sum
        method = "in-place-feet"
    if stride < MIN_STRIDE:
        return {
            "ok": False,
            "reason": "stride %.4f below min (rootNet=%.4f feet=%d)" % (stride, root_net, len(feet)),
            "length": length,
        }
    return {
        "ok": True,
        "strideMeters": round(stride, 4),
        "method": method,
        "rootPlanarMeters": round(root_net, 4),
        "clipLength": round(length, 4),
        "feet": len(feet),
    }


def resources_path(anim_path):
    rel = os.path.relpath(anim_path, os.path.join(ROOT, "l2-unity", "Assets", "Resources"))
    return os.path.splitext(rel)[0].replace(os.sep, "/")


def write_meta(path):
    meta = path + ".meta"
    if os.path.isfile(meta):
        return
    guid = uuid.uuid4().hex
    with open(meta, "w", newline="\n") as f:
        f.write(
            "fileFormatVersion: 2\n"
            "guid: %s\n"
            "DefaultImporter:\n"
            "  externalObjects: {}\n"
            "  userData: \n"
            "  assetBundleName: \n"
            "  assetBundleVariant: \n" % guid
        )


def main():
    entries = []
    failed_players = []
    failed_other = 0
    measured = 0
    for dirpath, _dirs, files in os.walk(ANIM_ROOT):
        for fn in files:
            if not fn.endswith(".anim"):
                continue
            gait = gait_of(fn)
            if gait is None:
                continue
            full = os.path.join(dirpath, fn)
            rel_dir = os.path.relpath(dirpath, ANIM_ROOT).replace(os.sep, "/")
            result = measure(full)
            clip = os.path.splitext(fn)[0]
            if not result or not result.get("ok"):
                reason = (result or {}).get("reason", "unmeasurable")
                if any(rel_dir == d or rel_dir.startswith(d + "/") for d in PLAYER_DIRS):
                    # only the catalog locomotion clips are required; other clips in the folder
                    # (mounts, attacks mis-named) are reported but player walk/run must pass
                    if gait in ("walk", "run"):
                        failed_players.append("%s: %s" % (clip, reason))
                else:
                    failed_other += 1
                continue
            sidecar = os.path.splitext(full)[0] + ".stride.json"
            payload = {
                "clip": clip,
                "gait": gait,
                "strideMeters": result["strideMeters"],
                "method": result["method"],
                "rootPlanarMeters": result["rootPlanarMeters"],
                "clipLength": result["clipLength"],
            }
            with open(sidecar, "w", newline="\n") as f:
                json.dump(payload, f, indent=2)
                f.write("\n")
            write_meta(sidecar)
            entries.append({
                "clip": clip,
                "path": resources_path(full),
                "gait": gait,
                "strideMeters": result["strideMeters"],
            })
            measured += 1

    entries.sort(key=lambda e: e["path"])
    names = {}
    dupes = []
    for e in entries:
        if e["clip"] in names:
            dupes.append(e["clip"])
        names[e["clip"]] = e["path"]
    index = {
        "generatedBy": "tools/locomotion-stride/bake_strides.py",
        "minStrideMeters": MIN_STRIDE,
        "entries": entries,
    }
    with open(INDEX_PATH, "w", newline="\n") as f:
        json.dump(index, f, indent=2)
        f.write("\n")
    write_meta(INDEX_PATH)
    print("measured", measured, "index", INDEX_PATH)
    print("duplicate clip names", len(set(dupes)))
    for d in sorted(set(dupes))[:30]:
        print("  dupe", d)
    print("non-player unmeasurable", failed_other)
    if failed_players:
        print("PLAYER FAILURES", len(failed_players))
        for line in failed_players:
            print(" ", line)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
