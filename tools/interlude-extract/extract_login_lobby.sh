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
#        4=Engine/UWindow .uc defaults, 5=sysstring-e.tsv.
#
# Prerequisites (already needed by other extraction tools):
#   - Cruma 1.2.0+ (CRUMA_JAR or default ~/Tools/Cruma install) for steps 1-2
#   - java, javac, python3, dotnet (UScriptDump) for steps 1-4
#   - L2TerrainExtractor fat jar for step 3: built by run_mac_tile_bake.sh, or
#     set L2TERRAIN_JAR=/path/to/l2terrain-1.0.0-all.jar
#   - l2encdec Darwin binary for step 5 (optional; step is skipped loudly if absent)
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

if want_step 3; then
  echo "== [3/5] BSP brushes (skybox box) of Lobby.unr (gap G4) =="
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
  java -cp "$L2JAR" "$ROOT/tools/interlude/BrushDump.java" \
    "$CLIENT/maps/Lobby.unr" "$WORK/l2_lobby/brushes.t3d"
  cp "$WORK/l2_lobby/brushes.t3d" "$WORLD/env/Lobby.brushes.t3d"
  echo "brushes -> $WORLD/env/Lobby.brushes.t3d"
fi

if want_step 4; then
  echo "== [4/5] Class defaults: Engine.u + UWindow.u .uc dump (gap G1) =="
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
  echo "== [5/5] sysstring-e.dat captions (gap G7) =="
  L2ENCDEC="${L2ENCDEC:-$ROOT/tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec}"
  if [[ -x "$L2ENCDEC" ]]; then
    "$L2ENCDEC" -c decode -p 413 -l -o "$WORK/sysstring-e.dec" "$CLIENT/system/sysstring-e.dat"
    python3 "$ROOT/tools/ui-extract/dump_sysstring.py" "$WORK/sysstring-e.dec" \
      --out "$ROOT/l2-unity/docs/interlude-ref/dats/sysstring-e.tsv"
  else
    echo "WARN: l2encdec not found at $L2ENCDEC — skipping sysstring-e (set L2ENCDEC)." >&2
  fi
fi

cat <<EOF

Done. Commit the new reference dumps:
  git add l2-unity/docs/interlude-ref/world/env \\
          l2-unity/docs/interlude-ref/world/quadvisibility \\
          l2-unity/docs/interlude-ref/scripts/Engine.u \\
          l2-unity/docs/interlude-ref/scripts/UWindow.u \\
          l2-unity/docs/interlude-ref/dats/sysstring-e.tsv
  (work-dir sidecars under .work/lobby-extract are build inputs, not committed.)
EOF
