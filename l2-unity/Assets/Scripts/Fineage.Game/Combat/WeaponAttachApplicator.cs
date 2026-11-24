using UnityEngine;

/// <summary>
/// Applies a resolved mount to a weapon instance at a skeleton socket (UE2 identity-at-socket contract).
/// </summary>
public static class WeaponAttachApplicator
{
    public static void Apply(GameObject weaponRoot, Transform socket, in WeaponAttachMount mount)
    {
        if (weaponRoot == null || socket == null)
            return;

        Transform t = weaponRoot.transform;
        t.SetParent(socket, false);

        if (mount.SnapIdentityAtSocket)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }

        NormalizeWeaponWorldScale(t);
        RepairSubmeshMaterials(weaponRoot);
        L2TwoSidedMaterials.BindHierarchy(weaponRoot, logIfPropsMissing: true, "weapon attach");

        if (mount.MirrorScaleX)
        {
            Vector3 scale = t.localScale;
            float ax = Mathf.Abs(scale.x);
            if (ax < 0.0001f)
                ax = 1f;
            t.localScale = new Vector3(-ax, scale.y, scale.z);
            WeaponAttachMirrorMaterials.ApplyForNegativeScaleX(weaponRoot);
        }

        if (mount.DoubleSidedMaterials)
            WeaponAttachMirrorMaterials.ForceDoubleSidedInstanceMaterials(weaponRoot);
    }

    public static void NormalizeWeaponWorldScale(Transform weapon)
    {
        Vector3 lossy = weapon.lossyScale;
        float sx = Mathf.Abs(lossy.x), sy = Mathf.Abs(lossy.y), sz = Mathf.Abs(lossy.z);
        if (sx <= 0f || sy <= 0f || sz <= 0f)
            return;
        if (Mathf.Approximately(sx, 1f) && Mathf.Approximately(sy, 1f) && Mathf.Approximately(sz, 1f))
            return;

        Vector3 local = weapon.localScale;
        float signX = Mathf.Sign(local.x == 0f ? 1f : local.x);
        weapon.localScale = new Vector3(signX * (Mathf.Abs(local.x) / sx), local.y / sy, local.z / sz);
    }

    public static void RepairSubmeshMaterialsPublic(GameObject go) => RepairSubmeshMaterials(go);

    static void RepairSubmeshMaterials(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            int submeshes = SubmeshCount(renderer);
            Material[] current = renderer.sharedMaterials;
            if (submeshes <= 1 || current == null || current.Length >= submeshes)
                continue;

            Material fallback = null;
            for (int m = 0; m < current.Length; m++)
            {
                if (current[m] != null)
                {
                    fallback = current[m];
                    break;
                }
            }
            if (fallback == null)
                continue;

            var filled = new Material[submeshes];
            for (int m = 0; m < submeshes; m++)
                filled[m] = m < current.Length && current[m] != null ? current[m] : fallback;
            renderer.sharedMaterials = filled;
        }
    }

    static int SubmeshCount(Renderer renderer)
    {
        if (renderer is SkinnedMeshRenderer skin && skin.sharedMesh != null)
            return skin.sharedMesh.subMeshCount;
        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
            return filter.sharedMesh.subMeshCount;
        return 0;
    }

}