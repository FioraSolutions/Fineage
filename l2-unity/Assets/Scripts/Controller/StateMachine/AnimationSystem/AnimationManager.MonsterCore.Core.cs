using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
    public void PlayCreatureAnimation(int objectId, string animationName)
    {
        PlayCreatureAnimation(objectId, animationName, null);
    }

    public void PlayCreatureAnimation(int objectId, string animationName, float? fixedDuration)
    {
        IAnimationController controller = GetCreatureController(objectId);
        if (controller == null)
            return;

        // MonsterAnim = shared Npc_Basic / Monster_Basic state name catalog.
        if (MonsterAnim.TryResolve(animationName, out string stateName, out MonsterAnimState family))
        {
            SetMonsterRecentName(objectId, stateName);
            float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
            MonsterCrossFade.TryPlay(controller, stateName, family, duration);
            return;
        }

        SetMonsterRecentName(objectId, animationName);
        float rawDuration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(animationName, controller);
        MonsterCrossFade.TryPlayRaw(controller, animationName, rawDuration);
    }

    public void StopCreatureCurrentAnimation(int objectId, string animationName)
    {
        if (GetCreatureController(objectId) is { } controller)
            controller.SetBool(animationName, false);
    }

    [Obsolete("Use PlayCreatureAnimation")]
    public void PlayMonsterAnimation(int objectId, string animationName) =>
        PlayCreatureAnimation(objectId, animationName);

    [Obsolete("Use PlayCreatureAnimation")]
    public void PlayMonsterAnimation(int objectId, string animationName, float? fixedDuration) =>
        PlayCreatureAnimation(objectId, animationName, fixedDuration);

    [Obsolete("Use StopCreatureCurrentAnimation")]
    public void StopMonsterCurrentAnimation(int objectId, string animationName) =>
        StopCreatureCurrentAnimation(objectId, animationName);
}
