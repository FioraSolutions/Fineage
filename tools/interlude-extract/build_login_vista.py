#!/usr/bin/env python3
"""Write LoginVista.json and Docs/LOGIN_LOBBY_VERIFY.md from the Lobby.unr dump.

The login camera is SceneManager Tag=Logon_Warp's action target. This dump's
Actions array has no object_array (Cruma omitted the inline ref). The script
then keeps the single InterpolationPoint whose horizontal forward aims at
StaticMeshActor lobby_back within 3 degrees, and exits 1 if that is not unique.
Camera actors tagged U2Viewport* are UnrealEd viewports and are not candidates.

Usage (repo root):
  python3 tools/interlude-extract/build_login_vista.py
"""
from __future__ import annotations

import json
import math
import struct
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMP = ROOT / "l2-unity/docs/interlude-ref/world/env/Lobby.json"
BMP = ROOT / "l2-unity/Assets/Resources/Data/Textures/lobby_terrain_t/Height.Height.bmp"
VISTA = ROOT / "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/LoginVista.json"
DOC = ROOT / "Docs/LOGIN_LOBBY_VERIFY.md"
UU = 52.5
AIM_DEG = 3.0


def die(msg: str) -> None:
    print("ERROR: " + msg, file=sys.stderr)
    sys.exit(1)


def props(export: dict) -> dict:
    out = {}
    for p in export.get("properties") or []:
        if p.get("array_index") not in (None, 0):
            continue
        out[p["name"]] = p
    return out


def vec(p):
    if not p:
        return None
    return p.get("vector")


def rot(p):
    if not p:
        return {"pitch": 0, "yaw": 0, "roll": 0}
    r = p.get("rotator") or {}
    return {"pitch": r.get("pitch", 0), "yaw": r.get("yaw", 0), "roll": r.get("roll", 0)}


def unity_pos(x, y, z):
    return (y / UU, z / UU, x / UU)


def unity_euler(pitch, yaw, roll):
    return (-360.0 * pitch / 65536.0, 360.0 * yaw / 65536.0, -360.0 * roll / 65536.0)


def ue_scale(draw, d3):
    ds = draw if draw else 1.0
    s = d3 or {"x": 1.0, "y": 1.0, "z": 1.0}
    return (s["x"] * ds, s["y"] * ds, s["z"] * ds)


def mesh_local_scale(ue):
    # +90° mesh-local yaw fix, identity prefab: (UE.X, UE.Z, UE.Y).
    return (ue[0], ue[2], ue[1])


def emitter_scale(ue):
    # yaw fix 0: (UE.Y, UE.Z, UE.X).
    return (ue[1], ue[2], ue[0])


def forward(rotator):
    yaw = rotator["yaw"] / 65536.0 * 2.0 * math.pi
    pitch = rotator["pitch"]
    if pitch > 32768:
        pitch -= 65536
    pitch = pitch / 65536.0 * 2.0 * math.pi
    return (
        math.cos(yaw) * math.cos(pitch),
        math.sin(yaw) * math.cos(pitch),
        math.sin(pitch),
    )


def main() -> None:
    if not DUMP.is_file():
        die(f"missing lossless dump {DUMP}")
    data = json.loads(DUMP.read_text())
    by_name = {e["name"]: e for e in data["exports"]}

    sm = by_name.get("SceneManager0")
    if sm is None:
        die("SceneManager0 is not in the dump")
    smp = props(sm)
    if (smp.get("Tag") or {}).get("value") != "Logon_Warp":
        die("SceneManager0 Tag is not Logon_Warp")
    actions = smp.get("Actions") or {}
    object_array = actions.get("object_array") or []
    if object_array:
        die("Actions object_array is present; follow that ref instead of the aim rule (not implemented)")

    arch_e = None
    for e in data["exports"]:
        if e["class"] != "StaticMeshActor":
            continue
        ref = (props(e).get("StaticMesh") or {}).get("ref") or ""
        if ref.endswith("lobby_back"):
            arch_e = e
            break
    if arch_e is None:
        die("no StaticMeshActor uses lobby_back")
    arch = vec(props(arch_e).get("Location"))

    aimed = []
    for e in data["exports"]:
        if e["class"] != "InterpolationPoint":
            continue
        p = props(e)
        loc = vec(p.get("Location"))
        r = rot(p.get("Rotation"))
        if not loc:
            continue
        fx, fy, fz = forward(r)
        dx, dy = arch["x"] - loc["x"], arch["y"] - loc["y"]
        dist = math.hypot(dx, dy)
        if dist < 1:
            continue
        fhl = math.hypot(fx, fy) or 1.0
        hang = math.degrees(math.acos(max(-1.0, min(1.0, (fx * dx + fy * dy) / (fhl * dist)))))
        if hang <= AIM_DEG and dist < 8000:
            aimed.append((hang, dist, e, loc, r))
    if len(aimed) != 1:
        die(f"expected one InterpolationPoint aimed at lobby_back within {AIM_DEG} deg, found {len(aimed)}")
    hang, dist, cam_e, cam_loc, cam_rot = aimed[0]

    zone = "ZoneInfo0"
    region = props(cam_e).get("Region") or {}
    subs = (region.get("subproperties") or [[]])[0]
    zone_ref = ""
    for sp in subs:
        if sp.get("name") == "Zone":
            zone_ref = sp.get("ref") or ""
    if not zone_ref.endswith(zone):
        die(f"{cam_e['name']} Region.Zone is {zone_ref}, expected {zone}")

    zone_p = props(by_name[zone])
    fog_flag = (zone_p.get("bDistanceFog") or {}).get("value")
    if fog_flag is not True:
        die(f"{zone} bDistanceFog is {fog_flag!r}, expected true")
    color = (zone_p.get("DistanceFogColor") or {}).get("color") or {}
    fog = {
        "r": int(color.get("r", 0)) & 255,
        "g": int(color.get("g", 0)) & 255,
        "b": int(color.get("b", 0)) & 255,
        "startUu": float((zone_p.get("DistanceFogStart") or {}).get("value") or 0),
        "endUu": float((zone_p.get("DistanceFogEnd") or {}).get("value") or 0),
    }
    if fog["endUu"] <= fog["startUu"]:
        die(f"{zone} fog range {fog['startUu']}..{fog['endUu']} is empty")

    sun_e = by_name.get("NMovableSunLight0")
    if sun_e is None:
        die("NMovableSunLight0 is not in the dump")
    sun_p = props(sun_e)
    sun_r = rot(sun_p.get("Rotation"))
    sun = {
        "name": "NMovableSunLight0",
        "pitch": sun_r["pitch"],
        "yaw": sun_r["yaw"],
        "roll": sun_r["roll"],
        "brightness": float((sun_p.get("LightBrightness") or {}).get("value") or 0),
        "hue": int((sun_p.get("LightHue") or {}).get("value") or 0),
        "saturation": int((sun_p.get("LightSaturation") or {}).get("value") or 255),
    }
    if sun["brightness"] <= 0:
        die("NMovableSunLight0 LightBrightness is missing")

    level = props(by_name["LevelInfo0"])
    amb = vec(level.get("AmbientVector")) or {}
    ambient = {
        "r": amb.get("x", 0),
        "g": amb.get("y", 0),
        "b": amb.get("z", 0),
        "brightness": float((level.get("AmbientBrightness") or {}).get("value") or 1),
    }

    terrain_p = props(by_name["TerrainInfo0"])
    tloc = vec(terrain_p.get("Location"))
    tscale = vec(terrain_p.get("TerrainScale"))
    if not tloc or not tscale:
        die("TerrainInfo0 Location or TerrainScale is missing")
    terrain = {
        "location": tloc,
        "scale": tscale,
        "sectors": 256,
        "drawScaleIgnored": float((terrain_p.get("DrawScale") or {}).get("value") or 0),
    }

    vista = {
        "cameraActor": cam_e["name"],
        "cameraReason": (
            "SceneManager0 Tag=Logon_Warp Actions has array_size but no object_array. "
            f"{cam_e['name']} is the only InterpolationPoint whose horizontal forward "
            f"aims at {arch_e['name']} lobby_back within {AIM_DEG:.0f} deg "
            f"(error {hang:.2f} deg, distance {dist:.0f} UU). "
            "Camera25 Tag=U2Viewport2 is the UnrealEd viewport and is not a candidate."
        ),
        "actionsObjectArray": False,
        "zoneActor": zone,
        "horizontalFov": 50.0,
        "fovSource": (
            "Engine.u/PlayerController.uc defaultproperties FovAngle=50. "
            "Camera.uc does not override it. UE FovAngle is horizontal; "
            "Unity Camera.fieldOfView is the vertical conversion at the view aspect. "
            "Camera25 stores FovAngle=50 in l2_lobby.t3d but its Tag is U2Viewport2."
        ),
        "location": cam_loc,
        "rotationUu": cam_rot,
        "fog": fog,
        "sun": sun,
        "ambient": ambient,
        "terrain": terrain,
    }
    VISTA.parent.mkdir(parents=True, exist_ok=True)
    VISTA.write_text(json.dumps(vista, indent=2) + "\n")
    print(f"wrote {VISTA}")

    write_doc(data, vista, arch_e, arch, hang)
    print(f"wrote {DOC}")


def sheet_height(samples, w, h, tloc, tscale, x, y):
    ox = tloc["x"] - w * tscale["x"] * 0.5
    oy = tloc["y"] - h * tscale["y"] * 0.5
    iu = int((x - ox) / tscale["x"])
    iv = int((y - oy) / tscale["y"])
    if iu < 0 or iv < 0 or iu >= w or iv >= h:
        return None
    sample = samples[(h - 1 - iv) * w + iu]
    z = tloc["z"] + (sample - 32768) * (tscale["z"] / 256.0)
    return z, iu, iv, sample


def write_doc(data, vista, arch_e, arch, hang) -> None:
    if not BMP.is_file():
        die(f"missing height bmp {BMP}")
    raw = BMP.read_bytes()
    off = struct.unpack_from("<I", raw, 10)[0]
    w = struct.unpack_from("<i", raw, 18)[0]
    h = struct.unpack_from("<i", raw, 22)[0]
    samples = list(struct.unpack_from("<" + "H" * (w * h), raw, off))
    tloc = vista["terrain"]["location"]
    tscale = vista["terrain"]["scale"]

    def row_mesh(e):
        p = props(e)
        loc = vec(p.get("Location"))
        r = rot(p.get("Rotation"))
        if (p.get("SwayRotationOrig") or {}).get("rotator"):
            r = rot(p.get("SwayRotationOrig"))
        ds = (p.get("DrawScale") or {}).get("value")
        d3 = vec(p.get("DrawScale3D"))
        ue = ue_scale(ds, d3)
        local = mesh_local_scale(ue)
        up = unity_pos(loc["x"], loc["y"], loc["z"])
        eu = unity_euler(r["pitch"], r["yaw"], r["roll"])
        ref = ((p.get("StaticMesh") or {}).get("ref") or "").split(":")[-1]
        sh = sheet_height(samples, w, h, tloc, tscale, loc["x"], loc["y"])
        delta = "" if sh is None else f"{sh[0] - loc['z']:.1f}"
        return (
            abs(ue[0]) if False else max(abs(ue[0]), abs(ue[1]), abs(ue[2])),
            e["name"],
            ref,
            loc,
            r,
            ue,
            local,
            up,
            eu,
            delta,
        )

    meshes = []
    for e in data["exports"]:
        if e["class"] != "StaticMeshActor":
            continue
        if (props(e).get("bDeleteMe") or {}).get("value") is True:
            continue
        meshes.append(row_mesh(e))
    meshes.sort(key=lambda r: -r[0])

    cam = vista["location"]
    cr = vista["rotationUu"]
    cup = unity_pos(cam["x"], cam["y"], cam["z"])
    ceu = unity_euler(cr["pitch"], cr["yaw"], cr["roll"])
    eye = sheet_height(samples, w, h, tloc, tscale, cam["x"], cam["y"])

    lines = []
    a = lines.append
    a("# Login lobby — numeric check against Lobby.unr")
    a("")
    a("Source dump: `l2-unity/docs/interlude-ref/world/env/Lobby.json` (lossless property export).")
    a("Shared placement: `L2Ue2ActorXform` (position, rotator, DrawScale×DrawScale3D, PrePivot) and `L2Ue2Terrain` (height and corner).")
    a("EditMode: `LoginLobbyDumpEditModeTests`.")
    a("")
    a("## Camera")
    a("")
    a("| | original | Unity |")
    a("|---|---|---|")
    a(f"| actor | `{vista['cameraActor']}` | login camera transform |")
    a(f"| Location UU | ({cam['x']:.5f}, {cam['y']:.3f}, {cam['z']:.5f}) | ({cup[0]:.4f}, {cup[1]:.4f}, {cup[2]:.4f}) m |")
    a(f"| Rotation UU (pitch, yaw, roll) | ({cr['pitch']}, {cr['yaw']}, {cr['roll']}) | euler ({ceu[0]:.3f}, {ceu[1]:.3f}, {ceu[2]:.3f}) ° |")
    a("| FOV | 50° horizontal (`PlayerController.uc` FovAngle) | vertical 29.395° at 16:9, 38.553° at 4:3 |")
    a(f"| why this actor | {vista['cameraReason']} | |")
    a("")
    a(f"Aim error to `{arch_e['name']}` lobby_back is {hang:.2f}°. "
      "The old presenter used Camera25 (Tag `U2Viewport2`) and a vertical FOV of 35 with no source.")
    a("")
    a("## Terrain")
    a("")
    a(f"TerrainInfo0 Location ({tloc['x']}, {tloc['y']}, {tloc['z']}), "
      f"TerrainScale ({tscale['x']}, {tscale['y']}, {tscale['z']}), sectors {vista['terrain']['sectors']}.")
    a(f"DrawScale {vista['terrain']['drawScaleIgnored']} is stored and not applied.")
    a("Height = Location.Z + (sample − 32768) × (TerrainScale.Z / 256). "
      "Height.bmp is bottom-up; file row = (height − 1 − v).")
    if eye:
        a(f"At the login eye, texel ({eye[1]}, {eye[2]}) sample {eye[3]} sheet Z {eye[0]:.2f}; "
          f"sheet − eye = {eye[0] - cam['z']:.1f} UU. The eye is under the sheet. The sheet is still drawn.")
    a("Far static-mesh bases (8–10k UU) sit on this sheet within about 2 UU. "
      "Bases inside 2k UU of the eye sit about 370–385 UU below it. That residual is in the dump, not a per-lobby offset.")
    a("")
    a("Layer alpha maps are the `AlphaMap` fields on TerrainInfo0 in `l2_lobby.t3d` "
      "(Lobby_terrain_t.D_M_01, D_M_02, DW_M_03, and the rest of the layer list). "
      "A missing alpha file is already a named error in `L2TerrainInfoParser`.")
    a("")
    a("## Thirty largest static meshes")
    a("")
    a("Size is max(|DrawScale×DrawScale3D|). Unity local scale includes the +90° mesh yaw fix used for Interlude FBX. "
      "PrePivot is absent on these actors. `sheet−base` is the height residual in UU.")
    a("")
    a("| actor | mesh | UE location | UE rotator | UE scale | Unity position m | Unity euler | Unity local scale | sheet−base UU |")
    a("|---|---|---|---|---|---|---|---|---|")
    for row in meshes[:30]:
        _, name, ref, loc, r, ue, local, up, eu, delta = row
        a(f"| {name} | {ref} | ({loc['x']:.1f}, {loc['y']:.1f}, {loc['z']:.1f}) | "
          f"({r['pitch']}, {r['yaw']}, {r['roll']}) | ({ue[0]:.3f}, {ue[1]:.3f}, {ue[2]:.3f}) | "
          f"({up[0]:.2f}, {up[1]:.2f}, {up[2]:.2f}) | ({eu[0]:.2f}, {eu[1]:.2f}, {eu[2]:.2f}) | "
          f"({local[0]:.3f}, {local[1]:.3f}, {local[2]:.3f}) | {delta} |")
    a("")
    a("## Emitters in the login view")
    a("")
    a("In view means within the 50° horizontal FOV of the login camera and inside ZoneInfo0 fog end (10000 UU). "
      "Unity scale is the yaw-fix-0 mapping (UE.Y, UE.Z, UE.X).")
    a("")
    a("| actor | UE location | DrawScale | distance UU | Unity position m | Unity scale |")
    a("|---|---|---|---|---|---|")
    fx, fy, _fz = forward(cr)
    n_in = 0
    for e in data["exports"]:
        if e["class"] != "Emitter":
            continue
        p = props(e)
        loc = vec(p.get("Location"))
        if not loc:
            die(f"{e['name']} has no Location")
        ds = (p.get("DrawScale") or {}).get("value")
        d3 = vec(p.get("DrawScale3D"))
        ue = ue_scale(ds if ds is not None else 1.0, d3)
        dx, dy = loc["x"] - cam["x"], loc["y"] - cam["y"]
        dist = math.hypot(dx, dy)
        fhl = math.hypot(fx, fy) or 1.0
        hang_e = math.degrees(math.acos(max(-1.0, min(1.0, (fx * dx + fy * dy) / (fhl * dist))))) if dist > 1 else 0
        if hang_e > 25 or dist > fog_end(vista):
            continue
        n_in += 1
        up = unity_pos(loc["x"], loc["y"], loc["z"])
        local = emitter_scale(ue)
        shown = ds if ds is not None else 1
        a(f"| {e['name']} | ({loc['x']:.1f}, {loc['y']:.1f}, {loc['z']:.1f}) | {shown} | {dist:.0f} | "
          f"({up[0]:.2f}, {up[1]:.2f}, {up[2]:.2f}) | ({local[0]:.3f}, {local[1]:.3f}, {local[2]:.3f}) |")
    a("")
    a(f"{n_in} emitters are inside that frustum. The package has 33 Emitter actors; the rest are outside the login view.")
    a("")
    a("## What #1793 had wrong")
    a("")
    a("- Camera: the presenter used Camera25, the UnrealEd viewport (`Tag=U2Viewport2`), and a vertical FOV of 35. The gameplay FOV is the PlayerController default 50 horizontal. The warp target is not stored: `Actions` has no `object_array`.")
    a("- Scale: `(5.20, 5.40, 4.00)` on lobby_back was written as a literal. It is DrawScale 4 × DrawScale3D (1.3, 1, 1.35) after the shared +90° FBX yaw fix, i.e. Unity local (UE.X, UE.Z, UE.Y) = (5.2, 5.4, 4.0).")
    a("- Floor: the height sheet matches static-mesh bases past 8k UU. Hiding the sheet removed the floor. The login eye is under the sheet by the residual above; the sheet stays visible and the gap is logged.")
    DOC.write_text("\n".join(lines) + "\n")


def fog_end(vista) -> float:
    return float(vista["fog"]["endUu"])


if __name__ == "__main__":
    main()
