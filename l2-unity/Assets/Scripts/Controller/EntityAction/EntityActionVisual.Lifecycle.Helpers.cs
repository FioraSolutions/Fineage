
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
            // Debug.Log(userStop.LogTag + " Visual.StopMove");
            PlayUserLocomotion(userStop, "wait");
            return;
        }

        if (!CanPlayAnim(entity))
            return;

        if (entity is NpcEntity npc)
            npc.OnStopL2jMoving();
        else if (entity is MonsterEntity monster)
        {
            if (ControllerPacketServices.Animations != null && monster.Identity != null)
            {

                ControllerPacketServices.Animations.PlayMonsterAnimation(
                    monster.Identity.Id,
                    AnimationNames.MONSTER_WAIT.ToString());
                EntitySpawnShared.LogStriderGait("stop-move", monster, "asked=wait");
            }
        }
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

        if (entity is MonsterEntity monster && ControllerPacketServices.Animations != null)
        {

            ControllerPacketServices.Animations.PlayMonsterAnimation(monster.Identity.Id, AnimationNames.MONSTER_ATK01.ToString());
        }
    }
}
