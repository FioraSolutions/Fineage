using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SkillList)]
public sealed class SkillListIncoming : IncomingWirePacket<SkillListDto>
{
    public override void Apply(SkillListDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.SkillList.UpdateSkillList(packet.Skills));
    }
}
