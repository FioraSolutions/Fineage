using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    protected object LoadMesh(EquipmentCategory category, params int[] itemIds)
    {
        try
        {
            switch (category)
            {
                case EquipmentCategory.Weapon:
                {
                    int weaponId = itemIds[0];
                    Weapon weapon = TablePacketServices.Items.GetWeapon(weaponId);
                    GameObject goWeapon = LoadWeapon(weapon, weaponId);
                    if (weapon != null)
                        ObjectPoolPacketServices.Pool?.AddPrefabToPool(ObjectType.Weapon, goWeapon);
                    return goWeapon;
                }
                case EquipmentCategory.EtcItem:
                {
                    int etcId = itemIds[0];
                    EtcItem etcItem = TablePacketServices.Items.GetEtcItem(etcId);
                    GameObject goEtcItem = LoadEtcItem(etcItem, etcId);
                    if (goEtcItem != null)
                        ObjectPoolPacketServices.Pool?.AddPrefabToPool(ObjectType.Arrow, goEtcItem);
                    return goEtcItem;
                }
                default:
                    return null;
            }
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    protected GameObject CreateCopy(GameObject originalGameObject, string name, ObjectType type)
    {
        if (originalGameObject == null)
            return null;

        _go = GetOrCreate(originalGameObject, type);
        _go.SetActive(false);
        _go.transform.name = name;
        return _go;
    }

    protected GameObject GetOrCreate(GameObject originalGameObject, ObjectType type)
    {
        if (originalGameObject == null)
            return null;

        if (ObjectPoolPacketServices.Pool != null)
        {
            ObjectPoolPacketServices.Pool.AddPrefabToPool(type, originalGameObject, count: 1);
            GameObject pooled = ObjectPoolPacketServices.Pool.SpawnFromPool(type, originalGameObject);
            if (pooled != null)
                return pooled;
        }

        return Object.Instantiate(originalGameObject);
    }

    GameObject LoadWeapon(Weapon weapon, int weaponId)
    {
        if (weapon == null)
            return null;
        return Models.GetWeaponById(weaponId);
    }

    GameObject LoadEtcItem(EtcItem etcItem, int etcId)
    {
        if (etcItem == null)
            return null;
        return Models.GetEtcById(etcId);
    }

    protected void SetType(WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        SetParentGo(_go, weaponType, leftSlot, allBone);
    }
}
