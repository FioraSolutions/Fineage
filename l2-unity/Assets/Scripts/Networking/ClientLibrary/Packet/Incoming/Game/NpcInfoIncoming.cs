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
        if (IncomingPacketServices.Domain != null && add != null)
            IncomingPacketServices.QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            IncomingPacketServices.QueueWorld(apply => apply.UpdateNpc(packet));
    }
}
