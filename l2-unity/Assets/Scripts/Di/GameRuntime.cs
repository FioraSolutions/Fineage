using VContainer;
using UnityEngine;

public sealed partial class GameRuntime
{
    private PlayerController _player;
    private PlayerActions _actions;
    private World _world;
    private PlayerInventory _inventory;
    private TargetManager _targets;
    private HitManager _hits;
    private ClickManager _clicks;
    private PlayerShortcuts _shortcuts;
    private InputManager _input;
    private PositionValidationController _positionValidation;
    private EffectManager _effects;
    private EventBus _bus;
    private IGameEventBus _gameEvents;
    private CameraController _camera;
    private IMoveAllCharacters _moves;
    private CombatFacingService _facing;
    private IAnimationManager _animations;
    private SkillExecutor _skillRunner;

    private EventProcessor _events;

    public EventProcessor Events => _events;

    public GameClient Game => DiRuntimePacketServices.Game;

    public LoginClient Login => DiRuntimePacketServices.Login;

    public GameManager Manager => DiRuntimePacketServices.Manager;

    public World World => _world;
    public PlayerInventory Inventory { get { return Live(ref _inventory, InventoryPacketServices.Inventory); } }
    public TargetManager Targets { get { return Live(ref _targets, TargetPacketServices.Targets); } }
    public HitManager Hits { get { return Live(ref _hits, HitPacketServices.Hits); } }
    public ClickManager Clicks { get { return Live(ref _clicks, ControllerPacketServices.Clicks); } }
    public PlayerShortcuts Shortcuts { get { return Live(ref _shortcuts, ShortcutPacketServices.Shortcuts); } }
    public InputManager Input { get { return Live(ref _input, ControllerPacketServices.Input); } }
    public PositionValidationController PositionValidation { get { return Live(ref _positionValidation, ControllerPacketServices.PositionValidation); } }
    public EffectManager Effects { get { return Live(ref _effects, GlobalEffectsPacketServices.Effects); } }
    public EventBus Bus { get { return Live(ref _bus, ControllerPacketServices.Bus); } }
    public IGameEventBus GameEvents => _gameEvents;
    public CameraController Camera { get { return Live(ref _camera, CameraPacketServices.Controller); } }
    public IMoveAllCharacters Moves { get { return Live(ref _moves, MoveAllCharactersPacketServices.Moves); } }
    public CombatFacingService Facing { get { return Live(ref _facing, CombatPacketServices.Facing); } }
    public IAnimationManager Animations { get { return Live(ref _animations, AnimationPacketServices.Animations); } }
    public SkillExecutor SkillRunner { get { return Live(ref _skillRunner, SkillExecutorPacketServices.Executor); } }

    public WorldPacketApply WorldApply { get; private set; }
    public MessagePacketApply MessageApply { get; private set; }

    public PlayerController Player => _player;

    public PlayerActions Actions { get { return Live(ref _actions, ShortcutPacketServices.Actions); } }



    private static T Live<T>(ref T field, T instance) where T : class
    {
        if (field == null)
            field = instance;
        return field;
    }
}
