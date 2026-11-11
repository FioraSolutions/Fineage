using System;

using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    void Start()
    {
        WireControllerPacketServices();
        EnsureCharacterController();
        _gearEffects = GetComponent<GearEffects>();
    }

    void EnsureCharacterController()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();
        if (_controller != null && !_controller.enabled)
            _controller.enabled = true;
        EntitySpawnShared.SanitizeCharacterControllerStepOffset(gameObject);
    }
}
