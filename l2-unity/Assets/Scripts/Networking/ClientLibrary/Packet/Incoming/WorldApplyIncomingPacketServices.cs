using System;

/// <summary>
/// World presence / AOI incoming handler access via domain facades and queue fallbacks (#142 sub-slice 52).
/// Apply facade hub (#142 batch slice 157). Re-homes from <see cref="WorldIncomingPacketServices"/>.
/// </summary>
public static class WorldApplyIncomingPacketServices
{
    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static SeedInfoWindow SeedInfo => ManorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => ManorApplyIncomingPacketServices.SellCrop;

    public static BufferPanel Buffer => UiApplyIncomingPacketServices.Buffer;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
