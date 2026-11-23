using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnRegistration, IWorldEntityLookup
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

    [Inject] private IEventQueue _eventQueue;
    [Inject] private Geodata _geodata;
    [Inject] private WorldCombat _worldCombat;
    [Inject] private ObjectPoolManager _objectPool;
    [Inject] private GravityNpc _gravityNpc;
    [Inject] private DeadManager _deadManager;
    [Inject] private AppearFadeService _appearFade;
    [Inject] private ICameraCollisionMask _camera;
    [Inject] private IAnimationManager _animations;
    [Inject] private IAnimationRoster _animationRoster;
    [Inject] private IVisibilityRoster _visibility;
    [Inject] private IEntityActionRoster _entityActions;
    [Inject] private IMoveRoster _moveRoster;
    [Inject] private INameplateRoster _nameplates;
    [Inject] private IClickMasks _clicks;
    [Inject] private IPlayerRebirth _playerRebirth;
    [Inject] private ILocalPlayerSlot _localPlayer;
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
    [Inject] private ICharacterInfoValues _characterInfo;
    [Inject] private IGameManager _gameManager;
    [Inject] private WorldPacketApply _worldApply;
    [Inject] private PacketApplyQueue _applyQueue;

    public IAnimationManager Animations
    {
        get { return _animations; }
    }

    public void RegisterSpawnAnimation(int objectId, ISpawnAnimator animator, IWorldEntity entity)
    {
        if (animator == null)
            return;

        if (animator is not IAnimationController controller)
        {
            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterSpawnAnimation is not an IAnimationController — the spawner passed a different type, so the animation was not registered.");
        }

        if (entity != null && entity is not Entity)
        {
            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterSpawnAnimation is not an Entity — the spawner passed a different type, so the animation was not registered.");
        }

        if (_animations == null)
        {
            throw new InvalidOperationException(
                "[World] IAnimationManager missing — AppLifetimeScope did not register AnimationManager, so the spawned animation was not registered.");
        }

        _animations.RegisterController(objectId, controller, entity as Entity);
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

    /// <summary>
    /// Default wait for an entity that has not been registered yet.
    /// Kept near the historical 150ms poll so late packets still time out instead of hanging.
    /// </summary>
    public static readonly TimeSpan EntityWaitTimeout = TimeSpan.FromMilliseconds(150);

    readonly ObjectIdWaitMap<Entity> _entityWait = new ObjectIdWaitMap<Entity>();
    readonly WorldEntityRegistry<Entity> _entities = new WorldEntityRegistry<Entity>();


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
