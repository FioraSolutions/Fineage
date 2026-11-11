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

}
