using System;

/// <summary>
/// My-target-selected wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MyTargetSelectedIncomingPacketServices"/>.
/// </summary>
public static class MyTargetSelectedApplyIncomingPacketServices
{
    public static TargetManager Targets => CombatPacketServices.Targets;

    public static void Queue(Action action) => WorldApplyPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyPacketServices.ApplyWorld(apply);

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
