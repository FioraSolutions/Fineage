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


    public void UpdateRunSpeed(float defaultRunSpeed)
    {
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        // Stub/CharSelected may omit UserInfo — never leave planar speed at 0 (run-in-place).
        if (defaultRunSpeed <= 0.001f)
            defaultRunSpeed = 0.7f;
        _calcSpeed.UpdateSpeedRun(defaultRunSpeed);
    }

    public void UpdateWalkSpeed(float defaultWalkSpeed)
    {
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        if (defaultWalkSpeed <= 0.001f)
            defaultWalkSpeed = 0.45f;
        _calcSpeed.UpdateSpeedWalk(defaultWalkSpeed);
    }

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

    
    private bool _isFirst = false;
  
    private bool _behindPlayer = false;
    private bool _switchWalkToRun = false;
    private bool _isRotateAttacker = false;
    private Vector3 _attackerPosition;
    public float angleThreshold = 3f;

    //newObject

    private PlayerPositionSender _pp_sender;
    private ICalcSpeed _calcSpeed;
    public void Initialize()
    {
        // Same-instance re-init (domain reload / re-enter world) must not Destroy(this):
        // that wiped Instance in OnDestroy and left move with zero speed.
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        WireControllerPacketServices();
        if (_pp_sender == null)
            _pp_sender = _injectedPositionSender != null ? _injectedPositionSender : new PlayerPositionSender();

        EnsureCharacterController();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    void Start()
    {
        WireControllerPacketServices();
        EnsureCharacterController();
        _gearEffects = GetComponent<GearEffects>();
    }

    void EnsureCharacterController()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();
        if (_controller != null && !_controller.enabled)
            _controller.enabled = true;
        EntitySpawnShared.SanitizeCharacterControllerStepOffset(gameObject);
    }


    public void Update()
    {
        if (_isRotateAttacker & !_isMove)
        {
            if (!VectorUtils.IsFacingAttacker(transform , _attackerPosition , angleThreshold))
            {
               // Debug.Log("Rotate to object Attacker running : ");
                TurnTowardsAttacker(_attackerPosition);
            }
            else
            {
               //Debug.Log("Rotate to object Attacker stop: ");
                _isRotateAttacker = false;
            }

        }

        StepLocalMove();
    }

    void StepLocalMove()
    {
        if (_controller == null || !_controller.enabled)
            return;

        //Если мы не двигаемся но мы в состоянии MoveToPawnDto == true тогда мы следим за обьектом двинется он или нет если двинется мы подбегаем к нему
        //Если мы получим MoveToPawnDto == false мы останавливаем слежение
        RestartMoveElseMoveToPawnTrue(ref _isMove);

        if (!_isMove)
            return;

        Vector3 characterPosition = VectorUtils.To2D(transform.position);
        Vector3 targetPosition = VectorUtils.To2D(_movementTarget.GetTarget());

        if(VectorUtils.Distance2D(characterPosition, targetPosition) > _movementTarget.GetDistance())
        {

            _moveTimer += Time.deltaTime;
            SwitchWalkToRun();

            float finalAngle = GetFinalAngle(targetPosition, characterPosition);

            float speed = GetMovementSpeed();
            Vector3 velocity = GetNewPosition(targetPosition, characterPosition, speed);

            // APawn::calcVelocity arrival clamp (Pawn.h): when the remaining
            // distance is below this frame's step, Velocity = Dest / DeltaTime so
            // the pawn lands on the destination instead of overshooting and
            // snapping back on the next frame.
            Vector3 toTarget = targetPosition - characterPosition;
            float toTargetDist = toTarget.magnitude;
            float stepLength = speed * Time.deltaTime;
            if (toTargetDist > 0.0001f && toTargetDist < stepLength)
                velocity = toTarget / Time.deltaTime;

            velocity.y = ApplyGravity(Time.deltaTime);

            Quaternion targetRotation = GetNewRotate(finalAngle);
            float r_speed = _calcSpeed.GetSpeedRotate(_behindPlayer);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, FrameIndependentLerp(r_speed, Time.deltaTime));

            _controller.Move(velocity * Time.deltaTime);


            _isFirst = false;
            _pp_sender.TickWhileMoving(transform.position, PlayerPositionSender.ToL2Heading(transform.eulerAngles.y), Time.deltaTime);
        }
        else
        {
             //Debug.Log("IsMoveToPawn мы на месте " + VectorUtils.Distance2D(characterPosition, targetPosition));
             SetPositionServer(_movementTarget, targetPosition);
             NotifyEvent(PlayerStatePacketServices.PlayerState.State);
             StopMove();

           // Debug.Log("IsMoveToPawn запуск повтора т.е StopMove " + PlayerStatePacketServices.PlayerState.IsMoveToPawn + " state " + PlayerStatePacketServices.PlayerState.State);
        }
    }

    private void NotifyEvent(PlayerState state)
    {
        PlayerStateMachine stateMachine = PlayerStatePacketServices.PlayerState;
        if (stateMachine == null)
            return;

        if (state == PlayerState.RUNNING | state == PlayerState.WALKING)
        {
            stateMachine.NotifyEvent(Event.ARRIVED);
        }
    }
}
