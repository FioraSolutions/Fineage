using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AcquireSkillInfo)]
public sealed class AcquireSkillInfoIncoming : IncomingWirePacket<AcquireSkillInfoDto>
{
    public override void Apply(AcquireSkillInfoDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SkillLearn.HideWindow();
            IncomingPacketServices.SkillDesc.AddData(packet);
            IncomingPacketServices.SkillDesc.ShowWindow();
        });
    }
}
