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
    # Networking holds engine-free Net helpers and may depend on the wire contract.
    "Fineage.Networking": ["Fineage.Domain", "Fineage.Net.Contract"],
    "Fineage.Game": ["Fineage.Domain", "Fineage.Networking", "Unity.InputSystem", "Unity.RenderPipelines.Universal.Runtime", "Unity.RenderPipelines.Core.Runtime"],
    "Fineage.Controller": ["Fineage.Game"],
    # Domain is the shared leaf (ChatTypeData, etc.); UI may reference it directly.
    "Fineage.UI": ["Fineage.Game", "Fineage.Controller", "Fineage.Domain"],
    "Fineage.Di": [
        "Fineage.Domain",
        "Fineage.Networking",
        "Fineage.Game",
        "Fineage.Controller",
        "Fineage.UI",
        "VContainer",
    ],
}


def load_asmdef(path: Path) -> dict:
    with path.open(encoding="utf-8") as handle:
        return json.load(handle)


def check_net_contract_assembly() -> list[str]:
    """Fineage.Net.Contract must stay engine-free and independent.

    It is the first real boundary cut out of Assembly-CSharp: 100 wire DTOs and
    the codec. Unity cannot let an asmdef reference Assembly-CSharp, so the
    moment anything in this folder touches UnityEngine or a game-side type the
    client stops compiling for everyone. Guard the two cheap invariants.
    """
    errors: list[str] = []
    contract = SCRIPTS / "Networking" / "Net" / "Contract"
    asmdef_path = contract / "Fineage.Net.Contract.asmdef"

    if not asmdef_path.is_file():
        errors.append(f"missing {asmdef_path.relative_to(REPO_ROOT)}")
        return errors

    data = load_asmdef(asmdef_path)
    if data.get("name") != "Fineage.Net.Contract":
        errors.append(f"{asmdef_path.name}: name must be 'Fineage.Net.Contract'")
    if data.get("references") != []:
        errors.append(f"{asmdef_path.name}: references must be [] (no assembly may be a dependency)")
    if data.get("noEngineReferences") is not True:
        errors.append(f"{asmdef_path.name}: noEngineReferences must be true")

    forbidden = ("UnityEngine", "UnityEditor", "FineageWireConfig", "NetSession", "GameClient")
    for source in contract.rglob("*.cs"):
        text = source.read_text(encoding="utf-8-sig", errors="ignore")
        for token in forbidden:
            if token in text:
                errors.append(
                    f"{source.relative_to(REPO_ROOT)}: {token} is not allowed in Fineage.Net.Contract"
                )
    return errors


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

    errors.extend(check_net_contract_assembly())

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

    errors.extend(check_retired_wire_selector())

    if errors:
        print("Client asmdef layout check failed:", file=sys.stderr)
        for item in errors:
            print(f"  - {item}", file=sys.stderr)
        return 1

    print("Client asmdef layout OK (#139 scaffold).")
    return 0


RETIRED_WIRE_SYMBOLS = (
    "UseLoginHandoff",
    "LoginPressStartsZoneSession",
    "BypassesInterludeLogin",
    "AllowsInterludeGameSocketAuth",
    "Mode.Interlude",
)


def check_retired_wire_selector() -> list[str]:
    """The client has one wire. Guard the selector that tried to offer two.

    ``FineageWireConfig`` used to expose ``Mode``/``IsNet``/``UseLoginHandoff`` with an
    ``Interlude`` default. Every entry point set Net immediately, so the "other" branch
    was unreachable -- but it still compiled, and an editor script happily gated
    auto-login on a flag that was always true. Net is now unconditional; reintroducing
    any of these names means someone is re-adding a mode switch, which is the thing that
    let a dead path rot in place.
    """
    errors: list[str] = []
    roots = (SCRIPTS, REPO_ROOT / "l2-unity/Assets/Editor")

    for root in roots:
        if not root.is_dir():
            continue
        for source in root.rglob("*.cs"):
            text = source.read_text(encoding="utf-8-sig", errors="ignore")
            for symbol in RETIRED_WIRE_SYMBOLS:
                if symbol in text:
                    errors.append(
                        f"{source.relative_to(REPO_ROOT)}: {symbol} was retired with the "
                        f"wire selector (Net is the only wire)"
                    )

    return errors


if __name__ == "__main__":
    sys.exit(main())
