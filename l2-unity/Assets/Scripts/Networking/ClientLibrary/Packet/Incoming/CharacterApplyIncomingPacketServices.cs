using System;

/// <summary>
/// Character lobby / selection apply hub (#142 batch slice 159; login accessors slice 166).
/// </summary>
public static class CharacterApplyIncomingPacketServices
{
    public static LoginClient Login => IncomingPacketLocatorApplyIncomingPacketServices.Login;

    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => IncomingPacketLocatorApplyIncomingPacketServices.Manager;

    public static LoginWindow LoginWindow => IncomingPacketLocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => IncomingPacketLocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => IncomingPacketLocatorApplyIncomingPacketServices.Characters;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
