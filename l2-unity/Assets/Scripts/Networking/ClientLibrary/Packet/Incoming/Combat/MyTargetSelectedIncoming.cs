using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MyTargetSelected)]
public sealed class MyTargetSelectedIncoming : IncomingWirePacket<MyTargetSelectDto>
{
    public override void Apply(MyTargetSelectDto packet)
    {
        CombatIncomingPacketServices.Queue(() =>
        {
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                CombatIncomingPacketServices.Targets.SetTarget(new ObjectData(PlayerEntity.Instance.transform.gameObject), "#ffffff");
            }
            else
            {
                CombatIncomingPacketServices.Targets.NextTargetById(packet.ObjectId, packet.Color);
                CombatIncomingPacketServices.ApplyWorld(apply => apply.SendArrivedPosition());
            }
        });
    }
}
