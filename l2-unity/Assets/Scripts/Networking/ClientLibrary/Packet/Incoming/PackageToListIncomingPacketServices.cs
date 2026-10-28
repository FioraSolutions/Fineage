using System;

/// <summary>
/// Package-to-list wire packet handler entry (#142 batch slice 145).
/// Re-homes apply from <see cref="ShowListIncomingPacketServices"/> (sub-slice 74).
/// </summary>
public static class PackageToListIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyPackageToList(PackageToListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.ShowList.AddList(packet.Players);
            UiIncomingPacketServices.ShowList.ShowWindow();
        });
}
