using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Concurrent;

public sealed partial class NetSession : MonoBehaviour
{
    /// <summary>Max zone packet applies per frame excluding critical queue traffic.</summary>
    public const int MaxNormalMainThreadActionsPerFrame = NetMainThreadPump.MaxNormalActionsPerFrame;

    readonly NetMainThreadPump _mainThread = new NetMainThreadPump();
    NetTransport _zone;
    NetCommandSender _commandSender;
    NetLobbyFlow _lobbyFlow;
    NetZoneDispatch _zoneDispatch;
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

}
