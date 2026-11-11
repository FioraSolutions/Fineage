using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnContext
{
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








}
