#!/usr/bin/env python3
"""The login shot's zone is already on InterpolationPoint5 in Lobby.json.

LoginVista.json zoneActor comes from that stored Region.Zone. Camera25 is the
UnrealEd viewport (Tag U2Viewport2). Nothing in the shot reads
Lobby.camera-zones.tsv, so a BSP zone walk is not an input to the picture.
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMP = ROOT / "l2-unity/docs/interlude-ref/world/env/Lobby.json"
T3D = ROOT / "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/l2_lobby.t3d"
ENGINE = ROOT / "l2-unity/docs/interlude-ref/scripts/Engine.u"
SHOT = ROOT / "l2-unity/Assets/Scripts/Rendering/L2LoginVista.cs"
SCRIPTS = ROOT / "l2-unity/Assets/Scripts"


def die(msg: str) -> None:
    print("ERROR: " + msg, file=sys.stderr)
    sys.exit(1)


def region_zone(export: dict) -> str:
    for prop in export.get("properties") or []:
        if prop.get("name") != "Region":
            continue
        subs = (prop.get("subproperties") or [[]])[0]
        for sp in subs:
            if sp.get("name") == "Zone":
                return sp.get("ref") or ""
    return ""


def prop_value(export: dict, name: str):
    for prop in export.get("properties") or []:
        if prop.get("name") == name:
            return prop.get("value")
    return None


def main() -> None:
    if not DUMP.is_file():
        die(f"missing lossless dump {DUMP}")
    data = json.loads(DUMP.read_text())
    by_name = {e["name"]: e for e in data["exports"]}

    point = by_name.get("InterpolationPoint5")
    if point is None:
        die("InterpolationPoint5 is not in Lobby.json")
    zone_ref = region_zone(point)
    if not zone_ref.endswith(":ZoneInfo0"):
        die(f"InterpolationPoint5 Region.Zone is {zone_ref or 'missing'}, expected ZoneInfo0")

    cam = by_name.get("Camera25")
    if cam is None:
        die("Camera25 is not in Lobby.json")
    tag = prop_value(cam, "Tag")
    if tag != "U2Viewport2":
        die(f"Camera25 Tag is {tag!r}, expected U2Viewport2 (UnrealEd viewport)")
    cam_zone = region_zone(cam)
    if not cam_zone.endswith(":ZoneInfo0"):
        die(f"Camera25 Region.Zone is {cam_zone or 'missing'}, expected ZoneInfo0")

    if not SHOT.is_file():
        die(f"missing {SHOT}")
    shot = SHOT.read_text()
    if "camera-zones" in shot:
        die("L2LoginVista.cs references camera-zones; the BSP file would be a shot input")
    if "zoneActor" not in shot:
        die("L2LoginVista.cs no longer reads zoneActor from LoginVista.json")

    hits = []
    for path in SCRIPTS.rglob("*.cs"):
        text = path.read_text(errors="replace")
        if "camera-zones" in text:
            hits.append(str(path.relative_to(ROOT)))
    if hits:
        die("shot scripts reference camera-zones: " + ", ".join(hits))

    no_fov = []
    for export in data["exports"]:
        if export.get("class") != "Camera":
            continue
        if not any(p.get("name") == "FovAngle" for p in export.get("properties") or []):
            no_fov.append(export["name"])
    if len(no_fov) != 13:
        die(f"expected 13 cameras with no FovAngle, found {len(no_fov)}: {', '.join(no_fov)}")
    camera_uc = (ENGINE / "Camera.uc").read_text(encoding="utf-8", errors="replace")
    if "FovAngle" in camera_uc:
        die("Camera.uc sets FovAngle; the class default must be re-read from that file")
    if not camera_uc.startswith("class Camera extends PlayerController"):
        die("Camera.uc does not extend PlayerController")
    player = (ENGINE / "PlayerController.uc").read_text(encoding="utf-8", errors="replace")
    if "FovAngle=50.0000000" not in player:
        die("PlayerController.uc defaultproperties has no FovAngle=50.0000000")

    actor = by_name.get("StaticMeshActor487")
    if actor is None:
        die("StaticMeshActor487 is not in Lobby.json")
    names = {p.get("name") for p in actor.get("properties") or []}
    if "StaticMesh" in names:
        die("StaticMeshActor487 has a StaticMesh property; the bake skip is no longer the right handling")
    flags = {p.get("name"): p.get("value") for p in actor.get("properties") or []}
    if flags.get("bDeleteMe") is not True or flags.get("bPendingDelete") is not True:
        die("StaticMeshActor487 is no longer bDeleteMe and bPendingDelete")
    if not T3D.is_file():
        die(f"missing {T3D}")
    t3d = T3D.read_text(encoding="utf-8")
    if "Begin Actor Class=StaticMeshActor Name=StaticMeshActor487\n    StaticMesh=None\n" not in t3d:
        die("l2_lobby.t3d does not record StaticMesh=None on StaticMeshActor487")

    print(
        "lobby camera zone: InterpolationPoint5 Region.Zone=ZoneInfo0; "
        "13 cameras inherit PlayerController.uc FovAngle=50.0000000; "
        "StaticMeshActor487 has no StaticMesh"
    )


if __name__ == "__main__":
    main()
