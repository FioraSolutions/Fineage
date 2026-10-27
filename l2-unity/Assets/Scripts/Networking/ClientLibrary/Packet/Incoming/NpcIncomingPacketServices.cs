using System;

/// <summary>
/// NPC / pet presence incoming handler access via WorldIncomingPacketServices (#142 sub-slice 65).
/// </summary>
public static class NpcIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldIncomingPacketServices.Domain;

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldIncomingPacketServices.QueueWorld(apply);

    public static void ApplyNpcHtmlMessage(NpcHtmlMessageDto packet) =>
        QueueWorld(apply => apply.ShowNpcHtml(packet));

    public static void ApplyNpcInfo(NpcInfoDto packet) => ApplyNpcOrPetInfo(packet);

    public static void ApplyPetInfo(NpcInfoDto packet) => ApplyNpcOrPetInfo(packet);

    public static void ApplyPetDelete(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }

    static void ApplyNpcOrPetInfo(NpcInfoDto packet)
    {
        if (packet.Identity != null)
            SpawnFrameBudget.ClearCancel(packet.Identity.Id);
        StorageNpc.getInstance().AddNpcInfo(packet);
        DomainKnownlistAdd add = InterludeDomainMap.Npc(packet);
        if (Domain != null && add != null)
            QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            QueueWorld(apply => apply.UpdateNpc(packet));
    }
}
