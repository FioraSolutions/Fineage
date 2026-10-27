/// <summary>
/// Entity social animation, wait-type, and abnormal-status incoming handler access via WorldIncomingPacketServices (#142 sub-slice 68).
/// </summary>
public static class EntityAnimIncomingPacketServices
{
    public static BufferPanel Buffer => WorldIncomingPacketServices.Buffer;

    public static void ApplySocialAction(SocialActionDto packet) =>
        WorldIncomingPacketServices.ApplyWorld(apply => apply.SocialAction(packet));

    public static void ApplyChangeWaitType(ChangeWaitTypeDto packet) =>
        WorldIncomingPacketServices.ApplyWorld(apply => apply.ChangeWaitType(packet));

    public static void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            if (Buffer == null)
                return;
            foreach (var item in packet.ListEffect)
                Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
