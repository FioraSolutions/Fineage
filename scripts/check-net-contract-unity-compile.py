#!/usr/bin/env python3
"""Compile the client Net contract copy at Unity language level (C# 9)."""

from __future__ import annotations

import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
CONTRACT_SRC = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/Contract"
CSPROJ_TEMPLATE = REPO_ROOT / "tools/net-contract-unity/Fineage.Net.Contract.UnityCompileCheck.csproj"

# Unity injects these; dotnet LangVersion 9 does not.
CONTRACT_USINGS = (
    "using System;\n"
    "using System.Buffers;\n"
    "using System.Buffers.Binary;\n"
    "using System.Collections.Generic;\n"
    "using System.IO;\n"
    "using System.Text;\n"
    "using System.Threading;\n"
    "using System.Threading.Tasks;\n"
)


def stage_contract_for_compile(stage_root: Path) -> int:
    """Mirror contract sources with implicit Unity usings prepended where missing."""
    count = 0
    for src in sorted(CONTRACT_SRC.rglob("*.cs")):
        rel = src.relative_to(CONTRACT_SRC)
        dst = stage_root / rel
        dst.parent.mkdir(parents=True, exist_ok=True)
        text = src.read_text(encoding="utf-8-sig")
        if re.match(r"\s*namespace\s", text) and "using System;" not in text.split("namespace", 1)[0]:
            text = CONTRACT_USINGS + "\n" + text
        dst.write_text(text, encoding="utf-8")
        count += 1
    return count


def write_stage_csproj(stage_root: Path, template: str) -> Path:
    compile_items = []
    for path in sorted(stage_root.rglob("*.cs")):
        rel = path.relative_to(stage_root).as_posix()
        compile_items.append(f'    <Compile Include="{path.as_posix()}" Link="{rel}" />')

    generated = template.replace("<!--STAGED_COMPILE_ITEMS-->", "\n".join(compile_items))
    proj = REPO_ROOT / "tools/net-contract-unity/Fineage.Net.Contract.UnityCompileCheck.staged.csproj"
    proj.write_text(generated, encoding="utf-8")
    return proj


def main() -> int:
    if not CONTRACT_SRC.is_dir():
        print(f"error: contract folder missing: {CONTRACT_SRC}", file=sys.stderr)
        return 1
    if not CSPROJ_TEMPLATE.is_file():
        print(f"error: missing {CSPROJ_TEMPLATE}", file=sys.stderr)
        return 1

    template = CSPROJ_TEMPLATE.read_text(encoding="utf-8")
    if "<!--STAGED_COMPILE_ITEMS-->" not in template:
        print("error: csproj missing STAGED_COMPILE_ITEMS placeholder", file=sys.stderr)
        return 1

    with tempfile.TemporaryDirectory(prefix="fineage-net-contract-stage-") as tmp:
        stage_root = Path(tmp) / "staged"
        count = stage_contract_for_compile(stage_root)
        proj = write_stage_csproj(stage_root, template)

        result = subprocess.run(
            ["dotnet", "build", str(proj), "-c", "Release", "-v", "minimal"],
            cwd=REPO_ROOT,
            capture_output=True,
            text=True,
        )
        try:
            if result.returncode != 0:
                print("Net contract Unity C# 9 compile check failed:", file=sys.stderr)
                if result.stdout:
                    print(result.stdout, file=sys.stderr)
                if result.stderr:
                    print(result.stderr, file=sys.stderr)
                return 1
        finally:
            if proj.is_file():
                proj.unlink(missing_ok=True)

    print(f"Net contract Unity C# 9 compile OK ({count} files)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())