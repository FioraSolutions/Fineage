using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicGaitTrace
{
    public static bool IsMagic(Entity entity)
    {
        if (entity == null)
            return false;
        CharacterRaceAnimation race = entity.RaceId;
        return race == CharacterRaceAnimation.MMagic || race == CharacterRaceAnimation.FMagic;
    }

    public static Entity Resolve(int objectId)
    {
        return EntityActionSkill.ResolveEntity(objectId);
    }

    public static void Event(Entity entity, string tag, string detail = null)
    {
        if (!Enabled || !IsMagic(entity) || entity.Identity == null)
            return;
        Write(entity, tag, detail, dumpAnim: true, stack: true);
    }

    public static void EventId(int objectId, string tag, string detail = null)
    {
        Entity entity = Resolve(objectId);
        Event(entity, tag, detail);
    }

    public static void SyncTick(Entity entity, string detail)
    {
        if (!Enabled || !IsMagic(entity) || entity.Identity == null)
            return;
        int id = entity.Identity.Id;
        float now = Time.unscaledTime;
        float interval = 0.1f;
        float watch;
        if (WatchUntil.TryGetValue(id, out watch) && now < watch)
            interval = 0.05f;
        float last;
        if (LastSyncAt.TryGetValue(id, out last) && now - last < interval)
            return;
        LastSyncAt[id] = now;
        Write(entity, "sync", detail, dumpAnim: true, stack: false);
    }
}
