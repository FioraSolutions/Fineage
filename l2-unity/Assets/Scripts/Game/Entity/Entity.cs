using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public partial class Entity : MonoBehaviour {
    [SerializeField] private bool _entityLoaded;
    [SerializeField] private EntityIdentity _identityInterlude;
    [SerializeField] private Status _status;
    [SerializeField] private Stats _stats;
    [SerializeField] private Status _statusInterlude;
    protected Hit _selfHit;
    [SerializeField] protected Appearance _appearance;
    [SerializeField] private bool _running;
    [SerializeField] private bool _dead;
    [SerializeField] private CharacterRace _race;
    [SerializeField] private CharacterRaceAnimation _raceId;
    [SerializeField] private EntityBodyType _bodyType = EntityBodyType.Humanoid;
    
    public Animator Animator { get; private set; }

    public bool HasAnimator()
    {
        if (Animator == null)
            Animator = GetComponentInChildren<Animator>(true);
        return Animator != null && Animator.runtimeAnimatorController != null;
    }
    [Header("Combat")]
    [SerializeField] private int _targetId;
    [SerializeField] private Transform _target;
    private Transform _lastTarget;
    private Entity _cachedTargetEntity;
    [SerializeField] private Transform _attackTarget;
    [SerializeField] private long _stopAutoAttackTime;
    [SerializeField] private long _startAutoAttackTime;

    private readonly EntityActionSlot _actionSlot = new EntityActionSlot();
    public EntityActionSlot ActionSlot { get { return _actionSlot; } }

    protected MagicCastData _castData;
    protected NetworkAnimationController _networkAnimationReceive;
    protected Gear _gear;
    public Gear Gear => _gear;
    public bool Running
    {
        get { return _running; }
        set
        {
            bool changed = _running != value;
            _running = value;
            if (changed && this is UserEntity && GaitLog.Enabled)
            {
                GaitLog.Log(this, "running-set",
                    "running=" + value +
                    " identityRunning=" + (Identity != null ? Identity.IsRunning : false));
            }
        }
    }

    // Locomotion gait state (Interlude APawn.MoveTimer / bRunning) persisted on the
    // actor rather than on the MovementData. MovementData is dropped on every
    // OnFinish between move packets, which reset MoveTimer to 0 and re-triggered the
    // 0.4s walk window each packet. Keep the timer/gait here so a running CharInfo
    // stays running across consecutive MoveToLocation packets.
    [System.NonSerialized] private float _locomotionTimer;
    [System.NonSerialized] private string _locomotionGait;

    public float LocomotionTimer
    {
        get { return _locomotionTimer; }
        set
        {
            // [GAIT] detect who resets the timer (regression: gait keeps restarting).
            if (_locomotionTimer > 0.05f && value <= 0.001f && GaitLog.Enabled)
            {
                GaitLog.Log(this, "timer-reset",
                    "from=" + _locomotionTimer.ToString("F3") + " to=" + value.ToString("F3"));
            }
            _locomotionTimer = value;
        }
    }

    public string LocomotionGait
    {
        get { return _locomotionGait; }
        set { _locomotionGait = value; }
    }

    public Transform GetWeaponTransform()
    {
        if (_gear == null)
            return null;

        // weapon_shot.int [BOW] RH=-1 LH=0: the charge is attached to the left-hand bow, not Sword Bone.
        if (_gear.WeaponType == WeaponType.bow)
        {
            Transform bow = FirstWeaponMesh(_gear.GetAllTransformByLeftHand(new[] { Gear.weaponName }));
            if (bow != null)
                return bow;
        }

        Transform right = FirstWeaponMesh(_gear.GetAllTransformByRightHand(new[] { Gear.weaponName }));
        if (right != null)
            return right;

        return _gear.GetTransformRightHandBone();
    }

    static Transform FirstWeaponMesh(Transform[] found)
    {
        if (found == null)
            return null;

        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                return found[i];
        }

        return null;
    }

    static readonly string[] SwordBladePointNames = { "Sword_Base", "Sword_Tip" };

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
    public Status Status { get => _status; set => _status = value; }
    public Stats Stats { get => _stats; set => _stats = value; }
    public Appearance Appearance { get => _appearance; set { _appearance = value; } }
    public EntityIdentity Identity { get => _identityInterlude; set => _identityInterlude = value; }

    /// <summary>
    /// Live PartySpelled / AbnormalStatusUpdate icons for this creature (target window).
    /// Self buff tray still uses <see cref="BufferPanel"/> separately.
    /// </summary>
    public System.Collections.Generic.List<EffectHolder> TargetEffects { get; } =
        new System.Collections.Generic.List<EffectHolder>();

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

    /// <summary>Melee/bow soulshot charge — HitManager may spawn SoulshotImpact on the target.</summary>
    public bool IsSoulshotCharged { get; set; }
    /// <summary>Spiritshot / blessed spiritshot charge — weapon glow only; no melee SS impact on the mob.</summary>
    public bool IsSpiritshotCharged { get; set; }
    public int TargetId { get => _targetId; set => _targetId = value; }

    public Transform LastTarget { get { return _lastTarget; } set { _lastTarget = value; } }
    public Transform Target
    {
        get { return _target; }
        set
        {
            _target = value;
            _lastTarget = null;
            _cachedTargetEntity = null;
        }
    }
    public Transform AttackTarget { get { return _attackTarget; } set { _attackTarget = value; } }
    public bool InCombat { get; set; }
    public long StopAutoAttackTime { get { return _stopAutoAttackTime; } }
    public long StartAutoAttackTime { get { return _startAutoAttackTime; } }
    public CharacterRace Race { get { return _race; } set { _race = value; } }
    public CharacterRaceAnimation RaceId { get { return _raceId; } set { _raceId = value; } }
    public EntityBodyType BodyType { get { return _bodyType; } set { _bodyType = value; } }
    public bool EntityLoaded { get { return _entityLoaded; } set { _entityLoaded = value; } }

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
