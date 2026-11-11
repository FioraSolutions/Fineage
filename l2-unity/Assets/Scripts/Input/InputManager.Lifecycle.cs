using System;

public partial class InputManager : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ControllerPacketServices.Bind(input: this);
        }
        else
        {
            Destroy(this);
        }

        _playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void SetupInputActions()
    {
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null || _playerInput.actions == null)
            return;

        _leftClickAction = _playerInput.actions["LeftClick"];
        _rightClickAction = _playerInput.actions["RightClick"];

        _cameraAxisAction = _playerInput.actions["CameraAxis"];
        _zoomAxisAction = _playerInput.actions["ZoomAxis"];

        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _nextTargetAction = _playerInput.actions["NextTarget"];
        _attackAction = _playerInput.actions["Attack"];
        _sitAction = _playerInput.actions["Sit"];

        _inventoryAction = _playerInput.actions["Inventory"];
        _characterStatusAction = _playerInput.actions["CharacterStatus"];
        _actionsAction = _playerInput.actions["Actions"];
        _closeWindowAction = _playerInput.actions["CloseWindow"];
        _systemMenuAction = _playerInput.actions["SystemMenu"];
        _mapAction = _playerInput.actions["Map"];
        _validateAction = _playerInput.actions["Validate"];

        SkillbarActions = new InputAction[5, 12];

        for (int skillbar = 1; skillbar <= 5; skillbar++)
        {
            for (int i = 1; i <= 12; i++)
            {
                SkillbarActions[skillbar - 1, i - 1] = _playerInput.actions[$"Shortcut{skillbar}-{i}"];
            }
        }

        SkillbarInputs = new bool[5, 12];
    }
}
