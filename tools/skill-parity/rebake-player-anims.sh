#!/usr/bin/env bash
# Rebake one Interlude player body anim set on the Mac that owns the client.
# The skill-parity report only needs this when a *_anim sequence from the
# Interlude ukx is missing under Assets/Resources/Data/Animations.
# L2SocialAnimImporter is not this path: it keeps Social_* only.
#
#   export L2_INTERLUDE_CLIENT_ROOT="$HOME/Downloads/L2_Interlude_Client/Lineage II"
#   ./tools/skill-parity/rebake-player-anims.sh MFighter
#
# Unity is not launched here. After the FBX lands in Assets/L2ImportTmp,
# run the printed -executeMethod on the Mac, project l2-unity.
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../.." && pwd)"
BODY="${1:-}"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-}"
ADDON="$REPO/l2-unity/Assets/Editor/L2EffectGenerator/Blender/io_import_scene_unreal_psa_psk_280.py"
BLENDER_PY="$REPO/l2-unity/Assets/Editor/L2EffectGenerator/Blender/psk_psa_to_fbx.py"

if [[ -z "$BODY" ]]; then
  echo "usage: $0 <MFighter|FFighter|MMagic|FMagic|MElf|FElf|MDarkElf|FDarkElf|MOrc|FOrc|MShaman|FShaman|MDwarf|FDwarf>" >&2
  exit 1
fi

case "$BODY" in
  MFighter) UKX=Fighter.ukx; IMPORTER="(no RunCli — MFighter clips are already the full Fighter.ukx set)" ;;
  FFighter) UKX=Fighter.ukx; IMPORTER="L2FemaleAnimImporter.RunCli" ;;
  MMagic) UKX=Magic.ukx; IMPORTER="(no RunCli — Shnok/Characters/Import MMagic social animations writes only sequences the clip folder does not have yet)" ;;
  FMagic) UKX=Magic.ukx; IMPORTER="L2FemaleAnimImporter.RunCli" ;;
  MElf) UKX=Elf.ukx; IMPORTER="L2MElfAnimImporter.RunCli" ;;
  FElf) UKX=Elf.ukx; IMPORTER="L2FElfAnimImporter.RunCli" ;;
  MDarkElf) UKX=DarkElf.ukx; IMPORTER="L2MDarkElfAnimImporter.RunCli" ;;
  FDarkElf) UKX=DarkElf.ukx; IMPORTER="L2FemaleAnimImporter.RunCli" ;;
  MOrc) UKX=Orc.ukx; IMPORTER="L2MOrcAnimImporter.RunCli" ;;
  FOrc) UKX=Orc.ukx; IMPORTER="(no RunCli for FOrc)" ;;
  MShaman|FShaman) UKX=Shaman.ukx; IMPORTER="(no RunCli for $BODY)" ;;
  MDwarf|FDwarf) UKX=Dwarf.ukx; IMPORTER="L2FemaleAnimImporter.RunCli" ;;
  *) echo "unknown body $BODY" >&2; exit 1 ;;
esac

# Mesh stem the importer loads. MMagic's shipped stem is MMagic_M000_b.
case "$BODY" in
  MMagic) MESH=MMagic_M000_b ;;
  FDwarf) MESH=FDwarf_m001_b ;;
  *) MESH=${BODY}_m000_b ;;
esac

echo "Interlude package: animations/$UKX"
echo "MeshAnimation: ${BODY}_anim"
echo "PSK/PSA: ${MESH}.psk + ${BODY}_anim.psa"
echo
echo "umodel -game=l2 -export (same invocation as tools/interlude/export_race_armor.sh):"
echo "  docker run --rm --platform linux/amd64 \\"
echo "    -v \"\$L2_INTERLUDE_CLIENT_ROOT:/client:ro\" \\"
echo "    -v \"\$OUT:/export\" \\"
echo "    -v \"$REPO/l2-unity/External/UEViewer/umodel:/umodel:ro\" \\"
echo "    ubuntu:22.04 bash -lc '/umodel -path=/client -game=l2 -export -out=/export animations/$UKX'"
echo
echo "blender -b -P \"$BLENDER_PY\" -- \\"
echo "  \"\$OUT/SkeletalMesh/${MESH}.psk\" \\"
echo "  \"\$OUT/MeshAnimation/${BODY}_anim.psa\" \\"
echo "  \"$REPO/l2-unity/Assets/L2ImportTmp/${BODY}_anim.fbx\" \\"
echo "  \"$ADDON\""
echo
echo "Unity, project $REPO/l2-unity, on the Mac:"
echo "  $IMPORTER"
if [[ "$IMPORTER" == L2* ]]; then
  echo "  -batchmode -quit -executeMethod $IMPORTER"
fi
echo
echo "Importers that copy every sequence in the FBX (no Social_ filter, no per-skill list):"
echo "  L2MElfAnimImporter, L2FElfAnimImporter, L2MDarkElfAnimImporter, L2MOrcAnimImporter."
echo "L2FemaleAnimImporter fills FFighter, FMagic, FDarkElf, FDwarf, MDwarf from that FBX."
echo "L2SocialAnimImporter keeps Social_* only. Do not use it for skill takes."
echo "Do not commit the .ukx, .psk, .psa, or .fbx."

if [[ -z "$CLIENT" ]]; then
  echo
  echo "L2_INTERLUDE_CLIENT_ROOT is unset; printed the Mac commands only."
  exit 0
fi

echo
echo "Client root is set. Export still runs on the Mac (umodel + Blender + Unity)."
echo "This environment does not bake clips."
exit 0
