#!/usr/bin/env python3
"""Write the playtest endpoint JSON that release builds ship."""

from __future__ import annotations

import argparse
import json
import os
import sys
import tempfile
from pathlib import Path

from playtest_hosts import GAME_HOST, LOGIN_HOST, LOGIN_PORT, PUBLIC_HOST, endpoint_defaults

REPO_ROOT = Path(__file__).resolve().parents[1]
STREAMING = REPO_ROOT / "l2-unity/Assets/StreamingAssets/fineage-endpoint.json"
RESOURCES = REPO_ROOT / "l2-unity/Assets/Resources/FineagePlaytest.json"
ENDPOINT_SOURCE = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Playtest/PlaytestEndpoint.cs"


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


def pick(env: dict[str, str], name: str, default: object) -> object:
    raw = env.get(name, "").strip()
    return raw or default


def endpoint_from_env(env: dict[str, str]) -> dict[str, object]:
    defaults = endpoint_defaults()
    login_port = optional_port(env, "FINEAGE_LOGIN_PORT") or defaults["loginPort"]
    game_port = optional_port(env, "FINEAGE_GAME_PORT") or defaults["gamePort"]
    return {
        "host": pick(env, "FINEAGE_PUBLIC_HOST", defaults["host"]),
        "loginHost": pick(env, "FINEAGE_LOGIN_HOST", defaults["loginHost"]),
        "gameHost": pick(env, "FINEAGE_GAME_HOST", defaults["gameHost"]),
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
    defaults = endpoint_defaults()
    source = ENDPOINT_SOURCE.read_text(encoding="utf-8")
    for marker in (PUBLIC_HOST, LOGIN_HOST, GAME_HOST):
        if f'"{marker}"' not in source:
            print(f"{ENDPOINT_SOURCE} is missing {marker}", file=sys.stderr)
            return 1

    for path in (STREAMING, RESOURCES):
        data = json.loads(path.read_text(encoding="utf-8"))
        if data != defaults:
            print(f"{path} must match the Cloudflare defaults {defaults}", file=sys.stderr)
            return 1

    assert endpoint_from_env({}) == defaults

    payload = endpoint_from_env(
        {
            "FINEAGE_PUBLIC_HOST": "play.example",
            "FINEAGE_GAME_PORT": "7777",
        }
    )
    assert payload["host"] == "play.example"
    assert payload["loginHost"] == LOGIN_HOST
    assert payload["gameHost"] == GAME_HOST
    assert payload["loginPort"] == LOGIN_PORT
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
    args = parser.parse_args(argv)
    payload = endpoint_from_env(dict(os.environ))
    write_endpoint(args.output_dir, payload)
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
