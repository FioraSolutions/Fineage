using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AcquireSkillList)]
public sealed class AcquireSkillListIncoming : IncomingWirePacket<AcquireSkillListDto>
{
    public override void Apply(AcquireSkillListDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.SkillLearn.AddData(packet.AcquireList);
            UiIncomingPacketServices.SkillLearn.ShowWindow();
        });
    }
}
