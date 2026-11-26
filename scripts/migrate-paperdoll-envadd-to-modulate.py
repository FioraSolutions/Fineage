#!/usr/bin/env python3
"""Rewrite player-race SysTextures mats on L2/World/ActorEnvAdd -> L2/World/ActorModulate2X.

NPC trader mats under LineageNpcsTex* stay on ActorEnvAdd (retail env-specular path).
"""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ACTOR_GUID = "c7e2a91b4f6d48e0a1b35d8c90e47a12"
ENV_ADD_GUID = "a1e9c4d85b2f4c6e9d07a3b8f1e24c60"
ENV_SPEC_GUID = "e8f4c21a6b3d49f0a7c15d8e90b47c33"  # L2/World/ActorEnvSpecular

RACE_FOLDERS = [
    "MFighter",
    "FFighter",
    "MMagic",
    "FMagic",
    "MElf",
    "FElf",
    "MShaman",
    "FShaman",
    "MOrc",
    "FOrc",
    "MDwarf",
    "FDwarf",
    "MDarkElf",
    "FDarkElf",
]

CUTOFF_SUFFIXES = ("_t01", "_t02", "_t03", "_t00_h", "_m00_ah", "_m00_bh")


def implies_cutout(name: str) -> bool:
    n = name.lower()
    if any(n.endswith(s) for s in CUTOFF_SUFFIXES):
        return True
    if n.endswith("_ah") or n.endswith("_bh"):
        return True
    if "_wand" in n or "_wing" in n:
        return True
    return False


OPAQUE_TEMPLATE = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name}
  m_Shader: {{fileID: 4800000, guid: {shader}, type: 3}}
  m_Parent: {{fileID: 0}}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 1
  m_CustomRenderQueue: 2000
  stringTagMap:
    RenderType: Opaque
  disabledShaderPasses:
  - MOTIONVECTORS
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {{fileID: 2800000, guid: {tex}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {tex}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _ActorAlpha: 1
    - _AlphaClip: 0
    - _Cull: 0
    - _Cutoff: 0.49803922
    - _DstBlend: 0
    - _SrcBlend: 1
    - _ZWrite: 1
    m_Colors:
    - _BaseColor: {{r: 1, g: 1, b: 1, a: 1}}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
"""

HAIR_TEMPLATE = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name}
  m_Shader: {{fileID: 4800000, guid: {shader}, type: 3}}
  m_Parent: {{fileID: 0}}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords:
  - _ALPHATEST_ON
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 1
  m_CustomRenderQueue: 2450
  stringTagMap:
    RenderType: TransparentCutout
  disabledShaderPasses: []
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {{fileID: 2800000, guid: {tex}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {tex}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _ActorAlpha: 1
    - _AlphaClip: 1
    - _Cull: 0
    - _Cutoff: 0.49803922
    - _DstBlend: 0
    - _SrcBlend: 1
    - _ZWrite: 1
    m_Colors:
    - _BaseColor: {{r: 1, g: 1, b: 1, a: 1}}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
"""


def extract_tex_guid(text: str) -> str | None:
    for prop in ("_BaseMap", "_MainTex"):
        m = re.search(
            rf"- {prop}:\n\s+m_Texture: \{{fileID: 2800000, guid: ([a-f0-9]+)",
            text,
        )
        if m:
            return m.group(1)
    return None


def shader_guid(text: str) -> str | None:
    m = re.search(r"m_Shader: \{fileID: [^,]+, guid: ([a-f0-9]+)", text)
    return m.group(1) if m else None


def migrate_mat(path: Path) -> bool:
    text = path.read_text(errors="ignore")
    sg = shader_guid(text)
    if sg == ACTOR_GUID:
        return False
    tex = extract_tex_guid(text)
    if not tex:
        print("skip no-tex", path.relative_to(ROOT))
        return False
    name = path.stem
    tpl = HAIR_TEMPLATE if implies_cutout(name) else OPAQUE_TEMPLATE
    path.write_text(tpl.format(name=name, shader=ACTOR_GUID, tex=tex))
    return True


def main() -> int:
    n = 0
    base = ROOT / "l2-unity/Assets/Resources/Data/SysTextures"
    for race in RACE_FOLDERS:
        folder = base / race / "Materials"
        if not folder.is_dir():
            continue
        for mat in sorted(folder.glob("*.mat")):
            if migrate_mat(mat):
                n += 1
    print(f"migrated {n} paperdoll materials -> L2ActorModulate2X")
    return 0


if __name__ == "__main__":
    sys.exit(main())