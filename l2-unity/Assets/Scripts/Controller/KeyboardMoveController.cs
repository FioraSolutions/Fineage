using UnityEngine;
using VContainer;

public sealed partial class KeyboardMoveController : MonoBehaviour
{
    [Inject] InputManager _input;

    [SerializeField] private float _stepMeters = 2.5f;
    [SerializeField] private float _sendIntervalSeconds = 0.12f;

    private float _nextSendTime;

    private InputManager GameInput =>
        _input != null ? _input : ControllerPacketServices.Input;


}
