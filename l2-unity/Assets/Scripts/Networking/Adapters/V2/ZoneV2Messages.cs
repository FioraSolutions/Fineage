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
