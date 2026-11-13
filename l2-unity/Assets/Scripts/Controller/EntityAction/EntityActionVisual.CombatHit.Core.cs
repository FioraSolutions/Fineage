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

        if (!UsesCreatureAnim(entity) || entity.Identity == null)
            return;

        // Terminal: clip or held corpse pose already up — do not CrossFade again.
        if (IsCreatureDeathVisualActive(entity))
            return;

        // Combat can leave animator.speed / patkspd at 0; death SMB needs a live clock.
        NetworkAnimationController nac = entity.GetAnimatorController();
        if (nac != null)
        {
            nac.SetPAtkSpeed(1f);
            nac.SetAnimatorSpeed(1f);
        }

        if (!alreadyCorpse && entity is MonsterEntity monster)
            MonsterCombatSound.Play(MonsterSoundEvent.Death, monster);

        TryPlayCreatureAnim(entity, AnimationNames.DEAD.ToString());
    }
}
