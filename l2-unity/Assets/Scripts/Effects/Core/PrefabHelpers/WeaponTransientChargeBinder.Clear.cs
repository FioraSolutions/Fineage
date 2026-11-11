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

    static bool TryGetBladeEndpoints(
        Transform weapon,
        out Vector3 bladeBase,
        out Vector3 tip)
    {
        bladeBase = Vector3.zero;
        tip = Vector3.zero;
        if (weapon == null)
        {
            return false;
        }

        Transform tipMarker = FindNamedChild(weapon, SwordTipName);
        Transform baseMarker = FindNamedChild(weapon, SwordBaseName);
        if (tipMarker == null || baseMarker == null)
        {
            return false;
        }

        // New weapon prefabs use Sword_Tip as the blade end. Some legacy
        // SwordSetup prefabs swapped the names, so prefer the marker farther
        // from the attachment pivot and use the current convention on ties.
        float tipDistance = (tipMarker.position - weapon.position).sqrMagnitude;
        float baseDistance = (baseMarker.position - weapon.position).sqrMagnitude;
        bool namesAreSwapped = baseDistance > tipDistance + 1e-8f;
        tip = namesAreSwapped ? baseMarker.position : tipMarker.position;
        bladeBase = namesAreSwapped ? tipMarker.position : baseMarker.position;
        return true;
    }

    static Vector3 Abs(Vector3 value)
    {
        return new Vector3(
            Mathf.Abs(value.x),
            Mathf.Abs(value.y),
            Mathf.Abs(value.z));
    }

    static Transform FindNamedChild(Transform root, string childName)
    {
        if (root == null)
        {
            return null;
        }

        Transform direct = root.Find(childName);
        if (direct != null)
        {
            return direct;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null && children[i].name == childName)
            {
                return children[i];
            }
        }

        return null;
    }
}
