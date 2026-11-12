#!/usr/bin/env python3
"""Assert every Zone→client NetOpcode has a NetZoneDispatch case (or is explicitly outbound).

Parses direction from NetOpcode.cs XML summaries ("Zone → client" / "Client → zone").
Opcodes without a direction marker must appear in HANDLED or OUTBOUND_EXPLICIT.
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
OPCODE_PATH = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/Contract/Opcodes/NetOpcode.cs"
HANDLERS_PATH = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/NetZoneDispatch.Handlers.cs"

CONST_RE = re.compile(r"public const ushort (?P<name>\w+)\s*=\s*0x(?P<hex>[0-9A-Fa-f]+)")
CASE_RE = re.compile(r"case\s+NetOpcode\.(?P<name>\w+)\s*:")
DOC_RE = re.compile(r"^\s*///")

# Session / lobby / client→zone opcodes that are not NetZoneDispatch inbound.
OUTBOUND_EXPLICIT = frozenset(
    {
        "Hello",
        "Auth",
        "AuthResult",
        "ListCharacters",
        "CharacterRoster",
        "CreateCharacter",
        "CreateResult",
        "SelectCharacter",
        "Handoff",
        "DeleteCharacter",
        "DeleteResult",
        "RestoreCharacter",
        "ZoneAuth",
        "EnterWorld",
        "MoveInput",
        "StopMove",
        "SelectTarget",
        "AttackRequest",
        "PickupRequest",
        "DropItemRequest",
        "UseItem",
        "UnequipItem",
        "Say",
        "UseSkill",
        "ReviveRequest",
        "NpcTalk",
        "BypassRequest",
        "RequestBuyItem",
        "RequestSellItem",
        "RequestMultiSellChoose",
        "WarehouseDepositRequest",
        "WarehouseWithdrawRequest",
        "RequestQuestList",
        "RequestQuestAbort",
        "PartyInviteRequest",
        "PartyAnswerJoinRequest",
        "PartyLeaveRequest",
        "PartyKickRequest",
        "TradeRequest",
        "TradeAnswerRequest",
        "TradeAddItemRequest",
        "TradeDoneRequest",
        "ValidatePosition",
        "RequestActionUse",
        "RequestAutoSoulShot",
        "RequestShortCutReg",
        "RequestShortCutDel",
        "RequestAcquireSkillInfo",
        "RequestAcquireSkill",
        "EnchantRequest",
        "EnchantConfirm",
        "EnchantCancel",
        "ClanCreateRequest",
        "ClanInviteRequest",
        "ClanAnswerJoinRequest",
        "ClanLeaveRequest",
        "ClanKickRequest",
        "ClanDissolveRequest",
        "ClanCancelDissolveRequest",
        "TargetCancelRequest",
        "DestroyItemRequest",
        "BossStatusRequest",
        "AdminBossSchedule",
        "SiegeListRequest",
        "SiegeRegisterRequest",
        "SiegeDefenderAnswerRequest",
        "AdminCaptureCastle",
        "OlympiadStatusRequest",
        "OlympiadRegisterRequest",
        "OlympiadUnregisterRequest",
        "OlympiadLeaveRequest",
        "AdminReportOlympiadResult",
        "SevenSignsStatusRequest",
        "SevenSignsActionRequest",
        "DungeonEnterRequest",
        "SetPrivateStoreSell",
        "PrivateStoreQuit",
        "PrivateStoreListRequest",
        "PrivateStoreBuy",
        "SetPrivateStoreBuy",
        "PrivateStoreSellTo",
        "RecipeBookOpen",
        "RecipeMakeSelf",
        "SetRecipeShop",
        "RecipeShopMakeInfo",
        "RecipeShopMakeItem",
        "FriendInvite",
        "FriendAnswer",
        "FriendListRequest",
        "FriendDelete",
        "BlockRequest",
    }
)


def parse_opcodes(text: str) -> dict[str, str]:
    """name → direction ('zone', 'client', or '')."""
    out: dict[str, str] = {}
    pending_docs: list[str] = []
    for raw in text.splitlines():
        line = raw.rstrip()
        if DOC_RE.match(line):
            pending_docs.append(line)
            continue
        match = CONST_RE.search(line)
        if not match:
            if line.strip() and not line.strip().startswith("//"):
                pending_docs.clear()
            continue
        comment = "\n".join(pending_docs)
        pending_docs.clear()
        direction = ""
        if "Zone → client" in comment or "Zone -> client" in comment:
            direction = "zone"
        elif "Client → zone" in comment or "Client -> zone" in comment:
            direction = "client"
        out[match.group("name")] = direction
    return out


def parse_handled(text: str) -> set[str]:
    return {m.group("name") for m in CASE_RE.finditer(text)}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--opcode-file", type=Path, default=OPCODE_PATH)
    parser.add_argument("--handlers-file", type=Path, default=HANDLERS_PATH)
    args = parser.parse_args()

    if not args.opcode_file.is_file():
        print(f"missing opcode file: {args.opcode_file}", file=sys.stderr)
        return 1
    if not args.handlers_file.is_file():
        print(f"missing handlers file: {args.handlers_file}", file=sys.stderr)
        return 1

    opcodes = parse_opcodes(args.opcode_file.read_text(encoding="utf-8"))
    handled = parse_handled(args.handlers_file.read_text(encoding="utf-8"))

    missing: list[str] = []
    for name, direction in sorted(opcodes.items()):
        if name in handled:
            continue
        if direction == "client" or name in OUTBOUND_EXPLICIT:
            continue
        if direction == "zone":
            missing.append(f"{name} (Zone→client, no NetZoneDispatch case)")
            continue
        missing.append(f"{name} (no direction comment; not handled and not OUTBOUND_EXPLICIT)")

    unknown_handled = sorted(name for name in handled if name not in opcodes)
    if unknown_handled:
        print("NetZoneDispatch references unknown NetOpcode names:", file=sys.stderr)
        for name in unknown_handled:
            print(f"  {name}", file=sys.stderr)
        return 1

    if missing:
        print(
            "Inbound zone opcodes missing from NetZoneDispatch (GPT opcode audit):",
            file=sys.stderr,
        )
        for line in missing:
            print(f"  {line}", file=sys.stderr)
        print(
            "Add a case, mark Client→zone in the NetOpcode summary, or extend OUTBOUND_EXPLICIT.",
            file=sys.stderr,
        )
        return 1

    zone_count = sum(1 for d in opcodes.values() if d == "zone")
    print(
        f"ok: {len(handled)} NetZoneDispatch cases; "
        f"{zone_count} Zone→client opcodes covered; {len(opcodes)} total opcodes"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
