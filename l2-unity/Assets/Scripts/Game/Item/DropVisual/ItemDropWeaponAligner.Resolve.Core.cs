using UnityEngine;

public sealed partial class ItemDropWeaponAligner
{
    public ItemDropWeaponAligner(ItemDropGrpCatalog grp)
    {
        _grp = grp;
    }

    public bool AlignBladeAlong(Transform root, Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude < 1e-12f)
            return false;
        if (!TryGetBladeMarkers(root, out Transform bladeBase, out Transform bladeTip))
            return false;

        Vector3 blade = bladeTip.position - bladeBase.position;
        if (blade.sqrMagnitude < 1e-12f)
            return false;

        root.rotation = Quaternion.FromToRotation(blade.normalized, worldDir.normalized) * root.rotation;
        return true;
    }

    public bool AlignBladeTipDown(Transform root, Vector3 throwDir)
    {
        Vector3 desired = Vector3.down;
        Vector3 flat = new Vector3(throwDir.x, 0f, throwDir.z);
        if (flat.sqrMagnitude > 0.0001f)
            desired = (Vector3.down + flat.normalized * 0.45f).normalized;
        return AlignBladeAlong(root, desired);
    }
}
