#!/usr/bin/env python3
"""Parse decrypted Interlude DAT files using l2asm-disasm DDF schemas."""

from __future__ import annotations

import json
import re
import struct
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any


class ParseError(Exception):
    pass

SAFE_PACKAGE_TAIL = b"\x0cSafePackage\x00"


def _strip_comments(text: str) -> str:
    text = re.sub(r"/\*.*?\*/", "", text, flags=re.S)
    out = []
    for line in text.splitlines():
        in_str = False
        buf = []
        i = 0
        while i < len(line):
            c = line[i]
            if c == '"' and not in_str:
                in_str = True
                buf.append(c)
            elif c == '"' and in_str:
                in_str = False
                buf.append(c)
            elif not in_str and c == "#" and (i == 0 or line[i - 1].isspace()):
                break
            elif not in_str and c == "/" and i + 1 < len(line) and line[i + 1] == "/":
                break
            else:
                buf.append(c)
            i += 1
        out.append("".join(buf))
    return "\n".join(out)


def _eval_literal(tok: str) -> Any:
    t = tok.strip()
    if t in ("YES",):
        return 1
    if t in ("NO",):
        return 0
    if t in ("OFF",):
        return -1
    if t.startswith('"') and t.endswith('"'):
        s = t[1:-1].replace("\\t", "\t")
        return s
    if re.fullmatch(r"-?\d+", t):
        return int(t)
    return t


@dataclass
class Field:
    typ: str
    name: str
    count: str | int | None = None  # None, int, or field name
    filler_size: int | None = None
    enbby: list[list[tuple[str, int | None, int]]] = field(default_factory=list)
    # enbby: list of OR-groups; each group is AND of (ident, mask_or_None, value)


@dataclass
class Schema:
    path: Path
    fs: str = "\t"
    reccnt: int = -1  # -1 == OFF == count is first UINT
    header: int = 1
    fields: list[Field] = field(default_factory=list)


def parse_ddf(path: Path) -> Schema:
    raw = _strip_comments(path.read_text(encoding="utf-8", errors="replace"))
    schema = Schema(path=path)
    # control vars
    for m in re.finditer(r"^\s*([A-Z_]+)\s*=\s*(.+?)\s*;", raw, re.M):
        key, val = m.group(1), _eval_literal(m.group(2))
        if key == "FS":
            schema.fs = str(val)
        elif key == "RECCNT":
            schema.reccnt = int(val)
        elif key == "HEADER":
            schema.header = 1 if val in (1, "YES", True) or str(val) == "1" else 0
    body = raw[raw.find("{") + 1 : raw.rfind("}")]
    # split into statements at semicolons, but keep property attachments
    tokens = []
    i = 0
    cur = []
    while i < len(body):
        c = body[i]
        if c == ";":
            tokens.append("".join(cur).strip())
            cur = []
            i += 1
            continue
        cur.append(c)
        i += 1
    if "".join(cur).strip():
        tokens.append("".join(cur).strip())

    field_re = re.compile(
        r"^(UINT|HEX|INT|UWORD|WORD|UCHAR|CHEX|CHAR|FLOAT|UNICODE|ASCF|MAT|MAT2|MTX|MTX2|MTX3|FILLER|CNTR)"
        r"\s+([^\s\[\]{};,]+)"
        r"(?:\[([^\]]+)\])?"
        r"(?:\{(\d+)\})?"
        r"\s*$",
        re.I,
    )
    enbby_re = re.compile(r"ENBBY\s*=\s*\[(.+)\]\s*$", re.I | re.S)
    current: Field | None = None
    for tok in tokens:
        if not tok:
            continue
        m = field_re.match(tok)
        if m:
            typ = m.group(1).upper()
            name = m.group(2)
            cnt_raw = m.group(3)
            fill = m.group(4)
            count: str | int | None = None
            if cnt_raw is not None:
                cnt_raw = cnt_raw.strip()
                count = int(cnt_raw) if re.fullmatch(r"-?\d+", cnt_raw) else cnt_raw
            current = Field(
                typ=typ,
                name=name,
                count=count,
                filler_size=int(fill) if fill else None,
            )
            schema.fields.append(current)
            continue
        # property of previous field
        if current is None:
            continue
        em = enbby_re.match(tok)
        if em:
            group: list[tuple[str, int | None, int]] = []
            inner = em.group(1)
            # (ident,val) or (ident:mask,val)
            for part in re.finditer(r"\(\s*([A-Za-z_][\w?]*)(?::(\d+))?\s*,\s*(-?\d+)\s*\)", inner):
                ident, mask, val = part.group(1), part.group(2), int(part.group(3))
                group.append((ident, int(mask) if mask is not None else None, val))
            if group:
                current.enbby.append(group)
    if not schema.fields:
        raise ParseError(f"no fields in {path}")
    return schema


def read_cntr(data: bytes, o: int, *, unicode_flag: bool = False) -> tuple[int, bool, int]:
    """Packed counter used by CNTR and ASCF.

    First byte: bits 0-5 = value, bit 6 = continuation, bit 7 = ASCF unicode flag.
    Later bytes: bits 0-6 = value, bit 7 = continuation (expanded 7-bit chunks).
    """
    if o >= len(data):
        raise ParseError(f"EOF at CNTR offset {o}")
    b = data[o]
    o += 1
    is_uni = bool(b & 0x80) if unicode_flag else False
    value = b & 0x3F
    shift = 6
    cont = bool(b & 0x40)
    while cont:
        if o >= len(data):
            raise ParseError("EOF in CNTR continuation")
        b = data[o]
        o += 1
        value |= (b & 0x7F) << shift
        shift += 7
        cont = bool(b & 0x80)
        if shift > 32 + 7:
            raise ParseError("CNTR overflow")
    return value, is_uni, o


def read_u32(data: bytes, o: int) -> tuple[int, int]:
    if o + 4 > len(data):
        raise ParseError(f"EOF UINT at {o}")
    return struct.unpack_from("<I", data, o)[0], o + 4


def read_i32(data: bytes, o: int) -> tuple[int, int]:
    if o + 4 > len(data):
        raise ParseError(f"EOF INT at {o}")
    return struct.unpack_from("<i", data, o)[0], o + 4


def read_u16(data: bytes, o: int) -> tuple[int, int]:
    if o + 2 > len(data):
        raise ParseError(f"EOF UWORD at {o}")
    return struct.unpack_from("<H", data, o)[0], o + 2


def read_i16(data: bytes, o: int) -> tuple[int, int]:
    if o + 2 > len(data):
        raise ParseError(f"EOF WORD at {o}")
    return struct.unpack_from("<h", data, o)[0], o + 2


def read_f32(data: bytes, o: int) -> tuple[float, int]:
    if o + 4 > len(data):
        raise ParseError(f"EOF FLOAT at {o}")
    u = struct.unpack_from("<I", data, o)[0]
    f = struct.unpack_from("<f", data, o)[0]
    # preserve NaN payload like l2disasm if needed
    if (u & 0x7FFFFFFF) > 0x7F800000:
        return float("nan"), o + 4
    return f, o + 4


def read_ascii_or_uni(data: bytes, o: int) -> tuple[str, int]:
    n, is_uni, o = read_cntr(data, o, unicode_flag=True)
    if n == 0:
        return "", o
    # When the unicode flag is set, CNTR is the UTF-16 code-unit count (incl. trailing NUL).
    nbytes = n * 2 if is_uni else n
    if o + nbytes > len(data):
        raise ParseError(f"ASCF length {n} ({nbytes} bytes, uni={is_uni}) overruns at {o}")
    raw = data[o : o + nbytes]
    o += nbytes
    if is_uni:
        return raw.decode("utf-16le", errors="replace").rstrip("\x00"), o
    return raw.split(b"\x00", 1)[0].decode("latin-1", errors="replace"), o


def read_unicode(data: bytes, o: int) -> tuple[str, int]:
    n, o = read_i32(data, o)
    if n < 0:
        raise ParseError(f"negative UNICODE length {n} at {o-4}")
    if o + n > len(data):
        raise ParseError(f"UNICODE length {n} overruns at {o}")
    raw = data[o : o + n]
    o += n
    if n == 0:
        return "", o
    if n % 2:
        # some files pad; try anyway
        raw = raw + b"\x00"
    return raw.decode("utf-16le", errors="replace").rstrip("\x00"), o


def _as_int(val: Any) -> int:
    if isinstance(val, bool):
        return int(val)
    if isinstance(val, int):
        return val
    if isinstance(val, list) and val:
        return _as_int(val[0])
    if isinstance(val, str) and re.fullmatch(r"-?\d+", val):
        return int(val)
    return 0


def _enabled(field: Field, rec: dict[str, Any]) -> bool:
    if not field.enbby:
        return True
    for group in field.enbby:
        ok = True
        for ident, mask, expect in group:
            cur = _as_int(rec.get(ident, 0))
            if mask is not None:
                if (cur & mask) != expect:
                    ok = False
                    break
            elif cur != expect:
                ok = False
                break
        if ok:
            return True
    return False


def _resolve_count(count: str | int | None, rec: dict[str, Any]) -> int:
    if count is None:
        return 1
    if isinstance(count, int):
        return count
    return _as_int(rec.get(count, 0))


def _read_mtx(kind: str, data: bytes, o: int) -> tuple[Any, int]:
    # MTX / MTX2 / MTX3 as in l2asm MANUAL
    cntm, o = read_i32(data, o)
    meshes: list[Any] = []
    for _ in range(cntm):
        if kind == "MTX":
            s, o = read_unicode(data, o)
            meshes.append(s)
        elif kind == "MTX2":
            s, o = read_unicode(data, o)
            v1, o = read_u32(data, o)
            v2 = data[o]
            o += 1
            meshes.append({"mesh": s, "val1": v1, "val2": v2})
        elif kind == "MTX3":
            s, o = read_unicode(data, o)
            v0, v1 = data[o], data[o + 1]
            o += 2
            meshes.append({"mesh": s, "val": [v0, v1]})
        else:
            raise ParseError(kind)
    cntt, o = read_i32(data, o)
    texts = []
    for _ in range(cntt):
        s, o = read_unicode(data, o)
        texts.append(s)
    extra = None
    if kind == "MTX3":
        extra, o = read_unicode(data, o)
    out: dict[str, Any] = {"mesh": meshes, "tex": texts}
    if extra is not None:
        out["texext"] = extra
    return out, o


def _read_mat(kind: str, data: bytes, o: int) -> tuple[Any, int]:
    cnt, o = read_i32(data, o)
    extra = None
    if kind == "MAT2":
        extra, o = read_i32(data, o)
    mats = []
    for _ in range(cnt):
        i, o = read_i32(data, o)
        v, o = read_i32(data, o)
        mats.append({"id": i, "val": v})
    out: dict[str, Any] = {"mats": mats}
    if extra is not None:
        out["extra"] = extra
    return out, o


def _read_scalar(typ: str, data: bytes, o: int) -> tuple[Any, int]:
    if typ in ("UINT", "HEX"):
        v, o = read_u32(data, o)
        return v, o
    if typ == "INT":
        return read_i32(data, o)
    if typ == "UWORD":
        return read_u16(data, o)
    if typ == "WORD":
        return read_i16(data, o)
    if typ == "UCHAR" or typ == "CHEX":
        if o >= len(data):
            raise ParseError("EOF UCHAR")
        return data[o], o + 1
    if typ == "CHAR":
        if o >= len(data):
            raise ParseError("EOF CHAR")
        v = struct.unpack_from("<b", data, o)[0]
        return v, o + 1
    if typ == "FLOAT":
        return read_f32(data, o)
    if typ == "UNICODE":
        return read_unicode(data, o)
    if typ == "ASCF":
        return read_ascii_or_uni(data, o)
    if typ == "CNTR":
        n, _, o = read_cntr(data, o, unicode_flag=False)
        return n, o
    if typ in ("MTX", "MTX2", "MTX3"):
        return _read_mtx(typ, data, o)
    if typ in ("MAT", "MAT2"):
        return _read_mat(typ, data, o)
    raise ParseError(f"unsupported type {typ}")


def parse_dat(schema: Schema, data: bytes) -> tuple[list[dict[str, Any]], int]:
    if data.endswith(SAFE_PACKAGE_TAIL):
        data = data[: -len(SAFE_PACKAGE_TAIL)]
    o = 0
    if schema.reccnt == -1:
        count, o = read_u32(data, o)
    else:
        count = schema.reccnt
    rows: list[dict[str, Any]] = []
    for i in range(count):
        rec: dict[str, Any] = {}
        try:
            for fld in schema.fields:
                if not _enabled(fld, rec):
                    continue
                if fld.typ == "FILLER":
                    n = fld.filler_size or 0
                    rec[fld.name] = data[o : o + n].hex()
                    o += n
                    continue
                n = _resolve_count(fld.count, rec) if fld.count is not None else None
                if n is None:
                    val, o = _read_scalar(fld.typ, data, o)
                    rec[fld.name] = val
                else:
                    if n < 0:
                        raise ParseError(f"negative count {n} for {fld.name}")
                    arr = []
                    for _ in range(n):
                        val, o = _read_scalar(fld.typ, data, o)
                        arr.append(val)
                    rec[fld.name] = arr
        except ParseError as e:
            raise ParseError(f"{schema.path.name} record {i}/{count} at {o}: {e}") from e
        rows.append(rec)
    # Some files under-count: extra records sit after the stated RECCNT.
    while o < len(data):
        try:
            rec = {}
            oo = o
            for fld in schema.fields:
                if not _enabled(fld, rec):
                    continue
                if fld.typ == "FILLER":
                    n = fld.filler_size or 0
                    rec[fld.name] = data[oo : oo + n].hex()
                    oo += n
                    continue
                n = _resolve_count(fld.count, rec) if fld.count is not None else None
                if n is None:
                    val, oo = _read_scalar(fld.typ, data, oo)
                    rec[fld.name] = val
                else:
                    if n < 0:
                        raise ParseError(f"negative count {n} for {fld.name}")
                    arr = []
                    for _ in range(n):
                        val, oo = _read_scalar(fld.typ, data, oo)
                        arr.append(val)
                    rec[fld.name] = arr
            if oo > len(data) or oo == o:
                break
            def _empty(val):
                if val is None or val == "" or val == [] or val == {}:
                    return True
                if isinstance(val, str) and not val.strip():
                    return True
                if isinstance(val, (int, float)) and val == 0:
                    return True
                return False
            if rec and all(_empty(v) for v in rec.values()):
                break
            rows.append(rec)
            o = oo
        except ParseError:
            break
    leftover = len(data) - o
    return rows, leftover


def _cell(val: Any) -> str:
    if val is None:
        return ""
    if isinstance(val, float):
        if val != val:
            return "NaN"
        s = f"{val:.6f}".rstrip("0").rstrip(".")
        return s
    if isinstance(val, (list, dict)):
        return json.dumps(val, ensure_ascii=False, separators=(",", ":"))
    s = str(val)
    return s.replace("\t", "\\t").replace("\n", "\\n").replace("\r", "\\r")


def rows_to_tsv(rows: list[dict[str, Any]], field_names: list[str]) -> str:
    lines = ["\t".join(field_names)]
    for rec in rows:
        lines.append("\t".join(_cell(rec.get(name, "")) for name in field_names))
    lines.append("")
    return "\n".join(lines)


def schema_columns(schema: Schema) -> list[str]:
    return [f.name for f in schema.fields if f.typ != "FILLER"]
