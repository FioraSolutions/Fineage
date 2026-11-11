public partial class L2WeaponChargeGroup : BaseEffectGroup, IWeaponEffect
{
    private void TrySetVfxFloat(VisualEffect vfx, string propertyName, float value)
    {
        if (vfx == null || string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        if (vfx.HasFloat(propertyName))
        {
            vfx.SetFloat(propertyName, value);
        }
    }

    private void ApplyMaterialsParams(Renderer renderer, float now, float seed)
    {

        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();


        renderer.GetPropertyBlock(_propBlock);


        Vector4 delayRange = renderer.sharedMaterial.GetVector(InitialDelayRangeID);
        float totalLife = _duration + delayRange.y;

        if (useFixedLifetime)
        {
            _propBlock.SetFloat(StartTimeID, now);
            _propBlock.SetFloat(SeedID, seed);
            _propBlock.SetVector(LifetimeRangeID, new Vector2(_duration, _duration));
            _propBlock.SetFloat(FadeoutStartTimeID, _duration * 0.7f);
            // Debug.Log($"ApplyMaterialsParams fixed: {now}, {seed}, {_duration}, {totalLife * 0.7f}");
        }
        else
        {
            _propBlock.SetFloat(StartTimeID, now);
            _propBlock.SetFloat(SeedID, seed);
            _propBlock.SetVector(LifetimeRangeID, new Vector2(adjustedLife, adjustedLife));
            _propBlock.SetFloat(FadeoutStartTimeID, totalLife * 0.7f);
            // Debug.Log($"ApplyMaterialsParams calc: {now}, {seed}, {adjustedLife}, {totalLife * 0.7f}");
        }


        renderer.SetPropertyBlock(_propBlock);
    }

    private void OnDrawGizmos()
    {
        if (_weaponTransform == null || _swordTip == null) return;

        // Линия вдоль лезвия (Синяя)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_weaponTransform.position, _swordTip.position);

        // Сфера в основании (Зеленая)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_weaponTransform.position, 0.02f);

        // Сфера на кончике (Красная)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_swordTip.position, 0.02f);
    }
}
