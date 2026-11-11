using UnityEngine;

public sealed partial class ItemDropClickAreaService
{
    void FitClickArea(ItemEntity entity, Transform clickGo, SphereCollider sphere)
    {
        if (!TryGetDropVisualBounds(entity.transform, out Bounds worldBounds))
        {
            clickGo.localPosition = Vector3.zero;
            sphere.center = Vector3.zero;
            sphere.radius = WorldToLocalRadius(clickGo, ClickRadiusMin);
            return;
        }

        clickGo.localPosition = entity.transform.InverseTransformPoint(worldBounds.center);
        float visualRadius = Mathf.Max(worldBounds.extents.x, worldBounds.extents.y, worldBounds.extents.z);
        sphere.center = Vector3.zero;
        sphere.radius = WorldToLocalRadius(clickGo, visualRadius + ClickPadMeters);
    }

    static float WorldToLocalRadius(Transform clickGo, float worldRadius)
    {
        float lossy = Mathf.Max(0.001f, clickGo.lossyScale.x);
        return Mathf.Max(ClickRadiusMin, worldRadius) / lossy;
    }

    bool TryGetDropVisualBounds(Transform root, out Bounds bounds)
    {
        bounds = default;
        Transform dropMesh = root.Find("DropMesh");
        Transform measure = dropMesh != null ? dropMesh : root;
        bool any = false;

        MeshFilter[] filters = measure.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < filters.Length; i++)
        {
            MeshFilter filter = filters[i];
            if (filter == null || filter.sharedMesh == null || _layers.IsFxTransform(filter.transform))
                continue;
            Encapsulate(ref bounds, ref any, TransformAabb(filter.transform, filter.sharedMesh.bounds));
        }

        SkinnedMeshRenderer[] skins = measure.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < skins.Length; i++)
        {
            SkinnedMeshRenderer skin = skins[i];
            if (skin == null || skin.sharedMesh == null || _layers.IsFxTransform(skin.transform))
                continue;
            Encapsulate(ref bounds, ref any, TransformAabb(skin.transform, skin.sharedMesh.bounds));
        }

        return any;
    }

    static void Encapsulate(ref Bounds bounds, ref bool any, Bounds add)
    {
        if (!any)
        {
            bounds = add;
            any = true;
            return;
        }

        bounds.Encapsulate(add);
    }

    static Bounds TransformAabb(Transform t, Bounds localBounds)
    {
        Vector3 center = t.TransformPoint(localBounds.center);
        Vector3 ext = localBounds.extents;
        Vector3 axisX = t.TransformVector(new Vector3(ext.x, 0f, 0f));
        Vector3 axisY = t.TransformVector(new Vector3(0f, ext.y, 0f));
        Vector3 axisZ = t.TransformVector(new Vector3(0f, 0f, ext.z));
        Vector3 worldExt = new Vector3(
            Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
            Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
            Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z));
        return new Bounds(center, worldExt * 2f);
    }
}
