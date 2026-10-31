using System;
using System.IO;

/// <summary>MessagePack array bodies matching tools/zone-server Wire/Messages.cs.</summary>
public static partial class ZoneV2Messages
{
    public static byte[] EncodeHello(int protocolVersion, string clientBuild)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, protocolVersion);
            ZoneV2MsgPack.WriteString(w, clientBuild ?? "");
        }, 2);
    }

    public static byte[] EncodeAuth(string account, string password)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteString(w, account ?? "");
            ZoneV2MsgPack.WriteString(w, password ?? "");
        }, 2);
    }

    public static byte[] EncodeEnterWorld(int characterSlot)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, characterSlot), 1);
    }

    /// <summary>
    /// SelectCharacter keys 0 id, 1 name, 2 slot. A positive id wins. Slot −1 means unspecified.
    /// </summary>
    public static byte[] EncodeSelectCharacter(int characterId, string name, int slot)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, characterId);
            ZoneV2MsgPack.WriteString(w, name ?? "");
            ZoneV2MsgPack.WriteInt(w, slot);
        }, 3);
    }

    public static byte[] EncodeSelectCharacter(int characterId, string name)
    {
        return EncodeSelectCharacter(characterId, name, -1);
    }

    /// <summary>Delete or restore: 0 slot (−1 when using id), 1 characterId (0 when using slot). A positive id wins.</summary>
    public static byte[] EncodeDeleteCharacter(int slot, int characterId)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, slot);
            ZoneV2MsgPack.WriteInt(w, characterId);
        }, 2);
    }

    public static byte[] EncodeRestoreCharacter(int slot, int characterId)
    {
        return EncodeDeleteCharacter(slot, characterId);
    }

    /// <summary>CreateCharacter keys 0 name, 1 classId, 2 race, 3 sex, 4 hairStyle, 5 hairColor, 6 face.</summary>
    public static byte[] EncodeCreateCharacter(string name, int classId, int race, int sex, int hairStyle, int hairColor, int face)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteString(w, name ?? "");
            ZoneV2MsgPack.WriteInt(w, classId);
            ZoneV2MsgPack.WriteInt(w, race);
            ZoneV2MsgPack.WriteInt(w, sex);
            ZoneV2MsgPack.WriteInt(w, hairStyle);
            ZoneV2MsgPack.WriteInt(w, hairColor);
            ZoneV2MsgPack.WriteInt(w, face);
        }, 7);
    }

    /// <summary>Zone Auth password is the handoff token. The account password must not be passed here.</summary>
    public static byte[] EncodeZoneAuth(ZoneV2Handoff handoff)
    {
        return EncodeAuth(handoff.Account, handoff.Token ?? "");
    }

    public static byte[] EncodeMoveTo(int objectId, int destX, int destY, int destZ, int originX, int originY, int originZ)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, objectId);
            ZoneV2MsgPack.WriteInt(w, destX);
            ZoneV2MsgPack.WriteInt(w, destY);
            ZoneV2MsgPack.WriteInt(w, destZ);
            ZoneV2MsgPack.WriteInt(w, originX);
            ZoneV2MsgPack.WriteInt(w, originY);
            ZoneV2MsgPack.WriteInt(w, originZ);
        }, 7);
    }

    public static byte[] EncodeSelectTarget(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    public static byte[] EncodeAttackRequest(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    /// <summary>MessagePack array [itemId, count] — Fineage-Backend <c>GrantItemRequestMsg</c>.</summary>
    public static byte[] EncodeGrantItemRequest(int itemId, int count)
    {
        if (count < 1)
            count = 1;
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, itemId);
            ZoneV2MsgPack.WriteInt(w, count);
        }, 2);
    }

    /// <summary>MessagePack array [objectId] — Fineage-Backend <c>PickupRequestMsg</c>.</summary>
    public static byte[] EncodePickupRequest(int groundObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, groundObjectId), 1);
    }

    /// <summary>MessagePack array [objectId, count, x, y, z] — Backend <c>DropItemRequestMsg</c>.</summary>
    public static byte[] EncodeDropItemRequest(int objectId, int count, int x, int y, int z)
    {
        if (count < 1)
            count = 1;
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, objectId);
            ZoneV2MsgPack.WriteInt(w, count);
            ZoneV2MsgPack.WriteInt(w, x);
            ZoneV2MsgPack.WriteInt(w, y);
            ZoneV2MsgPack.WriteInt(w, z);
        }, 5);
    }

    /// <summary>MessagePack array [objectId] — Fineage-Backend <c>UseItemMsg</c>.</summary>
    public static byte[] EncodeUseItem(int objectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, objectId), 1);
    }

    /// <summary>MessagePack array [paperdollSlot] — Fineage-Backend <c>UnequipItemMsg</c>.</summary>
    public static byte[] EncodeUnequipItem(int paperdollSlot)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, paperdollSlot), 1);
    }

    /// <summary>MessagePack array [itemId, count] — Fineage-Backend <c>GiveItemMsg</c>.</summary>
    public static byte[] EncodeGiveItem(int itemId, int count)
    {
        if (count < 1)
            count = 1;
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, itemId);
            ZoneV2MsgPack.WriteInt(w, count);
        }, 2);
    }

    /// <summary>MessagePack array [targetObjectId, itemDistribution].</summary>
    public static byte[] EncodePartyInviteRequest(int targetObjectId, int itemDistribution)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, targetObjectId);
            ZoneV2MsgPack.WriteInt(w, itemDistribution);
        }, 2);
    }

    /// <summary>MessagePack array [requestorObjectId, response]. Response 1 accept, 0 decline.</summary>
    public static byte[] EncodePartyAnswerJoinRequest(int requestorObjectId, int response)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, requestorObjectId);
            ZoneV2MsgPack.WriteInt(w, response);
        }, 2);
    }

    /// <summary>Empty MessagePack array — Fineage-Backend <c>PartyLeaveRequestMsg</c>.</summary>
    public static byte[] EncodePartyLeaveRequest()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    /// <summary>MessagePack array [targetObjectId].</summary>
    public static byte[] EncodePartyKickRequest(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    /// <summary>MessagePack array [targetObjectId] — Fineage-Backend <c>TradeRequestMsg</c>.</summary>
    public static byte[] EncodeTradeRequest(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    /// <summary>MessagePack array [requestorObjectId, response]. Response 1 accept, 0 decline.</summary>
    public static byte[] EncodeTradeAnswerRequest(int requestorObjectId, int response)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, requestorObjectId);
            ZoneV2MsgPack.WriteInt(w, response);
        }, 2);
    }

    /// <summary>MessagePack array [inventoryObjectId, count].</summary>
    public static byte[] EncodeTradeAddItemRequest(int inventoryObjectId, int count)
    {
        if (count < 1)
            count = 1;
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, inventoryObjectId);
            ZoneV2MsgPack.WriteInt(w, count);
        }, 2);
    }

    /// <summary>MessagePack array [response]. Response 1 confirm, 0 cancel.</summary>
    public static byte[] EncodeTradeDoneRequest(int response)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, response), 1);
    }

    /// <summary>MessagePack array [name] — Fineage-Backend <c>ClanCreateRequestMsg</c>.</summary>
    public static byte[] EncodeClanCreateRequest(string name)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteString(w, name ?? ""), 1);
    }

    /// <summary>MessagePack array [targetObjectId, pledgeType]. Pledge type 0 is the main clan.</summary>
    public static byte[] EncodeClanInviteRequest(int targetObjectId, int pledgeType)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, targetObjectId);
            ZoneV2MsgPack.WriteInt(w, pledgeType);
        }, 2);
    }

    /// <summary>MessagePack array [requestorObjectId, response]. Response 0 declines. Any other value accepts.</summary>
    public static byte[] EncodeClanAnswerJoinRequest(int requestorObjectId, int response)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, requestorObjectId);
            ZoneV2MsgPack.WriteInt(w, response);
        }, 2);
    }

    /// <summary>Empty MessagePack array — Fineage-Backend <c>ClanLeaveRequestMsg</c>.</summary>
    public static byte[] EncodeClanLeaveRequest()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    /// <summary>MessagePack array [targetObjectId].</summary>
    public static byte[] EncodeClanKickRequest(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    /// <summary>Empty MessagePack array — Fineage-Backend <c>ClanDissolveRequestMsg</c>.</summary>
    public static byte[] EncodeClanDissolveRequest()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    /// <summary>Empty MessagePack array — Fineage-Backend <c>ClanCancelDissolveRequestMsg</c>.</summary>
    public static byte[] EncodeClanCancelDissolveRequest()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    /// <summary>MessagePack array [scrollObjectId] — Fineage-Backend <c>EnchantRequestMsg</c>.</summary>
    public static byte[] EncodeEnchantRequest(int scrollObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, scrollObjectId), 1);
    }

    /// <summary>MessagePack array [targetObjectId] — Fineage-Backend <c>EnchantConfirmMsg</c>.</summary>
    public static byte[] EncodeEnchantConfirm(int targetObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, targetObjectId), 1);
    }

    /// <summary>Empty MessagePack array — Fineage-Backend <c>EnchantCancelMsg</c>.</summary>
    public static byte[] EncodeEnchantCancel()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    /// <summary>MessagePack array [skillId, targetObjectId, ctrlPressed] — Backend <c>SkillUseRequestMsg</c>.</summary>
    public static byte[] EncodeSkillUseRequest(int skillId, int targetObjectId, bool ctrlPressed)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, skillId);
            ZoneV2MsgPack.WriteInt(w, targetObjectId);
            ZoneV2MsgPack.WriteBool(w, ctrlPressed);
        }, 3);
    }

    /// <summary>MessagePack array [action] — Backend <c>ReviveRequestMsg</c>.</summary>
    public static byte[] EncodeReviveRequest(int action)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, action), 1);
    }

    /// <summary>Empty MessagePack array — Backend <c>AdminKillPlayerMsg</c>.</summary>
    public static byte[] EncodeAdminKillPlayer()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    public static bool TryDecodeChangeMoveType(byte[] body, out int objectId, out bool running)
    {
        objectId = 0;
        running = true;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            objectId = ZoneV2MsgPack.ReadInt(body, ref i);
            running = ZoneV2MsgPack.ReadBool(body, ref i);
            if (n > 2)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 2);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeDie(byte[] body, out ZoneV2Die die)
    {
        die = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            die = new ZoneV2Die
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                KillerObjectId = n > 1 ? ZoneV2MsgPack.ReadInt(body, ref i) : 0
            };
            if (n > 2)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 2);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeRevive(byte[] body, out ZoneV2Revive revive)
    {
        revive = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            revive = new ZoneV2Revive { ObjectId = ZoneV2MsgPack.ReadInt(body, ref i) };
            if (n > 1)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 1);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTeleportToLocation(byte[] body, out ZoneV2TeleportToLocation teleport)
    {
        teleport = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 4)
                return false;
            teleport = new ZoneV2TeleportToLocation
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                Heading = n > 4 ? ZoneV2MsgPack.ReadInt(body, ref i) : 0
            };
            if (n > 5)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 5);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodePartyAskJoin(byte[] body, out ZoneV2PartyAskJoin ask)
    {
        ask = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            ask = new ZoneV2PartyAskJoin
            {
                RequestorObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                RequestorName = ZoneV2MsgPack.ReadString(body, ref i),
                ItemDistribution = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodePartyJoinResult(byte[] body, out int response)
    {
        response = 0;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            response = ZoneV2MsgPack.ReadInt(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodePartyMemberList(byte[] body, out ZoneV2PartyMemberList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            list.LeaderObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.PartyId = ZoneV2MsgPack.ReadInt(body, ref i);
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                list.Members = Array.Empty<ZoneV2PartyMemberEntry>();
                return true;
            }

            int rowCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (rowCount <= 0)
            {
                list.Members = Array.Empty<ZoneV2PartyMemberEntry>();
                return true;
            }

            var rows = new ZoneV2PartyMemberEntry[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                if (!TryReadPartyMemberEntry(body, ref i, out rows[r]))
                    return false;
            }

            list.Members = rows;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    static bool TryReadPartyMemberEntry(byte[] body, ref int i, out ZoneV2PartyMemberEntry entry)
    {
        entry = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 8)
            return false;
        entry = new ZoneV2PartyMemberEntry
        {
            ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
            Name = ZoneV2MsgPack.ReadString(body, ref i),
            ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
            Level = ZoneV2MsgPack.ReadInt(body, ref i),
            CurHp = ZoneV2MsgPack.ReadInt(body, ref i),
            MaxHp = ZoneV2MsgPack.ReadInt(body, ref i),
            CurMp = ZoneV2MsgPack.ReadInt(body, ref i),
            MaxMp = ZoneV2MsgPack.ReadInt(body, ref i)
        };
        return true;
    }

    public static string FormatPartyMemberList(in ZoneV2PartyMemberList list)
    {
        if (list.Members == null || list.Members.Length == 0)
            return "Party: (none)";
        var names = new string[list.Members.Length];
        for (int i = 0; i < list.Members.Length; i++)
            names[i] = list.Members[i].Name ?? "?";
        return "Party (" + list.Members.Length + "): " + string.Join(", ", names);
    }

    public static bool TryDecodeTradeAsk(byte[] body, out ZoneV2TradeAsk ask)
    {
        ask = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            ask = new ZoneV2TradeAsk
            {
                RequestorObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                RequestorName = ZoneV2MsgPack.ReadString(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTradeRequestResult(byte[] body, out int response)
    {
        response = 0;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            response = ZoneV2MsgPack.ReadInt(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTradeStart(byte[] body, out ZoneV2TradeStart start)
    {
        start = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            start = new ZoneV2TradeStart
            {
                PartnerObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                PartnerName = ZoneV2MsgPack.ReadString(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTradeOfferUpdate(byte[] body, out ZoneV2TradeOfferUpdate update)
    {
        update = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 5)
                return false;
            update = new ZoneV2TradeOfferUpdate
            {
                IsOwn = ZoneV2MsgPack.ReadInt(body, ref i),
                InventoryObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                ItemId = ZoneV2MsgPack.ReadInt(body, ref i),
                Count = ZoneV2MsgPack.ReadInt(body, ref i),
                EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTradeConfirmState(byte[] body, out ZoneV2TradeConfirmState state)
    {
        state = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            state = new ZoneV2TradeConfirmState
            {
                SelfConfirmed = ZoneV2MsgPack.ReadInt(body, ref i),
                PartnerConfirmed = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTradeFinished(byte[] body, out int response)
    {
        response = 0;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            response = ZoneV2MsgPack.ReadInt(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    /// <summary>MessagePack array [text, chatType] — Fineage-Backend <c>SayMsg</c> opcode 8. No whisper-target key.</summary>
    public static byte[] EncodeSay(string text, int chatType)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteString(w, text ?? "");
            ZoneV2MsgPack.WriteInt(w, chatType);
        }, 2);
    }

    public static bool TryDecodeHelloAck(byte[] body, out int protocolVersion, out string serverName)
    {
        protocolVersion = 0;
        serverName = "";
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            protocolVersion = ZoneV2MsgPack.ReadInt(body, ref i);
            serverName = ZoneV2MsgPack.ReadString(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeAuthOk(byte[] body, out int sessionId, out string characterName)
    {
        sessionId = 0;
        characterName = "";
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            sessionId = ZoneV2MsgPack.ReadInt(body, ref i);
            characterName = ZoneV2MsgPack.ReadString(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodePlayerSpawned(byte[] body, out ZoneV2PlayerSpawned spawned)
    {
        spawned = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 8)
                return false;
            spawned = new ZoneV2PlayerSpawned
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                RunSpeed = ZoneV2MsgPack.ReadFloat(body, ref i),
                WalkSpeed = ZoneV2MsgPack.ReadFloat(body, ref i),
                Running = ZoneV2MsgPack.ReadBool(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeMoveTo(byte[] body, out ZoneV2MoveTo move)
    {
        move = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 7)
                return false;
            move = new ZoneV2MoveTo
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                DestX = ZoneV2MsgPack.ReadInt(body, ref i),
                DestY = ZoneV2MsgPack.ReadInt(body, ref i),
                DestZ = ZoneV2MsgPack.ReadInt(body, ref i),
                OriginX = ZoneV2MsgPack.ReadInt(body, ref i),
                OriginY = ZoneV2MsgPack.ReadInt(body, ref i),
                OriginZ = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeCharInfo(byte[] body, out ZoneV2CharInfo info)
    {
        info = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 6)
                return false;
            info = new ZoneV2CharInfo
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                ClassId = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            int used = 6;
            // Key 6 is the paperdoll. Older zone servers stop at class id.
            // Keys 7–8 are clan id and crest. Key 9 is the enchant of each paperdoll slot.
            if (n > used)
            {
                if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
                {
                }
                else if (ZoneV2MsgPack.IsArray(body, i))
                {
                    info.PaperdollItemIds = ZoneV2MsgPack.ReadIntArray(body, ref i);
                    info.HasPaperdoll = true;
                }
                else
                {
                    ZoneV2MsgPack.SkipValue(body, ref i);
                }

                used++;
            }

            if (n > used)
            {
                if (ZoneV2MsgPack.IsInt(body, i))
                {
                    info.ClanId = ZoneV2MsgPack.ReadInt(body, ref i);
                    info.HasClan = true;
                }
                else
                {
                    ZoneV2MsgPack.SkipValue(body, ref i);
                }

                used++;
            }

            if (n > used)
            {
                if (ZoneV2MsgPack.IsInt(body, i))
                    info.ClanCrestId = ZoneV2MsgPack.ReadInt(body, ref i);
                else
                    ZoneV2MsgPack.SkipValue(body, ref i);
                used++;
            }

            if (n > used)
            {
                if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
                {
                }
                else if (ZoneV2MsgPack.IsArray(body, i))
                {
                    info.PaperdollEnchant = ZoneV2MsgPack.ReadIntArray(body, ref i);
                }
                else
                {
                    ZoneV2MsgPack.SkipValue(body, ref i);
                }

                used++;
            }

            if (n > used)
                ZoneV2MsgPack.SkipValues(body, ref i, n - used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeNpcInfo(byte[] body, out ZoneV2NpcInfo info)
    {
        info = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 5)
                return false;
            int objectId = ZoneV2MsgPack.ReadInt(body, ref i);
            int templateId = ZoneV2MsgPack.ReadInt(body, ref i);
            int x;
            int y;
            int z;
            string name = "";
            int used;
            bool attackable = true;
            string title = "";
            string npcType = "";
            // Fineage-Backend key 2 is the npc name. Legacy zone-server key 2 is x.
            if (n >= 6 && ZoneV2MsgPack.IsStringOrNil(body, i))
            {
                name = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
                x = ZoneV2MsgPack.ReadInt(body, ref i);
                y = ZoneV2MsgPack.ReadInt(body, ref i);
                z = ZoneV2MsgPack.ReadInt(body, ref i);
                used = 6;
                if (n > used)
                {
                    ZoneV2MsgPack.ReadInt(body, ref i);
                    used++;
                }

                if (n > used)
                {
                    attackable = ZoneV2MsgPack.ReadBool(body, ref i);
                    used++;
                }

                float runSpeed = 0f;
                float walkSpeed = 0f;
                if (n > used)
                {
                    runSpeed = ZoneV2MsgPack.ReadFloat(body, ref i);
                    used++;
                }

                if (n > used)
                {
                    walkSpeed = ZoneV2MsgPack.ReadFloat(body, ref i);
                    used++;
                }

                if (n > used)
                {
                    title = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
                    used++;
                }

                if (n > used)
                {
                    npcType = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
                    used++;
                }

                if (n > used)
                    ZoneV2MsgPack.SkipValues(body, ref i, n - used);

                info = new ZoneV2NpcInfo
                {
                    ObjectId = objectId,
                    NpcTemplateId = templateId,
                    Name = name,
                    X = x,
                    Y = y,
                    Z = z,
                    Attackable = attackable,
                    RunSpeed = runSpeed,
                    WalkSpeed = walkSpeed,
                    Title = title,
                    NpcType = npcType
                };
                return true;
            }
            else
            {
                x = ZoneV2MsgPack.ReadInt(body, ref i);
                y = ZoneV2MsgPack.ReadInt(body, ref i);
                z = ZoneV2MsgPack.ReadInt(body, ref i);
                used = 5;
            }

            if (n > used)
                ZoneV2MsgPack.SkipValues(body, ref i, n - used);

            info = new ZoneV2NpcInfo
            {
                ObjectId = objectId,
                NpcTemplateId = templateId,
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Attackable = attackable,
                Title = title,
                NpcType = npcType
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeDeleteObject(byte[] body, out int objectId)
    {
        objectId = 0;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            objectId = ZoneV2MsgPack.ReadInt(body, ref i);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    /// <summary>Backend <c>SpawnItemMsg</c> keys 0–7 (MessagePack array).</summary>
    public static bool TryDecodeSpawnItem(byte[] body, out ZoneV2SpawnItem item)
    {
        item = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 7)
                return false;
            item = new ZoneV2SpawnItem
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                ItemId = ZoneV2MsgPack.ReadInt(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                Stackable = ZoneV2MsgPack.ReadBool(body, ref i),
                Count = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n >= 8)
                item.Unknown = ZoneV2MsgPack.ReadInt(body, ref i);
            else
                item.Unknown = 0;
            if (n > 8)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 8);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    /// <summary>Backend <c>DropItemMsg</c> keys 0–8 (MessagePack array).</summary>
    public static bool TryDecodeDropItem(byte[] body, out ZoneV2DropItem item)
    {
        item = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 8)
                return false;
            item = new ZoneV2DropItem
            {
                CharObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                ItemId = ZoneV2MsgPack.ReadInt(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                Stackable = ZoneV2MsgPack.ReadBool(body, ref i),
                Count = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n >= 9)
                item.Unknown = ZoneV2MsgPack.ReadInt(body, ref i);
            else
                item.Unknown = 0;
            if (n > 9)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 9);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    /// <summary>Normalized spawn fields shared with <see cref="ZoneV2DomainMap.ToSpawnItemDto"/> (testable without Unity DTOs).</summary>
    public static SpawnItemApplyFields ToSpawnItemApplyFields(ZoneV2SpawnItem item) =>
        new SpawnItemApplyFields
        {
            ObjectId = item.ObjectId,
            ItemId = item.ItemId,
            X = item.X,
            Y = item.Y,
            Z = item.Z,
            Stackable = item.Stackable,
            Count = item.Count,
            Unknown = item.Unknown
        };

    public static bool TryDecodeError(byte[] body, out string code, out string message)
    {
        code = "";
        message = "";
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            code = ZoneV2MsgPack.ReadString(body, ref i);
            message = ZoneV2MsgPack.ReadString(body, ref i);
            if (n > 2)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 2);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeLoginOk(byte[] body, out string account)
    {
        account = "";
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            account = ZoneV2MsgPack.ReadString(body, ref i);
            if (n > 1)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 1);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeCharacterList(byte[] body, out ZoneV2CharacterSummary[] characters)
    {
        characters = Array.Empty<ZoneV2CharacterSummary>();
        try
        {
            int i = 0;
            int outer = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (outer < 1)
                return false;
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                characters = Array.Empty<ZoneV2CharacterSummary>();
                return true;
            }

            int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (count < 0)
                return false;
            ZoneV2CharacterSummary[] list = new ZoneV2CharacterSummary[count];
            for (int c = 0; c < count; c++)
            {
                if (!TryReadCharacterSummary(body, ref i, out list[c]))
                    return false;
            }

            if (outer > 1)
                ZoneV2MsgPack.SkipValues(body, ref i, outer - 1);
            characters = list;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeHandoff(byte[] body, out ZoneV2Handoff handoff)
    {
        handoff = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 7)
                return false;
            handoff = new ZoneV2Handoff
            {
                Account = ZoneV2MsgPack.ReadString(body, ref i),
                CharacterName = ZoneV2MsgPack.ReadString(body, ref i),
                CharacterId = ZoneV2MsgPack.ReadInt(body, ref i),
                ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
                Token = ZoneV2MsgPack.ReadString(body, ref i),
                ZoneHost = ZoneV2MsgPack.ReadString(body, ref i),
                ZonePort = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n >= 8)
                handoff.ExpiresUnixSeconds = ZoneV2MsgPack.ReadInt64(body, ref i);
            if (n > 8)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 8);
            return !string.IsNullOrEmpty(handoff.Token);
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeCharacterCreated(byte[] body, out ZoneV2CharacterCreated created)
    {
        created = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 8)
                return false;
            created = new ZoneV2CharacterCreated
            {
                CharacterId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i),
                ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
                Race = ZoneV2MsgPack.ReadInt(body, ref i),
                Sex = ZoneV2MsgPack.ReadInt(body, ref i),
                HairStyle = ZoneV2MsgPack.ReadInt(body, ref i),
                HairColor = ZoneV2MsgPack.ReadInt(body, ref i),
                Face = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n >= 9)
                created.X = ZoneV2MsgPack.ReadInt(body, ref i);
            if (n >= 10)
                created.Y = ZoneV2MsgPack.ReadInt(body, ref i);
            if (n >= 11)
                created.Z = ZoneV2MsgPack.ReadInt(body, ref i);
            if (n > 11)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 11);
            return created.CharacterId > 0;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeCharacterDeleted(byte[] body, out ZoneV2CharacterDeleted deleted)
    {
        deleted = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            deleted = new ZoneV2CharacterDeleted
            {
                Slot = ZoneV2MsgPack.ReadInt(body, ref i),
                CharacterId = ZoneV2MsgPack.ReadInt(body, ref i),
                HardDeleted = ZoneV2MsgPack.ReadBool(body, ref i)
            };
            if (n > 3)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 3);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeCharacterDeleteFailed(byte[] body, out ZoneV2CharacterDeleteFailed failed)
    {
        failed = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 4)
                return false;
            failed = new ZoneV2CharacterDeleteFailed
            {
                Slot = ZoneV2MsgPack.ReadInt(body, ref i),
                CharacterId = ZoneV2MsgPack.ReadInt(body, ref i),
                Code = ZoneV2MsgPack.ReadString(body, ref i),
                Message = ZoneV2MsgPack.ReadString(body, ref i)
            };
            if (n > 4)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 4);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    static bool TryReadCharacterSummary(byte[] body, ref int i, out ZoneV2CharacterSummary row)
    {
        row = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 6)
            return false;
        row = new ZoneV2CharacterSummary
        {
            CharacterId = ZoneV2MsgPack.ReadInt(body, ref i),
            Name = ZoneV2MsgPack.ReadString(body, ref i),
            ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
            Race = ZoneV2MsgPack.ReadInt(body, ref i),
            Sex = ZoneV2MsgPack.ReadInt(body, ref i),
            Level = ZoneV2MsgPack.ReadInt(body, ref i)
        };
        if (n >= 7)
            row.Slot = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 8)
            row.HairStyle = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 9)
            row.HairColor = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 10)
            row.Face = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 11)
            row.X = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 12)
            row.Y = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 13)
            row.Z = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 14)
            row.DeleteSecondsLeft = ZoneV2MsgPack.ReadInt(body, ref i);
        if (n >= 15)
            row.IsLastPlayed = ZoneV2MsgPack.ReadBool(body, ref i);
        // LobbyProtocol CharacterSummaryMsg keys 15–25 (vitals + paperdoll). Contract length is 26.
        // Older logins stop at 15 or 25; n>=25 always carries vitals — read them without soft-skip.
        if (n >= 25)
        {
            row.CurHp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.MaxHp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.CurMp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.MaxMp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.CurCp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.MaxCp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.Exp = ZoneV2MsgPack.ReadInt64(body, ref i);
            row.Sp = ZoneV2MsgPack.ReadInt(body, ref i);
            row.Karma = ZoneV2MsgPack.ReadInt(body, ref i);
            row.WeaponItemId = ZoneV2MsgPack.ReadInt(body, ref i);
            if (n >= 26)
            {
                row.Paperdoll = ZoneV2MsgPack.ReadIntArray(body, ref i);
                if (n > 26)
                    ZoneV2MsgPack.SkipValues(body, ref i, n - 26);
            }
            else if (n > 25)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 25);
            return true;
        }

        // Partial / experimental lengths between 16 and 24: only consume leading ints.
        int read = 15;
        while (read < n)
        {
            if (!ZoneV2MsgPack.IsInt(body, i) &&
                !(read == 21 && i < body.Length && (body[i] == 0xcf || body[i] == 0xd3)))
                break;
            long value = read == 21
                ? ZoneV2MsgPack.ReadInt64(body, ref i)
                : ZoneV2MsgPack.ReadInt(body, ref i);
            switch (read)
            {
                case 15: row.CurHp = (int)value; break;
                case 16: row.MaxHp = (int)value; break;
                case 17: row.CurMp = (int)value; break;
                case 18: row.MaxMp = (int)value; break;
                case 19: row.CurCp = (int)value; break;
                case 20: row.MaxCp = (int)value; break;
                case 21: row.Exp = value; break;
                case 22: row.Sp = (int)value; break;
                case 23: row.Karma = (int)value; break;
                case 24: row.WeaponItemId = (int)value; break;
            }
            read++;
        }
        if (n > read)
            ZoneV2MsgPack.SkipValues(body, ref i, n - read);
        return true;
    }

    public static bool TryDecodeMyTargetSelected(byte[] body, out ZoneV2MyTargetSelected selected)
    {
        selected = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            selected = new ZoneV2MyTargetSelected
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                ColorHint = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeAttack(byte[] body, out ZoneV2Attack attack)
    {
        attack = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 10)
                return false;
            attack = new ZoneV2Attack
            {
                AttackerObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                TargetObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                Damage = ZoneV2MsgPack.ReadInt(body, ref i),
                Flags = (byte)ZoneV2MsgPack.ReadInt(body, ref i),
                AttackerX = ZoneV2MsgPack.ReadInt(body, ref i),
                AttackerY = ZoneV2MsgPack.ReadInt(body, ref i),
                AttackerZ = ZoneV2MsgPack.ReadInt(body, ref i),
                TargetX = ZoneV2MsgPack.ReadInt(body, ref i),
                TargetY = ZoneV2MsgPack.ReadInt(body, ref i),
                TargetZ = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static byte[] EncodeStatusUpdate(int objectId, int[] attributeIds, int[] values)
    {
        if (attributeIds == null || values == null || attributeIds.Length != values.Length)
            throw new ArgumentException("attributeIds and values must be the same length");

        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, objectId);
            ZoneV2MsgPack.WriteIntArray(w, attributeIds);
            ZoneV2MsgPack.WriteIntArray(w, values);
        }, 3);
    }

    public static bool TryDecodeStatusUpdate(byte[] body, out ZoneV2StatusUpdate status)
    {
        status = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            status = new ZoneV2StatusUpdate
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                AttributeIds = ZoneV2MsgPack.ReadIntArray(body, ref i),
                Values = ZoneV2MsgPack.ReadIntArray(body, ref i)
            };
            return status.AttributeIds != null &&
                   status.Values != null &&
                   status.AttributeIds.Length == status.Values.Length;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeItemList(byte[] body, out ZoneV2ItemEntry[] items)
    {
        items = Array.Empty<ZoneV2ItemEntry>();
        try
        {
            int i = 0;
            int outer = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (outer < 1)
                return false;
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                items = Array.Empty<ZoneV2ItemEntry>();
                return true;
            }

            int rowCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (rowCount <= 0)
            {
                items = Array.Empty<ZoneV2ItemEntry>();
                return true;
            }

            var rows = new ZoneV2ItemEntry[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                if (!TryReadItemEntry(body, ref i, out rows[r]))
                    return false;
            }

            items = rows;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeInventoryUpdate(byte[] body, out ZoneV2InventoryChange[] changes)
    {
        changes = Array.Empty<ZoneV2InventoryChange>();
        try
        {
            int i = 0;
            int outer = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (outer < 1)
                return false;
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                changes = Array.Empty<ZoneV2InventoryChange>();
                return true;
            }

            int rowCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (rowCount <= 0)
            {
                changes = Array.Empty<ZoneV2InventoryChange>();
                return true;
            }

            var rows = new ZoneV2InventoryChange[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                if (!TryReadInventoryChange(body, ref i, out rows[r]))
                    return false;
            }

            changes = rows;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeUserInfo(byte[] body, out ZoneV2UserInfo info)
    {
        info = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 9)
                return false;
            info = new ZoneV2UserInfo
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i),
                ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i),
                RunSpeed = ZoneV2MsgPack.ReadFloat(body, ref i),
                WalkSpeed = ZoneV2MsgPack.ReadFloat(body, ref i),
                Running = ZoneV2MsgPack.ReadBool(body, ref i)
            };
            if (n >= 10 && !ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                info.Paperdoll = ReadPaperdoll(body, ref i);
                info.HasPaperdoll = true;
            }

            // Keys 10–12 expertise, 13–14 clan, 15–37 vitals/stats. Older bodies stop earlier.
            if (n > 10)
                ReadUserInfoTail(body, ref i, n, ref info);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    static void ReadUserInfoTail(byte[] body, ref int i, int n, ref ZoneV2UserInfo info)
    {
        for (int key = 10; key < n; key++)
        {
            if (key == 22)
            {
                // Exp is int64 on the wire.
                if (i >= body.Length)
                    return;
                info.Exp = ZoneV2MsgPack.ReadInt64(body, ref i);
                info.HasVitals = true;
                continue;
            }

            if (!IsMsgPackInt(body, i))
            {
                ZoneV2MsgPack.SkipValues(body, ref i, n - key);
                return;
            }

            int value = ZoneV2MsgPack.ReadInt(body, ref i);
            switch (key)
            {
                case 13:
                    info.ClanId = value;
                    info.HasClan = true;
                    break;
                case 14:
                    info.ClanCrestId = value;
                    break;
                case 15:
                    info.Level = value;
                    info.HasVitals = true;
                    break;
                case 16: info.CurHp = value; info.HasVitals = true; break;
                case 17: info.MaxHp = value; info.HasVitals = true; break;
                case 18: info.CurMp = value; info.HasVitals = true; break;
                case 19: info.MaxMp = value; info.HasVitals = true; break;
                case 20: info.CurCp = value; info.HasVitals = true; break;
                case 21: info.MaxCp = value; info.HasVitals = true; break;
                case 23: info.Sp = value; info.HasVitals = true; break;
                case 24: info.Str = value; info.HasStats = true; break;
                case 25: info.Dex = value; info.HasStats = true; break;
                case 26: info.Con = value; info.HasStats = true; break;
                case 27: info.Int = value; info.HasStats = true; break;
                case 28: info.Wit = value; info.HasStats = true; break;
                case 29: info.Men = value; info.HasStats = true; break;
                case 30: info.PAtk = value; info.HasStats = true; break;
                case 31: info.MAtk = value; info.HasStats = true; break;
                case 32: info.PDef = value; info.HasStats = true; break;
                case 33: info.MDef = value; info.HasStats = true; break;
                case 34: info.PAtkSpd = value; info.HasStats = true; break;
                case 35: info.MAtkSpd = value; info.HasStats = true; break;
                case 36: info.Accuracy = value; info.HasStats = true; break;
                case 37: info.CritRate = value; info.HasStats = true; break;
            }
        }
    }

    static bool IsMsgPackInt(byte[] buf, int index)
    {
        if (buf == null || index < 0 || index >= buf.Length)
            return false;
        byte b = buf[index];
        if (b <= 0x7f || b >= 0xe0)
            return true;
        return b == 0xcc || b == 0xcd || b == 0xce || b == 0xcf
            || b == 0xd0 || b == 0xd1 || b == 0xd2 || b == 0xd3;
    }

    static ZoneV2PaperdollSlot[] ReadPaperdoll(byte[] body, ref int i)
    {
        int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (count <= 0)
            return Array.Empty<ZoneV2PaperdollSlot>();
        var slots = new ZoneV2PaperdollSlot[count];
        for (int s = 0; s < count; s++)
        {
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
                continue;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            int used = 0;
            if (n >= 1)
            {
                slots[s].ObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n >= 2)
            {
                slots[s].ItemId = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n >= 3 && ZoneV2MsgPack.IsInt(body, i))
            {
                slots[s].EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n > used)
                ZoneV2MsgPack.SkipValues(body, ref i, n - used);
        }

        return slots;
    }

    static bool TryReadItemEntry(byte[] body, ref int i, out ZoneV2ItemEntry entry)
    {
        entry = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 3)
        {
            ZoneV2MsgPack.SkipValues(body, ref i, n);
            return false;
        }

        entry = new ZoneV2ItemEntry
        {
            ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
            ItemId = ZoneV2MsgPack.ReadInt(body, ref i),
            Count = ZoneV2MsgPack.ReadInt(body, ref i),
            EquippedSlot = -1,
            BodySlot = "none",
            Kind = "etc",
            Icon = "",
            WeaponType = "",
            ArmorType = ""
        };
        int used = 3;
        if (n >= 4)
        {
            entry.EquippedSlot = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n >= 5)
        {
            entry.BodySlot = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 6)
        {
            entry.Kind = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 7)
        {
            entry.Icon = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 8)
        {
            entry.WeaponType = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 9)
        {
            entry.ArmorType = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 10)
        {
            if (ZoneV2MsgPack.IsInt(body, i))
                entry.EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i);
            else
                ZoneV2MsgPack.SkipValue(body, ref i);
            used++;
        }

        if (n > used)
            ZoneV2MsgPack.SkipValues(body, ref i, n - used);
        return true;
    }

    static bool TryReadInventoryChange(byte[] body, ref int i, out ZoneV2InventoryChange change)
    {
        change = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 4)
        {
            ZoneV2MsgPack.SkipValues(body, ref i, n);
            return false;
        }

        change = new ZoneV2InventoryChange
        {
            Change = ZoneV2MsgPack.ReadInt(body, ref i),
            ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
            ItemId = ZoneV2MsgPack.ReadInt(body, ref i),
            Count = ZoneV2MsgPack.ReadInt(body, ref i),
            EquippedSlot = -1,
            BodySlot = "none",
            Kind = "etc",
            Icon = "",
            WeaponType = "",
            ArmorType = ""
        };
        int used = 4;
        if (n >= 5)
        {
            change.EquippedSlot = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n >= 6)
        {
            change.BodySlot = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 7)
        {
            change.Kind = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 8)
        {
            change.Icon = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 9)
        {
            change.WeaponType = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 10)
        {
            change.ArmorType = ZoneV2MsgPack.ReadString(body, ref i);
            used++;
        }

        if (n >= 11)
        {
            if (ZoneV2MsgPack.IsInt(body, i))
                change.EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i);
            else
                ZoneV2MsgPack.SkipValue(body, ref i);
            used++;
        }

        if (n > used)
            ZoneV2MsgPack.SkipValues(body, ref i, n - used);
        return true;
    }

    public static bool TryDecodeEnchantResult(byte[] body, out ZoneV2EnchantResult result)
    {
        result = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            result = new ZoneV2EnchantResult
            {
                Result = ZoneV2MsgPack.ReadInt(body, ref i),
                EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i),
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            int used = 3;
            if (n >= 4)
            {
                result.CrystalItemId = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n >= 5)
            {
                result.CrystalCount = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n > used)
                ZoneV2MsgPack.SkipValues(body, ref i, n - used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeSkillList(byte[] body, out ZoneV2SkillEntry[] skills)
    {
        skills = Array.Empty<ZoneV2SkillEntry>();
        try
        {
            int i = 0;
            int outer = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (outer < 1)
                return false;
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
                return true;

            int rowCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (rowCount <= 0)
                return true;

            var rows = new ZoneV2SkillEntry[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                if (!TryReadSkillListEntry(body, ref i, out rows[r]))
                    return false;
            }

            skills = rows;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeMagicSkillUse(byte[] body, out ZoneV2MagicSkillUse use)
    {
        use = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 4)
                return false;
            use = new ZoneV2MagicSkillUse
            {
                CasterObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                TargetObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                SkillId = ZoneV2MsgPack.ReadInt(body, ref i),
                SkillLevel = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            int used = 4;
            if (n >= 5)
            {
                use.HitTimeMs = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n >= 6)
            {
                use.ReuseDelayMs = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n > used)
                ZoneV2MsgPack.SkipValues(body, ref i, n - used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    static bool TryReadSkillListEntry(byte[] body, ref int i, out ZoneV2SkillEntry row)
    {
        row = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 3)
            return false;
        row = new ZoneV2SkillEntry
        {
            SkillId = ZoneV2MsgPack.ReadInt(body, ref i),
            Level = ZoneV2MsgPack.ReadInt(body, ref i),
            ReuseDelayMs = ZoneV2MsgPack.ReadInt(body, ref i)
        };
        if (n > 3)
            ZoneV2MsgPack.SkipValues(body, ref i, n - 3);
        return true;
    }

    public static bool TryDecodeAbnormalStatusUpdate(byte[] body, out ZoneV2AbnormalStatusUpdate update)
    {
        update = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            update.ObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            int effectCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (effectCount <= 0)
            {
                update.Effects = Array.Empty<ZoneV2AbnormalEffect>();
                return true;
            }

            var effects = new ZoneV2AbnormalEffect[effectCount];
            for (int r = 0; r < effectCount; r++)
            {
                if (!TryReadAbnormalEffect(body, ref i, out effects[r]))
                    return false;
            }

            update.Effects = effects;
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeCreatureSay(byte[] body, out ZoneV2CreatureSay say)
    {
        say = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 4)
                return false;
            say = new ZoneV2CreatureSay
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
                Text = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
                ChatType = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n > 4)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 4);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeSystemMessage(byte[] body, out ZoneV2SystemMessage message)
    {
        message = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            int id = ZoneV2MsgPack.ReadInt(body, ref i);
            string[] parms = ZoneV2MsgPack.ReadStringArray(body, ref i);
            string text = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            if (n > 3)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 3);
            message = new ZoneV2SystemMessage
            {
                Id = id,
                Params = parms ?? Array.Empty<string>(),
                Text = text
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodeClanAskJoin(byte[] body, out ZoneV2ClanAskJoin ask)
    {
        ask = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 5)
                return false;
            ask = new ZoneV2ClanAskJoin
            {
                RequestorObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                RequestorName = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
                ClanId = ZoneV2MsgPack.ReadInt(body, ref i),
                ClanName = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
                PledgeType = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            if (n > 5)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 5);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodePledgeStatusChanged(byte[] body, out ZoneV2PledgeStatus status)
    {
        status = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 8)
                return false;
            status = new ZoneV2PledgeStatus
            {
                ClanId = ZoneV2MsgPack.ReadInt(body, ref i),
                Name = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
                Level = ZoneV2MsgPack.ReadInt(body, ref i),
                LeaderObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                CrestId = ZoneV2MsgPack.ReadInt(body, ref i),
                MemberCount = ZoneV2MsgPack.ReadInt(body, ref i),
                DissolvingExpiryUnixMs = ZoneV2MsgPack.ReadInt64(body, ref i),
                CharPenaltyExpiryUnixMs = ZoneV2MsgPack.ReadInt64(body, ref i)
            };
            if (n > 8)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 8);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryDecodePledgeMemberList(byte[] body, out ZoneV2PledgeMemberList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            list.ClanId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.LeaderObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            if (ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                list.Members = Array.Empty<ZoneV2PledgeMember>();
                return true;
            }

            int rowCount = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (rowCount <= 0)
            {
                list.Members = Array.Empty<ZoneV2PledgeMember>();
                if (n > 3)
                    ZoneV2MsgPack.SkipValues(body, ref i, n - 3);
                return true;
            }

            var rows = new ZoneV2PledgeMember[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                if (!TryReadPledgeMember(body, ref i, out rows[r]))
                    return false;
            }

            list.Members = rows;
            if (n > 3)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 3);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    static bool TryReadPledgeMember(byte[] body, ref int i, out ZoneV2PledgeMember member)
    {
        member = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 6)
            return false;
        member = new ZoneV2PledgeMember
        {
            ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
            Name = ZoneV2MsgPack.ReadString(body, ref i) ?? "",
            ClassId = ZoneV2MsgPack.ReadInt(body, ref i),
            Level = ZoneV2MsgPack.ReadInt(body, ref i),
            Online = ZoneV2MsgPack.ReadBool(body, ref i),
            PledgeType = ZoneV2MsgPack.ReadInt(body, ref i)
        };
        if (n > 6)
            ZoneV2MsgPack.SkipValues(body, ref i, n - 6);
        return true;
    }

    public static bool TryDecodeClanJoinResult(byte[] body, out int response)
    {
        response = 0;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            response = ZoneV2MsgPack.ReadInt(body, ref i);
            if (n > 1)
                ZoneV2MsgPack.SkipValues(body, ref i, n - 1);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    static bool TryReadAbnormalEffect(byte[] body, ref int i, out ZoneV2AbnormalEffect effect)
    {
        effect = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 3)
            return false;
        effect = new ZoneV2AbnormalEffect
        {
            Id = ZoneV2MsgPack.ReadInt(body, ref i),
            Value = ZoneV2MsgPack.ReadInt(body, ref i),
            DurationSeconds = ZoneV2MsgPack.ReadInt(body, ref i)
        };
        return true;
    }
}

public struct ZoneV2PlayerSpawned
{
    public int ObjectId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
}

public struct ZoneV2MoveTo
{
    public int ObjectId;
    public int DestX;
    public int DestY;
    public int DestZ;
    public int OriginX;
    public int OriginY;
    public int OriginZ;
}

public struct ZoneV2CharInfo
{
    public int ObjectId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public int ClassId;
    public bool HasPaperdoll;
    public int[] PaperdollItemIds;
    public bool HasClan;
    public int ClanId;
    public int ClanCrestId;
    /// <summary>Enchant of each paperdoll slot, parallel to <see cref="PaperdollItemIds"/>. Key 9.</summary>
    public int[] PaperdollEnchant;
}

/// <summary>
/// Paperdoll indexes from Fineage-Backend. Right hand is 7 (Squire's Sword 2369, Apprentice's Wand 6).
/// Missing indexes read as empty.
/// </summary>
public static class ZoneV2Paperdoll
{
    public const int Count = 17;
    public const int Under = 0;
    public const int LeftEar = 1;
    public const int RightEar = 2;
    public const int Neck = 3;
    public const int LeftFinger = 4;
    public const int RightFinger = 5;
    public const int Head = 6;
    public const int RightHand = 7;
    public const int LeftHand = 8;
    public const int Gloves = 9;
    public const int Chest = 10;
    public const int Legs = 11;
    public const int Feet = 12;
    public const int Cloak = 13;
    public const int Face = 14;
    public const int Hair = 15;
    public const int Hair2 = 16;

    public static int ItemIdAt(int[] itemIds, int slot)
    {
        if (itemIds == null || slot < 0 || slot >= itemIds.Length)
            return 0;
        return itemIds[slot];
    }

    public static int RightHandItemId(ZoneV2PaperdollSlot[] slots)
    {
        if (slots == null || slots.Length <= RightHand)
            return 0;
        return slots[RightHand].ItemId;
    }

    public static int[] ItemIds(ZoneV2PaperdollSlot[] slots)
    {
        if (slots == null || slots.Length == 0)
            return Array.Empty<int>();
        var ids = new int[slots.Length];
        for (int i = 0; i < slots.Length; i++)
            ids[i] = slots[i].ItemId;
        return ids;
    }

    /// <summary>Inventory paperdoll cell name (ItemSlot) to the zone index. Unknown cells are -1.</summary>
    public static int IndexForGearCell(string cell)
    {
        if (string.IsNullOrEmpty(cell))
            return -1;
        switch (cell)
        {
            case "underwear":
                return Under;
            case "lear":
                return LeftEar;
            case "rear":
                return RightEar;
            case "neck":
                return Neck;
            case "lfinger":
                return LeftFinger;
            case "rfinger":
                return RightFinger;
            case "head":
                return Head;
            case "rhand":
                return RightHand;
            case "lhand":
                return LeftHand;
            case "gloves":
                return Gloves;
            case "chest":
                return Chest;
            case "legs":
                return Legs;
            case "feet":
            case "boots":
                return Feet;
            case "hat1":
            case "hair":
                return Hair;
            case "hat2":
            case "hair2":
                return Hair2;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Two-handed weapons occupy the right hand. Full armor and formal wear occupy the chest.
    /// A ghost icon on the other cell still clears that primary slot.
    /// </summary>
    public static int UnequipIndex(int cellIndex, string bodySlot)
    {
        if (bodySlot == "lrhand")
            return RightHand;
        if (bodySlot == "fullarmor" || bodySlot == "alldress")
            return Chest;
        if (cellIndex < 0 || cellIndex >= Count)
            return -1;
        return cellIndex;
    }
}

public struct ZoneV2PaperdollSlot
{
    public int ObjectId;
    public int ItemId;
    public int EnchantLevel;
}

public struct ZoneV2UserInfo
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
    public bool HasPaperdoll;
    public ZoneV2PaperdollSlot[] Paperdoll;
    public bool HasClan;
    public int ClanId;
    public int ClanCrestId;
    public bool HasVitals;
    public int Level;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp;
    public bool HasStats;
    public int Str, Dex, Con, Int, Wit, Men;
    public int PAtk, MAtk, PDef, MDef, PAtkSpd, MAtkSpd, Accuracy, CritRate;
}

public struct ZoneV2NpcInfo
{
    public int ObjectId;
    public int NpcTemplateId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    /// <summary>Legacy five-int rows stay attackable. Backend key 7 is the real flag.</summary>
    public bool Attackable;
    /// <summary>Backend key 8; 0 when the wire omitted speeds.</summary>
    public float RunSpeed;
    /// <summary>Backend key 9; 0 when the wire omitted speeds.</summary>
    public float WalkSpeed;
    public string Title;
    public string NpcType;
}

public struct ZoneV2MyTargetSelected
{
    public int ObjectId;
    public int ColorHint;
}

public struct ZoneV2Attack
{
    public int AttackerObjectId;
    public int TargetObjectId;
    public int Damage;
    public byte Flags;
    public int AttackerX;
    public int AttackerY;
    public int AttackerZ;
    public int TargetX;
    public int TargetY;
    public int TargetZ;
}

public struct ZoneV2StatusUpdate
{
    public int ObjectId;
    public int[] AttributeIds;
    public int[] Values;
}

public struct ZoneV2ItemEntry
{
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int EquippedSlot;
    public string BodySlot;
    public string Kind;
    public string Icon;
    public string WeaponType;
    public string ArmorType;
    public int EnchantLevel;
}

public struct ZoneV2SpawnItem
{
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

public struct ZoneV2DropItem
{
    public int CharObjectId;
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

/// <summary>Wire spawn row after decode — same field set as Interlude <c>SpawnItemDto</c> apply.</summary>
public struct SpawnItemApplyFields
{
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

public struct ZoneV2InventoryChange
{
    public int Change;
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int EquippedSlot;
    public string BodySlot;
    public string Kind;
    public string Icon;
    public string WeaponType;
    public string ArmorType;
    public int EnchantLevel;
}

public struct ZoneV2EnchantResult
{
    public int Result;
    public int EnchantLevel;
    public int ObjectId;
    public int CrystalItemId;
    public int CrystalCount;
}

public struct ZoneV2SkillEntry
{
    public int SkillId;
    public int Level;
    public int ReuseDelayMs;
}

public struct ZoneV2MagicSkillUse
{
    public int CasterObjectId;
    public int TargetObjectId;
    public int SkillId;
    public int SkillLevel;
    public int HitTimeMs;
    public int ReuseDelayMs;
}

public struct ZoneV2Die
{
    public int ObjectId;
    public int KillerObjectId;
}

public struct ZoneV2Revive
{
    public int ObjectId;
}

public struct ZoneV2TeleportToLocation
{
    public int ObjectId;
    public int X;
    public int Y;
    public int Z;
    public int Heading;
}

public struct ZoneV2AbnormalStatusUpdate
{
    public int ObjectId;
    public ZoneV2AbnormalEffect[] Effects;
}

public struct ZoneV2AbnormalEffect
{
    public int Id;
    public int Value;
    public int DurationSeconds;
}

public struct ZoneV2PartyAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ItemDistribution;
}

public struct ZoneV2PartyMemberEntry
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public int CurHp;
    public int MaxHp;
    public int CurMp;
    public int MaxMp;
}

public struct ZoneV2PartyMemberList
{
    public int LeaderObjectId;
    public int PartyId;
    public ZoneV2PartyMemberEntry[] Members;
}

public struct ZoneV2TradeAsk
{
    public int RequestorObjectId;
    public string RequestorName;
}

public struct ZoneV2TradeStart
{
    public int PartnerObjectId;
    public string PartnerName;
}

public struct ZoneV2TradeOfferUpdate
{
    public int IsOwn;
    public int InventoryObjectId;
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct ZoneV2TradeConfirmState
{
    public int SelfConfirmed;
    public int PartnerConfirmed;
}

public struct ZoneV2ClanAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ClanId;
    public string ClanName;
    public int PledgeType;
}

public struct ZoneV2PledgeStatus
{
    public int ClanId;
    public string Name;
    public int Level;
    public int LeaderObjectId;
    public int CrestId;
    public int MemberCount;
    public long DissolvingExpiryUnixMs;
    public long CharPenaltyExpiryUnixMs;
}

public struct ZoneV2PledgeMember
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public bool Online;
    public int PledgeType;
}

public struct ZoneV2PledgeMemberList
{
    public int ClanId;
    public int LeaderObjectId;
    public ZoneV2PledgeMember[] Members;
}

/// <summary>
/// Login roster row (<c>CharacterSummaryMsg</c>). Keys 0–14 identity/appearance/position;
/// keys 15–24 CharSelect vitals (cur/max HP MP CP, exp, sp, karma, weaponItemId);
/// key 25 paperdoll int[17]. Array length 26.
/// </summary>
public struct ZoneV2CharacterSummary
{
    public int CharacterId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int Level;
    public int Slot;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
    public int DeleteSecondsLeft;
    public bool IsLastPlayed;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp, Karma, WeaponItemId;
    /// <summary>Equipped item ids by paperdoll slot (length 17). Null when the login did not send key 25.</summary>
    public int[] Paperdoll;
}

/// <summary>Character select ticket. <see cref="Token"/> is the zone Auth password.</summary>
public struct ZoneV2Handoff
{
    public string Account;
    public string CharacterName;
    public int CharacterId;
    public int ClassId;
    public string Token;
    public string ZoneHost;
    public int ZonePort;
    public long ExpiresUnixSeconds;
}

public struct ZoneV2CharacterCreated
{
    public int CharacterId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
}

public struct ZoneV2CharacterDeleted
{
    public int Slot;
    public int CharacterId;
    public bool HardDeleted;
}

public struct ZoneV2CharacterDeleteFailed
{
    public int Slot;
    public int CharacterId;
    public string Code;
    public string Message;
}

/// <summary>Opcode 1012. <see cref="ChatType"/> is the type the speaker sent (0 all, 1 shout).</summary>
public struct ZoneV2CreatureSay
{
    public int ObjectId;
    public string Name;
    public string Text;
    public int ChatType;
}

/// <summary>Opcode 1040. <see cref="Text"/> is the line to show. Id 0 means show it as written.</summary>
public struct ZoneV2SystemMessage
{
    public int Id;
    public string[] Params;
    public string Text;
}
