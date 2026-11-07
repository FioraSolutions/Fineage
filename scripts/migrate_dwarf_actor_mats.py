#!/usr/bin/env python3
"""Rewrite FDwarf/MDwarf SpeedTree/URP-Lit mats onto L2ActorModulate2X (same as DE/Orc)."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ACTOR_GUID = "c7e2a91b4f6d48e0a1b35d8c90e47a12"
SPEEDTREE = "9920c1f1781549a46ba081a2a15a16ec"
URP_LIT = "933532a4fcc9baf4fa0491de14d08ed7"
ROOT = Path(__file__).resolve().parents[1]
FOLDERS = [
    ROOT / "l2-unity/Assets/Resources/Data/SysTextures/FDwarf/Materials",
    ROOT / "l2-unity/Assets/Resources/Data/SysTextures/MDwarf/Materials",
]

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
    # Prefer albedo slots used by SpeedTree / URP Lit.
    for prop in ("_BaseMap", "_MainTex"):
        m = re.search(
            rf"- {prop}:\n\s+m_Texture: \{{fileID: 2800000, guid: ([a-f0-9]+)",
            text,
        )
        if m:
            return m.group(1)
    m = re.search(r"m_Texture: \{fileID: 2800000, guid: ([a-f0-9]+)", text)
    return m.group(1) if m else None


def shader_guid(text: str) -> str | None:
    m = re.search(r"m_Shader: \{fileID: [^,]+, guid: ([a-f0-9]+)", text)
    return m.group(1) if m else None


def migrate_mat(path: Path) -> bool:
    text = path.read_text(errors="ignore")
    sg = shader_guid(text)
    if sg == ACTOR_GUID:
        return False
    if sg not in (SPEEDTREE, URP_LIT):
        return False
    tex = extract_tex_guid(text)
    if not tex:
        print("skip no-tex", path.name)
        return False
    name = path.stem
    hair = "_m00_" in name
    body = (HAIR_TEMPLATE if hair else OPAQUE_TEMPLATE).format(
        name=name, shader=ACTOR_GUID, tex=tex
    )
    path.write_text(body)
    return True


def main() -> int:
    n = 0
    for folder in FOLDERS:
        if not folder.is_dir():
            print("missing", folder)
            continue
        for mat in sorted(folder.glob("*.mat")):
            if migrate_mat(mat):
                n += 1
    print(f"migrated {n} dwarf materials -> L2ActorModulate2X")
    return 0


if __name__ == "__main__":
    sys.exit(main())
