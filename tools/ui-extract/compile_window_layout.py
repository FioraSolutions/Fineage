#!/usr/bin/env python3
"""Write the runtime window-layout asset from l2-unity/docs/ui-original."""

from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(Path(__file__).resolve().parent))

from interlude_layout import CompileError, compile_docs, self_test  # noqa: E402

DOCS = ROOT / "l2-unity/docs/ui-original"
TEXTURES = DOCS / "textures-map.json"
OUT = ROOT / "l2-unity/Assets/Resources/Data/UI/Interlude/WindowLayouts.json"


def main() -> int:
    check = "--check" in sys.argv
    try:
        self_test(DOCS, TEXTURES)
        document = compile_docs(DOCS, TEXTURES)
    except CompileError as exc:
        print("interlude window layout failed:", file=sys.stderr)
        for line in exc.errors:
            print(f"  - {line}", file=sys.stderr)
        return 1

    text = json.dumps(document, indent=2) + "\n"
    if check:
        if not OUT.is_file():
            print(f"missing generated layout: {OUT}", file=sys.stderr)
            return 1
        current = OUT.read_text(encoding="utf-8")
        if current != text:
            print(
                "WindowLayouts.json does not match docs/ui-original. "
                "Run: python3 tools/ui-extract/compile_window_layout.py",
                file=sys.stderr,
            )
            return 1
        print(f"interlude window layout OK ({len(document['windows'])} windows)")
        return 0

    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(text, encoding="utf-8")
    print(f"wrote {OUT} ({len(document['windows'])} windows)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
