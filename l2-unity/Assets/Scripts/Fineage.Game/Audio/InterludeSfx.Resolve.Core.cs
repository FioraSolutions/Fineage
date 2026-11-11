using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class InterludeSfx
{
    public static void PlayAfter(float delaySeconds, System.Action action)
    {
        if (action == null)
            return;
        if (delaySeconds <= 0f)
        {
            action();
            return;
        }
        InterludeSfxRunner runner = EnsureRunner();
        if (runner == null)
        {
            action();
            return;
        }
        runner.StartCoroutine(DelayThen(delaySeconds, action));
    }

    static IEnumerator DelayThen(float delay, System.Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action();
    }

    static AudioSource EnsureOneShotSource()
    {
        EnsureRunner();
        if (_oneShotSrc != null)
            return _oneShotSrc;
        _oneShotSrc = CreateChildSource("OneShot");
        return _oneShotSrc;
    }
}
