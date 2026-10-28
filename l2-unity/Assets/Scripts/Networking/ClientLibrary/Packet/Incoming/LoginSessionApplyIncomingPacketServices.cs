using System;

/// <summary>
/// Login-session wire apply hub (#142 batch slice 169; login hub slice 174–175).
/// </summary>
public static class LoginSessionApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => LoginApplyIncomingPacketServices.Queue(action);
}
