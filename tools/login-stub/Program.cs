using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Fineage.LoginStub;
using Fineage.LoginStub.Crypto;

// Fineage login+game stub: Init/Blowfish auth → ServerList → PlayOk → CharSelectionInfo.
const int LoginPort = 2106;
const int GamePort = 7777;
StubLog.Configure(args.Contains("--verbose") || args.Contains("-v"));

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
StubLog.Info($"listen bind login=127.0.0.1:{LoginPort} game=127.0.0.1:{GamePort}");
Console.WriteLine("Menu → Login → Start on Test → Talking Island (17_25). Click ground to run.");
Console.WriteLine(StubLog.Verbose
    ? "Verbose: on (opcode/len + Debug)."
    : "Info milestones; pass --verbose or FINEAGE_STUB_DEBUG=1 for Debug.");
Console.WriteLine("File: logs/stub.log  |  Ctrl+C to stop.");

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
            catch (Exception ex) { StubLog.Error($"handler: {ex.Message}"); }
        });
    }
}

static async Task HandleLoginAsync(TcpClient client)
{
    var ep = client.Client.RemoteEndPoint?.ToString() ?? "?";
    int sessionId = 0;
    StubLog.Info($"login connect remote={ep}");
    using (client)
    await using (var stream = client.GetStream())
    {
        try
        {
            byte[] bfKey = RandomNumberGenerator.GetBytes(16);
            sessionId = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            int sessionKey1 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            int sessionKey2 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            int playKey1 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            int playKey2 = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            byte[] scrambled = LoginCrypt.GenerateRsaModulusScrambled(out RSA _);
            string sid = $"session={sessionId:X8}";

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
            StubLog.Info($"TX Init {sid}");

            // Expect AuthGameGuard (still static BF until client switches — client switches after Init parse)
            // Client decrypts Init with static, then SetBlowFishKey(dynamic) before sending AuthGameGuard.
            // Client C→S login frames: Blowfish only (checksum layout differs by opcode) — don't verify.
            byte[]? rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"login EOF before AuthGameGuard {sid}");
                return;
            }
            byte[] plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
            StubLog.Milestone($"RX AuthGameGuard {sid}", plain);
            StubLog.Hex("AuthGameGuard", plain);

            // GGAuth
            var gg = new PacketBuilder()
                .C(0x0B)
                .D(sessionId)
                .D(0).D(0).D(0).D(0)
                .ToArray();
            await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(gg, bfKey));
            StubLog.Info($"TX GGAuth {sid}");

            // RequestAuthLogin
            rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"login EOF before RequestAuthLogin {sid}");
                return;
            }
            plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
            StubLog.Milestone($"RX RequestAuthLogin {sid}", plain);
            StubLog.Hex("RequestAuthLogin", plain);

            // LoginOk (stub always accepts)
            var loginOk = new PacketBuilder().C(0x03).D(sessionKey1).D(sessionKey2).ToArray();
            await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(loginOk, bfKey));
            StubLog.Info($"TX LoginOk {sid} sk1={sessionKey1:X8} sk2={sessionKey2:X8}");

            // RequestServerList
            rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"login EOF before RequestServerList {sid}");
                return;
            }
            plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
            StubLog.Milestone($"RX RequestServerList {sid}", plain);

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
            StubLog.Info($"TX ServerList {sid} → 127.0.0.1:{GamePort} servers=1");

            // RequestServerLogin
            rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"login EOF before RequestServerLogin {sid}");
                return;
            }
            plain = LoginCrypt.BlowfishProcess(bfKey, encrypt: false, rx);
            int serverId = plain.Length > 9 ? plain[9] : -1;
            StubLog.Milestone($"RX RequestServerLogin {sid} serverId={serverId}", plain);

            var playOk = new PacketBuilder().C(0x07).D(playKey1).D(playKey2).ToArray();
            await Framed.SendAsync(stream, LoginCrypt.EncryptLogin(playOk, bfKey));
            StubLog.Info($"TX PlayOk {sid} → client should connect game :{GamePort}");
        }
        finally
        {
            StubLog.Info($"login disconnect remote={ep} session={(sessionId != 0 ? sessionId.ToString("X8") : "?")}");
        }
    }
}

// Talking Island Village — tile 17_25 (maps present in project).
const int SpawnX = -84318;
const int SpawnY = 244578;
const int SpawnZ = -3730;
const int PlayerObjectId = 1;

static async Task HandleGameAsync(TcpClient client)
{
    var ep = client.Client.RemoteEndPoint?.ToString() ?? "?";
    string account = "?";
    StubLog.Info($"game connect remote={ep}");
    using (client)
    await using (var stream = client.GetStream())
    {
        try
        {
            byte[]? rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"game EOF before ProtocolVersion remote={ep}");
                return;
            }
            StubLog.Milestone($"RX ProtocolVersion remote={ep}", rx);

            var keyPkt = new PacketBuilder()
                .C(0x00)
                .C(1)
                .B(new byte[8])
                .D(0)
                .D(1)
                .ToArray();
            await Framed.SendAsync(stream, PadGame(keyPkt));
            StubLog.Info($"TX InterludeKey (no crypt) remote={ep}");

            rx = await Framed.ReadBodyAsync(stream);
            if (rx == null)
            {
                StubLog.Warn($"game EOF before AuthLogin remote={ep}");
                return;
            }
            account = TryParseAuthLoginAccount(rx) ?? "?";
            StubLog.Milestone($"RX AuthLogin account={account} remote={ep}", rx);

            await Framed.SendAsync(stream, PadGame(BuildCharSelectionInfo("Test", account == "?" ? "test" : account)));
            StubLog.Info($"TX CharSelectionInfo char=Test account={account}");

            int curX = SpawnX, curY = SpawnY, curZ = SpawnZ;
            bool entered = false;
            string ctx = $"account={account}";

            while (client.Connected)
            {
                rx = await Framed.ReadBodyAsync(stream);
                if (rx == null || rx.Length == 0)
                    break;

                byte op = rx[0];
                switch (op)
                {
                    case 0x0D: // CharacterSelect
                        StubLog.Info($"RX CharacterSelect {ctx}");
                        await Framed.SendAsync(stream, PadGame(BuildCharSelected("Test", PlayerObjectId, curX, curY, curZ)));
                        StubLog.Info($"TX CharSelected {ctx} @ {curX},{curY},{curZ} (17_25)");
                        break;

                    case 0x03: // EnterWorld
                        StubLog.Info($"RX EnterWorld {ctx}");
                        entered = true;
                        await Framed.SendAsync(stream, PadGame(new PacketBuilder().C(0xC1).D(0).ToArray())); // SkillCoolTime empty
                        StubLog.Info($"TX SkillCoolTime (empty) {ctx}");
                        break;

                    case 0x9D: // RequestSkillCoolTime
                        StubLog.Debug($"RX RequestSkillCoolTime {ctx}");
                        await Framed.SendAsync(stream, PadGame(new PacketBuilder().C(0xC1).D(0).ToArray()));
                        break;

                    case 0x01: // MoveToLocation
                        if (rx.Length < 29)
                        {
                            StubLog.Warn($"RX Move short len={rx.Length} {ctx}");
                            break;
                        }
                        int tX = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(1));
                        int tY = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(5));
                        int tZ = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(9));
                        int oX = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(13));
                        int oY = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(17));
                        int oZ = BinaryPrimitives.ReadInt32LittleEndian(rx.AsSpan(21));
                        StubLog.Info($"RX Move {ctx} → {tX},{tY},{tZ}");
                        await Framed.SendAsync(stream, PadGame(BuildCharMove(PlayerObjectId, tX, tY, tZ, oX, oY, oZ)));
                        StubLog.Info($"TX CharMoveToLocation {ctx} → {tX},{tY},{tZ}");
                        curX = tX; curY = tY; curZ = tZ;
                        break;

                    case 0x48: // ValidatePosition — ignore
                        StubLog.Debug($"RX ValidatePosition {ctx}");
                        break;

                    default:
                        if (!entered || op != 0x00)
                            StubLog.Debug($"RX game opcode=0x{op:X2} len={rx.Length} {ctx}");
                        break;
                }
            }
        }
        finally
        {
            StubLog.Info($"game disconnect remote={ep} account={account}");
        }
    }
}

/// <summary>Best-effort AuthLogin account: null-terminated ASCII, or length-prefixed (ClientSmoke).</summary>
static string? TryParseAuthLoginAccount(byte[] rx)
{
    if (rx.Length < 3 || rx[0] != 0x08)
        return null;

    // Length-prefixed ASCII (ClientSmoke): C(len) + bytes
    int len = rx[1];
    if (len > 0 && len < 32 && 2 + len <= rx.Length)
    {
        bool ascii = true;
        for (int i = 0; i < len; i++)
        {
            byte b = rx[2 + i];
            if (b < 0x20 || b > 0x7E) { ascii = false; break; }
        }
        if (ascii)
            return Encoding.ASCII.GetString(rx, 2, len);
    }

    // Null-terminated ASCII from offset 1
    int end = 1;
    while (end < rx.Length && rx[end] != 0 && end - 1 < 32)
        end++;
    if (end > 1)
    {
        bool ascii = true;
        for (int i = 1; i < end; i++)
        {
            if (rx[i] < 0x20 || rx[i] > 0x7E) { ascii = false; break; }
        }
        if (ascii)
            return Encoding.ASCII.GetString(rx, 1, end - 1);
    }

    return null;
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
