using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Fineage Net session: lobby + zone over PacketEnvelope. Maps inbound events to
/// <see cref="IWorldDomainSink"/> via NetEntity/Gear map adapters and inline DTO construction.
/// </summary>
public sealed partial class NetSession : MonoBehaviour
{

    /// <summary>
    /// Socket thread enqueues; <see cref="Update"/> drains with a per-frame cap on normal work.
    /// Critical queue (despawn, inventory) is drained first and is not budget-limited.
    /// </summary>
    readonly ConcurrentQueue<Action> _mainCritical = new ConcurrentQueue<Action>();
    readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();

    /// <summary>Max zone packet applies per frame excluding critical queue traffic.</summary>
    public const int MaxNormalMainThreadActionsPerFrame = 96;

    const int MainThreadQueueDepthWarnThreshold = 256;
    const float MainThreadQueueDepthLogCooldownSeconds = 5f;

    float _nextMainQueueDepthLogTime;
    NetTransport _zone;
    NetCommandSender _commandSender;
    NetLobbyFlow _lobbyFlow;
    uint _seq = 1;
    IWorldDomainSink _sink;
    CombatAttackApplyService _combatAttack;
    CombatDieApplyService _combatDie;
    CombatTargetApplyService _combatTarget;
    StatusUpdateApplyService _statusUpdate;
    DeleteObjectGroundApplyService _deleteObjectGround;
    StatusApplyService _status;
    SkillApplyService _skills;
    PresenceApplyService _presence;
    InventoryApplyService _inventoryApply;
    PartyApplyService _party;
    TradeApplyService _trade;
    TradeRequestApplyService _tradeRequest;
    bool _helloZone;

    public bool HasHandoff => _lobbyFlow != null && _lobbyFlow.HasHandoff;
    public HandoffMsg LastHandoff => _lobbyFlow?.LastHandoff;

    /// <summary>
    /// Stable lobby character id of the selected character, captured from the handoff in
    /// <see cref="EnterZoneWithHandoff"/>. Unlike <see cref="SessionObjectId"/> it does not
    /// change on every EnterWorld, so callers keyed by character (shortcuts, preferences)
    /// prefer it. Zero until a character has been selected, and reset in <see cref="BeginLobby"/>.
    /// </summary>
    public int CharacterId { get; private set; }

    /// <summary>
    /// Zone object id of the local player, captured from the self <c>EntitySpawn</c>
    /// received after EnterWorld. Zero until the world spawn arrives, and reset when a
    /// new lobby or zone session begins. Mirrors the old V2 session id so callers such as
    /// <c>PlayerShortcuts</c> and <c>ClanWindow</c> read one source.
    /// </summary>
    public int SessionObjectId { get; private set; }

    /// <summary>
    /// Ground item object id from the last <see cref="SendPickupRequest"/>, so
    /// <c>EntityDespawn</c> can play the pickup gesture instead of a silent delete.
    /// </summary>
    int _pendingPickupObjectId;

    public event Action<CharacterRosterMsg> RosterReceived;
    public event Action<DeleteResultMsg> DeleteResultReceived;
    public event Action<AuthResultMsg> AuthResultReceived;
    public event Action<HandoffMsg> HandoffReceived;
    public event Action EnteredWorld;

    static NetSession _instance;

    [System.Obsolete("Use NetworkingPacketServices.Session instead")]
    public static NetSession Instance
    {
        get => _instance;
        private set => _instance = value;
    }

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
        _commandSender = new NetCommandSender(() => _zone, NextSeq);
        _lobbyFlow = new NetLobbyFlow(
            NextSeq,
            Post,
            ResetLobbySessionState,
            characterId => CharacterId = characterId,
            () => SessionObjectId = 0,
            ConnectZoneForHandoffAsync,
            result => AuthResultReceived?.Invoke(result),
            roster => RosterReceived?.Invoke(roster),
            deleted => DeleteResultReceived?.Invoke(deleted),
            handoff => HandoffReceived?.Invoke(handoff));
    }

    void ResetLobbySessionState()
    {
        CharacterId = 0;
        SessionObjectId = 0;
        NetGearSession.Reset();
        NetShortCutReceiver.Reset();
        NetAutoShot.Reset();
    }

    void Update()
    {
        DrainMainThreadQueue(_mainCritical, int.MaxValue);
        DrainMainThreadQueue(_main, MaxNormalMainThreadActionsPerFrame);
        MaybeWarnMainThreadQueueDepth();

        // ItemList may arrive before the local player / inventory UI exist (same as the legacy bridge).
        NetGearSession.FlushPending();
        // Same for the hotbar: ShortCutInit lands during EnterWorld, before the skillbar.
        NetShortCutReceiver.FlushPending();
    }

    public void BindDomainSink(IWorldDomainSink sink) => _sink = sink;

    /// <summary>
    /// Packet applies are container-constructed (#142 slice 3). NetSession is created by
    /// <see cref="Ensure"/> outside the VContainer scope, so the composition root calls
    /// <see cref="VContainer.IObjectResolver.InjectGameObject"/> on it — which fills the
    /// <c>[Inject]</c> members below. Each is null until then, and every call site keeps the
    /// legacy static facade as a fallback for that window.
    /// </summary>
    [VContainer.Inject]
    public void ConstructApplyServices(
        CombatAttackApplyService combatAttack,
        CombatDieApplyService combatDie,
        CombatTargetApplyService combatTarget,
        StatusUpdateApplyService statusUpdate,
        DeleteObjectGroundApplyService deleteObjectGround,
        StatusApplyService status,
        SkillApplyService skills,
        PresenceApplyService presence,
        InventoryApplyService inventory,
        PartyApplyService party,
        TradeApplyService trade,
        TradeRequestApplyService tradeRequest,
        IWorldDomainSink domain)
    {
        _combatAttack = combatAttack;
        _combatDie = combatDie;
        _combatTarget = combatTarget;
        _statusUpdate = statusUpdate;
        _deleteObjectGround = deleteObjectGround;
        _status = status;
        _skills = skills;
        _presence = presence;
        _inventoryApply = inventory;
        _party = party;
        _trade = trade;
        _tradeRequest = tradeRequest;
        // The sink used to be injected into the static facades only, so this seam never ran.
        // Wire it here as well: with a sink bound, NetSession applies domain moves directly.
        if (domain != null)
            BindDomainSink(domain);
    }

    public Task BeginLobby(string account, string password, bool registerIfMissing = false) =>
        _lobbyFlow.BeginLobby(account, password, registerIfMissing);

    public Task RequestRoster() => _lobbyFlow.RequestRoster();

    public Task SelectCharacter(int characterId, string name = "", int slot = -1) =>
        _lobbyFlow.SelectCharacter(characterId, name, slot);

    public Task CreateCharacter(
        string name,
        int classId,
        int race,
        int sex,
        int hairStyle = 0,
        int hairColor = 0,
        int face = 0) =>
        _lobbyFlow.CreateCharacter(name, classId, race, sex, hairStyle, hairColor, face);

    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>
    public Task DeleteCharacter(int characterId, int slot = -1) =>
        _lobbyFlow.DeleteCharacter(characterId, slot);

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>
    public Task RestoreCharacter(int characterId, int slot = -1) =>
        _lobbyFlow.RestoreCharacter(characterId, slot);

    public Task EnterZoneWithHandoff(HandoffMsg handoff) =>
        _lobbyFlow.EnterZoneWithHandoff(handoff);


    void OnZonePacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloZone = true;
        if (IsCriticalZoneOpcode(envelope.Opcode))
            PostCritical(() => HandleZone(envelope));
        else
            Post(() => HandleZone(envelope));
    }

    static bool IsCriticalZoneOpcode(ushort opcode) =>
        opcode == NetOpcode.EntityDespawn
        || opcode == NetOpcode.InventoryUpdate
        || opcode == NetOpcode.ItemList
        || opcode == NetOpcode.EntityDie
        || opcode == NetOpcode.DropItem
        || opcode == NetOpcode.Revive
        || opcode == NetOpcode.TeleportToLocation;

    uint NextSeq() => _seq++;

    void Post(Action action) => _main.Enqueue(action);

    void PostCritical(Action action) => _mainCritical.Enqueue(action);

    void DrainMainThreadQueue(ConcurrentQueue<Action> queue, int maxActions)
    {
        int remaining = maxActions;
        while (remaining > 0 && queue.TryDequeue(out Action action))
        {
            remaining--;
            try { action(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }

    void MaybeWarnMainThreadQueueDepth()
    {
        int depth = _main.Count + _mainCritical.Count;
        if (depth < MainThreadQueueDepthWarnThreshold)
            return;

        float now = Time.unscaledTime;
        if (now < _nextMainQueueDepthLogTime)
            return;

        _nextMainQueueDepthLogTime = now + MainThreadQueueDepthLogCooldownSeconds;
        Debug.LogWarning(
            $"[Net] main-thread packet queue depth {depth} (critical {_mainCritical.Count}, normal {_main.Count}); "
            + $"normal applies capped at {MaxNormalMainThreadActionsPerFrame}/frame.");
    }

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
        _lobbyFlow?.Dispose();
        _zone?.Dispose();
        if (Instance == this)
            Instance = null;
    }
}
