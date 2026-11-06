using System;
using VContainer;

/// <summary>
/// Controller-layer access to game services with [Inject] wiring and live/peer fallback (#142).
/// </summary>
public static class ControllerPacketServices
{
    private static World _gameWorld;
    private static PlayerController _player;
    private static TargetManager _targets;
    private static InputManager _input;
    private static IAnimationManager _animations;
    private static IMoveAllCharacters _moves;
    private static EffectManager _effects;
    private static EffectSkillsmanager _effectSkills;
    private static SkillsManager _skillCombos;
    private static BufferPanel _buffer;
    private static DeadWindow _dead;
    private static EventBus _bus;
    private static PositionValidationController _positionValidation;

    public static void Bind(
        World gameWorld = null,
        PlayerController player = null,
        TargetManager targets = null,
        InputManager input = null,
        IAnimationManager animations = null,
        IMoveAllCharacters moves = null,
        EffectManager effects = null,
        EffectSkillsmanager effectSkills = null,
        SkillsManager skillCombos = null,
        BufferPanel buffer = null,
        DeadWindow dead = null,
        EventBus bus = null,
        PositionValidationController positionValidation = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (player != null)
            _player = player;
        if (targets != null)
            _targets = targets;
        if (input != null)
            _input = input;
        if (animations != null)
            _animations = animations;
        if (moves != null)
            _moves = moves;
        if (effects != null)
            _effects = effects;
        if (effectSkills != null)
            _effectSkills = effectSkills;
        if (skillCombos != null)
            _skillCombos = skillCombos;
        if (buffer != null)
            _buffer = buffer;
        if (dead != null)
            _dead = dead;
        if (bus != null)
            _bus = bus;
        if (positionValidation != null)
            _positionValidation = positionValidation;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;

    public static TargetManager Targets =>
        _targets != null
            ? _targets
            : ResolveFromGame<TargetManager>() ?? LegacyTargetManagerInstance();

    public static InputManager Input =>
        _input != null
            ? _input
            : ResolveFromGame<InputManager>() ?? InputManager.Instance;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static IMoveAllCharacters Moves =>
        _moves != null
            ? _moves
            : ResolveFromGame<IMoveAllCharacters>() ?? MoveAllCharacters.Instance;

    public static EffectManager Effects =>
        _effects != null
            ? _effects
            : ResolveFromGame<EffectManager>() ?? EffectManager.Instance;

    public static EffectSkillsmanager EffectSkills =>
        _effectSkills != null ? _effectSkills : GamePacketServices.EffectSkills;

    public static SkillsManager SkillCombos =>
        _skillCombos != null
            ? _skillCombos
            : ResolveFromGame<SkillsManager>() ?? SkillsManager.Instance;

    public static BufferPanel Buffer =>
        _buffer != null
            ? _buffer
            : ResolveFromGame<BufferPanel>() ?? BufferPanel.Instance;

    public static DeadWindow Dead =>
        _dead != null
            ? _dead
            : ResolveFromGame<DeadWindow>() ?? DeadWindow.Instance;

    public static EventBus Bus =>
        _bus != null ? _bus : GamePacketServices.Bus;

    public static PositionValidationController PositionValidation =>
        _positionValidation != null
            ? _positionValidation
            : ResolveFromGame<PositionValidationController>()
              ?? LegacyPositionValidationInstance();

    public static void Queue(Action action) => NetworkingPacketServices.Queue(action);

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static TargetManager LegacyTargetManagerInstance() => TargetManager.Instance;

    static PositionValidationController LegacyPositionValidationInstance() =>
        PositionValidationController.Instance;
#pragma warning restore CS0618
}

/// <summary>
/// Wires <see cref="ControllerPacketServices"/> from VContainer on the player controller.
/// </summary>
public sealed partial class PlayerController
{
    [Inject] World _packetWorld;
    [Inject] TargetManager _packetTargets;
    [Inject] InputManager _packetInput;
    [Inject] IAnimationManager _packetAnimations;
    [Inject] MoveAllCharacters _packetMoves;
    [Inject] EffectManager _packetEffects;
    [Inject] EffectSkillsmanager _packetEffectSkills;
    [Inject] SkillsManager _packetSkillCombos;
    [Inject] BufferPanel _packetBuffer;
    [Inject] DeadWindow _packetDead;
    [Inject] EventBus _packetBus;
    [Inject] PlayerPositionSender _injectedPositionSender;
    [Inject] ClickManager _packetClicks;

    void WireControllerPacketServices()
    {
        PlayerEntity localEntity = GetComponent<PlayerEntity>();
        if (localEntity != null)
            DomainPacketServices.Bind(localPlayer: localEntity);

        ControllerPacketServices.Bind(
            gameWorld: _packetWorld,
            player: this,
            targets: _packetTargets,
            input: _packetInput,
            animations: _packetAnimations,
            moves: _packetMoves,
            effects: _packetEffects,
            effectSkills: _packetEffectSkills,
            skillCombos: _packetSkillCombos,
            buffer: _packetBuffer,
            dead: _packetDead,
            bus: _packetBus);
    }
}
