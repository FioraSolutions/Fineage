using UnityEngine;

public sealed partial class ItemDropClickAreaService
{
    public ItemDropClickAreaService(ItemDropLayerService layers)
    {
        _layers = layers;
    }

    public void Refresh(ItemEntity entity)
    {
        if (entity == null)
            return;
        EnsureClickCollider(entity);
        _layers.ApplyDropItemLayer(entity.gameObject);
    }

    public void SitOnGround(ItemEntity item)
    {
        if (item == null)
            return;
        Transform dropMesh = item.transform.Find("DropMesh");
        if (dropMesh != null)
            SitMeshOnGround(dropMesh.gameObject);
        else
            SitMeshOnGround(item.gameObject);
    }

    public void SitMeshOnGround(GameObject visual)
    {
        if (visual == null)
            return;

        MeshFilter filter = visual.GetComponentInChildren<MeshFilter>();
        Mesh mesh = filter != null ? filter.sharedMesh : null;
        if (mesh == null)
            return;

        Bounds world = TransformAabb(filter.transform, mesh.bounds);
        Transform anchor = visual.transform.parent != null ? visual.transform.parent : visual.transform;
        visual.transform.position += Vector3.up * (anchor.position.y - world.min.y);
    }

    void EnsureClickCollider(ItemEntity entity)
    {
        Transform existing = entity.transform.Find("ClickArea");
        GameObject clickGo = existing != null ? existing.gameObject : new GameObject("ClickArea");
        if (existing == null)
            clickGo.transform.SetParent(entity.transform, false);

        SphereCollider sphere = clickGo.GetComponent<SphereCollider>();
        if (sphere == null)
            sphere = clickGo.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        FitClickArea(entity, clickGo.transform, sphere);
    }
}
