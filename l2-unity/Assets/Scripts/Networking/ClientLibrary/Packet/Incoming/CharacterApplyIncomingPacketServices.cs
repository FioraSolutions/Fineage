using System;

/// <summary>
/// Character lobby / selection apply hub (#142 batch slice 159; world session orchestration slice 194).
/// </summary>
public static class CharacterApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginApplyIncomingPacketServices.Characters;

    public static IWorldDomainSink Domain => PresenceApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => PresenceApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PresenceApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.QueueWorld(apply);
}
