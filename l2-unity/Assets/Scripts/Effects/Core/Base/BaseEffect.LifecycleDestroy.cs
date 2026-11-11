using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    public void DestoryEffect(EffectSettings settings, MagicCastData castData = null)
    {
        float fadeOutTime = Mathf.Max(0, settings.defaultLifeTime - settings.hideTime);
        _scheduledLifeTime = settings.defaultLifeTime;
        _scheduledDestroyAt = Time.time + _scheduledLifeTime;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float now = Time.time;
        float globalSinceCastStart = _castStartTimeSnapshot > 0f ? now - _castStartTimeSnapshot : -1f;
        // Debug.Log(
            // $"[EffectDestroySchedule] effect='{name}' owner='{(_owner != null ? _owner.name : "null")}' " +
            // $"now={now:F3}s globalSinceCastStart={globalSinceCastStart:F3}s " +
            // $"lifeTime={settings.defaultLifeTime:F3}s hideTime={settings.hideTime:F3}s " +
            // $"fadeStartIn={fadeOutTime:F3}s castHit={_castHitTimeSnapshot:F3}s " +
            // $"castFlight={_castFlightTimeSnapshot:F3}s shader={BuildLifetimeShaderSnapshot()}\n" +
            // $"scheduleStack={Environment.StackTrace}");
#endif

        Invoke(nameof(BeginFadeOut), fadeOutTime);
        
        // Удаляем сам эффект через заданное время
        Destroy(gameObject, settings.defaultLifeTime);
        
        // Проверяем, есть ли родитель с именем HitPointProxy и удаляем его тоже
        if (transform.parent != null && transform.parent.name == "HitPointProxy")
        {
            Destroy(transform.parent.gameObject, settings.defaultLifeTime);
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropAdenaDestroyLog.Event(
            // "SCHEDULE_DESTROY",
            // this,
            // $"lifeTime={settings.defaultLifeTime:F3}s hideTime={settings.hideTime:F3}s " +
            // $"fadeStartIn={fadeOutTime:F3}s destroyAt={_scheduledDestroyAt:F3}s " +
            // $"alsoDestroyHitPointProxy={(transform.parent != null && transform.parent.name == "HitPointProxy")}",
            // includeStack: true);
#endif
    }

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
