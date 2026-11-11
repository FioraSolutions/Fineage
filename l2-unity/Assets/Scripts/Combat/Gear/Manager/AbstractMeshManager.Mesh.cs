using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    protected void SetTypeOtherGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        SetParentGo(go, weaponType, leftSlot, allBone);
    }

    void SetParentGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        if (go == null)
            return;

        // Shield and EquipShield (WeaponType.none) both bind to Shield_L_Bone.
        if (weaponType == WeaponType.none || weaponType == WeaponType.shield)
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
