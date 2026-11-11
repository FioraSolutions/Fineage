using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
    public void OnWorldSceneLoaded()
    {
        L2LoginUI loginUi = LoginUi;
        if (loginUi != null)
            GameObject.Destroy(loginUi.gameObject);
        PlayerInfoInterlude playerInfo = ZoneCommands.LocalPlayerInfo;
        DomainPlayerSpawned spawned = InterludeDomainMap.PlayerSpawned(playerInfo);
        IWorldDomainSink domain = Domain;
        if (domain != null && spawned != null)
            domain.ApplyPlayerSpawned(spawned);
        else
            GameWorld.SpawnPlayer(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance);
        L2GameUI ui = GameUi;
        if (ui != null)
            ui.StopLoading();
        PlayerStateMachine stateMachine = PlayerStatePacketServices.PlayerState;
        if (stateMachine != null)
            stateMachine.enabled = true;
        _game.WorldSpawnReady = true;
        // NetSession already sent NetOpcode.EnterWorld on ZoneAuthResult.
        // Do not re-fire Interlude EnterWorldCommand / RequestSkillCoolTimeCommand —
        // FineageZoneTransport has no Net mapping and only spam-warns "TX dropped".
        FineageLog.Info(FineageLogCategory.GameNet, "world spawn ready (Net EnterWorld already sent)");
        if (domain != null)
            domain.FlushKnownlist();
        else
        {
            WorldPacketApply apply = WorldApply;
            if (apply != null)
                apply.FlushKnownlist();
        }

        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerState.IDLE);
            stateMachine.NotifyEvent(Event.ENTER_WORLD);
        }
    }

    public void OnLoginServerConnected()
    {
        _game.GameState = GameState.LOGIN_CONNECTED;
    }

    public void OnLoginServerAuthAllowed()
    {
        _game.GameState = GameState.READING_LICENSE;

        LoginUi.ShowLicenseWindow();
    }

    public void OnLoginServerPlayOk()
    {
        _game.GameState = GameState.READY_TO_CONNECT;
    }

    public void OnConnectingToGameServer()
    {
        _game.GameState = GameState.CONNECTING_TO_GAMESERVER;
    }
}
