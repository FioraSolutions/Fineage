using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity : MonoBehaviour
{
    public Transform[] GetSwordBasePoints()
    {
        if (_gear == null)
            return null;
        return _gear.GetAllTransformByRightHand(SwordBladePointNames);
    }

    public Vector3 GetPositionRightHand()
    {
        if (_gear == null)
            return transform.position;
        return _gear.GetPositionRightHand();
    }

    public float GetWeaponRage() { return _gear.GetWeaponRange(); }

    public void ReplaceTargetEffects(System.Collections.Generic.IList<EffectHolder> effects)
    {
        TargetEffects.Clear();
        if (effects == null)
            return;
        for (int i = 0; i < effects.Count; i++)
            TargetEffects.Add(effects[i]);
        if (GetComponent<EntityAbnormalVisuals>() == null)
            gameObject.AddComponent<EntityAbnormalVisuals>();
    }
}
