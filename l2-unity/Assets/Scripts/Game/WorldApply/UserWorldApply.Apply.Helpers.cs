using UnityEngine;

public sealed partial class UserWorldApply : EntityWorldApply
{
    public override void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
        if (entity == null || entity.IsDead())
            return;
        World world = GameWorld;
        Entity pawn = world != null
            ? world.GetEntityNoLockSync(dto.TarObjid)
            : null;
        Vector3 origin = dto.ObjPos;
        Vector3 now = entity.transform.position;
        float distNow = pawn != null ? VectorUtils.Distance2D(now, pawn.transform.position) : -1f;
        float distPkt = pawn != null ? VectorUtils.Distance2D(origin, pawn.transform.position) : -1f;
        float originNow = VectorUtils.Distance2D(origin, now);
        // EntityActionCombatLog.LogCiPawn(entity,
            // "MoveToPawn nick=" + EntityActionCombatLog.NameOf(entity) +
            // " action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat +
            // " distPkt=" + dto.Distance.ToString("F2") +
            // " distNow=" + distNow.ToString("F2") +
            // " distOriginToPawn=" + distPkt.ToString("F2") +
            // " originNow=" + originNow.ToString("F2") +
            // " origin=" + EntityActionCombatLog.Vec(origin) +
            // " now=" + EntityActionCombatLog.Vec(now) +
            // " pawn=" + EntityActionCombatLog.Describe(pawn) +
            // " pawnPos=" + (pawn != null ? EntityActionCombatLog.Vec(pawn.transform.position) : "-") +
            // " pawnDead=" + (pawn != null && pawn.IsDead()) +
            // " pawnMoving=" + EntityActionCombatLog.IsPawnMoving(pawn));
        // if (pawn == null || pawn.IsDead())
            // EntityActionCombatLog.LogGap(entity, "MoveToPawn PAWN_BAD", pawn,
                // " tarId=" + dto.TarObjid +
                // " originNow=" + originNow.ToString("F2") +
                // " distNow=" + distNow.ToString("F2"));
        // else if (originNow >= 2f || (distPkt < 2f && distNow >= 2f))
            // EntityActionCombatLog.LogGap(entity, "MoveToPawn ORIGIN_DESYNC", pawn,
                // " originNow=" + originNow.ToString("F2") +
                // " distPktToPawn=" + distPkt.ToString("F2") +
                // " distNow=" + distNow.ToString("F2") +
                // " origin=" + EntityActionCombatLog.Vec(origin));
        // EntityActionCombatLog.LogIfWatch(entity,
            // "User.MoveToPawn chase action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat);
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnDie(Entity entity, DieDto dto)
    {
        if (entity != null && entity.GetDead())
            return;
        _actions.Die(entity, false);
    }

    public override void OnRevive(Entity entity, ReviveDto dto)
    {
        _actions.Revive(entity);
    }
}
