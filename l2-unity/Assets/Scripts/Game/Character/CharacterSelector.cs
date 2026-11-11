using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector : MonoBehaviour
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

    private List<Logongrp> LogonGrps => TablePacketServices.LogonGrps.LogonGrps;

    public Camera Camera { get { return _charSelectCamera; } set { _charSelectCamera = value; } }
    public int SelectedSlot { get { return _selectedCharacterSlot; } }

    public CharSelectInfoPackage SelectedInterludeCharacter => _selectedCharacterInterlude;

    /// <summary>Spawned lobby pawns (Interlude / legacy select row).</summary>
    public IReadOnlyList<GameObject> CharacterPawns => _characterGameObjects;


    private static CharacterSelector _instance;
    [System.Obsolete("Use LoginPacketServices.Characters instead")]
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
        CharacterPacketServices.Bind();
        TablePacketServices.Bind(logonGrps: _logonGrps);
    }
}
