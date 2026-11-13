
using UnityEngine;

public static partial class EntityActionVisual
{
    public static void CancelMove(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Moves == null)
            return;
        if (entity.Identity.Id > 0)
        {
            MagicGaitTrace.Event(entity, "CancelMove", null);
            ControllerPacketServices.Moves.CancelMove(entity.Identity.Id);
            ControllerPacketServices.Moves.CancelRotate(entity.Identity.Id);
        }
    }

    public static void FreezeUserMove(Entity entity)
    {
        UserEntity user = entity as UserEntity;
        if (user == null || user.IsDead())
            return;
        NetworkAnimationController nac = user.GetAnimatorController();
        if (nac == null)
            return;
        nac.SetAnimatorSpeed(0f);
    }

    public static void StartMove(Entity entity, bool walking, bool force = false)
    {
        if (entity is UserEntity userStart)
        {
            PlayUserLocomotion(userStart, walking ? "walk" : "run", null, force);
            return;
        }

        if (!CanPlayAnim(entity))
        {
            EntitySpawnShared.LogStriderGait("start-move-blocked", entity, "walking=" + walking);
            return;
        }

        string anim = walking
            ? AnimationNames.MONSTER_WALK.ToString()
            : AnimationNames.MONSTER_RUN.ToString();
        if (TryPlayCreatureAnim(entity, anim))
            EntitySpawnShared.LogStriderGait("start-move", entity, "asked=" + anim);
    }
}
