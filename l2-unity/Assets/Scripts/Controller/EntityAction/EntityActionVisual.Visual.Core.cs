
using UnityEngine;

public static partial class EntityActionVisual
{
    public static void PlayPhysicalSkill(Entity entity, MagicSkillUseDto magic)
    {
        if (!CanPlayAnim(entity))
            return;

        if (entity is UserEntity user)
        {
            PlayUserPhysicalSkill(user, magic);
            return;
        }

        PlayMeleeAttack(entity);
    }

    public static void RestoreLocomotion(Entity entity, string state)
    {
        if (entity == null || entity.Identity == null)
            return;
        if (state != "walk" && state != "run" && state != "wait")
            state = "wait";

        if (state == "wait")
        {
            StopMove(entity);
            return;
        }

        if (ControllerPacketServices.Animations == null)
            return;
        ControllerPacketServices.Animations.PlayMonsterAnimation(entity.Identity.Id, state);
    }

    public static void PlayStandWait(Entity entity)
    {
        if (entity == null || entity.IsDead())
            return;
        UserEntity user = entity as UserEntity;
        if (user != null && user.IsAttackVisualPlaying())
        {
            MagicGaitTrace.Event(entity, "PlayStandWait-SKIP", "attackVis=true");
            return;
        }
        MagicGaitTrace.Event(entity, "PlayStandWait",
            "inCombat=" + entity.InCombat + " → " + (entity.InCombat ? "atkwait" : "wait"));
        if (entity.InCombat)
            PlayCombatWait(entity);
        else
            StopMove(entity);
    }
}
