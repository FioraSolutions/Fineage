using System;
using VContainer;

/// <summary>
/// Own-target selection apply, constructed by the container instead of reached through the
/// static facade (#142 slice 3). <see cref="TargetManager"/> lives in Input.prefab and
/// appears after the scope, so it is read through <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class CombatTargetApplyService
{
    private readonly ILateBound<TargetManager> _targets;

    [Inject]
    public CombatTargetApplyService(ILateBound<TargetManager> targets)
    {
        _targets = targets;
    }

    public void ApplyMyTargetSelected(MyTargetSelectDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            TargetManager targets = _targets != null ? _targets.Value : null;
            if (targets == null)
                return;

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
