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
}
