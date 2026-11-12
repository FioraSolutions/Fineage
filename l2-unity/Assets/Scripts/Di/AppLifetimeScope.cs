using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class AppLifetimeScope : LifetimeScope
{
    IZoneTransport _boundTransport;
    IGameManager _boundManager;
    Action _onDisconnected;

    protected override void OnDestroy()
    {
        if (_boundTransport != null && _onDisconnected != null)
            _boundTransport.OnDisconnected -= _onDisconnected;
        _onDisconnected = null;
        ZoneCommands.Unbind(_boundTransport);
        _boundTransport = null;
        _boundManager = null;
        if (ReferenceEquals(App.Container, Container))
            App.Container = null;
        base.OnDestroy();
    }
}
