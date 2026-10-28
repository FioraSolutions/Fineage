using System;

/// <summary>
/// Char info wire packet handler entry (#142 sub-slice 103).
/// Splits char-info apply out of <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharInfoIncomingPacketServices
{
    public static IWorldDomainSink Domain => IncomingPacketServices.Domain;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketServices.QueueWorld(apply);

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
