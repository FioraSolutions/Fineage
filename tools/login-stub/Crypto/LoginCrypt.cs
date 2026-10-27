using System.Security.Cryptography;

namespace Fineage.LoginStub.Crypto;

static class LoginCrypt
{
    public static readonly byte[] StaticBlowfishKey =
    {
        0x6b, 0x60, 0xcb, 0x5b, 0x82, 0xce, 0x90, 0xb1,
        0xcc, 0x2b, 0x6c, 0x55, 0x6c, 0x6c, 0x6c, 0x6c
    };

    /// <summary>
    /// Inverse of client <c>NewCrypt.decXORPass</c>: store the <b>final</b> ecx
    /// (not the initial key). Matches the Unity client decode path.
    /// </summary>
    public static void EncXorPass(byte[] raw, int key)
    {
        int stop = raw.Length - 8;
        int pos = 4;
        int ecx = key;
        while (pos < stop)
        {
            int edx = raw[pos] & 0xff;
            edx |= (raw[pos + 1] & 0xff) << 8;
            edx |= (raw[pos + 2] & 0xff) << 16;
            edx |= (raw[pos + 3] & 0xff) << 24;

            ecx += edx;
            edx ^= ecx;

            raw[pos++] = (byte)(edx & 0xff);
            raw[pos++] = (byte)((edx >> 8) & 0xff);
            raw[pos++] = (byte)((edx >> 16) & 0xff);
            raw[pos++] = (byte)((edx >> 24) & 0xff);
        }

        // Client decrypt starts ecx from these 4 bytes — must be final ecx.
        raw[pos++] = (byte)(ecx & 0xff);
        raw[pos++] = (byte)((ecx >> 8) & 0xff);
        raw[pos++] = (byte)((ecx >> 16) & 0xff);
        raw[pos++] = (byte)((ecx >> 24) & 0xff);
    }

    public static bool DecXorPass(byte[] packet)
    {
        int blen = packet.Length;
        if (blen < 12)
            return false;

        uint xorKey = packet[blen - 8];
        xorKey |= (uint)(packet[blen - 7] << 8);
        xorKey |= (uint)(packet[blen - 6] << 16);
        xorKey |= (uint)(packet[blen - 5] << 24);

        int offset = blen - 12;
        uint ecx = xorKey;
        while (offset > 2)
        {
            uint edx = packet[offset];
            edx |= (uint)(packet[offset + 1] << 8);
            edx |= (uint)(packet[offset + 2] << 16);
            edx |= (uint)(packet[offset + 3] << 24);

            edx ^= ecx;
            ecx -= edx;

            packet[offset] = (byte)(edx & 0xff);
            packet[offset + 1] = (byte)((edx >> 8) & 0xff);
            packet[offset + 2] = (byte)((edx >> 16) & 0xff);
            packet[offset + 3] = (byte)((edx >> 24) & 0xff);
            offset -= 4;
        }

        return true;
    }

    public static void AppendChecksum(byte[] raw)
    {
        ulong chksum = 0;
        int count = raw.Length - 4;
        int i;
        for (i = 0; i < count; i += 4)
        {
            ulong ecx = raw[i];
            ecx |= (ulong)raw[i + 1] << 8;
            ecx |= (ulong)raw[i + 2] << 16;
            ecx |= (ulong)raw[i + 3] << 24;
            chksum ^= ecx;
        }

        raw[i] = (byte)(chksum & 0xff);
        raw[i + 1] = (byte)((chksum >> 8) & 0xff);
        raw[i + 2] = (byte)((chksum >> 16) & 0xff);
        raw[i + 3] = (byte)((chksum >> 24) & 0xff);
    }

    public static bool VerifyChecksum(byte[] raw)
    {
        if ((raw.Length & 3) != 0 || raw.Length <= 4)
            return false;

        ulong chksum = 0;
        int count = raw.Length - 4;
        int i;
        for (i = 0; i < count; i += 4)
        {
            ulong check = raw[i];
            check |= (ulong)raw[i + 1] << 8;
            check |= (ulong)raw[i + 2] << 16;
            check |= (ulong)raw[i + 3] << 24;
            chksum ^= check;
        }

        ulong stored = raw[i];
        stored |= (ulong)raw[i + 1] << 8;
        stored |= (ulong)raw[i + 2] << 16;
        stored |= (ulong)raw[i + 3] << 24;
        return stored == chksum;
    }

    public static byte[] Pad8(byte[] data)
    {
        int rem = data.Length % 8;
        if (rem == 0)
            return data;
        byte[] padded = new byte[data.Length + (8 - rem)];
        Buffer.BlockCopy(data, 0, padded, 0, data.Length);
        return padded;
    }

    public static byte[] BlowfishProcess(byte[] key, bool encrypt, byte[] data)
    {
        var engine = new BlowfishEngine();
        engine.init(encrypt, key);
        byte[] output = new byte[data.Length];
        engine.processBigBlock(data, 0, output, 0, data.Length);
        return output;
    }

    public static byte[] EncryptInit(byte[] body, int xorKey)
    {
        // Trailing 8 bytes are XOR-key storage and must not overlap payload
        // (Blowfish key sits at the end of Init content).
        int size = body.Length + 8;
        int align = size % 8;
        if (align != 0)
            size += 8 - align;
        byte[] work = new byte[size];
        Buffer.BlockCopy(body, 0, work, 0, body.Length);
        EncXorPass(work, xorKey);
        return BlowfishProcess(StaticBlowfishKey, true, work);
    }

    public static byte[] EncryptLogin(byte[] body, byte[] blowfishKey)
    {
        // Reserve 4 bytes for checksum, then pad to 8.
        byte[] withChecksumSlot = new byte[body.Length + 4];
        Buffer.BlockCopy(body, 0, withChecksumSlot, 0, body.Length);
        byte[] work = Pad8(withChecksumSlot);
        AppendChecksum(work);
        return BlowfishProcess(blowfishKey, true, work);
    }

    public static byte[] DecryptLogin(byte[] data, byte[] blowfishKey, bool init)
    {
        byte[] work = BlowfishProcess(blowfishKey, false, data);
        if (init)
        {
            DecXorPass(work);
            return work;
        }

        if (!VerifyChecksum(work))
            throw new InvalidDataException("login packet checksum mismatch");
        return work;
    }

    public static byte[] ScrambleModulus(byte[] modulus)
    {
        byte[] key = (byte[])modulus.Clone();
        for (int i = 0; i < 4; i++)
        {
            byte temp = key[0x00 + i];
            key[0x00 + i] = key[0x4d + i];
            key[0x4d + i] = temp;
        }

        for (int i = 0; i < 0x40; i++)
            key[i] = (byte)(key[i] ^ key[0x40 + i]);

        for (int i = 0; i < 4; i++)
            key[0x0d + i] = (byte)(key[0x0d + i] ^ key[0x34 + i]);

        for (int i = 0; i < 0x40; i++)
            key[0x40 + i] = (byte)(key[0x40 + i] ^ key[i]);

        return key;
    }

    public static byte[] GenerateRsaModulusScrambled(out RSA rsa)
    {
        rsa = RSA.Create(1024);
        RSAParameters p = rsa.ExportParameters(false);
        byte[] mod = p.Modulus ?? throw new InvalidOperationException("no modulus");
        if (mod.Length != 128)
        {
            // Normalize to 128 bytes.
            byte[] fixed128 = new byte[128];
            Buffer.BlockCopy(mod, Math.Max(0, mod.Length - 128), fixed128, Math.Max(0, 128 - mod.Length), Math.Min(128, mod.Length));
            mod = fixed128;
        }

        return ScrambleModulus(mod);
    }

    public static byte[] Concat(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, r, 0, a.Length);
        Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
        return r;
    }
}
