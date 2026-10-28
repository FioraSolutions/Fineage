using System;

/// <summary>
/// Char info wire packet handler entry (#142 batch slice 147; world hub slice 180).
/// Re-homes char-info apply from <see cref="CharInfoIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharInfoApplyIncomingPacketServices
{
    public static IWorldDomainSink Domain => WorldApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.QueueWorld(apply);

    public static void ApplyCharInfo(CharInfoDto packet)
    {
        if (packet == null || packet.Identity == null)
            return;

        PlayerEntity local = PlayerEntity.Instance;
        if (local != null && local.Identity != null && local.Identity.Id == packet.Identity.Id)
            return;

        MoveCostLog.NoteCharInfo();
        SpawnFrameBudget.ClearCancel(packet.Identity.Id);
        StorageNpc.getInstance().AddCharInfo(packet);
        DomainKnownlistAdd add = InterludeDomainMap.Char(packet);
        if (Domain != null && add != null)
            QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            QueueWorld(apply => apply.UpdateUser(packet));
    }
}
