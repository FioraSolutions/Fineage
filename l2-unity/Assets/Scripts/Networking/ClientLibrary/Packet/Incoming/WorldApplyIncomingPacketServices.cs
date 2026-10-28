using System;

/// <summary>
/// World presence / AOI incoming handler access via domain facades and queue fallbacks (#142 sub-slice 52).
/// Apply facade hub (#142 batch slice 157). Re-homes from <see cref="WorldIncomingPacketServices"/>.
/// </summary>
public static class WorldApplyIncomingPacketServices
{
    public static GameClient Game => GamePacketServices.Game;

    public static World GameWorld => WorldApplyPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldApplyPacketServices.Domain;

    public static SeedInfoWindow SeedInfo => AuxiliaryGameUIPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => AuxiliaryGameUIPacketServices.SellCrop;

    public static BufferPanel Buffer => AuxiliaryGameUIPacketServices.Buffer;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
