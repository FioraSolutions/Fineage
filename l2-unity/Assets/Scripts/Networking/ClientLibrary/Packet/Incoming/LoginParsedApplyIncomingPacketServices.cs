using System;

/// <summary>
/// Parsed login wire apply hub (#142 batch slice 169).
/// </summary>
public static class LoginParsedApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;
}
