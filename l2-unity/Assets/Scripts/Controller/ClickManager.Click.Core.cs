using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager : MonoBehaviour, IClickMasks
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
