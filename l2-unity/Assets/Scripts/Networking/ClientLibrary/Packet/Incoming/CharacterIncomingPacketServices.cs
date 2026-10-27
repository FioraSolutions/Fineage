using System;

/// <summary>
/// Character lobby / selection incoming handler access via LoginPacketServices and queue fallbacks (#142 sub-slice 51).
/// </summary>
public static class CharacterIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static CharSelectWindow CharSelect => LoginPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginPacketServices.Characters;

    public static IWorldDomainSink Domain => IncomingPacketServices.Domain;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketServices.QueueWorld(apply);
}
