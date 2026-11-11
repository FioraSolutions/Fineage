using UnityEngine;
using VContainer;

public sealed partial class KeyboardMoveController : MonoBehaviour
{
    void Update()
    {
        InputManager input = GameInput;
        if (input == null || !input.Move || input.TurnCamera)
            return;

        if (CastMoveLock.IsLocked)
            return;

        L2GameUI ui = SystemUIPacketServices.Ui;
        if (ui != null && (ui.MouseOverUI || ui.IsPointerOverPanel()))
            return;

        ChatWindow chat = MessagePacketServices.Chat;
        if (chat != null && chat.ChatOpened)
            return;

        if (Time.time < _nextSendTime)
            return;

        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return;

        CameraController camera = CameraPacketServices.Controller;
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

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;

        _nextSendTime = Time.time + _sendIntervalSeconds;
        ZoneCommands.Send(new MoveToCommand(from, to));
    }
}
