using UnityEngine;
using static AttackingState;

public class AttackAction : L2Action
{
    private TargetManager _targets;
    private GameClient _game;

    public void Bind(TargetManager targets, GameClient game)
    {
        _targets = targets;
        _game = game;
    }

    private TargetManager Targets
    {
        get { return _targets != null ? _targets : IncomingPacketActions.Targets; }
    }

    private GameClient Game
    {
        get { return _game != null ? _game : IncomingPacketActions.Game; }
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
                GameClient game = Game;
                if (game != null)
                    game.Send(new ClickActionCommand(target.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
            }
        }
    }
}
