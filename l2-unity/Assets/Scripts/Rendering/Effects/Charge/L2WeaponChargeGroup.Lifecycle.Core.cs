using UnityEngine;
using UnityEngine.VFX;

public partial class L2WeaponChargeGroup : BaseEffectGroup, IWeaponEffect
{
    public void SetWeapon(Transform weaponTransform)
    {
        _weaponTransform = weaponTransform;

        if (_weaponTransform != null)
        {
            _swordTip = weaponTransform.Find("Sword_Tip");
            if (_swordTip == null)
            {
                foreach (Transform child in weaponTransform.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "Sword_Tip")
                    {
                        _swordTip = child;
                        break;
                    }
                }
            }

            if (_swordTip == null)
                return;

            float currentLength = Vector3.Distance(_weaponTransform.position, _swordTip.position);

           float maxMultiplierFor2Meters = 2.0f / ReferenceWeaponLength;
           _timeMultiplier= Mathf.Min(maxMultiplierFor2Meters, currentLength / ReferenceWeaponLength);
           
           // Если оружие короче эталонного (timeMultiplier < 1), 
           // используем квадратичное уменьшение, чтобы эффект быстрее проигрывался на коротком оружии
           if (_timeMultiplier < 1.0f)
           {
               _timeMultiplier = _timeMultiplier * _timeMultiplier;
           }
           
           // Ограничиваем итоговое время жизни снизу нашим порогом minLifeTime
           adjustedLife = Mathf.Max(minLifeTime, _duration * _timeMultiplier);
           // Debug.Log($"_swordTip размер меча: {currentLength} , {_timeMultiplier} , {adjustedLife}");
        }
    }
}
