#!/usr/bin/env bash
# Export LineageWeapons.ukx from a local Interlude client and solidify every .pskx to FBX.
# Does not commit game binaries; writes only into the working tree under .work/.
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT_PATH="${L2_CLIENT_PATH:-/Users/$(whoami)/Projects/Lineage2_Interlude/system}"
EXPORT_ROOT="${L2_UMODEL_EXPORT_WEAPONS:-$REPO/.work/umodel-export-weapons}"
UNITY_MODELS="$REPO/l2-unity/Assets/Resources/Data/Animations/LineageWeapons/Models"
UMODEL="$REPO/l2-unity/External/UEViewer/umodel"
BLENDER="${BLENDER_BIN:-/Applications/Blender.app/Contents/MacOS/Blender}"

if [[ ! -x "$UMODEL" ]]; then
  echo "Missing umodel at $UMODEL" >&2
  exit 1
fi
if [[ ! -x "$BLENDER" ]]; then
  echo "Missing Blender at $BLENDER (set BLENDER_BIN)" >&2
  exit 1
fi
if [[ ! -d "$CLIENT_PATH" ]]; then
  echo "Missing Interlude client at $CLIENT_PATH (set L2_CLIENT_PATH)" >&2
  exit 1
fi

mkdir -p "$EXPORT_ROOT/animations"

echo "== umodel export LineageWeapons -> $EXPORT_ROOT"
docker run --rm \
  -v "$CLIENT_PATH:/client:ro" \
  -v "$EXPORT_ROOT:/export" \
  -v "$UMODEL:/umodel:ro" \
  ubuntu:22.04 /bin/bash -lc '/umodel -path=/client -game=l2 -export -out=/export animations/LineageWeapons.ukx'

echo "== Blender: pskx -> FBX with weapon solidify"
"$BLENDER" -b -P "$REPO/tools/interlude/weapon_pskx_to_fbx.py" -- "$EXPORT_ROOT"

echo "== Python solidify pass on exported FBX"
python3 "$REPO/tools/interlude/weapon_mesh_solidify.py" --fix "$EXPORT_ROOT/animations"/*.fbx 2>/dev/null || true
find "$EXPORT_ROOT" -name '*.fbx' -print0 | while IFS= read -r -d '' f; do
  dest="$UNITY_MODELS/$(basename "$f")"
  if [[ -f "$dest" ]]; then
  cp "$f" "$dest"
    echo "updated $dest"
  else
    echo "new mesh (manual review): $f"
  fi
done

echo "== Unity: reimport + rebuild weapon prefabs"
UNITY="${UNITY_BIN:-/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity}"
"$UNITY" -batchmode -nographics -quit -projectPath "$REPO/l2-unity" \
  -executeMethod L2WeaponMeshPipeline.RunFixCli \
  -logFile /tmp/fineage-weapon-mesh-fix.log
"$UNITY" -batchmode -nographics -quit -projectPath "$REPO/l2-unity" \
  -executeMethod L2WeaponPrefabBuilder.BuildAll \
  -logFile /tmp/fineage-weapon-prefab-build.log

echo "Done. Verify with:"
echo "  python3 tools/interlude/weapon_mesh_solidify.py --audit"
echo "  $UNITY -batchmode -quit -projectPath $REPO/l2-unity -executeMethod L2WeaponMeshPipeline.RunAuditCli"