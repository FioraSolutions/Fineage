using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine;

public sealed partial class CastClipPlayable
{
    public void CrossFadeInFixedTime(int stateHash, float fixedDuration, int layer, float fixedTimeOffset)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.CrossFadeInFixedTime(stateHash, fixedDuration, layer, fixedTimeOffset);
    }

    public void CrossFade(int stateHash, float duration, int layer)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.CrossFade(stateHash, duration, layer, 0f);
    }

    public void SetFloat(string name, float value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetBool(name, value);
    }

    public void SetInteger(string name, int value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetInteger(name, value);
    }

    public void SetTrigger(string name)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetTrigger(name);
    }

    public void ResetTrigger(string name)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.ResetTrigger(name);
    }

    public void SetControllerSpeed(float value)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.SetSpeed(value);
    }
}
