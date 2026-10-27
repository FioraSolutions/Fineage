using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AcquireSkillInfo)]
public sealed class AcquireSkillInfoIncoming : IncomingWirePacket<AcquireSkillInfoDto>
{
    public override void Apply(AcquireSkillInfoDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.SkillLearn.HideWindow();
            UiIncomingPacketServices.SkillDesc.AddData(packet);
            UiIncomingPacketServices.SkillDesc.ShowWindow();
        });
    }
}
