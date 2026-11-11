using UnityEngine;

public sealed partial class WeaponTransientChargeBinder
{
    public static bool MatchesChargeEffectName(string classOrPrefabName)
    {
        if (string.IsNullOrEmpty(classOrPrefabName))
        {
            return false;
        }

        // Native selects OnSticks before applying the mesh-extra velocity branch.
        // OnBooks uses its authored playback and must not receive this override.
        return classOrPrefabName.EndsWith(
            "_stick",
            System.StringComparison.OrdinalIgnoreCase);
    }

    public void ConfigureBeforePlay(Transform owner)
    {
        _weapon = owner != null ? owner : transform.parent;
        Apply();
    }

    public void Apply()
    {
        Transform weapon = ResolveWeapon();
        AlignLocalPositiveXToBlade(weapon);
        BindAuthoredPlayback();
        EnsureAuthoredVelocityCache();
        bool measured = TryMeasureExtraUu(weapon, out float extraUu);
        bool overwrite = measured && extraUu > ExtraOverwriteThresholdUu;

        int count = _travelingEmitters != null
            ? Mathf.Min(MaxTravelingEmitters, _travelingEmitters.Length)
            : 0;
        for (int i = 0; i < count; i++)
        {
            ParticleGroupV2 group = _travelingEmitters[i];
            if (group == null || !_hasAuthoredVelocityX[i])
            {
                continue;
            }

            Vector4 range = overwrite
                ? new Vector4(extraUu, extraUu, 0f, 0f)
                : _authoredVelocityX[i];
            group.SetRuntimeMaterialVector(
                L2MaterialPropertyCopier.StartVelocityRangeXUcId,
                range);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_debugLogs)
            {
                // Debug.Log(
                    // "[WeaponTransientChargeBinder] '" + name + "' emitter='" + group.name +
                    // "' extra=" + extraUu.ToString("0.###") +
                    // " overwrite=" + overwrite +
                    // " velX=(" + range.x.ToString("0.###") + "," +
                    // range.y.ToString("0.###") + ")" +
                    // " weapon='" + (weapon != null ? weapon.name : "null") + "'",
                    // this);
            }
#endif
        }
    }
}
