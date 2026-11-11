using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector
{
    public void SelectCharacter(int slot) {
        if (slot >= 0 && slot < _characters.Count) {
            if (_selectedCharacterSlot == slot) {
                return;
            }

            if(_selectedCharacterSlot != -1) {
                _characterGameObjects[_selectedCharacterSlot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[_selectedCharacterSlot]);
            }

            _characterGameObjects[slot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[7]);

            _selectedCharacterSlot = slot;
            _selectedCharacter = _characters[slot];

            LoginPacketServices.CharSelect.SelectSlot(slot);
        }
    }

    public void SelectInterludeCharacter(int slot)
    {
        if (slot >= 0 && slot < _charactersInterlude.Count)
        {
            if (_selectedCharacterSlot == slot)
            {
                return;
            }

            if (_selectedCharacterSlot != -1)
            {
                _characterGameObjects[_selectedCharacterSlot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[_selectedCharacterSlot]);
            }

            _characterGameObjects[slot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[7]);

            _selectedCharacterSlot = slot;
            _selectedCharacterInterlude = _charactersInterlude[slot];

            LoginPacketServices.CharSelect.SelectInterludeSlot(slot);
        }
    }
}
