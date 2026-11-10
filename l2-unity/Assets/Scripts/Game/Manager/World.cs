using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnContext {
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerPlaceholder;
    [SerializeField] private GameObject _userPlaceholder;
    [SerializeField] private GameObject _npcPlaceHolder;
    [SerializeField] private GameObject _monsterPlaceholder;

    [SerializeField] private GameObject _monstersContainer;
    [SerializeField] private GameObject _npcsContainer;
    [SerializeField] private GameObject _usersContainer;
    [SerializeField] private GameObject _itemsContainer;

    [Inject] private EventProcessor _eventProcessor;
    [Inject] private Geodata _geodata;
    [Inject] private WorldCombat _worldCombat;
    [Inject] private ObjectPoolManager _objectPool;
    [Inject] private GravityNpc _gravityNpc;
    [Inject] private DeadManager _deadManager;
    [Inject] private AppearFadeService _appearFade;
    [Inject] private CameraController _camera;
    [Inject] private IAnimationManager _animations;
    [Inject] private PlayerSpawner _playerSpawner;
    [Inject] private UserSpawner _userSpawner;
    [Inject] private NpcSpawner _npcSpawner;
    [Inject] private MonsterSpawner _monsterSpawner;
    [Inject] private ItemSpawner _itemSpawner;
    [Inject] private ItemDropPresentationService _itemDropPresentation;
    [Inject] private NpcDecoService _npcDeco;
    [Inject] private ItemDropLayerService _dropLayers;
    [Inject] private NpcgrpTable _npcGrps;
    [Inject] private NpcNameTable _npcNames;
    [Inject] private ModelTable _models;
    [Inject] private CharacterInfoWindow _characterInfo;
    [Inject] private GameManager _gameManager;
    [Inject] private PacketApplyQueue _applyQueue;

    private CharacterInfoWindow CharacterInfo
    {
        get { return _characterInfo != null ? _characterInfo : HudCorePacketServices.CharacterInfo; }
    }

    public IAnimationManager Animations
    {
        get { return _animations != null ? _animations : WorldPacketServices.Animations; }
    }

    public Transform UsersContainer
    {
        get { return _usersContainer != null ? _usersContainer.transform : null; }
    }

    public Transform NpcsContainer
    {
        get { return _npcsContainer != null ? _npcsContainer.transform : null; }
    }

    public Transform MonstersContainer
    {
        get { return _monstersContainer != null ? _monstersContainer.transform : null; }
    }

    public Transform ItemsContainer
    {
        get { return _itemsContainer != null ? _itemsContainer.transform : null; }
    }

    private Dictionary<int, Entity> _players = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _npcs = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _objects = new Dictionary<int, Entity>();


    [Header("Layer Masks")]
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _entityClickAreaMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _clickThroughMask;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private bool _offlineMode = false;

    public bool OfflineMode { get { return _offlineMode; } }
    public LayerMask GroundMask { get { return _groundMask; } }

    private static World _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static World Instance { get { return _instance; } }

    static bool ShouldEnsureGameScope()
    {
        if (SystemUIPacketServices.Ui != null)
            return true;

        GameManager manager = WorldPacketServices.Manager ?? GamePacketServices.Manager;
        if (manager != null && manager.GameState == GameState.LOGIN_SCREEN)
            return false;

        return true;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
            return;
        }

        if (ShouldEnsureGameScope())
            DiBootstrap.EnsureGameScope();
        // Prefer [Inject] EventProcessor; NetworkingPacketServices is the #142 fallback bridge.
        if (_eventProcessor == null)
            _eventProcessor = NetworkingPacketServices.Events;
        _playerPlaceholder = Resources.Load<GameObject>("Prefab/Player_FDarkElf");
        _userPlaceholder = Resources.Load<GameObject>("Prefab/User_FDarkElf");
        _npcPlaceHolder = Resources.Load<GameObject>("Prefab/Npc");
        _monsterPlaceholder = Resources.Load<GameObject>("Data/Animations/LineageMonsters/gremlin_m00/gremlin_m00");
        _npcsContainer = GameObject.Find("Npcs");
        _monstersContainer = GameObject.Find("Monsters");
        _usersContainer = GameObject.Find("Users");
        _itemsContainer = GameObject.Find("Items");
        if (_itemsContainer == null)
        {
            _itemsContainer = new GameObject("Items");
            _itemsContainer.transform.SetParent(transform, false);
        }

        BindItemDropPresentationRunner();
    }

    [Inject]
    void BindItemDropPresentationRunner()
    {
        ItemDropPresentationRunner runner = GetComponent<ItemDropPresentationRunner>();
        if (runner == null)
            runner = gameObject.AddComponent<ItemDropPresentationRunner>();
        if (_itemDropPresentation != null)
            runner.Bind(_itemDropPresentation);
    }

    void OnDestroy() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove -= RemoveObject;
        _instance = null;
    }

    void Start() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove += RemoveObject;
        UpdateMasks();
    }

    public void UpdateMasks() {
        NameplatesManager nameplates = NameplatePacketServices.Nameplates;
        if (nameplates != null)
            nameplates.SetMask(_entityMask);
        if (_geodata != null)
            _geodata.ObstacleMask = _obstacleMask;
        ClickManager clicks = ControllerPacketServices.Clicks;
        if (clicks != null)
        {
            LayerMask dropMask = _dropLayers != null ? _dropLayers.Mask : 0;
            clicks.SetMasks(_entityClickAreaMask | dropMask, _clickThroughMask);
        }
        if (_camera != null)
            _camera.SetMask(_obstacleMask);
    }

    public void ClearEntities() {
        _objects.Clear();
        _players.Clear();
        _npcs.Clear();
    }

    public bool ContainsNpc(int id)
    {
        return _npcs.ContainsKey(id);
    }
}
