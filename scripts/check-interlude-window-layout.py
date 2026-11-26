#!/usr/bin/env python3
"""Fail when the runtime window layout drifts from docs/ui-original."""

from __future__ import annotations

import runpy
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
TOOL = ROOT / "tools/ui-extract/compile_window_layout.py"

if __name__ == "__main__":
    sys.argv = [str(TOOL), "--check"]
    runpy.run_path(str(TOOL), run_name="__main__")
