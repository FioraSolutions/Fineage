using System;

/// <summary>
/// Login-session wire apply hub (#142 batch slice 169; locator queue orchestration slice 203).
/// </summary>
public static class LoginSessionApplyIncomingPacketServices
{
    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => LocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LocatorApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => LoginApplyIncomingPacketServices.Queue(action);
}
