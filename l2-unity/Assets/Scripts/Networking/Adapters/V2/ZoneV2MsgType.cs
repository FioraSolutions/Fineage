/// <summary>Must stay in sync with tools/zone-server Wire/MsgType.cs.</summary>
public enum ZoneV2MsgType : ushort
{
    Hello = 1,
    Auth = 2,
    EnterWorld = 3,
    MoveTo = 4,

    HelloAck = 1001,
    AuthOk = 1002,
    PlayerSpawned = 1003,
    MoveToAck = 1004,
    Error = 1099
}
