using System;

/// <summary>
/// Login / char-select lobby apply hub (#142 batch slice 162; forwards to character hub slice 166).
/// </summary>
public static class LobbyApplyIncomingPacketServices
{
    public static LoginClient Login => CharacterApplyIncomingPacketServices.Login;

    public static GameClient Game => CharacterApplyIncomingPacketServices.Game;

    public static GameManager Manager => CharacterApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => CharacterApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => CharacterApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => CharacterApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => CharacterApplyIncomingPacketServices.Queue(action);
}
