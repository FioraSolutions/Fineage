using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using VContainer;

public partial class CharSelectWindow
{
    public void RefreshLobbyActions()
    {
        if (_lobbyCharacters == null)
            return;
        LobbyCharacter selected = SelectedLobbyCharacter();
        bool pending = selected != null && selected.PendingDelete;
        if (_startButton != null)
            _startButton.SetEnabled(LobbyCharacterText.CanEnter(selected));
        if (_deleteButton != null)
            _deleteButton.SetEnabled(selected != null);
        if (_createButton != null)
            _createButton.SetEnabled(LobbyCharacterText.CanCreate(_lobbyCharacters.Count));
        if (_deleteButtonLabel != null)
        {
            if (pending)
                _deleteButtonLabel.text = "Restore";
            else if (_deleteArmed && selected != null && _deleteArmedSlot == selected.Slot)
                _deleteButtonLabel.text = "Confirm";
            else
                _deleteButtonLabel.text = "Delete";
        }
    }

    void ApplyLobbyNames()
    {
        string[] names = new string[_lobbyCharacters.Count];
        for (int i = 0; i < _lobbyCharacters.Count; i++)
            names[i] = _lobbyCharacters[i] != null ? _lobbyCharacters[i].Name : "";
        _charNameManipulator.UpdateValues(names);
        EnableInterludeButtons();
        if (_lobbyCharacters.Count == 0)
        {
            SetEmptyData();
            ShowLobbyStatus(LobbyCharacterText.EmptyRosterStatus);
        }

        RefreshLobbyActions();
    }

    LobbyCharacter SelectedLobbyCharacter()
    {
        if (_lobbyCharacters == null || _lobbySelectIndex < 0 || _lobbySelectIndex >= _lobbyCharacters.Count)
            return null;
        return _lobbyCharacters[_lobbySelectIndex];
    }

    void EnableInterludeButtons()
    {
        if (_lobbyCharacters != null)
            return;
        if (_startButton != null)
            _startButton.SetEnabled(true);
        if (_deleteButton != null)
            _deleteButton.SetEnabled(true);
        if (_createButton != null)
            _createButton.SetEnabled(true);
        if (_deleteButtonLabel != null)
            _deleteButtonLabel.text = "Delete";
    }
}
