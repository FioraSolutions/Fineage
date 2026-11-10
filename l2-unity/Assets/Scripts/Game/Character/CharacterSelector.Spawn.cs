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

    public void SpawnInterludeCharacterSlot(int id)
    {
        CharSelectInfoPackage info = _charactersInterlude[id];
        CharacterCreator creator = LoginPacketServices.Creator;
        if (creator == null)
            return;
        GameObject pawnObject = creator.CreatePawnInterlude(info.CharacterRaceAnimation, info.Appreance);
        if (pawnObject == null)
        {
            // LobbyFlowLog.Error(
                // "lobby pawn null slot=" + id + " name=" + info.Name +
                // " raceAnim=" + info.CharacterRaceAnimation + " — skip (would NRE and freeze char select)");
            return;
        }

        pawnObject.GetComponent<SelectableCharacterEntity>().CharacterInfoInterlude = info;
        pawnObject.GetComponent<SelectableCharacterEntity>().WeaponAnim = pawnObject.GetComponent<UserGear>().WeaponAnim;
        string name = info.Name;
        creator.PlacePawn(pawnObject, _pawnData[id], name, _container);
        _characterGameObjects.Add(pawnObject);
        // LobbyFlowLog.Info("lobby pawn spawned slot=" + id + " name=" + name);
    }

    Bounds GetMaxBounds(GameObject parent)
    {
        var total = new Bounds(parent.transform.position, Vector3.zero);
        foreach (var child in parent.GetComponentsInChildren<Collider>())
        {
            total.Encapsulate(child.bounds);
        }
        return total;
    }

    public void SpawnCharacterSlot(int id) {
        CharacterCreator creator = LoginPacketServices.Creator;
        if (creator == null)
            return;
        GameObject pawnObject = creator.CreatePawn(_characters[id].CharacterRaceAnimation, _characters[id].PlayerAppearance);
        pawnObject.GetComponent<SelectableCharacterEntity>().CharacterInfo = _characters[id];
        pawnObject.GetComponent<SelectableCharacterEntity>().WeaponAnim = pawnObject.GetComponent<UserGear>().WeaponAnim;
        creator.PlacePawn(pawnObject, _pawnData[id], _characters[id].Name, _container);
        _characterGameObjects.Add(pawnObject);
    }

}
