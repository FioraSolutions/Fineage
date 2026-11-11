using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ServerSelectWindow : L2Window
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                CancelButtonPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                ConfirmButtonPressed();
            }
        }
    }
}
