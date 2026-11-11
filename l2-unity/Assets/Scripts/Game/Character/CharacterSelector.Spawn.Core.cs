using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector
{
    public void SetCharacterList(List<CharSelectionInfoPackage> characters) {
        ClearLobbyVisuals();
        EnsureCharacterContainer();

        _characters = characters;
        _pawnData = LogonGrps;
        _selectedCharacterSlot = -1;

        for (int i = 0; i < characters.Count; i++) {
            SpawnCharacterSlot(i);
        }
    }

    public void SetCharacterInterludeList(List<CharSelectInfoPackage> characters)
    {
        ClearLobbyVisuals();
        EnsureCharacterContainer();

        _charactersInterlude = characters;
        _pawnData = LogonGrps;
        _selectedCharacterSlot = -1;

        for (int i = 0; i < characters.Count; i++)
        {
            SpawnInterludeCharacterSlot(i);
        }
    }
}
