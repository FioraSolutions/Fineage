using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
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
