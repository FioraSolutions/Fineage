using System;

/// <summary>
/// Login / char-select locator hub (#142 batch slice 168).
/// </summary>
public static class LoginApplyIncomingPacketServices
{
    public static LoginClient Login => IncomingPacketLocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => IncomingPacketLocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => IncomingPacketLocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => IncomingPacketLocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => IncomingPacketLocatorApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
