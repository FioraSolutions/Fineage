using UnityEngine;
using System;
using System.Threading.Tasks;
using VContainer;

public abstract class DefaultClient : MonoBehaviour
{
    [Inject] protected NetworkRuntime _network;
    [Inject] protected GameManager _gameManager;
    [Inject] protected World _world;
    [Header("Connection")]
    [SerializeField] protected string _serverIp = "127.0.0.1";
    [SerializeField] protected int _serverPort = 11000;

    protected AsynchronousClient _client;
    protected int _connectionTimeoutMs = 10000;
    protected bool _connected = false;
    protected bool _logReceivedPackets = false;
    protected bool _logSentPackets = false;
    protected bool _logCryptography = false;
    protected int _sessionKey1;
    protected int _sessionKey2;
    protected int _ping;

    private bool _connecting = false;
    public bool LogReceivedPackets { get { return _logReceivedPackets; } }
    public bool LogSentPackets { get { return _logSentPackets; } }
    public bool LogCryptography { get { return _logCryptography; } }
    public int ConnectionTimeoutMs { get { return _connectionTimeoutMs; } }
    public string ServerIp { get { return _serverIp; } set { _serverIp = value; } }
    public int ServerPort { get { return _serverPort; } set { _serverPort = value; } }
    public int SessionKey1 { get { return _sessionKey1; } set { _sessionKey1 = value; } }
    public int SessionKey2 { get { return _sessionKey2; } set { _sessionKey2 = value; } }
    public bool IsConnected { get { return _connected; } }
    public int Ping { get { return _ping; } set { _ping = value; } }

    private GameManager Manager
    {
        get { return _gameManager != null ? _gameManager : IncomingPacketActions.Manager; }
    }

    private World GameWorld
    {
        get
        {
            if (_world != null)
                return _world;
            if (App.GameContainer != null)
            {
                try
                {
                    return App.GameContainer.Resolve<World>();
                }
                catch
                {
                }
            }
            return IncomingPacketActions.GameWorld;
        }
    }

    private void Start()
    {
        World world = GameWorld;
        if (world != null && world.OfflineMode)
        {
            this.enabled = false;
        }
    }

    public async void Connect()
    {
        _connected = false;
        if (_connecting)
        {
            LobbyFlowLog.Warn(GetType().Name + ".Connect skipped — already connecting");
            return;
        }

        // Arm guard before CreateAsyncClient so a re-entrant Connect cannot replace the socket mid-flight.
        _connecting = true;
        LobbyFlowLog.Info(GetType().Name + ".Connect start ip=" + _serverIp + " port=" + _serverPort);
        try
        {
            CreateAsyncClient();
        }
        catch (Exception ex)
        {
            _connecting = false;
            LobbyFlowLog.Exception(GetType().Name + ".CreateAsyncClient", ex);
            return;
        }

        WhileConnecting();

        bool connected = false;
        try
        {
            connected = await Task.Run(_client.Connect);
        }
        catch (Exception ex)
        {
            LobbyFlowLog.Exception(GetType().Name + ".Connect Task.Run", ex);
        }

        _connecting = false;
        EventProcessor events = Events;
        LobbyFlowLog.Info(GetType().Name + ".Connect socketResult=" + connected +
            " events=" + (events != null) + " manager=" + (Manager != null));

        if (connected)
        {
            if (events != null)
            {
                events.QueueEvent(() =>
                {
                    LobbyFlowLog.Info(GetType().Name + ".OnConnectionSuccess (EventProcessor)");
                    OnConnectionSuccess();
                });
            }
            else
            {
                LobbyFlowLog.Warn(GetType().Name + ".Connect no EventProcessor — OnConnectionSuccess inline");
                OnConnectionSuccess();
            }
        }
        else if (events != null)
        {
            events.QueueEvent(() => OnConnectionFailed());
        }
        else
        {
            OnConnectionFailed();
        }
    }

    protected virtual void WhileConnecting()
    {
        _connecting = true;
    }

    protected abstract void CreateAsyncClient();

    protected virtual void OnConnectionSuccess()
    {
        _connected = true;
    }

    public virtual void OnConnectionFailed()
    {
        _connecting = false;
        _connected = false;
        // Allow a fresh AsynchronousClient on the next Connect (failed sockets leave a dead instance).
        _client = null;
    }

    public abstract void OnAuthAllowed();

    public void Disconnect()
    {
        _connected = false;

        if (_client != null)
        {
            _client.Disconnect();
        }
    }

    public virtual void OnDisconnect()
    {
        OnDisconnectFrom(_client);
    }

    /// <summary>
    /// Only clear the active socket when this callback is for that instance.
    /// CreateAsyncClient may replace _client before a prior Disconnect event runs.
    /// </summary>
    public virtual void OnDisconnectFrom(AsynchronousClient source)
    {
        _connected = false;
        if (source != null && !ReferenceEquals(_client, source))
            return;

        _client = null;
        GameManager manager = Manager;
        if (manager != null)
            manager.OnDisconnect();
    }

    protected NetworkRuntime Network
    {
        get
        {
            if (_network == null && App.HasContainer)
                _network = App.Resolve<NetworkRuntime>();
            return _network;
        }
    }

    protected EventProcessor Events
    {
        get
        {
            NetworkRuntime runtime = Network;
            if (runtime != null && runtime.Events != null)
                return runtime.Events;
            return EventProcessor.Instance;
        }
    }

    void OnApplicationQuit()
    {
        if (_client != null)
        {
            _client.Disconnect();
        }
    }
}