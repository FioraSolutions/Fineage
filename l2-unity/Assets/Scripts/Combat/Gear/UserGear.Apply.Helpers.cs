using UnityEngine;

public partial class UserGear : Gear
{
    public void EquipFace(GameObject facePiece)
    {
        if (Face != null)
            Destroy(Face);

        Face = facePiece;
        if (Face != null)
            Face.transform.SetParent(_container.transform, false);

        _skinnedMeshSync?.SyncMesh();
    }

    public void OnDestroyGameObject(GameObject go)
    {
        IPoolManager pool = ObjectPoolPacketServices.Pool;
        if (pool != null && pool.ReturnToPool(ObjectType.Armor, go))
            return;
        Destroy(go);
    }

    void OnSyncMesh()
    {
        _skinnedMeshSync?.SyncMesh();
    }

    public void AddUserGearLink(GameObject face, GameObject hair1, GameObject hair2) {
        Face = face;
        Hair1 = hair1;
        Hair2 = hair2;
    }
}
