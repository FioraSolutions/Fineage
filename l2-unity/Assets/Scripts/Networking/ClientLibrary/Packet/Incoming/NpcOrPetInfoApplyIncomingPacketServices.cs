using System;

/// <summary>
/// Shared NPC/pet presence apply (#142 batch slice 138).
/// </summary>
internal static class NpcOrPetInfoApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldApplyIncomingPacketServices.Domain;

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.QueueWorld(apply);

    public static void Apply(NpcInfoDto packet)
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
