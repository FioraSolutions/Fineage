#!/usr/bin/env bash
# Bake Interlude starting-city tile quartets (same pipeline as Elven Village).
#
# Quartet map (center tile from Mobius admin teleports + VectorUtils.GetSwMapTile):
#   Human TI:     already full (16_24..17_25)
#   Elf EV:       already full (20_19,20_20,21_19,21_20)
#   Dark Elf:     19_17,19_18,20_17,20_18
#   Orc:          18_14,18_15,19_14,19_15
#   Dwarf:        23_11,23_12,24_11,24_12
#
# Usage:
#   export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
#   export UNITY_EDITOR=/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity
#   ./tools/interlude/bake_starting_villages.sh darkelf
#   ./tools/interlude/bake_starting_villages.sh all
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-}"
UNITY="${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity}"
BAKE="$REPO/tools/interlude/run_mac_tile_bake.sh"

if [[ -z "$CLIENT" ]]; then
  echo "Set L2_INTERLUDE_CLIENT_ROOT" >&2
  exit 1
fi
export L2_INTERLUDE_CLIENT_ROOT="$CLIENT"
export UNITY_EDITOR="$UNITY"

declare -A GROUPS=(
  [darkelf]="19_17 19_18 20_17 20_18"
  [orc]="18_14 18_15 19_14 19_15"
  [dwarf]="23_11 23_12 24_11 24_12"
)

bake_tile() {
  local tile="$1"
  echo ""
  echo "######## TILE $tile ########"
  export FINEAGE_INTERLUDE_TILE="$tile"
  export L2_UMODEL_EXPORT_DIR="$REPO/.work/umodel-export-$tile"
  # Skip unity if already full-ish
  if [[ -f "$REPO/l2-unity/Assets/Resources/Data/Maps/$tile/StaticMeshes.prefab" \
     && -f "$REPO/l2-unity/Assets/Resources/Data/Maps/$tile/${tile}.prefab" ]]; then
    echo "skip (already has StaticMeshes + mesh prefab): $tile"
    return 0
  fi
  "$BAKE" all
}

bake_group() {
  local name="$1"
  local tiles="${GROUPS[$name]:-}"
  if [[ -z "$tiles" ]]; then
    echo "Unknown group $name" >&2
    exit 1
  fi
  echo "== baking starting village: $name ($tiles)"
  for t in $tiles; do
    bake_tile "$t"
  done
}

target="${1:-}"
case "$target" in
  all)
    for g in darkelf orc dwarf; do bake_group "$g"; done
    ;;
  darkelf|orc|dwarf)
    bake_group "$target"
    ;;
  *)
    echo "Usage: $0 darkelf|orc|dwarf|all" >&2
    exit 1
    ;;
esac
