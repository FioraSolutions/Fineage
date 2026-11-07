using System;

/// <summary>
/// Presence applies for NPCs, characters, ground items and teleports, constructed by the
/// container instead of reached through static facades (#142 slice 3).
/// </summary>
public sealed class PresenceApplyService
{
    public void ApplyNpcInfo(NpcInfoDto packet)
    {
        if (packet.Identity != null)
            SpawnFrameBudget.ClearCancel(packet.Identity.Id);
        StorageNpc.getInstance().AddNpcInfo(packet);
        DomainKnownlistAdd add = InterludeDomainMap.Npc(packet);
        if (WorldApplyIncomingPacketServices.Domain != null && add != null)
            WorldApplyIncomingPacketServices.QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            WorldApplyIncomingPacketServices.QueueWorld(apply => apply.UpdateNpc(packet));
    }

    public void ApplyCharInfo(CharInfoDto packet)
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
        if (WorldApplyIncomingPacketServices.Domain != null && add != null)
            WorldApplyIncomingPacketServices.QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            WorldApplyIncomingPacketServices.QueueWorld(apply => apply.UpdateUser(packet));
    }

    public void ApplyDropItem(DropItemDto dto)
    {
        World gameWorld = WorldApplyIncomingPacketServices.GameWorld;
        if (dto == null || gameWorld == null)
            return;

        gameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            dto.CharObjId);
    }

    public void ApplyTeleportToLocation(TeleportToLocationDto packet) =>
        WorldApplyIncomingPacketServices.Queue(
            () => WorldApplyIncomingPacketServices.GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos));

    public void ApplyRevive(ReviveDto dto) =>
        WorldApplyIncomingPacketServices.ApplyWorld(apply => apply.Revive(dto));
}
