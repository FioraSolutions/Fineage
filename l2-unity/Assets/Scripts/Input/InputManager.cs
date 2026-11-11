using System;

public partial class InputManager : MonoBehaviour
{
    #region InputActions
    // Mouse
    private InputAction _leftClickAction;
    private InputAction _rightClickAction;
    // Camera
    private InputAction _cameraAxisAction;
    private InputAction _zoomAxisAction;
    // Movements
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _nextTargetAction;
    private InputAction _sitAction;
    // UI
    private InputAction _inventoryAction;
    private InputAction _characterStatusAction;
    private InputAction _actionsAction;
    private InputAction _closeWindowAction;
    private InputAction _systemMenuAction;
    private InputAction _mapAction;
    private InputAction _validateAction;

    #endregion

    #region InputValues
    [field: Header("Mouse")]
    // Mouse
    [field: SerializeField] public bool LeftClickDown { get; private set; }
    [field: SerializeField] public bool RightClickDown { get; private set; }
    [field: SerializeField] public bool RightClickUp { get; private set; }
    [field: SerializeField] public bool LeftClickHeld { get; private set; }
    [field: SerializeField] public bool RightClickHeld { get; private set; }

    // Camera
    [field: Header("Camera")]
    [field: SerializeField] public Vector2 CameraAxis { get; private set; }
    [field: SerializeField] public bool CameraMoving { get; private set; }
    [field: SerializeField] public bool TurnCamera { get; private set; }
    [field: SerializeField] public float ZoomAxis { get; private set; }

    // Movements
    [field: Header("Movements")]
    [field: SerializeField] public Vector2 MoveInput { get; private set; }
    [field: SerializeField] public bool Move { get; private set; }
    [field: SerializeField] public bool MoveForward { get; private set; }
    [field: SerializeField] public bool Jump { get; private set; }
    [field: SerializeField] public bool Attack { get; private set; }
    [field: SerializeField] public bool NextTarget { get; private set; }
    [field: SerializeField] public bool Sit { get; private set; }

    // UI
    [field: Header("UI")]
    [field: SerializeField] public bool OpenInventory { get; private set; }
    [field: SerializeField] public bool OpenCharacerStatus { get; private set; }
    [field: SerializeField] public bool OpenSystemMenu { get; private set; }
    [field: SerializeField] public bool OpenActions { get; private set; }
    [field: SerializeField] public bool OpenMap { get; private set; }
    [field: SerializeField] public bool CloseWindow { get; private set; }
    [field: SerializeField] public bool Validate { get; private set; }

    public InputAction[,] SkillbarActions { get; private set; }
    public bool[,] SkillbarInputs { get; private set; }

    #endregion

    private PlayerInput _playerInput;

    private static InputManager _instance;
    [System.Obsolete("Use ControllerPacketServices.Input instead")]
        public static InputManager Instance { get { return _instance; } }

}
