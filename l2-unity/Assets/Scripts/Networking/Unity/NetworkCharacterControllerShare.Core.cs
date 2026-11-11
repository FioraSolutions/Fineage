using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class NetworkCharacterControllerShare : MonoBehaviour
{
    public void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    void Start() {
        NetworkingPacketServices.Bind(world: _world);
        _characterController = GetComponent<CharacterController>();
        World world = GameWorld;
        if (_characterController == null || (world != null && world.OfflineMode)) {
            enabled = false;
            return;
        }

        if (_networkTransformShare == null) {
            _networkTransformShare = GetComponent<NetworkTransformShare>();
        }
    }
}
