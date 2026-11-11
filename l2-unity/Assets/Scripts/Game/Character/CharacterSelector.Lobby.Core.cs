using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector
{
    public void SetLobbyCharacters(IReadOnlyList<LobbyCharacter> characters, int preselectId)
    {
        _lobbyMode = true;
        _lobbyCharacters = characters == null
            ? new List<LobbyCharacter>()
            : new List<LobbyCharacter>(characters);
        ClearLobbyVisuals();
        EnsureCharacterContainer();

        _pawnData = LogonGrps;
        _selectedCharacterSlot = -1;
        _selectedLobbyIndex = -1;
        if (_pawnData == null || _pawnData.Count < 8)
        {
            Debug.LogError("[Lobby] character slot layout is missing.");
            return;
        }

        for (int i = 0; i < _lobbyCharacters.Count && i < 7; i++)
            SpawnLobbyCharacterSlot(i);

        int index = LobbyCharacterMap.PreferredIndex(_lobbyCharacters, preselectId);
        if (index >= 0)
            SelectLobbyCharacter(index);
    }

    public void ClearLobbyVisuals()
    {
        if (_characterGameObjects != null)
        {
            for (int i = 0; i < _characterGameObjects.Count; i++)
            {
                if (_characterGameObjects[i] != null)
                    Object.Destroy(_characterGameObjects[i]);
            }
        }

        if (_container != null)
        {
            Object.Destroy(_container);
            _container = null;
        }

        // Roster refresh stays in lobby: keep _charSelectCamera (SwitchCamera("CharSelect") already ran and
        // won't run again, so nulling it here made Update() bail and clicks did nothing).
        ResetLobbyRefs();
    }

    public void ForgetLobbyVisuals()
    {
        ResetLobbyRefs();
        _charSelectCamera = null;
    }

    void ResetLobbyRefs()
    {
        _container = null;
        _characterGameObjects = new List<GameObject>();
        _selectedCharacterSlot = -1;
        _selectedLobbyIndex = -1;
    }

    void EnsureCharacterContainer()
    {
        if (_container == null)
        {
            _container = new GameObject("Characters");
            LobbyPresentation.Host(_container);
        }

        if (_characterGameObjects == null)
            _characterGameObjects = new List<GameObject>();
    }
}
