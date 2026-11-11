using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
    public void PlayMonsterAnimation(int objectId, string animationName)
    {
        PlayMonsterAnimation(objectId, animationName, null);
    }

    public void PlayMonsterAnimation(int objectId, string animationName, float? fixedDuration)
    {
        IAnimationController controller = GetMonsterController(objectId);
        if (controller == null)
        {
            // Debug.LogWarning(
                // $"[ANIM] PlayMonster MISS id={objectId} request='{animationName}' reason=no_controller");
            return;
        }

        if (MonsterAnim.TryResolve(animationName, out string stateName, out MonsterAnimState family))
        {
            SetMonsterRecentName(objectId, stateName);
            float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
            bool already = PlayerLocomotionCrossFade.IsAlreadyPlaying(controller, stateName);
            bool played = MonsterCrossFade.TryPlay(controller, stateName, family, duration);

            return;
        }

        SetMonsterRecentName(objectId, animationName);
        float rawDuration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(animationName, controller);
        // Debug.Log(
            // $"[ANIM] PlayMonster RAW id={objectId} request='{animationName}' duration={rawDuration:F3}s");
        MonsterCrossFade.TryPlayRaw(controller, animationName, rawDuration);
    }

    public void StopMonsterCurrentAnimation(int objectId, string animationName)
    {
        if (GetMonsterController(objectId) is { } controller)
        {
            controller.SetBool(animationName, false);
        }
        else
        {
            // Debug.LogWarning($"AnimationManager->StopMonsterCurrentAnimation: Не критическая ошибка - animator not found for monster {objectId}. Animation: {animationName}");
        }
    }
}
