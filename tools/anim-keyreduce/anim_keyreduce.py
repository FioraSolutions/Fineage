#!/usr/bin/env python3
"""Offline keyframe reduction for Unity YAML AnimationClip (.anim) files.

Removes keys that the clip's own Hermite interpolation reproduces from the
surrounding kept keys within a per-channel tolerance. Constant curves collapse
to their first and last key. The file stays text YAML; every non-key line
(GUIDs, fileIDs, bindings, settings, events) is written back byte-for-byte.

Validation (always on, per clip, before anything is written):
  * the reduced curve is evaluated at every ORIGINAL key time and at the
    midpoint of every original segment, and compared to the original curve;
    the max error per channel must be <= tolerance;
  * the output re-parses, and its non-key skeleton is identical to the input.
A clip that fails parsing or validation is logged with a concrete reason and
left untouched (exit code 1 at the end if any clip failed).

Usage:
  anim_keyreduce.py [--dry-run] [--report out.csv] [--probe-out probes.json]
                    [--rot 1e-5] [--pos 1e-5] [--scale 1e-5] [--euler 1e-3]
                    [--float 1e-5] PATH...
PATH may be a .anim file or a directory (searched recursively).
"""
from __future__ import annotations

import argparse
import math
import os
import re
import sys
from dataclasses import dataclass, field

import numpy as np

CURVE_SECTIONS = {
    "m_RotationCurves": "rotation",
    "m_EulerCurves": "euler",
    "m_PositionCurves": "position",
    "m_ScaleCurves": "scale",
    "m_FloatCurves": "float",
    "m_EditorCurves": "editor",
    "m_EulerEditorCurves": "eulereditor",
}
SECTION_RE = re.compile(r"^  (m_\w+):(.*)$")
ITEM_RE = re.compile(r"^  - ")
KEY_START = "      - serializedVersion: 3"
CURVE_END_RE = re.compile(r"^      m_PreInfinity:")
FIELD_RE = re.compile(r"^        (\w+): (.*)$")
VEC_RE = re.compile(r"(\w+): ([^,}]+)")


class ClipError(Exception):
    pass


def parse_num(s: str) -> float:
    s = s.strip()
    try:
        return float(s)
    except ValueError:
        raise ClipError(f"cannot parse number {s!r}")


def parse_val(s: str):
    s = s.strip()
    if s.startswith("{"):
        return [parse_num(v) for _, v in VEC_RE.findall(s)]
    return [parse_num(s)]


@dataclass
class Key:
    start: int  # first line index of the key block
    end: int    # one past the last line
    time: float = 0.0
    value: list = field(default_factory=list)
    in_slope: list = field(default_factory=list)
    out_slope: list = field(default_factory=list)
    weighted: int = 0


@dataclass
class Curve:
    section: str
    item_line: int
    keys: list = field(default_factory=list)
    path: str = ""
    attribute: str = ""


def parse_clip(lines: list[str]):
    """Return (curves, header_flags). Raises ClipError on anything unexpected."""
    curves: list[Curve] = []
    flags = {}
    section = None
    cur: Curve | None = None
    key: Key | None = None
    i = 0
    n = len(lines)

    def close_key(end):
        nonlocal key
        if key is not None:
            key.end = end
            if len(key.value) == 0 or not key.in_slope or not key.out_slope:
                raise ClipError(f"incomplete key block at line {key.start + 1}")
            cur.keys.append(key)
            key = None

    while i < n:
        line = lines[i]
        m = SECTION_RE.match(line)
        if m:
            if cur is not None:
                close_key(i)
                curves.append(cur)
                cur = None
            name, rest = m.group(1), m.group(2).strip()
            flags[name] = rest
            section = name if name in CURVE_SECTIONS else None
            i += 1
            continue
        if section is None:
            i += 1
            continue
        if ITEM_RE.match(line):
            if cur is not None:
                close_key(i)
                curves.append(cur)
            cur = Curve(section=section, item_line=i)
            i += 1
            continue
        if cur is None:
            i += 1
            continue
        if line == KEY_START:
            close_key(i)
            key = Key(start=i, end=i)
            i += 1
            continue
        if key is not None:
            fm = FIELD_RE.match(line)
            if fm:
                f, v = fm.group(1), fm.group(2)
                if f == "time":
                    key.time = parse_num(v)
                elif f == "value":
                    key.value = parse_val(v)
                elif f == "inSlope":
                    key.in_slope = parse_val(v)
                elif f == "outSlope":
                    key.out_slope = parse_val(v)
                elif f == "weightedMode":
                    key.weighted = int(parse_num(v))
                i += 1
                continue
            close_key(i)
        if line.startswith("    path: "):
            cur.path = line[len("    path: "):]
        elif line.startswith("    attribute: "):
            cur.attribute = line[len("    attribute: "):]
        i += 1
    if cur is not None:
        close_key(n)
        curves.append(cur)
    return curves, flags


def hermite(t, t0, v0, m0, t1, v1, m1):
    """Unity AnimationCurve evaluation for a non-weighted segment (vectorised in t)."""
    dt = t1 - t0
    s = (t - t0) / dt
    s2 = s * s
    s3 = s2 * s
    a = 2 * s3 - 3 * s2 + 1
    b = s3 - 2 * s2 + s
    c = -2 * s3 + 3 * s2
    d = s3 - s2
    return (a[:, None] * v0 + b[:, None] * (m0 * dt) + c[:, None] * v1 + d[:, None] * (m1 * dt))


def eval_curve(T, V, IN, OUT, kept_idx, ts):
    """Evaluate the curve made of keys kept_idx at times ts (sorted)."""
    kt = T[kept_idx]
    out = np.empty((len(ts), V.shape[1]))
    seg = np.searchsorted(kt, ts, side="right") - 1
    seg = np.clip(seg, 0, len(kt) - 2) if len(kt) > 1 else np.zeros_like(seg)
    if len(kt) == 1:
        out[:] = V[kept_idx[0]]
        return out
    for s in np.unique(seg):
        mask = seg == s
        a, b = kept_idx[s], kept_idx[s + 1]
        tt = np.clip(ts[mask], T[a], T[b])
        out[mask] = hermite(tt, T[a], V[a], OUT[a], T[b], V[b], IN[b])
    return out


def reduce_group(arrays, tols):
    """Greedy reduction shared by all curves in a group (same key times).

    arrays: list of (T, V, IN, OUT); tols: tolerance per array.
    Returns sorted list of kept key indices.
    """
    T = arrays[0][0]
    n = len(T)
    if n <= 2:
        return list(range(n))
    # reference samples: original key times + original segment midpoints
    mids = (T[:-1] + T[1:]) / 2
    refs = []
    for (Ta, V, IN, OUT) in arrays:
        full = np.arange(n)
        refs.append((V, eval_curve(Ta, V, IN, OUT, full, mids)))

    def ok(a, b):
        # can keys a+1..b-1 be dropped, interpolating a->b?
        ts_k = T[a + 1:b]
        ts_m = mids[a:b]
        for (Ta, V, IN, OUT), (Vref, Mref), tol in zip(arrays, refs, tols):
            dt = Ta[b] - Ta[a]
            if dt <= 0:
                return False
            ek = hermite(ts_k, Ta[a], V[a], OUT[a], Ta[b], V[b], IN[b])
            if ek.size and np.max(np.abs(ek - V[a + 1:b])) > tol:
                return False
            em = hermite(ts_m, Ta[a], V[a], OUT[a], Ta[b], V[b], IN[b])
            if np.max(np.abs(em - Mref[a:b])) > tol:
                return False
        return True

    kept = [0]
    a = 0
    b = 2
    while b < n:
        if ok(a, b):
            b += 1
            continue
        kept.append(b - 1)
        a = b - 1
        b = a + 2
    kept.append(n - 1)
    return kept


def family(c: Curve):
    """Group key: curves that describe the same transform property."""
    if c.section in ("m_RotationCurves",):
        return (c.path, "rot")
    if c.section in ("m_PositionCurves",):
        return (c.path, "pos")
    if c.section in ("m_ScaleCurves",):
        return (c.path, "scale")
    if c.section in ("m_EulerCurves",):
        return (c.path, "rot")
    attr = c.attribute
    if attr.startswith("m_LocalRotation.") or attr.startswith("localEulerAngles"):
        return (c.path, "rot")
    if attr.startswith("m_LocalPosition."):
        return (c.path, "pos")
    if attr.startswith("m_LocalScale."):
        return (c.path, "scale")
    return (c.path, "float:" + c.section + ":" + attr)


def tol_for(c: Curve, args):
    if c.section == "m_RotationCurves" or c.attribute.startswith("m_LocalRotation."):
        return args.rot, "rot"
    if c.section in ("m_EulerCurves",) or c.attribute.startswith("localEulerAngles"):
        return args.euler, "euler"
    if c.section == "m_PositionCurves" or c.attribute.startswith("m_LocalPosition."):
        return args.pos, "pos"
    if c.section == "m_ScaleCurves" or c.attribute.startswith("m_LocalScale."):
        return args.scale, "scale"
    return args.float, "float"


def curve_arrays(c: Curve):
    T = np.array([k.time for k in c.keys], dtype=np.float64)
    V = np.array([k.value for k in c.keys], dtype=np.float64)
    IN = np.array([k.in_slope for k in c.keys], dtype=np.float64)
    OUT = np.array([k.out_slope for k in c.keys], dtype=np.float64)
    return T, V, IN, OUT


def reducible(c: Curve):
    """Why a curve must keep all its keys, or None if it can be reduced."""
    if any(k.weighted != 0 for k in c.keys):
        return "weighted tangents"
    T, V, IN, OUT = curve_arrays(c)
    if not (np.all(np.isfinite(V)) and np.all(np.isfinite(IN)) and np.all(np.isfinite(OUT))):
        return "non-finite value/tangent (stepped key)"
    if np.any(np.diff(T) <= 0):
        return "non-increasing key times"
    return None


def skeleton(lines, curves):
    drop = set()
    for c in curves:
        for k in c.keys:
            drop.update(range(k.start, k.end))
    return [l for i, l in enumerate(lines) if i not in drop]


def make_probes(curves, args, count):
    """Pick the `count` editor curves with the widest value range and record the
    ORIGINAL curve at every key time and segment midpoint (for the Unity-side test)."""
    cands = []
    for c in curves:
        if c.section != "m_EditorCurves" or len(c.keys) < 3 or not c.attribute:
            continue
        # Unity often exposes rotation via m_RotationCurves; editor float rotation
        # components are not always readable through AnimationUtility.GetEditorCurve.
        if c.attribute.startswith("m_LocalRotation."):
            continue
        if c.path.startswith(("'", '"')) or reducible(c):
            continue
        T, V, IN, OUT = curve_arrays(c)
        cands.append((float(np.ptp(V[:, 0])), c, T, V, IN, OUT))
    cands.sort(key=lambda x: -x[0])
    out = []
    for rng, c, T, V, IN, OUT in cands[:count]:
        mids = (T[:-1] + T[1:]) / 2
        ts = np.sort(np.concatenate([T, mids]))
        vals = eval_curve(T, V, IN, OUT, np.arange(len(T)), ts)[:, 0]
        tol = tol_for(c, args)[0]
        out.append({"path": c.path, "attribute": c.attribute, "tolerance": tol,
                    "times": [float(t) for t in ts], "values": [float(v) for v in vals]})
    return out


def process(path, args, log):
    with open(path, "r", encoding="utf-8", newline="") as f:
        text = f.read()
    lines = text.split("\n")
    before = len(text.encode("utf-8"))
    try:
        if not lines[0].startswith("%YAML") or "AnimationClip:" not in text[:400]:
            raise ClipError("not a Unity YAML AnimationClip")
        curves, flags = parse_clip(lines)
        if flags.get("m_CompressedRotationCurves", "[]") != "[]":
            raise ClipError("m_CompressedRotationCurves is not empty (unsupported)")
        probes = make_probes(curves, args, args.probe_curves) if args.probe_out else []
        groups: dict = {}
        for c in curves:
            if c.keys:
                groups.setdefault(family(c), []).append(c)
        drop_lines = set()
        stats = {"keys_before": 0, "keys_after": 0, "max_err": {}, "kept_full": 0}
        for gk, members in groups.items():
            # members with identical key times share one mask; others reduce alone
            buckets: dict = {}
            for c in members:
                why = reducible(c)
                stats["keys_before"] += len(c.keys)
                if why:
                    stats["kept_full"] += 1
                    stats["keys_after"] += len(c.keys)
                    continue
                tkey = tuple(k.time for k in c.keys)
                buckets.setdefault(tkey, []).append(c)
            for tkey, cs in buckets.items():
                arrays = [curve_arrays(c) for c in cs]
                tols = [tol_for(c, args)[0] for c in cs]
                kept = reduce_group(arrays, tols)
                keptset = set(kept)
                kidx = np.array(kept)
                for c, arr, tol in zip(cs, arrays, tols):
                    T, V, IN, OUT = arr
                    # independent validation against the original curve
                    mids = (T[:-1] + T[1:]) / 2
                    full = np.arange(len(T))
                    err_k = np.max(np.abs(eval_curve(T, V, IN, OUT, kidx, T) - V)) if len(T) else 0.0
                    err_m = (np.max(np.abs(eval_curve(T, V, IN, OUT, kidx, mids) - eval_curve(T, V, IN, OUT, full, mids)))
                             if len(T) > 1 else 0.0)
                    err = float(max(err_k, err_m))
                    ch = tol_for(c, args)[1]
                    if err > tol * (1 + 1e-9):
                        raise ClipError(f"validation failed: {ch} curve {c.path!r} {c.attribute!r} err {err:.3g} > tol {tol:g}")
                    stats["max_err"][ch] = max(stats["max_err"].get(ch, 0.0), err)
                    stats["keys_after"] += len(kept)
                    for i, k in enumerate(c.keys):
                        if i not in keptset:
                            drop_lines.update(range(k.start, k.end))
        new_lines = [l for i, l in enumerate(lines) if i not in drop_lines]
        new_text = "\n".join(new_lines)
        # round-trip check: re-parse output and compare skeletons + key counts
        new_curves, _ = parse_clip(new_lines)
        if skeleton(lines, curves) != skeleton(new_lines, new_curves):
            raise ClipError("round-trip check failed: non-key content changed")
        if sum(len(c.keys) for c in new_curves) != stats["keys_after"]:
            raise ClipError("round-trip check failed: key count mismatch after re-parse")
    except ClipError as e:
        log(f"ERROR {path}: {e} -- clip left unchanged")
        return None
    after = len(new_text.encode("utf-8"))
    if not args.dry_run and after < before:
        tmp = path + ".tmp-keyreduce"
        with open(tmp, "w", encoding="utf-8", newline="") as f:
            f.write(new_text)
        os.replace(tmp, path)
    stats["probes"] = probes
    return before, after, stats


def iter_paths(paths):
    for p in paths:
        if os.path.isdir(p):
            for root, _, files in os.walk(p):
                for fn in sorted(files):
                    if fn.endswith(".anim"):
                        yield os.path.join(root, fn)
        elif p.endswith(".anim"):
            yield p
        else:
            print(f"ERROR {p}: not a .anim file or directory", file=sys.stderr)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("paths", nargs="+")
    ap.add_argument("--rot", type=float, default=1e-5, help="quaternion component tolerance")
    ap.add_argument("--pos", type=float, default=1e-5, help="position tolerance (Unity units)")
    ap.add_argument("--scale", type=float, default=1e-5, help="scale tolerance")
    ap.add_argument("--euler", type=float, default=1e-3, help="euler editor curve tolerance (degrees)")
    ap.add_argument("--float", type=float, default=1e-5, help="other float curves tolerance")
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument("--report", help="write per-clip CSV report")
    ap.add_argument("--probe-out", help="write JSON probes of the ORIGINAL curves for the Unity EditMode check")
    ap.add_argument("--probe-curves", type=int, default=6, help="editor curves probed per clip")
    args = ap.parse_args()

    failures = 0
    tb = ta = 0
    rows = []
    gmax: dict = {}
    probe_clips = []
    for p in iter_paths(args.paths):
        res = process(p, args, lambda m: print(m, file=sys.stderr))
        if res is None:
            failures += 1
            continue
        b, a, st = res
        if args.probe_out and st["probes"] and a < b:
            norm = p.replace(os.sep, "/")
            idx = norm.find("Assets/")
            if idx < 0:
                print(f"ERROR {p}: --probe-out needs paths inside a Unity Assets/ folder", file=sys.stderr)
                failures += 1
            else:
                probe_clips.append({"asset": norm[idx:], "curves": st["probes"]})
        tb += b
        ta += a
        for k, v in st["max_err"].items():
            gmax[k] = max(gmax.get(k, 0.0), v)
        rows.append((p, b, a, st["keys_before"], st["keys_after"], st["kept_full"],
                     ";".join(f"{k}={v:.3g}" for k, v in sorted(st["max_err"].items()))))
        print(f"{b/1e6:9.2f} MB -> {a/1e6:8.2f} MB  keys {st['keys_before']} -> {st['keys_after']}  {p}")
    if args.probe_out:
        import json
        with open(args.probe_out, "w", encoding="utf-8") as f:
            json.dump({"clips": probe_clips}, f, separators=(",", ":"))
    if args.report:
        with open(args.report, "w", encoding="utf-8") as f:
            f.write("path,bytes_before,bytes_after,keys_before,keys_after,curves_not_reduced,max_err\n")
            for r in rows:
                f.write(",".join(f'"{x}"' if isinstance(x, str) else str(x) for x in r) + "\n")
    print(f"TOTAL {len(rows)} clips: {tb/1e6:.2f} MB -> {ta/1e6:.2f} MB"
          f" ({(1 - ta / tb) * 100 if tb else 0:.1f}% smaller); max error {gmax}; failures {failures}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
