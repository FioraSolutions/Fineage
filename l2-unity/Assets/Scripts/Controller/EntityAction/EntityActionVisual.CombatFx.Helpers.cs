using UnityEngine;

public partial class EntityActionVisual
{
    static void PlayUserDeath(UserEntity user, bool alreadyCorpse)
    {
        if (user == null || user.Identity == null || ControllerPacketServices.Animations == null)
            return;

        user.ClearAttackVisual();
        ControllerPacketServices.Animations.PlayExactAnimatorState(
            user.Identity.Id, PlayerDeathAnim.Death, alreadyCorpse);
        // Debug.Log(user.LogTag + " Visual.Death alreadyCorpse=" + alreadyCorpse);
    }

    public static void PlayRevive(Entity entity)
    {
        UserEntity user = entity as UserEntity;
        if (user == null || user.Identity == null || ControllerPacketServices.Animations == null)
            return;

        user.SetDead(false);
        user.ClearAttackVisual();
        float duration = ControllerPacketServices.Animations.PlayExactAnimatorState(
            user.Identity.Id, PlayerDeathAnim.Rebirth);
        user.BeginAttackVisual(duration);
        // Debug.Log(user.LogTag + " Visual.Revive duration=" + duration);
    }
}
