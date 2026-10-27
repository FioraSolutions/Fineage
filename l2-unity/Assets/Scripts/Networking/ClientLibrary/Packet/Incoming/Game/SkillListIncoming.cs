using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SkillList)]
public sealed class SkillListIncoming : IncomingWirePacket<SkillListDto>
{
    public override void Apply(SkillListDto packet)
    {
        UiIncomingPacketServices.Queue(() => UiIncomingPacketServices.SkillList.UpdateSkillList(packet.Skills));
    }
}
