#!/usr/bin/env bash
# Rebuild the Interlude login vista from the Lobby.unr dump and shoot it.
# Runs on the owner's Mac (Unity 6000.3.27f1). Every step exits non-zero
# on failure. Python steps do not need Unity. The Unity steps need the
# client export root and the editor binary.
#
#   L2_INTERLUDE_CLIENT_ROOT="/path/to/Lineage II" \
#   L2_UMODEL_EXPORT_DIR="/path/to/umodel-export" \
#   bash scripts/login-lobby-rebuild.sh
#
# Skip a fresh client extract when the committed dump is already current:
#   bash scripts/login-lobby-rebuild.sh --skip-extract
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

UNITY="${UNITY:-/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity}"
SKIP_EXTRACT=0
SHOT="${SHOT:-$HOME/Desktop/fineage-login-forest-proof.png}"
for arg in "$@"; do
  case "$arg" in
    --skip-extract) SKIP_EXTRACT=1 ;;
    *) echo "ERROR: unknown arg '$arg' (want --skip-extract)" >&2; exit 2 ;;
  esac
done

if [[ "$SKIP_EXTRACT" -eq 0 ]]; then
  echo "== extract Lobby.unr (fails if the client or Cruma is missing) =="
  bash tools/interlude-extract/extract_login_lobby.sh
fi

if [[ ! -f l2-unity/docs/interlude-ref/world/env/Lobby.json ]]; then
  echo "ERROR: missing l2-unity/docs/interlude-ref/world/env/Lobby.json" >&2
  exit 1
fi

echo "== login vista numbers from the dump =="
python3 tools/interlude-extract/build_login_vista.py

echo "== lobby bake meta (t3d / terrain info) =="
python3 tools/interlude-extract/build_lobby_bake_meta.py

if [[ ! -x "$UNITY" ]]; then
  echo "ERROR: Unity editor not found at $UNITY" >&2
  echo "Set UNITY to Unity 6000.3.27f1 and re-run. The python steps above already wrote LoginVista.json." >&2
  exit 1
fi

if [[ -z "${L2_UMODEL_EXPORT_DIR:-}" ]]; then
  echo "ERROR: set L2_UMODEL_EXPORT_DIR to the umodel export root" >&2
  exit 1
fi

LOG_DIR="${TMPDIR:-/tmp}/fineage-login-lobby"
mkdir -p "$LOG_DIR"

echo "== bake l2_lobby through the shared tile pipeline =="
FINEAGE_INTERLUDE_TILE=l2_lobby \
  "$UNITY" -batchmode -nographics -quit \
  -projectPath "$ROOT/l2-unity" \
  -executeMethod L2InterludeTileBatchCli.RunTileBakeCli \
  -logFile "$LOG_DIR/bake.log"
echo "bake log: $LOG_DIR/bake.log"

echo "== lobby world emitters (no Quad fallback) =="
"$UNITY" -batchmode -nographics -quit \
  -projectPath "$ROOT/l2-unity" \
  -executeMethod L2EffectGeneratorWindow.GenerateWorldMapEmittersCli \
  -worldEmitterMap Lobby \
  -logFile "$LOG_DIR/emitters.log"
echo "emitter log: $LOG_DIR/emitters.log"

echo "== deterministic login shot =="
mkdir -p "$(dirname "$SHOT")"
"$UNITY" -batchmode -quit \
  -projectPath "$ROOT/l2-unity" \
  -screen-width 2560 -screen-height 1440 \
  -executeMethod LoginForestShot.RunCli \
  -fineage-login-forest-shot \
  -shotOut "$SHOT" \
  -logFile "$LOG_DIR/shot.log"
if [[ ! -f "$SHOT" ]]; then
  echo "ERROR: shot was not written to $SHOT (see $LOG_DIR/shot.log)" >&2
  exit 1
fi
echo "shot: $SHOT"
echo "shot log: $LOG_DIR/shot.log"
