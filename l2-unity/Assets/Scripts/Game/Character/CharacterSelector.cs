using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int _selectedCharacterSlot;
    [SerializeField] private CharSelectionInfoPackage _selectedCharacter;
    [SerializeField] private CharSelectInfoPackage _selectedCharacterInterlude;
    [SerializeField] private List<CharSelectionInfoPackage> _characters;
    [SerializeField] private List<CharSelectInfoPackage> _charactersInterlude;
    private List<LobbyCharacter> _lobbyCharacters;
    private bool _lobbyMode;
    private int _selectedLobbyIndex = -1;
    [SerializeField] private LayerMask _characterMask;
    [SerializeField] private Camera _charSelectCamera;
    private Dictionary<int, float> dict;
    private GameObject _container;
    private List<Logongrp> _pawnData;
    private List<GameObject> _characterGameObjects;

    [Inject] private LogongrpTable _logonGrps;
    [Inject] private GameClient _gameClient;

    private GameClient Game
    {
        get { return _gameClient != null ? _gameClient : CharacterPacketServices.Game; }
    }

    private List<Logongrp> LogonGrps
    {
        get
        {
            LogongrpTable table = _logonGrps != null ? _logonGrps : LogongrpTable.Instance;
            return table.LogonGrps;
        }
    }

    public Camera Camera { get { return _charSelectCamera; } set { _charSelectCamera = value; } }
    public int SelectedSlot { get { return _selectedCharacterSlot; } }

    public CharSelectInfoPackage SelectedInterludeCharacter => _selectedCharacterInterlude;

    /// <summary>Spawned lobby pawns (Interlude / legacy select row).</summary>
    public IReadOnlyList<GameObject> CharacterPawns => _characterGameObjects;


    private static CharacterSelector _instance;
    public static CharacterSelector Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
            dict = new Dictionary<int, float>();
            App.InjectGameObject(gameObject);
            WireCharacterPacketServices();
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireCharacterPacketServices();
    }

    private void WireCharacterPacketServices()
    {
        CharacterPacketServices.Bind(game: _gameClient);
    }

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

        ForgetLobbyVisuals();
    }

    /// <summary>
    /// After Menu unload the scene objects are gone; only drop local refs.
    /// </summary>
    public void ForgetLobbyVisuals()
    {
        _container = null;
        _characterGameObjects = new List<GameObject>();
        _selectedCharacterSlot = -1;
        _selectedLobbyIndex = -1;
        _charSelectCamera = null;
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
        CharacterCreator creator = CharacterCreator.Instance;
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
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SelectLobbySlot(index);
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
        if (character.ClassId >= 0 && character.ClassId <= byte.MaxValue &&
            CharacterClassParser.IsMage((CharacterClass)character.ClassId))
            baseClass = (int)BaseClass.MMagic;
        info.Appreance.BaseClass = baseClass;
        CharacterRace race = character.Race >= 0 && character.Race <= (int)CharacterRace.Dwarf
            ? (CharacterRace)character.Race
            : CharacterRace.Human;
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
        if (doll != null && doll.Length > ZoneV2Paperdoll.Feet)
        {
            appearance.RHand = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.RightHand);
            appearance.LHand = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.LeftHand);
            appearance.Gloves = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.Gloves);
            appearance.Chest = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.Chest);
            appearance.Legs = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.Legs);
            appearance.Feet = ZoneV2Paperdoll.ItemIdAt(doll, ZoneV2Paperdoll.Feet);
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

   

    public void SpawnInterludeCharacterSlot(int id)
    {
        CharSelectInfoPackage info = _charactersInterlude[id];
        GameObject pawnObject = CharacterCreator.Instance.CreatePawnInterlude(info.CharacterRaceAnimation, info.Appreance);
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
        CharacterCreator.Instance.PlacePawn(pawnObject, _pawnData[id], name, _container);
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
        GameObject pawnObject = CharacterCreator.Instance.CreatePawn(_characters[id].CharacterRaceAnimation, _characters[id].PlayerAppearance);
        pawnObject.GetComponent<SelectableCharacterEntity>().CharacterInfo = _characters[id];
        pawnObject.GetComponent<SelectableCharacterEntity>().WeaponAnim = pawnObject.GetComponent<UserGear>().WeaponAnim;
        CharacterCreator.Instance.PlacePawn(pawnObject, _pawnData[id], _characters[id].Name, _container);
        _characterGameObjects.Add(pawnObject);
    }

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

            CharSelectWindow.Instance.SelectSlot(slot);
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

            CharSelectWindow.Instance.SelectInterludeSlot(slot);
        }
    }

    public void ConfirmSelection()
     {
        if (SelectedSlot == -1) {
            // Debug.LogWarning("Please select a character");
            return;
        }
        GameClient game = Game;
        if (game != null)
            game.Send(new CharacterSelectCommand(SelectedSlot));
       // GameClient.Instance.ClientPacketHandler.SendRequestSelectCharacter(SelectedSlot);
    }

    public void TryToDeleteCharacter()
    {
          if (SelectedSlot == -1)
          {
                // Debug.LogWarning("Please select a character");
                return;
            }
            GameClient game = Game;
            if (game != null)
                game.Send(new CharacterDeleteCommand(SelectedSlot));
            // Debug.LogWarning("Requesting delete character , slot: " + SelectedSlot);
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


    void Update() {
        if(_charSelectCamera == null) {
            return;
        }


        if(Input.GetMouseButtonDown(0)) {
            Ray ray = _charSelectCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f)) {
                int hitLayer = hit.collider.gameObject.layer;
                if (_characterMask == (_characterMask | (1 << hitLayer))) {
                    SelectableCharacterEntity entity = hit.transform.parent.GetComponent<SelectableCharacterEntity>();
                    if (entity == null || entity.CharacterInfoInterlude == null)
                        return;
                    if (_lobbyMode)
                    {
                        int index = IndexOfLobbySlot(entity.CharacterInfoInterlude.Slot);
                        if (index >= 0)
                            SelectLobbyCharacter(index);
                    }
                    else
                    {
                        SelectInterludeCharacter(entity.CharacterInfoInterlude.Slot);
                    }
                }
            }
        }
    }
}
