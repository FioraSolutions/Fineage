#!/usr/bin/env python3
"""Fail a client release early when Unity license or playtest host secrets are missing."""

from __future__ import annotations

import os
import sys
from collections.abc import Mapping

DOC = "Docs/PLAYTEST_RELEASE.md"


def problems(env: Mapping[str, str]) -> list[str]:
    missing: list[str] = []
    email = bool(env.get("UNITY_EMAIL", "").strip())
    password = bool(env.get("UNITY_PASSWORD", "").strip())
    license_file = bool(env.get("UNITY_LICENSE", "").strip())
    serial = bool(env.get("UNITY_SERIAL", "").strip())
    personal = license_file and email and password
    professional = serial and email and password
    if not personal and not professional:
        missing.append(
            "Unity license: UNITY_LICENSE + UNITY_EMAIL + UNITY_PASSWORD (Personal .ulf), "
            "or UNITY_SERIAL + UNITY_EMAIL + UNITY_PASSWORD (Pro/Plus)"
        )
    return missing


def render(missing: list[str]) -> str:
    lines = [
        "Release client build needs secrets before Unity starts.",
        "",
        "Missing:",
    ]
    lines.extend(f"- {item}" for item in missing)
    lines.extend(
        [
            "",
            "Set them on the GitHub repo: Settings → Secrets and variables → Actions.",
            "Do not commit a license file. Playtest hostnames already have Cloudflare defaults;",
            "FINEAGE_PUBLIC_HOST, FINEAGE_LOGIN_HOST, and FINEAGE_GAME_HOST only override them.",
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
    print(f"::error title=Release secrets missing::{message.replace(chr(10), ' ')}")
    return 1


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
