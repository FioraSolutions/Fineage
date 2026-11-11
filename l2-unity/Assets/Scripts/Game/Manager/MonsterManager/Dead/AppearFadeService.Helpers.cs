using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public sealed partial class AppearFadeService
{
    public void Tick()
    {
        TickDict(_appear, true);
        TickDict(_disappear, false);
    }

    void TickDict(Dictionary<int, Entry> dict, bool appear)
    {
        if (dict.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<int, Entry> kvp in dict)
        {
            Entry entry = kvp.Value;
            if (entry == null || !entry.IsValid())
            {
                _remove.Add(kvp.Key);
                continue;
            }

            entry.Elapsed += Time.deltaTime;
            if (entry.Elapsed >= L2ActorFade.DurationSeconds)
            {
                if (appear)
                {
                    entry.Restore(_fade);
                }
                else
                {
                    InvokeFinished(entry.OnFinished, entry.Entity);
                }

                _remove.Add(kvp.Key);
                continue;
            }

            byte alpha = appear
                ? _fade.AppearAlphaByte(entry.Elapsed)
                : _fade.AlphaByte(entry.Elapsed);
            entry.SetAlpha(_fade, alpha);
        }

        for (int i = 0; i < _remove.Count; i++)
        {
            dict.Remove(_remove[i]);
        }

        _remove.Clear();
    }

    void StartDisappearFromCurrent(Entry entry)
    {
        byte current = _fade.AppearAlphaByte(entry.Elapsed);
        entry.Elapsed = (255f - current) / L2ActorFade.AlphaPerSecond;
        entry.SetAlpha(_fade, current);
    }

    bool TryGetAppear(Entity entity, out Entry entry)
    {
        entry = null;
        if (entity == null || entity.Identity == null)
        {
            return false;
        }

        return _appear.TryGetValue(entity.Identity.Id, out entry);
    }

    static void InvokeFinished(Action<GameObject> onFinished, Entity entity)
    {
        if (onFinished == null)
        {
            return;
        }

        onFinished(entity != null ? entity.gameObject : null);
    }
}
