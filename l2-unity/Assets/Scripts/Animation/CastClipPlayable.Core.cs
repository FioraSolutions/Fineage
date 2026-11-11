using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
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
}
