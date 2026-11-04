#!/usr/bin/env python3
"""Keep the client's Net wire contract in sync with the backend's.

The Net contract exists in two repositories:

  Fineage-Backend/src/Fineage.Net.Contract  -- source of truth
  Fineage/l2-unity/Assets/Scripts/Networking/Net/Contract  -- Unity needs it in-assets

Unity cannot reference an assembly outside `Assets/`, and the client CI checks
out only the client repo, so a symlink is not an option: the target would be
missing in CI and Unity's asset pipeline does not re-stat symlink targets (a
symlink's mtime does not change when the file it points to is edited, so Bee
silently skips the rebuild).

Instead the client keeps real copies and this script synchronises them. Run it
in two ways:

  * `--check`   exit 1 if the copies differ (what CI does on every client PR)
  * `--write`   copy the backend sources over the client copies

Both modes need a sibling `Fineage-Backend` checkout (the normal developer
layout). When it is absent `--check` passes with a notice rather than failing:
a client-only checkout must not go red just because the neighbour is missing.

`.meta` files are never touched -- they hold the Unity asset GUIDs, and
regenerating them would break every serialized reference to these types.
"""

from __future__ import annotations

import argparse
import filecmp
import shutil
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
CLIENT = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/Contract"
# Sibling checkout is the normal developer layout (see AGENTS.md).
BACKEND = REPO_ROOT.parent / "Fineage-Backend/src/Fineage.Net.Contract"

# Relative to each contract root. Order is the diff/report order.
FILES = (
    "Envelope/PacketEnvelope.cs",
    "Envelope/NetFraming.cs",
    "Envelope/ProtocolCapabilities.cs",
    "Ids/EntityId.cs",
    "Opcodes/NetOpcode.cs",
    "Messages/SessionMessages.cs",
    "Messages/LobbyMessages.cs",
    "Messages/WorldMessages.cs",
    "Codec/NetCodec.cs",
    "Modules/IDomainModule.cs",
)


def backend_root(explicit: str | None) -> Path:
    return Path(explicit).resolve() if explicit else BACKEND


def compare(source: Path, target: Path) -> tuple[list[str], list[str]]:
    """Return (missing, differing) relative paths."""
    missing: list[str] = []
    differing: list[str] = []

    for rel in FILES:
        src = source / rel
        dst = target / rel
        if not src.is_file():
            missing.append(rel)
            continue
        if not dst.is_file():
            differing.append(rel)
            continue
        if not filecmp.cmp(src, dst, shallow=False):
            differing.append(rel)

    return missing, differing


def write_contract(source: Path, target: Path) -> int:
    """Copy the backend sources over the client copies, preserving .meta files."""
    changed = 0

    for rel in FILES:
        src = source / rel
        dst = target / rel
        if not src.is_file():
            print(f"skip (no backend source): {rel}", file=sys.stderr)
            continue
        if dst.is_file() and filecmp.cmp(src, dst, shallow=False):
            continue

        dst.parent.mkdir(parents=True, exist_ok=True)
        # copy2 keeps mtime current, which is what Unity's importer keys on.
        shutil.copy2(src, dst)
        print(f"updated {rel}")
        changed += 1

    return changed


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument("--check", action="store_true", help="fail if the copies differ")
    mode.add_argument("--write", action="store_true", help="pull the backend sources")
    parser.add_argument(
        "--backend",
        default=None,
        help="path to Fineage.Net.Contract (defaults to the sibling checkout)",
    )
    args = parser.parse_args()

    source = backend_root(args.backend)

    if not source.is_dir():
        # CI checks out one repo; a missing neighbour is not a client failure.
        print(f"note: backend contract not found at {source}; skipping sync check")
        return 0

    if args.write:
        changed = write_contract(source, CLIENT)
        print("contract already in sync" if changed == 0 else f"synced {changed} file(s)")
        return 0

    missing, differing = compare(source, CLIENT)

    if missing:
        # A file exists in the client but not the backend: it was added client-side
        # and will be silently reverted by the next --write.
        print("backend contract is missing files the client expects:", file=sys.stderr)
        for rel in missing:
            print(f"  - {rel}", file=sys.stderr)
        return 1

    if differing:
        print(
            "client Net contract is out of sync with Fineage-Backend:\n"
            f"  backend: {source}\n"
            f"  client : {CLIENT}\n",
            file=sys.stderr,
        )
        for rel in differing:
            print(f"  - {rel}", file=sys.stderr)
        print(
            "\nRun `python3 scripts/sync-net-contract.py --write` and commit the result.\n"
            "Editing only one side is how the client fell 15 opcodes, 12 message types\n"
            "and 10 codec methods behind, silently disabling the own-UserInfo sheet and\n"
            "the whole hotbar.",
            file=sys.stderr,
        )
        return 1

    print(f"Net contract in sync ({len(FILES)} files)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
