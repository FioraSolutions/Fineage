# Read scale/unit facts straight out of an FBX file (no Blender, no Unity).
# python3 tools/interlude/inspect_fbx_units.py <file.fbx> [...]
import re
import struct
import sys


def _doubles_near(data, key, count=2, window=200):
    """Return doubles that follow the ascii key inside the binary property block."""
    out = []
    for match in re.finditer(re.escape(key), data):
        start = match.end()
        chunk = data[start:start + window]
        for i in range(len(chunk) - 8):
            value = struct.unpack_from("<d", chunk, i)[0]
            if 0 < abs(value) < 1e6 and value == value:
                if not (abs(value - 1.0) < 1e-12 and i == 0):
                    out.append((i, round(value, 9)))
                    break
        if len(out) >= count:
            break
    return out


def _model_local_scales(data):
    """Lcl Scaling keys inside Model nodes: 'Lcl Scaling' then doubles."""
    scales = []
    for match in re.finditer(rb"Lcl Scaling", data):
        chunk = data[match.end():match.end() + 400]
        text = re.sub(rb"[^\x20-\x7e]", b" ", chunk).decode("ascii", "ignore")
        nums = re.findall(r"(?<![A-Za-z])[-+]?\d*\.\d+(?:[eE][-+]?\d+)?", text)
        scales.append(nums[:4])
    return scales


def main():
    for path in sys.argv[1:]:
        data = open(path, "rb").read()
        unit = _doubles_near(data, b"UnitScaleFactor")
        print(f"=== {path}")
        print(f"  UnitScaleFactor      {unit}")
        print(f"  OriginalUnitScale    {_doubles_near(data, b'OriginalUnitScaleFactor')}")
        print(f"  Lcl Scaling entries  {_model_local_scales(data)[:6]}")
        for key in (b"Bip01_Pelvis", b"bip01", b"Weapon_R_Bone"):
            hits = [m.start() for m in re.finditer(re.escape(key), data)]
            print(f"  node '{key.decode()}' occurrences {len(hits)}")


if __name__ == "__main__":
    main()
