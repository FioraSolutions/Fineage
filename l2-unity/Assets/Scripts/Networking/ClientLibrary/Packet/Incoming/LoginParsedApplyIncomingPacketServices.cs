using System;

/// <summary>
/// Parsed login wire apply hub (#142 batch slice 169; login hub slice 177).
/// </summary>
public static class LoginParsedApplyIncomingPacketServices
{
    public static LoginClient Login => LobbyApplyIncomingPacketServices.Login;

    public static GameClient Game => LobbyApplyIncomingPacketServices.Game;

    public static void Queue(Action action) => LobbyApplyIncomingPacketServices.Queue(action);
}
