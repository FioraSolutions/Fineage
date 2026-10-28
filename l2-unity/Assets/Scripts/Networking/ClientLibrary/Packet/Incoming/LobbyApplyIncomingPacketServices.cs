using System;

/// <summary>
/// Login / char-select lobby apply hub (#142 batch slice 162; locator backing slice 165).
/// </summary>
public static class LobbyApplyIncomingPacketServices
{
    public static LoginClient Login => IncomingPacketLocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => IncomingPacketLocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => IncomingPacketLocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => IncomingPacketLocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => IncomingPacketLocatorApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => CharacterApplyIncomingPacketServices.Queue(action);
}
