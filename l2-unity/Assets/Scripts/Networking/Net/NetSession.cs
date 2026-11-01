using System;
using System.Collections.Concurrent;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Fineage Net session: lobby + zone over PacketEnvelope. Maps inbound events to
/// <see cref="IWorldDomainSink"/> — no ZoneV2DomainMap / Interlude DTO bridge.
/// </summary>
public sealed class NetSession : MonoBehaviour
{
    public static NetSession Instance { get; private set; }

    readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();
    NetTransport _lobby;
    NetTransport _zone;
    uint _seq = 1;
    IWorldDomainSink _sink;
    string _account;
    HandoffMsg _handoff;
    bool _helloLobby;
    bool _helloZone;

    public bool HasHandoff => _handoff != null;
    public HandoffMsg LastHandoff => _handoff;
    public event Action<CharacterRosterMsg> RosterReceived;
    public event Action<AuthResultMsg> AuthResultReceived;
    public event Action<HandoffMsg> HandoffReceived;
    public event Action EnteredWorld;

    public static NetSession Ensure()
    {
        if (Instance != null)
            return Instance;
        var go = new GameObject("NetSession");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<NetSession>();
        return Instance;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        while (_main.TryDequeue(out Action action))
        {
            try { action(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }

    public void BindDomainSink(IWorldDomainSink sink) => _sink = sink;

    public async void BeginLobby(string account, string password, bool registerIfMissing = false)
    {
        _account = account;
        try
        {
            _lobby?.Dispose();
            _lobby = new NetTransport(OnLobbyPacket, reason => Post(() => Debug.Log("[Net] lobby " + reason)));
            await _lobby.ConnectAsync(FineageWireConfig.LoginHost, FineageWireConfig.LoginPort);
            await SendHelloAsync(_lobby, "unity-net");
            await WaitUntil(() => _helloLobby, 5f);
            await _lobby.SendAsync(
                NetOpcode.Auth,
                NetCodec.EncodeAuth(new AuthMsg
                {
                    Account = account,
                    Password = password,
                    RegisterIfMissing = registerIfMissing
                }),
                NextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] BeginLobby failed: " + ex.Message);
        }
    }

    public async void RequestRoster()
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(NetOpcode.ListCharacters, NetCodec.EncodeListCharacters(new ListCharactersMsg()), NextSeq());
    }

    public async void SelectCharacter(int characterId, string name = "", int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(
            NetOpcode.SelectCharacter,
            NetCodec.EncodeSelectCharacter(new SelectCharacterMsg
            {
                CharacterId = characterId,
                Name = name ?? "",
                Slot = slot
            }),
            NextSeq());
    }

    public async void CreateCharacter(
        string name,
        int classId,
        int race,
        int sex,
        int hairStyle = 0,
        int hairColor = 0,
        int face = 0)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] CreateCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.CreateCharacter,
            NetCodec.EncodeCreateCharacter(new CreateCharacterMsg
            {
                Name = name ?? "",
                ClassId = classId,
                Race = race,
                Sex = sex,
                HairStyle = hairStyle,
                HairColor = hairColor,
                Face = face
            }),
            NextSeq());
    }

    public async void EnterZoneWithHandoff(HandoffMsg handoff)
    {
        _handoff = handoff;
        try
        {
            _lobby?.Dispose();
            _lobby = null;
            _zone?.Dispose();
            _zone = new NetTransport(OnZonePacket, reason => Post(() => Debug.Log("[Net] zone " + reason)));
            string host = string.IsNullOrWhiteSpace(handoff.ZoneHost) ? FineageWireConfig.ZoneHost : handoff.ZoneHost;
            int port = handoff.ZonePort > 0 ? handoff.ZonePort : FineageWireConfig.ZonePort;
            await _zone.ConnectAsync(host, port);
            await SendHelloAsync(_zone, "unity-net-zone");
            await WaitUntil(() => _helloZone, 5f);
            await _zone.SendAsync(
                NetOpcode.ZoneAuth,
                NetCodec.EncodeZoneAuth(new ZoneAuthMsg { Account = handoff.Account, Token = handoff.Token }),
                NextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] EnterZone failed: " + ex.Message);
        }
    }

    public async void SendMove(int destX, int destY, int destZ)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.MoveInput,
            NetCodec.EncodeMoveInput(new MoveInputMsg { DestX = destX, DestY = destY, DestZ = destZ }),
            NextSeq());
    }

    async System.Threading.Tasks.Task SendHelloAsync(NetTransport transport, string clientName)
    {
        await transport.SendAsync(
            NetOpcode.Hello,
            NetCodec.EncodeHello(new HelloMsg
            {
                ClientName = clientName,
                FeatureFlags = ProtocolCapabilities.FeatureSession
                    | ProtocolCapabilities.FeatureLobby
                    | ProtocolCapabilities.FeatureWorldPresence
            }),
            NextSeq());
    }

    void OnLobbyPacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloLobby = true;
        Post(() => HandleLobby(envelope));
    }

    void OnZonePacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloZone = true;
        Post(() => HandleZone(envelope));
    }

    void HandleLobby(PacketEnvelope envelope)
    {
        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.AuthResult:
            {
                var result = NetCodec.DecodeAuthResult(envelope.Payload);
                AuthResultReceived?.Invoke(result);
                if (result.Ok)
                    RequestRoster();
                break;
            }
            case NetOpcode.CharacterRoster:
            {
                var roster = NetCodec.DecodeCharacterRoster(envelope.Payload);
                ApplyRosterUi(roster);
                RosterReceived?.Invoke(roster);
                break;
            }
            case NetOpcode.CreateResult:
            {
                var created = NetCodec.DecodeCreateResult(envelope.Payload);
                if (!created.Ok)
                {
                    Debug.LogWarning($"[Net] create failed {created.Code}: {created.Message}");
                    if (CharCreationWindow.Instance != null)
                        CharCreationWindow.Instance.SetlabelError(
                            LobbyCharacterText.ErrorMessage(created.Code, created.Message));
                    break;
                }

                CharacterSummaryNet summary = created.Character;
                int newId = summary != null ? summary.CharacterId : 0;
                Debug.Log("[Net] character created id=" + newId);
                if (CharCreationWindow.Instance != null)
                    CharCreationWindow.Instance.HideWindow();
                RequestRoster();
                break;
            }
            case NetOpcode.Handoff:
            {
                var handoff = NetCodec.DecodeHandoff(envelope.Payload);
                _handoff = handoff;
                HandoffReceived?.Invoke(handoff);
                EnterZoneWithHandoff(handoff);
                break;
            }
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] lobby reject {reject.Code}: {reject.Message}");
                break;
            }
        }
    }

    void ApplyRosterUi(CharacterRosterMsg roster)
    {
        LobbyCharacter[] characters = LobbyCharacterMap.FromNetRoster(roster);
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SetLobbyCharacters(characters);
        if (CharacterSelector.Instance != null)
            CharacterSelector.Instance.SetLobbyCharacters(characters, 0);
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SelectLobbySlot(LobbyCharacterMap.PreferredIndex(characters, 0));
        if (LoginWindow.Instance != null)
            LoginWindow.Instance.ShowErrorText("");
        if (GameManager.Instance != null)
            GameManager.Instance.OnAuthAllowed();
        Debug.Log("[Net] roster count=" + characters.Length);
    }

    void HandleZone(PacketEnvelope envelope)
    {
        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.ZoneAuthResult:
            {
                var result = NetCodec.DecodeZoneAuthResult(envelope.Payload);
                if (!result.Ok)
                {
                    Debug.LogWarning($"[Net] zone auth failed {result.Code}: {result.Message}");
                    return;
                }

                _ = EnterWorldAsync();
                break;
            }
            case NetOpcode.EntitySpawn:
                ApplyEntitySpawn(NetCodec.DecodeEntitySpawn(envelope.Payload));
                break;
            case NetOpcode.PlayerStats:
                ApplyPlayerStats(NetCodec.DecodePlayerStats(envelope.Payload));
                break;
            case NetOpcode.MoveState:
                ApplyMoveState(NetCodec.DecodeMoveState(envelope.Payload));
                break;
            case NetOpcode.PositionCorrection:
                ApplyCorrection(NetCodec.DecodePositionCorrection(envelope.Payload));
                break;
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] zone reject {reject.Code}: {reject.Message}");
                break;
            }
        }
    }

    async System.Threading.Tasks.Task EnterWorldAsync()
    {
        if (_zone == null)
            return;
        await _zone.SendAsync(NetOpcode.EnterWorld, NetCodec.EncodeEnterWorld(new EnterWorldMsg()), NextSeq());
    }

    void ApplyEntitySpawn(EntitySpawnMsg spawn)
    {
        if (!spawn.IsSelf || spawn.Kind != EntityKindNet.Player)
            return;

        var identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = (int)spawn.EntityValue,
            Name = string.IsNullOrEmpty(spawn.Name) ? "Player" : spawn.Name,
            PlayerClass = spawn.ClassId,
            Owned = true,
            IsRunning = spawn.Running
        };
        identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);

        var status = new PlayerStatus();
        status.SetHp(100);
        status.SetMp(50);

        var stats = new PlayerStats
        {
            Level = 1,
            MaxHp = 100,
            MaxMp = 50,
            MaxCp = 0,
            BaseRunSpeed = Mathf.RoundToInt(spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f),
            RunRealSpeed = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f,
            WalkRealSpeed = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f
        };

        var appearance = new PlayerAppearance
        {
            Race = spawn.Race,
            Sex = spawn.Sex,
            Running = spawn.Running,
            BaseClass = spawn.ClassId
        };

        var domain = new DomainPlayerSpawned
        {
            ObjectId = identity.Id,
            Identity = identity,
            Status = status,
            Stats = stats,
            Appearance = appearance
        };

        if (_sink != null)
            _sink.ApplyPlayerSpawned(domain);
        else
            World.Instance?.SpawnPlayer(identity, status, stats, appearance);

        // Match V2 enter: notify GameManager character select / world load.
        if (GameManager.Instance != null)
            GameManager.Instance.OnCharacterSelect();

        EnteredWorld?.Invoke();
    }

    void ApplyPlayerStats(PlayerStatsMsg msg)
    {
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local == null || local.Stats == null)
            return;

        var stats = local.Stats as PlayerStats;
        if (stats == null)
            return;

        stats.Level = msg.Level;
        stats.Exp = msg.Exp;
        stats.Sp = msg.Sp;
        stats.MaxHp = msg.MaxHp;
        stats.MaxMp = msg.MaxMp;
        stats.MaxCp = msg.MaxCp;
        stats.Cp = msg.Cp;
        if (local.Status != null)
        {
            local.Status.SetHp(msg.Hp);
            local.Status.SetMp(msg.Mp);
            if (local.Status is PlayerStatus ps)
                ps.Cp = msg.Cp;
        }
    }

    void ApplyMoveState(MoveStateMsg move)
    {
        int objectId = (int)move.EntityValue;
        Vector3 dest = VectorUtils.ConvertPosToUnity(new Vector3(move.DestX, move.DestY, move.DestZ));
        Vector3 origin = VectorUtils.ConvertPosToUnity(new Vector3(move.OriginX, move.OriginY, move.OriginZ));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = dest,
            Origin = origin,
            InterludeDto = null
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
        else if (WorldApplyPacketServices.WorldApply != null)
            WorldApplyPacketServices.WorldApply.MoveTo(objectId, dest, origin, null);
    }

    void ApplyCorrection(PositionCorrectionMsg corr)
    {
        int objectId = (int)corr.EntityValue;
        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(corr.X, corr.Y, corr.Z));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = pos,
            Origin = pos,
            InterludeDto = null
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
    }

    uint NextSeq() => _seq++;

    void Post(Action action) => _main.Enqueue(action);

    static async System.Threading.Tasks.Task WaitUntil(Func<bool> pred, float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (!pred())
        {
            if (Time.realtimeSinceStartup - start > seconds)
                throw new TimeoutException("net wait");
            await System.Threading.Tasks.Task.Delay(20);
        }
    }

    void OnDestroy()
    {
        _lobby?.Dispose();
        _zone?.Dispose();
        if (Instance == this)
            Instance = null;
    }
}
