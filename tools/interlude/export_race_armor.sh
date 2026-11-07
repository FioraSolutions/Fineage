#!/usr/bin/env bash
# Export Interlude race armor meshes + textures via umodel (Docker linux binary),
# convert PSK→FBX with Blender (same recipe as MElf / .work/elfarmor/conv.py).
#
# Usage:
#   export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
#   ./tools/interlude/export_race_armor.sh DarkElf
#   ./tools/interlude/export_race_armor.sh all
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-}"
UMODEL="$REPO/l2-unity/External/UEViewer/umodel"
OUT_ROOT="${L2_RACE_IMPORT_DIR:-$REPO/.work/race-import}"
CONV="$REPO/tools/interlude/psk_to_fbx_armor.py"
PSK_ADDON="$REPO/tools/interlude/psk_import_patched.py"

if [[ -z "$CLIENT" ]]; then
  echo "Set L2_INTERLUDE_CLIENT_ROOT" >&2
  exit 1
fi
if [[ ! -x "$UMODEL" && ! -f "$UMODEL" ]]; then
  echo "Missing umodel at $UMODEL" >&2
  exit 1
fi

# mesh_ukx:tex_packages (comma-separated utx under systextures/)
declare -A RACE_PKGS=(
  [Elf]="Elf.ukx:melf.utx,felf.utx"
  [DarkElf]="DarkElf.ukx:mdarkelf.utx,fdarkelf.utx"
  [Dwarf]="Dwarf.ukx:mdwarf.utx,fdwarf.utx"
  [Orc]="Orc.ukx:MOrc.utx,FOrc.utx"
  [Shaman]="Shaman.ukx:MShaman.utx,FShaman.utx"
  [Fighter]="Fighter.ukx:MFighter.utx,FFighter.utx"
  [Magic]="Magic.ukx:MMagic.utx,FMagic.utx"
)

export_one() {
  local race="$1"
  local spec="${RACE_PKGS[$race]:-}"
  if [[ -z "$spec" ]]; then
    echo "Unknown race package key: $race (Elf|DarkElf|Dwarf|Orc|Shaman|Fighter|Magic)" >&2
    exit 1
  fi
  local ukx="${spec%%:*}"
  local texs="${spec#*:}"
  local out="$OUT_ROOT/$race"
  mkdir -p "$out"

  echo "== umodel export $race ($ukx + textures) -> $out"
  local inner="apt-get update -qq && apt-get install -y -qq libpng16-16 libsdl2-2.0-0 >/dev/null
/umodel -path=/client -game=l2 -export -out=/export animations/$ukx
"
  IFS=',' read -ra TEX_ARR <<<"$texs"
  for t in "${TEX_ARR[@]}"; do
    # Resolve casing on the host; pass the real filename into the container.
    local real=""
    for cand in "$CLIENT/systextures/$t" "$CLIENT/systextures/${t}" ; do
      [[ -f "$cand" ]] && real="$(basename "$cand")" && break
    done
    if [[ -z "$real" ]]; then
      real="$(ls "$CLIENT/systextures" | awk -v t="$t" 'BEGIN{tl=tolower(t)} tolower($0)==tl {print; exit}')"
    fi
    if [[ -n "$real" ]]; then
      inner+="/umodel -path=/client -game=l2 -export -out=/export systextures/$real
"
    else
      echo "WARN: missing texture package $t" >&2
    fi
  done

  docker run --rm --platform linux/amd64 \
    -v "$CLIENT:/client:ro" \
    -v "$out:/export" \
    -v "$UMODEL:/umodel:ro" \
    ubuntu:22.04 bash -lc "$inner"

  local psk_dir
  psk_dir="$(find "$out" -type d -name SkeletalMesh 2>/dev/null | head -1 || true)"
  local fbx_dir="$out/FBX"
  mkdir -p "$fbx_dir"
  if [[ -n "$psk_dir" && -f "$CONV" && -f "$PSK_ADDON" ]]; then
    echo "== blender PSK→FBX $psk_dir -> $fbx_dir"
    blender -b -P "$CONV" -- "$psk_dir" "$fbx_dir" "$PSK_ADDON"
  else
    echo "WARN: skip FBX convert (psk_dir=$psk_dir conv=$CONV)" >&2
  fi
  # flatten textures for importer
  mkdir -p "$OUT_ROOT/tex"
  find "$out" -type f \( -iname '*.tga' -o -iname '*.png' \) -print0 |
    while IFS= read -r -d '' f; do
      base="$(basename "$f")"
      dest="$OUT_ROOT/tex/$base"
      if [[ ! -e "$dest" ]]; then
        ln -s "$f" "$dest" 2>/dev/null || cp "$f" "$dest"
      fi
    done
  echo "DONE $race fbx=$(ls "$fbx_dir"/*.fbx 2>/dev/null | wc -l | tr -d ' ')"
}

target="${1:-}"
case "$target" in
  all)
    for r in Elf DarkElf Dwarf Orc Shaman Fighter Magic; do
      export_one "$r"
    done
    ;;
  Elf|DarkElf|Dwarf|Orc|Shaman|Fighter|Magic)
    export_one "$target"
    ;;
  *)
    echo "Usage: $0 Elf|DarkElf|Dwarf|Orc|Shaman|Fighter|Magic|all" >&2
    exit 1
    ;;
esac
