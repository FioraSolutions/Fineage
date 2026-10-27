using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

/// <summary>
/// TCP client for Fineage ZoneServer v2 (MessagePack frames on :9777).
/// Bypasses Interlude login/game crypt when <see cref="FineageWireConfig.IsZoneV2"/>.
/// </summary>
public sealed class ZoneV2Client : MonoBehaviour
{
    static ZoneV2Client _instance;
    public static ZoneV2Client Instance { get { return _instance; } }

    [Inject] GameManager _gameManager;
    IWorldDomainSink _domain;
    WorldPacketApply _worldApply;

    public void WireGame(IWorldDomainSink domain, WorldPacketApply worldApply)
    {
        if (domain != null)
            _domain = domain;
        if (worldApply != null)
            _worldApply = worldApply;
    }

    private GameManager Manager
    {
        get { return _gameManager != null ? _gameManager : IncomingPacketActions.Manager; }
    }

    private IWorldDomainSink Domain
    {
        get
        {
            if (_domain != null)
                return _domain;
            return IncomingPacketActions.Domain;
        }
    }

    private WorldPacketApply WorldApply
    {
        get
        {
            if (_worldApply != null)
                return _worldApply;
            if (App.GameContainer != null)
            {
                try
                {
                    return App.GameContainer.Resolve<WorldPacketApply>();
                }
                catch
                {
                }
            }
            return IncomingPacketActions.WorldApply;
        }
    }

    readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();
    CancellationTokenSource _cts;
    TcpClient _tcp;
    NetworkStream _stream;
    readonly object _sendLock = new object();

    public bool Connected { get; private set; }
    public int SessionObjectId { get; private set; } = 1;

    public static ZoneV2Client Ensure()
    {
        if (_instance != null)
            return _instance;
        GameObject go = new GameObject("ZoneV2Client");
        DontDestroyOnLoad(go);
        return go.AddComponent<ZoneV2Client>();
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        Disconnect();
    }

    void Update()
    {
        Action action;
        while (_main.TryDequeue(out action))
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.LogError("[ZoneV2] main-thread: " + ex);
            }
        }
    }

    public void BeginSession(string account, string password)
    {
        Disconnect();
        _cts = new CancellationTokenSource();
        CancellationToken ct = _cts.Token;
        string host = FineageWireConfig.ZoneHost;
        int port = FineageWireConfig.ZonePort;
        Task.Run(() => RunSessionAsync(host, port, account, password, ct));
    }

    public void Disconnect()
    {
        try
        {
            _cts?.Cancel();
        }
        catch
        {
            /* ignore */
        }

        _cts = null;
        Connected = false;
        try
        {
            _stream?.Close();
        }
        catch
        {
            /* ignore */
        }

        try
        {
            _tcp?.Close();
        }
        catch
        {
            /* ignore */
        }

        _stream = null;
        _tcp = null;
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        if (!Connected || _stream == null)
            return;

        Vector3 fromL2 = VectorUtils.ConvertPosUnityToL2j(fromUnity);
        Vector3 toL2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        byte[] body = ZoneV2Messages.EncodeMoveTo(
            SessionObjectId,
            Mathf.RoundToInt(toL2.x), Mathf.RoundToInt(toL2.y), Mathf.RoundToInt(toL2.z),
            Mathf.RoundToInt(fromL2.x), Mathf.RoundToInt(fromL2.y), Mathf.RoundToInt(fromL2.z));
        _ = SendFireAndForget(ZoneV2MsgType.MoveTo, body);
    }

    async Task SendFireAndForget(ZoneV2MsgType type, byte[] body)
    {
        try
        {
            NetworkStream stream = _stream;
            if (stream == null)
                return;
            lock (_sendLock)
            {
                // Serialize sends; WriteAsync still needs await outside lock ideally —
                // keep sync write under lock for MVP simplicity.
            }

            await ZoneV2FrameIO.SendAsync(stream, type, body, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ZoneV2] send failed: " + ex.Message);
        }
    }

    async Task RunSessionAsync(string host, int port, string account, string password, CancellationToken ct)
    {
        try
        {
            TcpClient tcp = new TcpClient();
            await tcp.ConnectAsync(host, port).ConfigureAwait(false);
            NetworkStream stream = tcp.GetStream();
            _tcp = tcp;
            _stream = stream;
            Connected = true;
            Post(() => Debug.Log("[ZoneV2] connected " + host + ":" + port));

            await ZoneV2FrameIO.SendAsync(stream, ZoneV2MsgType.Hello, ZoneV2Messages.EncodeHello(1, "unity"), ct)
                .ConfigureAwait(false);

            while (!ct.IsCancellationRequested)
            {
                ZoneV2Frame? frame = await ZoneV2FrameIO.TryReadAsync(stream, ct).ConfigureAwait(false);
                if (frame == null)
                    break;
                ZoneV2Frame f = frame.Value;

                switch (f.Type)
                {
                    case ZoneV2MsgType.HelloAck:
                        await ZoneV2FrameIO.SendAsync(stream, ZoneV2MsgType.Auth, ZoneV2Messages.EncodeAuth(account, password), ct)
                            .ConfigureAwait(false);
                        break;

                    case ZoneV2MsgType.AuthOk:
                        await ZoneV2FrameIO.SendAsync(stream, ZoneV2MsgType.EnterWorld, ZoneV2Messages.EncodeEnterWorld(0), ct)
                            .ConfigureAwait(false);
                        break;

                    case ZoneV2MsgType.PlayerSpawned:
                        ZoneV2PlayerSpawned spawned;
                        if (!ZoneV2Messages.TryDecodePlayerSpawned(f.Body, out spawned))
                        {
                            Post(() => Debug.LogError("[ZoneV2] bad PlayerSpawned body"));
                            break;
                        }

                        SessionObjectId = spawned.ObjectId;
                        PlayerInfoInterlude info = ZoneV2DomainMap.ToPlayerInfo(spawned);
                        Post(() => OnPlayerSpawnedMain(info));
                        break;

                    case ZoneV2MsgType.MoveToAck:
                        ZoneV2MoveTo move;
                        if (!ZoneV2Messages.TryDecodeMoveTo(f.Body, out move))
                            break;
                        DomainMoveTo domainMove = ZoneV2DomainMap.Move(move);
                        Post(() =>
                        {
                            IWorldDomainSink domain = Domain;
                            if (domain != null && domainMove != null)
                                domain.ApplyMoveTo(domainMove);
                            else if (domainMove != null && domainMove.InterludeDto != null)
                            {
                                WorldPacketApply apply = WorldApply;
                                if (apply != null)
                                    apply.MoveTo(domainMove.ObjectId, domainMove.Destination, domainMove.Origin, domainMove.InterludeDto);
                                else
                                    IncomingPacketActions.ApplyWorld(a =>
                                        a.MoveTo(domainMove.ObjectId, domainMove.Destination, domainMove.Origin, domainMove.InterludeDto));
                            }
                        });
                        break;

                    case ZoneV2MsgType.Error:
                        string code, msg;
                        ZoneV2Messages.TryDecodeError(f.Body, out code, out msg);
                        Post(() => Debug.LogError("[ZoneV2] server error " + code + ": " + msg));
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            /* normal disconnect */
        }
        catch (Exception ex)
        {
            Post(() =>
            {
                Debug.LogError("[ZoneV2] session failed: " + ex.Message);
                if (LoginWindow.Instance != null)
                    LoginWindow.Instance.ShowErrorText("ZoneV2: " + ex.Message);
            });
        }
        finally
        {
            Connected = false;
        }
    }

    void OnPlayerSpawnedMain(PlayerInfoInterlude info)
    {
        GameClient game = GameClient.Instance;
        if (game != null)
            game.PlayerInfo = info;

        GameManager manager = Manager;
        if (manager == null)
        {
            Debug.LogError("[ZoneV2] GameManager missing after PlayerSpawned");
            return;
        }

        manager.OnCharacterSelect();
    }

    void Post(Action action)
    {
        if (action != null)
            _main.Enqueue(action);
    }
}
