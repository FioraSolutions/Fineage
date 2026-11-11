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















}
