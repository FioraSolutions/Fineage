using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2LoginUI : L2UI
{
    public void SetLoading(bool loading)
    {
        this.loading = loading;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DiBootstrap.EnsureLoginScope();
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
}
