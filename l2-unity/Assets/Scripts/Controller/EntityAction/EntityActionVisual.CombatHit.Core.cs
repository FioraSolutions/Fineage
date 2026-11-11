using UnityEngine;

public partial class EntityActionVisual
{
    public static void PlayDeath(Entity entity, bool alreadyCorpse = false)
    {
        if (entity == null)
            return;

        entity.SetDead(true);
        if (!CanPlayAnim(entity))
            return;

        if (entity is UserEntity user)
        {
            if (!alreadyCorpse && IsPlayingDeath(entity))
                return;
            PlayUserDeath(user, alreadyCorpse);
            return;
        }

        if (entity is not MonsterEntity monster || ControllerPacketServices.Animations == null)
            return;

        // Shared Monster_* path for every mob. Death is terminal: if the clip or held
        // corpse pose is already up (client predict + Die, or pool leftover cleared on spawn),
        // do not CrossFade again — MonsterCrossFade also skips when already in "death".
        if (IsMonsterDeathVisualActive(monster))
            return;

        // Combat can leave animator.speed / patkspd at 0; death SMB needs a live clock.
        NetworkAnimationController nac = monster.GetAnimatorController();
        if (nac != null)
        {
            nac.SetPAtkSpeed(1f);
            nac.SetAnimatorSpeed(1f);
        }

        if (!alreadyCorpse)
            MonsterCombatSound.Play(MonsterSoundEvent.Death, monster);

        ControllerPacketServices.Animations.PlayMonsterAnimation(
            monster.Identity.Id, AnimationNames.DEAD.ToString());
    }

    static bool IsMonsterDeathVisualActive(MonsterEntity monster)
    {
        if (monster == null)
            return false;
        if (monster.ActionSlot != null && monster.ActionSlot.DeathPoseHeld)
            return true;
        return IsPlayingDeath(monster);
    }
}
