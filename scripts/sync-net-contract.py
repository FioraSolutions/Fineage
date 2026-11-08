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
layout). When it is absent, local `--check` passes with a notice rather than
failing (a client-only tree must not go red without Backend). Set env
``FINEAGE_REQUIRE_CONTRACT_SYNC`` to ``1``/``true``/``TRUE`` (CI does this) to
fail instead when the backend contract path is missing.

`.meta` files are never touched -- they hold the Unity asset GUIDs, and
regenerating them would break every serialized reference to these types.
"""

from __future__ import annotations

import argparse
import os
import subprocess
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
    "Messages/BossMessages.cs",
    "Messages/SiegeMessages.cs",
    "Messages/OlympiadMessages.cs",
    "Messages/DungeonMessages.cs",
    "Messages/SevenSignsMessages.cs",
    "Codec/NetCodec.cs",
    "Codec/NetCodec.SocialEconomy.cs",
    "Modules/IDomainModule.cs",
)


def backend_root(explicit: str | None) -> Path:
    return Path(explicit).resolve() if explicit else BACKEND


def require_contract_sync() -> bool:
    """True when CI (or a strict local run) must not soft-skip a missing backend."""
    value = os.environ.get("FINEAGE_REQUIRE_CONTRACT_SYNC", "")
    return value in ("1", "true", "TRUE")


def _git_source(repo: Path, ref: str, rel: str) -> bytes | None:
    """Contents of `rel` at `ref` in `repo`, or None when unavailable."""
    try:
        out = subprocess.run(
            ["git", "-C", str(repo), "show", f"{ref}:{rel}"],
            capture_output=True,
            check=False,
        )
    except OSError:
        return None
    return out.stdout if out.returncode == 0 else None


def read_source(
    source: Path,
    rel: str,
    ref: str | None,
    backend_repo: Path | None = None,
) -> bytes | None:
    """
    The bytes this file should have on the client.

    With a `ref`, the backend's committed version wins over whatever is checked out.
    That matters: syncing from a stale working tree (an old feature branch, a rebase
    mid-flight) silently reverts opcodes the client already merged, and the client
    stays green until the next Unity compile fails on a missing type.

    `git show` must run against the Backend **repo root** with a path relative to that
    root (e.g. ``src/Fineage.Net.Contract/Opcodes/NetOpcode.cs``), not against the
    contract folder with a bare relative path — the latter always fails and silently
    fell back to a dirty working tree.
    """
    if ref and backend_repo is not None:
        try:
            git_rel = (source / rel).resolve().relative_to(backend_repo.resolve()).as_posix()
        except ValueError:
            git_rel = rel
        data = _git_source(backend_repo, ref, git_rel)
        if data is not None:
            return data
    path = source / rel
    return path.read_bytes() if path.is_file() else None


def compare(
    source: Path,
    target: Path,
    ref: str | None,
    backend_repo: Path | None = None,
) -> tuple[list[str], list[str]]:
    """Return (missing, differing) relative paths."""
    missing: list[str] = []
    differing: list[str] = []

    for rel in FILES:
        data = read_source(source, rel, ref, backend_repo)
        dst = target / rel
        if data is None:
            missing.append(rel)
            continue
        if not dst.is_file():
            differing.append(rel)
            continue
        if dst.read_bytes() != data:
            differing.append(rel)

    return missing, differing


def write_contract(
    source: Path,
    target: Path,
    ref: str | None,
    backend_repo: Path | None = None,
) -> int:
    """Copy the backend sources over the client copies, preserving .meta files."""
    changed = 0

    for rel in FILES:
        data = read_source(source, rel, ref, backend_repo)
        dst = target / rel
        if data is None:
            print(f"skip (no backend source): {rel}", file=sys.stderr)
            continue
        if dst.is_file() and dst.read_bytes() == data:
            continue

        dst.parent.mkdir(parents=True, exist_ok=True)
        # write_bytes keeps mtime current, which is what Unity's importer keys on.
        dst.write_bytes(data)
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
    parser.add_argument(
        "--ref",
        default="origin/main",
        help="backend ref to read from (default origin/main; pass '' to read the working tree)",
    )
    args = parser.parse_args()

    source = backend_root(args.backend)
    ref = args.ref or None

    if not source.is_dir():
        if require_contract_sync():
            print(
                "error: backend contract not found "
                f"(required sync check): {source}\n"
                "Set --backend to Fineage-Backend/src/Fineage.Net.Contract "
                "or checkout the Backend repo.",
                file=sys.stderr,
            )
            return 1
        print(f"note: backend contract not found at {source}; skipping sync check")
        return 0

    backend_repo = source.parents[1] if len(source.parents) > 1 else None
    if backend_repo is not None and not (backend_repo / ".git").exists():
        backend_repo = None
    if ref and backend_repo is None:
        print(f"note: {source} is not inside a git checkout; reading the working tree")
        ref = None

    if args.write:
        changed = write_contract(source, CLIENT, ref, backend_repo)
        print("contract already in sync" if changed == 0 else f"synced {changed} file(s)")
        return 0

    missing, differing = compare(source, CLIENT, ref, backend_repo)

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
