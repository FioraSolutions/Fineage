using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    GameObject LoadWeapon(Weapon weapon, int weaponId, bool leftHand = false)
    {
        if (weapon?.Weapongrp == null)
            return null;
        string model = WeapongrpHandMeshes.ModelForHand(weapon.Weapongrp, leftHand);
        return Models.GetWeapon(model);
    }

    GameObject LoadEtcItem(EtcItem etcItem, int etcId)
    {
        if (etcItem == null)
            return null;
        return Models.GetEtcById(etcId);
    }

}
