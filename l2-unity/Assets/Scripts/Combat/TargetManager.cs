using System;
using UnityEngine;
using VContainer;

public partial class TargetManager : MonoBehaviour
{
    // L2 UNetworkHandler GetNextObject type ids.
    public const int KindAllPawns = 0;
    public const int KindItem = 1;
    public const int KindEnemy = 2;
    public const int KindFriend = 3;
    public const int KindParty = 4;
    public const int KindNpc = 5;
    public const int KindCreature = 6;

    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : ControllerPacketServices.GameWorld;

    private TargetData _target = null;
    private TargetData _attackTarget = null;
    private Transform _playerTransform;

    [SerializeField] private float _maximumTargetDistance = 15f;
    [SerializeField] private LayerMask _entityMask;
    private string defaultHexColor = "#ffffff";

    private int _cycleKind = int.MinValue;
    private float _cycleLastDistance;
    private int _cycleLastObjectId = -1;

    private Action<Entity> _considerCandidate;
    private int _scanKind;
    private int _scanSelfId;
    private int _scanExcludeId;
    private float _scanMinDistance;
    private float _scanBestDistance;
    private Vector3 _scanOrigin;
    private Entity _scanBest;

    public TargetData Target { get { return _target; } }
    public TargetData AttackTarget { get { return _attackTarget; } }

    private static TargetManager _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static TargetManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _considerCandidate = ConsiderCandidate;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _target = null;
        _attackTarget = null;
        WireControllerPacketServices();
    }

    [Inject]
    void WireControllerPacketServices()
    {
        ControllerPacketServices.Bind(targets: this, gameWorld: GameWorld);
        TargetPacketServices.Bind(targets: this);
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void SetMask(LayerMask mask)
    {
        _entityMask = mask;
    }

    void Update()
    {
        if (DomainPacketServices.LocalPlayer == null || !HasTarget())
        {
            return;
        }

        // Target gone from the world (destroyed, pooled after corpse removal,
        // or hidden like a ride pet): drop the selection immediately so the
        // TargetWindow/nameplate bubble do not keep a dead/removed entity.
        if (IsTargetGone())
        {
            ClearTarget();
            return;
        }

        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
        {
            return;
        }

        _target.Distance = VectorUtils.Distance2D(
            player.transform.position,
            _target.Data.ObjectTransform.position);
    }

    private bool IsTargetGone()
    {
        if (_target == null || _target.Data == null)
        {
            return true;
        }

        Transform t = _target.Data.ObjectTransform;
        if (t == null)
        {
            return true;
        }

        Entity entity = _target.GetEntity();
        if (entity == null)
        {
            return true;
        }

        // Pooled / hidden (SetActive(false)) objects are no longer in the world.
        if (!t.gameObject.activeInHierarchy)
        {
            return true;
        }

        // Local player is never registered in World as a target candidate;
        // anything else must still exist in the world registry.
        World world = GameWorld;
        if (entity != DomainPacketServices.LocalPlayer &&
            world != null &&
            world.GetEntityNoLockSync(entity.Identity != null ? entity.Identity.Id : 0) == null)
        {
            return true;
        }

        return false;
    }

    private bool TryResolvePlayerTransform()
    {
        if (_playerTransform != null)
        {
            return true;
        }

        if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.transform != null)
        {
            _playerTransform = DomainPacketServices.LocalPlayer.transform;
            return true;
        }

        return false;
    }

    public void SetTarget(ObjectData target , string hexColor)
    {
        if (target == null)
        {
            ClearTarget();
            return;
        }

        // New select is Target (blue) until AttackDto/AutoAttackStart marks attack target again.
        int newId = -1;
        Entity newEntity = target.Entity;
        if (newEntity == null && target.ObjectTransform != null)
        {
            newEntity = target.ObjectTransform.GetComponent<Entity>();
        }

        if (newEntity != null && newEntity.Identity != null)
        {
            newId = newEntity.Identity.Id;
        }

        if (_target != null && _target.Identity != null && _target.Identity.Id != newId)
        {
            ClearAttackTarget();
        }

        _target = new TargetData(target);
        _target.SetColor(hexColor);

        DomainPacketServices.LocalPlayer.TargetId = _target.Identity.Id;
        DomainPacketServices.LocalPlayer.Target = _target.Data.ObjectTransform;
    }

    public void SetAttackTarget()
    {
        _attackTarget = _target;
    }

    public void ClearAttackTarget()
    {
        _attackTarget = null;
    }

    public bool IsAttackTargetSet()
    {
        if (_target == null || _attackTarget == null)
        {
            return false;
        }

        if (_target.Identity == null || _attackTarget.Identity == null)
        {
            return _target == _attackTarget;
        }

        return _target.Identity.Id == _attackTarget.Identity.Id;
    }

    public bool HasTarget()
    {
        return _target != null && _target.Data.ObjectTransform != null;
    }

    public bool HasAttackTarget()
    {
        return _attackTarget != null;
    }

    public void UnselectedTarget(int objId)
    {
         ClearTarget();
    }

    public void ClearTarget()
    {
        if (HasTarget())
        {
            if (DomainPacketServices.LocalPlayer.TargetId != -1)
            {
                IZoneTransport game = ZoneCommands.Transport;
                if (game != null)
                    ZoneCommands.Send(new RequestTargetCanceldCommand());
                DomainPacketServices.LocalPlayer.TargetId = -1;
                DomainPacketServices.LocalPlayer.Target = null;
            }

            _target = null;
            _attackTarget = null;
        }
    }

    /// <summary>
    /// Drop the selection because the zone said so (<c>TargetUnselected</c>). Same local clear as
    /// <see cref="ClearTarget"/>, but it must not send <c>RequestTargetCanceld</c> back: the packet is the
    /// answer to that very request, and echoing it would ping-pong two clients' cancels forever.
    /// </summary>
    public void ClearTargetSilently()
    {
        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.TargetId = -1;
            DomainPacketServices.LocalPlayer.Target = null;
        }

        _target = null;
        _attackTarget = null;
    }
}
