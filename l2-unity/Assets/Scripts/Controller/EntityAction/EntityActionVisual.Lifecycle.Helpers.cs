
using UnityEngine;

public static partial class EntityActionVisual
{
    public static void PlayDisappearWait(Entity entity)
    {
        if (entity == null || entity.IsDead())
            return;
        if (entity is UserEntity user)
        {
            PlayUserLocomotion(user, "wait", null, true);
            return;
        }
        StopMove(entity);
    }

    public static void StopMove(Entity entity)
    {
        if (entity is UserEntity userStop)
        {
            PlayUserLocomotion(userStop, "wait");
            return;
        }

        if (!CanPlayAnim(entity))
            return;

        if (TryPlayCreatureAnim(entity, AnimationNames.MONSTER_WAIT.ToString()))
            EntitySpawnShared.LogStriderGait("stop-move", entity, "asked=wait");
    }

    public static void PlayMeleeAttack(Entity entity)
    {
        if (!CanPlayAnim(entity))
            return;

        if (entity is UserEntity user)
        {
            PlayUserMeleeAttack(user);
            return;
        }

        TryPlayCreatureAnim(entity, AnimationNames.MONSTER_ATK01.ToString());
    }
}
