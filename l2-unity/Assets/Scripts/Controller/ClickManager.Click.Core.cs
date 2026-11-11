using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager : MonoBehaviour
{
    [Inject]
    void WireClickPacketServices()
    {
        ControllerPacketServices.Bind(
            input: GameInput,
            targets: Targets,
            clicks: this,
            player: ControllerPacketServices.Player);
    }
}
