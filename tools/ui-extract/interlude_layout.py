"""Compile extracted window JSON into the runtime layout asset.

The runtime (`InterludeLayoutGeometry`) places controls with the same rules
as `resolve_tree` below. Negative or zero width/height means
parentSize + value. -9999 means the extracted control has no size.
"""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any

UNSPECIFIED = -9999
ANCHORS = {
    "TOP_LEFT": (0.0, 0.0),
    "TOP_CENTER": (0.5, 0.0),
    "TOP_RIGHT": (1.0, 0.0),
    "CENTER_LEFT": (0.0, 0.5),
    "CENTER": (0.5, 0.5),
    "CENTER_RIGHT": (1.0, 0.5),
    "BOTTOM_LEFT": (0.0, 1.0),
    "BOTTOM_CENTER": (0.5, 1.0),
    "BOTTOM_RIGHT": (1.0, 1.0),
}

_SKIP_FILES = {
    "_index.json",
    "textures.json",
    "textures-map.json",
    "textures-missing.txt",
}

_TEXTURE_KEYS = (
    ("file", "texture"),
    ("backTex", "texture"),
    ("foreTexture", "texture"),
    ("normalTex", "texture"),
    ("backTexture", "textureBack"),
    ("pushedTex", "texturePushed"),
    ("warnTexture", "textureWarn"),
    ("regenTexture", "textureRegen"),
)


def axis_size(value: float, parent: float) -> float | None:
    """Return a pixel size, or None when the extract left the size unset."""
    if value <= -9000:
        return None
    if value <= 0:
        return parent + value
    return value


def resolve_tree(window: dict[str, Any]) -> dict[str, tuple[float, float, float | None, float | None]]:
    """Parent-space rects (left, top, width, height) for every control."""
    controls = window["controls"]
    rects: dict[str, tuple[float, float, float | None, float | None]] = {}
    root = controls[0]
    rects[root["name"]] = (0.0, 0.0, float(root["width"]), float(root["height"]))
    by_parent_space: dict[str, str] = {root["name"]: root["name"]}

    for control in controls[1:]:
        parent_name = control["parent"]
        if parent_name not in rects:
            raise ValueError(
                f"{window['name']} control '{control['name']}' parent '{parent_name}' "
                "is not an earlier control"
            )
        parent_rect = rects[parent_name]
        parent_w = parent_rect[2]
        parent_h = parent_rect[3]
        if parent_w is None or parent_h is None or parent_w <= 0 or parent_h <= 0:
            raise ValueError(
                f"{window['name']} control '{control['name']}' parent '{parent_name}' "
                f"has no positive size ({parent_w}x{parent_h})"
            )
        width = axis_size(float(control["width"]), parent_w)
        height = axis_size(float(control["height"]), parent_h)
        ref = (0.0, 0.0, parent_w, parent_h)
        relative = control.get("relativeTo") or ""
        if relative:
            ref_name = relative
            if ref_name not in rects and "." in ref_name:
                ref_name = ref_name.split(".")[-1]
            if ref_name not in rects:
                raise ValueError(
                    f"{window['name']} control '{control['name']}' relativeTo '{relative}' "
                    "was not resolved"
                )
            if by_parent_space[ref_name] != parent_name and ref_name != parent_name:
                raise ValueError(
                    f"{window['name']} control '{control['name']}' relativeTo '{relative}' "
                    "is not in the same parent"
                )
            ref = rects[ref_name] if ref_name != parent_name else (0.0, 0.0, parent_w, parent_h)
        this_point = ANCHORS[control["anchorThis"]]
        parent_point = ANCHORS[control["anchorParent"]]
        anchor_x = ref[0] + parent_point[0] * ref[2] + float(control["x"])
        anchor_y = ref[1] + parent_point[1] * ref[3] + float(control["y"])
        used_w = width if width is not None else 0.0
        used_h = height if height is not None else 0.0
        left = anchor_x - this_point[0] * used_w
        top = anchor_y - this_point[1] * used_h
        rects[control["name"]] = (left, top, width, height)
        by_parent_space[control["name"]] = parent_name
    return rects


def _clean(value: Any) -> str:
    if value is None:
        return ""
    text = str(value).strip()
    if text in {"", "undefined", "null", "None"}:
        return ""
    return text


def _num(value: Any, fallback: float = UNSPECIFIED) -> float:
    if value is None or value == "":
        return fallback
    return float(value)


def _resource_key(unity_path: str) -> str:
    marker = "Assets/Resources/"
    index = unity_path.find(marker)
    if index < 0:
        raise ValueError(f"texture path is not under Assets/Resources: {unity_path}")
    rel = unity_path[index + len(marker) :]
    if rel.endswith(".png"):
        rel = rel[: -len(".png")]
    return rel


def _load_texture_map(path: Path) -> dict[str, dict[str, Any]]:
    data = json.loads(path.read_text(encoding="utf-8"))
    out: dict[str, dict[str, Any]] = {}
    for entry in data.get("textures") or []:
        unreal = _clean(entry.get("unreal"))
        if not unreal:
            continue
        out[unreal.casefold()] = entry
    return out


def _texture_entry(tex_map: dict[str, dict[str, Any]], unreal: str) -> dict[str, Any] | None:
    return tex_map.get(unreal.casefold())


class CompileError(Exception):
    def __init__(self, errors: list[str]):
        super().__init__("\n".join(errors))
        self.errors = errors


def _flatten_tabs(control: dict[str, Any]) -> list[dict[str, Any]]:
    tabs = control.get("tabs")
    if control.get("type") != "Tab" or not isinstance(tabs, list):
        return []
    children: list[dict[str, Any]] = []
    cursor = 0.0
    for index, tab in enumerate(tabs):
        target = _clean(tab.get("target")) or str(index)
        width = _num(tab.get("width"), UNSPECIFIED)
        gap = _num(tab.get("gap"), 0.0)
        if gap <= -9000:
            gap = 0.0
        children.append(
            {
                "name": f"{control['name']}_{target}",
                "type": "TabButton",
                "parent": control["name"],
                "x": cursor,
                "y": 0,
                "width": width,
                "height": _num(tab.get("height"), UNSPECIFIED),
                "anchorParentPoint": "TOP_LEFT",
                "anchorThisPoint": "TOP_LEFT",
                "relativeTo": "",
                "textures": {
                    "normalTex": tab.get("normalTex"),
                    "pushedTex": tab.get("pushedTex"),
                },
                "buttonText": tab.get("buttonText"),
            }
        )
        if width > -9000:
            cursor += width + max(gap, 0.0)
    return children


def _apply_extends(controls: list[dict[str, Any]], errors: list[str], source_name: str) -> None:
    """xdat extendsName inherits unset size (and only unset size) from the named control."""
    by_name = {c.get("name"): c for c in controls if c.get("name")}
    for control in controls:
        extended = _clean(control.get("extendsName"))
        if not extended:
            continue
        ref_name = extended.split(".")[-1]
        source = by_name.get(ref_name)
        if source is None:
            errors.append(
                f"{source_name}: control '{control.get('name')}' extends '{extended}' "
                "but that control is not in the window"
            )
            continue
        for key in ("width", "height"):
            if control.get(key) is None and source.get(key) is not None:
                control[key] = source[key]


def _anchor_name(value: Any, errors: list[str], where: str) -> str:
    text = _clean(value) or "TOP_LEFT"
    if text == "NONE":
        # xdat Alignment.NONE is an origin offset, the same point as TOP_LEFT.
        return "TOP_LEFT"
    if text not in ANCHORS:
        errors.append(f"{where}: anchor '{text}' is not an extracted alignment")
        return "TOP_LEFT"
    return text


def _iter_source_controls(doc: dict[str, Any]) -> list[dict[str, Any]]:
    controls = doc.get("controls")
    if isinstance(controls, list) and controls:
        flat: list[dict[str, Any]] = []
        for control in controls:
            if control.get("type") == "TextStyle":
                continue
            flat.append(control)
            flat.extend(_flatten_tabs(control))
        return flat
    windows = doc.get("windows")
    if isinstance(windows, list):
        flat = []
        for window in windows:
            node = {
                "name": window.get("name"),
                "type": window.get("type") or "Window",
                "parent": window.get("parent"),
                "x": window.get("x"),
                "y": window.get("y"),
                "width": window.get("width"),
                "height": window.get("height"),
                "anchorParentPoint": window.get("anchorParentPoint"),
                "anchorThisPoint": window.get("anchorThisPoint"),
                "relativeTo": window.get("relativeTo"),
                "textures": {"backTex": window.get("backTex")} if window.get("backTex") else {},
            }
            flat.append(node)
            for child in window.get("controls") or []:
                if child.get("type") == "TextStyle":
                    continue
                flat.append(child)
                flat.extend(_flatten_tabs(child))
        return flat
    return []


def _window_groups(controls: list[dict[str, Any]]) -> list[list[dict[str, Any]]]:
    names = {c.get("name") for c in controls}
    roots = [
        c
        for c in controls
        if not c.get("parent") or c.get("parent") not in names
    ]
    if not roots:
        return []
    children: dict[str, list[dict[str, Any]]] = {}
    for control in controls:
        parent = control.get("parent")
        children.setdefault(parent, []).append(control)

    def walk(node: dict[str, Any]) -> list[dict[str, Any]]:
        out = [node]
        for child in children.get(node.get("name"), []):
            if child is node:
                continue
            out.extend(walk(child))
        return out

    return [walk(root) for root in roots]


def _placement(doc: dict[str, Any], root: dict[str, Any]) -> tuple[int, str, float, float]:
    """Screen anchor. Native lobby windows leave this unset (computed at runtime)."""
    wnd_def = doc.get("wndDefPos")
    if isinstance(wnd_def, dict) and wnd_def.get("wnd") == root.get("name"):
        return (
            1,
            _clean(wnd_def.get("alignment")) or "TOP_LEFT",
            _num(wnd_def.get("x"), 0.0),
            _num(wnd_def.get("y"), 0.0),
        )
    parent = root.get("parent")
    anchor = _clean(root.get("anchorParentPoint"))
    if anchor and parent in (None, "", "undefined"):
        return 1, anchor, _num(root.get("x"), 0.0), _num(root.get("y"), 0.0)
    return 0, "", 0.0, 0.0


def _emit_control(
    control: dict[str, Any],
    *,
    tex_map: dict[str, dict[str, Any]],
    errors: list[str],
    window_name: str,
    force_size: tuple[float, float] | None = None,
    zero_offset: bool = False,
) -> dict[str, Any]:
    textures = control.get("textures") if isinstance(control.get("textures"), dict) else {}
    slots = {
        "texture": "",
        "textureBack": "",
        "texturePushed": "",
        "textureWarn": "",
        "textureRegen": "",
    }
    for source_key, slot in _TEXTURE_KEYS:
        unreal = _clean(textures.get(source_key))
        if not unreal:
            unreal = _clean(control.get(source_key))
        if not unreal:
            continue
        entry = _texture_entry(tex_map, unreal)
        if entry is None:
            errors.append(
                f"{window_name} control '{control.get('name')}' texture '{unreal}' "
                "is not in textures-map.json"
            )
            continue
        paths = entry.get("unityPaths") or []
        if not paths:
            errors.append(
                f"{window_name} control '{control.get('name')}' texture '{unreal}' "
                "has no Unity path in textures-map.json"
            )
            continue
        try:
            slots[slot] = _resource_key(paths[0])
        except ValueError as exc:
            errors.append(f"{window_name} control '{control.get('name')}': {exc}")

    text = _clean(control.get("buttonText")) or _clean(control.get("sysstringText"))
    if not text:
        text = _clean(control.get("text"))
    if not text and not isinstance(control.get("title"), (int, float)):
        text = _clean(control.get("title"))

    width = _num(control.get("width"))
    height = _num(control.get("height"))
    if force_size is not None:
        width, height = force_size
    return {
        "name": control.get("name") or "",
        "type": control.get("type") or "",
        "parent": "" if zero_offset else (_clean(control.get("parent"))),
        "x": 0.0 if zero_offset else _num(control.get("x"), 0.0),
        "y": 0.0 if zero_offset else _num(control.get("y"), 0.0),
        "width": width,
        "height": height,
        "anchorParent": "TOP_LEFT"
        if zero_offset
        else _anchor_name(control.get("anchorParentPoint"), errors, f"{window_name} control '{control.get('name')}'"),
        "anchorThis": "TOP_LEFT"
        if zero_offset
        else _anchor_name(control.get("anchorThisPoint"), errors, f"{window_name} control '{control.get('name')}'"),
        "relativeTo": "" if zero_offset else _clean(control.get("relativeTo")),
        "texture": slots["texture"],
        "textureBack": slots["textureBack"],
        "texturePushed": slots["texturePushed"],
        "textureWarn": slots["textureWarn"],
        "textureRegen": slots["textureRegen"],
        "text": text,
        "textAlign": _clean(control.get("textAlign")),
        "fontColor": _clean(control.get("fontColor")),
        "fontType": int(_num(control.get("fontType"), UNSPECIFIED)),
    }


def _positive_size(control: dict[str, Any], tex_map: dict[str, dict[str, Any]]) -> tuple[float, float] | None:
    width = control.get("width")
    height = control.get("height")
    if isinstance(width, (int, float)) and isinstance(height, (int, float)) and width > 0 and height > 0:
        return float(width), float(height)
    textures = control.get("textures") if isinstance(control.get("textures"), dict) else {}
    unreal = _clean(textures.get("backTex")) or _clean(textures.get("file")) or _clean(control.get("backTex"))
    if not unreal:
        return None
    entry = _texture_entry(tex_map, unreal)
    if entry is None:
        return None
    tw = entry.get("width")
    th = entry.get("height")
    if isinstance(tw, (int, float)) and isinstance(th, (int, float)) and tw > 0 and th > 0:
        return float(tw), float(th)
    return None


def _fill_missing_child_size(
    control: dict[str, Any],
    tex_map: dict[str, dict[str, Any]],
    errors: list[str],
    window_name: str,
) -> None:
    if control.get("width") is not None and control.get("height") is not None:
        return
    filled = _positive_size(control, tex_map)
    if filled is None:
        if control.get("width") is None:
            control["width"] = UNSPECIFIED
        if control.get("height") is None:
            control["height"] = UNSPECIFIED
        return
    if control.get("width") is None:
        control["width"] = filled[0]
    if control.get("height") is None:
        control["height"] = filled[1]


def compile_docs(docs_dir: Path, texture_map_path: Path) -> dict[str, Any]:
    tex_map = _load_texture_map(texture_map_path)
    errors: list[str] = []
    windows: list[dict[str, Any]] = []
    seen: set[str] = set()

    for path in sorted(docs_dir.glob("*.json")):
        if path.name in _SKIP_FILES:
            continue
        doc = json.loads(path.read_text(encoding="utf-8"))
        controls = _iter_source_controls(doc)
        if not controls:
            continue
        _apply_extends(controls, errors, path.name)
        for group in _window_groups(controls):
            root = group[0]
            name = root.get("name") or ""
            if not name:
                errors.append(f"{path.name}: a window root has no name")
                continue
            if name in seen:
                errors.append(f"duplicate window '{name}' (also in {path.name})")
                continue
            seen.add(name)
            size = _positive_size(root, tex_map)
            stretch = 0
            if size is None and root.get("width") == 0 and root.get("height") == 0:
                # Fullscreen window: both axes stretch with the screen.
                size = (0.0, 0.0)
                stretch = 1
            if size is None:
                errors.append(
                    f"{name}: extracted layout has no positive window size "
                    f"(width={root.get('width')}, height={root.get('height')}, "
                    "and no sized back texture in textures-map.json)"
                )
                continue
            known, alignment, place_x, place_y = _placement(doc, root)
            emitted = [
                _emit_control(
                    root,
                    tex_map=tex_map,
                    errors=errors,
                    window_name=name,
                    force_size=size,
                    zero_offset=True,
                )
            ]
            for child in group[1:]:
                _fill_missing_child_size(child, tex_map, errors, name)
                emitted.append(
                    _emit_control(
                        child,
                        tex_map=tex_map,
                        errors=errors,
                        window_name=name,
                    )
                )
            windows.append(
                {
                    "name": name,
                    "alignment": alignment,
                    "placeX": place_x,
                    "placeY": place_y,
                    "placementKnown": known,
                    "stretchToParent": stretch,
                    "controls": emitted,
                }
            )

    if errors:
        raise CompileError(errors)
    windows.sort(key=lambda item: item["name"])
    return {"windows": windows}


def self_test(docs_dir: Path, texture_map_path: Path) -> None:
    doc = compile_docs(docs_dir, texture_map_path)
    by_name = {window["name"]: window for window in doc["windows"]}

    def rect(window_name: str, control_name: str) -> tuple[float, float, float | None, float | None]:
        return resolve_tree(by_name[window_name])[control_name]

    def expect(window_name: str, control_name: str, left: float, top: float, width: float, height: float) -> None:
        got = rect(window_name, control_name)
        if got != (left, top, width, height):
            raise SystemExit(
                f"{window_name}/{control_name}: expected {(left, top, width, height)} got {got}"
            )

    expect("StatusWnd", "StatusWnd", 0, 0, 176, 84)
    expect("StatusWnd", "CPBar", 16, 27, 150, 12)
    expect("StatusWnd", "HPBar", 16, 41, 150, 12)
    expect("StatusWnd", "MPBar", 16, 55, 150, 12)
    expect("StatusWnd", "EXPBar", 16, 69, 150, 12)
    expect("StatusWnd", "StatusWndCenterTex", 28, 0, 144, 84)
    expect("StatusWnd", "StatusWndRightTex", 168, 0, 4, 84)
    expect("StatusWnd", "StatusWnd_LevelTextBox_back", 15, 6, 22, 20)
    status = by_name["StatusWnd"]
    if status["alignment"] != "TOP_LEFT" or status["placeX"] != 0 or status["placeY"] != 0:
        raise SystemExit(f"StatusWnd placement {status['alignment']} {status['placeX']},{status['placeY']}")

    abnormal = by_name["AbnormalStatusWnd"]
    if abnormal["placeX"] != 176 or abnormal["placeY"] != 0 or abnormal["alignment"] != "TOP_LEFT":
        raise SystemExit(
            f"AbnormalStatusWnd placement {abnormal['alignment']} {abnormal['placeX']},{abnormal['placeY']}"
        )
    expect("AbnormalStatusWnd", "StatusIcon", 12, 0, 26, 26)

    target = by_name["TargetStatusWnd"]
    if target["alignment"] != "TOP_CENTER":
        raise SystemExit(f"TargetStatusWnd alignment {target['alignment']}")
    expect("TargetStatusWnd", "barHP", 16, 26, 150, 6)
    expect("TargetStatusWnd", "barMP", 16, 33, 150, 6)
    expect("TargetStatusWnd", "btnClose", 151, 6, 15, 15)

    expect("ChatWnd", "ChatWnd", 0, 0, 348, 187)
    expect("ChatWnd", "ChatEditBox", 39, 165, 303, 16)
    expect("ChatWnd", "ChatTabCtrl", 23, 141, 320, 23)
    expect("ChatWnd", "ChatTabCtrl_NormalChat", 0, 0, 64, 23)
    expect("ChatWnd", "ChatTabCtrl_TradeChat", 64, 0, 64, 23)
    expect("ChatWnd", "ChatTabCtrl_PartyChat", 128, 0, 64, 23)
    expect("ChatWnd", "ChatTabCtrl_ClanChat", 192, 0, 64, 23)
    expect("ChatWnd", "ChatTabCtrl_AllyChat", 256, 0, 64, 23)
    chat_tabs = [c for c in by_name["ChatWnd"]["controls"] if c["type"] == "TabButton"]
    labels = [c["text"] for c in chat_tabs]
    if labels != ["All", "+Trade", "#Party", "@Clan", "$Alliance"]:
        raise SystemExit(f"chat tab labels {labels}")

    expect("RadarContainerWnd", "RadarContainerWnd", 0, 0, 102, 102)
    radar = by_name["RadarContainerWnd"]
    if radar["alignment"] != "TOP_RIGHT" or radar["placeX"] != 0 or radar["placeY"] != 0:
        raise SystemExit(f"RadarContainerWnd placement {radar['alignment']} {radar['placeX']},{radar['placeY']}")
    expect("RadarContainerWnd", "Radar", 0, 0, 102, 102)
    radar_icons = by_name["RadarWnd"]
    if radar_icons["alignment"] != "TOP_RIGHT" or radar_icons["placeX"] != -10 or radar_icons["placeY"] != 85:
        raise SystemExit(
            f"RadarWnd placement {radar_icons['alignment']} {radar_icons['placeX']},{radar_icons['placeY']}"
        )

    expect("MinimapWnd", "MinimapWnd", 0, 0, 334, 413)
    expect("MinimapWnd", "Minimap", 3, 51, 328, 328)
    expect("MinimapWnd", "MyLocButton", 10, 385, 76, 23)
    expect("MinimapWnd", "PartyLocButton", 90, 385, 76, 23)

    expect("NCLobbyFunctionWnd", "btnCreate", 13, 20, 96, 23)
    expect("NCLobbyFunctionWnd", "btnDelete", 13, 45, 96, 23)
    expect("NCLobbyFunctionWnd", "btnReLogin", 13, 95, 96, 23)
    expect("NCLobbyFunctionWnd", "NCLobbyFunctionWnd", 0, 0, 120, 138)
    expect("NCLobbyStartWnd", "btnStart", 13, 10, 116, 31)
    expect("NCLobbyWnd_CharInfo", "HPBar", 35, 55, 85, 12)
    expect("NCLobbyWnd_CharInfo", "MPBar", 35, 69, 85, 12)
    expect("NCLobbyWnd_CharInfo", "EXPBar", 35, 83, 206, 12)
    card = by_name["NCLobbyWnd_CharInfo"]
    if card["controls"][0]["width"] != 256 or card["controls"][0]["height"] != 128:
        raise SystemExit(
            f"char card size {card['controls'][0]['width']}x{card['controls'][0]['height']}"
        )
    if card["placementKnown"] != 0 or by_name["NCLobbyFunctionWnd"]["placementKnown"] != 0:
        raise SystemExit("lobby screen placement was invented; the extract does not include it")

    menu_back = next(c for c in by_name["NCLobbyFunctionWnd"]["controls"] if c["name"] == "NCLobbyFunctionWnd")
    if not menu_back["texture"].endswith("lobby_menuback"):
        raise SystemExit(f"lobby_menuback path {menu_back['texture']}")

    name = rect("StatusWnd", "UserName")
    if name[3] is None or name[3] > 0:
        raise SystemExit(f"UserName height should stay non-positive so the client logs it, got {name}")
