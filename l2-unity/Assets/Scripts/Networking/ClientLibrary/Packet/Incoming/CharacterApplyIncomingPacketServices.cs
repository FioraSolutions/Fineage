using System;

/// <summary>
/// Character lobby / selection apply hub (#142 batch slice 159; login hub slice 174).
/// </summary>
public static class CharacterApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LoginApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginApplyIncomingPacketServices.Characters;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueWorld(apply);
}
