using System;
using System.IO;

/// <summary>MessagePack array bodies matching tools/zone-server Wire/Messages.cs.</summary>
public static class ZoneV2Messages
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
            // Key 6 is optional. Older zone servers stop at class id.
            if (n >= 7 && !ZoneV2MsgPack.TryConsumeNil(body, ref i))
            {
                info.PaperdollItemIds = ZoneV2MsgPack.ReadIntArray(body, ref i);
                info.HasPaperdoll = true;
            }

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
            // Fineage-Backend key 2 is the npc name. Legacy zone-server key 2 is x.
            if (n >= 6 && ZoneV2MsgPack.IsStringOrNil(body, i))
            {
                ZoneV2MsgPack.ReadString(body, ref i);
                x = ZoneV2MsgPack.ReadInt(body, ref i);
                y = ZoneV2MsgPack.ReadInt(body, ref i);
                z = ZoneV2MsgPack.ReadInt(body, ref i);
            }
            else
            {
                x = ZoneV2MsgPack.ReadInt(body, ref i);
                y = ZoneV2MsgPack.ReadInt(body, ref i);
                z = ZoneV2MsgPack.ReadInt(body, ref i);
            }

            info = new ZoneV2NpcInfo
            {
                ObjectId = objectId,
                NpcTemplateId = templateId,
                X = x,
                Y = y,
                Z = z
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
        if (n > 15)
            ZoneV2MsgPack.SkipValues(body, ref i, n - 15);
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

            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
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

        if (n > used)
            ZoneV2MsgPack.SkipValues(body, ref i, n - used);
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
}

public struct ZoneV2PaperdollSlot
{
    public int ObjectId;
    public int ItemId;
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
}

public struct ZoneV2NpcInfo
{
    public int ObjectId;
    public int NpcTemplateId;
    public int X;
    public int Y;
    public int Z;
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

/// <summary>
/// Login roster row. Keys 0–5 are id, name, class, race, sex, level.
/// Keys 6–14 are slot, appearance, position, delete timer, and last-played.
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
