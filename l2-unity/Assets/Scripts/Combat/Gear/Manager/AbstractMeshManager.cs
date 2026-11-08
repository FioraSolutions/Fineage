using UnityEngine;
using VContainer;
using static ModelTable;
using static UnityEngine.EventSystems.EventTrigger;


public class AbstractMeshManager : MonoBehaviour
{
    [Inject] private ModelTable _models;

    protected ModelTable Models =>
        _models != null ? _models : TablePacketServices.Models;

    protected GameObject _go;

    protected object LoadMesh(EquipmentCategory category, params int[] itemIds) {
        try {
            switch (category) {
                case EquipmentCategory.Weapon:
                    int weaponId = itemIds[0];
                    Weapon weapon = TablePacketServices.Items.GetWeapon(weaponId);
                    GameObject goWeapon = LoadWeapon(weapon, weaponId);
                    if (weapon != null) ObjectPoolPacketServices.Pool?.AddPrefabToPool(ObjectType.Weapon, goWeapon);
                    return goWeapon;
                case EquipmentCategory.Armor:
                    int aramorId = itemIds[0];
                    int raceId = itemIds[1];
                    Armor armor = TablePacketServices.Items.GetArmor(aramorId);
                    ModelTable.L2ArmorPiece goArmor = LoadArmor(armor, CharacterRaceAnimationParser.SafeConvertToRace(raceId));
                    if (goArmor == null)
                        return null;
                    if (armor != null && goArmor.baseArmorModel != null)
                        ObjectPoolPacketServices.Pool?.AddPrefabToPool(ObjectType.Armor, goArmor.baseArmorModel);
                    return goArmor;
                case EquipmentCategory.FullArmor:
                    int fullArmorId = itemIds[0];
                    int fullArmorRaceId = itemIds[1];
                    Armor armorModel = TablePacketServices.Items.GetArmor(fullArmorId);
                    ModelTable.L2ArmorPiece fullGoArmor = LoadArmor(armorModel, CharacterRaceAnimationParser.SafeConvertToRace(fullArmorRaceId));
                    if (fullGoArmor == null)
                        return null;
                    if (armorModel != null)
                        AddPrefabToList(ObjectType.Armor, fullGoArmor.baseAllModels);
                    return fullGoArmor;
                case EquipmentCategory.EtcItem:
                    int etcId = itemIds[0];
                    EtcItem etcItem = TablePacketServices.Items.GetEtcItem(etcId);
                    GameObject goEtcItem = LoadEtcItem(etcItem, etcId);
                    if (goEtcItem != null) ObjectPoolPacketServices.Pool?.AddPrefabToPool(ObjectType.Arrow, goEtcItem);
                    return goEtcItem;

                default:
                    // Debug.LogWarning("Unknown equipment category");
                    return null;
            }
        } catch (System.Exception) {
            return null;
        }
    }

    protected GameObject CreateCopy(GameObject originalGameObject , string name , ObjectType type)
    {
        // Instantiating weapon
        if(originalGameObject != null)
        {
            _go = GetOrCreate(originalGameObject , type);
            _go.SetActive(false);
            _go.transform.name = name;

            return _go;
        }
        return null;
    }

    private void AddPrefabToList(ObjectType type , GameObject[] allModels)
    {
        if(allModels != null)
        {
            for (int i = 0; i < allModels.Length; i++)
            {
                GameObject go = allModels[i];
                ObjectPoolPacketServices.Pool?.AddPrefabToPool(type, go);
            }
        }
    }
    protected GameObject GetOrCreate(GameObject originalGameObject , ObjectType type)
    {
        if (originalGameObject == null)
            return null;

        if (ObjectPoolPacketServices.Pool != null)
        {
            // Pool may not know this prefab yet (fullarmor robe meshes). Grow or fall back.
            ObjectPoolPacketServices.Pool.AddPrefabToPool(type, originalGameObject, count: 1);
            GameObject pooled = ObjectPoolPacketServices.Pool.SpawnFromPool(type, originalGameObject);
            if (pooled != null)
                return pooled;
        }

        return GameObject.Instantiate(originalGameObject);
    }

    private GameObject LoadWeapon(Weapon weapon , int weaponId)
    {
        if (weapon == null)
        {
            // Debug.LogWarning($"Could find weapon {weaponId}");
            return null;
        }

        GameObject weaponPrefab = Models.GetWeaponById(weaponId);
        if (weaponPrefab == null)
        {
            // Debug.LogWarning($"Could load prefab for {weaponId}");
            return null;
        }

        return weaponPrefab;
    }

    private GameObject LoadEtcItem(EtcItem etcItem, int etcId)
    {
        if (etcItem == null)
        {
            // Debug.LogWarning($"Could find etc item {etcItem}");
            return null;
        }

        GameObject weaponPrefab = Models.GetEtcById(etcId);
        if (weaponPrefab == null)
        {
            // Debug.LogWarning($"Could load prefab for {etcId}");
            return null;
        }

        return weaponPrefab;
    }

    private L2ArmorPiece LoadArmor(Armor armor, CharacterRaceAnimation raceId)
    {
        if (armor == null)
            return null;

        ArmorVisual visual = new ArmorVisualResolver(Models).Resolve(armor, raceId);
        return visual.IsMissing ? null : visual.LegacyPiece;
    }



    //GetShieldBone
    //GetLeftHandBone
    //GetLeftHandBone
    //GetRightHandBone
    protected void SetType(WeaponType weaponType , bool leftSlot , Transform[] allBone)
    {
        SetParentGo(_go, weaponType, leftSlot, allBone);
    }

    protected void SetTypeOtherGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        SetParentGo(go, weaponType, leftSlot, allBone);
    }
    private void SetParentGo(GameObject go , WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        if (go == null)
        {
            // Debug.LogError("Gear: Not Create GameObject " + weaponType);
            return;
        }

        if (weaponType == WeaponType.none)
        {
            go.transform.SetParent(allBone[0], false);
        }
        else if (weaponType == WeaponType.bow)
        {
            go.transform.SetParent(allBone[1], false);
        }
        else if (weaponType == WeaponType.arrow)
        {
            // Expect arrow mesh pivot at nock (reimport). No runtime shaft offset.
            go.transform.SetParent(allBone[3], false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }
        else if (leftSlot)
        {
            go.transform.SetParent(allBone[2], false);
        }
        else
        {
            go.transform.SetParent(allBone[3], false);
        }
    }


    public void GetDefaultGoWithArmorModel(ItemSlot slot, out Armor[] defaultArmor, out GameObject[] listArmorPiece , int raceId)
    {
        //int race = (int)_raceId;
        slot = ArmorBodySlots.Normalize(slot);

        defaultArmor = CharacterDefaultEquipment.GetDefaultArmorByItemSlot(slot);
        listArmorPiece = CopyListMash(slot, defaultArmor, raceId);
    }


    protected GameObject[] CopyListMash(ItemSlot slot , Armor[] defaultArmor , int raceId)
    {
        var armorPiece = GetMeshBaseArmor(slot, defaultArmor, raceId);
        return  CreateCopyOrGetPool(armorPiece, slot);
    }

    public L2ArmorPiece[] GetMeshBaseArmor(ItemSlot slot, Armor[] defaultArmor, int raceId)
    {
        if (defaultArmor == null)
            return System.Array.Empty<L2ArmorPiece>();

        L2ArmorPiece[] baseArmorArr = new L2ArmorPiece[defaultArmor.Length];

        for (int i = 0; i < defaultArmor.Length; i++)
        {
            if (defaultArmor[i] == null)
                continue;
            int baseArmorId = defaultArmor[i].Id;
            baseArmorArr[i] = (L2ArmorPiece)LoadMesh(EquipmentCategory.Armor, baseArmorId, raceId);
        }

        return baseArmorArr;
    }

    protected GameObject[] CreateCopyOrGetPool(L2ArmorPiece[] armorArrPiece, ItemSlot slot)
    {
        if (armorArrPiece == null)
            return System.Array.Empty<GameObject>();

        GameObject[] arrGo = new GameObject[armorArrPiece.Length];
        for (int i = 0; i < armorArrPiece.Length; i++)
        {
            var armorPiece = armorArrPiece[i];
            if (armorPiece == null || armorPiece.baseArmorModel == null || armorPiece.material == null)
                continue;
            arrGo[i] = CreateArmorMesh(armorPiece.baseArmorModel, armorPiece.material);
        }
        return arrGo;
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
        {
            return "Not_Found_Model_Name";
        }
       return  weapon.Weapongrp.Model.Replace("LineageWeapons." , "");

    }

}
