using System.Buffers.Binary;
using System.Net.Sockets;
using Fineage.LoginStub.Crypto;

namespace Fineage.LoginStub;

sealed class PacketBuilder
{
    readonly List<byte> _buf = new();

    public PacketBuilder C(byte v) { _buf.Add(v); return this; }
    public PacketBuilder B(ReadOnlySpan<byte> v) { _buf.AddRange(v.ToArray()); return this; }

    public PacketBuilder D(int v)
    {
        Span<byte> tmp = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(tmp, v);
        _buf.AddRange(tmp.ToArray());
        return this;
    }

    public PacketBuilder Q(long v)
    {
        Span<byte> tmp = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(tmp, v);
        _buf.AddRange(tmp.ToArray());
        return this;
    }

    public PacketBuilder H(short v)
    {
        Span<byte> tmp = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(tmp, v);
        _buf.AddRange(tmp.ToArray());
        return this;
    }

    public PacketBuilder F(double v)
    {
        _buf.AddRange(BitConverter.GetBytes(v));
        return this;
    }

    /// <summary>UTF-16LE null-terminated (client ReadOtherS).</summary>
    public PacketBuilder S(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            foreach (char ch in text)
            {
                _buf.Add((byte)(ch & 0xff));
                _buf.Add((byte)(ch >> 8));
            }
        }

        _buf.Add(0);
        _buf.Add(0);
        return this;
    }

    public byte[] ToArray() => _buf.ToArray();
}

static class Framed
{
    public static async Task SendAsync(NetworkStream stream, byte[] body, CancellationToken ct = default)
    {
        int total = body.Length + 2;
        byte[] packet = new byte[total];
        packet[0] = (byte)(total & 0xff);
        packet[1] = (byte)((total >> 8) & 0xff);
        Buffer.BlockCopy(body, 0, packet, 2, body.Length);
        await stream.WriteAsync(packet, ct);
        await stream.FlushAsync(ct);
    }

    public static async Task<byte[]?> ReadBodyAsync(NetworkStream stream, CancellationToken ct = default)
    {
        byte[] header = new byte[2];
        if (!await ReadExactAsync(stream, header, ct))
            return null;
        int total = header[0] | (header[1] << 8);
        if (total <= 2)
            return null;
        byte[] body = new byte[total - 2];
        if (!await ReadExactAsync(stream, body, ct))
            return null;
        return body;
    }

    static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buf, CancellationToken ct)
    {
        int off = 0;
        while (off < buf.Length)
        {
            int n = await stream.ReadAsync(buf.AsMemory(off, buf.Length - off), ct);
            if (n <= 0)
                return false;
            off += n;
        }

        return true;
    }
}

static class SelfTest
{
    public static void Run()
    {
        Console.WriteLine("selftest: Init XOR+BF roundtrip");
        var body = new PacketBuilder()
            .C(0x00)
            .D(0x11223344)
            .D(0x0000c621)
            .B(new byte[128])
            .D(1).D(2).D(3).D(4)
            .B(Enumerable.Repeat((byte)0xAB, 16).ToArray())
            .C(0x00)
            .D(0)
            .H(0)
            .ToArray();

        int xorKey = unchecked((int)0xA1B2C3D4);
        byte[] enc = LoginCrypt.EncryptInit(body, xorKey);
        byte[] dec = LoginCrypt.DecryptLogin(enc, LoginCrypt.StaticBlowfishKey, init: true);
        if (dec[0] != 0x00)
            throw new Exception("opcode lost");
        int session = BinaryPrimitives.ReadInt32LittleEndian(dec.AsSpan(1));
        if (session != 0x11223344)
            throw new Exception($"session mismatch {session:X}");

        Console.WriteLine("selftest: login checksum roundtrip");
        var ok = new PacketBuilder().C(0x03).D(10).D(20).ToArray();
        byte[] key = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();
        byte[] enc2 = LoginCrypt.EncryptLogin(ok, key);
        byte[] dec2 = LoginCrypt.DecryptLogin(enc2, key, init: false);
        if (dec2[0] != 0x03)
            throw new Exception("LoginOk opcode lost");


        Console.WriteLine("selftest OK");
    }
}
