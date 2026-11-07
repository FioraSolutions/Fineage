using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGear : UserGear
{
    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = AnimationPacketServices.PlayerAnimation?.transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = AnimationPacketServices.PlayerAnimation?.transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = AnimationPacketServices.PlayerAnimation?.transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

   
    public string GetNameWeapon()
    {
        return _weaponAnim;
    }
}
