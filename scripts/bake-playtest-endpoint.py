#!/usr/bin/env python3
"""Write the playtest endpoint JSON that release builds ship."""

from __future__ import annotations

import argparse
import json
import os
import sys
import tempfile
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
STREAMING = REPO_ROOT / "l2-unity/Assets/StreamingAssets/fineage-endpoint.json"
RESOURCES = REPO_ROOT / "l2-unity/Assets/Resources/FineagePlaytest.json"
DOC = "Docs/PLAYTEST_RELEASE.md"


def optional_port(env: dict[str, str], name: str) -> int:
    raw = env.get(name, "").strip()
    if not raw:
        return 0
    try:
        value = int(raw)
    except ValueError as exc:
        raise SystemExit(f"{name} must be an integer port, got {raw!r}") from exc
    if not 1 <= value <= 65535:
        raise SystemExit(f"{name} must be 1-65535")
    return value


def endpoint_from_env(env: dict[str, str], *, allow_empty: bool) -> dict[str, object]:
    host = env.get("FINEAGE_PUBLIC_HOST", "").strip()
    login_host = env.get("FINEAGE_LOGIN_HOST", "").strip()
    game_host = env.get("FINEAGE_GAME_HOST", "").strip()
    if not allow_empty and not (host or login_host or game_host):
        raise SystemExit(
            "FINEAGE_PUBLIC_HOST is empty. Set the repo secret or workflow_dispatch input.\n"
            f"See {DOC}."
        )
    login_port = optional_port(env, "FINEAGE_LOGIN_PORT")
    game_port = optional_port(env, "FINEAGE_GAME_PORT")
    if (host or login_host) and login_port == 0:
        login_port = 2106
    return {
        "host": host,
        "loginHost": login_host,
        "gameHost": game_host,
        "loginPort": login_port,
        "gamePort": game_port,
    }


def write_endpoint(directory: Path | None, payload: dict[str, object]) -> None:
    text = json.dumps(payload, indent=2) + "\n"
    if directory is None:
        targets = (STREAMING, RESOURCES)
    else:
        targets = (
            directory / "fineage-endpoint.json",
            directory / "FineagePlaytest.json",
        )
    for path in targets:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding="utf-8")
        print(f"wrote {path}")


def self_test() -> int:
    for path in (STREAMING, RESOURCES):
        data = json.loads(path.read_text(encoding="utf-8"))
        if data.get("host") or data.get("loginHost") or data.get("gameHost"):
            print(f"{path} must keep an empty host in git. Bake at build time.", file=sys.stderr)
            return 1
        if data.get("loginPort") != 0 or data.get("gamePort") != 0:
            print(f"{path} template ports must stay 0", file=sys.stderr)
            return 1

    try:
        endpoint_from_env({}, allow_empty=False)
    except SystemExit as exc:
        if "FINEAGE_PUBLIC_HOST" not in str(exc):
            raise
    else:
        print("empty host should fail", file=sys.stderr)
        return 1

    payload = endpoint_from_env(
        {
            "FINEAGE_PUBLIC_HOST": "play.example",
            "FINEAGE_GAME_PORT": "7777",
        },
        allow_empty=False,
    )
    assert payload["host"] == "play.example"
    assert payload["loginPort"] == 2106
    assert payload["gamePort"] == 7777

    with tempfile.TemporaryDirectory() as tmp:
        write_endpoint(Path(tmp), payload)
        baked = json.loads((Path(tmp) / "FineagePlaytest.json").read_text(encoding="utf-8"))
        assert baked == payload

    print("bake-playtest-endpoint self-test OK")
    return 0


def main(argv: list[str]) -> int:
    if "--self-test" in argv:
        return self_test()

    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--output-dir",
        type=Path,
        help="Write both JSON files here instead of the Unity project (tests).",
    )
    parser.add_argument(
        "--allow-empty",
        action="store_true",
        help="Write an empty host. Release builds should not use this.",
    )
    args = parser.parse_args(argv)
    payload = endpoint_from_env(dict(os.environ), allow_empty=args.allow_empty)
    write_endpoint(args.output_dir, payload)
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
