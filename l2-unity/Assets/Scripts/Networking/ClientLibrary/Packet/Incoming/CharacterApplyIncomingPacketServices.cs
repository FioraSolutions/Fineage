using System;

/// <summary>
/// Character lobby / selection apply hub (#142 batch slice 159; world session orchestration slice 194).
/// </summary>
public static class CharacterApplyIncomingPacketServices
{
    public static LoginClient Login => LobbyApplyIncomingPacketServices.Login;

    public static GameClient Game => LobbyApplyIncomingPacketServices.Game;

    public static GameManager Manager => LobbyApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LobbyApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LobbyApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LobbyApplyIncomingPacketServices.Characters;

    public static IWorldDomainSink Domain => PlayerApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => PlayerApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PlayerApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PlayerApplyIncomingPacketServices.QueueWorld(apply);
}
