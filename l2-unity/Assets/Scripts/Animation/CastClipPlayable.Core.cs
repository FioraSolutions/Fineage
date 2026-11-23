using System;
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

        MotionGraphAvatar.Ensure(animator);
        if (animator.avatar == null || !animator.avatar.isValid)
        {
            throw new InvalidOperationException(
                "[MotionPipeline] Animator '" + animator.name +
                "' has no Avatar — the shared motion graph was not bound, so a clip would inflate the mesh.");
        }

        // Sample bind scale, then put the controller back so a mid-walk cast does not restart it.
        AnimatorStateInfo playing = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorControllerParameter[] parameters = animator.parameters;
        var saved = new ParameterSample[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            saved[i] = new ParameterSample
            {
                hash = parameter.nameHash,
                type = parameter.type,
                floatValue = parameter.type == AnimatorControllerParameterType.Float
                    ? animator.GetFloat(parameter.nameHash) : 0f,
                intValue = parameter.type == AnimatorControllerParameterType.Int
                    ? animator.GetInteger(parameter.nameHash) : 0,
                boolValue = parameter.type == AnimatorControllerParameterType.Bool
                    && animator.GetBool(parameter.nameHash),
            };
        }

        animator.Rebind();
        var boneHandles = new System.Collections.Generic.List<TransformStreamHandle>();
        var boneScales = new System.Collections.Generic.List<Vector3>();
        MotionBoneScale.BindBones(animator, boneHandles, boneScales);

        for (int i = 0; i < saved.Length; i++)
        {
            ParameterSample sample = saved[i];
            switch (sample.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(sample.hash, sample.floatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(sample.hash, sample.intValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(sample.hash, sample.boolValue);
                    break;
            }
        }

        if (playing.fullPathHash != 0)
        {
            animator.Play(playing.fullPathHash, 0, playing.normalizedTime);
            animator.Update(0f);
        }

        _graph = PlayableGraph.Create(animator.name + ".CastClip");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        try
        {
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            _controllerPlayable = AnimatorControllerPlayable.Create(_graph, animator.runtimeAnimatorController);
            _graph.Connect(_controllerPlayable, 0, _mixer, 0);
            _mixer.SetInputWeight(0, 1f);
            _mixer.SetInputWeight(1, 0f);

            _scaleLock = MotionBoneScale.Create(_graph, boneHandles, boneScales);
            _graph.Connect(_mixer, 0, _scaleLock.Playable, 0);
            _scaleLock.Playable.SetInputWeight(0, 1f);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "CastClip", animator);
            output.SetSourcePlayable(_scaleLock.Playable);
            _graph.Play();
            _ready = true;
            MatchAnimatorState();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    struct ParameterSample
    {
        public int hash;
        public AnimatorControllerParameterType type;
        public float floatValue;
        public int intValue;
        public bool boolValue;
    }
}
