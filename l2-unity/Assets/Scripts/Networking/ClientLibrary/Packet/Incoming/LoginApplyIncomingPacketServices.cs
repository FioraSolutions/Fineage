using System;

/// <summary>
/// Login / char-select compat shell (#142 batch slice 168; LoginSession orchestration slice 206).
/// </summary>
public static class LoginApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginSessionApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginSessionApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginSessionApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);
}
