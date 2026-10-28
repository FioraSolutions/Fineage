using System;

/// <summary>
/// World presence / AOI incoming handler access via domain facades (#142 sub-slice 52; session hub slice 221).
/// </summary>
public static class WorldApplyIncomingPacketServices
{
    public static GameClient Game => WorldSessionApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldSessionApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static SeedInfoWindow SeedInfo => LocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => LocatorApplyIncomingPacketServices.SellCrop;

    public static BufferPanel Buffer => UiApplyIncomingPacketServices.Buffer;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueWorld(apply);
}
