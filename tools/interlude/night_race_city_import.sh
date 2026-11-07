#!/usr/bin/env bash
# Overnight orchestrator: race armor export/import + starting-village bakes.
# Logs under .work/night-import/. Safe to re-run (skips existing assets/tiles).
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-$HOME/Downloads/L2_Interlude_Client/Lineage II}"
UNITY="${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity}"
LOG_DIR="$REPO/.work/night-import"
AGR="$REPO/l2-unity/Assets/StreamingAssets/Data/Meta/Armorgrp_interlude.txt"
mkdir -p "$LOG_DIR"

export L2_INTERLUDE_CLIENT_ROOT="$CLIENT"
export UNITY_EDITOR="$UNITY"
export L2_RACE_IMPORT_DIR="$REPO/.work/race-import"

log() { echo "[$(date '+%H:%M:%S')] $*" | tee -a "$LOG_DIR/night.log"; }

import_race_pair() {
  # $1=package key (DarkElf), $2=classic race field stem without m_/f_  ... actually pass race prefixes
  local pkg="$1"
  shift
  local races=("$@")
  log "EXPORT $pkg"
  "$REPO/tools/interlude/export_race_armor.sh" "$pkg" >>"$LOG_DIR/export-$pkg.log" 2>&1 || {
    log "WARN export failed $pkg (see export-$pkg.log)"
    return 0
  }
  for race in "${races[@]}"; do
    local names="$REPO/.work/race-import/${race}_names.txt"
    # build names from missing list + all FBX stems
    {
      [[ -f "$REPO/.work/race-import/m_${race#M}_missing_meshes.txt" ]] && cat "$REPO/.work/race-import/m_${race#M}_missing_meshes.txt" || true
      # map race -> gap file keys
      true
    } >/dev/null
    # Prefer FBX directory listing so we import everything exported.
    ls "$REPO/.work/race-import/$pkg/FBX"/*.fbx 2>/dev/null | xargs -n1 basename | sed 's/\.fbx$//' >"$names" || true
    if [[ ! -s "$names" ]]; then
      log "WARN no FBX for $race"
      continue
    fi
    log "IMPORT $race ($(wc -l <"$names") pieces)"
    python3 "$REPO/tools/interlude/import_race_armor.py" \
      --race "$race" \
      --fbx-dir "$REPO/.work/race-import/$pkg/FBX" \
      --tex-root "$REPO/.work/race-import/tex" \
      --names "$names" \
      --armorgrp "$AGR" >>"$LOG_DIR/import-$race.log" 2>&1 || log "WARN import $race failed"
  done
}

log "=== night race+city import start ==="

# 1) Cities first (long Unity jobs) — Dark Elf village
log "BAKE darkelf villages"
"$REPO/tools/interlude/bake_starting_villages.sh" darkelf >>"$LOG_DIR/bake-darkelf.log" 2>&1 || log "WARN darkelf bake issues"

# 2) Race armor — sparse races first
import_race_pair DarkElf MDarkElf FDarkElf
import_race_pair Dwarf MDwarf FDwarf
import_race_pair Orc MOrc FOrc
import_race_pair Shaman MShaman FShaman
import_race_pair Fighter MFighter FFighter
import_race_pair Magic MMagic FMagic
# Elf fill remaining gaps
import_race_pair Elf MElf FElf

# 3) Remaining villages
log "BAKE orc villages"
"$REPO/tools/interlude/bake_starting_villages.sh" orc >>"$LOG_DIR/bake-orc.log" 2>&1 || log "WARN orc bake issues"
log "BAKE dwarf villages"
"$REPO/tools/interlude/bake_starting_villages.sh" dwarf >>"$LOG_DIR/bake-dwarf.log" 2>&1 || log "WARN dwarf bake issues"

log "=== night race+city import done ==="
log "Next: commit Maps/ + Animations/ + SysTextures/ in portions; open PRs into dev."
