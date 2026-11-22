#!/usr/bin/env bash
# Closes the #932 login-screen extraction gaps (Docs/LOBBY_LOGIN_EXTRACTION_AUDIT.md).
# Runs on a machine with the official Interlude client (Nikita's Mac).
# Everything lands in committed reference dirs; no original binaries are written
# to the repo.
#
# Usage:
#   L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II" \
#     bash tools/interlude-extract/extract_login_lobby.sh [--only 4,5] [--skip 3]
#
# Steps: 1=property dumps, 2=terrain quadvisibility, 3=BSP brushes,
#        4=Engine/UWindow .uc defaults, 5=sysstring-e.tsv,
#        6=texture pixels (java, no umodel), 7=G8 meshes (Docker umodel+Blender).
#
# Prerequisites (already needed by other extraction tools):
#   - Cruma 1.2.0+ (CRUMA_JAR or default ~/Tools/Cruma install) for steps 1-2
#   - java, javac, python3, dotnet (UScriptDump) for steps 1-4
#   - L2TerrainExtractor fat jar for steps 3+6: built by run_mac_tile_bake.sh, or
#     set L2TERRAIN_JAR=/path/to/l2terrain-1.0.0-all.jar
#   - l2encdec Darwin binary for step 5 (optional; step is skipped loudly if absent)
#   - step 7: Docker (umodel runs as linux/amd64) + Blender on PATH + the umodel
#     linux binary at l2-unity/External/UEViewer/umodel (UE Viewer,
#     https://www.gildor.org/downloads — no native macOS arm64 build exists;
#     the linux/amd64 binary in Docker is the supported path on Apple Silicon)
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
WORLD="$ROOT/l2-unity/docs/interlude-ref/world"
WORK="$ROOT/.work/lobby-extract"

ONLY=""
SKIP=""
while [[ $# -gt 0 ]]; do
  case "$1" in
    --only) ONLY="$2"; shift 2 ;;
    --skip) SKIP="$2"; shift 2 ;;
    *) echo "ERROR: unknown arg '$1' (want --only a,b,c or --skip a,b,c)" >&2; exit 2 ;;
  esac
done

want_step() {
  case ",$ONLY," in *",$1,"*) ;; *) [[ -z "$ONLY" ]] || return 1 ;; esac
  case ",$SKIP," in *",$1,"*) return 1 ;; esac
  return 0
}

cruma_jar() {
  if [[ -n "${CRUMA_JAR:-}" ]]; then
    JAR="$CRUMA_JAR"
  elif [[ -f "$HOME/Tools/Cruma/Cruma.app/Contents/app/cruma.jar" ]]; then
    JAR="$HOME/Tools/Cruma/Cruma.app/Contents/app/cruma.jar"
    JAVA="${CRUMA_JAVA:-$HOME/Tools/Cruma/Cruma.app/Contents/runtime/Contents/Home/bin/java}"
  elif [[ -f "$HOME/Tools/Cruma/Cruma/lib/app/cruma.jar" ]]; then
    JAR="$HOME/Tools/Cruma/Cruma/lib/app/cruma.jar"
    JAVA="${CRUMA_JAVA:-$HOME/Tools/Cruma/Cruma/lib/runtime/bin/java}"
  else
    echo "ERROR: set CRUMA_JAR to cruma.jar (Cruma 1.2.0+)." >&2
    exit 2
  fi
  JAVA="${JAVA:-${CRUMA_JAVA:-java}}"
}

if [[ ! -d "$CLIENT/maps" || ! -d "$CLIENT/system" ]]; then
  echo "ERROR: client not found at '$CLIENT' (need maps/ and system/)." >&2
  echo "Set L2_INTERLUDE_CLIENT_ROOT to the 'Lineage II' folder." >&2
  exit 2
fi
mkdir -p "$WORK/l2_lobby" "$WORK/decrypted"

if want_step 1; then
  echo "== [1/5] Property dumps: Lobby + lobby packages (gap G2) =="
  EXPORT_OUT="$WORLD/env" bash "$ROOT/tools/interlude-extract/export_client_packages.sh" -- \
    Lobby L2_Lobby_T l2_lobby Lobby_terrain_t l2_skies L2Font
fi

if want_step 2; then
  echo "== [2/5] Terrain QuadVisibilityBitmap sidecar for Lobby (gap G3) =="
  cruma_jar
  TOOL_DIR="$ROOT/tools/interlude-extract"
  javac --release 25 -proc:none -cp "$JAR" -d "$TOOL_DIR" \
    "$TOOL_DIR/ExportPackageProperties.java" \
    "$TOOL_DIR/MeshAnimationNotifyReader.java" \
    "$TOOL_DIR/DumpTerrainHoles.java"
  # DumpTerrainHoles writes the sidecar into <tsv-dir>/quadvisibility/, so emit
  # the tsv at world/ root and then move it next to the sidecar.
  "$JAVA" --enable-native-access=ALL-UNNAMED -Xmx4g -cp "$TOOL_DIR:$JAR" \
    DumpTerrainHoles "$CLIENT" "$WORLD/lobby-holes.tsv" Lobby
  mv "$WORLD/lobby-holes.tsv" "$WORLD/quadvisibility/lobby-holes.tsv"
  echo "sidecar -> $WORLD/quadvisibility/Lobby.quadvisibility.txt"
fi

l2_jar() {
  L2JAR="${L2TERRAIN_JAR:-}"
  if [[ -z "$L2JAR" ]]; then
    L2JAR="$(ls "$ROOT"/tools/interlude/L2TerrainExtractor/build/libs/*-all.jar 2>/dev/null | head -1 || true)"
  fi
  if [[ -z "$L2JAR" && -f "$ROOT/tools/interlude/L2TerrainExtractor/gradle/wrapper/gradle-wrapper.jar" ]]; then
    echo "L2TerrainExtractor jar missing — building (gradle wrapper)."
    (cd "$ROOT/tools/interlude/L2TerrainExtractor" && ./gradlew shadowJar)
    L2JAR="$(ls "$ROOT"/tools/interlude/L2TerrainExtractor/build/libs/*-all.jar | head -1)"
  fi
  if [[ -z "$L2JAR" || ! -f "$L2JAR" ]]; then
    echo "ERROR: no L2TerrainExtractor fat jar. Build one with run_mac_tile_bake.sh" >&2
    echo "or set L2TERRAIN_JAR=/path/to/l2terrain-1.0.0-all.jar" >&2
    exit 2
  fi
}

if want_step 3; then
  echo "== [3/7] BSP brushes (skybox box) of Lobby.unr (gap G4) =="
  l2_jar
  java -cp "$L2JAR" "$ROOT/tools/interlude/BrushDump.java" \
    "$CLIENT/maps/Lobby.unr" "$WORK/l2_lobby/brushes.t3d"
  cp "$WORK/l2_lobby/brushes.t3d" "$WORLD/env/Lobby.brushes.t3d"
  echo "brushes -> $WORLD/env/Lobby.brushes.t3d"
fi

if want_step 4; then
  echo "== [4/7] Class defaults: Engine.u + UWindow.u .uc dump (gap G1) =="
  # system/*.u is L2-encrypted (Lineage2Ver111/121 XOR header); UELib only reads
  # plain UPackage. decrypt_package_file strips the header and verifies the
  # 0x9E2A83C1 signature.
  python3 "$ROOT/tools/interlude-extract/decrypt.py" \
    "$CLIENT/system/Engine.u" "$WORK/decrypted/Engine.u"
  python3 "$ROOT/tools/interlude-extract/decrypt.py" \
    "$CLIENT/system/UWindow.u" "$WORK/decrypted/UWindow.u"
  dotnet run --project "$ROOT/tools/interlude-extract/UScriptDump" -- \
    "$WORK/decrypted/Engine.u" "$ROOT/l2-unity/docs/interlude-ref/scripts/Engine.u"
  dotnet run --project "$ROOT/tools/interlude-extract/UScriptDump" -- \
    "$WORK/decrypted/UWindow.u" "$ROOT/l2-unity/docs/interlude-ref/scripts/UWindow.u"
fi

if want_step 5; then
  echo "== [5/7] sysstring-e.dat captions (gap G7) =="
  L2ENCDEC="${L2ENCDEC:-$ROOT/tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec}"
  if [[ -x "$L2ENCDEC" ]]; then
    "$L2ENCDEC" -c decode -p 413 -l -o "$WORK/sysstring-e.dec" "$CLIENT/system/sysstring-e.dat"
    python3 "$ROOT/tools/ui-extract/dump_sysstring.py" "$WORK/sysstring-e.dec" \
      --out "$ROOT/l2-unity/docs/interlude-ref/dats/sysstring-e.tsv"
  else
    echo "WARN: l2encdec not found at $L2ENCDEC — skipping sysstring-e (set L2ENCDEC)." >&2
  fi
fi

if want_step 6; then
  echo "== [6/7] Texture pixels: l2_skies set, L2Font logos, H_M_05 (gaps G5/G6/G8) =="
  # ExportNamedTextures self-decrypts the package (Lineage2Ver XOR) and writes
  # PNGs — java + the l2terrain jar only, no umodel/Docker.
  l2_jar
  SKY_NAMES="$(python3 -c "import json; print(','.join(sorted({e['name'] for e in json.load(open('$WORLD/env/l2_skies.json'))['exports'] if e['class']=='Texture'})))")"
  if [[ -z "$SKY_NAMES" ]]; then
    echo "ERROR: no Texture exports in $WORLD/env/l2_skies.json — run step 1 first." >&2
    exit 2
  fi
  java -cp "$L2JAR" "$ROOT/tools/interlude/ExportNamedTextures.java" \
    "$CLIENT/textures/l2_skies.utx" "$WORK/tex/l2_skies" "$SKY_NAMES"
  java -cp "$L2JAR" "$ROOT/tools/interlude/ExportNamedTextures.java" \
    "$CLIENT/systextures/L2Font.utx" "$WORK/tex/L2Font" "start_logo-k,start_minilogo-k"
  java -cp "$L2JAR" "$ROOT/tools/interlude/ExportNamedTextures.java" \
    "$CLIENT/textures/Lobby_terrain_t.utx" "$WORK/tex/Lobby_terrain_t" "H_M_05"
  # PNGs land in Unity (Textures/l2_skies/*, Textures/lobby_terrain_t/H_M_05.png,
  # Data/UI/Assets/Login/start_logo-k.png + start_minilogo-k.png), .meta cloned
  # from existing siblings. Missing exports are ERRORs.
  python3 "$ROOT/tools/interlude-extract/place_login_assets.py" textures --src "$WORK/tex"
fi

if want_step 7; then
  echo "== [7/7] G8 meshes via umodel (Docker linux/amd64) + Blender =="
  UMODEL="$ROOT/l2-unity/External/UEViewer/umodel"
  if [[ ! -x "$UMODEL" ]]; then
    echo "ERROR: umodel linux binary missing at $UMODEL" >&2
    echo "Get UE Viewer (linux build) from https://www.gildor.org/downloads and" >&2
    echo "place the binary there. There is no official macOS arm64 build; the" >&2
    echo "linux/amd64 binary inside Docker is the supported path on Apple Silicon." >&2
    exit 2
  fi
  if ! command -v docker >/dev/null 2>&1; then
    echo "ERROR: docker is required to run the linux/amd64 umodel binary." >&2
    exit 2
  fi
  INNER='apt-get update -qq && apt-get install -y -qq libpng16-16 libsdl2-2.0-0 >/dev/null'
  for mesh in dion_grass015 Delf_Grass_001; do
    INNER="$INNER && /umodel -path=/client -game=l2 -export -out=/export staticmeshes/field_deco_S.usx $mesh"
  done
  for mesh in Dwarftree01 Dwarftree04 Dwarftree06 Orctree01 Orctree02 Orctree03 Orctree04; do
    INNER="$INNER && /umodel -path=/client -game=l2 -export -out=/export staticmeshes/speaking_tree_s.usx $mesh"
  done
  docker run --rm --platform linux/amd64 \
    -v "$CLIENT:/client:ro" \
    -v "$WORK/mesh-export:/export" \
    -v "$UMODEL:/umodel:ro" \
    ubuntu:22.04 bash -lc "$INNER"
  if ! command -v blender >/dev/null 2>&1; then
    echo "ERROR: blender not on PATH (need: blender -b -P tools/interlude/pskx_to_fbx_batch.py -- $WORK/mesh-export)." >&2
    exit 2
  fi
  blender -b -P "$ROOT/tools/interlude/pskx_to_fbx_batch.py" -- "$WORK/mesh-export"
  # Only the 9 lobby-referenced FBX are placed into StaticMeshes/<pkg>/.
  python3 "$ROOT/tools/interlude-extract/place_login_assets.py" meshes --src "$WORK/mesh-export"
fi

cat <<EOF

Done. Commit the new reference dumps and assets:
  git add l2-unity/docs/interlude-ref/world/env \\
          l2-unity/docs/interlude-ref/world/quadvisibility \\
          l2-unity/docs/interlude-ref/scripts/Engine.u \\
          l2-unity/docs/interlude-ref/scripts/UWindow.u \\
          l2-unity/docs/interlude-ref/dats/sysstring-e.tsv \\
          l2-unity/Assets/Resources/Data/Textures/l2_skies \\
          l2-unity/Assets/Resources/Data/Textures/lobby_terrain_t/H_M_05.png* \\
          l2-unity/Assets/Resources/Data/UI/Assets/Login/start_logo-k.png* \\
          l2-unity/Assets/Resources/Data/UI/Assets/Login/start_minilogo-k.png* \\
          l2-unity/Assets/Resources/Data/StaticMeshes/speaking_tree_s \\
          l2-unity/Assets/Resources/Data/StaticMeshes/field_deco_S
  (work-dir sidecars under .work/lobby-extract are build inputs, not committed.)
EOF
