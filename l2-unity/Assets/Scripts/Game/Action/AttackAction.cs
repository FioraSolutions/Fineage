using UnityEngine;
using static AttackingState;

public class AttackAction : L2Action
{
    private TargetManager _targets;

    public void Bind(TargetManager targets)
    {
        _targets = targets;
    }

    private TargetManager Targets
    {
        get { return _targets != null ? _targets : ActionPacketServices.Targets; }
    }

    public AttackAction() : base() { }

    public override void UseAction()
    {
        TargetManager targets = Targets;
        if (targets == null || !targets.HasTarget())
            return;

        if (!PlayerEntity.Instance.IsAttack && PlayerStateMachine.Instance.State != PlayerState.DEAD)
        {
            TargetData target = targets.Target;

            if (target != null && !target.IsDead())
            {
                Entity entity = target.GetEntity();
                if (entity is MonsterEntity)
                {
                    targets.SetAttackTarget();
                }

                var l2jpos = target.Identity.GetL2jPos();
                IZoneTransport game = ZoneCommands.Transport;
                if (game != null)
                    ZoneCommands.Send(new ClickActionCommand(target.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
            }
        }
    }
}
