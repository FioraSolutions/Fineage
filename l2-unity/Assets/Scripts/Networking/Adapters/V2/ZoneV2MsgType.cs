/// <summary>
/// Zone opcodes match Fineage-Backend <c>Fineage.Protocol.MsgType</c>
/// (and legacy tools/zone-server where ids overlap).
/// Login opcodes match Fineage-Backend Fineage.Protocol.MsgType.
/// </summary>
public enum ZoneV2MsgType : ushort
{
    Hello = 1,
    Auth = 2,
    EnterWorld = 3,
    MoveTo = 4,
    SelectTarget = 5,
    AttackRequest = 6,
    Say = 8,
    GrantItemRequest = 13,
    /// <summary>Pick up a visible ground item. Backend <c>PickupRequestMsg</c> key 0 objectId.</summary>
    PickupRequest = 14,
    /// <summary>Equip bag item / toggle worn. Backend <c>UseItemMsg</c> (client TX wired on inventory D2).</summary>
    UseItem = 15,
    /// <summary>Clear paperdoll slot. Backend <c>UnequipItemMsg</c> (client TX wired on inventory D2).</summary>
    UnequipItem = 16,
    /// <summary>Admin bag grant. Backend <c>GiveItemMsg</c> (client TX wired on inventory D2).</summary>
    GiveItem = 17,
    /// <summary>Party invite. Backend <c>PartyInviteRequestMsg</c>.</summary>
    PartyInviteRequest = 18,
    /// <summary>Accept or decline pending invite. Backend <c>PartyAnswerJoinRequestMsg</c>.</summary>
    PartyAnswerJoinRequest = 19,

    Register = 20,
    CreateCharacter = 21,
    ListCharacters = 22,
    SelectCharacter = 23,
    /// <summary>Leave current party. Backend <c>PartyLeaveRequestMsg</c> (empty).</summary>
    PartyLeaveRequest = 26,
    /// <summary>Leader kick. Backend <c>PartyKickRequestMsg</c> key 0 targetObjectId.</summary>
    PartyKickRequest = 27,
    /// <summary>Use a known skill. Backend <c>SkillUseRequestMsg</c> keys 0 skillId, 1 targetObjectId, 2 ctrl.</summary>
    SkillUseRequest = 50,
    /// <summary>Respawn after death. Backend <c>ReviveRequestMsg</c> key 0 action (<see cref="ZoneV2Death.ReviveToVillage"/>).</summary>
    ReviveRequest = 52,
    /// <summary>Admin kill (empty body). Backend <c>AdminKillPlayerMsg</c>.</summary>
    AdminKillPlayer = 53,

    HelloAck = 1001,
    AuthOk = 1002,
    PlayerSpawned = 1003,
    MoveToAck = 1004,
    CharInfo = 1005,
    NpcInfo = 1006,
    DeleteObject = 1007,
    MyTargetSelected = 1008,
    Attack = 1009,
    StatusUpdate = 1010,
    CreatureSay = 1012,
    Pong = 1013,
    ItemList = 1014,
    InventoryUpdate = 1015,
    UserInfo = 1016,
    AbnormalStatusUpdate = 1017,
    /// <summary>Ground item in AOI. Keys match Backend <c>SpawnItemMsg</c> / Interlude <c>SpawnItemDto</c>.</summary>
    SpawnItem = 1018,

    RegisterOk = 1020,
    CharacterCreated = 1021,
    CharacterList = 1022,
    Handoff = 1023,
    LoginOk = 1024,
    /// <summary>Party invite prompt. Backend <c>PartyAskJoinMsg</c>.</summary>
    PartyAskJoin = 1027,
    /// <summary>Invite result to inviter. Backend <c>PartyJoinResultMsg</c>.</summary>
    PartyJoinResult = 1028,
    /// <summary>Full party roster snapshot. Backend <c>PartyMemberListMsg</c>.</summary>
    PartyMemberList = 1029,
    /// <summary>Known skills after <see cref="ItemList"/> on enter. Backend <c>SkillListMsg</c>.</summary>
    SkillList = 1065,
    /// <summary>Skill cast fan-out. Backend <c>MagicSkillUseMsg</c>.</summary>
    MagicSkillUse = 1066,
    /// <summary>Creature died. Backend <c>DieMsg</c>. See <c>DeathProtocol</c>.</summary>
    Die = 1070,
    /// <summary>Creature revived. Backend <c>ReviveMsg</c>.</summary>
    Revive = 1071,
    /// <summary>Instant relocation (respawn, …). Backend <c>TeleportToLocationMsg</c>.</summary>
    TeleportToLocation = 1072,

    Error = 1099
}
