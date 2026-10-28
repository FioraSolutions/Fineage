using System;

/// <summary>
/// Login / char-select lobby apply hub (#142 batch slice 162).
/// </summary>
public static class LobbyApplyIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginPacketServices.Characters;

    public static void Queue(Action action) => CharacterApplyIncomingPacketServices.Queue(action);
}
