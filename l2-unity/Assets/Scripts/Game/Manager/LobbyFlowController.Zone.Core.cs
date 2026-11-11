using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
    public void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers)
    {
        _game.GameState = GameState.SERVER_LIST;

        LoginUi.ShowServerSelectWindow();

        ServerSelect.UpdateServerList(lastServer, serverData, charsOnServers);
    }

    public void OnAuthAllowed()
    {
        _game.GameState = GameState.CHAR_SELECT;
        LoginCamera.SwitchCamera("CharSelect");

        LoginUi.ShowCharSelectWindow();
    }

    public void OnCharacterSelect()
    {
        _game.GameState = GameState.IN_GAME;
        _game.WorldSpawnReady = false;

        LoginUi.StartLoading();
        Scenes.LoadSWMap();
    }

    public void OnCreateUser(List<PlayerTemplates> playerTemplates)
    {
        _game.GameState = GameState.CHAR_CREATION;

        LoginCamera.SwitchCamera("Login");

        LoginUi.SetCharTemplations(playerTemplates);
        LoginUi.ShowCharCreationWindow();
    }

    public void OnCreateUserFail(string text)
    {
        LoginUi.ShowCharCreationError(text);
    }
}
