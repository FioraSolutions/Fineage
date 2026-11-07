#!/usr/bin/env bash
# Poll-friendly continuation: after DarkElf export / tile phase-a finish, keep going.
# Invoked by the overnight loop; safe to re-run.
set -euo pipefail
REPO="$(cd "$(dirname "$0")/../.." && pwd)"
LOG="$REPO/.work/night-import"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
UNITY="${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity}"
export L2_INTERLUDE_CLIENT_ROOT="$CLIENT"
export UNITY_EDITOR="$UNITY"
mkdir -p "$LOG"
AGR="$REPO/l2-unity/Assets/StreamingAssets/Data/Meta/Armorgrp_interlude.txt"

stamp() { echo "[$(date '+%H:%M:%S')] $*"; }

import_pkg() {
  local pkg="$1"; shift
  local races=("$@")
  local marker="$LOG/done-import-$pkg"
  [[ -f "$marker" ]] && { stamp "skip import $pkg"; return 0; }
  if [[ ! -d "$REPO/.work/race-import/$pkg/FBX" ]] || ! ls "$REPO/.work/race-import/$pkg/FBX"/*.fbx >/dev/null 2>&1; then
    stamp "export $pkg"
    "$REPO/tools/interlude/export_race_armor.sh" "$pkg" >>"$LOG/export-$pkg.log" 2>&1
  fi
  for race in "${races[@]}"; do
    local names="$LOG/names-$race.txt"
    ls "$REPO/.work/race-import/$pkg/FBX"/*.fbx 2>/dev/null | xargs -n1 basename | sed 's/\.fbx$//' >"$names" || true
    [[ -s "$names" ]] || continue
    stamp "import $race ($(wc -l <"$names") fbx)"
    python3 "$REPO/tools/interlude/import_race_armor.py" \
      --race "$race" \
      --fbx-dir "$REPO/.work/race-import/$pkg/FBX" \
      --tex-root "$REPO/.work/race-import/tex" \
      --names "$names" \
      --armorgrp "$AGR" >>"$LOG/import-$race.log" 2>&1 || stamp "WARN import $race"
  done
  touch "$marker"
}

bake_tile() {
  local tile="$1"
  local marker="$LOG/done-bake-$tile"
  [[ -f "$marker" ]] && return 0
  if [[ -f "$REPO/l2-unity/Assets/Resources/Data/Maps/$tile/StaticMeshes.prefab" \
     && -f "$REPO/l2-unity/Assets/Resources/Data/Maps/$tile/${tile}.prefab" ]]; then
    touch "$marker"; return 0
  fi
  stamp "bake $tile"
  export FINEAGE_INTERLUDE_TILE="$tile"
  export L2_UMODEL_EXPORT_DIR="$REPO/.work/umodel-export-$tile"
  "$REPO/tools/interlude/run_mac_tile_bake.sh" all >>"$LOG/bake-$tile.log" 2>&1 || stamp "WARN bake $tile"
  touch "$marker"
}

# Wait for in-flight DarkElf export if still running
if pgrep -f "export_race_armor.sh DarkElf" >/dev/null 2>&1; then
  stamp "waiting for DarkElf export..."
  while pgrep -f "export_race_armor.sh DarkElf" >/dev/null 2>&1; do sleep 30; done
fi

import_pkg DarkElf MDarkElf FDarkElf

for t in 20_18 19_18 20_17 19_17; do
  # wait for phase-a of 20_18 if still running
  while pgrep -f "run_mac_tile_bake.sh phase-a" >/dev/null 2>&1; do sleep 20; done
  bake_tile "$t"
done

import_pkg Dwarf MDwarf FDwarf
import_pkg Orc MOrc FOrc
import_pkg Shaman MShaman FShaman

for t in 18_14 18_15 19_14 19_15; do bake_tile "$t"; done

import_pkg Fighter MFighter FFighter
import_pkg Magic MMagic FMagic
import_pkg Elf MElf FElf

for t in 23_11 23_12 24_11 24_12; do bake_tile "$t"; done

stamp "continuation finished"
