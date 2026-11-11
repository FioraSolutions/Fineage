using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
    private GameManager _game;
    private LoginRuntime _loginRuntime;
    private World _world;
    private IWorldDomainSink _domain;
    private WorldPacketApply _worldApply;
    private PacketApplyQueue _applyQueue;
    private L2GameUI _gameUi;

    GameClient Game => DiRuntimePacketServices.Game;
    LoginClient Login => NetworkingPacketServices.Login;




    private SceneLoader Scenes => LobbyFlowPacketServices.Scenes;

    private L2LoginUI LoginUi
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Ui != null)
                return _loginRuntime.Ui;
            return LobbyUiPacketServices.LoginUi;
        }
    }

    private L2GameUI GameUi
    {
        get
        {
            if (_gameUi != null)
                return _gameUi;
            return LobbyUiPacketServices.GameUi;
        }
    }

    private ServerSelectWindow ServerSelect
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Servers != null)
                return _loginRuntime.Servers;
            return LoginPacketServices.ServerSelect;
        }
    }

    private LoginCameraManager LoginCamera
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Camera != null)
                return _loginRuntime.Camera;
            return LoginPacketServices.LoginCamera;
        }
    }

    private CharacterCreator Creator
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Creator != null)
                return _loginRuntime.Creator;
            return LoginPacketServices.Creator;
        }
    }

    private World GameWorld
    {
        get
        {
            if (_world != null)
                return _world;
            return WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        }
    }

    private IWorldDomainSink Domain
    {
        get
        {
            if (_domain != null)
                return _domain;
            return WorldApplyPacketServices.Domain;
        }
    }

    private WorldPacketApply WorldApply
    {
        get
        {
            if (_worldApply != null)
                return _worldApply;
            return WorldApplyPacketServices.WorldApply ?? WorldPacketServices.WorldApply;
        }
    }








}
