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

    /// <summary>
    /// Drop select-row visuals while still in lobby (roster refresh). Uses normal Destroy.
    /// World enter relies on Menu unload, not this.
    /// </summary>

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

    /// <summary>
    /// After Menu unload the scene objects are gone; only drop local refs.
    /// </summary>

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

    public void SpawnLobbyCharacterSlot(int index)
    {
        LobbyCharacter character = _lobbyCharacters[index];
        CharSelectInfoPackage info = ToSelectPackage(character);
        CharacterCreator creator = LoginPacketServices.Creator;
        if (creator == null)
        {
            _characterGameObjects.Add(null);
            return;
        }

        GameObject pawnObject = creator.CreatePawnInterlude(info.CharacterRaceAnimation, info.Appreance);
        _characterGameObjects.Add(pawnObject);
        if (pawnObject == null)
            return;

        SelectableCharacterEntity entity = pawnObject.GetComponent<SelectableCharacterEntity>();
        entity.CharacterInfoInterlude = info;
        UserGear gear = pawnObject.GetComponent<UserGear>();
        if (gear != null)
            entity.WeaponAnim = gear.WeaponAnim;
        creator.PlacePawn(pawnObject, _pawnData[index], character.Name, _container);
    }

    public void SelectLobbyCharacter(int index)
    {
        if (!_lobbyMode || _lobbyCharacters == null || index < 0 || index >= _lobbyCharacters.Count)
            return;
        if (_selectedLobbyIndex == index)
            return;
        if (_pawnData == null || _pawnData.Count < 8)
            return;

        if (_selectedLobbyIndex != -1)
            MoveLobbyPawn(_selectedLobbyIndex, _selectedLobbyIndex);

        MoveLobbyPawn(index, 7);
        _selectedLobbyIndex = index;
        _selectedCharacterSlot = _lobbyCharacters[index].Slot;
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.SelectLobbySlot(index);
    }

    static CharSelectInfoPackage ToSelectPackage(LobbyCharacter character)
    {
        CharSelectInfoPackage info = new CharSelectInfoPackage();
        info.Name = character.Name;
        info.Slot = character.Slot;
        info.ObjId = character.Id;
        info.ClassId = character.ClassId;
        info.Race = character.Race;
        info.Sex = character.Sex;
        info.Lvl = character.Level;
        info.HairStyle = character.HairStyle;
        info.HairColor = character.HairColor;
        info.Face = character.Face;
        info.Hp = character.CurHp;
        info.MaxHp = character.MaxHp;
        info.Mp = character.CurMp;
        info.MaxMp = character.MaxMp;
        info.Exp = character.Exp;
        info.Sp = character.Sp;
        info.Karma = character.Karma;
        ApplyLobbyPaperdoll(info.Appreance, character);
        int baseClass = (int)BaseClass.Fighter;
        if (CharacterClassParser.IsMage(character.ClassId))
            baseClass = (int)BaseClass.MMagic;
        info.Appreance.BaseClass = baseClass;
        CharacterRace race = character.Race >= 0 && character.Race <= (int)CharacterRace.Dwarf
            ? (CharacterRace)character.Race
            : MapClassId.GetCharacterRace(character.ClassId);
        info.CharacterRaceAnimation = CharacterRaceAnimationParser.ParseRaceInterlude(race, character.Sex, baseClass);
        return info;
    }

    /// <summary>
    /// Maps login paperdoll (or legacy WeaponItemId) onto CharSelect appearance.
    /// CreatePawnInterlude then SyncEquippedArmor + EquipWeapon from these fields.
    /// </summary>

    static void ApplyLobbyPaperdoll(PlayerAppearance appearance, LobbyCharacter character)
    {
        int[] doll = character.Paperdoll;
        if (doll != null && doll.Length > NetPaperdoll.Feet)
        {
            appearance.RHand = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.RightHand);
            appearance.LHand = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.LeftHand);
            appearance.Gloves = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.Gloves);
            appearance.Chest = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.Chest);
            appearance.Legs = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.Legs);
            appearance.Feet = NetPaperdoll.ItemIdAt(doll, NetPaperdoll.Feet);
        }

        if (appearance.RHand == 0 && character.WeaponItemId > 0)
            appearance.RHand = character.WeaponItemId;
    }

    void MoveLobbyPawn(int characterIndex, int pawnDataIndex)
    {
        if (_characterGameObjects == null || characterIndex < 0 || characterIndex >= _characterGameObjects.Count)
            return;
        GameObject pawn = _characterGameObjects[characterIndex];
        if (pawn == null || _pawnData == null || pawnDataIndex < 0 || pawnDataIndex >= _pawnData.Count)
            return;
        SelectableCharacterEntity entity = pawn.GetComponent<SelectableCharacterEntity>();
        if (entity != null)
            entity.SetDestination(_pawnData[pawnDataIndex]);
    }

    int IndexOfLobbySlot(int slot)
    {
        if (_lobbyCharacters == null)
            return -1;
        for (int i = 0; i < _lobbyCharacters.Count; i++)
        {
            if (_lobbyCharacters[i] != null && _lobbyCharacters[i].Slot == slot)
                return i;
        }

        return -1;
    }

}
