using UnityEngine;

[System.Serializable]
public abstract partial class EffectPlacement
{
    public abstract bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition);

}
