using UnityEngine;

public sealed partial class WeaponTransientChargeBinder
{
    void EnsureAuthoredVelocityCache()
    {
        int count = _travelingEmitters != null
            ? Mathf.Min(MaxTravelingEmitters, _travelingEmitters.Length)
            : 0;
        if (_authoredVelocityX != null && _authoredVelocityX.Length == count)
        {
            return;
        }

        _authoredVelocityX = new Vector4[count];
        _hasAuthoredVelocityX = new bool[count];
        for (int i = 0; i < count; i++)
        {
            ParticleGroupV2 group = _travelingEmitters[i];
            if (group != null &&
                group.TryGetMaterialVector(
                    L2MaterialPropertyCopier.StartVelocityRangeXUcId,
                    out Vector4 range))
            {
                _authoredVelocityX[i] = range;
                _hasAuthoredVelocityX[i] = true;
            }
        }
    }

    public static bool TryMeasureExtraUu(Transform weapon, out float extraUu)
    {
        extraUu = 0f;
        if (weapon == null)
        {
            return false;
        }

        if (TryGetBladeEndpoints(weapon, out Vector3 bladeBase, out Vector3 tip))
        {
            extraUu = Vector3.Distance(tip, bladeBase) * UnityMetersToUu;
            return extraUu > 0.01f;
        }

        MeshFilter filter = weapon.GetComponentInChildren<MeshFilter>();
        if (filter == null || filter.sharedMesh == null)
        {
            return false;
        }

        Vector3 scaledSize = Vector3.Scale(
            filter.sharedMesh.bounds.size,
            Abs(filter.transform.lossyScale));
        float longestAxis = Mathf.Max(scaledSize.x, scaledSize.y, scaledSize.z);
        extraUu = longestAxis * UnityMetersToUu;
        return extraUu > 0.01f;
    }

    void AlignLocalPositiveXToBlade(Transform weapon)
    {
        if (!TryGetBladeEndpoints(weapon, out Vector3 bladeBase, out Vector3 tip))
        {
            return;
        }

        Vector3 worldDirection = tip - bladeBase;
        if (worldDirection.sqrMagnitude < 1e-8f)
        {
            return;
        }

        Transform basis = transform.parent != null ? transform.parent : weapon;
        Vector3 localDirection = basis != null
            ? basis.InverseTransformDirection(worldDirection.normalized)
            : worldDirection.normalized;
        transform.localRotation = Quaternion.FromToRotation(
            Vector3.right,
            localDirection);
    }
}
