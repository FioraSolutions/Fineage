using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
    static IAnimationManager Animations => AnimationPacketServices.Animations;

    private const float PhaseDoneNormalizedTime = 1f;

    private BaseAnimationController _owner;
    private Animator _animator;
    private PlayableGraph _graph;
    private AnimationMixerPlayable _mixer;
    private AnimatorControllerPlayable _controllerPlayable;
    private AnimationClipPlayable _clipPlayable;
    private bool _ready;

    private bool _active;
    private bool _finished;
    private bool _finalShot;
    private int _stateIndex;
    private int _objectId;
    private float _speed = 1f;
    private float _clipLength;
    private float _enterTime;
    private float _lastSample;
    private string _finishName;
    private MagicCastData _castData;
    private AnimationEvent[] _events;
    private bool[] _eventFired;
    bool _oneShot;

    /// <summary>Graph already exists. TryPlay must not Bind again on later casts.</summary>
    public bool IsGraphAlive => _ready && _graph.IsValid() && _controllerPlayable.IsValid();

    public bool IsActive => _active;
}
