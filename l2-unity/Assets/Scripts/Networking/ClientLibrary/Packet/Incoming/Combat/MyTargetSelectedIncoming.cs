using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MyTargetSelected)]
public sealed class MyTargetSelectedIncoming : IncomingWirePacket<MyTargetSelectDto>
{
    public override void Apply(MyTargetSelectDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                IncomingPacketServices.Targets.SetTarget(new ObjectData(PlayerEntity.Instance.transform.gameObject), "#ffffff");
            }
            else
            {
                IncomingPacketServices.Targets.NextTargetById(packet.ObjectId, packet.Color);
                IncomingPacketServices.ApplyWorld(apply => apply.SendArrivedPosition());
            }
        });
    }
}
