using UnityEngine;

public partial class EntityActionVisual
{
    /// <summary>
    /// ObjectId of the pawn currently held in the Fake Death pose (Mobius <c>_isFakeDeath</c>).
    /// The pose is a terminal clip: a later wait/atkwait CrossFade must not stand the pawn up
    /// until the zone sends <c>WT_STOP_FAKEDEATH</c>.
    /// </summary>
    static int _fakeDeathPoseEntityId;

    public static bool IsFakeDeathPoseHeld(Entity entity)
    {
        return entity != null && entity.Identity != null &&
            _fakeDeathPoseEntityId == entity.Identity.Id;
    }

    /// <summary>Object-id overload for the animation manager (packets address the pawn, not the component).</summary>
    public static bool IsFakeDeathPoseHeld(int objectId)
    {
        return objectId > 0 && _fakeDeathPoseEntityId == objectId;
    }

    static void PlayUserDeath(Entity entity, bool alreadyCorpse)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return;

        ClearFakeDeathPose(entity);
        // Only the remote-pawn shape carries the swing latch; the local hero has none.
        if (entity is UserEntity user)
            user.ClearAttackVisual();

        ControllerPacketServices.Animations.PlayExactAnimatorState(
            entity.Identity.Id, PlayerDeathAnim.Death, alreadyCorpse);
    }

    /// <summary>
    /// Mobius <c>Creature.startFakeDeath</c> → <c>ChangeWaitType(WT_START_FAKEDEATH)</c>: the pawn plays the death
    /// clip without being dead (HP/MP stay, no DeadWindow). Held until the zone sends the stop wait type.
    /// </summary>
    public static void PlayFakeDeathPose(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return;

        if (entity is UserEntity user)
            user.ClearAttackVisual();
        // The pose interrupts a cast/social channel; drop the hero's swing latch too.
        AnimChannel.Clear();
        CastMoveLock.Clear();
        if (entity is PlayerEntity hero)
        {
            hero.IsAttack = false;
            hero.LastAtkAnimation = null;
        }

        _fakeDeathPoseEntityId = entity.Identity.Id;
        ControllerPacketServices.Animations.PlayExactAnimatorState(
            entity.Identity.Id, PlayerDeathAnim.Death);
        // Debug.Log("[FAKEDEATH] pose held id=" + entity.Identity.Id);
    }

    /// <summary>Release the fake-death latch (wake-up, real death, or a fresh spawn reusing the id).</summary>
    public static void ClearFakeDeathPose(Entity entity = null)
    {
        if (entity == null || IsFakeDeathPoseHeld(entity))
            _fakeDeathPoseEntityId = 0;
    }

    public static void PlayRevive(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return;

        ClearFakeDeathPose(entity);
        entity.SetDead(false);
        if (entity is UserEntity user)
            user.ClearAttackVisual();
        if (entity is PlayerEntity revivedHero)
        {
            revivedHero.IsAttack = false;
            revivedHero.LastAtkAnimation = null;
        }

        float duration = ControllerPacketServices.Animations.PlayExactAnimatorState(
            entity.Identity.Id, PlayerDeathAnim.Rebirth);
        if (entity is UserEntity revived)
            revived.BeginAttackVisual(duration);
        // Debug.Log(user.LogTag + " Visual.Revive duration=" + duration);
    }
}
