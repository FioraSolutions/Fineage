using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public partial class AnimationManager
{
    public void PlayerAnimationTrigger(int objectId , string animationName , bool useFinalName = true)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
        {
            if (SkillAnimationRunner.Trace)
                Debug.Log("[SUMMON_ANIM] trigger skip controller=null name=" + animationName + " objectId=" + objectId);
            return;
        }

        if (SkillAnimationRunner.Trace)
            Debug.Log("[SUMMON_ANIM] trigger name=" + animationName + " useFinal=" + useFinalName + " objectId=" + objectId);

        if(useFinalName) animationName = GetFinalNameAnim(objectId, animationName);

        Entity entity = GetEntity(objectId);
        DesableLastPlayerAnimationElseTrue(objectId , controller);

        bool isMagicShotTrigger = IsMagicShotTrigger(animationName);
        if (isMagicShotTrigger)
        {
            MagicCastData castData = entity != null ? entity.GetMagicCastData() : null;
            if (!MagicShotCoordinator.TryStartShot(objectId, castData, SHOT_SOURCE_RUNNER, out string coordinatorMessage))
            {
                // Debug.Log($"{coordinatorMessage} trigger='{animationName}' action=skip");
                return;
            }

            // Debug.Log($"{coordinatorMessage} trigger='{animationName}' action=run");
        }

        if (IsMagicCastTrigger(animationName))
        {
            // Prevent stale cast triggers from keeping previous cast states alive.
            controller.ResetAnimationTrigger(CAST_TRIGGER_MID);
            controller.ResetAnimationTrigger(CAST_TRIGGER_END);
            controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT);
            controller.ResetAnimationTrigger(CAST_TRIGGER_END_2P);
            controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT_2P);
        }

        // Skill runner / async path — same CrossFade for jatk / SpAtk as PlayAnimationTrigger.
        if (TryPlayCombatCrossFade(objectId, controller, entity, animationName))
        {
            return;
        }

        controller.ToggleAnimationTrigger(animationName);
        SetRecentName(objectId, animationName);

        // Debug.Log($"AnimationManager> start Async AnimationTrigger(  {entity} animation  {animationName}");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsMagicShotTrigger(animationName) && entity != null)
        {
            MagicCastData cd = entity.GetMagicCastData();
            if (cd != null)
            {
                float globalSinceCast = Time.time - cd.StartTime;
                // Debug.Log(
                    // "[MAGIC_PROJECTILE_SYNC] AnimatorTriggerMagicShot " +
                    // $"globalSinceCast={globalSinceCast:F3}s serverShoot={cd.serverTimeToShoot:F3}s " +
                    // $"deltaToShoot={globalSinceCast - cd.serverTimeToShoot:F3}s trigger={animationName}");
            }
        }
#endif
    }

    private static void ResetMagicCastTriggers(IAnimationController controller)
    {
        if (controller == null)
        {
            return;
        }

        controller.ResetAnimationTrigger(CAST_TRIGGER_MID);
        controller.ResetAnimationTrigger(CAST_TRIGGER_END);
        controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT);
        controller.ResetAnimationTrigger(CAST_TRIGGER_END_2P);
        controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT_2P);
    }
}
