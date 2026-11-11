using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine;

public sealed partial class CastClipPlayable
{
    public void Stop()
    {
        _oneShot = false;
        MagicGaitTrace.EventId(_objectId, "CastClip.Stop",
            "finishName=" + _finishName + " finalShot=" + _finalShot + " keepGraph=True");
        _active = false;
        _finished = true;
        if (!_ready)
        {
            return;
        }

        _mixer.SetInputWeight(0, 1f);
        _mixer.SetInputWeight(1, 0f);
        if (_clipPlayable.IsValid())
        {
            _clipPlayable.SetSpeed(0f);
        }

        if (_animator != null)
        {
            _animator.speed = 1f;
        }

        if (_controllerPlayable.IsValid())
        {
            _controllerPlayable.SetSpeed(1f);
        }
    }

    void MatchAnimatorState()
    {
        if (!_controllerPlayable.IsValid() || _animator == null)
        {
            return;
        }

        AnimatorControllerParameter[] parameters = _animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    _controllerPlayable.SetFloat(parameter.nameHash, _animator.GetFloat(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Int:
                    _controllerPlayable.SetInteger(parameter.nameHash, _animator.GetInteger(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Bool:
                    _controllerPlayable.SetBool(parameter.nameHash, _animator.GetBool(parameter.nameHash));
                    break;
            }
        }

        _controllerPlayable.SetSpeed(1f);
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        _controllerPlayable.Play(state.fullPathHash, 0, state.normalizedTime);
    }
}
