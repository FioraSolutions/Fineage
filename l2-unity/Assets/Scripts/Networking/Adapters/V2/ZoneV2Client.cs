using System;
using System.Collections;
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
        NetworkingPacketServices.Bind(
            domain: _domain,
            worldApply: _worldApply,
            manager: _gameManager);
    }

    private GameManager Manager => NetworkingPacketServices.Manager;

    private IWorldDomainSink Domain => NetworkingPacketServices.Domain;

    private WorldPacketApply WorldApply => NetworkingPacketServices.WorldApply;

    readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();
    readonly object _lobbyGate = new object();
    CancellationTokenSource _cts;
    TcpClient _tcp;
    NetworkStream _stream;
    readonly object _sendLock = new object();
    FineageLobbySession _lobby;
    int _preselectId;
    int _selectGeneration;
    int _lobbyEpoch;
    volatile bool _zoneStarted;
    bool _selectUiShown;
    bool _returnToSelectOnRoster;

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
        NetworkingPacketServices.Bind(manager: _gameManager);
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

        ZoneV2GearSession.FlushPending();
        ZoneV2ChatFeed.Flush();
    }

    public void BeginSession(string account, string password)
    {
        Disconnect();
        ZoneV2ClanBook.Shared.Reset();
        ClanPacketServices.RefreshWindow();
        _preselectId = 0;
        _selectGeneration++;
        _cts = new CancellationTokenSource();
        CancellationToken ct = _cts.Token;
        if (FineageWireConfig.UseLoginHandoff)
        {
            string loginHost = FineageWireConfig.LoginHost;
            int loginPort = FineageWireConfig.LoginPort;
            string character = FineageLocalBackend.CharacterHint;
            if (!string.IsNullOrEmpty(character))
            {
                Task.Run(() => RunLoginThenZoneAsync(loginHost, loginPort, account, password, character, ct));
                return;
            }

            Task.Run(() => OpenLobbyAsync(loginHost, loginPort, account, password, ct));
            return;
        }

        string host = FineageWireConfig.ZoneHost;
        int port = FineageWireConfig.ZonePort;
        Task.Run(() => RunSessionAsync(host, port, account, password, ct));
    }

    async Task OpenLobbyAsync(string loginHost, int loginPort, string account, string password, CancellationToken ct)
    {
        int epoch = _lobbyEpoch;
        FineageLobbySession session = new FineageLobbySession(ct);
        session.Roster = rows => Post(() =>
        {
            if (epoch != _lobbyEpoch)
                return;
            ApplyLobbyRoster(rows);
        });
        session.Handoff = handoff =>
        {
            if (epoch != _lobbyEpoch)
                return;
            HandleLobbyHandoff(handoff, ct);
        };
        session.Failed = (code, message) => Post(() =>
        {
            if (epoch != _lobbyEpoch)
                return;
            ShowLobbyError(code, message);
        });
        session.Created = created => Post(() =>
        {
            if (epoch != _lobbyEpoch)
                return;
            _preselectId = created.CharacterId;
        });
        session.DeleteFailed = failed => Post(() =>
        {
            if (epoch != _lobbyEpoch)
                return;
            ShowLobbyError(failed.Code, failed.Message);
        });
        session.Lost = reason => Post(() =>
        {
            if (epoch != _lobbyEpoch)
                return;
            OnLobbyLost(reason);
        });
        lock (_lobbyGate)
            _lobby = session;

        try
        {
            await session.ConnectAsync(loginHost, loginPort, account, password).ConfigureAwait(false);
            Post(() => Debug.Log("[FineageLocal] lobby ready account=" + session.Account));
        }
        catch (OperationCanceledException)
        {
            session.Close();
        }
        catch (Exception ex)
        {
            session.Close();
            lock (_lobbyGate)
            {
                if (_lobby == session)
                    _lobby = null;
            }

            Post(() =>
            {
                Debug.LogError("[FineageLocal] login failed: " + ex.Message);
                if (LoginWindow.Instance != null)
                    LoginWindow.Instance.ShowErrorText(ex.Message);
            });
        }
    }

    void ApplyLobbyRoster(ZoneV2CharacterSummary[] rows)
    {
        LobbyCharacter[] characters = LobbyCharacterMap.FromSummaries(rows);
        int preselect = _preselectId;
        _preselectId = 0;
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SetLobbyCharacters(characters);
        if (CharacterSelector.Instance != null)
            CharacterSelector.Instance.SetLobbyCharacters(characters, preselect);
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SelectLobbySlot(LobbyCharacterMap.PreferredIndex(characters, preselect));

        bool returnFromCreate = _returnToSelectOnRoster;
        _returnToSelectOnRoster = false;
        if (_selectUiShown && !returnFromCreate)
            return;
        _selectUiShown = true;
        GameManager manager = Manager;
        if (manager != null)
            manager.OnAuthAllowed();
        else
            Debug.LogError("[FineageLocal] GameManager missing; character select was not shown.");
    }

    void HandleLobbyHandoff(ZoneV2Handoff handoff, CancellationToken ct)
    {
        if (_zoneStarted)
            return;
        _zoneStarted = true;
        _selectGeneration++;
        if (string.IsNullOrWhiteSpace(handoff.ZoneHost))
            handoff.ZoneHost = FineageWireConfig.ZoneHost;
        if (handoff.ZonePort <= 0)
            handoff.ZonePort = FineageWireConfig.ZonePort;

        string prefix = handoff.Token != null && handoff.Token.StartsWith("fa1.", StringComparison.Ordinal)
            ? "fa1."
            : "missing";
        int tokenLength = handoff.Token != null ? handoff.Token.Length : 0;
        Post(() =>
        {
            Debug.Log("[FineageLocal] handoff character=" + handoff.CharacterName
                + " zone=" + handoff.ZoneHost + ":" + handoff.ZonePort
                + " tokenPrefix=" + prefix
                + " tokenLength=" + tokenLength);
            if (CharSelectWindow.Instance != null)
                CharSelectWindow.Instance.ShowLobbyStatus("Entering the world...");
        });

        CloseLobby();
        // Zone Auth uses the handoff token. The account password is not in this call.
        Task.Run(() => RunSessionAsync(handoff.ZoneHost, handoff.ZonePort, handoff.Account, handoff.Token, ct));
    }

    void ShowLobbyError(string code, string message)
    {
        _selectGeneration++;
        string text = LobbyCharacterText.ErrorMessage(code, message);
        Debug.LogWarning("[FineageLocal] lobby " + code + ": " + message);
        if (_returnToSelectOnRoster)
        {
            _returnToSelectOnRoster = false;
            CharCreationWindow creation = CharCreationWindow.Instance;
            if (creation != null && creation.IsOpen)
            {
                creation.SetlabelError(text);
                return;
            }
        }

        if (CharSelectWindow.Instance != null)
        {
            CharSelectWindow.Instance.ShowLobbyStatus(text);
            CharSelectWindow.Instance.RefreshLobbyActions();
        }
        else if (LoginWindow.Instance != null)
            LoginWindow.Instance.ShowErrorText(text);
    }

    void OnLobbyLost(string reason)
    {
        if (_zoneStarted)
            return;
        Debug.LogWarning("[FineageLocal] lobby closed: " + reason);
        CloseLobby();
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.ShowLobbyStatus(reason);
        GameManager manager = Manager;
        if (manager != null && _selectUiShown)
            manager.OnRelogin();
        else if (LoginWindow.Instance != null)
            LoginWindow.Instance.ShowErrorText(reason);
    }

    public void SelectLobbyCharacter(LobbyCharacter character)
    {
        if (character == null || !LobbyCharacterText.CanEnter(character))
            return;
        FineageLobbySession session = Lobby;
        if (session == null)
            return;
        int generation = ++_selectGeneration;
        session.SendSelect(character.Id > 0 ? character.Id : 0, character.Name, character.Id > 0 ? -1 : character.Slot);
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000).ConfigureAwait(false);
            }
            catch
            {
                return;
            }

            Post(() =>
            {
                if (generation != _selectGeneration || _zoneStarted)
                    return;
                if (CharSelectWindow.Instance != null)
                {
                    CharSelectWindow.Instance.ShowLobbyStatus("The server did not enter the world. Try again.");
                    CharSelectWindow.Instance.RefreshLobbyActions();
                }
            });
        });
    }

    public void DeleteLobbyCharacter(LobbyCharacter character)
    {
        if (character == null)
            return;
        FineageLobbySession session = Lobby;
        if (session == null)
            return;
        if (character.Id > 0)
            session.SendDelete(-1, character.Id);
        else
            session.SendDelete(character.Slot, 0);
    }

    public void RestoreLobbyCharacter(LobbyCharacter character)
    {
        if (character == null)
            return;
        FineageLobbySession session = Lobby;
        if (session == null)
            return;
        if (character.Id > 0)
            session.SendRestore(-1, character.Id);
        else
            session.SendRestore(character.Slot, 0);
    }

    public bool CreateLobbyCharacter(string name, int classId, int race, int sex, int hairStyle, int hairColor, int face)
    {
        FineageLobbySession session = Lobby;
        if (session == null)
            return false;
        _returnToSelectOnRoster = true;
        session.SendCreate(name, classId, race, sex, hairStyle, hairColor, face);
        return true;
    }

    public void CancelLobbyCreate()
    {
        _returnToSelectOnRoster = false;
    }

    public void CloseLobby()
    {
        FineageLobbySession session;
        lock (_lobbyGate)
        {
            session = _lobby;
            _lobby = null;
        }

        if (session != null)
            session.Close();
    }

    FineageLobbySession Lobby
    {
        get
        {
            lock (_lobbyGate)
                return _lobby;
        }
    }

    async Task RunLoginThenZoneAsync(
        string loginHost,
        int loginPort,
        string account,
        string password,
        string character,
        CancellationToken ct)
    {
        ZoneV2Handoff handoff;
        try
        {
            handoff = await FineageLoginSession.LoginAndSelectAsync(
                loginHost,
                loginPort,
                account,
                password,
                character,
                message => Post(() => Debug.Log(message)),
                ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            Post(() =>
            {
                Debug.LogError("[FineageLocal] login failed: " + ex.Message);
                if (LoginWindow.Instance != null)
                    LoginWindow.Instance.ShowErrorText(ex.Message);
            });
            return;
        }

        await RunSessionAsync(handoff.ZoneHost, handoff.ZonePort, handoff.Account, handoff.Token, ct)
            .ConfigureAwait(false);
    }

    public void Disconnect()
    {
        _lobbyEpoch++;
        CloseLobby();
        _selectUiShown = false;
        _zoneStarted = false;
        _returnToSelectOnRoster = false;
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
        ZoneV2GearSession.Reset();
        ZoneV2ChatFeed.Reset();
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

    public void SendSelectTarget(int targetObjectId)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeSelectTarget(targetObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.SelectTarget, body);
    }

    public void SendAttackRequest(int targetObjectId)
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeAttackRequest(targetObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.AttackRequest, body);
    }

    public void SendGrantItemRequest(int itemId, int count)
    {
        if (!Connected || _stream == null || itemId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeGrantItemRequest(itemId, count);
        _ = SendFireAndForget(ZoneV2MsgType.GrantItemRequest, body);
    }

    public void SendPickupRequest(int groundObjectId)
    {
        if (!Connected || _stream == null || groundObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodePickupRequest(groundObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.PickupRequest, body);
    }

    public void SendUseItem(int objectId)
    {
        if (!Connected || _stream == null || objectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeUseItem(objectId);
        _ = SendFireAndForget(ZoneV2MsgType.UseItem, body);
    }

    public void SendUnequipItem(int paperdollSlot)
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeUnequipItem(paperdollSlot);
        _ = SendFireAndForget(ZoneV2MsgType.UnequipItem, body);
    }

    public void SendGiveItem(int itemId, int count)
    {
        if (!Connected || _stream == null || itemId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeGiveItem(itemId, count);
        _ = SendFireAndForget(ZoneV2MsgType.GiveItem, body);
    }

    public void SendPartyInviteRequest(int targetObjectId, int itemDistribution = 0)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodePartyInviteRequest(targetObjectId, itemDistribution);
        _ = SendFireAndForget(ZoneV2MsgType.PartyInviteRequest, body);
    }

    public void SendPartyAnswerJoinRequest(int requestorObjectId, int response)
    {
        if (!Connected || _stream == null || requestorObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodePartyAnswerJoinRequest(requestorObjectId, response);
        _ = SendFireAndForget(ZoneV2MsgType.PartyAnswerJoinRequest, body);
    }

    public void SendPartyLeaveRequest()
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodePartyLeaveRequest();
        _ = SendFireAndForget(ZoneV2MsgType.PartyLeaveRequest, body);
    }

    public void SendPartyKickRequest(int targetObjectId)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodePartyKickRequest(targetObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.PartyKickRequest, body);
    }

    public void SendClanCreateRequest(string name)
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeClanCreateRequest(name);
        _ = SendFireAndForget(ZoneV2MsgType.ClanCreateRequest, body);
    }

    public void SendClanInviteRequest(int targetObjectId, int pledgeType)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeClanInviteRequest(targetObjectId, pledgeType);
        _ = SendFireAndForget(ZoneV2MsgType.ClanInviteRequest, body);
    }

    public void SendClanAnswerJoinRequest(int requestorObjectId, int response)
    {
        if (!Connected || _stream == null || requestorObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeClanAnswerJoinRequest(requestorObjectId, response);
        _ = SendFireAndForget(ZoneV2MsgType.ClanAnswerJoinRequest, body);
    }

    public void SendClanLeaveRequest()
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeClanLeaveRequest();
        _ = SendFireAndForget(ZoneV2MsgType.ClanLeaveRequest, body);
    }

    public void SendClanKickRequest(int targetObjectId)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeClanKickRequest(targetObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.ClanKickRequest, body);
    }

    public void SendClanDissolveRequest()
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeClanDissolveRequest();
        _ = SendFireAndForget(ZoneV2MsgType.ClanDissolveRequest, body);
    }

    public void SendClanCancelDissolveRequest()
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeClanCancelDissolveRequest();
        _ = SendFireAndForget(ZoneV2MsgType.ClanCancelDissolveRequest, body);
    }

    public void SendEnchantRequest(int scrollObjectId)
    {
        if (!Connected || _stream == null || scrollObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeEnchantRequest(scrollObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.EnchantRequest, body);
    }

    public void SendEnchantConfirm(int targetObjectId)
    {
        if (!Connected || _stream == null || targetObjectId <= 0)
            return;
        byte[] body = ZoneV2Messages.EncodeEnchantConfirm(targetObjectId);
        _ = SendFireAndForget(ZoneV2MsgType.EnchantConfirm, body);
    }

    public void SendEnchantCancel()
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeEnchantCancel();
        _ = SendFireAndForget(ZoneV2MsgType.EnchantCancel, body);
    }

    public void SendSay(string text, int chatType)
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeSay(text ?? "", chatType);
        _ = SendFireAndForget(ZoneV2MsgType.Say, body);
    }

    public void SendSkillUseRequest(int skillId, int targetObjectId, bool ctrlPressed)
    {
        if (!Connected || _stream == null)
            return;
        byte[] body = ZoneV2Messages.EncodeSkillUseRequest(skillId, targetObjectId, ctrlPressed);
        _ = SendFireAndForget(ZoneV2MsgType.SkillUseRequest, body);
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
                        bool handoffToken = password != null && password.StartsWith("fa1.", StringComparison.Ordinal);
                        string credential = handoffToken ? "handoff-token" : "password";
                        Post(() => Debug.Log("[ZoneV2] Auth account=" + account + " credential=" + credential));
                        byte[] authBody = handoffToken
                            ? ZoneV2Messages.EncodeZoneAuth(new ZoneV2Handoff { Account = account, Token = password })
                            : ZoneV2Messages.EncodeAuth(account, password);
                        await ZoneV2FrameIO.SendAsync(stream, ZoneV2MsgType.Auth, authBody, ct)
                            .ConfigureAwait(false);
                        break;

                    case ZoneV2MsgType.AuthOk:
                        Post(() => Debug.Log("[ZoneV2] EnterWorld " + host + ":" + port));
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
                        int objectId = spawned.ObjectId;
                        int spawnX = spawned.X;
                        int spawnY = spawned.Y;
                        int spawnZ = spawned.Z;
                        string spawnName = spawned.Name;
                        Post(() =>
                        {
                            Debug.Log("[ZoneV2] PlayerSpawned name=" + spawnName
                                + " objectId=" + objectId
                                + " x=" + spawnX
                                + " y=" + spawnY
                                + " z=" + spawnZ);
                            OnPlayerSpawnedMain(ZoneV2GearSession.PrepareSpawn(spawned));
                        });
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
                                    NetworkingPacketServices.ApplyWorld(a =>
                                        a.MoveTo(domainMove.ObjectId, domainMove.Destination, domainMove.Origin, domainMove.InterludeDto));
                            }
                        });
                        break;

                    case ZoneV2MsgType.CharInfo:
                        ZoneV2CharInfo charInfo;
                        if (!ZoneV2Messages.TryDecodeCharInfo(f.Body, out charInfo))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad CharInfo body"));
                            break;
                        }

                        CharInfoDto charDto = ZoneV2DomainMap.ToCharInfoDto(charInfo);
                        Post(() => CharInfoApplyIncomingPacketServices.ApplyCharInfo(charDto));
                        break;

                    case ZoneV2MsgType.NpcInfo:
                        ZoneV2NpcInfo npcInfo;
                        if (!ZoneV2Messages.TryDecodeNpcInfo(f.Body, out npcInfo))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad NpcInfo body"));
                            break;
                        }

                        NpcInfoDto npcDto = ZoneV2DomainMap.ToNpcInfoDto(npcInfo);
                        Post(() => NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(npcDto));
                        break;

                    case ZoneV2MsgType.DeleteObject:
                        int deleteId;
                        if (!ZoneV2Messages.TryDecodeDeleteObject(f.Body, out deleteId))
                            break;
                        DeleteObjectDto deleteDto = new DeleteObjectDto { ObjectId = deleteId };
                        Post(() => DeleteObjectGroundApplyIncomingPacketServices.ApplyDeleteObject(deleteDto));
                        break;

                    case ZoneV2MsgType.SpawnItem:
                        ZoneV2SpawnItem ground;
                        if (!ZoneV2Messages.TryDecodeSpawnItem(f.Body, out ground))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad SpawnItem body"));
                            break;
                        }

                        SpawnItemDto spawnDto = ZoneV2DomainMap.ToSpawnItemDto(ground);
                        Post(() => SpawnItemApplyIncomingPacketServices.ApplySpawnItem(spawnDto));
                        break;

                    case ZoneV2MsgType.MyTargetSelected:
                        ZoneV2MyTargetSelected myTarget;
                        if (!ZoneV2Messages.TryDecodeMyTargetSelected(f.Body, out myTarget))
                            break;
                        Post(() =>
                        {
                            MyTargetSelectDto targetDto = MyTargetSelectDto.FromZoneV2(myTarget.ObjectId, myTarget.ColorHint);
                            MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(targetDto);
                        });
                        break;

                    case ZoneV2MsgType.Attack:
                        ZoneV2Attack attack;
                        if (!ZoneV2Messages.TryDecodeAttack(f.Body, out attack))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad Attack body"));
                            break;
                        }

                        AttackDto attackDto = ZoneV2DomainMap.ToAttackDto(attack);
                        Post(() => AttackApplyIncomingPacketServices.ApplyAttack(attackDto));
                        break;

                    case ZoneV2MsgType.StatusUpdate:
                        ZoneV2StatusUpdate status;
                        if (!ZoneV2Messages.TryDecodeStatusUpdate(f.Body, out status))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad StatusUpdate body"));
                            break;
                        }

                        Post(() =>
                        {
                            var attributes = ZoneV2DomainMap.ToStatusAttributes(status);
                            StatusUpdateApplyIncomingPacketServices.ApplyStatusUpdate(status.ObjectId, attributes);
                        });
                        break;

                    case ZoneV2MsgType.ItemList:
                        ZoneV2ItemEntry[] itemRows;
                        if (!ZoneV2Messages.TryDecodeItemList(f.Body, out itemRows))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad ItemList body"));
                            break;
                        }

                        Post(() => ZoneV2GearSession.NoteItemList(itemRows));
                        break;

                    case ZoneV2MsgType.InventoryUpdate:
                        ZoneV2InventoryChange[] invChanges;
                        if (!ZoneV2Messages.TryDecodeInventoryUpdate(f.Body, out invChanges))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad InventoryUpdate body"));
                            break;
                        }

                        Post(() => ZoneV2GearSession.NoteInventoryUpdate(invChanges));
                        break;

                    case ZoneV2MsgType.UserInfo:
                        ZoneV2UserInfo userInfo;
                        if (!ZoneV2Messages.TryDecodeUserInfo(f.Body, out userInfo))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad UserInfo body"));
                            break;
                        }

                        Post(() => ZoneV2GearSession.NoteUserInfo(userInfo));
                        break;

                    case ZoneV2MsgType.SkillList:
                        ZoneV2SkillEntry[] skillRows;
                        if (!ZoneV2Messages.TryDecodeSkillList(f.Body, out skillRows))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad SkillList body"));
                            break;
                        }

                        Post(() =>
                        {
                            SkillListDto list = ZoneV2DomainMap.ToSkillListDto(skillRows);
                            SkillListApplyIncomingPacketServices.ApplySkillList(list);
                        });
                        break;

                    case ZoneV2MsgType.MagicSkillUse:
                        ZoneV2MagicSkillUse magicUse;
                        if (!ZoneV2Messages.TryDecodeMagicSkillUse(f.Body, out magicUse))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad MagicSkillUse body"));
                            break;
                        }

                        Post(() =>
                        {
                            MagicSkillUseDto dto = ZoneV2DomainMap.ToMagicSkillUseDto(magicUse);
                            MagicSkillUseApplyIncomingPacketServices.ApplyMagicSkillUse(dto);
                        });
                        break;

                    case ZoneV2MsgType.AbnormalStatusUpdate:
                        ZoneV2AbnormalStatusUpdate abnormal;
                        if (!ZoneV2Messages.TryDecodeAbnormalStatusUpdate(f.Body, out abnormal))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad AbnormalStatusUpdate body"));
                            break;
                        }

                        Post(() =>
                        {
                            AbnormalStatusUpdateDto dto = ZoneV2DomainMap.ToAbnormalStatusUpdateDto(abnormal);
                            AbnormalStatusUpdateApplyIncomingPacketServices.ApplyAbnormalStatusUpdate(dto);
                        });
                        break;

                    case ZoneV2MsgType.PartyAskJoin:
                        ZoneV2PartyAskJoin partyAsk;
                        if (!ZoneV2Messages.TryDecodePartyAskJoin(f.Body, out partyAsk))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad PartyAskJoin body"));
                            break;
                        }

                        Post(() =>
                        {
                            AskJoinPartyDto dto = ZoneV2DomainMap.ToAskJoinPartyDto(partyAsk);
                            AskJoinPartyApplyIncomingPacketServices.ApplyAskJoinParty(dto);
                        });
                        break;

                    case ZoneV2MsgType.PartyJoinResult:
                        int joinResult;
                        if (!ZoneV2Messages.TryDecodePartyJoinResult(f.Body, out joinResult))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad PartyJoinResult body"));
                            break;
                        }

                        Post(() => PartyJoinResultApplyIncomingPacketServices.ApplyPartyJoinResult(joinResult));
                        break;

                    case ZoneV2MsgType.PartyMemberList:
                        ZoneV2PartyMemberList partyList;
                        if (!ZoneV2Messages.TryDecodePartyMemberList(f.Body, out partyList))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad PartyMemberList body"));
                            break;
                        }

                        Post(() => PartyMemberListApplyIncomingPacketServices.ApplyPartyMemberList(partyList));
                        break;

                    case ZoneV2MsgType.ClanAskJoin:
                        ZoneV2ClanAskJoin clanAsk;
                        if (!ZoneV2Messages.TryDecodeClanAskJoin(f.Body, out clanAsk))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad ClanAskJoin body"));
                            break;
                        }

                        ZoneV2ClanAskJoin clanAskCopy = clanAsk;
                        Post(() => ClanPacketServices.ApplyAskJoin(clanAskCopy));
                        break;

                    case ZoneV2MsgType.PledgeStatusChanged:
                        ZoneV2PledgeStatus pledgeStatus;
                        if (!ZoneV2Messages.TryDecodePledgeStatusChanged(f.Body, out pledgeStatus))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad PledgeStatusChanged body"));
                            break;
                        }

                        ZoneV2PledgeStatus pledgeStatusCopy = pledgeStatus;
                        Post(() => ClanPacketServices.ApplyStatus(pledgeStatusCopy));
                        break;

                    case ZoneV2MsgType.PledgeMemberList:
                        ZoneV2PledgeMemberList pledgeMembers;
                        if (!ZoneV2Messages.TryDecodePledgeMemberList(f.Body, out pledgeMembers))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad PledgeMemberList body"));
                            break;
                        }

                        ZoneV2PledgeMemberList pledgeMembersCopy = pledgeMembers;
                        Post(() => ClanPacketServices.ApplyMembers(pledgeMembersCopy));
                        break;

                    case ZoneV2MsgType.ClanJoinResult:
                        int clanJoin;
                        if (!ZoneV2Messages.TryDecodeClanJoinResult(f.Body, out clanJoin))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad ClanJoinResult body"));
                            break;
                        }

                        int clanJoinCopy = clanJoin;
                        Post(() => Debug.Log("[ZoneV2] ClanJoinResult response=" + clanJoinCopy));
                        break;

                    case ZoneV2MsgType.CreatureSay:
                        ZoneV2CreatureSay said;
                        if (!ZoneV2Messages.TryDecodeCreatureSay(f.Body, out said))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad CreatureSay body"));
                            break;
                        }

                        ZoneV2CreatureSay saidCopy = said;
                        Post(() =>
                        {
                            Debug.Log("[ZoneV2] CreatureSay type=" + saidCopy.ChatType + " name=" + saidCopy.Name);
                            ZoneV2ChatFeed.NoteSay(saidCopy.Name, saidCopy.Text, saidCopy.ChatType);
                        });
                        break;

                    case ZoneV2MsgType.SystemMessage:
                        ZoneV2SystemMessage system;
                        if (!ZoneV2Messages.TryDecodeSystemMessage(f.Body, out system))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad SystemMessage body"));
                            break;
                        }

                        ZoneV2SystemMessage systemCopy = system;
                        Post(() =>
                        {
                            Debug.Log("[ZoneV2] SystemMessage id=" + systemCopy.Id + " text=" + systemCopy.Text);
                            ZoneV2ChatFeed.NoteSystem(systemCopy.Text);
                        });
                        break;

                    case ZoneV2MsgType.EnchantResult:
                        ZoneV2EnchantResult enchantResult;
                        if (!ZoneV2Messages.TryDecodeEnchantResult(f.Body, out enchantResult))
                        {
                            Post(() => Debug.LogWarning("[ZoneV2] bad EnchantResult body"));
                            break;
                        }

                        ZoneV2EnchantResult enchantCopy = enchantResult;
                        Post(() =>
                        {
                            Debug.Log("[ZoneV2] EnchantResult result=" + enchantCopy.Result
                                + " level=" + enchantCopy.EnchantLevel
                                + " object=" + enchantCopy.ObjectId);
                            EnchantWindow window = EnchantWindow.Instance;
                            if (window != null)
                                window.ApplyZoneResult(enchantCopy);
                        });
                        break;

                    case ZoneV2MsgType.Error:
                        string code, msg;
                        if (!ZoneV2Messages.TryDecodeError(f.Body, out code, out msg))
                        {
                            Post(() => ShowServerLine(ZoneV2EquipNotices.Line("", "")));
                            break;
                        }

                        string clanLine;
                        bool clan = ZoneV2ClanNotices.TryLine(code, msg, out clanLine);
                        string skillLine;
                        bool skill = !clan && ZoneV2SkillNotices.TryLine(code, msg, out skillLine);
                        string line = clan ? clanLine : skill ? skillLine : ZoneV2Enchant.Notice(code, msg);
                        bool closeEnchant = !clan && ZoneV2Enchant.ClearsWindow(code);
                        Post(() =>
                        {
                            ShowServerLine(line);
                            if (!closeEnchant)
                                return;
                            EnchantWindow window = EnchantWindow.Instance;
                            if (window != null)
                                window.AbortZone();
                        });
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

    static void ShowServerLine(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        Debug.LogWarning("[ZoneV2] " + text);
        ZoneV2ChatFeed.NoteSystem(text);
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
        if (FineageLocalBackend.QuitAfterSpawn)
            StartCoroutine(QuitAfterSpawnRoutine());
    }

    IEnumerator QuitAfterSpawnRoutine()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("[FineageLocal] quit after spawn");
        Application.Quit();
    }

    void Post(Action action)
    {
        if (action != null)
            _main.Enqueue(action);
    }
}
