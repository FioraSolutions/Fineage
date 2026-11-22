#!/usr/bin/env bash
# Dump the Interlude skill-effect table from Animations/skill.usk using Cruma's
# SkillEffects.listVisuals() (same algorithm the viewer uses for C3–Interlude).
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${CRUMA_CLIENT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
OUT="${1:-$ROOT/l2-unity/Docs/interlude-ref/skill-effects-engine.tsv}"

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
  echo "Install: unzip the macOS or Linux build into ~/Tools/Cruma" >&2
  exit 2
fi

JAVA="${JAVA:-${CRUMA_JAVA:-java}}"
TOOL_DIR="$(cd "$(dirname "$0")" && pwd)"

if [[ ! -d "$CLIENT/Animations" && ! -d "$CLIENT/animations" ]]; then
  echo "Client folder not found or has no Animations/: $CLIENT" >&2
  echo "Pass the official Interlude client root, or set CRUMA_CLIENT." >&2
  exit 2
fi

# The checked-in class is built for Java 26, matching Cruma 1.2.0.
# Recompile with a JDK 26+ if you change ExportSkillEffects.java:
#   javac --release 26 -proc:none -cp "$JAR" ExportSkillEffects.java
exec "$JAVA" --enable-native-access=ALL-UNNAMED -cp "$TOOL_DIR:$JAR" ExportSkillEffects "$CLIENT" "$OUT"
