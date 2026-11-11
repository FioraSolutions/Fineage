using UnityEngine;

public sealed partial class NpcWorldApply : EntityWorldApply
{
    public NpcWorldApply(HtmlWindow html, EntityActionMachine actions)
    {
        _html = html;
        _actions = actions;
    }

    public override void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnStopMove(Entity entity, StopMoveDto dto)
    {
        _actions.ApplyStop(entity, dto);
    }

    public override void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnDie(Entity entity, DieDto dto)
    {
        _actions.Die(entity, false, "packet");
    }

    public override void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
        if (attacker == null || attacker.IsDead() || attacker == target)
            return;
        _actions.Set(attacker, EntityActionKind.Attack, dto);
    }
}
