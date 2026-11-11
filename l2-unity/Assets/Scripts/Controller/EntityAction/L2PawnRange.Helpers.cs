using System;
using UnityEngine;

public sealed partial class L2PawnRange
{
    public static bool HasReachedPawnDist(Entity entity)
    {
        if (entity == null)
            return false;
        Entity pawn = EntityPawnLookup.PawnOf(entity);
        float dist = entity.ActionSlot.PawnDist;
        if (pawn == null || dist <= 0.01f)
            return false;
        return VectorUtils.Distance2D(entity.transform.position, pawn.transform.position) <= dist + 0.12f;
    }

    public static void IgnorePawnCollision(Entity mover, Entity pawn)
    {
        ClearIgnoredPawn(mover);
        if (mover == null || pawn == null)
            return;
        IgnoreBetween(mover, pawn, true);
        mover.ActionSlot.CollisionPawn = pawn;
    }

    public static void ClearIgnoredPawn(Entity mover)
    {
        if (mover == null || mover.ActionSlot == null)
            return;
        Entity pawn = mover.ActionSlot.CollisionPawn;
        if (pawn == null)
            return;
        IgnoreBetween(mover, pawn, false);
        mover.ActionSlot.CollisionPawn = null;
    }

    public static bool TrySnapUserToPacket(Entity entity, Vector3 packetPos, string reason)
    {
        if (entity == null || entity is PlayerEntity)
            return false;

        float d = VectorUtils.Distance2D(entity.transform.position, packetPos);
        if (d <= VectorUtils.ConvertL2UuToMeters(AdjustSkipUu))
            return false;

        Vector3 from = entity.transform.position;
        Vector3 grounded = GroundSnapHelper.SnapToGroundOrKeep(packetPos);
        if (entity.Identity != null)
            entity.Identity.Position = grounded;
        EntitySpawnShared.ApplyGroundedTransform(entity.gameObject, grounded, entity.transform.rotation);
        // EntityActionCombatLog.LogGap(entity, "SNAP " + reason, EntityPawnLookup.PawnOf(entity),
            // " snapD=" + d.ToString("F2") +
            // " from=" + EntityActionCombatLog.Vec(from) +
            // " to=" + EntityActionCombatLog.Vec(grounded));
        return true;
    }

    static void IgnoreBetween(Entity a, Entity b, bool ignore)
    {
        if (a == null || b == null)
            return;

        CharacterController[] movers = a.GetComponentsInChildren<CharacterController>(true);
        CharacterController[] pawns = b.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < movers.Length; i++)
        {
            if (movers[i] == null)
                continue;
            for (int j = 0; j < pawns.Length; j++)
            {
                if (pawns[j] == null)
                    continue;
                Physics.IgnoreCollision(movers[i], pawns[j], ignore);
            }
        }
    }
}
