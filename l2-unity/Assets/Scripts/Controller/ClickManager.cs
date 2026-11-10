using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager : MonoBehaviour
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

        // Entity pick SoT (#930): EntityClick only first — CharacterController on Entity must not win.
        if (EntityPick.TryPick(ray, MousePickDistance, out Entity picked, out hit))
        {
            _hoverObjectData = new ObjectData(picked.gameObject);
            if (GameInput != null &&
                GameInput.LeftClickDown &&
                !GameInput.RightClickHeld)
            {
                _targetObjectData = _hoverObjectData;
                OnClickOnEntity();
            }
            return;
        }

        if (EntityPick.TryPickMove(ray, MousePickDistance, _clickThroughMask, out hit))
        {
            _hoverObjectData = new ObjectData(hit.collider.gameObject);
            if (GameInput != null &&
                GameInput.LeftClickDown &&
                !GameInput.RightClickHeld)
            {
                _targetObjectData = _hoverObjectData;
                OnClickToMove(hit);
            }
        }
        else
        {
            _hoverObjectData = null;
        }
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

}
