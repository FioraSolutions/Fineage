namespace Fineage.ZoneServer.Wire;

/// <summary>v2 opcodes — names align with client Domain events where possible.</summary>
public enum MsgType : ushort
{
    // C → S
    Hello = 1,
    Auth = 2,
    EnterWorld = 3,
    MoveTo = 4,

    // S → C
    HelloAck = 1001,
    AuthOk = 1002,
    PlayerSpawned = 1003,
    MoveToAck = 1004,
    CharInfo = 1005,
    NpcInfo = 1006,
    DeleteObject = 1007,
    Error = 1099
}
