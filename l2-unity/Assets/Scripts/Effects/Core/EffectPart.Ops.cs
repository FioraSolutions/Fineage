using UnityEngine;

public abstract partial class EffectPart : MonoBehaviour
{
    protected internal Vector3 ResolveOwnerWorldPosForShader(Material material)
    {
        if (material != null &&
            material.HasProperty(L2MaterialPropertyCopier.UseOwnerFromShaderTargetId) &&
            material.GetFloat(L2MaterialPropertyCopier.UseOwnerFromShaderTargetId) > 0.5f &&
            TryResolveShaderTargetWorldPos(out Vector3 targetWorldPos))
        {
            return targetWorldPos;
        }

        return ResolveOwnerWorldPos();
    }

    public void SetShaderTargetWorldPosOverride(bool active, Vector3 worldPosition, Transform followTransform = null)
    {
        _shaderTargetWorldPosOverrideActive = active;
        _shaderTargetWorldPosFollowTransform = null;
        if (active)
        {
            _shaderTargetWorldPosOverride = worldPosition;
            if (followTransform != null)
            {
                _shaderTargetWorldPosFollowTransform = followTransform;
                _shaderTargetWorldPosLocal = followTransform.InverseTransformPoint(worldPosition);
            }
        }
    }

    protected internal bool TryResolveShaderTargetWorldPos(out Vector3 worldPosition)
    {
        if (_shaderTargetWorldPosOverrideActive)
        {
            if (_shaderTargetWorldPosFollowTransform != null)
            {
                worldPosition = _shaderTargetWorldPosFollowTransform.TransformPoint(_shaderTargetWorldPosLocal);
                return true;
            }

            worldPosition = _shaderTargetWorldPosOverride;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }

    protected Vector3 ResolveOwnerWorldPosDefault()
    {
        if (DomainPacketServices.LocalPlayer != null)
        {
            return DomainPacketServices.LocalPlayer.transform.position;
        }

        if (OwnerTarget != null)
        {
            return OwnerTarget.position;
        }

        if (FollowTarget != null)
        {
            return FollowTarget.position;
        }

        return transform.position + transform.forward;
    }

    protected float Now()
    {
#if UNITY_EDITOR
        float now = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
        float now = Time.time;
#endif
        return now;
    }
}
