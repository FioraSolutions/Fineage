#!/usr/bin/env python3
"""Map Unreal texture names from ui-original JSON to files already in l2-unity/Assets."""
from __future__ import annotations

import argparse
import json
import re
from pathlib import Path

TEX_KEYS = {
    "normalTex", "pushedTex", "highlightTex", "dropTex", "file", "backTex",
    "foreTexture", "backTexture", "warnTexture", "regenTexture", "checkTexture",
    "unCheckTexture", "disableTexture", "disableCheckTexture", "barTexture",
    "effectTex1", "effectTex2", "backgroundItemTex", "selectedItemTex",
    "unselectedItemTex", "outLineUp", "outLineDown", "thumbBtnNormalTexture",
    "thumbBtnDownTexture", "tickTexture", "disableBackTexture",
}

UNREAL_TEX = re.compile(r"^(L2UI|l2ui|Default|town_map)\b", re.I)


def is_tex_name(val: str) -> bool:
    if not val or val in ("undefined",) or "/" in val:
        return False
    return bool(UNREAL_TEX.match(val)) or val.count(".") >= 2


def collect(obj, win, ctrl, refs):
    if isinstance(obj, dict):
        name = obj.get("name") or ctrl
        tex = obj.get("textures")
        if isinstance(tex, dict):
            for slot, val in tex.items():
                if isinstance(val, str) and is_tex_name(val):
                    refs.append((win, name, slot, val))
        for k, val in obj.items():
            if k in TEX_KEYS and isinstance(val, str) and is_tex_name(val):
                refs.append((win, name, k, val))
            elif k == "tabs" and isinstance(val, list):
                for tab in val:
                    tname = f"{name}.tab:{tab.get('target') or tab.get('buttonText')}"
                    for slot in ("normalTex", "pushedTex"):
                        if tab.get(slot) and is_tex_name(tab[slot]):
                            refs.append((win, tname, slot, tab[slot]))
            else:
                collect(val, win, name, refs)
    elif isinstance(obj, list):
        for v in obj:
            collect(v, win, ctrl, refs)


def norm(s: str) -> str:
    return re.sub(r"[^a-z0-9]+", "", s.lower())


def last_component(tex: str) -> str:
    return tex.split(".")[-1]


def group_component(tex: str) -> str | None:
    parts = tex.split(".")
    return parts[1] if len(parts) >= 3 else None


def match_paths(tex: str, files: list[Path], root: Path) -> list[str]:
    last = last_component(tex)
    needle = norm(last)
    group = group_component(tex)
    group_n = norm(group) if group else ""
    # icon1..icon9 are too generic to suffix-match (would hit MainWndTabIcon1).
    radar_icon = bool(re.fullmatch(r"icon\d+", last, re.I))
    hits = []
    for f in files:
        ns = norm(f.stem)
        rel = str(f.relative_to(root)).replace("\\", "/")
        if ns == needle:
            hits.append(rel)
            continue
        if radar_icon:
            continue
        if len(needle) >= 5 and ns.endswith(needle):
            hits.append(rel)
    # Prefer paths whose folder or filename mentions the xdat group (ChatWnd, SmallWnd, …)
    if group_n and hits:
        preferred = [h for h in hits if group_n in norm(h)]
        if preferred:
            # keep preferred first, then the rest
            rest = [h for h in hits if h not in preferred]
            hits = preferred + rest
    return list(dict.fromkeys(hits))


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--docs", type=Path, required=True, help="l2-unity/Docs/ui-original")
    ap.add_argument("--assets", type=Path, required=True, help="l2-unity/Assets")
    ap.add_argument("-o", "--output", type=Path, required=True)
    args = ap.parse_args()

    refs: list[tuple[str, str, str, str]] = []
    for p in sorted(args.docs.glob("*.json")):
        if p.name.startswith("_") or p.name in {"textures.json"}:
            continue
        collect(json.loads(p.read_text()), p.stem, "", refs)

    names, seen = [], set()
    for *_, n in refs:
        k = n.lower()
        if k not in seen:
            seen.add(k)
            names.append(n)

    files = [
        f
        for f in args.assets.rglob("*")
        if f.suffix.lower() in {".png", ".tga", ".jpg", ".jpeg", ".psd"}
        and "Library" not in f.parts
    ]
    repo = args.assets
    while repo.name != "l2-unity" and repo != repo.parent:
        repo = repo.parent
    repo = repo.parent  # Fineage root

    present, missing = [], []
    for tex in names:
        hits = match_paths(tex, files, repo)
        entry = {"texture": tex, "lastComponent": last_component(tex), "unityPaths": hits}
        (present if hits else missing).append(entry)

    inv = {
        "scannedAssets": "l2-unity/Assets image files (Library excluded)",
        "matchRule": (
            "Case-insensitive alphanumeric stem equals the Unreal last component, "
            "or (if that component is at least 5 characters) ends with it. "
            "Example: Status_Interlude_Ps_HpBar matches ps_hpbar. "
            "Radar icon1–icon9 require an exact stem match so they are not confused "
            "with Status_Interlude_MainWndTabIconN."
        ),
        "referencedUnique": len(names),
        "presentCount": len(present),
        "missingCount": len(missing),
        "present": present,
        "missing": missing,
        "references": [
            {"window": w, "control": c, "slot": s, "texture": t} for w, c, s, t in refs
        ],
    }
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(inv, indent=2, ensure_ascii=False) + "\n")
    missing_txt = args.output.with_name("textures-missing.txt")
    missing_txt.write_text("".join(m["texture"] + "\n" for m in missing), encoding="utf-8")
    print(f"unique={len(names)} present={len(present)} missing={len(missing)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
