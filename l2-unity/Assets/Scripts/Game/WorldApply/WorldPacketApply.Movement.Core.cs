using UnityEngine;

public sealed partial class WorldPacketApply
{
    public void MoveTo(int objId, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        Entity entity = GetEntity(objId);
        if (entity == null || string.Equals(entity.name, "Elpy"))
            return;

        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write(entity.IsDead() ? "MOVE_SKIP" : "MOVE", entity, null,
                // NpcCombatTrace.Place(entity) +
                // " dest=" + destination.x.ToString("F0") + "," + destination.y.ToString("F0") + "," + destination.z.ToString("F0"));

        EntityWorldApply apply;
        if (TryResolve(entity, out apply))
            apply.OnMoveTo(entity, destination, current, dto);
        // UnityReplaySession.OnMoveTo(entity, destination);
    }

    public void ApplyPositionCorrection(int objId, Vector3 position)
    {
        Entity entity = GetEntity(objId);
        if (entity == null)
            return;

        EntityWorldApply apply;
        if (TryResolve(entity, out apply))
            apply.OnPositionCorrection(entity, position);
        else
            EntityWorldApply.SnapTransform(entity, position);
    }
}
