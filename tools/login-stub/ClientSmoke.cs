using System.Buffers.Binary;
using System.Net.Sockets;
using Fineage.LoginStub.Crypto;

namespace Fineage.LoginStub;

static class ClientSmoke
{
    public static async Task RunAsync()
    {
        Console.WriteLine("client smoke → login+game");
        using var login = new TcpClient();
        await login.ConnectAsync("127.0.0.1", 2106);
        await using var ls = login.GetStream();

        byte[]? initEnc = await Framed.ReadBodyAsync(ls);
        if (initEnc == null) throw new Exception("no Init");
        byte[] init = LoginCrypt.DecryptLogin(initEnc, LoginCrypt.StaticBlowfishKey, init: true);
        if (init[0] != 0x00) throw new Exception("bad Init opcode");
        int sessionId = BinaryPrimitives.ReadInt32LittleEndian(init.AsSpan(1));
        byte[] bfKey = init.AsSpan(1 + 4 + 4 + 128 + 16, 16).ToArray();
        Console.WriteLine($"Init OK session={sessionId:X8} bf={Convert.ToHexString(bfKey)}");

        // AuthGameGuard — minimal opcode+session (pad to 8, blowfish). Skip fancy checksum.
        byte[] ggReq = PadBlowfish(new PacketBuilder().C(0x07).D(sessionId).D(0).D(0).D(0).D(0).ToArray());
        await Framed.SendAsync(ls, LoginCrypt.BlowfishProcess(bfKey, true, ggReq));

        byte[]? ggAuthEnc = await Framed.ReadBodyAsync(ls);
        if (ggAuthEnc == null) throw new Exception("no GGAuth");
        byte[] ggAuth = LoginCrypt.DecryptLogin(ggAuthEnc, bfKey, init: false);
        if (ggAuth[0] != 0x0B) throw new Exception($"expected GGAuth got 0x{ggAuth[0]:X2}");
        Console.WriteLine("GGAuth OK");

        byte[] auth = PadBlowfish(new PacketBuilder().C(0x00).B(new byte[128]).ToArray());
        await Framed.SendAsync(ls, LoginCrypt.BlowfishProcess(bfKey, true, auth));

        byte[]? loginOkEnc = await Framed.ReadBodyAsync(ls);
        if (loginOkEnc == null) throw new Exception("no LoginOk");
        byte[] loginOk = LoginCrypt.DecryptLogin(loginOkEnc, bfKey, init: false);
        if (loginOk[0] != 0x03) throw new Exception("expected LoginOk");
        int sk1 = BinaryPrimitives.ReadInt32LittleEndian(loginOk.AsSpan(1));
        int sk2 = BinaryPrimitives.ReadInt32LittleEndian(loginOk.AsSpan(5));
        Console.WriteLine($"LoginOk OK keys={sk1:X8}/{sk2:X8}");

        byte[] reqList = PadBlowfish(new PacketBuilder().C(0x05).D(sk1).D(sk2).ToArray());
        await Framed.SendAsync(ls, LoginCrypt.BlowfishProcess(bfKey, true, reqList));

        byte[]? listEnc = await Framed.ReadBodyAsync(ls);
        if (listEnc == null) throw new Exception("no ServerList");
        byte[] list = LoginCrypt.DecryptLogin(listEnc, bfKey, init: false);
        if (list[0] != 0x04) throw new Exception("expected ServerList");
        Console.WriteLine("ServerList OK");

        byte[] reqPlay = PadBlowfish(new PacketBuilder().C(0x02).D(sk1).D(sk2).C(1).ToArray());
        await Framed.SendAsync(ls, LoginCrypt.BlowfishProcess(bfKey, true, reqPlay));

        byte[]? playEnc = await Framed.ReadBodyAsync(ls);
        if (playEnc == null) throw new Exception("no PlayOk");
        byte[] play = LoginCrypt.DecryptLogin(playEnc, bfKey, init: false);
        if (play[0] != 0x07) throw new Exception("expected PlayOk");
        Console.WriteLine("PlayOk OK");
        login.Close();

        using var game = new TcpClient();
        await game.ConnectAsync("127.0.0.1", 7777);
        await using var gs = game.GetStream();
        byte[] proto = PadBlowfish(new PacketBuilder().C(0x00).D(746).ToArray());
        await Framed.SendAsync(gs, proto);

        byte[]? keyPkt = await Framed.ReadBodyAsync(gs);
        if (keyPkt == null || keyPkt[0] != 0x00) throw new Exception("no InterludeKey");
        Console.WriteLine("InterludeKey OK");

        byte[] authLogin = PadBlowfish(new PacketBuilder().C(0x08).C(4).B("test"u8.ToArray()).C(0).D(1).D(2).D(sk1).D(sk2).ToArray());
        await Framed.SendAsync(gs, authLogin);

        byte[]? chars = await Framed.ReadBodyAsync(gs);
        if (chars == null || chars[0] != 0x13) throw new Exception("no CharSelectionInfo");
        int charCount = chars.Length >= 5 ? BinaryPrimitives.ReadInt32LittleEndian(chars.AsSpan(1)) : 0;
        Console.WriteLine($"CharSelectionInfo OK len={chars.Length} count={charCount}");

        if (charCount == 0)
        {
            byte[] newChar = PadBlowfish(new PacketBuilder().C(0x0E).ToArray());
            await Framed.SendAsync(gs, newChar);
            byte[]? templates = await Framed.ReadBodyAsync(gs);
            if (templates == null || templates[0] != 0x17) throw new Exception("no CharTemplate");
            Console.WriteLine("CharTemplate OK");

            byte[] create = PadBlowfish(BuildCharacterCreatePacket("Test"));
            await Framed.SendAsync(gs, create);
            byte[]? createOk = await Framed.ReadBodyAsync(gs);
            if (createOk == null || createOk[0] != 0x19) throw new Exception("no CharCreateOk");
            chars = await Framed.ReadBodyAsync(gs);
            if (chars == null || chars[0] != 0x13) throw new Exception("no CharSelectionInfo after create");
            charCount = BinaryPrimitives.ReadInt32LittleEndian(chars.AsSpan(1));
            if (charCount < 1) throw new Exception("CharSelectionInfo empty after create");
            Console.WriteLine($"CharacterCreate OK count={charCount}");
        }

        // CharacterSelect slot 0
        byte[] sel = PadBlowfish(new PacketBuilder().C(0x0D).D(0).H(0).D(0).D(0).D(0).ToArray());
        await Framed.SendAsync(gs, sel);
        byte[]? selected = await Framed.ReadBodyAsync(gs);
        if (selected == null || selected[0] != 0x15) throw new Exception("no CharSelected");
        Console.WriteLine($"CharSelected OK len={selected.Length}");

        byte[] enter = PadBlowfish(new PacketBuilder().C(0x03).B(new byte[32]).D(1).D(1).D(1).D(1).B(new byte[32]).D(1).ToArray());
        await Framed.SendAsync(gs, enter);
        byte[]? cool = await Framed.ReadBodyAsync(gs);
        if (cool == null || cool[0] != 0xC1) throw new Exception("no SkillCoolTime");
        Console.WriteLine("EnterWorld/SkillCoolTime OK");

        // Move echo
        byte[] move = PadBlowfish(new PacketBuilder()
            .C(0x01).D(-84300).D(244600).D(-3730).D(-84318).D(244578).D(-3730).D(1).ToArray());
        await Framed.SendAsync(gs, move);
        byte[]? moveAck = await Framed.ReadBodyAsync(gs);
        if (moveAck == null || moveAck[0] != 0x01) throw new Exception("no CharMoveToLocation");
        Console.WriteLine("CharMoveToLocation OK");
        Console.WriteLine("client smoke PASSED");
    }

    static byte[] BuildCharacterCreatePacket(string name)
    {
        // Mirrors CharacterCreateDto + Human Fighter template stats.
        byte[] nameUtf16 = System.Text.Encoding.Unicode.GetBytes(name);
        var p = new PacketBuilder()
            .C(0x0B)
            .C((byte)nameUtf16.Length)
            .B(nameUtf16)
            .C(0)
            .D(0)   // race human
            .D(0)   // male
            .D(0)   // fighter
            .D(21).D(40).D(43).D(25).D(30).D(11)
            .D(0).D(0).D(0);
        return p.ToArray();
    }

    static byte[] PadBlowfish(byte[] data)
    {
        int rem = data.Length % 8;
        if (rem == 0) return data;
        byte[] p = new byte[data.Length + (8 - rem)];
        Buffer.BlockCopy(data, 0, p, 0, data.Length);
        return p;
    }
}
