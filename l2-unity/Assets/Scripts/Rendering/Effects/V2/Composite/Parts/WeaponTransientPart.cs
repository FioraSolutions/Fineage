using UnityEngine;


[System.Serializable]
public sealed class WeaponTransientPart : IndependentEffectPart
{
    public override bool UsesWeaponBoneRotation => true;

    public override void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
        base.OnAfterSpawn(instance, context);
        if (instance == null)
        {
            return;
        }

        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localPosition = Vector3.zero;
    }
}
