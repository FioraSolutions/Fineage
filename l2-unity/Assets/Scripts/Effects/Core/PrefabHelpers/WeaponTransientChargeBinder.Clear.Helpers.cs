using UnityEngine;

public sealed partial class WeaponTransientChargeBinder
{
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
