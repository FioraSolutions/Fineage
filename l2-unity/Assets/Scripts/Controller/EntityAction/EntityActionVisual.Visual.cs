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

    public static void PlayCombatWait(Entity entity, float? fadeDuration = null)
    {
        UserEntity userWait = entity as UserEntity;
        if (userWait != null && userWait.IsAttackVisualPlaying())
            return;

        if (entity is UserEntity user)
        {
            MagicGaitTrace.Event(entity, "PlayCombatWait", "→atkwait");
            PlayUserLocomotion(user, "atkwait", fadeDuration);
            return;
        }

        if (!CanPlayAnim(entity))
            return;

        if (entity is MonsterEntity monster && ControllerPacketServices.Animations != null && monster.Identity != null)
        {
      
            ControllerPacketServices.Animations.PlayMonsterAnimation(monster.Identity.Id, "atkwait");
        }
    }

    public static void PlaySocial(Entity entity, int actionId)
    {
        if (entity == null)
            return;

        // Mobius SocialAction.LEVEL_UP = 15 → EventBus → e_u004_a + levelup.ogg.
        if (actionId == 15 && ControllerPacketServices.Bus != null)
            ControllerPacketServices.Bus.LevelUp(
                entity, entity.Identity != null ? entity.Identity.Id : 0);

        // Cast owns the Animator while MagicSkillPresentation holds AnimChannel.Cast.
        if (!AnimChannel.CanPlay(AnimChannel.Kind.Social))
            return;

        NetworkAnimationController nac = entity.GetAnimatorController();
        if (nac != null)
        {
            AnimPlayback.PrepareGesture(nac);
            nac.CrossFadeInFixedTime("social01", LocomotionCrossFadeSettings.FixedDuration);
            return;
        }

        if ((entity is NpcEntity || entity is MonsterEntity) &&
            ControllerPacketServices.Animations != null &&
            entity.Identity != null)
        {
            ControllerPacketServices.Animations.PlayMonsterAnimation(
                entity.Identity.Id, "social01");
        }
    }

    public static void PlayWaitType(Entity entity, WaitType waitType)
    {
        UserEntity user = entity as UserEntity;
        if (user == null)
            return;

        if (waitType == WaitType.WT_SITTING)
        {
            NetworkAnimationController nac = user.GetAnimatorController();
            if (nac == null)
                return;
            nac.SetAnimatorSpeed(1f);
            nac.CrossFadeInFixedTime("sit", LocomotionCrossFadeSettings.FixedDuration);
            return;
        }

        if (waitType == WaitType.WT_START_FAKEDEATH)
        {
            // WaitType 2 is not bDead. OnMoveTo stores it back to standing and walks.
            PlayUserDeath(user, false);
            return;
        }

        if (waitType == WaitType.WT_STOP_FAKEDEATH)
        {
            PlayRevive(user);
            return;
        }

        PlayUserLocomotion(user, "wait");
    }
}
