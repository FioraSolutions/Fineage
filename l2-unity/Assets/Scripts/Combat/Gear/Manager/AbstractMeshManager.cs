using UnityEngine;
using VContainer;

public class AbstractMeshManager : MonoBehaviour
{
    [Inject] private ModelTable _models;

    protected ModelTable Models =>
        _models != null ? _models : TablePacketServices.Models;

    protected GameObject _go;

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

    protected void SetTypeOtherGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        SetParentGo(go, weaponType, leftSlot, allBone);
    }

    void SetParentGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        if (go == null)
            return;

        if (weaponType == WeaponType.none)
            go.transform.SetParent(allBone[0], false);
        else if (weaponType == WeaponType.bow)
            go.transform.SetParent(allBone[1], false);
        else if (weaponType == WeaponType.arrow)
        {
            go.transform.SetParent(allBone[3], false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }
        else if (leftSlot)
            go.transform.SetParent(allBone[2], false);
        else
            go.transform.SetParent(allBone[3], false);
    }

    public void GetDefaultGoWithArmorModel(
        ItemSlot slot,
        out Armor[] defaultArmor,
        out GameObject[] listArmorPiece,
        int raceId)
    {
        slot = ArmorBodySlots.Normalize(slot);
        defaultArmor = CharacterDefaultEquipment.GetDefaultArmorByItemSlot(slot);
        listArmorPiece = InstantiateArmorMeshes(defaultArmor, raceId);
    }

    protected GameObject[] InstantiateArmorMeshes(Armor[] armors, int raceId)
    {
        if (armors == null || armors.Length == 0)
            return System.Array.Empty<GameObject>();

        CharacterRaceAnimation race = CharacterRaceAnimationParser.SafeConvertToRace(raceId);
        ArmorVisualResolver resolver = new ArmorVisualResolver(Models);
        GameObject[] meshes = new GameObject[armors.Length];

        for (int i = 0; i < armors.Length; i++)
        {
            Armor armor = armors[i];
            if (armor == null)
                continue;

            ArmorVisual visual = resolver.Resolve(armor, race);
            if (visual.IsMissing || visual.Parts.Length == 0)
                continue;

            ArmorVisualPart part = visual.Parts[0];
            meshes[i] = CreateArmorMesh(part.Mesh, part.Material);
        }

        return meshes;
    }

    protected GameObject CreateArmorMesh(GameObject baseArmorModel, Material material)
    {
        GameObject mesh = GetOrCreate(baseArmorModel, ObjectType.Armor);
        if (mesh == null)
        {
            Debug.LogWarning("[Gear] CreateArmorMesh pool/instantiate failed base=" +
                (baseArmorModel != null ? baseArmorModel.name : "null"));
            return null;
        }

        if (mesh.GetComponentInChildren<SkinnedMeshRenderer>(true) == null)
        {
            Debug.LogWarning("[Gear] CreateArmorMesh no SkinnedMeshRenderer on " + mesh.name +
                " children=" + mesh.transform.childCount);
            return null;
        }

        L2ActorMeshPolicy.ApplyPaperdollMaterial(mesh, material);
        return mesh;
    }

    protected string GetWeaponModelName(int itemId)
    {
        Weapon weapon = TablePacketServices.Items.GetWeapon(itemId);
        if (weapon == null)
            return "Not_Found_Model_Name";
        return weapon.Weapongrp.Model.Replace("LineageWeapons.", "");
    }
}
