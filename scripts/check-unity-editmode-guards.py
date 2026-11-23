#!/usr/bin/env python3
"""Cheap static guards for Unity EditMode / C# version footguns."""

from __future__ import annotations

import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
ASSETS = REPO / "l2-unity/Assets"
SCRIPTS = ASSETS / "Scripts"
EDITMODE_TESTS = ASSETS / "Tests/EditMode"
CONTRACT = SCRIPTS / "Networking/Net/Contract"
# asmdef trees that must not reference Assembly-CSharp types directly.
ASM_DEF_SOURCES = {
    "Fineage.Game": SCRIPTS / "Fineage.Game",
    "Fineage.UI": SCRIPTS / "Fineage.UI",
}
EDITOR_ROOT = ASSETS / "Editor"
UNITY_GLOBALS = frozenset(
    {
        "Mathf",
        "Vector2",
        "Vector3",
        "Vector4",
        "Quaternion",
        "Color",
        "Color32",
        "Rect",
        "Bounds",
        "Ray",
        "Time",
        "Debug",
        "Application",
        "Resources",
        "ScriptableObject",
        "MonoBehaviour",
        "Behaviour",
        "Component",
        "Rigidbody",
        "Collider",
        "Renderer",
        "Material",
        "Texture",
        "Texture2D",
        "Shader",
        "GUILayout",
        "GUI",
    }
)

FILE_SCOPED_NS = re.compile(r"^\s*namespace\s+[^;{]+;\s*$")
COLLECTION_EMPTY = re.compile(r"\{\s*get;\s*set;\s*\}\s*=\s*\[\]\s*;")
TYPE_NAME = re.compile(
    r"^\s*public\s+(?:static\s+|sealed\s+|partial\s+|abstract\s+)*"
    r"(?:class|struct|enum|interface)\s+(\w+)",
    re.MULTILINE,
)
BUILTIN_ALLOW = frozenset(
    {
        "Assert",
        "String",
        "Object",
        "GameObject",
        "Transform",
        "Vector3",
        "Quaternion",
        "Mathf",
        "Exception",
        "Int32",
        "Single",
        "Boolean",
        "Byte",
        "Array",
        "List",
        "Dictionary",
        "HashSet",
        "Task",
        "Action",
        "Func",
        "Enumerable",
        "Span",
        "ReadOnlySpan",
    }
)


def _code_fragment(line: str) -> str:
    """Drop comments and string literals so guards ignore log text and messages."""
    stripped = line.lstrip()
    if stripped.startswith("///") or stripped.startswith("*"):
        return ""
    if "//" in line:
        line = line[: line.index("//")]
    return re.sub(r'"[^"\\]*(?:\\.[^"\\]*)*"', '""', line)


def _type_defined_in_file(text: str, name: str) -> bool:
    return bool(
        re.search(
            rf"(?:class|struct|interface|enum)\s+{re.escape(name)}\b"
            rf"|\b{re.escape(name)}\s*\{{",
            text,
        )
    )


def _method_defined_in_file(text: str, name: str) -> bool:
    """A helper declared in this file is not an Assembly-CSharp type of the same name."""
    return bool(
        re.search(
            rf"(?:^|\n)\s*(?:(?:public|private|protected|internal|static|async|override|virtual|sealed|unsafe)\s+)*"
            rf"(?:[\w.<>,\[\]?]+\s+){re.escape(name)}\s*\(",
            text,
        )
    )


def _uses_type_name(fragment: str, name: str) -> bool:
    # `Outer.Poly(` is the nested type, not Assembly-CSharp `Poly`.
    if not re.search(rf"(?<!\.)\b{re.escape(name)}\b", fragment):
        return False
    if re.search(rf"\bNetOpcode\.{re.escape(name)}\b", fragment):
        return False
    if len(name) >= 12:
        return True
    return bool(
        re.search(
            rf"(?:\btypeof\({re.escape(name)}\)|AddComponent<{re.escape(name)}>|"
            rf"\bis\s+{re.escape(name)}\b|{re.escape(name)}\.|{re.escape(name)}\()",
            fragment,
        )
    )


def asmdef_roots(scripts: Path) -> list[Path]:
    roots: list[Path] = []
    for asm in sorted(scripts.rglob("*.asmdef")):
        roots.append(asm.parent)
    return roots


def owning_asmdef(path: Path, roots: list[Path]) -> Path | None:
    best: Path | None = None
    for root in roots:
        if path == root or root in path.parents:
            if best is None or len(root.parts) > len(best.parts):
                best = root
    return best


def type_names_in_tree(root: Path) -> set[str]:
    names: set[str] = set()
    if not root.is_dir():
        return names
    for cs in root.rglob("*.cs"):
        try:
            text = cs.read_text(encoding="utf-8-sig", errors="replace")
        except OSError:
            continue
        for m in TYPE_NAME.finditer(text):
            names.add(m.group(1))
    return names


def editor_script_type_names() -> set[str]:
    """Types in the default UnityEditor assembly (Assets/Editor, no nested asmdef)."""
    roots = asmdef_roots(SCRIPTS)
    editor_asmdef_roots = [asm.parent for asm in sorted((ASSETS / "Editor").rglob("*.asmdef"))]
    names: set[str] = set()
    if not EDITOR_ROOT.is_dir():
        return names
    for cs in EDITOR_ROOT.rglob("*.cs"):
        if any(root in cs.parents or cs.parent == root for root in editor_asmdef_roots):
            continue
        if owning_asmdef(cs, roots) is not None:
            continue
        try:
            text = cs.read_text(encoding="utf-8-sig", errors="replace")
        except OSError:
            continue
        for m in TYPE_NAME.finditer(text):
            names.add(m.group(1))
    return names


def assembly_csharp_type_names(scripts: Path) -> set[str]:
    roots = asmdef_roots(scripts)
    names: set[str] = set()
    for cs in scripts.rglob("*.cs"):
        if CONTRACT in cs.parents:
            continue
        if owning_asmdef(cs, roots) is not None:
            continue
        try:
            text = cs.read_text(encoding="utf-8-sig", errors="replace")
        except OSError:
            continue
        for m in TYPE_NAME.finditer(text):
            names.add(m.group(1))
    return names


def check_editmode_asmdef_types() -> list[str]:
    if not (EDITMODE_TESTS / "Fineage.EditModeTests.asmdef").is_file():
        return [f"missing {EDITMODE_TESTS / 'Fineage.EditModeTests.asmdef'}"]

    ac_types = assembly_csharp_type_names(SCRIPTS)
    errors: list[str] = []
    for test in sorted(EDITMODE_TESTS.glob("*.cs")):
        text = test.read_text(encoding="utf-8-sig", errors="replace")
        fragments = [_code_fragment(line) for line in text.splitlines()]
        code = "\n".join(fragments)
        used = sorted(
            name
            for name in ac_types
            if name not in BUILTIN_ALLOW
            and not _type_defined_in_file(text, name)
            and not _method_defined_in_file(code, name)
            and _uses_type_name(code, name)
        )
        if not used:
            continue
        errors.append(
            f"{test.relative_to(REPO)}: references Assembly-CSharp type(s) {used} "
            "(move test to Assets/Editor/EditModeTests/)"
        )
    return errors


def check_asmdef_references_assembly_csharp() -> list[str]:
    """Fail when a Fineage.* asmdef .cs file uses types from the default assembly."""
    ac_types = assembly_csharp_type_names(SCRIPTS)
    errors: list[str] = []
    for asm_label, root in ASM_DEF_SOURCES.items():
        if not root.is_dir():
            continue
        own_types = type_names_in_tree(root)
        foreign = ac_types - own_types
        for path in sorted(root.rglob("*.cs")):
            text = path.read_text(encoding="utf-8-sig", errors="replace")
            fragments = [_code_fragment(line) for line in text.splitlines()]
            code = "\n".join(fragments)
            used = sorted(
                name
                for name in foreign
                if name not in BUILTIN_ALLOW
                and not _type_defined_in_file(text, name)
                and _uses_type_name(code, name)
            )
            if not used:
                continue
            errors.append(
                f"{path.relative_to(REPO)} ({asm_label}): uses Assembly-CSharp type(s) {used} "
                "(move implementation or introduce a Fineage.*-visible seam)"
            )
    return errors


def check_asmdef_references_editor_scripts() -> list[str]:
    editor_types = editor_script_type_names()
    errors: list[str] = []
    for asm_label, root in ASM_DEF_SOURCES.items():
        if not root.is_dir():
            continue
        own_types = type_names_in_tree(root)
        foreign_editor = editor_types - own_types
        for path in sorted(root.rglob("*.cs")):
            text = path.read_text(encoding="utf-8-sig", errors="replace")
            fragments = [_code_fragment(line) for line in text.splitlines()]
            code = "\n".join(fragments)
            used = sorted(
                name
                for name in foreign_editor
                if name not in BUILTIN_ALLOW
                and not _type_defined_in_file(text, name)
                and _uses_type_name(code, name)
            )
            if not used:
                continue
            errors.append(
                f"{path.relative_to(REPO)} ({asm_label}): references Editor-only type(s) {used} "
                "(move shared code into runtime asmdef folder)"
            )
    return errors


def check_asmdef_missing_unityengine_usings() -> list[str]:
    errors: list[str] = []
    for asm_label, root in ASM_DEF_SOURCES.items():
        if not root.is_dir():
            continue
        for path in sorted(root.rglob("*.cs")):
            text = path.read_text(encoding="utf-8-sig", errors="replace")
            if re.search(r"^\s*using\s+UnityEngine\s*;", text, re.MULTILINE):
                continue
            code = "\n".join(_code_fragment(line) for line in text.splitlines())
            missing = sorted(
                ident
                for ident in UNITY_GLOBALS
                if re.search(rf"\b{re.escape(ident)}\.", code)
                or re.search(rf"\b{re.escape(ident)}\s*\(", code)
            )
            if not missing:
                continue
            errors.append(
                f"{path.relative_to(REPO)} ({asm_label}): uses {missing} without `using UnityEngine;`"
            )
    return errors


def check_cs9_syntax() -> list[str]:
    errors: list[str] = []
    for path in sorted(ASSETS.rglob("*.cs")):
        rel = path.relative_to(REPO).as_posix()
        if rel.startswith("l2-unity/Assets/Plugins"):
            continue
        text = path.read_text(encoding="utf-8-sig", errors="replace")
        for line_no, line in enumerate(text.splitlines(), start=1):
            stripped = line.strip()
            if stripped.startswith("//") or stripped.startswith("*"):
                continue
            if FILE_SCOPED_NS.match(line):
                errors.append(f"{rel}:{line_no}: file-scoped namespace (C# 10+)")
            if COLLECTION_EMPTY.search(line):
                errors.append(f"{rel}:{line_no}: collection expression `= []` (C# 12+)")
    return errors


_USING_NS = re.compile(r"^\s*using\s+([A-Za-z_][\w.]*)\s*;", re.MULTILINE)
_USING_ALIAS = re.compile(r"^\s*using\s+(Object|Debug)\s*=", re.MULTILINE)
_BARE_OBJECT = re.compile(r"(?<![\w.])Object\.")
_BARE_DEBUG = re.compile(r"(?<![\w.])Debug\.")
# Namespaces that publish a type also named Debug. `using FMOD;` imports
# FMOD.Debug; `using FMODUnity` and `using FMOD.Studio` do not.
_DEBUG_CLASH_USINGS = frozenset({"FMOD", "System.Diagnostics"})


def _using_namespaces(text: str) -> set[str]:
    return {match.group(1) for match in _USING_NS.finditer(text)}


def _aliased_names(text: str) -> set[str]:
    return {match.group(1) for match in _USING_ALIAS.finditer(text)}


def check_ambiguous_types() -> list[str]:
    """Bare names that resolve to two imported types.

    `Object` with `using System` and `using UnityEngine`.
    `Debug` with `using UnityEngine` and either `using FMOD` or
    `using System.Diagnostics`. A `using Object = ...` or `using Debug = ...`
    alias picks one, so the bare name is legal.
    """
    errors: list[str] = []
    roots = [SCRIPTS, ASSETS / "Editor", ASSETS / "Tests"]
    paths: list[Path] = []
    for root in roots:
        if root.is_dir():
            paths.extend(root.rglob("*.cs"))
    for path in sorted(set(paths)):
        if "Plugins" in path.parts:
            continue
        text = path.read_text(encoding="utf-8-sig", errors="replace")
        usings = _using_namespaces(text)
        aliases = _aliased_names(text)
        has_unity = "UnityEngine" in usings
        check_object = has_unity and "System" in usings and "Object" not in aliases
        check_debug = (
            has_unity
            and bool(usings & _DEBUG_CLASH_USINGS)
            and "Debug" not in aliases
        )
        if not check_object and not check_debug:
            continue
        rel = path.relative_to(REPO)
        for line_no, line in enumerate(text.splitlines(), start=1):
            code = _code_fragment(line)
            if check_object and _BARE_OBJECT.search(code):
                errors.append(
                    f"{rel}:{line_no}: `Object.` is ambiguous with System + UnityEngine — use UnityEngine.Object"
                )
            if check_debug and _BARE_DEBUG.search(code):
                errors.append(
                    f"{rel}:{line_no}: `Debug.` is ambiguous with UnityEngine and FMOD or System.Diagnostics — use UnityEngine.Debug"
                )
    return errors


def check_anim_keyreduce_probe_registration() -> list[str]:
    """Every anim-keyreduce-*-probes.json fixture must appear in AnimKeyReducePilotEditModeTests.ProbeFiles."""
    test = EDITMODE_TESTS / "AnimKeyReducePilotEditModeTests.cs"
    fixtures = EDITMODE_TESTS / "Fixtures"
    if not test.is_file():
        return []
    text = test.read_text(encoding="utf-8-sig", errors="replace")
    listed = set(
        re.findall(
            r"Tests/EditMode/Fixtures/(anim-keyreduce-[^\"]+\.json)",
            text,
        )
    )
    on_disk = {p.name for p in fixtures.glob("anim-keyreduce-*-probes.json")}
    errors: list[str] = []
    for name in sorted(on_disk - listed):
        errors.append(
            f"anim-keyreduce fixture not listed in ProbeFiles: Assets/Tests/EditMode/Fixtures/{name}"
        )
    for name in sorted(listed - on_disk):
        errors.append(
            f"ProbeFiles references missing anim-keyreduce fixture: Assets/Tests/EditMode/Fixtures/{name}"
        )
    return errors


_TRANSFORM_TEX = re.compile(r"TRANSFORM_TEX\s*\(")


def _transform_tex_first_arg(text: str, open_paren: int) -> str | None:
    depth = 0
    start = open_paren + 1
    for index in range(start, len(text)):
        char = text[index]
        if char == "(":
            depth += 1
        elif char == ")":
            if depth == 0:
                return text[start:index]
            depth -= 1
        elif char == "," and depth == 0:
            return text[start:index]
    return None


def check_transform_tex_macro() -> list[str]:
    """TRANSFORM_TEX pastes .xy onto its first argument.

    `TRANSFORM_TEX(uv * max(scale, 0.001), _Map)` becomes `max(...).xy` and
    fails to compile on Metal. The first argument has to be a value that
    already has .xy, not an expression.
    """
    errors: list[str] = []
    for path in sorted(ASSETS.rglob("*")):
        if path.suffix not in {".shader", ".hlsl", ".cginc"}:
            continue
        text = path.read_text(encoding="utf-8-sig", errors="replace")
        for match in _TRANSFORM_TEX.finditer(text):
            arg = _transform_tex_first_arg(text, match.end() - 1)
            if arg is None:
                continue
            if re.search(r"\*|max\s*\(", arg):
                line_no = text.count("\n", 0, match.start()) + 1
                errors.append(
                    f"{path.relative_to(REPO)}:{line_no}: TRANSFORM_TEX first argument "
                    f"must not be an expression ({arg.strip()})"
                )
    test = EDITMODE_TESTS / "L2WorldShaderCompileTests.cs"
    if not test.is_file():
        errors.append("missing Assets/Tests/EditMode/L2WorldShaderCompileTests.cs")
    else:
        body = test.read_text(encoding="utf-8-sig", errors="replace")
        for needle in (
            "ShaderCompilerPlatform.Metal",
            "ShaderCompilerPlatform.Vulkan",
            "ShaderCompilerPlatform.D3D",
            "CompileVariant",
            "ShaderUtil.ShaderHasError",
            "ShaderUtil.GetShaderMessages",
            "Assets/Resources/Data/Shaders",
            "Hidden/L2/ZoneFog",
            "L2/World/TerrainLayerModulate2X",
        ):
            if needle not in body:
                errors.append(f"L2WorldShaderCompileTests.cs does not mention {needle}")
        # Metal compiles every pass of a shader as one translation unit. A second
        # `struct Varyings` next to Blit.hlsl's Varyings is the ZoneFog failure.
        zone = ASSETS / "Resources/Data/Shaders/World/L2ZoneFog.shader"
        if zone.is_file():
            zone_text = zone.read_text(encoding="utf-8", errors="replace")
            if zone_text.count("struct Varyings") != 0:
                errors.append("L2ZoneFog.shader redefines struct Varyings (Blit.hlsl already defines it)")
        common = ASSETS / "Resources/Data/Shaders/World/L2TerrainCommon.hlsl"
        if common.is_file():
            common_text = common.read_text(encoding="utf-8", errors="replace")
            impl = common_text.find(
                "float2 L2_TerrainLayerUv(float2 worldUu, float4 uvScale, float tileUu, float texRotUu, float axis)"
            )
            call = common_text.find("L2_TerrainLayerUv(worldUu, uvScale, tileUu, 0.0, 0.0)")
            if impl < 0 or call < 0 or call < impl:
                errors.append(
                    "L2_TerrainLayerUv 5-argument implementation must be defined before the 3-argument overload calls it"
                )
        # The compile test discovers shaders by folder. The chain shader must stay in that folder.
        chain = ASSETS / "Resources/Data/Shaders/World/L2Ue2Chain.shader"
        if not chain.is_file():
            errors.append("missing L2Ue2Chain.shader")
        elif 'Shader "L2/World/Ue2Chain"' not in chain.read_text(encoding="utf-8", errors="replace"):
            errors.append("L2Ue2Chain.shader is not Shader \"L2/World/Ue2Chain\"")
    return errors


def check_zone_fog_factor() -> list[str]:
    """The login wash was a fog factor of 1 on every pixel. The amount has to stay
    the C# mirror of the shader, and cleared depth must not count as world depth."""
    errors: list[str] = []
    hlsl = ASSETS / "Resources/Data/Shaders/World/L2Modulate2XLighting.hlsl"
    zone = ASSETS / "Resources/Data/Shaders/World/L2ZoneFog.shader"
    math = ASSETS / "Scripts/Fineage.Game/Rendering/L2ZoneFogMath.cs"
    test = EDITMODE_TESTS / "L2ZoneRenderEditModeTests.cs"
    pas = ASSETS / "Scripts/Rendering/L2FxCompositor/L2ZoneFogRenderPass.cs"
    for path in (hlsl, zone, math, test, pas):
        if not path.is_file():
            errors.append(f"missing {path.relative_to(REPO)}")
    if errors:
        return errors
    hlsl_text = hlsl.read_text(encoding="utf-8", errors="replace")
    if "lerp(rgb, _L2FogColor.rgb, factor)" not in hlsl_text:
        errors.append("L2_LinearFog must lerp the pixel toward _L2FogColor by the fog factor")
    if "(viewAbsZ - _L2FogStart) * _L2FogScale" not in hlsl_text:
        errors.append("L2_LinearFog must use _L2FogStart meters, matching FogAmountMeters")
    if "rgb * _L2FogColor" in hlsl_text or "rgb + _L2FogColor" in hlsl_text:
        errors.append("L2_LinearFog must not multiply or add the fog color")
    zone_text = zone.read_text(encoding="utf-8", errors="replace")
    for needle in ("_L2ZBufferParams", "_L2ReversedZ", "_L2FogFarClip", "raw <= 1e-6", "LinearEyeDepth(raw, _L2ZBufferParams)"):
        if needle not in zone_text:
            errors.append(f"L2ZoneFog.shader does not mention {needle}")
    if "struct Varyings" in zone_text:
        errors.append("L2ZoneFog.shader redefines struct Varyings (Blit.hlsl already defines it)")
    math_text = math.read_text(encoding="utf-8", errors="replace")
    for needle in ("FogAmountUu", "FogAmountMeters", "ZBufferParams", "LinearEyeDepth", "EyeDepthUsable", "L2UuToUnity"):
        if needle not in math_text:
            errors.append(f"L2ZoneFogMath.cs does not mention {needle}")
    test_text = test.read_text(encoding="utf-8", errors="replace")
    for needle in ("FogAmountUu(300f", "FogAmountUu(5250f", "FogAmountUu(20000f", "ZBufferParams", "Vector4.zero"):
        if needle not in test_text:
            errors.append(f"L2ZoneRenderEditModeTests.cs does not mention {needle}")
    pas_text = pas.read_text(encoding="utf-8", errors="replace")
    if "L2ZoneFogMath.ZBufferParams" not in pas_text or "_L2ZBufferParams" not in pas_text:
        errors.append("L2ZoneFogRenderPass must upload _L2ZBufferParams from L2ZoneFogMath")
    return errors


def main() -> int:
    errors = (
        check_editmode_asmdef_types()
        + check_asmdef_references_assembly_csharp()
        + check_asmdef_references_editor_scripts()
        + check_asmdef_missing_unityengine_usings()
        + check_cs9_syntax()
        + check_ambiguous_types()
        + check_anim_keyreduce_probe_registration()
        + check_transform_tex_macro()
        + check_zone_fog_factor()
    )
    if errors:
        print("Unity EditMode guard failed:", file=sys.stderr)
        for line in errors:
            print(f"  - {line}", file=sys.stderr)
        return 1
    print("Unity EditMode guards OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())