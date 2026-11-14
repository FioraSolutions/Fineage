using System;

using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    /* Components */
    private CharacterController _controller;
    /*Rotate*/
    private float _finalAngle;

    /* Movement */
     private Vector3 _moveDirection;
     private bool _running = true;

    /* Gravity */
    private float _verticalVelocity = 0;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private float _gravity = 28;

    // PhysicsVolume.TerminalVelocity (2500 UU/s) scaled to Unity meters (1/52.5).
    // Caps falling speed the same way Interlude physFalling does.
    private const float TerminalVelocity = 47.62f;


    /* Target */
    private MovementTarget _movementTarget;

    private bool _isMove = false;
    private Transform _lookAtTarget;
    private GearEffects _gearEffects;

    public bool RunningToDestination { get { return _isMove; } }
    public bool Running { get { return _running; } set { _running = value; } }
    public Vector3 MoveDirection { get { return _moveDirection; } }

    private static PlayerController _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static PlayerController Instance { get { return _instance; } }

    private Entity _monster;
    private bool _isRotate = false;
    private bool _isFinish = true;
    private bool _turnsAround = true;

    private float _moveTimer = 0f;
    // Time.time of the last StopMove. A chase repath arrives within a few frames
    // of reaching the previous point; that must not replay the 0.4s walk start.
    private float _moveStoppedAt = -1f;

    private bool _isFirst = false;

    private bool _behindPlayer = false;
    private bool _switchWalkToRun = false;
    private bool _isRotateAttacker = false;
    private Vector3 _attackerPosition;
    public float angleThreshold = 3f;

    private PlayerPositionSender _pp_sender;
    private ICalcSpeed _calcSpeed;
}

