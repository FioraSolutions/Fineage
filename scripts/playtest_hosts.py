"""Cloudflare hostnames for the shared Fineage playtest.

Keep these in sync with PlaytestEndpoint in
l2-unity/Assets/Scripts/Networking/Playtest/PlaytestEndpoint.cs.
Secrets and workflow_dispatch inputs override them. A release must not
ship 127.0.0.1, localhost, or the local Fineage-Backend ports.
"""

from __future__ import annotations

from collections.abc import Mapping

PUBLIC_HOST = "fineage.evpoint.by"
LOGIN_HOST = "login.fineage.evpoint.by"
GAME_HOST = "game.fineage.evpoint.by"
LOGIN_PORT = 2106
GAME_PORT = 7777
LANDING_URL = "https://fineage.evpoint.by"
# Local Fineage-Backend sockets. Never bake these into a playtest zip.
LOCAL_LOGIN_PORT = 9106
LOCAL_GAME_PORT = 9777

_HOST_FIELDS = (
    ("host", "FINEAGE_PUBLIC_HOST"),
    ("loginHost", "FINEAGE_LOGIN_HOST"),
    ("gameHost", "FINEAGE_GAME_HOST"),
)


def endpoint_defaults() -> dict[str, object]:
    return {
        "host": PUBLIC_HOST,
        "loginHost": LOGIN_HOST,
        "gameHost": GAME_HOST,
        "loginPort": LOGIN_PORT,
        "gamePort": GAME_PORT,
    }


def host_points_at_loopback(value: object) -> bool:
    text = str(value or "").strip().lower()
    if not text:
        return False
    if "127.0.0.1" in text or "localhost" in text:
        return True
    return text.strip("[]") == "::1"


def shipped_endpoint_problems(payload: Mapping[str, object]) -> list[str]:
    """Problems that must stop a release from writing endpoint JSON."""
    found: list[str] = []
    for key, secret in _HOST_FIELDS:
        if host_points_at_loopback(payload.get(key)):
            found.append(
                f"{secret} contains 127.0.0.1 or localhost. "
                f"Leave it unset to keep the Cloudflare host."
            )
    if payload.get("loginPort") == LOCAL_LOGIN_PORT:
        found.append(
            f"FINEAGE_LOGIN_PORT is {LOCAL_LOGIN_PORT} (local Fineage-Backend). "
            f"Playtest releases use {LOGIN_PORT}."
        )
    if payload.get("gamePort") == LOCAL_GAME_PORT:
        found.append(
            f"FINEAGE_GAME_PORT is {LOCAL_GAME_PORT} (local Fineage-Backend). "
            f"Playtest releases use {GAME_PORT}."
        )
    return found


def refuse_shipped_loopback(payload: Mapping[str, object]) -> None:
    found = shipped_endpoint_problems(payload)
    if not found:
        return
    message = (
        "Refusing to ship a playtest endpoint aimed at localhost or the local Fineage-Backend.\n"
        + "\n".join(f"- {item}" for item in found)
        + "\nLeave FINEAGE_* unset to keep the Cloudflare hosts. See Docs/PLAYTEST_RELEASE.md."
    )
    raise SystemExit(message)
