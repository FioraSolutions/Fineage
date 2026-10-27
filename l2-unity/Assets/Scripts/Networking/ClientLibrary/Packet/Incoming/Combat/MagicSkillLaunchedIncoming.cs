using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MagicSkillLaunched)]
public sealed class MagicSkillLaunchedIncoming : IncomingWirePacket<MagicSkillLaunchedDto>
{
    public override void Apply(MagicSkillLaunchedDto packet)
    {
        if (packet == null)
            return;
        IncomingPacketServices.QueueWorld(apply => apply.MagicSkillLaunched(packet));
    }
}
