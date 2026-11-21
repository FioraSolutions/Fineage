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
    if "//" in line:
        line = line[: line.index("//")]
    return re.sub(r'"[^"\\]*(?:\\.[^"\\]*)*"', '""', line)


def _uses_type_name(fragment: str, name: str) -> bool:
    if not re.search(rf"\b{re.escape(name)}\b", fragment):
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
            if name not in BUILTIN_ALLOW and _uses_type_name(code, name)
        )
        if not used:
            continue
        errors.append(
            f"{test.relative_to(REPO)}: references Assembly-CSharp type(s) {used} "
            "(move test to Assets/Editor/EditModeTests/)"
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


def main() -> int:
    errors = check_editmode_asmdef_types() + check_cs9_syntax()
    if errors:
        print("Unity EditMode guard failed:", file=sys.stderr)
        for line in errors:
            print(f"  - {line}", file=sys.stderr)
        return 1
    print("Unity EditMode guards OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())