using UnityEngine;
using VContainer;

public class GameClient : DefaultClient {
    [Inject] IZoneTransport _zoneTransport;

    private GameManager Manager => NetworkingPacketServices.Manager;
    protected PlayerInfoInterlude _playerInfo;
    protected int _serverId;
    private readonly object syncLock = new object();
    private bool _isLoadComplete { get; set; }

    public PlayerInfoInterlude PlayerInfo { get { return _playerInfo; } set { _playerInfo = value; } }
    public string CurrentPlayer { get { return _playerInfo.Identity.Name; } }
    public int ServerId { get { return _serverId; } set { _serverId = value; } }

    private GameClientPacketHandler clientPacketHandler;
    private GameServerPacketHandler serverPacketHandler;

    public GameClientPacketHandler ClientPacketHandler { get { return clientPacketHandler; } }
    public GameServerPacketHandler ServerPacketHandler { get { return serverPacketHandler; } }

    private static GameClient _instance;
    public static GameClient Instance { get { return _instance; } }

    private void Awake() {
        GameManager manager = GameManager.Instance;
        if (manager != null)
            NetworkingPacketServices.Bind(manager: manager);
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    protected override void CreateAsyncClient() {
        // Always replace prior game socket before PlayOk→Connect (half-open after skipped AuthLogin).
        AsynchronousClient previous = _client;
        _client = null;
        if (previous != null)
        {
            try { previous.Disconnect(); } catch { /* ignore */ }
        }

        if (Network == null)
            throw new System.InvalidOperationException("NetworkRuntime not ready (AppLifetimeScope)");

        clientPacketHandler = new GameClientPacketHandler();
        serverPacketHandler = new GameServerPacketHandler(Network.Protocol, Network.Dispatcher);

        _client = new AsynchronousClient(_serverIp, _serverPort, this, clientPacketHandler, serverPacketHandler, false, Network);
    }

    public void EnableCrypt(byte[] key) {
        if (Network != null && Network.Protocol != null)
            Network.Protocol.SetGameCryptKey(key);
        _client.CryptEnabled = true;
    }

    public bool IsCryptEnabled()
    {
        return _client != null && _client.CryptEnabled;
    }


    protected override void WhileConnecting() {
        base.WhileConnecting();

        Manager.OnConnectingToGameServer();
    }

    protected override void OnConnectionSuccess() {
        base.OnConnectionSuccess();
        LobbyFlowLog.Info("GameClient.OnConnectionSuccess — send ProtocolVersion");

        GameManager manager = Manager;
        if (manager != null)
            manager.IsSwitchingServer = false;
        else
            LobbyFlowLog.Error("GameClient.OnConnectionSuccess Manager is null");

        int protocol = manager != null ? manager.ProtocolVersion : 746;
        LobbyFlowLog.Info("TX ProtocolVersion=" + protocol + " crypt=" + IsCryptEnabled());
        SendPlain(new ProtocolVersionCommand(protocol));
    }

    public void Send(INetworkCommand command)
    {
        IZoneTransport transport = ResolveZoneTransport();
        if (transport != null && transport.TrySendGameCommand(command))
            return;

        EnqueueGame(command, IsCryptEnabled());
    }

    public void SendPlain(INetworkCommand command)
    {
        EnqueueGame(command, false);
    }

    private void EnqueueGame(INetworkCommand command, bool crypt)
    {
        if (command == null || Network == null)
            return;

        Network.SendGame.AddItem(command, crypt);
    }

    private IZoneTransport ResolveZoneTransport()
    {
        if (_zoneTransport != null)
            return _zoneTransport;
        if (App.HasContainer)
            return App.Resolve<IZoneTransport>();
        return null;
    }

    public void EndLoadWorld()
    {
        Send(new RequestSkillCoolTimeCommand());
    }

    public override void OnConnectionFailed() {
        LobbyFlowLog.Warn("GameClient.OnConnectionFailed ip=" + _serverIp + " port=" + _serverPort);
        base.OnConnectionFailed();
        GameManager manager = Manager;
        if (manager != null)
        {
            manager.IsSwitchingServer = false;
            manager.OnRelogin();
        }
    }

    public override void OnAuthAllowed() {
        GameManager manager = Manager;
        if (manager != null)
            manager.OnAuthAllowed();
    }

    public override void OnDisconnect() {
        LobbyFlowLog.Info("GameClient.Disconnect");
        base.OnDisconnect();
    }

   public bool DataPreparationCompleted()
    {
        lock (syncLock) {
           return _isLoadComplete;
        }
    }

    public bool SetDataPreparationCompleted(bool isComplete)
    {
        lock (syncLock)
        {
            return _isLoadComplete = isComplete;
        }
    }
}
