using System;

/// <summary>
/// Parsed login wire apply hub (#142 batch slice 169; login hub slice 177).
/// </summary>
public static class LoginParsedApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);
}
