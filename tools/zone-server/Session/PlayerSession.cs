using System.Net.Sockets;
using Fineage.ZoneServer.Wire;
using MessagePack;

namespace Fineage.ZoneServer.Session;

sealed class PlayerSession
{
    // Talking Island spawn used by login-stub (tile 17_25).
    public const int SpawnX = -84318;
    public const int SpawnY = 244578;
    public const int SpawnZ = -3730;
    public const int PlayerObjectId = 1;

    readonly TcpClient _client;
    readonly NetworkStream _stream;
    readonly int _sessionId;

    public bool Authenticated { get; private set; }
    public bool InWorld { get; private set; }
    public int X { get; private set; } = SpawnX;
    public int Y { get; private set; } = SpawnY;
    public int Z { get; private set; } = SpawnZ;

    public PlayerSession(TcpClient client, int sessionId)
    {
        _client = client;
        _stream = client.GetStream();
        _sessionId = sessionId;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        string ep = _client.Client.RemoteEndPoint?.ToString() ?? "?";
        Console.WriteLine($"[{Stamp()}] zone connect {ep}");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var frame = await FrameIO.ReadAsync(_stream, ct).ConfigureAwait(false);
                if (frame == null)
                    break;

                await HandleAsync(frame.Value.Type, frame.Value.Body, ct).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Stamp()}] zone error {ep}: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"[{Stamp()}] zone disconnect {ep}");
            _client.Dispose();
        }
    }

    async Task HandleAsync(ushort type, byte[] body, CancellationToken ct)
    {
        switch ((MsgType)type)
        {
            case MsgType.Hello:
            {
                var hello = Decode<HelloMsg>(body) ?? new HelloMsg();
                Console.WriteLine($"[{Stamp()}] RX Hello proto={hello.ProtocolVersion} build={hello.ClientBuild}");
                await SendAsync(MsgType.HelloAck, new HelloAckMsg(), ct).ConfigureAwait(false);
                break;
            }
            case MsgType.Auth:
            {
                var auth = Decode<AuthMsg>(body) ?? new AuthMsg();
                Console.WriteLine($"[{Stamp()}] RX Auth account={auth.Account}");
                // Stub: accept any credentials.
                Authenticated = true;
                await SendAsync(MsgType.AuthOk, new AuthOkMsg
                {
                    SessionId = _sessionId,
                    CharacterName = string.IsNullOrWhiteSpace(auth.Account) ? "Test" : auth.Account
                }, ct).ConfigureAwait(false);
                break;
            }
            case MsgType.EnterWorld:
            {
                if (!Authenticated)
                {
                    await SendAsync(MsgType.Error, new ErrorMsg { Code = "auth", Message = "Auth required" }, ct)
                        .ConfigureAwait(false);
                    break;
                }

                Console.WriteLine($"[{Stamp()}] RX EnterWorld → spawn TI");
                InWorld = true;
                X = SpawnX;
                Y = SpawnY;
                Z = SpawnZ;
                await SendAsync(MsgType.PlayerSpawned, new PlayerSpawnedMsg
                {
                    ObjectId = PlayerObjectId,
                    Name = "Test",
                    X = X,
                    Y = Y,
                    Z = Z,
                    RunSpeed = 120f,
                    WalkSpeed = 80f,
                    Running = true
                }, ct).ConfigureAwait(false);
                break;
            }
            case MsgType.MoveTo:
            {
                if (!InWorld)
                {
                    await SendAsync(MsgType.Error, new ErrorMsg { Code = "world", Message = "EnterWorld first" }, ct)
                        .ConfigureAwait(false);
                    break;
                }

                var move = Decode<MoveToMsg>(body);
                if (move == null)
                    break;

                Console.WriteLine($"[{Stamp()}] RX Move → {move.DestX},{move.DestY},{move.DestZ}");
                int oX = move.OriginX;
                int oY = move.OriginY;
                int oZ = move.OriginZ;
                X = move.DestX;
                Y = move.DestY;
                Z = move.DestZ;
                // Authoritative echo (DomainMoveTo on client).
                move.ObjectId = PlayerObjectId;
                move.OriginX = oX;
                move.OriginY = oY;
                move.OriginZ = oZ;
                await SendAsync(MsgType.MoveToAck, move, ct).ConfigureAwait(false);
                break;
            }
            default:
                Console.WriteLine($"[{Stamp()}] RX unknown type={type} len={body.Length}");
                await SendAsync(MsgType.Error, new ErrorMsg
                {
                    Code = "unknown",
                    Message = $"msgType={type}"
                }, ct).ConfigureAwait(false);
                break;
        }
    }

    async Task SendAsync<T>(MsgType type, T msg, CancellationToken ct)
    {
        byte[] body = MessagePackSerializer.Serialize(msg);
        await FrameIO.SendAsync(_stream, (ushort)type, body, ct).ConfigureAwait(false);
        Console.WriteLine($"[{Stamp()}] TX {type}");
    }

    static T? Decode<T>(byte[] body) where T : class
    {
        if (body.Length == 0)
            return null;
        return MessagePackSerializer.Deserialize<T>(body);
    }

    static string Stamp() => DateTime.Now.ToString("HH:mm:ss");
}
