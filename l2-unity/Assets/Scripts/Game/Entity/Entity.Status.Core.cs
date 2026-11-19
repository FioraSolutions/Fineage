using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    protected  void Awake() 
    {
        _castData = new MagicCastData();

        Animator = GetComponent<Animator>();

        if (Animator == null)
        {
            Animator = GetComponentInChildren<Animator>();
        }
    }

    public void HideObject()
    {
        gameObject.SetActive(false);
    }

    public void ShowObject()
    {
        gameObject.SetActive(true);
    }

    public void SetDead(bool dead)
    {
       _dead = dead;
        if(_dead) Status.SetHp(0);
    }

    public bool GetDead()
    {
        ///lock (this)
        //{
            return _dead;
        //}
    }

    public virtual void Initialize() {
        TryGetComponent(out _networkAnimationReceive);
        TryGetComponent(out _gear);
       
        UpdatePAtkSpeed((int)_stats.PAtkSpd);
        UpdateMAtkSpeed(_stats.MAtkSpd);
        UpdateSpeed(_stats.Speed);

        EquipAllWeapons();
        // Bind wait/walk/run/atk01 clips to the equipped weapon (bow guards must not keep the baked 1HS set).
        CreatureWeaponAnimApply.Bind(this);
        if (_gear != null)
        {
            _gear.OnEquipAnimationRefresh -= OnWeaponEquipped;
            _gear.OnUnequipAnimationRefresh -= OnWeaponUnequipped;
            _gear.OnEquipAnimationRefresh += OnWeaponEquipped;
            _gear.OnUnequipAnimationRefresh += OnWeaponUnequipped;
        }
    }

    public NetworkAnimationController GetAnimatorController()
    {
        return _networkAnimationReceive;
    }
}
