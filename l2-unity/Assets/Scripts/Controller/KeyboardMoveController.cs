using UnityEngine;
using VContainer;

/// <summary>
/// Sends MoveToLocation from WASD / click-move-forward input (same path as click-to-move).
/// </summary>
public sealed class KeyboardMoveController : MonoBehaviour
{
    [Inject] InputManager _input;
    [Inject] GameClient _gameClient;

    [SerializeField] private float _stepMeters = 2.5f;
    [SerializeField] private float _sendIntervalSeconds = 0.12f;

    private float _nextSendTime;

    private InputManager GameInput =>
        _input != null ? _input : ControllerPacketServices.Input;

    private GameClient Game =>
        _gameClient != null ? _gameClient : ControllerPacketServices.Game;

    [Inject]
    void WireServices()
    {
        ControllerPacketServices.Bind(input: GameInput, game: Game);
    }

    void Update()
    {
        InputManager input = GameInput;
        if (input == null || !input.Move || input.TurnCamera)
            return;

        if (CastMoveLock.IsLocked)
            return;

        L2GameUI ui = L2GameUI.Instance;
        if (ui != null && (ui.MouseOverUI || ui.IsPointerOverPanel()))
            return;

        ChatWindow chat = ChatWindow.Instance;
        if (chat != null && chat.ChatOpened)
            return;

        if (Time.time < _nextSendTime)
            return;

        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            player = PlayerController.Instance;
        if (player == null)
            return;

        CameraController camera = CameraController.Instance;
        if (camera == null)
            return;

        Vector2 move = input.MoveInput;
        if (input.MoveForward)
            move.y = 1f;

        if (move.sqrMagnitude < 0.01f)
            return;

        Vector3 forward = camera.transform.forward;
        forward.y = 0f;
        Vector3 right = camera.transform.right;
        right.y = 0f;
        if (forward.sqrMagnitude < 0.0001f || right.sqrMagnitude < 0.0001f)
            return;

        forward.Normalize();
        right.Normalize();
        Vector3 direction = forward * move.y + right * move.x;
        if (direction.sqrMagnitude < 0.0001f)
            return;
        direction.Normalize();

        Vector3 from = player.transform.position;
        Vector3 to = from + direction * _stepMeters;

        GameClient game = Game;
        if (game == null)
            return;

        _nextSendTime = Time.time + _sendIntervalSeconds;
        game.Send(new MoveToCommand(from, to));
    }
}
