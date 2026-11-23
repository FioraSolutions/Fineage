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
_USING_ALIAS = re.compile(r"^\s*using\s+([A-Za-z_]\w*)\s*=", re.MULTILINE)
_USING_LINE = re.compile(r"^\s*using\s")
_BLOCK_COMMENT = re.compile(r"/\*.*?\*/", re.DOTALL)

# Simple type names exported by more than one namespace. A file that imports
# two namespaces of one entry cannot use the bare name — every spelling
# (`Name.`, `Name[]`, `<Name>`, `new Name(`, `typeof(Name)`, `is Name`,
# `(Name)x`, `Name?`, a field of type `Name`) is CS0104, so the guard matches
# any unqualified occurrence, not just `Name.`.
_AMBIGUOUS_TYPE_NAMESPACES: dict[str, frozenset[str]] = {
    "Object": frozenset({"System", "UnityEngine"}),
    "Random": frozenset({"System", "UnityEngine"}),
    # `using FMODUnity` / `using FMOD.Studio` do not import FMOD.Debug.
    "Debug": frozenset({"UnityEngine", "FMOD", "System.Diagnostics"}),
    "Color": frozenset({"UnityEngine", "System.Drawing"}),
    "Font": frozenset({"UnityEngine", "System.Drawing"}),
    "Image": frozenset({"UnityEngine.UI", "UnityEngine.UIElements", "System.Drawing"}),
    "Button": frozenset({"UnityEngine.UI", "UnityEngine.UIElements", "System.Windows.Forms"}),
    "Toggle": frozenset({"UnityEngine.UI", "UnityEngine.UIElements"}),
    "Slider": frozenset({"UnityEngine.UI", "UnityEngine.UIElements"}),
    "Text": frozenset({"UnityEngine.UI", "UnityEngine.UIElements"}),
    "Cursor": frozenset({"UnityEngine", "UnityEngine.UIElements", "System.Windows.Forms"}),
    "Label": frozenset({"UnityEngine.UIElements", "System.Reflection.Emit", "System.Windows.Forms"}),
    "Application": frozenset({"UnityEngine", "System.Windows.Forms"}),
    "Screen": frozenset({"UnityEngine", "System.Windows.Forms"}),
    "Vector2": frozenset({"UnityEngine", "System.Numerics"}),
    "Vector3": frozenset({"UnityEngine", "System.Numerics"}),
    "Vector4": frozenset({"UnityEngine", "System.Numerics"}),
    "Quaternion": frozenset({"UnityEngine", "System.Numerics"}),
    "Matrix4x4": frozenset({"UnityEngine", "System.Numerics"}),
    "Plane": frozenset({"UnityEngine", "System.Numerics"}),
    "Touch": frozenset({"UnityEngine", "UnityEngine.InputSystem.EnhancedTouch"}),
    "TouchPhase": frozenset({"UnityEngine", "UnityEngine.InputSystem"}),
    "Gyroscope": frozenset({"UnityEngine", "UnityEngine.InputSystem"}),
    "Timer": frozenset({"System.Threading", "System.Timers"}),
}

# `<type> <name> (= | ; | ( | { | , | ) | ])` declares a member/local/parameter
# named <name>. Keywords are not a type, so `new Color()` / `return Color;`
# never count as declarations. `=` must not be `==` or `=>`.
_MEMBER_DECL = (
    r"(?<![\w.])"
    r"(?:(?:public|private|protected|internal|static|readonly|const|volatile|"
    r"new|sealed|override|virtual|abstract|extern|unsafe|async|required)\s+)*"
    r"(?!(?:new|return|is|as|case|typeof|nameof|sizeof|default|switch|throw|"
    r"using|in|out|ref|checked|unchecked)\b)"
    r"[\w.<>,\[\]?]+\s+{name}\s*(?:=(?!=)|[;({{,)\]])"
)


def _using_namespaces(text: str) -> set[str]:
    return {match.group(1) for match in _USING_NS.finditer(text)}


def _aliased_names(text: str) -> set[str]:
    return {match.group(1) for match in _USING_ALIAS.finditer(text)}


def _member_declared_in_file(text: str, name: str) -> bool:
    """A field/property/local/parameter named `name` shadows type lookup in this
    file, so its bare uses are not type references."""
    return bool(re.search(_MEMBER_DECL.format(name=re.escape(name)), text))


def _strip_block_comments(text: str) -> str:
    return _BLOCK_COMMENT.sub(lambda m: "\n" * m.group(0).count("\n"), text)


def ambiguous_names_for_usings(usings: set[str], aliases: set[str]) -> dict[str, str]:
    """Bare name -> the imported namespaces it clashes between, per file usings."""
    clashes: dict[str, str] = {}
    for name, namespaces in _AMBIGUOUS_TYPE_NAMESPACES.items():
        imported = sorted(namespaces & usings)
        if len(imported) >= 2 and name not in aliases:
            clashes[name] = " + ".join(imported)
    return clashes


def check_ambiguous_types() -> list[str]:
    """Bare names that resolve to two imported types (CS0104).

    The ambiguous set is derived from the file's own usings through
    _AMBIGUOUS_TYPE_NAMESPACES, so any spelling of the name is caught — the
    LoginForestShot break was `Image[]` / `GetComponentsInChildren<Image>`,
    which a `Name\\.`-only matcher never sees. A `using Name = ...` alias or a
    type/member of the same name declared in the file makes the bare name legal.
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
        clashes = ambiguous_names_for_usings(_using_namespaces(text), _aliased_names(text))
        if not clashes:
            continue
        code_text = _strip_block_comments(text)
        rel = path.relative_to(REPO)
        lines = code_text.splitlines()
        for name, where in clashes.items():
            if (
                _type_defined_in_file(text, name)
                or _method_defined_in_file(text, name)
                or _member_declared_in_file(text, name)
            ):
                continue
            bare = re.compile(rf"(?<![\w.]){re.escape(name)}\b")
            for line_no, line in enumerate(lines, start=1):
                if _USING_LINE.match(line):
                    continue
                if bare.search(_code_fragment(line)):
                    errors.append(
                        f"{rel}:{line_no}: `{name}` is ambiguous with {where} — qualify it"
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


def main(argv: list[str]) -> int:
    if "--self-test" in argv:
        return self_test()
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


def self_test() -> int:
    """Fixture-tree run of check_ambiguous_types: the LoginForestShot break and
    the suppression matrix (alias, same-name member/type, qualified spellings,
    comments, strings) must produce exactly the expected flags."""
    import tempfile

    cases: dict[str, tuple[str, str | None]] = {
        # file: (source, flagged name or None)
        "f01_image_array.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M(Canvas canvas) {\n"
            "        Image[] images = canvas.GetComponentsInChildren<Image>(true);\n    }\n}\n",
            "Image",
        ),
        "f02_object_dot.cs": (
            "using System;\nusing UnityEngine;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M() {\n"
            "        UIDocument[] docs = Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);\n    }\n}\n",
            "Object",
        ),
        "f03_alias.cs": (
            "using System;\nusing UnityEngine;\nusing Object = UnityEngine.Object;\n"
            "class C {\n    void M() {\n        Object.Destroy(null);\n    }\n}\n",
            None,
        ),
        "f04_random_field.cs": (
            "using System;\nusing UnityEngine;\n"
            "class C {\n    public readonly int Random;\n"
            "    C(int random) {\n        Random = random;\n    }\n}\n",
            None,
        ),
        "f05_random_range.cs": (
            "using System;\nusing UnityEngine;\n"
            "class C {\n    void M() {\n        var range = Random.Range(0, 1);\n    }\n}\n",
            "Random",
        ),
        "f06_label_ui_uielements.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M() {\n        var label = new Label();\n    }\n}\n",
            None,
        ),
        "f07_label_emit.cs": (
            "using System.Reflection.Emit;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M() {\n        var label = new Label();\n    }\n}\n",
            "Label",
        ),
        "f08_debug_fmodunity.cs": (
            "using FMODUnity;\nusing UnityEngine;\n"
            "class C {\n    void M() {\n        Debug.Log(\"x\");\n    }\n}\n",
            None,
        ),
        "f09_debug_fmod.cs": (
            "using FMOD;\nusing UnityEngine;\n"
            "class C {\n    void M() {\n        Debug.Log(\"x\");\n    }\n}\n",
            "Debug",
        ),
        "f10_color_member.cs": (
            "using System.Drawing;\nusing UnityEngine;\n"
            "class C {\n    string M() {\n        return \"<color=\" + _data.Color + \">\";\n    }\n"
            "    Data _data;\n    class Data { public int Color; }\n}\n",
            None,
        ),
        "f11_color_var.cs": (
            "using System.Drawing;\nusing UnityEngine;\n"
            "class C {\n    void M() {\n        Color c = Color.red;\n    }\n}\n",
            "Color",
        ),
        "f12_block_comment.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "/* the Image card\n   more Image */\n"
            "class C {\n    void M() {\n    }\n}\n",
            None,
        ),
        "f13_string_literal.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M() {\n        var s = \"the Image card\";\n    }\n}\n",
            None,
        ),
        "f14_qualified.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    UnityEngine.UI.Image img;\n}\n",
            None,
        ),
        "f15_local_type.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class Image {\n    Image other;\n}\n",
            None,
        ),
        "f16_property_text.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    public string Text { get; set; }\n"
            "    void M() {\n        Text = \"a\";\n    }\n}\n",
            None,
        ),
        "f17_nameof.cs": (
            "using UnityEngine.UI;\nusing UnityEngine.UIElements;\n"
            "class C {\n    void M() {\n        var n = nameof(Image);\n    }\n}\n",
            "Image",
        ),
        "f18_button_ui_only.cs": (
            "using UnityEngine.UI;\n"
            "class C {\n    Button b;\n}\n",
            None,
        ),
    }

    global REPO, ASSETS, SCRIPTS
    with tempfile.TemporaryDirectory() as tmp:
        REPO = Path(tmp)
        ASSETS = REPO / "l2-unity/Assets"
        SCRIPTS = ASSETS / "Scripts"
        editor = ASSETS / "Editor"
        editor.mkdir(parents=True)
        for name, (source, _) in cases.items():
            (editor / name).write_text(source, encoding="utf-8")
        errors = check_ambiguous_types()

    failed = False
    for name, (_, flagged) in cases.items():
        hits = [e for e in errors if e.startswith(f"l2-unity/Assets/Editor/{name}:")]
        if flagged is None and hits:
            print(f"self-test: {name}: unexpected flag(s): {hits}", file=sys.stderr)
            failed = True
        elif flagged is not None:
            if len(hits) != 1 or f"`{flagged}`" not in hits[0]:
                print(f"self-test: {name}: want one `{flagged}` flag, got {hits}", file=sys.stderr)
                failed = True
    stray = [
        e
        for e in errors
        if not any(e.startswith(f"l2-unity/Assets/Editor/{name}:") for name in cases)
    ]
    if stray:
        print(f"self-test: stray flags: {stray}", file=sys.stderr)
        failed = True
    if failed:
        return 1
    print("check-unity-editmode-guards self-test OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))