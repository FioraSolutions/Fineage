#!/usr/bin/env python3
"""Extract Interlude Interface.xdat window layouts to JSON.

The binary schema is Interlude / ct0, as documented by the open-source
acmi/xdat_editor_schema (https://github.com/acmi/xdat_editor_schema)
Window / DefaultProperty / control classes. Interface.xdat in a retail
Interlude client is stored unencrypted. sysstring-e.dat is Lineage2Ver413
and must be decoded first (open-l2encdec -p 413 --legacy).

This script emits text-only JSON. It never copies .xdat / .utx / .u binaries
into the output tree.
"""
from __future__ import annotations

import argparse
import json
import struct
import sys
from pathlib import Path
from typing import Any, Callable

ALIGN = [
    "NONE",
    "TOP_LEFT",
    "TOP_CENTER",
    "TOP_RIGHT",
    "CENTER_LEFT",
    "CENTER",
    "CENTER_RIGHT",
    "BOTTOM_LEFT",
    "BOTTOM_CENTER",
    "BOTTOM_RIGHT",
]

TEX_CTRL_TYPE = ["Stretch", "Normal", "Tile", "Draggable", "Control", "Mask"]
TEX_LAYER = ["None", "Normal", "Background"]
TEXT_ALIGN = ["Undefined", "Left", "Center", "Right", "MacroIcon"]
EDIT_TYPE = ["NORMAL", "CHAT", "PASSWORD", "COUNT", "NUMBER", "UNK5", "TYPE6"]

# First four Window strings match later-chronicle (ct26+) names and observed values.
WINDOW_STRING_NAMES = ("parent", "backTex", "script", "state")


class XdatError(RuntimeError):
    pass


class Reader:
    def __init__(self, data: bytes):
        self.data = data
        self.pos = 0

    def remaining(self) -> int:
        return len(self.data) - self.pos

    def _need(self, n: int) -> None:
        if self.pos + n > len(self.data):
            raise XdatError(f"unexpected EOF at {self.pos}, need {n} more bytes")

    def read(self, n: int) -> bytes:
        self._need(n)
        b = self.data[self.pos : self.pos + n]
        self.pos += n
        return b

    def i32(self) -> int:
        return struct.unpack("<i", self.read(4))[0]

    def u32(self) -> int:
        return struct.unpack("<I", self.read(4))[0]

    def f32(self) -> float:
        return struct.unpack("<f", self.read(4))[0]

    def boolean(self) -> bool | None:
        v = self.i32()
        if v == 0:
            return False
        if v == 1:
            return True
        if v == -1:
            return None
        return v  # type: ignore[return-value]

    def compact_int(self) -> int:
        output = 0
        signed = False
        for i in range(5):
            self._need(1)
            x = self.data[self.pos]
            self.pos += 1
            if i == 0:
                if x & 0x80:
                    signed = True
                output |= x & 0x3F
                if (x & 0x40) == 0:
                    break
            elif i == 4:
                output |= (x & 0x1F) << (6 + 3 * 7)
            else:
                output |= (x & 0x7F) << (6 + (i - 1) * 7)
                if (x & 0x80) == 0:
                    break
        return -output if signed else output

    def string(self) -> str:
        length = self.compact_int()
        if length == 0:
            return ""
        if length > 0:
            raw = self.read(length)
            return raw[:-1].decode("ascii", errors="replace")
        raw = self.read(-2 * length)
        return raw[:-2].decode("utf-16le", errors="replace")

    def enum(self, names: list[str], what: str = "enum") -> str:
        v = self.i32()
        if 0 <= v < len(names):
            return names[v]
        return f"{what}({v})"

    def color_argb(self) -> str:
        v = self.u32()
        return f"#{v:08X}"


def _omit_undefined(s: str | None) -> str | None:
    if s is None or s == "" or s == "undefined":
        return None
    return s


def read_default_property(r: Reader) -> dict[str, Any]:
    name = r.string()
    extends_name = r.string()
    always_on_top = r.i32()
    always_on_back = r.i32()
    parent_name = r.string()
    pre_order = r.string()
    post_order = r.string()
    unk7 = r.i32()
    size = r.boolean()
    size_obj: dict[str, Any] | None = None
    if size:
        absolute = r.boolean()
        size_obj = {"absoluteValues": absolute}
        if not absolute:
            r.compact_int()  # unused empty array, per acmi schema
            size_obj["percentWidth"] = r.f32()
            size_obj["percentHeight"] = r.f32()
        size_obj["width"] = r.i32()
        size_obj["height"] = r.i32()
    anchored = r.boolean()
    anchor: dict[str, Any] | None = None
    if anchored:
        anchor = {
            "parentPoint": r.enum(ALIGN, "Alignment"),
            "thisPoint": r.enum(ALIGN, "Alignment"),
            "relativeTo": r.string(),
            "x": r.i32(),
            "y": r.i32(),
        }
    unk22 = r.i32()
    unk23 = r.i32()
    unk24 = r.i32()
    tooltip_type = r.string()
    tooltip_text = r.i32()
    return {
        "name": name,
        "extendsName": extends_name,
        "alwaysOnTop": always_on_top,
        "alwaysOnBack": always_on_back,
        "parentName": parent_name,
        "preOrder": pre_order,
        "postOrder": post_order,
        "unk7": unk7,
        "size": size_obj,
        "anchor": anchor,
        "unk22": unk22,
        "unk23": unk23,
        "unk24": unk24,
        "tooltipType": tooltip_type,
        "tooltipText": tooltip_text,
    }


def _fields(pairs: list[tuple[str, Callable[[], Any]]]) -> dict[str, Any]:
    return {k: fn() for k, fn in pairs}


def read_extra(r: Reader, typ: str) -> dict[str, Any]:
    if typ == "BarCtrl":
        return _fields([
            ("foreTexture", r.string),
            ("backTexture", r.string),
            ("uSize", r.i32),
            ("vSize", r.i32),
        ])
    if typ == "Button":
        return _fields([
            ("normalTex", r.string),
            ("pushedTex", r.string),
            ("highlightTex", r.string),
            ("dropTex", r.string),
            ("buttonName", r.i32),
            ("noHighlight", r.i32),
            ("defaultSoundOn", r.i32),
            ("disableTime", r.i32),
        ])
    if typ == "ChatWindow":
        return {"lineGap": r.i32()}
    if typ == "CheckBox":
        return _fields([
            ("titleIndex", r.i32),
            ("checked", r.boolean),
            ("leftAligned", r.boolean),
            ("maxWidth", r.i32),
            ("checkTexture", r.string),
            ("unCheckTexture", r.string),
            ("disableTexture", r.string),
            ("disableCheckTexture", r.string),
        ])
    if typ == "ComboBox":
        n = r.i32()
        values = []
        for _ in range(n):
            values.append(_fields([
                ("sysString", r.i32),
                ("systemMsg", r.i32),
                ("text", r.string),
                ("reserved", r.i32),
            ]))
        return {"values": values}
    if typ == "EditBox":
        return _fields([
            ("editType", lambda: r.enum(EDIT_TYPE, "EditType")),
            ("maxLength", r.i32),
            ("showCursor", r.i32),
            ("chatMarkOn", r.boolean),
        ])
    if typ == "EffectButton":
        return _fields([
            ("effectType", r.i32),
            ("normalTex", r.string),
            ("pushedTex", r.string),
            ("highlightTex", r.string),
            ("effectTex1", r.string),
            ("effectTex2", r.string),
        ])
    if typ == "FishViewportWindow":
        return _fields([
            ("texBack", r.string),
            ("texClock", r.string),
            ("texFishHPBar", r.string),
            ("texFishHPBarBack", r.string),
            ("texFishFakeHPBarWarning", r.string),
            ("texFishingEffect", r.string),
            ("texIconPumping", r.string),
            ("texIconReeling", r.string),
        ])
    if typ == "HtmlCtrl":
        return {"viewType": r.string()}
    if typ in ("InvenWeight", "MultiSellItemInfo", "MultiSellNeededItem", "NameCtrl", "Radar"):
        return {}
    if typ == "ItemWindow":
        return _fields([
            ("col", r.i32),
            ("row", r.i32),
            ("maxItemNum", r.i32),
            ("iconWidth", r.i32),
            ("iconHeight", r.i32),
            ("gapX", r.i32),
            ("gapY", r.i32),
            ("offsetX", r.i32),
            ("offsetY", r.i32),
            ("backgroundItemWidth", r.i32),
            ("backgroundItemHeight", r.i32),
            ("backgroundItemTex", r.string),
            ("selectedItemWidth", r.i32),
            ("selectedItemHeight", r.i32),
            ("selectedItemTex", r.string),
            ("unselectedItemWidth", r.i32),
            ("unselectedItemHeight", r.i32),
            ("unselectedItemTex", r.string),
            ("noSelectItem", r.i32),
            ("noItemDrag", r.i32),
            ("buttonClick", r.i32),
            ("useCoolTime", r.i32),
            ("noScroll", r.i32),
            ("outLineUp", r.string),
            ("outLineDown", r.string),
        ])
    if typ == "ListCtrl":
        extra = _fields([
            ("unk100", r.i32),
            ("unk101", r.i32),
            ("unk102", r.i32),
            ("unk103", r.i32),
            ("unk104", r.i32),
        ])
        n = r.i32()
        values = []
        for _ in range(n):
            values.append(_fields([
                ("textStringId", r.i32),
                ("width", r.i32),
                ("unk108", r.boolean),
                ("unk109", r.boolean),
                ("unk110", r.boolean),
            ]))
        extra["values"] = values
        return extra
    if typ == "MinimapCtrl":
        return _fields([
            ("showTime", r.boolean),
            ("showTown", r.boolean),
            ("showGrid", r.boolean),
            ("showMyLocMark", r.boolean),
            ("showMyLocText", r.boolean),
            ("showSSQText", r.boolean),
        ])
    if typ == "MultiEdit":
        return {"unk100": r.i32(), "unk101": r.i32()}
    if typ == "Progress":
        return _fields([
            ("backTexture", r.string),
            ("barTexture", r.string),
            ("gap", r.i32),
        ])
    if typ == "RadioButton":
        return _fields([
            ("sysstring", r.i32),
            ("radioGroupID", r.i32),
            ("isChecked", r.i32),
        ])
    if typ == "ScrollArea":
        extra = {"areaHeight": r.i32()}
        n = r.i32()
        extra["children"] = [read_ui_entity(r) for _ in range(n)]
        return extra
    if typ == "ShortcutItemWindow":
        return {"alwaysShowOutline": r.boolean()}
    if typ == "SliderCtrl":
        return _fields([
            ("numOfTick", r.i32),
            ("currTick", r.i32),
            ("thumbBtnWidth", r.i32),
            ("thumbBtnHeight", r.i32),
            ("backTexture", r.string),
            ("disableBackTexture", r.string),
            ("thumbBtnNormalTexture", r.string),
            ("thumbBtnDownTexture", r.string),
            ("pushBtnWidth", r.i32),
            ("pushBtnHeight", r.i32),
            ("pushBtnAutoHitTime", r.i32),
            ("unk111", r.i32),
            ("tickTexture", r.string),
        ])
    if typ == "StatusBar":
        return _fields([
            ("title", r.string),
            ("textureWidth", r.i32),
            ("textureHeight", r.i32),
            ("foreTexture", r.string),
            ("backTexture", r.string),
            ("warnTexture", r.string),
            ("regenTexture", r.string),
        ])
    if typ == "StatusIconCtrl":
        return {"noClip": r.boolean(), "noTooltip": r.boolean()}
    if typ == "Tab":
        n = r.i32()
        tabs = []
        for _ in range(n):
            tabs.append(_fields([
                ("buttonName", r.i32),
                ("target", r.string),
                ("width", r.i32),
                ("height", r.i32),
                ("normalTex", r.string),
                ("pushedTex", r.string),
                ("movable", r.boolean),
                ("gap", r.i32),
                ("tooltip", r.i32),
                ("noHighlight", r.i32),
            ]))
        return {"tabs": tabs}
    if typ == "TextBox":
        return _fields([
            ("text", r.string),
            ("textAlign", lambda: r.enum(TEXT_ALIGN, "TextAlign")),
            ("offsetY", r.i32),
            ("backTex", r.string),
            ("fontType", r.i32),
            ("sysstring", r.i32),
            ("systemMsg", r.i32),
            ("textColor", r.color_argb),
            ("emoticon", r.i32),
            ("autosize", r.i32),
        ])
    if typ == "TextListBox":
        return _fields([
            ("maxRow", r.i32),
            ("showRow", r.i32),
            ("lineGap", r.i32),
            ("isShowScroll", r.i32),
        ])
    if typ == "Texture":
        return _fields([
            ("file", r.string),
            ("texType", lambda: r.enum(TEX_CTRL_TYPE, "TextureCtrlType")),
            ("layer", lambda: r.enum(TEX_LAYER, "TextureLayer")),
            ("u", r.f32),
            ("v", r.f32),
            ("uSize", r.f32),
            ("vSize", r.f32),
            ("alpha", r.i32),
            ("isAnimTex", r.i32),
        ])
    if typ == "TreeCtrl":
        return {"saveExpandedNode": r.boolean(), "multiExpand": r.i32()}
    if typ == "Window":
        return read_window_body(r)
    raise XdatError(f"unknown UI entity type {typ!r} at offset {r.pos}")


def read_window_body(r: Reader) -> dict[str, Any]:
    body: dict[str, Any] = {
        "parent": r.string(),
        "backTex": r.string(),
        "script": r.string(),
        "state": r.string(),
        "unk104": r.i32(),
        "unk105": r.i32(),
        "unk106": r.i32(),
        "unk107": r.i32(),
        "unk108": r.i32(),
        "unk109": r.i32(),
        "unk110": r.i32(),
        "title": r.i32(),  # unk111, sysstring in later chronicles
        "unk112": r.i32(),
        "unk113": r.i32(),
        "unk114": r.i32(),
        "unk115": r.i32(),
        "unk116": r.i32(),
        "unk117": r.i32(),
        "unk118": r.i32(),
        "unk119": r.i32(),
        "unk120": r.f32(),
        "unk121": r.f32(),
        "unk122": r.i32(),
        "unk123": r.i32(),
        "unk124": r.i32(),
        "unk125": r.i32(),
        "unk126": r.i32(),
        "unk127": r.i32(),
        "unk128": r.i32(),
        "unk129": r.string(),
        "unk130": r.i32(),
        "unk131": r.i32(),
        "unk132": r.i32(),
        "unk133": r.i32(),
        "unk134": r.i32(),
        "unk135": r.i32(),
        "unk136": r.string(),
        "unk137": r.i32(),
        "unk138": r.i32(),
        "unk139": r.string(),
    }
    n = r.i32()
    body["children"] = [read_ui_entity(r) for _ in range(n)]
    return body


def read_window(r: Reader) -> dict[str, Any]:
    obj = read_default_property(r)
    obj["type"] = "Window"
    obj.update(read_window_body(r))
    return obj


def read_ui_entity(r: Reader) -> dict[str, Any]:
    typ = r.string()
    obj = read_default_property(r)
    obj["type"] = typ
    obj.update(read_extra(r, typ))
    return obj


def read_shortcut(r: Reader) -> dict[str, Any]:
    unk0 = r.string()
    unk1 = r.string()
    n = r.i32()
    actions = []
    for _ in range(n):
        actions.append({
            "key1": r.i32(),
            "key2": r.i32(),
            "key3": r.i32(),
            "action": r.string(),
        })
    return {"unk0": unk0, "unk1": unk1, "actions": actions}


def read_wnd_def_pos(r: Reader) -> dict[str, Any]:
    return {
        "wnd": r.string(),
        "alignment": r.enum(ALIGN, "Alignment"),
        "x": r.i32(),
        "y": r.i32(),
        "moveParent": r.boolean(),
        "width": r.i32(),
        "height": r.i32(),
    }


def parse_xdat(data: bytes) -> dict[str, Any]:
    r = Reader(data)
    n = r.i32()
    windows = [read_window(r) for _ in range(n)]
    n = r.i32()
    shortcuts = [read_shortcut(r) for _ in range(n)]
    marker = r.i32()
    n = r.i32()
    wnd_def_pos = [read_wnd_def_pos(r) for _ in range(n)]
    tail = r.data[r.pos :]
    return {
        "windows": windows,
        "shortcutCount": len(shortcuts),
        "wndDefPos": wnd_def_pos,
        "marker": marker,
        "tailHex": tail.hex(),
        "bytesConsumed": r.pos,
        "fileSize": len(data),
        "trailingBytes": len(tail),
    }


def parse_sysstring(data: bytes) -> dict[int, str]:
    """Interlude sysstring-e.dat after Lineage2Ver413 decrypt.

    Layout: u32 count, then repeated (u32 id, compact-int length, length
    bytes including a trailing NUL). A trailing "SafePackage" footer is
    ignored.
    """
    r = Reader(data)
    count = r.u32()
    out: dict[int, str] = {}
    for _ in range(count):
        if r.remaining() < 5:
            break
        # Peek: if the leftover looks like the SafePackage footer, stop.
        sid = r.u32()
        try:
            text = r.string()
        except XdatError:
            break
        out[sid] = text
    return out



def _sys_label(sid: int | None, table: dict[int, str] | None) -> str | None:
    if sid is None or table is None:
        return None
    if sid in (-9999, -1, 0):
        return None
    return table.get(sid)


def flatten_controls(
    node: dict[str, Any],
    parent: str | None,
    sysstr: dict[int, str] | None,
    acc: list[dict[str, Any]],
) -> None:
    size = node.get("size") or {}
    anchor = node.get("anchor") or {}
    typ = node.get("type") or "Window"
    name = node.get("name")
    entry: dict[str, Any] = {
        "name": name,
        "type": typ,
        "parent": parent,
        "x": anchor.get("x"),
        "y": anchor.get("y"),
        "width": size.get("width"),
        "height": size.get("height"),
        "sizePercentWidth": size.get("percentWidth"),
        "sizePercentHeight": size.get("percentHeight"),
        "sizeAbsoluteValues": size.get("absoluteValues"),
        "anchorParentPoint": anchor.get("parentPoint"),
        "anchorThisPoint": anchor.get("thisPoint"),
        "relativeTo": _omit_undefined(anchor.get("relativeTo")),
        "extendsName": _omit_undefined(node.get("extendsName")),
        "logicalParentName": _omit_undefined(node.get("parentName")),
    }
    textures: dict[str, Any] = {}
    for key in (
        "normalTex",
        "pushedTex",
        "highlightTex",
        "dropTex",
        "file",
        "backTex",
        "foreTexture",
        "backTexture",
        "warnTexture",
        "regenTexture",
        "checkTexture",
        "unCheckTexture",
        "disableTexture",
        "disableCheckTexture",
        "barTexture",
        "effectTex1",
        "effectTex2",
        "backgroundItemTex",
        "selectedItemTex",
        "unselectedItemTex",
        "outLineUp",
        "outLineDown",
        "thumbBtnNormalTexture",
        "thumbBtnDownTexture",
        "tickTexture",
        "disableBackTexture",
    ):
        val = _omit_undefined(node.get(key))
        if val:
            textures[key] = val
    if node.get("type") == "Texture":
        textures["uv"] = {
            "u": node.get("u"),
            "v": node.get("v"),
            "uSize": node.get("uSize"),
            "vSize": node.get("vSize"),
            "texType": node.get("texType"),
            "layer": node.get("layer"),
            "alpha": node.get("alpha"),
            "isAnimTex": node.get("isAnimTex"),
        }
    if textures:
        entry["textures"] = textures

    if typ == "TextBox":
        entry["text"] = node.get("text") or None
        entry["textAlign"] = node.get("textAlign")
        entry["fontType"] = node.get("fontType")
        entry["fontColor"] = node.get("textColor")
        entry["sysstring"] = node.get("sysstring")
        entry["sysstringText"] = _sys_label(node.get("sysstring"), sysstr)
        entry["systemMsg"] = node.get("systemMsg")
        entry["offsetY"] = node.get("offsetY")
        entry["autosize"] = node.get("autosize")
    if typ == "Button":
        entry["buttonName"] = node.get("buttonName")
        entry["buttonText"] = _sys_label(node.get("buttonName"), sysstr)
        entry["noHighlight"] = node.get("noHighlight")
    if typ == "Tab":
        tabs = []
        for tab in node.get("tabs") or []:
            tabs.append({
                "buttonName": tab.get("buttonName"),
                "buttonText": _sys_label(tab.get("buttonName"), sysstr),
                "target": tab.get("target"),
                "width": tab.get("width"),
                "height": tab.get("height"),
                "normalTex": _omit_undefined(tab.get("normalTex")),
                "pushedTex": _omit_undefined(tab.get("pushedTex")),
                "movable": tab.get("movable"),
                "gap": tab.get("gap"),
                "tooltip": tab.get("tooltip"),
                "tooltipText": _sys_label(tab.get("tooltip"), sysstr),
                "noHighlight": tab.get("noHighlight"),
            })
        entry["tabs"] = tabs
    if typ == "EditBox":
        entry["editType"] = node.get("editType")
        entry["maxLength"] = node.get("maxLength")
        entry["showCursor"] = node.get("showCursor")
        entry["chatMarkOn"] = node.get("chatMarkOn")
    if typ == "StatusBar":
        entry["title"] = node.get("title")
        entry["textureWidth"] = node.get("textureWidth")
        entry["textureHeight"] = node.get("textureHeight")
    if typ == "ChatWindow":
        entry["lineGap"] = node.get("lineGap")
    if typ == "MinimapCtrl":
        for k in ("showTime", "showTown", "showGrid", "showMyLocMark", "showMyLocText", "showSSQText"):
            entry[k] = node.get(k)
    if typ == "Window":
        entry["script"] = _omit_undefined(node.get("script"))
        entry["state"] = _omit_undefined(node.get("state"))
        entry["backTex"] = _omit_undefined(node.get("backTex"))
        entry["windowParent"] = _omit_undefined(node.get("parent"))
        entry["title"] = node.get("title")
        entry["titleText"] = _sys_label(node.get("title"), sysstr)

    # drop empty None-only noise keys except the required geometry ones
    acc.append(entry)
    for child in node.get("children") or []:
        flatten_controls(child, name, sysstr, acc)


def build_window_json(
    window: dict[str, Any],
    defpos: dict[str, Any] | None,
    sysstr: dict[int, str] | None,
    source_note: dict[str, Any],
) -> dict[str, Any]:
    controls: list[dict[str, Any]] = []
    flatten_controls(window, None, sysstr, controls)
    return {
        "source": source_note,
        "wndDefPos": defpos,
        "controls": controls,
        "raw": window,
    }


NEEDED = {
    "CharacterSelect": [],
    "StatusWnd": ["StatusWnd"],
    "TargetStatusWnd": ["TargetStatusWnd"],
    "ChatWnd": ["ChatWnd"],
    "RadarMapWnd": ["RadarContainerWnd"],
    "RadarWnd": ["RadarWnd"],
    "MinimapWnd": ["MinimapWnd"],
    "MinimapWnd_Expand": ["MinimapWnd_Expand"],
    "TownMapWnd": ["TownMapWnd"],
    "AbnormalStatusWnd": ["AbnormalStatusWnd"],
}


def main(argv: list[str] | None = None) -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("xdat", type=Path, help="Path to Interlude Interface.xdat (unencrypted)")
    ap.add_argument("-o", "--out-dir", type=Path, required=True, help="Directory for <Window>.json")
    ap.add_argument("--sysstring", type=Path, help="Decoded sysstring-e.dat (not Ver413-wrapped)")
    ap.add_argument("--all", action="store_true", help="Also write _all_windows_index.json")
    args = ap.parse_args(argv)

    data = args.xdat.read_bytes()
    if data[:22] == bytes([0x4C, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x65, 0x00, 0x61, 0x00, 0x67, 0x00, 0x65, 0x00, 0x32, 0x00, 0x56, 0x00, 0x65, 0x00, 0x72, 0x00]):
        raise SystemExit(
            f"{args.xdat} is Lineage2Ver-encrypted; Interface.xdat for Interlude should be plain. "
            "Decrypt first if this is a later chronicle."
        )
    parsed = parse_xdat(data)
    if parsed["bytesConsumed"] != parsed["fileSize"] and parsed["trailingBytes"] not in (0, 20):
        print(
            f"warning: consumed {parsed['bytesConsumed']} of {parsed['fileSize']} bytes "
            f"(trailing {parsed['trailingBytes']})",
            file=sys.stderr,
        )
    sysstr = parse_sysstring(args.sysstring.read_bytes()) if args.sysstring else {}

    by_name: dict[str, dict[str, Any]] = {}
    for w in parsed["windows"]:
        by_name.setdefault(w["name"], w)
    defpos_by: dict[str, dict[str, Any]] = {d["wnd"]: d for d in parsed["wndDefPos"]}

    args.out_dir.mkdir(parents=True, exist_ok=True)
    source_base = {
        "file": "system/interface.xdat",
        "chronicle": "Interlude (ct0)",
        "schema": "acmi/xdat_editor_schema ct0",
        "xdatBytes": parsed["fileSize"],
        "windowCount": len(parsed["windows"]),
    }

    written = []
    missing = []
    for out_name, aliases in NEEDED.items():
        found = None
        hit = None
        for alias in aliases:
            if alias in by_name:
                found = by_name[alias]
                hit = alias
                break
        if not found:
            missing.append(out_name)
            continue
        payload = build_window_json(
            found,
            defpos_by.get(hit) or defpos_by.get(found["name"]),
            sysstr,
            {**source_base, "window": found["name"], "output": f"{out_name}.json"},
        )
        dest = args.out_dir / f"{out_name}.json"
        dest.write_text(json.dumps(payload, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        written.append(str(dest))

    index = {
        "source": source_base,
        "windowNames": [w["name"] for w in parsed["windows"]],
        "wndDefPos": parsed["wndDefPos"],
        "bytesConsumed": parsed["bytesConsumed"],
        "fileSize": parsed["fileSize"],
        "trailingBytes": parsed["trailingBytes"],
        "marker": parsed["marker"],
        "shortcutCount": parsed["shortcutCount"],
        "extracted": written,
        "notFoundInXdat": missing,
    }
    (args.out_dir / "_index.json").write_text(json.dumps(index, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    if args.all:
        (args.out_dir / "_all_windows_index.json").write_text(
            json.dumps(
                {
                    "windows": [
                        {
                            "name": w["name"],
                            "size": w.get("size"),
                            "anchor": w.get("anchor"),
                            "state": w.get("state"),
                            "script": w.get("script"),
                            "backTex": w.get("backTex"),
                            "childCount": len(w.get("children") or []),
                        }
                        for w in parsed["windows"]
                    ]
                },
                indent=2,
                ensure_ascii=False,
            )
            + "\n",
            encoding="utf-8",
        )
    print(f"parsed {len(parsed['windows'])} windows, wrote {len(written)} layouts, missing={missing}")
    print(f"consumed {parsed['bytesConsumed']}/{parsed['fileSize']} bytes, tail={parsed['trailingBytes']}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
