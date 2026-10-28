using System;

/// <summary>
/// Login-session wire apply hub (#142 batch slice 169).
/// </summary>
public static class LoginSessionApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => LoginApplyIncomingPacketServices.Queue(action);
}
