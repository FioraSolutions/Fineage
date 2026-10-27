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
        if (IncomingPacketActions.Domain != null && add != null)
            IncomingPacketActions.QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            IncomingPacketActions.QueueWorld(apply => apply.UpdateNpc(packet));
    }
}
