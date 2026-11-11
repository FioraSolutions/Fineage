using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    protected virtual void OnDestroy()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropAdenaDestroyLog.Event(
            // "ON_DESTROY",
            // this,
            // $"scheduledAt={_scheduledDestroyAt:F3}s scheduledLife={_scheduledLifeTime:F3}s " +
            // $"castHit={_castHitTimeSnapshot:F3}s owner='{(_owner != null ? _owner.name : "null")}'",
            // includeStack: true);
        if (_scheduledDestroyAt <= 0f)
        {
            return;
        }

        float now = Time.time;
        float globalSinceCastStart = _castStartTimeSnapshot > 0f ? now - _castStartTimeSnapshot : -1f;
        // Debug.Log(
            // $"[EffectDestroyed] effect='{name}' now={now:F3}s scheduledAt={_scheduledDestroyAt:F3}s " +
            // $"globalSinceCastStart={globalSinceCastStart:F3}s castHit={_castHitTimeSnapshot:F3}s castFlight={_castFlightTimeSnapshot:F3}s " +
            // $"deltaToCastHit={(globalSinceCastStart >= 0f && _castHitTimeSnapshot >= 0f ? globalSinceCastStart - _castHitTimeSnapshot : -1f):F3}s " +
            // $"scheduledLifeTime={_scheduledLifeTime:F3}s shader={BuildLifetimeShaderSnapshot()}");
#endif
    }

    protected void BeginFadeOut()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float now = Time.time;
        float globalSinceCastStart = _castStartTimeSnapshot > 0f ? now - _castStartTimeSnapshot : -1f;
        // Debug.Log(
            // $"[BeginFadeOut] effect='{name}' now={now:F3}s globalSinceCastStart={globalSinceCastStart:F3}s " +
            // $"castHit={_castHitTimeSnapshot:F3}s castFlight={_castFlightTimeSnapshot:F3}s owner='{(_owner != null ? _owner.name : "null")}'.\n" +
            // $"stack={Environment.StackTrace}");
#endif
        EffectPart[] parts = GetComponentsInChildren<EffectPart>(true);

        foreach (var part in parts)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DropAdenaDestroyLog.Event(
                // "BEGIN_FADEOUT_STOPPART",
                // part,
                // $"fromEffect='{name}'",
                // includeStack: false);
#endif
            part.StopPart();
        }
    }
}
