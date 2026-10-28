using System;
using System.IO;
using System.Text;

/// <summary>
/// Minimal MessagePack codec for ZoneServer v2 array-shaped objects
/// (MessagePack-CSharp [MessagePackObject] + [Key(n)] default).
/// </summary>
public static class ZoneV2MsgPack
{
    public static byte[] WriteArray(Action<BinaryWriter> writeElements, int count)
    {
        using (MemoryStream ms = new MemoryStream(64))
        using (BinaryWriter w = new BinaryWriter(ms))
        {
            WriteArrayHeader(w, count);
            writeElements(w);
            return ms.ToArray();
        }
    }

    public static void WriteArrayHeader(BinaryWriter w, int count)
    {
        if (count < 0 || count > 15)
            throw new ArgumentOutOfRangeException(nameof(count), "fixarray only (0..15)");
        w.Write((byte)(0x90 | count));
    }

    public static void WriteInt(BinaryWriter w, int value)
    {
        if (value >= 0 && value <= 0x7f)
        {
            w.Write((byte)value);
            return;
        }

        if (value >= -32 && value < 0)
        {
            w.Write((byte)(0xe0 | (value + 32)));
            return;
        }

        if (value >= short.MinValue && value <= short.MaxValue)
        {
            w.Write((byte)0xd1);
            WriteBe16(w, (ushort)(short)value);
            return;
        }

        w.Write((byte)0xd2);
        WriteBe32(w, (uint)value);
    }

    public static void WriteUInt(BinaryWriter w, uint value)
    {
        if (value <= 0x7f)
        {
            w.Write((byte)value);
            return;
        }

        if (value <= byte.MaxValue)
        {
            w.Write((byte)0xcc);
            w.Write((byte)value);
            return;
        }

        if (value <= ushort.MaxValue)
        {
            w.Write((byte)0xcd);
            WriteBe16(w, (ushort)value);
            return;
        }

        w.Write((byte)0xce);
        WriteBe32(w, value);
    }

    public static void WriteString(BinaryWriter w, string value)
    {
        if (value == null)
            value = "";
        byte[] utf8 = Encoding.UTF8.GetBytes(value);
        if (utf8.Length <= 31)
        {
            w.Write((byte)(0xa0 | utf8.Length));
            w.Write(utf8);
            return;
        }

        if (utf8.Length <= byte.MaxValue)
        {
            w.Write((byte)0xd9);
            w.Write((byte)utf8.Length);
            w.Write(utf8);
            return;
        }

        w.Write((byte)0xda);
        WriteBe16(w, (ushort)utf8.Length);
        w.Write(utf8);
    }

    public static void WriteFloat(BinaryWriter w, float value)
    {
        w.Write((byte)0xca);
        byte[] bits = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bits);
        w.Write(bits);
    }

    public static void WriteBool(BinaryWriter w, bool value)
    {
        w.Write(value ? (byte)0xc3 : (byte)0xc2);
    }

    public static int ReadArrayHeader(byte[] buf, ref int i)
    {
        byte b = buf[i++];
        if ((b & 0xf0) == 0x90)
            return b & 0x0f;
        if (b == 0xdc)
        {
            int n = ReadBe16(buf, ref i);
            return n;
        }

        throw new InvalidDataException("expected array header, got 0x" + b.ToString("X2"));
    }

    public static int ReadInt(byte[] buf, ref int i)
    {
        byte b = buf[i++];
        if (b <= 0x7f)
            return b;
        if (b >= 0xe0)
            return (b & 0x1f) - 32;
        if (b == 0xd0)
            return (sbyte)buf[i++];
        if (b == 0xd1)
            return (short)ReadBe16(buf, ref i);
        if (b == 0xd2)
            return (int)ReadBe32(buf, ref i);
        if (b == 0xcc)
            return buf[i++];
        if (b == 0xcd)
            return ReadBe16(buf, ref i);
        if (b == 0xce)
        {
            uint u = ReadBe32(buf, ref i);
            if (u > int.MaxValue)
                throw new InvalidDataException("uint too large for int");
            return (int)u;
        }

        throw new InvalidDataException("expected int, got 0x" + b.ToString("X2"));
    }

    public static string ReadString(byte[] buf, ref int i)
    {
        byte b = buf[i++];
        int len;
        if ((b & 0xe0) == 0xa0)
            len = b & 0x1f;
        else if (b == 0xd9)
            len = buf[i++];
        else if (b == 0xda)
            len = ReadBe16(buf, ref i);
        else if (b == 0xc0)
            return "";
        else
            throw new InvalidDataException("expected string, got 0x" + b.ToString("X2"));

        string s = Encoding.UTF8.GetString(buf, i, len);
        i += len;
        return s;
    }

    public static float ReadFloat(byte[] buf, ref int i)
    {
        byte b = buf[i++];
        if (b != 0xca)
            throw new InvalidDataException("expected float32, got 0x" + b.ToString("X2"));
        byte[] bits = new byte[4];
        Buffer.BlockCopy(buf, i, bits, 0, 4);
        i += 4;
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bits);
        return BitConverter.ToSingle(bits, 0);
    }

    public static bool ReadBool(byte[] buf, ref int i)
    {
        byte b = buf[i++];
        if (b == 0xc3)
            return true;
        if (b == 0xc2)
            return false;
        throw new InvalidDataException("expected bool, got 0x" + b.ToString("X2"));
    }

    public static int[] ReadIntArray(byte[] buf, ref int i)
    {
        int n = ReadArrayHeader(buf, ref i);
        if (n <= 0)
            return Array.Empty<int>();
        var values = new int[n];
        for (int j = 0; j < n; j++)
            values[j] = ReadInt(buf, ref i);
        return values;
    }

    public static void WriteIntArray(BinaryWriter w, int[] values)
    {
        if (values == null)
            values = Array.Empty<int>();
        WriteArrayHeader(w, values.Length);
        for (int j = 0; j < values.Length; j++)
            WriteInt(w, values[j]);
    }

    static void WriteBe16(BinaryWriter w, ushort v)
    {
        w.Write((byte)(v >> 8));
        w.Write((byte)(v & 0xff));
    }

    static void WriteBe32(BinaryWriter w, uint v)
    {
        w.Write((byte)(v >> 24));
        w.Write((byte)((v >> 16) & 0xff));
        w.Write((byte)((v >> 8) & 0xff));
        w.Write((byte)(v & 0xff));
    }

    static int ReadBe16(byte[] buf, ref int i)
    {
        int v = (buf[i] << 8) | buf[i + 1];
        i += 2;
        return v;
    }

    static uint ReadBe32(byte[] buf, ref int i)
    {
        uint v = ((uint)buf[i] << 24) | ((uint)buf[i + 1] << 16) | ((uint)buf[i + 2] << 8) | buf[i + 3];
        i += 4;
        return v;
    }
}
