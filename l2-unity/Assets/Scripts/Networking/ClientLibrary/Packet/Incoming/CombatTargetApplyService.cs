using System;

/// <summary>
/// Own-target selection apply, constructed by the container instead of reached through the
/// static facade (#142 slice 3).
/// </summary>
public sealed class CombatTargetApplyService
{
    public void ApplyMyTargetSelected(MyTargetSelectDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            TargetManager targets = CombatPacketApplyIncomingPacketServices.Targets;
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                targets.SetTarget(
                    new ObjectData(PlayerEntity.Instance.transform.gameObject),
                    "#ffffff");
            }
            else
            {
                targets.NextTargetById(packet.ObjectId, packet.Color);
                QueueApplyIncomingPacketServices.ApplyWorld(apply => apply.SendArrivedPosition());
            }
        });
}
