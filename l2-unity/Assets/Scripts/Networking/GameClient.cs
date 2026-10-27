using UnityEngine;

public class GameClient : DefaultClient {
    protected PlayerInfoInterlude _playerInfo;
    protected int _serverId;
    private int _playKey1;
    private int _playKey2;
    private readonly object syncLock = new object();
    private bool _isLoadComplete { get; set; }

    public PlayerInfoInterlude PlayerInfo { get { return _playerInfo; } set { _playerInfo = value; } }
    public string CurrentPlayer { get { return _playerInfo.Identity.Name; } }
    public int ServerId { get { return _serverId; } set { _serverId = value; } }
    public int PlayKey1 { get { return _playKey1; } set { _playKey1 = value; } }
    public int PlayKey2 { get { return _playKey2; } set { _playKey2 = value; } } 

    private GameClientPacketHandler clientPacketHandler;
    private GameServerPacketHandler serverPacketHandler;

    public GameClientPacketHandler ClientPacketHandler { get { return clientPacketHandler; } }
    public GameServerPacketHandler ServerPacketHandler { get { return serverPacketHandler; } }

    private static GameClient _instance;
    public static GameClient Instance { get { return _instance; } }

    private void Awake() {
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

        IncomingPacketActions.Manager.OnConnectingToGameServer();
    }

    protected override void OnConnectionSuccess() {
        base.OnConnectionSuccess();
        LobbyFlowLog.Info("GameClient.OnConnectionSuccess — send ProtocolVersion");

        GameManager manager = IncomingPacketActions.Manager;
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
        if (FineageWireConfig.IsZoneV2)
        {
            MoveToCommand move = command as MoveToCommand;
            if (move != null)
            {
                ZoneV2Client zone = ZoneV2Client.Instance;
                if (zone != null)
                    zone.SendMove(move.From, move.To);
                return;
            }

            // Other Interlude game commands are no-ops on ZoneV2 until wired.
            return;
        }

        EnqueueGame(command, IsCryptEnabled());
    }

    public void SendPlain(INetworkCommand command)
    {
        EnqueueGame(command, false);
    }

    /// <summary>
    /// Encode + write AuthLogin on the current game socket immediately (no SendGame queue).
    /// Used right after InterludeKey so lobby cannot stall when the queue worker is stale.
    /// </summary>
    public bool SendAuthLoginNow(string account, int playKey1, int playKey2, int loginKey1, int loginKey2)
    {
        if (FineageWireConfig.IsZoneV2)
        {
            LobbyFlowLog.Error("SendAuthLoginNow blocked — WireMode=ZoneV2 (Interlude stub path required)");
            return false;
        }

        if (Network == null || Network.Protocol == null)
        {
            LobbyFlowLog.Error("SendAuthLoginNow failed — NetworkRuntime/Protocol null");
            return false;
        }

        if (_client == null || !_client.IsConnected)
        {
            LobbyFlowLog.Error("SendAuthLoginNow failed — game socket null/disconnected");
            return false;
        }

        try
        {
            IOutgoingPacket packet = Network.Protocol.EncodeGame(
                new AuthLoginCommand(account, playKey1, playKey2, loginKey1, loginKey2),
                encrypt: false);
            if (packet == null)
            {
                LobbyFlowLog.Error("SendAuthLoginNow failed — EncodeGame returned null");
                return false;
            }

            byte[] data = packet.GetData();
            int bodyLen = data != null ? data.Length : 0;
            bool ok = _client.TrySendPacket(packet);
            LobbyFlowLog.Info("SendAuthLoginNow bodyLen=" + bodyLen + " wrote=" + ok);
            return ok;
        }
        catch (System.Exception ex)
        {
            LobbyFlowLog.Exception("SendAuthLoginNow", ex);
            return false;
        }
    }

    private void EnqueueGame(INetworkCommand command, bool crypt)
    {
        if (command == null || Network == null)
            return;

        Network.SendGame.AddItem(command, crypt);
    }

    public void EndLoadWorld()
    {
        Send(new RequestSkillCoolTimeCommand());
    }

    public override void OnConnectionFailed() {
        LobbyFlowLog.Warn("GameClient.OnConnectionFailed ip=" + _serverIp + " port=" + _serverPort);
        base.OnConnectionFailed();
        IncomingPacketActions.Manager.IsSwitchingServer = false;
        IncomingPacketActions.Manager.OnRelogin();
    }

    public override void OnAuthAllowed() {
        IncomingPacketActions.Manager.OnAuthAllowed();
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
