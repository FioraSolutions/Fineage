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

    public float GetCollissionRadius()
    {
        return VectorUtils.ConvertL2jDistance(_appearance.CollisionRadius);
    }

 


    // Called when ApplyDamage packet is received 
    public Entity GetTargetEntity()
    {
        if (_target == null)
        {
            _lastTarget = null;
            _cachedTargetEntity = null;
            return null;
        }

        if (_cachedTargetEntity == null || _lastTarget != _target)
        {
            _lastTarget = _target;
            _cachedTargetEntity = _target.GetComponent<Entity>();
        }

        return _cachedTargetEntity;
    }

    public void SetDamage(int damage) 
    {
        _status.SetDamage(damage);
    }

    public void SetSelfHit(Hit hit)
    {
        _selfHit = hit;
    }
    
    public bool HitIsMissed()
    {
        if (_selfHit == null) return false;
        return _selfHit.isMiss();
    }

    public double CalculateRemainingHp()
    {
        return _status.GetRemainingHp();
    }

    public void SetupTotalCastDuration(float serverHitTimeMs, float flightTimeMs, float[] clipsDurations, float shotEventTime, int targetObjectId = 0, int castStyle = 1, bool mixCombo = false)
    {
        if (_castData == null) _castData = new MagicCastData();

        _castData.Setup(serverHitTimeMs, flightTimeMs, clipsDurations, shotEventTime, targetObjectId, castStyle, mixCombo);
    }

    public MagicCastData GetMagicCastData()
    {
        return _castData;
    }

}
