#!/usr/bin/env python3
"""Fail a client release early when Unity license or playtest host secrets are missing."""

from __future__ import annotations

import os
import sys
from collections.abc import Mapping

from playtest_hosts import (
    GAME_HOST,
    GAME_PORT,
    LOCAL_GAME_PORT,
    LOCAL_LOGIN_PORT,
    LOGIN_HOST,
    LOGIN_PORT,
    PUBLIC_HOST,
    host_points_at_loopback,
)

DOC = "Docs/PLAYTEST_RELEASE.md"

_HOST_SECRETS = (
    "FINEAGE_PUBLIC_HOST",
    "FINEAGE_LOGIN_HOST",
    "FINEAGE_GAME_HOST",
)


def problems(env: Mapping[str, str]) -> list[str]:
    issues: list[str] = []
    email = bool(env.get("UNITY_EMAIL", "").strip())
    password = bool(env.get("UNITY_PASSWORD", "").strip())
    license_file = bool(env.get("UNITY_LICENSE", "").strip())
    serial = bool(env.get("UNITY_SERIAL", "").strip())
    # Personal accounts cannot download an offline .ulf. game-ci accepts a
    # floating Personal login from email + password alone.
    personal_login = email and password
    personal_ulf = license_file and email and password
    professional = serial and email and password
    if not (personal_login or personal_ulf or professional):
        issues.append(
            "Unity license: UNITY_EMAIL + UNITY_PASSWORD (Personal login), "
            "UNITY_LICENSE + UNITY_EMAIL + UNITY_PASSWORD (Personal .ulf), "
            "or UNITY_SERIAL + UNITY_EMAIL + UNITY_PASSWORD (Pro/Plus)"
        )
    for name in _HOST_SECRETS:
        if host_points_at_loopback(env.get(name, "")):
            issues.append(
                f"{name} contains 127.0.0.1 or localhost. "
                "Leave the secret unset so the release keeps the Cloudflare host."
            )
    for name, banned, public_port in (
        ("FINEAGE_LOGIN_PORT", LOCAL_LOGIN_PORT, LOGIN_PORT),
        ("FINEAGE_GAME_PORT", LOCAL_GAME_PORT, GAME_PORT),
    ):
        raw = env.get(name, "").strip()
        if not raw:
            continue
        try:
            value = int(raw)
        except ValueError:
            issues.append(f"{name} must be an integer port")
            continue
        if value == banned:
            issues.append(
                f"{name} is {banned} (local Fineage-Backend). "
                f"Leave it unset to keep {public_port}."
            )
    return issues


def render(issues: list[str]) -> str:
    lines = [
        "Release client build stopped before Unity starts.",
        "",
        "Problems:",
    ]
    lines.extend(f"- {item}" for item in issues)
    lines.extend(
        [
            "",
            "Set Unity license secrets on the GitHub repo: Settings → Secrets and variables → Actions.",
            "Do not commit a license file. Playtest host secrets are optional.",
            "Leave FINEAGE_PUBLIC_HOST, FINEAGE_LOGIN_HOST, FINEAGE_GAME_HOST, FINEAGE_LOGIN_PORT,",
            "and FINEAGE_GAME_PORT unset to keep the Cloudflare hosts",
            f"({PUBLIC_HOST}, {LOGIN_HOST}:{LOGIN_PORT}, {GAME_HOST}:{GAME_PORT}).",
            "Do not set a host to 127.0.0.1 or localhost, login port to 9106, or game port to 9777.",
            f"See {DOC}.",
        ]
    )
    return "\n".join(lines)


def self_test() -> int:
    canary = "CANARY-SECRET-VALUE"
    assert problems({})
    assert problems({"FINEAGE_PUBLIC_HOST": "play.example"})
    assert not problems(
        {
            "UNITY_EMAIL": "dev@example.com",
            "UNITY_PASSWORD": canary,
        }
    )
    assert not problems(
        {
            "UNITY_LICENSE": canary,
            "UNITY_EMAIL": "dev@example.com",
            "UNITY_PASSWORD": canary,
        }
    )
    assert not problems(
        {
            "FINEAGE_LOGIN_HOST": "login.example",
            "FINEAGE_GAME_HOST": "game.example",
            "UNITY_SERIAL": canary,
            "UNITY_EMAIL": "dev@example.com",
            "UNITY_PASSWORD": canary,
        }
    )
    assert problems({"UNITY_EMAIL": "dev@example.com"})
    assert problems({"UNITY_PASSWORD": canary})
    assert problems(
        {
            "UNITY_EMAIL": "   ",
            "UNITY_PASSWORD": "   ",
        }
    )
    assert problems(
        {
            "FINEAGE_PUBLIC_HOST": "play.example",
            "UNITY_LICENSE": canary,
        }
    )
    text = render(problems({"UNITY_LICENSE": canary, "UNITY_PASSWORD": canary}))
    if canary in text:
        print("secret value leaked into the error text", file=sys.stderr)
        return 1

    unity = {"UNITY_EMAIL": "dev@example.com", "UNITY_PASSWORD": canary}
    assert not problems(unity)
    assert not problems(
        {
            **unity,
            "FINEAGE_PUBLIC_HOST": PUBLIC_HOST,
            "FINEAGE_LOGIN_HOST": LOGIN_HOST,
            "FINEAGE_GAME_HOST": GAME_HOST,
            "FINEAGE_LOGIN_PORT": str(LOGIN_PORT),
            "FINEAGE_GAME_PORT": str(GAME_PORT),
        }
    )
    assert problems({**unity, "FINEAGE_PUBLIC_HOST": "127.0.0.1"})
    assert problems({**unity, "FINEAGE_LOGIN_HOST": "127.0.0.1"})
    assert problems({**unity, "FINEAGE_GAME_HOST": "localhost"})
    assert problems({**unity, "FINEAGE_LOGIN_HOST": "http://127.0.0.1"})
    assert problems({**unity, "FINEAGE_LOGIN_PORT": str(LOCAL_LOGIN_PORT)})
    assert problems({**unity, "FINEAGE_GAME_PORT": str(LOCAL_GAME_PORT)})
    loopback = render(
        problems(
            {
                **unity,
                "FINEAGE_LOGIN_HOST": "127.0.0.1",
                "FINEAGE_GAME_PORT": str(LOCAL_GAME_PORT),
            }
        )
    )
    if canary in loopback or "127.0.0.1" not in loopback:
        print("loopback prepare error is missing or leaked a secret", file=sys.stderr)
        return 1
    print("check-release-secrets self-test OK")
    return 0


def main(argv: list[str]) -> int:
    if "--self-test" in argv:
        return self_test()

    missing = problems(os.environ)
    if not missing:
        print("Release secrets are set (values not printed).")
        return 0

    message = render(missing)
    print(message, file=sys.stderr)
    print(f"::error title=Release prepare failed::{message.replace(chr(10), ' ')}")
    return 1


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
