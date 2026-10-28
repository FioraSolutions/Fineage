using System;

/// <summary>
/// NPC HTML message wire packet handler entry (#142 batch slice 138).
/// </summary>
public static class NpcHtmlMessageIncomingPacketServices
{
    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldIncomingPacketServices.QueueWorld(apply);

    public static void ApplyNpcHtmlMessage(NpcHtmlMessageDto packet) =>
        QueueWorld(apply => apply.ShowNpcHtml(packet));
}
