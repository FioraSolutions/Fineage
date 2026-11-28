using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a Die packet until the matching skill / projectile hit has resolved visually, so a
/// Power Shot (and any other skill kill) still plays the death pipeline instead of despawning
/// mid-flight.
/// </summary>
public static class EntityActionPendingDeath
{
    const float FlyGraceSeconds = 2f;

    struct Queued
    {
        public Entity Entity;
        public bool AlreadyCorpse;
        public string Source;
        public float FlushAt;
    }

    static readonly Dictionary<int, float> ArmedUntil = new Dictionary<int, float>();
    static readonly Dictionary<int, Queued> QueuedDeaths = new Dictionary<int, Queued>();

    public static void ArmSkillHit(Entity target, int hitTimeMs)
    {
        if (target == null || target.Identity == null)
            return;
        float until = Time.time + Mathf.Max(hitTimeMs / 1000f, 0.15f) + FlyGraceSeconds;
        ArmedUntil[target.Identity.Id] = until;
    }

    public static bool ShouldDefer(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return false;
        if (HasInboundProjectile(entity))
            return true;
        int id = entity.Identity.Id;
        return ArmedUntil.TryGetValue(id, out float until) && Time.time < until;
    }

    public static void Queue(Entity entity, bool alreadyCorpse, string source)
    {
        if (entity == null || entity.Identity == null)
            return;
        int id = entity.Identity.Id;
        float flushAt = Time.time + FlyGraceSeconds;
        if (ArmedUntil.TryGetValue(id, out float until) && until > flushAt)
            flushAt = until;
        if (CombatTrace.Enabled)
            CombatTrace.Write("death.deferred", "id=" + id + " source=" + source + " waitMs=" + (int)((flushAt - Time.time) * 1000f)
                + " projectile=" + HasInboundProjectile(entity));
        QueuedDeaths[id] = new Queued
        {
            Entity = entity,
            AlreadyCorpse = alreadyCorpse,
            Source = source,
            FlushAt = flushAt
        };
    }

    public static void Flush(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return;
        int id = entity.Identity.Id;
        ArmedUntil.Remove(id);
        if (!QueuedDeaths.TryGetValue(id, out Queued queued))
            return;
        QueuedDeaths.Remove(id);
        PlayQueued(queued);
    }

    public static void Tick()
    {
        if (QueuedDeaths.Count == 0 && ArmedUntil.Count == 0)
            return;

        List<int> expiredArms = null;
        foreach (var kv in ArmedUntil)
        {
            if (Time.time < kv.Value)
                continue;
            expiredArms ??= new List<int>();
            expiredArms.Add(kv.Key);
        }
        if (expiredArms != null)
        {
            for (int i = 0; i < expiredArms.Count; i++)
                ArmedUntil.Remove(expiredArms[i]);
        }

        List<int> ready = null;
        foreach (var kv in QueuedDeaths)
        {
            Queued queued = kv.Value;
            Entity entity = queued.Entity;
            bool inbound = HasInboundProjectile(entity);
            if (inbound && Time.time < queued.FlushAt)
                continue;
            if (ShouldDefer(entity) && Time.time < queued.FlushAt)
                continue;
            ready ??= new List<int>();
            ready.Add(kv.Key);
        }
        if (ready == null)
            return;
        for (int i = 0; i < ready.Count; i++)
        {
            int id = ready[i];
            if (!QueuedDeaths.TryGetValue(id, out Queued queued))
                continue;
            QueuedDeaths.Remove(id);
            ArmedUntil.Remove(id);
            PlayQueued(queued);
        }
    }

    static void PlayQueued(Queued queued)
    {
        Entity entity = queued.Entity;
        if (entity == null)
            return;
        EntityActionVisual.PlayDeath(entity, queued.AlreadyCorpse);
    }

    static bool HasInboundProjectile(Entity entity)
    {
        IProjectileManager projectiles = ProjectilePacketServices.Projectiles;
        return projectiles != null && projectiles.HasInboundToward(entity);
    }
}
