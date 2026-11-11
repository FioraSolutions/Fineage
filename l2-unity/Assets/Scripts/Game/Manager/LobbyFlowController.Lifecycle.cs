using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
    [Inject] IZoneTransport _zoneTransport;

    public void Attach(GameManager game)
    {
        _game = game;
    }

    public void WireLogin(LoginRuntime loginRuntime)
    {
        _loginRuntime = loginRuntime;
    }

    public void WireGame(
        World world,
        IWorldDomainSink domain,
        WorldPacketApply worldApply,
        PacketApplyQueue applyQueue,
        L2GameUI gameUi)
    {
        _world = world;
        _domain = domain;
        _worldApply = worldApply;
        _applyQueue = applyQueue;
        _gameUi = gameUi;
    }

    public void LogIn()
    {
    }

    public void LogOut()
    {
        IZoneTransport transport = _zoneTransport ?? ZoneCommands.Transport;
        if (transport != null)
            transport.Disconnect();
    }
}
