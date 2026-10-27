using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AcquireSkillInfo)]
public sealed class AcquireSkillInfoIncoming : IncomingWirePacket<AcquireSkillInfoDto>
{
    public override void Apply(AcquireSkillInfoDto packet) =>
        SkillLearnIncomingPacketServices.ApplyAcquireSkillInfo(packet);
}
