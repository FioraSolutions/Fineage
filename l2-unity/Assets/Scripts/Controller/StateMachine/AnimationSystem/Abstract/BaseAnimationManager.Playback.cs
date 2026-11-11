using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract partial class BaseAnimationManager
{
    public string GetCurrentAnimationName(int objectId)
    {
        if (!_recentAnimationNames.ContainsKey(objectId) ||
            _recentAnimationNames[objectId][0] == null)
        {
            return "";
        }
        return _recentAnimationNames[objectId][0];
    }

    public void PlayerSetAllFloat(int objectId , Dictionary<string, float> floatValues)
    {
        PlayerAnimationController controller = GetPlayerController(objectId);
        controller.SetParametrs(floatValues);
    }

    public void PlayOriginalAnimation(int objectId , string animationName)
    {
        IAnimationController controller = GetPlayerController(objectId);
        if (controller == null)
        {
            return;
        }

        // death / rebirth (no weapon suffix) → CrossFade, not SetBool.
        if (PlayerDeathAnim.TryResolve(animationName, out string bareState))
        {
            DesableLastPlayerAnimationElseTrue(objectId, controller);
            SetRecentName(objectId, bareState);
            controller.SetAnimatorSpeed(1f);
            controller.CrossFadeInFixedTime(bareState, LocomotionCrossFadeSettings.FixedDuration);
            // Debug.Log(
                // $"AnimationManager> start crossfade {bareState} player objectId={objectId} " +
                // $"duration={LocomotionCrossFadeSettings.FixedDuration:F3}s");
            return;
        }

        controller.SetBool(animationName, true);
    }

    public string GetFinalNameAnim(int objectId , string animationName)
    {
        if(animationName.IndexOf("sit") > -1)
        {
            return "sit";
        }

        return animationName + GetEquipAnimName(objectId);
    }

    public string GetEquipAnimName(int objectId)
    {
        Entity entity = GetEntity(objectId);
        if (entity is PlayerEntity player)
            return player.GetEquippedWeaponName();

        UserEntity user = entity as UserEntity;
        if (user != null)
            return user.WeaponAnim;

        return "";
    }

    public string GetLastAnimationName(int objectId)
    {
        if (!_recentAnimationNames.ContainsKey(objectId) ||
            _recentAnimationNames[objectId][1] == null)
        {
            return "";
        }
        return _recentAnimationNames[objectId][1];
    }

    public void StopCurrentAnimation(int objectId, string paramName, string runName = "")
    {
        if (!_animationControllers.ContainsKey(objectId))
        {
            //Debug.LogError($"No animation controller found for object ID: {objectId}");
            return;
        }

        if (!string.IsNullOrEmpty(paramName))
        {
            _animationControllers[objectId].GetController().SetBool(paramName, false);
        }
        else
        {
            // Debug.Log("AnimationManager>StopCurrentAnimation: Non-critical error stopping bool animation");
        }
    }

    protected void DisableLastMonsterAnimationElseTrue(int objectId, IAnimationController controllerAnimator, string animationName)
    {
  
        if (_recentMonsterAnimationNames.ContainsKey(objectId)) 
        {
            string[] arrAnim = _recentMonsterAnimationNames[objectId];

            if (arrAnim[0] != null && controllerAnimator.GetBool(arrAnim[0]))
            {
                // Debug.Log($"MonsterAnimation> Stopping previous animation {arrAnim[0]} for monster/object {objectId}");
                controllerAnimator.SetBool(arrAnim[0], false);
            }
        }
    }

    protected void DesableLastPlayerAnimationElseTrue(int objectId, IAnimationController controller)
    {
        if (!string.IsNullOrEmpty(GetCurrentAnimationName(objectId)))
        {
            string currentAnimation = GetCurrentAnimationName(objectId);
            if (controller.GetBool(currentAnimation))
            {
                controller.SetBool(currentAnimation, false);
                //Debug.Log($"AnimationManager> stop name player  {_player.name} name animation {currentAnimation}");
            }

        }
    }

    protected bool PlayLocomotionCrossFade(
        IAnimationController controller,
        string stateName,
        float? fixedDuration = null)
    {
        return PlayerLocomotionCrossFade.TryPlay(controller, stateName, fixedDuration);
    }

    protected void SetRecentName(int objectId, string animationName)
    {
        _recentAnimationNames[objectId] = _recentAnimationNames.TryGetValue(objectId, out var arrAnim)
            ? new[] { animationName, arrAnim[0] }
            : new[] { animationName, null };
    }

    protected void SetMonsterRecentName(int objId, string animationName)
    {
        if (!_recentMonsterAnimationNames.TryGetValue(objId, out var arrAnim))
        {
            arrAnim = new string[2];
            _recentMonsterAnimationNames[objId] = arrAnim;
        }

        arrAnim[1] = arrAnim[0];
        arrAnim[0] = animationName;
    }
}
