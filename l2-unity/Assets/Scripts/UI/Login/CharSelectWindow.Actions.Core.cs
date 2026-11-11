using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using VContainer;

public partial class CharSelectWindow
{
    public CharSelectInfoPackage GetSelectChar()
    {
        return _selectChar;
    }

    private void StartGamePressed()
    {
        LobbyCharacter selected = SelectedLobbyCharacter();
        if (!LobbyCharacterText.CanEnter(selected))
            return;
        if (_startButton != null)
            _startButton.SetEnabled(false);
        _ = NetSession.Ensure().SelectCharacter(selected.Id, selected.Name, selected.Slot);
    }
}
