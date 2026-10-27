using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Fineage.LoginStub;
using Fineage.LoginStub.Crypto;

// Fineage login+game stub: Init/Blowfish auth → ServerList → PlayOk → CharSelectionInfo.
const int LoginPort = 2106;
const int GamePort = 7777;

if (args.Contains("--selftest"))
{
    SelfTest.Run();
    return;
}

if (args.Contains("--clienttest"))
{
    // Assumes stub already listening, or start nested? Prefer separate process.
    await ClientSmoke.RunAsync();
    return;
}

var loginListener = new TcpListener(IPAddress.Loopback, LoginPort);
var gameListener = new TcpListener(IPAddress.Loopback, GamePort);
loginListener.Start();
gameListener.Start();
Console.WriteLine($"Fineage stub login 127.0.0.1:{LoginPort}  game 127.0.0.1:{GamePort}");
Console.WriteLine("Menu → Login → Start on Test → Talking Island (17_25). Click ground to run.");
Console.WriteLine("Ctrl+C to stop.");

_ = AcceptLoop(loginListener, HandleLoginAsync);
_ = AcceptLoop(gameListener, HandleGameAsync);
await Task.Delay(Timeout.Infinite);

static async Task AcceptLoop(TcpListener listener, Func<TcpClient, Task> handler)
{
    while (true)
    {
        var client = await listener.AcceptTcpClientAsync();
        _ = Task.Run(async () =>
        {
            try { await handler(client); }
            catch (Exception ex) { Console.WriteLine($"[{Stamp()}] handler: {ex.Message}"); }
        });
    }
}

static async Task HandleLoginAsync(TcpClient client)
{
    var ep = client.Client.RemoteEndPoint;
    Console.WriteLine($"[{Stamp()}] login connect {ep}");
    using (client)
    await using (var stream = client.GetStream())
    {
        byte[] bfKey = RandomNumberGenerator.GetBytes(16);
        int sessionId = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        int sessionKey1 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        int sessionKey2 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        int playKey1 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        int playKey2 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        byte[] scrambled = LoginCrypt.GenerateRsaModulusScrambled(out RSA _);

        // Init
        var initBody = new PacketBuilder()
            .C(0x00)
            .D(sessionId)
            .D(0x0000c621)
            .B(scrambled)
            .D(unchecked((int)0x29dd954e))
            .D(unchecked((int)0x77c39cfc))
            .D(unchecked((int)0x97adb620))
            .D(unchecked((int)0x07bde0f7))
            .B(bfKey)
            .C(0x00)
            .ToArray();
        int xorKey = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
        await Framed.SendAsync(stream, LoginCrypt.EncryptInit(initBody, xorKey));
        Console.WriteLine($"[{Stamp()}] TX Init session={sessionId:X8}");

        // Expect AuthGameGuard (still static BF until client switches — client switches after Init parse)
        // Client decrypts Init with static, then SetBlowFishKey(dynamic) before sending AuthGameGuard.
        // Client C→S login frames: Blowfish only (checksum layout differs by opcode) — don't verify.
        byte[]? rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        byte[] plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
        Console.WriteLine($"[{Stamp()}] RX login opcode=0x{plain[0]:X2} len={plain.Length}");

        // GGAuth
        var gg = new PacketBuilder()
            .C(0x0B)
            .D(sessionId)
            .D(0).D(0).D(0).D(0)
            .ToArray();
        await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(gg, bfKey));
        Console.WriteLine($"[{Stamp()}] TX GGAuth");

        // RequestAuthLogin
        rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
        Console.WriteLine($"[{Stamp()}] RX login opcode=0x{plain[0]:X2} (auth)");

        // LoginOk
        var loginOk = new PacketBuilder().C(0x03).D(sessionKey1).D(sessionKey2).ToArray();
        await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(loginOk, bfKey));
        Console.WriteLine($"[{Stamp()}] TX LoginOk");

        // RequestServerList
        rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
        Console.WriteLine($"[{Stamp()}] RX login opcode=0x{plain[0]:X2} (server list)");

        // ServerList — one server at 127.0.0.1:7777
        var list = new PacketBuilder()
            .C(0x04)
            .C(1)   // count
            .C(1)   // lastServer
            .C(1)   // serverId
            .C(127).C(0).C(0).C(1)
            .D(GamePort)
            .C(0x00) // age limit
            .C(0x01) // pvp
            .H(0)    // currentPlayers
            .H(500)  // maxPlayers
            .C(1)    // status online
            .D(1)    // type / brackets bits
            .C(0)    // brackets
            .ToArray();
        await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(list, bfKey));
        Console.WriteLine($"[{Stamp()}] TX ServerList → 127.0.0.1:{GamePort}");

        // RequestServerLogin
        rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
        Console.WriteLine($"[{Stamp()}] RX login opcode=0x{plain[0]:X2} (server login)");

        var playOk = new PacketBuilder().C(0x07).D(playKey1).D(playKey2).ToArray();
        await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(playOk, bfKey));
        Console.WriteLine($"[{Stamp()}] TX PlayOk — client should connect game");
    }
}

// Talking Island Village — tile 17_25 (maps present in project).
const int SpawnX = -84318;
const int SpawnY = 244578;
const int SpawnZ = -3730;
const int PlayerObjectId = 1;

static async Task HandleGameAsync(TcpClient client)
{
    var ep = client.Client.RemoteEndPoint;
    Console.WriteLine($"[{Stamp()}] game connect {ep}");
    using (client)
    await using (var stream = client.GetStream())
    {
        byte[]? rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        Console.WriteLine($"[{Stamp()}] RX game opcode=0x{rx[0]:X2} len={rx.Length} (ProtocolVersion)");

        var keyPkt = new PacketBuilder()
            .C(0x00)
            .C(1)
            .B(new byte[8])
            .D(0)
            .D(1)
            .ToArray();
        await Framed.SendAsync(stream, PadGame(keyPkt));
        Console.WriteLine($"[{Stamp()}] TX InterludeKey (no crypt)");

        rx = await Framed.ReadBodyAsync(stream);
        if (rx == null) return;
        Console.WriteLine($"[{Stamp()}] RX game opcode=0x{rx[0]:X2} (AuthLogin)");

        await Framed.SendAsync(stream, PadGame(BuildCharSelectionInfo("Test", "test")));
        Console.WriteLine($"[{Stamp()}] TX CharSelectionInfo char=Test");

        int curX = SpawnX, curY = SpawnY, curZ = SpawnZ;
        bool entered = false;

        while (client.Connected)
        {
            rx = await Framed.ReadBodyAsync(stream);
            if (rx == null || rx.Length == 0)
                break;

            byte op = rx[0];
            switch (op)
            {
                case 0x0D: // CharacterSelect
                    Console.WriteLine($"[{Stamp()}] RX CharacterSelect");
                    await Framed.SendAsync(stream, PadGame(BuildCharSelected("Test", PlayerObjectId, curX, curY, curZ)));
                    Console.WriteLine($"[{Stamp()}] TX CharSelected @ {curX},{curY},{curZ} (17_25)");
                    break;

                case 0x03: // EnterWorld
                    Console.WriteLine($"[{Stamp()}] RX EnterWorld");
                    entered = true;
                    await Framed.SendAsync(stream, PadGame(new PacketBuilder().C(0xC1).D(0).ToArray())); // SkillCoolTime empty
                    Console.WriteLine($"[{Stamp()}] TX SkillCoolTime (empty)");
                    break;

                case 0x9D: // RequestSkillCoolTime
                    await Framed.SendAsync(stream, PadGame(new PacketBuilder().C(0xC1).D(0).ToArray()));
                    break;

                case 0x01: // MoveToLocation
                    if (rx.Length < 29)
                    {
                        Console.WriteLine($"[{Stamp()}] RX Move short len={rx.Length}");
                        break;
                    }
                    int tX = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(1));
                    int tY = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(5));
                    int tZ = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(9));
                    int oX = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(13));
                    int oY = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(17));
                    int oZ = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(21));
                    Console.WriteLine($"[{Stamp()}] RX Move → {tX},{tY},{tZ}");
                    await Framed.SendAsync(stream, PadGame(BuildCharMove(PlayerObjectId, tX, tY, tZ, oX, oY, oZ)));
                    curX = tX; curY = tY; curZ = tZ;
                    break;

                case 0x48: // ValidatePosition — ignore
                    break;

                default:
                    if (!entered || op != 0x00)
                        Console.WriteLine($"[{Stamp()}] RX game opcode=0x{op:X2} len={rx.Length}");
                    break;
            }
        }

        Console.WriteLine($"[{Stamp()}] game disconnect {ep}");
    }
}

static byte[] PadGame(byte[] body)
{
    // Mirror client BuildGame: trailing zero int + pad8 (harmless extras after DTO fields).
    var b = new PacketBuilder().B(body).D(0).ToArray();
    int rem = b.Length % 8;
    if (rem == 0)
        return b;
    byte[] padded = new byte[b.Length + (8 - rem)];
    Buffer.BlockCopy(b, 0, padded, 0, b.Length);
    return padded;
}

static byte[] BuildCharSelected(string name, int objectId, int x, int y, int z)
{
    // Matches CharSelectedDto.ReadFrom.
    var p = new PacketBuilder()
        .C(0x15)
        .S(name)
        .D(objectId)
        .S("")          // title
        .D(1)           // sessionId
        .D(0)           // clan
        .D(0)           // empty
        .D(0)           // sex
        .D(0)           // race
        .D(0)           // classId
        .D(1)           // active
        .D(x).D(y).D(z)
        .F(200.0).F(50.0) // hp mp
        .D(0)           // cp (read as int in dto — actually ReadI for Cp)
        .Q(0)           // exp
        .D(1)           // level
        .D(0)           // karma
        .D(0)           // pk
        .D(21).D(40).D(43).D(25).D(30).D(11); // int str con men dex wit

    for (int i = 0; i < 30; i++)
        p.D(0);

    p.D(0).D(0)         // empty1 empty2
        .D(0)           // reset tick
        .D(0)           // empty3
        .D(0);          // classId2
    for (int i = 0; i < 12; i++) // empty31 + empty4..empty14
        p.D(0);

    return p.ToArray();
}

static byte[] BuildCharMove(int objectId, int xDst, int yDst, int zDst, int x, int y, int z)
{
    return new PacketBuilder()
        .C(0x01)
        .D(objectId)
        .D(xDst).D(yDst).D(zDst)
        .D(x).D(y).D(z)
        .ToArray();
}

static byte[] BuildCharSelectionInfo(string name, string account)
{
    // Matches CharSelectionInfoDto.ReadFrom field order (Interlude).
    var p = new PacketBuilder()
        .C(0x13)
        .D(1); // size

    p.S(name)
        .D(1)          // objId
        .S(account)
        .D(1)          // accountId
        .D(0)          // clanId
        .D(0)          // buildLvl
        .D(0)          // sex male
        .D(0)          // race human
        .D(0)          // classId human fighter
        .D(1)          // gsName / active
        .D(0).D(0).D(0)
        .F(200.0)      // hp
        .F(50.0)       // mp
        .D(0)          // sp
        .Q(0)          // exp
        .D(1)          // lvl
        .D(0);         // karma

    for (int i = 0; i < 9; i++)
        p.D(0);

    // paperdoll object ids (17) + item ids ... follow reader carefully
    // Obj: Under, Pear, Lear, Neck, RFinger, LFinger, Head, RHand, LHand, Gloves, Chest, Legs, Feet, Cloak, RHand, Hair, Hair2 = 17
    for (int i = 0; i < 17; i++)
        p.D(0);

    // Item: Under, Rear, Lear, Neck, RFinger, LFinger, Head, RHand, LHand, Gloves = 10
    for (int i = 0; i < 10; i++)
        p.D(0);

    // Appearance Chest, Legs
    p.D(0).D(0);
    // Item Feet, Cloak, RHand, Hair, Hair2
    p.D(0).D(0).D(0).D(0).D(0);

    p.D(0).D(0).D(0); // hairStyle, hairColor, face
    p.F(200.0).F(50.0); // maxHp maxMp
    p.D(0);             // delete timer
    p.D(0);             // classId again
    p.D(1);             // selected
    p.C(0);             // enchant
    p.D(0);             // augmentation

    return p.ToArray();
}

static string Stamp() => DateTime.Now.ToString("HH:mm:ss");
