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
    GrantItemRequest = 13,
    /// <summary>Pick up a visible ground item. Backend <c>PickupRequestMsg</c> key 0 objectId.</summary>
    PickupRequest = 14,
    /// <summary>Equip a bag item, or unequip it when it is already worn. Backend <c>UseItemMsg</c> key 0 objectId.</summary>
    UseItem = 15,
    /// <summary>Clear one paperdoll index (0..16). Backend <c>UnequipItemMsg</c> key 0 paperdollSlot.</summary>
    UnequipItem = 16,
    /// <summary>Admin bag grant. Backend <c>GiveItemMsg</c> keys 0 itemId, 1 count. The zone rejects it unless admin commands are on.</summary>
    GiveItem = 17,

    Register = 20,
    CreateCharacter = 21,
    ListCharacters = 22,
    SelectCharacter = 23,
    /// <summary>Lobby delete. Do not reuse zone CharInfo/NpcInfo/DeleteObject (1005–1007) or V2 lobby 1030–1033.</summary>
    DeleteCharacter = 24,
    RestoreCharacter = 25,

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
    /// <summary>Zone chat fan-out (see also legacy Interlude CreatureSay).</summary>
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
    /// <summary>Always followed by a fresh <see cref="CharacterList"/>.</summary>
    CharacterDeleted = 1025,
    /// <summary>Delete rejected. The roster is not resent.</summary>
    CharacterDeleteFailed = 1026,

    Error = 1099
}
