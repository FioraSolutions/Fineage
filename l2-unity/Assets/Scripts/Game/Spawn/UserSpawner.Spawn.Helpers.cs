using System;
using UnityEngine;

public sealed partial class UserSpawner
{
    static void ResetRuntime(UserEntity user)
    {
        if (user == null)
            return;

        EntitySpawnShared.ResetNpcRuntimeForSpawn(user);
        user.ClearAttackVisual();
        NetworkAnimationController animation = user.GetAnimatorController();
        if (animation != null)
            animation.enabled = true;
    }
}
