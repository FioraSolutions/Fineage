using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AcquireSkillList)]
public sealed class AcquireSkillListIncoming : IncomingWirePacket<AcquireSkillListDto>
{
    public override void Apply(AcquireSkillListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SkillLearn.AddData(packet.AcquireList);
            IncomingPacketServices.SkillLearn.ShowWindow();
        });
    }
}
