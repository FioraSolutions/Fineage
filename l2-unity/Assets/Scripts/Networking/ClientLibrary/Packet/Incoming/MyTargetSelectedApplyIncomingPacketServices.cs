using System;

/// <summary>
/// My-target-selected wire packet handler entry (#142 batch slice 144; combat packet slice 179).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MyTargetSelectedIncomingPacketServices"/>.
/// </summary>
public static class MyTargetSelectedApplyIncomingPacketServices
{
    public static TargetManager Targets => CombatPacketApplyIncomingPacketServices.Targets;

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply);

    public static void ApplyMyTargetSelected(MyTargetSelectDto packet)
    {
        Queue(() =>
        {
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                Targets.SetTarget(new ObjectData(PlayerEntity.Instance.transform.gameObject), "#ffffff");
            }
            else
            {
                Targets.NextTargetById(packet.ObjectId, packet.Color);
                ApplyWorld(apply => apply.SendArrivedPosition());
            }
        });
    }
}
