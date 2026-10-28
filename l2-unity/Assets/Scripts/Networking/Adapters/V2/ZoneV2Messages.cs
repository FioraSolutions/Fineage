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
            info = new ZoneV2NpcInfo
            {
                ObjectId = ZoneV2MsgPack.ReadInt(body, ref i),
                NpcTemplateId = ZoneV2MsgPack.ReadInt(body, ref i),
                X = ZoneV2MsgPack.ReadInt(body, ref i),
                Y = ZoneV2MsgPack.ReadInt(body, ref i),
                Z = ZoneV2MsgPack.ReadInt(body, ref i)
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
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
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
                Name = ZoneV2MsgPack.ReadString(body, ref i),
                Text = ZoneV2MsgPack.ReadString(body, ref i),
                ChatType = ZoneV2MsgPack.ReadInt(body, ref i)
            };
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
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

public struct ZoneV2CreatureSay
{
    public int ObjectId;
    public string Name;
    public string Text;
    public int ChatType;
}
