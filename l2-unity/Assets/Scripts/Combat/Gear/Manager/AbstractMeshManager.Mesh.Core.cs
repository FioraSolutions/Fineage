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
}
