#!/usr/bin/env python3
"""Compile the committed client Net contract like Unity (C# 9, no implicit usings)."""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
CONTRACT_SRC = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/Contract"
CSPROJ = REPO_ROOT / "tools/net-contract-unity/Fineage.Net.Contract.UnityCompileCheck.csproj"


def main() -> int:
    if not CONTRACT_SRC.is_dir():
        print(f"error: contract folder missing: {CONTRACT_SRC}", file=sys.stderr)
        return 1
    if not CSPROJ.is_file():
        print(f"error: missing {CSPROJ}", file=sys.stderr)
        return 1

    count = len(list(CONTRACT_SRC.rglob("*.cs")))
    result = subprocess.run(
        ["dotnet", "build", str(CSPROJ), "-c", "Release", "-v", "minimal"],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
    )
    if result.returncode != 0:
        print(
            "Net contract Unity C# 9 compile check failed "
            "(LangVersion 9, ImplicitUsings disabled, committed client tree):",
            file=sys.stderr,
        )
        if result.stdout:
            print(result.stdout, file=sys.stderr)
        if result.stderr:
            print(result.stderr, file=sys.stderr)
        return 1

    print(f"Net contract Unity C# 9 compile OK ({count} files)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())