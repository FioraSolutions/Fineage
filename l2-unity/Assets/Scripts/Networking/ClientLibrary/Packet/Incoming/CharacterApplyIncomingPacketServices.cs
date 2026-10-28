using System;

/// <summary>
/// Character lobby / selection apply hub (#142 batch slice 159; world session orchestration slice 194).
/// </summary>
public static class CharacterApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginSessionApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginSessionApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginSessionApplyIncomingPacketServices.Characters;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueWorld(apply);
}
