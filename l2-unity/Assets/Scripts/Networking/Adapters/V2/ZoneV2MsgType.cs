/// <summary>
/// Zone opcodes match tools/zone-server Wire/MsgType.cs.
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

    Register = 20,
    CreateCharacter = 21,
    ListCharacters = 22,
    SelectCharacter = 23,

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

    RegisterOk = 1020,
    CharacterCreated = 1021,
    CharacterList = 1022,
    Handoff = 1023,
    LoginOk = 1024,

    Error = 1099
}
