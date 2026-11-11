using System.Collections;
using UnityEngine;

public sealed partial class EffectPrefabStaggeredDrift : MonoBehaviour
{
    private void ScheduleDeferredRestart()
    {
        if (_deferredRestartRoutine != null)
        {
            StopCoroutine(_deferredRestartRoutine);
        }

        _deferredRestartRoutine = StartCoroutine(DeferredRestartRoutine());
    }

    private IEnumerator DeferredRestartRoutine()
    {
        // PlayPart / composite attach can run after our first OnEnable — re-anchor transforms.
        yield return null;
        Restart();
        LogState("Deferred+1");
        yield return null;
        Restart();
        LogState("Deferred+2");
        _deferredRestartRoutine = null;
    }
}
