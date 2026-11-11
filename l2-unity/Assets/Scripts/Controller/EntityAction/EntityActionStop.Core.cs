
using UnityEngine;

public sealed partial class EntityActionStop : IEntityActionProcess
{
    public EntityActionStop(L2PawnRange pawnRange)
    {
        Range = pawnRange;
    }

    public void Enter(Entity entity, object payload)
    {
        if (entity == null || entity.IsDead())
            return;

        EntityActionVisual.PlayStandWait(entity);
    }

    public void Tick(Entity entity)
    {
    }
}
