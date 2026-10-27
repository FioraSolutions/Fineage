using System;

/// <summary>
/// Package-to-list / show-list wire packet handler access via UiIncomingPacketServices (#142 sub-slice 74).
/// </summary>
public static class ShowListIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyPackageToList(PackageToListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.ShowList.AddList(packet.Players);
            UiIncomingPacketServices.ShowList.ShowWindow();
        });
}
