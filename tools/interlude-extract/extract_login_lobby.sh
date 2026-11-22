#!/usr/bin/env bash
# Closes the #932 login-screen extraction gaps (Docs/LOBBY_LOGIN_EXTRACTION_AUDIT.md).
# Runs on a machine with the official Interlude client (Nikita's Mac).
# Everything lands in committed reference dirs; no original binaries are written
# to the repo.
#
# Usage:
#   L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II" \
#     bash tools/interlude-extract/extract_login_lobby.sh
#
# Prerequisites (already needed by other extraction tools):
#   - Cruma 1.2.0+ (CRUMA_JAR or default ~/Tools/Cruma install) for steps 1-2
#   - java, javac, python3, dotnet (UScriptDump) for steps 1-4
#   - tools/interlude/L2TerrainExtractor fat jar for step 3 (built by
#     tools/interlude/run_mac_tile_bake.sh phase A, or `cd tools/interlude/L2TerrainExtractor && ./gradlew shadowJar`)
#   - l2encdec Darwin binary for step 5 (optional; step is skipped loudly if absent)
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
WORLD="$ROOT/l2-unity/docs/interlude-ref/world"
WORK="$ROOT/.work/lobby-extract"

if [[ ! -d "$CLIENT/maps" || ! -d "$CLIENT/system" ]]; then
  echo "ERROR: client not found at '$CLIENT' (need maps/ and system/)." >&2
  echo "Set L2_INTERLUDE_CLIENT_ROOT to the 'Lineage II' folder." >&2
  exit 2
fi
mkdir -p "$WORK/l2_lobby"

echo "== [1/5] Property dumps: Lobby + lobby packages (gaps G2) =="
# Lobby refresh + the packages Lobby.unr references. Output: world/env/<pkg>.<ext>.json(.gz)
EXPORT_OUT="$WORLD/env" bash "$ROOT/tools/interlude-extract/export_client_packages.sh" -- \
  Lobby L2_Lobby_T l2_lobby Lobby_terrain_t l2_skies L2Font

echo "== [2/5] Terrain QuadVisibilityBitmap sidecar for Lobby (gap G3) =="
# Same Cruma jar resolution as export_client_packages.sh.
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
TOOL_DIR="$ROOT/tools/interlude-extract"
javac --release 25 -proc:none -cp "$JAR" -d "$TOOL_DIR" \
  "$TOOL_DIR/ExportPackageProperties.java" \
  "$TOOL_DIR/MeshAnimationNotifyReader.java" \
  "$TOOL_DIR/DumpTerrainHoles.java"
# Writes lobby-holes.tsv + quadvisibility/Lobby.quadvisibility.txt next to the other tiles.
"$JAVA" --enable-native-access=ALL-UNNAMED -Xmx4g -cp "$TOOL_DIR:$JAR" \
  DumpTerrainHoles "$CLIENT" "$WORLD/quadvisibility/lobby-holes.tsv" Lobby

echo "== [3/5] BSP brushes (skybox box) of Lobby.unr (gap G4) =="
L2JAR="$(ls "$ROOT"/tools/interlude/L2TerrainExtractor/build/libs/*-all.jar 2>/dev/null | head -1 || true)"
if [[ -z "$L2JAR" ]]; then
  echo "L2TerrainExtractor jar missing — building (gradle wrapper)."
  (cd "$ROOT/tools/interlude/L2TerrainExtractor" && ./gradlew shadowJar)
  L2JAR="$(ls "$ROOT"/tools/interlude/L2TerrainExtractor/build/libs/*-all.jar | head -1)"
fi
java -cp "$L2JAR" "$ROOT/tools/interlude/BrushDump.java" \
  "$CLIENT/maps/Lobby.unr" "$WORK/l2_lobby/brushes.t3d"
echo "brushes -> $WORK/l2_lobby/brushes.t3d (folded into Maps/l2_lobby/Meta by the Step 2 generator)"

echo "== [4/5] Class defaults: Engine.u + UWindow.u .uc dump (gap G1) =="
# Camera default FovAngle (Camera25 stores none), emitter/zone/light defaults.
dotnet run --project "$ROOT/tools/interlude-extract/UScriptDump" -- \
  "$CLIENT/system/Engine.u" "$ROOT/l2-unity/docs/interlude-ref/scripts/Engine.u"
dotnet run --project "$ROOT/tools/interlude-extract/UScriptDump" -- \
  "$CLIENT/system/UWindow.u" "$ROOT/l2-unity/docs/interlude-ref/scripts/UWindow.u"

echo "== [5/5] sysstring-e.dat captions (gap G7) =="
L2ENCDEC="${L2ENCDEC:-$ROOT/tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec}"
if [[ -x "$L2ENCDEC" ]]; then
  "$L2ENCDEC" -c decode -p 413 "$CLIENT/system/sysstring-e.dat" "$WORK/sysstring-e.dec"
  python3 "$ROOT/tools/ui-extract/dump_sysstring.py" "$WORK/sysstring-e.dec" \
    --out "$ROOT/l2-unity/docs/interlude-ref/dats/sysstring-e.tsv"
else
  echo "WARN: l2encdec not found at $L2ENCDEC — skipping sysstring-e (set L2ENCDEC)." >&2
fi

cat <<EOF

Done. Commit the new reference dumps:
  git add l2-unity/docs/interlude-ref/world/env \\
          l2-unity/docs/interlude-ref/world/quadvisibility \\
          l2-unity/docs/interlude-ref/scripts/Engine.u \\
          l2-unity/docs/interlude-ref/scripts/UWindow.u \\
          l2-unity/docs/interlude-ref/dats/sysstring-e.tsv
  (work-dir sidecars under .work/lobby-extract are build inputs for the
  Step 2 lobby bake-Meta generator, not committed as-is.)
EOF
