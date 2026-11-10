using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class ClickManager : MonoBehaviour
{
    [Inject] InputManager _input;
    [Inject] TargetManager _targets;
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
    [System.Obsolete("Use [Inject] or ControllerPacketServices.Clicks instead")]
    public static ClickManager Instance => _instance;

    private InputManager GameInput
    {
        get
        {
            if (_input != null)
                return _input;
            return ControllerPacketServices.Input;
        }
    }

    private TargetManager Targets
    {
        get { return _targets != null ? _targets : ControllerPacketServices.Targets; }
    }

    // L2 FindMouseTargetObject far = 10000 UU.
    private const float MousePickDistance = 10000f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ControllerPacketServices.Bind(clicks: this);
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
        _locator = null;
        WireClickPacketServices();
    }

    [Inject]
    void WireClickPacketServices()
    {
        ControllerPacketServices.Bind(
            input: GameInput,
            targets: Targets,
            clicks: this,
            player: ControllerPacketServices.Player);
    }

    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask)
    {
        _entityMask = entityMask;
        _clickThroughMask = clickThroughMask;
    }

    void Update()
    {
        Camera cam = ResolvePickCamera();
        if (cam == null)
        {
            _hoverObjectData = null;
            return;
        }

        Ray ray = cam.ScreenPointToRay(PollInput.MousePosition);
        RaycastHit hit;
        ItemEntity stickyDrop = null;
        if (_hoverObjectData != null && _hoverObjectData.ObjectTransform != null)
            stickyDrop = _hoverObjectData.ObjectTransform.GetComponent<ItemEntity>();

        bool overUi = SystemUIPacketServices.Ui != null && SystemUIPacketServices.Ui.IsPointerOverPanel();
        if (!overUi && _dropPicker != null &&
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

        if (SystemUIPacketServices.Ui != null &&
            (SystemUIPacketServices.Ui.MouseOverUI || SystemUIPacketServices.Ui.IsPointerOverPanel()))
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

    static Camera ResolvePickCamera()
    {
        CameraController controller = CameraPacketServices.Controller;
        if (controller != null)
        {
            Camera cam = controller.GetComponent<Camera>();
            if (cam != null && cam.enabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return Camera.main;
    }

    public void OnClickToMove(RaycastHit hit)
    {
        if (CastMoveLock.IsLocked)
        {
            return;
        }

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
        if (PlayerStatePacketServices.PlayerState == null)
        {
            return;
        }

        PlayerStatePacketServices.PlayerState.Follow = null;
        PlayerStatePacketServices.PlayerState.IsMoveToPawn = false;
    }

    private void SendPacketMoveToLocation(Vector3 lastClickPosition)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;

        if (!TryResolveMoveOrigin(out Vector3 from))
            return;

        ZoneCommands.Send(new MoveToCommand(from, lastClickPosition));
    }

    static bool TryResolveMoveOrigin(out Vector3 from)
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
        {
            from = player.transform.position;
            return true;
        }

        PlayerController controller = ControllerPacketServices.Player;
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

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;

        // Already selected monster → melee attack intent. Ctrl force-attacks players / NPCs / mobs
        // (FineageZoneTransport also keys off GameCtrl.IsHeld so the first Ctrl-click swings).
        Entity clickedEntity = clickTarget.GetEntity();
        bool sameSelected = Targets != null &&
            Targets.HasTarget() &&
            Targets.Target.Identity != null &&
            Targets.Target.Identity.Id == clickTarget.Identity.Id &&
            !clickTarget.IsDead();
        bool isMonster = clickedEntity is MonsterEntity &&
            !PetVisuals.IsPet(clickTarget.Identity.Owned, clickTarget.Identity.NpcId);
        if (Targets != null && sameSelected && (isMonster || GameCtrl.IsHeld))
            Targets.SetAttackTarget();

        var l2jpos = clickTarget.Identity.GetL2jPos();
        ZoneCommands.Send(new ClickActionCommand(
            clickTarget.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
    }

    public void PlaceLocator(Vector3 position)
    {
        EnsureLocator();
        if (_locator == null)
        {
            Debug.LogWarning("[MoveMark] locator prefab not loaded");
            return;
        }

        ClickMoveLocator mark = _locator.GetComponent<ClickMoveLocator>();
        if (mark != null)
        {
            mark.PlaceAt(position);
            return;
        }

        _locator.SetActive(true);
        _locator.transform.position = position;
    }

    void EnsureLocator()
    {
        if (_locator != null)
            return;

        // Prefer generated Interlude move-click (Tools/L2 Effects/Build Move Click).
        GameObject prefab = Resources.Load<GameObject>("Data/Effects/locator/e_u093_a/e_u093_a");
        if (prefab == null)
            prefab = Resources.Load<GameObject>("Data/Effects/locator/e_u093_a_composite");
        if (prefab == null)
            prefab = Resources.Load<GameObject>("Data/Effects/locator/Locator");
        if (prefab == null)
            return;

        _locator = Instantiate(prefab);
        _locator.name = "ClickMoveMark";
        if (_locator.GetComponent<ClickMoveLocator>() == null)
            _locator.AddComponent<ClickMoveLocator>();

        // Keep alive until HideLocator — L2Particle would otherwise self-destroy on Play.
        BaseEffect effect = _locator.GetComponentInChildren<BaseEffect>(true);
        if (effect != null)
            effect.BindLifetimeToHost();

        _locator.SetActive(false);
    }

    public void HideLocator()
    {
        if (_locator == null)
            return;

        _locator.SetActive(false);
    }
}
