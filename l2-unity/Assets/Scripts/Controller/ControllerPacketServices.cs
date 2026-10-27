using System;
using VContainer;

/// <summary>
/// Controller-layer access to game services with [Inject] wiring and IncomingPacketActions fallback (#142).
/// </summary>
public static class ControllerPacketServices
{
    private static World _gameWorld;
    private static PlayerController _player;
    private static TargetManager _targets;
    private static InputManager _input;
    private static GameClient _game;
    private static IAnimationManager _animations;
    private static IMoveAllCharacters _moves;
    private static EffectManager _effects;
    private static EffectSkillsmanager _effectSkills;
    private static SkillsManager _skillCombos;
    private static BufferPanel _buffer;
    private static DeadWindow _dead;
    private static EventBus _bus;

    public static void Bind(
        World gameWorld = null,
        PlayerController player = null,
        TargetManager targets = null,
        InputManager input = null,
        GameClient game = null,
        IAnimationManager animations = null,
        IMoveAllCharacters moves = null,
        EffectManager effects = null,
        EffectSkillsmanager effectSkills = null,
        SkillsManager skillCombos = null,
        BufferPanel buffer = null,
        DeadWindow dead = null,
        EventBus bus = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (player != null)
            _player = player;
        if (targets != null)
            _targets = targets;
        if (input != null)
            _input = input;
        if (game != null)
            _game = game;
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
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static PlayerController Player =>
        _player != null ? _player : IncomingPacketActions.Player;

    public static TargetManager Targets =>
        _targets != null ? _targets : IncomingPacketActions.Targets;

    public static InputManager Input =>
        _input != null ? _input : IncomingPacketActions.Input;

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;

    public static IMoveAllCharacters Moves =>
        _moves != null ? _moves : IncomingPacketActions.Moves;

    public static EffectManager Effects =>
        _effects != null ? _effects : IncomingPacketActions.Effects;

    public static EffectSkillsmanager EffectSkills =>
        _effectSkills != null ? _effectSkills : IncomingPacketActions.EffectSkills;

    public static SkillsManager SkillCombos =>
        _skillCombos != null ? _skillCombos : IncomingPacketActions.SkillCombos;

    public static BufferPanel Buffer =>
        _buffer != null ? _buffer : IncomingPacketActions.Buffer;

    public static DeadWindow Dead =>
        _dead != null ? _dead : IncomingPacketActions.Dead;

    public static EventBus Bus =>
        _bus != null ? _bus : IncomingPacketActions.Bus;

    public static void Queue(Action action) => IncomingPacketActions.Queue(action);
}

/// <summary>
/// Wires <see cref="ControllerPacketServices"/> from VContainer on the player controller.
/// </summary>
public sealed partial class PlayerController
{
    [Inject] World _packetWorld;
    [Inject] TargetManager _packetTargets;
    [Inject] InputManager _packetInput;
    [Inject] GameClient _packetGame;
    [Inject] IAnimationManager _packetAnimations;
    [Inject] MoveAllCharacters _packetMoves;
    [Inject] EffectManager _packetEffects;
    [Inject] EffectSkillsmanager _packetEffectSkills;
    [Inject] SkillsManager _packetSkillCombos;
    [Inject] BufferPanel _packetBuffer;
    [Inject] DeadWindow _packetDead;
    [Inject] EventBus _packetBus;
    [Inject] PlayerPositionSender _injectedPositionSender;

    void WireControllerPacketServices()
    {
        ControllerPacketServices.Bind(
            gameWorld: _packetWorld,
            player: this,
            targets: _packetTargets,
            input: _packetInput,
            game: _packetGame,
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
