using System;

public partial class InputManager : MonoBehaviour
{
    void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        if (_moveAction == null)
        {
            SetupInputActions();
            if (_moveAction == null)
                return;
        }

        CameraAxis = _cameraAxisAction.ReadValue<Vector2>();
        CameraMoving = CameraAxis.y != 0 || CameraAxis.x != 0;

        LeftClickDown = _leftClickAction.WasPerformedThisFrame();
        RightClickDown = _rightClickAction.WasPerformedThisFrame();
        RightClickUp = _rightClickAction.WasReleasedThisFrame();
        LeftClickHeld = _leftClickAction.IsPressed();
        RightClickHeld = _rightClickAction.IsPressed();

        CloseWindow = _closeWindowAction.WasPerformedThisFrame();
        Validate = _validateAction.WasPerformedThisFrame();

        L2GameUI gameUi = SystemUIPacketServices.Ui;
        bool mouseOverUi = gameUi != null && gameUi.MouseOverUI;

        if (!mouseOverUi)
        {
            if (RightClickHeld && CameraMoving)
            {
                TurnCamera = true;
                gameUi?.DisableMouse();
            }

            ZoomAxis = _zoomAxisAction.ReadValue<float>();
        }

        if (RightClickUp)
        {
            TurnCamera = false;
            gameUi?.EnableMouse();
        }

        ChatWindow chat = MessagePacketServices.Chat;
        bool chatOpen = chat != null && chat.ChatOpened;
        if (!chatOpen)
        {
            MoveInput = _moveAction.ReadValue<Vector2>();
            Jump = _jumpAction.WasPerformedThisFrame();
            Attack = _attackAction.WasPerformedThisFrame();
            NextTarget = _nextTargetAction.WasPerformedThisFrame();
            Sit = _sitAction.WasPerformedThisFrame();

            OpenCharacerStatus = _characterStatusAction.WasPerformedThisFrame();
            OpenInventory = _inventoryAction.WasPerformedThisFrame();
            OpenSystemMenu = _systemMenuAction.WasPerformedThisFrame();
            OpenActions = _actionsAction.WasPerformedThisFrame();
            OpenMap = _mapAction != null && _mapAction.WasPerformedThisFrame();
        }
        else
        {
            MoveInput = Vector2.zero;
            OpenMap = false;
        }

        MoveForward = LeftClickHeld && RightClickHeld;
        Move = MoveInput.y != 0 || MoveInput.x != 0 || MoveForward;

        if (SkillbarActions == null || SkillbarInputs == null)
            return;

        for (int skillbar = 0; skillbar < 5; skillbar++)
        {
            for (int i = 0; i < 12; i++)
            {
                SkillbarInputs[skillbar, i] = SkillbarActions[skillbar, i].WasPerformedThisFrame();
            }
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
