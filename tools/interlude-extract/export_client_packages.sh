#!/usr/bin/env bash
# Lossless dump of named Interlude Unreal packages (maps, uax, ukx, u).
# Does not write original binaries or sound/texture payloads.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${CRUMA_CLIENT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
OUT="${EXPORT_OUT:-$ROOT/l2-unity/docs/interlude-ref/world}"

if [[ -n "${CRUMA_JAR:-}" ]]; then
  JAR="$CRUMA_JAR"
elif [[ -f "$HOME/Tools/Cruma/Cruma.app/Contents/app/cruma.jar" ]]; then
  JAR="$HOME/Tools/Cruma/Cruma.app/Contents/app/cruma.jar"
  JAVA="${CRUMA_JAVA:-$HOME/Tools/Cruma/Cruma.app/Contents/runtime/Contents/Home/bin/java}"
elif [[ -f "$HOME/Tools/Cruma/Cruma/lib/app/cruma.jar" ]]; then
  JAR="$HOME/Tools/Cruma/Cruma/lib/app/cruma.jar"
  JAVA="${CRUMA_JAVA:-$HOME/Tools/Cruma/Cruma/lib/runtime/bin/java}"
else
  echo "Set CRUMA_JAR to cruma.jar from a Cruma 1.2.0 (or later) install." >&2
  exit 2
fi

JAVA="${JAVA:-${CRUMA_JAVA:-java}}"
TOOL_DIR="$(cd "$(dirname "$0")" && pwd)"
JAVAC="${JAVAC:-$(command -v javac)}"
"$JAVAC" --release 25 -proc:none -cp "$JAR" -d "$TOOL_DIR" \
  "$TOOL_DIR/ExportPackageProperties.java" \
  "$TOOL_DIR/MeshAnimationNotifyReader.java" \
  "$TOOL_DIR/ExportClientPackages.java"

mkdir -p "$OUT"
# bash 3.2 (macOS): `--` inside $(( )) parses as the decrement operator.
if [[ "${1:-}" == "--" ]]; then
  shift
fi
exec "$JAVA" --enable-native-access=ALL-UNNAMED -Xmx8g -cp "$TOOL_DIR:$JAR" ExportClientPackages "$CLIENT" "$OUT" "$@"
