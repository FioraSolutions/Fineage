using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class ClickManager : MonoBehaviour
{
    [Inject] InputManager _input;
    [Inject] TargetManager _targets;
    [Inject] GameClient _gameClient;
    [Inject] ItemDropPicker _dropPicker;
    [SerializeField] private GameObject _locator;
    [SerializeField] private ObjectData _targetObjectData;
    [SerializeField] private ObjectData _hoverObjectData;

    public ObjectData HoverObjectData => _hoverObjectData;

    private Vector3 _lastClickPosition = Vector3.zero;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _clickThroughMask;
    private UIDocument uiDocument;
    private static ClickManager _instance;
    public static ClickManager Instance => _instance;

    private InputManager GameInput
    {
        get { return _input != null ? _input : ControllerPacketServices.Input; }
    }

    private TargetManager Targets
    {
        get { return _targets != null ? _targets : ControllerPacketServices.Targets; }
    }

    private GameClient Game
    {
        get
        {
            if (_gameClient != null)
                return _gameClient;
            GameClient fromController = ControllerPacketServices.Game;
            if (fromController != null)
                return fromController;
            return IncomingPacketActions.Game;
        }
    }

    // L2 FindMouseTargetObject far = 10000 UU.
    private const float MousePickDistance = 10000f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            uiDocument = GetComponent<UIDocument>();
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        _locator = GameObject.Find("Locator");
        HideLocator();
        WireClickPacketServices();
    }

    [Inject]
    void WireClickPacketServices()
    {
        ControllerPacketServices.Bind(
            game: Game,
            input: GameInput,
            targets: Targets);
    }

    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask)
    {
        _entityMask = entityMask;
        _clickThroughMask = clickThroughMask;
    }

    void Update()
    {
        if (Camera.main == null)
        {
            _hoverObjectData = null;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        ItemEntity stickyDrop = null;
        if (_hoverObjectData != null && _hoverObjectData.ObjectTransform != null)
            stickyDrop = _hoverObjectData.ObjectTransform.GetComponent<ItemEntity>();

        if (_dropPicker != null &&
            _dropPicker.TryPick(ray, MousePickDistance, stickyDrop, out ItemEntity dropItem, out _))
        {
            Transform root = dropItem.transform;
            _hoverObjectData = new ObjectData(root.gameObject);

            if (GameInput != null &&
                GameInput.LeftClickDown &&
                !GameInput.RightClickHeld)
            {
                _targetObjectData = _hoverObjectData;
                OnClickOnEntity();
            }

            return;
        }

        if (L2GameUI.Instance != null && L2GameUI.Instance.MouseOverUI)
        {
            _hoverObjectData = null;
            return;
        }

        if (Physics.Raycast(ray, out hit, MousePickDistance, ~_clickThroughMask))
        {
            int hitLayer = hit.collider.gameObject.layer;
            if (_entityMask == (_entityMask | (1 << hitLayer)))
            {
                Entity entity = hit.collider.GetComponentInParent<Entity>();
                Transform root = entity != null
                    ? entity.transform
                    : (hit.transform.parent != null ? hit.transform.parent : hit.transform);
                _hoverObjectData = new ObjectData(root.gameObject);
            }
            else
            {
                _hoverObjectData = new ObjectData(hit.collider.gameObject);
            }

            if (GameInput != null &&
                GameInput.LeftClickDown &&
                !GameInput.RightClickHeld)
            {
                _targetObjectData = _hoverObjectData;

                if (_entityMask == (_entityMask | (1 << hitLayer)))
                {
                    OnClickOnEntity();
                }
                else if (_targetObjectData != null)
                {
                    OnClickToMove(hit);
                }
            }
        }
        else
        {
            _hoverObjectData = null;
        }
    }

    public void OnClickToMove(RaycastHit hit)
    {
        if (CastMoveLock.IsLocked)
            return;

        _lastClickPosition = hit.point;

        StopFollow();
        SendPacketMoveToLocation(_lastClickPosition);

        if (Targets != null)
        {
            Targets.ClearAttackTarget();
        }

        float angle = Vector3.Angle(hit.normal, Vector3.up);
        if (angle < 85f)
        {
            PlaceLocator(_lastClickPosition);
        }
        else
        {
            HideLocator();
        }
    }

    private void StopFollow()
    {
        if (PlayerStateMachine.Instance == null)
        {
            return;
        }

        PlayerStateMachine.Instance.Follow = null;
        PlayerStateMachine.Instance.IsMoveToPawn = false;
    }

    private void SendPacketMoveToLocation(Vector3 lastClickPosition)
    {
        GameClient game = Game;
        if (game == null)
            return;

        if (!TryResolveMoveOrigin(out Vector3 from))
            return;

        game.Send(new MoveToCommand(from, lastClickPosition));
    }

    static bool TryResolveMoveOrigin(out Vector3 from)
    {
        PlayerEntity player = PlayerEntity.Instance;
        if (player != null)
        {
            from = player.transform.position;
            return true;
        }

        PlayerController controller = IncomingPacketActions.Player;
        if (controller == null)
            controller = PlayerController.Instance;
        if (controller != null)
        {
            from = controller.transform.position;
            return true;
        }

        from = default;
        return false;
    }

    public void OnClickOnEntity()
    {
        TargetData clickTarget = new TargetData(_targetObjectData);
        if (clickTarget == null || clickTarget.Identity == null)
        {
            return;
        }

        GameClient game = Game;
        if (game == null)
            return;

        // Already selected monster → melee attack intent. NPCs = talk, stay Target (blue).
        if (Targets != null &&
            Targets.HasTarget() &&
            Targets.Target.Identity != null &&
            Targets.Target.Identity.Id == clickTarget.Identity.Id &&
            !clickTarget.IsDead() &&
            clickTarget.GetEntity() is MonsterEntity &&
            !PetVisuals.IsPet(clickTarget.Identity))
        {
            Targets.SetAttackTarget();
        }

        var l2jpos = clickTarget.Identity.GetL2jPos();
        game.Send(new ClickActionCommand(
            clickTarget.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
    }

    public void PlaceLocator(Vector3 position)
    {
        EnsureLocator();
        if (_locator == null)
        {
            return;
        }

        _locator.SetActive(true);
        _locator.transform.position = position;
    }

    void EnsureLocator()
    {
        if (_locator != null)
            return;
        _locator = GameObject.Find("Locator");
        if (_locator != null)
            return;
        GameObject prefab = Resources.Load<GameObject>("Data/Effects/locator/Locator");
        if (prefab == null)
            return;
        _locator = Instantiate(prefab);
        _locator.name = "Locator";
        _locator.SetActive(false);
    }

    public void HideLocator()
    {
        if (_locator == null)
        {
            return;
        }

        _locator.SetActive(false);
    }
}
