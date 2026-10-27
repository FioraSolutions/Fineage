#!/usr/bin/env python3
"""Verify Fineage client assembly definition layout for #139."""

from __future__ import annotations

import json
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
SCRIPTS = REPO_ROOT / "l2-unity/Assets/Scripts"

EXPECTED: dict[str, list[str]] = {
    "Fineage.Domain": [],
    "Fineage.Networking": ["Fineage.Domain"],
    "Fineage.Game": ["Fineage.Domain", "Fineage.Networking"],
    "Fineage.Controller": ["Fineage.Game"],
    "Fineage.UI": ["Fineage.Game", "Fineage.Controller"],
    "Fineage.Di": [
        "Fineage.Domain",
        "Fineage.Networking",
        "Fineage.Game",
        "Fineage.Controller",
        "Fineage.UI",
    ],
}


def load_asmdef(path: Path) -> dict:
    with path.open(encoding="utf-8") as handle:
        return json.load(handle)


def main() -> int:
    errors: list[str] = []

    for assembly_name, expected_refs in EXPECTED.items():
        asmdef_path = SCRIPTS / assembly_name / f"{assembly_name}.asmdef"
        if not asmdef_path.is_file():
            errors.append(f"missing {asmdef_path.relative_to(REPO_ROOT)}")
            continue

        data = load_asmdef(asmdef_path)
        if data.get("name") != assembly_name:
            errors.append(f"{asmdef_path.name}: name must be {assembly_name!r}, got {data.get('name')!r}")

        refs = data.get("references", [])
        if refs != expected_refs:
            errors.append(
                f"{asmdef_path.name}: references must be {expected_refs}, got {refs}"
            )

    domain_kind = SCRIPTS / "Fineage.Domain" / "KnownlistKind.cs"
    if not domain_kind.is_file():
        errors.append(f"missing seed domain type {domain_kind.relative_to(REPO_ROOT)}")

    legacy = SCRIPTS / "Networking" / "Domain" / "KnownlistKind.cs"
    if legacy.is_file():
        errors.append("KnownlistKind.cs must live under Fineage.Domain, not Networking/Domain")

    legacy_asmdefs = (
        ("Networking", "Fineage.Networking.asmdef"),
        ("Game", "Fineage.Game.asmdef"),
        ("UI", "Fineage.UI.asmdef"),
        ("Controller", "Fineage.Controller.asmdef"),
        ("Di", "Fineage.Di.asmdef"),
    )
    for folder, asmdef_name in legacy_asmdefs:
        stray = SCRIPTS / folder / asmdef_name
        if stray.is_file():
            errors.append(
                f"asmdef must not be attached to legacy folder {folder}/ yet (use Fineage.{folder}/ scaffold)"
            )

    if errors:
        print("Client asmdef layout check failed:", file=sys.stderr)
        for item in errors:
            print(f"  - {item}", file=sys.stderr)
        return 1

    print("Client asmdef layout OK (#139 scaffold).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
