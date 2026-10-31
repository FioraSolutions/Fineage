#!/usr/bin/env bash
# Local Mac (or Linux + client) one-tile Interlude bake helper (#476).
# Does not commit client binaries; writes under repo Maps/ + Textures/.
#
# Usage:
#   export L2_INTERLUDE_CLIENT_ROOT="/path/to/Lineage II"
#   export FINEAGE_INTERLUDE_TILE=22_23   # 3×3 ring: 21_21…23_23 around 22_22
#   ./tools/interlude/run_mac_tile_bake.sh phase-a    # extractor + prepare_interlude_tile.py
#   ./tools/interlude/run_mac_tile_bake.sh meshes     # umodel export + blender fbx
#   ./tools/interlude/run_mac_tile_bake.sh unity      # Unity batchmode Shnok pipeline
#   ./tools/interlude/run_mac_tile_bake.sh all        # phase-a, meshes, unity (long)
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
TILE="${FINEAGE_INTERLUDE_TILE:-22_21}"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-}"
EXTRACT="${L2_INTERLUDE_EXTRACT_DIR:-$REPO/.work/interlude-extract}"
EXPORT="${L2_UMODEL_EXPORT_DIR:-$REPO/.work/umodel-export-$TILE}"
EXTRACTOR="${L2_TERRAIN_EXTRACTOR:-$REPO/l2-unity/External/L2TerrainExtractor}"
UNITY="${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.0.38f1/Unity.app/Contents/MacOS/Unity}"
TEX_PKG="T_${TILE}"

phase_a() {
  if [[ -z "$CLIENT" ]]; then
    echo "Set L2_INTERLUDE_CLIENT_ROOT to your Lineage II folder (maps/, textures/, staticmeshes/)." >&2
    exit 1
  fi
  local unr="$CLIENT/maps/${TILE}.unr"
  local utx="$CLIENT/textures/t_${TILE}.utx"
  if [[ ! -f "$unr" ]]; then
    echo "Missing map: $unr" >&2
    exit 1
  fi
  if [[ ! -f "$utx" ]]; then
    echo "Missing terrain texture package: $utx" >&2
    exit 1
  fi
  if [[ ! -d "$EXTRACTOR" ]]; then
    echo "L2TerrainExtractor not found at $EXTRACTOR (clone/build per Docs/INTERLUDE_TILE_BAKE.md)." >&2
    exit 1
  fi
  mkdir -p "$EXTRACT"
  echo "== Phase A: L2TerrainExtractor -> $EXTRACT/$TILE"
  local jar
  jar="$(ls "$EXTRACTOR"/build/libs/*-all.jar 2>/dev/null | head -1 || true)"
  if [[ -f "$EXTRACT/$TILE/${TILE}_metadata.txt" && -f "$EXTRACT/$TILE/staticmeshes.json" ]]; then
    echo "extract cache hit: $EXTRACT/$TILE"
  elif [[ -n "$jar" ]]; then
    # Extractor writes <out>/<XX_YY>/... for every tile it finds (one pass covers all tiles).
    (cd "$EXTRACTOR" && java -jar "$jar" "$CLIENT/textures" -p "t_${TILE}.utx" --maps="$CLIENT/maps" --detail-maps="$CLIENT/textures" --static-meshes --terrain-textures -o "$EXTRACT")
  else
    (cd "$EXTRACTOR" && ./gradlew run --args="--client-root $CLIENT --map $TILE --output $EXTRACT/$TILE --static-meshes")
  fi
  echo "== Phase A: prepare_interlude_tile.py"
  python3 "$REPO/tools/interlude/prepare_interlude_tile.py" "$TILE" --extract-dir "$EXTRACT"
}

meshes() {
  export L2_UMODEL_EXPORT_DIR="$EXPORT"
  export FINEAGE_INTERLUDE_TILE="$TILE"
  "$REPO/tools/interlude/export_tile_staticmeshes.sh"
  python3 "$REPO/tools/interlude/link_export_package_dirs.py"
  if command -v blender >/dev/null 2>&1; then
    blender -b -P "$REPO/tools/interlude/pskx_to_fbx_batch.py" -- "$EXPORT"
  else
    echo "Install Blender and re-run: blender -b -P tools/interlude/pskx_to_fbx_batch.py -- $EXPORT" >&2
    exit 1
  fi
}

unity_bake() {
  if [[ -z "$CLIENT" ]]; then
    echo "Set L2_INTERLUDE_CLIENT_ROOT for brush/sound paths." >&2
    exit 1
  fi
  export L2_INTERLUDE_CLIENT_ROOT="$CLIENT"
  export L2_UMODEL_EXPORT_DIR="$EXPORT"
  export FINEAGE_INTERLUDE_TILE="$TILE"
  echo "== Unity batchmode bake tile=$TILE"
  "$UNITY" -batchmode -nographics -quit \
    -projectPath "$REPO/l2-unity" \
    -executeMethod L2InterludeTileBatchCli.RunTileBakeCli \
    -logFile "$REPO/.work/unity-interlude-bake-${TILE}.log"
  echo "Log: $REPO/.work/unity-interlude-bake-${TILE}.log"
  echo "Manifest refreshed at Docs/INTERLUDE_TILE_MANIFEST.json (or run Shnok 0.1)."
}

cmd="${1:-}"
case "$cmd" in
  phase-a) phase_a ;;
  meshes) meshes ;;
  unity) unity_bake ;;
  all)
    phase_a
    meshes
    unity_bake
    ;;
  *)
    echo "Usage: FINEAGE_INTERLUDE_TILE=22_23 L2_INTERLUDE_CLIENT_ROOT=... $0 {phase-a|meshes|unity|all}" >&2
    exit 1
    ;;
esac
