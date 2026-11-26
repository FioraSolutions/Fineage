#!/usr/bin/env python3
"""Build the Interlude asset whitelist from the original Interlude client files.

Inputs (never committed):
  --client   "Lineage II" folder of the original Interlude client
  --psa-dir  umodel PSA export of animations/{Fighter,Magic,Elf,DarkElf,Dwarf,Orc,Shaman}.ukx
             (umodel -game=l2 -export -nomesh -notex -nostat -out=<psa-dir> <ukx>)
  --l2encdec open-l2encdec binary (decodes Lineage2Ver413 skillgrp.dat)
Output: Docs/interlude-whitelist/*.tsv (see README there).
"""
from __future__ import annotations

import argparse
import collections
import os
import struct
import subprocess
import sys
import tempfile
from pathlib import Path


# ---------------------------------------------------------------- UE2 package
def decode_lineage2_header(data: bytes) -> bytes:
    """Strip Lineage2Ver111/121 header and XOR body (key = first byte ^ package tag byte)."""
    head = data[:28].decode("utf-16le", errors="ignore")
    if not head.startswith("Lineage2Ver"):
        return data
    ver = head[11:14]
    if ver not in ("111", "121"):
        raise SystemExit(f"unsupported Lineage2Ver{ver} (needs l2encdec)")
    body = data[28:]
    key = body[0] ^ 0xC1
    return bytes(b ^ key for b in body)


class Package:
    def __init__(self, data: bytes):
        self.d = data
        tag, self.ver, self.lic, _flags, nc, no, ec, eo, ic, io = struct.unpack_from("<IHHIiiiiii", data, 0)
        if tag != 0x9E2A83C1:
            raise SystemExit("not an Unreal package")
        self.p = no
        self.names = []
        for _ in range(nc):
            n = self.ci()
            if n < 0:
                s = data[self.p:self.p - 2 * n].decode("utf-16le").rstrip("\0")
                self.p += -2 * n
            else:
                s = data[self.p:self.p + n].rstrip(b"\0").decode("latin1")
                self.p += n
            self.p += 4
            self.names.append(s)
        self.p = io
        self.imports = []
        for _ in range(ic):
            cp, cn = self.ci(), self.ci()
            pk = self.i32()
            on = self.ci()
            self.imports.append((self.names[cp], self.names[cn], pk, self.names[on]))
        self.p = eo
        self.exports = []
        for _ in range(ec):
            c, s = self.ci(), self.ci()
            pk = self.i32()
            on = self.ci()
            fl = self.i32()
            sz = self.ci()
            off = self.ci() if sz > 0 else 0
            self.exports.append(dict(cls=c, sup=s, outer=pk, name=self.names[on], size=sz, off=off))
        for e in self.exports:
            e["class"] = self.objname(e["cls"])
            e["super"] = self.objname(e["sup"]) if e["sup"] else ""
            e["outername"] = self.objname(e["outer"]) if e["outer"] else ""

    def ci_at(self, o: int) -> tuple[int, int]:
        b = self.d[o]
        o += 1
        neg, v = b & 0x80, b & 0x3F
        if b & 0x40:
            sh = 6
            while True:
                b = self.d[o]
                o += 1
                v |= (b & 0x7F) << sh
                sh += 7
                if not b & 0x80:
                    break
        return (-v if neg else v), o

    def ci(self) -> int:
        v, self.p = self.ci_at(self.p)
        return v

    def i32(self) -> int:
        v = struct.unpack_from("<i", self.d, self.p)[0]
        self.p += 4
        return v

    def objname(self, i: int) -> str:
        if i > 0:
            return self.exports[i - 1]["name"]
        if i < 0:
            return self.imports[-i - 1][3]
        return "Class"

    def import_path(self, i: int) -> str:
        imp = self.imports[-i - 1]
        parts, pk = [imp[3]], imp[2]
        while pk < 0:
            q = self.imports[-pk - 1]
            parts.insert(0, q[3])
            pk = q[2]
        return ".".join(parts)

    def props(self, off: int, end: int):
        """Tagged properties: yields (name, type, value_offset, size, info)."""
        o = off
        while o < end:
            ni, o = self.ci_at(o)
            name = self.names[ni]
            if name == "None":
                return
            info = self.d[o]
            o += 1
            t, st, arr = info & 0xF, (info >> 4) & 7, info & 0x80
            if t == 10:
                _, o = self.ci_at(o)
            size = {0: 1, 1: 2, 2: 4, 3: 12, 4: 16}.get(st)
            if st == 5:
                size = self.d[o]; o += 1
            elif st == 6:
                size = struct.unpack_from("<H", self.d, o)[0]; o += 2
            elif st == 7:
                size = struct.unpack_from("<i", self.d, o)[0]; o += 4
            if arr and t != 3:
                b = self.d[o]; o += 1
                if b & 0x80:
                    o += 3 if b & 0x40 else 1
            yield name, t, o, size, info
            o += size


def load_pkg(path: Path) -> Package:
    return Package(decode_lineage2_header(path.read_bytes()))


# ---------------------------------------------------------------- PSA
def psa_sequences(path: Path):
    d = path.read_bytes()
    o = 0
    while o < len(d):
        name = d[o:o + 20].split(b"\0")[0].decode()
        sz, cnt = struct.unpack_from("<ii", d, o + 24)
        o += 32
        if name == "ANIMINFO":
            for i in range(cnt):
                r = d[o + i * sz:o + (i + 1) * sz]
                yield (r[:64].split(b"\0")[0].decode("latin1"),
                       struct.unpack_from("<i", r, 164)[0],
                       struct.unpack_from("<f", r, 152)[0])  # AnimRate
            return  # ANIMINFO precedes ANIMKEYS; the keys are not needed
        o += sz * cnt


# ---------------------------------------------------------------- skillgrp
def parse_skillgrp(dec: bytes):
    o = 0

    def i32():
        nonlocal o
        v = struct.unpack_from("<i", dec, o)[0]; o += 4; return v

    def f32():
        nonlocal o
        v = struct.unpack_from("<f", dec, o)[0]; o += 4; return v

    def uni():
        nonlocal o
        n = i32(); v = dec[o:o + n].decode("utf-16le").rstrip("\0"); o += n; return v

    rows = []
    for _ in range(i32()):
        sid, lvl, _op, _mp, _rng, _cs = i32(), i32(), i32(), i32(), i32(), i32()
        _hit = f32()
        _mag = i32()
        ani, vis, _icon = uni(), uni(), uni()
        for _ in range(6):
            i32()
        rows.append((sid, lvl, ani, vis))
    return rows


EAM = ["EAM_None", "EAM_RH", "EAM_LH", "EAM_BoneSpecified", "EAM_AliasSpecified", "EAM_Trail", "EAM_RF", "EAM_LF"]


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--client", required=True, type=Path)
    ap.add_argument("--psa-dir", required=True, type=Path)
    ap.add_argument("--l2encdec", required=True, type=Path)
    ap.add_argument("--out", type=Path, default=Path(__file__).resolve().parents[2] / "Docs/interlude-whitelist")
    a = ap.parse_args()
    out = a.out
    out.mkdir(parents=True, exist_ok=True)
    system, anims = a.client / "system", a.client / "animations"

    # 1. player animation takes
    with open(out / "player-anim-takes.tsv", "w") as w:
        w.write("package\tanim_set\tsequence\tframes\trate\n")
        for psa in sorted(a.psa_dir.glob("*/MeshAnimation/*.psa")):
            for seq, frames, rate in psa_sequences(psa):
                w.write(f"{psa.parts[-3]}\t{psa.stem}\t{seq}\t{frames}\t{rate:g}\n")

    # 2. effect classes (lineageeffect.u) + every class in every Interlude script package
    effect = load_pkg(system / "lineageeffect.u")
    with open(out / "lineageeffect-classes.tsv", "w") as w:
        w.write("class\tsuper\n")
        for e in sorted((e for e in effect.exports if e["class"] == "Class"), key=lambda e: e["name"].lower()):
            w.write(f"{e['name']}\t{e['super']}\n")
    allc = {}
    engine = None
    for upk in sorted(system.glob("*.u")):
        p = load_pkg(upk)
        if upk.name.lower() == "engine.u":
            engine = p
        for e in p.exports:
            if e["class"] == "Class":
                allc.setdefault(e["name"].lower(), (upk.name, e["name"]))
    with open(out / "script-classes-all-packages.tsv", "w") as w:
        w.write("package\tclass\n")
        for _, (pk, c) in sorted(allc.items()):
            w.write(f"{pk}\t{c}\n")

    # 3. animation names the Interlude engine knows (Get<Name>AnimName natives in Engine.u)
    with open(out / "engine-anim-names.txt", "w") as w:
        for n in sorted({n[3:-8] for n in engine.names if n.startswith("Get") and n.endswith("AnimName")}, key=str.lower):
            w.write(n + "\n")

    # 4. skill.usk: SkillVisualEffect -> SkillAction_LocateEffect -> LineageEffect class
    usk = load_pkg(anims / "skill.usk")
    rows = []
    for e in usk.exports:
        if e["class"] != "SkillVisualEffect":
            continue
        for pname, t, vo, size, _ in usk.props(e["off"], e["off"] + e["size"]):
            if t != 9:
                continue
            cnt, o = usk.ci_at(vo)
            for _ in range(cnt):
                while True:
                    ni, o = usk.ci_at(o)
                    if usk.names[ni] == "None":
                        break
                    info = usk.d[o]; o += 1
                    st = (info >> 4) & 7
                    sz = {0: 1, 1: 2, 2: 4, 3: 12, 4: 16}[st] if st < 5 else usk.d[o]
                    if st == 5:
                        o += 1
                    if usk.names[ni] == "Action":
                        ref, _ = usk.ci_at(o)
                        act = usk.exports[ref - 1]
                        f = {}
                        for an, at, ao, asz, ainfo in usk.props(act["off"], act["off"] + act["size"]):
                            if an == "EffectClass":
                                r, _ = usk.ci_at(ao); f[an] = usk.import_path(r)
                            elif an == "AttachOn":
                                f[an] = EAM[usk.d[ao]]
                            elif an == "AttachBoneName":
                                r, _ = usk.ci_at(ao); f[an] = usk.names[r]
                            elif an == "SpawnDelay":
                                f[an] = "%g" % struct.unpack_from("<f", usk.d, ao)[0]
                            elif at == 3:
                                f[an] = "1" if ainfo & 0x80 else ""
                        rows.append((e["outername"], e["name"], pname, f))
                    o += sz
    key = lambda r: ((int(r[1]), "") if r[1].isdigit() else (10 ** 9, r[1]), r[0], r[2])
    with open(out / "skill-visual-effects.tsv", "w") as w:
        w.write("group\tvisual_id\tphase\teffect_class\tattach_method\tattach_bone\tspawn_delay\tspawn_on_target\tsize_scale\ton_multi_target\n")
        for g, vid, ph, f in sorted(rows, key=key):
            w.write("\t".join([g, vid, ph, f.get("EffectClass", ""), f.get("AttachOn", ""), f.get("AttachBoneName", ""),
                               f.get("SpawnDelay", ""), f.get("bSpawnOnTarget", ""), f.get("bSizeScale", ""),
                               f.get("bOnMultiTarget", "")]) + "\n")
    usk_ids = {r[1] for r in rows}

    # 5. skillgrp.dat (Lineage2Ver413) -> skill id, animation tag, visual reference
    with tempfile.TemporaryDirectory() as td:
        dec = Path(td) / "skillgrp.dec"
        subprocess.run([str(a.l2encdec), "-c", "decode", "-p", "413", "-l", str(system / "skillgrp.dat"), "-o", str(dec)],
                       check=True, capture_output=True)
        sg = parse_skillgrp(dec.read_bytes())
    groups = collections.OrderedDict()
    for sid, lvl, ani, vis in sg:
        groups.setdefault((sid, ani, vis), []).append(lvl)
    with open(out / "skillgrp-visual.tsv", "w") as w:
        w.write("skill_id\tlevels\tani_char\tvisual_ref\tvisual_id\tin_skill_usk\n")
        for (sid, ani, vis), lv in groups.items():
            vid = vis.rsplit(".", 1)[-1] if vis else str(sid)
            w.write(f"{sid}\t{min(lv)}-{max(lv)}\t{ani}\t{vis}\t{vid}\t{1 if vid in usk_ids else 0}\n")
    print(f"takes ok; effect classes {sum(1 for e in effect.exports if e['class'] == 'Class')}; "
          f"skill.usk visuals {len(usk_ids)}; skillgrp rows {len(sg)}", file=sys.stderr)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
