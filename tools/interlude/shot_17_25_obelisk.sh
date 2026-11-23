#!/bin/sh
# Deterministic Talking Island 17_25 obelisk shot.
# Camera matches the #1827 pose: Unity (4632.45, -64.25, -1590.26), euler (18.17, -154.83, 0).
# Needs a GPU editor. Do not pass -nographics. The png is written outside the repo.
set -eu
ROOT=$(CDPATH= cd -- "$(dirname "$0")/../.." && pwd)
UNITY=${UNITY:-/Applications/Unity/Unity.app/Contents/MacOS/Unity}
OUT=${FINEAGE_TERRAIN_SHOT:-"$HOME/FineageShots/17_25-obelisk.png"}
mkdir -p "$(dirname "$OUT")"
export FINEAGE_TERRAIN_SHOT="$OUT"
"$UNITY" -batchmode -quit \
  -projectPath "$ROOT/l2-unity" \
  -executeMethod L2TerrainParityShot.RunTalkingIslandObelisk \
  -logFile -
echo "$OUT"
