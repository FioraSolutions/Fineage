"""Cloudflare hostnames for the shared Fineage playtest.

Keep these in sync with PlaytestEndpoint in
l2-unity/Assets/Scripts/Networking/Playtest/PlaytestEndpoint.cs.
Secrets and workflow_dispatch inputs override them.
"""

PUBLIC_HOST = "fineage.evpoint.by"
LOGIN_HOST = "login.fineage.evpoint.by"
GAME_HOST = "game.fineage.evpoint.by"
LOGIN_PORT = 2106
GAME_PORT = 7777
LANDING_URL = "https://fineage.evpoint.by"


def endpoint_defaults() -> dict[str, object]:
    return {
        "host": PUBLIC_HOST,
        "loginHost": LOGIN_HOST,
        "gameHost": GAME_HOST,
        "loginPort": LOGIN_PORT,
        "gamePort": GAME_PORT,
    }
