using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// Plays a race cast clip on top of the existing animator controller.
/// Input 1 is the current CastMid / CastEnd / MagicShot clip. Swapping that
/// input does not write AnimatorOverrideController, so the controller is not rebuilt.
/// The graph is created once per body and kept. Rebuilding it per cast stalls
/// a crowd of CharInfo. While it is bound, the mesh is the mixer: input 0 is
/// this controller playable, input 1 is the cast clip. Locomotion must be
/// written here as well as on the Animator — the Animator crossfade does not
/// move this playable, so the body stays on the pose from Bind.
/// </summary>
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


    public void Bind(BaseAnimationController owner, Animator animator)
    {
        Dispose();
        _owner = owner;
        _animator = animator;
        if (owner == null || animator == null || animator.runtimeAnimatorController == null)
        {
            return;
        }

        _graph = PlayableGraph.Create(animator.name + ".CastClip");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        _mixer = AnimationMixerPlayable.Create(_graph, 2);
        _controllerPlayable = AnimatorControllerPlayable.Create(_graph, animator.runtimeAnimatorController);
        _graph.Connect(_controllerPlayable, 0, _mixer, 0);
        _mixer.SetInputWeight(0, 1f);
        _mixer.SetInputWeight(1, 0f);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "CastClip", animator);
        output.SetSourcePlayable(_mixer);
        _graph.Play();
        _ready = true;
        MatchAnimatorState();
    }

    public void Dispose()
    {
        _active = false;
        _finished = true;
        _ready = false;
        if (_graph.IsValid())
        {
            _graph.Destroy();
        }

        _clipPlayable = default;
        _controllerPlayable = default;
        _mixer = default;
    }
}
