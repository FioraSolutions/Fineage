using UnityEngine;

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

        // Do not gate on IsAttack: that latch used to stick after the first swing (#522), which
        // blocked the attack hotkey until a second entity click. Zone ignores redundant AttackRequest
        // while already swinging the same target.
        if (PlayerStateMachine.Instance != null &&
            PlayerStateMachine.Instance.State == PlayerState.DEAD)
            return;

        TargetData target = targets.Target;
        if (target == null || target.IsDead())
            return;

        Entity entity = target.GetEntity();
        if (entity is MonsterEntity)
            targets.SetAttackTarget();

        var l2jpos = target.Identity.GetL2jPos();
        if (ZoneCommands.Transport != null)
            ZoneCommands.Send(new ClickActionCommand(
                target.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
    }
}
