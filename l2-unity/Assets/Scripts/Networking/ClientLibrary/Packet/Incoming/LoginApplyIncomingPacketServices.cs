using System;

/// <summary>
/// Login / char-select locator hub (#142 batch slice 168; bind facade slice 174–175).
/// </summary>
public static class LoginApplyIncomingPacketServices
{
    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => LocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LocatorApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
