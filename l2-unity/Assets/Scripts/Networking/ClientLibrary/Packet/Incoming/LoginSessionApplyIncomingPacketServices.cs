using System;

/// <summary>
/// Login-session wire apply hub (#142 batch slice 169; locator queue orchestration slice 203; thin slice 257).
/// Char-select locators live on per-packet facades via <see cref="LocatorApplyIncomingPacketServices"/> (slice 255+).
/// </summary>
public static class LoginSessionApplyIncomingPacketServices
{
    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => LocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LocatorApplyIncomingPacketServices.LoginWindow;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);
}
