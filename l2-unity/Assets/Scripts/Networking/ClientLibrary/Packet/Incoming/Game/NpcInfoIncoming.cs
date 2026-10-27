using UnityEngine;

[IncomingGamePacket(GameServerPacketType.NpcInfo)]
public sealed class NpcInfoIncoming : IncomingWirePacket<NpcInfoDto>
{
    public override void Apply(NpcInfoDto packet)
    {
        if (packet.Identity != null)
            SpawnFrameBudget.ClearCancel(packet.Identity.Id);
        StorageNpc.getInstance().AddNpcInfo(packet);
        DomainKnownlistAdd add = InterludeDomainMap.Npc(packet);
        if (WorldIncomingPacketServices.Domain != null && add != null)
            WorldIncomingPacketServices.QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            WorldIncomingPacketServices.QueueWorld(apply => apply.UpdateNpc(packet));
    }
}
