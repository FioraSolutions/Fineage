using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract partial class BaseAnimationManager
{
    protected BaseAnimationManager()
    {
        _animationControllers = new Dictionary<int, AnimationModel>();
        _recentAnimationNames = new Dictionary<int, string[]>();
        _recentMonsterAnimationNames = new Dictionary<int, string[]>();
        _tcsMap = new();
    }

    public void RegisterController(int objectId, IAnimationController controller , Entity entity)
    {
        _animationControllers[objectId] = new AnimationModel(controller , entity);
        controller.SetInt(AnimatorUtils.OBJECT_ID, objectId);
        AnimationEventForwarder.BindAnimator(controller);
    }

    public void UnregisterController(int objectId)
    {
        _animationControllers.Remove(objectId);
        _recentAnimationNames.Remove(objectId);
        _recentMonsterAnimationNames.Remove(objectId);
        _tcsMap.Remove(objectId);
    }

    public bool IsPlayerController(int objectId)
    {
        return _animationControllers.ContainsKey(objectId) &&
               _animationControllers[objectId].GetType() == typeof(PlayerAnimationController);
    }

    public bool IsMonsterController(int objectId)
    {
        return _animationControllers.ContainsKey(objectId) &&
               _animationControllers[objectId].GetType() == typeof(NetworkAnimationController);
    }

    public PlayerAnimationController GetPlayerController(int objectId)
    {
        if (IsPlayerController(objectId))
        {
            return _animationControllers[objectId].GetController() as PlayerAnimationController;
        }
        return null;
    }

    public IAnimationController GetRegisteredController(int objectId)
    {
        AnimationModel model = GetModel(objectId);
        return model != null ? model.GetController() : null;
    }

    public AnimationEventsBase GetAnimationEvents(int objectId)
    {
        if (_animationControllers.ContainsKey(objectId))
        {
            return _animationControllers[objectId].GetEvents();
        }
        return null;
    }

    public AnimationModel GetModel(int objectId)
    {
        AnimationModel model;
        return _animationControllers.TryGetValue(objectId, out model) ? model : null;
    }

    public Entity GetEntity(int objectId)
    {
        AnimationModel model = GetModel(objectId);
        return model != null ? model.GetEntity() : null;
    }

    public NetworkAnimationController GetMonsterController(int objectId)
    {
        if (IsMonsterController(objectId))
        {
            return _animationControllers[objectId].GetController() as NetworkAnimationController;
        }
        return null;
    }
}
