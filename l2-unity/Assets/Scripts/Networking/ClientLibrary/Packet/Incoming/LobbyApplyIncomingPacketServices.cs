using System;

/// <summary>
/// Login / char-select lobby apply hub (#142 batch slice 162; login session orchestration slice 193).
/// </summary>
public static class LobbyApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => LoginApplyIncomingPacketServices.Queue(action);
}
