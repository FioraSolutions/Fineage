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
}
