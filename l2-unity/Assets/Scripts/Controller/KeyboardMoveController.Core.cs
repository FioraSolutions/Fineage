using UnityEngine;
using VContainer;

public sealed partial class KeyboardMoveController : MonoBehaviour
{
    [Inject]
    void WireServices()
    {
        ControllerPacketServices.Bind(input: GameInput);
    }
}
