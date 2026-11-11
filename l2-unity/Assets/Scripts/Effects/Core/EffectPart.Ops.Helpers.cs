using UnityEngine;

public abstract partial class EffectPart : MonoBehaviour
{
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
