using System;

/// <summary>
/// NPC HTML message wire packet handler entry (#142 batch slice 138; world hub slice 180).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="NpcHtmlMessageIncomingPacketServices"/>.
/// </summary>
public static class NpcHtmlMessageApplyIncomingPacketServices
{
    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.QueueWorld(apply);

    public static void ApplyNpcHtmlMessage(NpcHtmlMessageDto packet) =>
        QueueWorld(apply => apply.ShowNpcHtml(packet));
}
