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

        if (mount.MirrorScaleX)
        {
            Vector3 scale = t.localScale;
            float ax = Mathf.Abs(scale.x);
            if (ax < 0.0001f)
                ax = 1f;
            t.localScale = new Vector3(-ax, scale.y, scale.z);
            InvertMaterialCull(weaponRoot);
        }

        if (mount.DoubleSidedMaterials)
            ForceDoubleSided(weaponRoot);
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

    static void ForceDoubleSided(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            Material[] shared = renderer.sharedMaterials;
            bool needsCopy = false;
            for (int m = 0; m < shared.Length; m++)
            {
                Material mat = shared[m];
                if (mat != null && mat.HasProperty("_Cull") && mat.GetFloat("_Cull") > 0.5f)
                {
                    needsCopy = true;
                    break;
                }
            }
            if (!needsCopy)
                continue;

            var copy = new Material[shared.Length];
            for (int m = 0; m < shared.Length; m++)
            {
                if (shared[m] == null)
                    continue;
                copy[m] = new Material(shared[m]);
                if (copy[m].HasProperty("_Cull"))
                    copy[m].SetFloat("_Cull", 0f);
            }
            renderer.sharedMaterials = copy;
        }
    }

    static void InvertMaterialCull(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            Material[] shared = renderer.sharedMaterials;
            bool needsCopy = false;
            for (int m = 0; m < shared.Length; m++)
            {
                Material mat = shared[m];
                if (mat != null && mat.HasProperty("_Cull"))
                {
                    needsCopy = true;
                    break;
                }
            }
            if (!needsCopy)
                continue;

            var copy = new Material[shared.Length];
            for (int m = 0; m < shared.Length; m++)
            {
                if (shared[m] == null)
                    continue;
                copy[m] = new Material(shared[m]);
                if (copy[m].HasProperty("_Cull"))
                {
                    float cull = copy[m].GetFloat("_Cull");
                    // 1 = Back, 2 = Front in URP Lit — flip so mirrored winding stays visible.
                    copy[m].SetFloat("_Cull", Mathf.Approximately(cull, 2f) ? 1f : 2f);
                }
            }
            renderer.sharedMaterials = copy;
        }
    }
}