#!/usr/bin/env bash
# Lossless dump of every property on every object in Animations/skill.usk,
# plus the full lineageeffect.u package and any other packages skill.usk references.
# Does not write original binaries or sound/texture payloads.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${CRUMA_CLIENT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
OUT="${1:-$ROOT/l2-unity/docs/interlude-ref/packages/usk-dump}"

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

if [[ ! -d "$CLIENT/Animations" && ! -d "$CLIENT/animations" ]]; then
  echo "Client folder not found or has no Animations/: $CLIENT" >&2
  exit 2
fi

# Compile against the same JDK the tree already uses for helper tools (Java 25 source,
# run on Cruma's Java 26 runtime).
JAVAC="${JAVAC:-$(command -v javac)}"
"$JAVAC" --release 25 -proc:none -cp "$JAR" -d "$TOOL_DIR" "$TOOL_DIR/ExportPackageProperties.java"
mkdir -p "$OUT"
exec "$JAVA" --enable-native-access=ALL-UNNAMED -cp "$TOOL_DIR:$JAR" ExportPackageProperties "$CLIENT" "$OUT"
