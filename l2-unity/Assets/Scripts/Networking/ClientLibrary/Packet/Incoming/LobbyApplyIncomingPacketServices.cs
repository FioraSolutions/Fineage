using System;

/// <summary>
/// Login / char-select lobby apply hub (#142 batch slice 162; login session orchestration slice 193).
/// </summary>
public static class LobbyApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginSessionApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginSessionApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginSessionApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);
}
