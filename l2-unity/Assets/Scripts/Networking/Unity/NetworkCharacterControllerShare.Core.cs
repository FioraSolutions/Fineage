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
        if (_world == null) {
            Debug.LogError("[NetworkCharacterControllerShare] IWorldSpawnContext missing — GameLifetimeScope.RegisterWorld did not register World.");
            enabled = false;
            return;
        }
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null || _world.OfflineMode) {
            enabled = false;
            return;
        }

        if (_networkTransformShare == null) {
            _networkTransformShare = GetComponent<NetworkTransformShare>();
        }
    }
}
