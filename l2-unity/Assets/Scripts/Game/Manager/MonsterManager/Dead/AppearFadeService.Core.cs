using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public sealed partial class AppearFadeService
{
    public AppearFadeService(L2ActorFade fade)
    {
        _fade = fade;
    }

    public void Begin(Entity entity)
    {
        if (_fade == null || entity == null || entity.Identity == null || entity.IsDead())
        {
            return;
        }

        int id = entity.Identity.Id;
        if (_appear.ContainsKey(id))
        {
            return;
        }

        Entry entry = new Entry(entity);
        if (!entry.Apply(_fade, L2ActorFade.AppearStartAlpha))
        {
            return;
        }

        _appear.Add(id, entry);
    }

    public void BeginDisappear(Entity entity, Action<GameObject> onFinished)
    {
        if (_fade == null || entity == null || entity.gameObject == null)
        {
            InvokeFinished(onFinished, entity);
            return;
        }

        int objectId = entity.Identity != null ? entity.Identity.Id : 0;
        Entry entry;
        if (objectId != 0 && _appear.TryGetValue(objectId, out entry) && entry.Entity == entity)
        {
            _appear.Remove(objectId);
            entry.OnFinished = onFinished;
            StartDisappearFromCurrent(entry);
            _disappear[entity.gameObject.GetInstanceID()] = entry;
            return;
        }

        int goId = entity.gameObject.GetInstanceID();
        if (_disappear.ContainsKey(goId))
        {
            return;
        }

        entry = new Entry(entity);
        entry.OnFinished = onFinished;
        if (!entry.Apply(_fade, 255))
        {
            InvokeFinished(onFinished, entity);
            return;
        }

        _disappear.Add(goId, entry);
    }

    public void Cancel(int id)
    {
        Entry entry;
        if (!_appear.TryGetValue(id, out entry))
        {
            return;
        }

        entry.Restore(_fade);
        _appear.Remove(id);
    }

    public void AroundVisualRefresh(Entity entity, Action refresh)
    {
        if (refresh == null)
        {
            return;
        }

        Entry entry;
        bool fading = TryGetAppear(entity, out entry);
        if (fading)
        {
            entry.Restore(_fade);
        }

        refresh();

        if (!fading)
        {
            return;
        }

        if (!entry.IsValid() || !entry.Apply(_fade, _fade.AppearAlphaByte(entry.Elapsed)))
        {
            _appear.Remove(entry.Id);
        }
    }
}
