using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity : MonoBehaviour
{
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



    static readonly string[] SwordBladePointNames = { "Sword_Base", "Sword_Tip" };



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

}
