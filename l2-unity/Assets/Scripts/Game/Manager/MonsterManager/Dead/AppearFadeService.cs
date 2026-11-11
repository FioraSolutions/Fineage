using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// Body alpha for CharInfo users and city NPCs: CreateSkin fade-in, FNDisappearAction fade-out.
/// </summary>
public sealed partial class AppearFadeService : ITickable
{
    readonly L2ActorFade _fade;
    readonly Dictionary<int, Entry> _appear = new Dictionary<int, Entry>();
    readonly Dictionary<int, Entry> _disappear = new Dictionary<int, Entry>();
    readonly List<int> _remove = new List<int>();



    sealed class Entry
    {
        readonly Entity _entity;
        Renderer[] _renderers;
        Material[][] _instances;
        Material[][] _sharedBackup;
        bool _applied;

        public Entry(Entity entity)
        {
            _entity = entity;
        }

        public Entity Entity
        {
            get { return _entity; }
        }

        public Action<GameObject> OnFinished { get; set; }

        public int Id
        {
            get { return _entity != null && _entity.Identity != null ? _entity.Identity.Id : 0; }
        }

        public float Elapsed { get; set; }

        public bool IsValid()
        {
            return _entity != null && _entity.gameObject != null;
        }

        public bool Apply(L2ActorFade fade, byte startAlpha)
        {
            Restore(fade);
            if (!IsValid() || fade == null)
            {
                return false;
            }

            _applied = fade.TryBegin(_entity, startAlpha, out _renderers, out _instances, out _sharedBackup);
            return _applied;
        }

        public void SetAlpha(L2ActorFade fade, byte alphaByte)
        {
            if (fade != null)
            {
                fade.SetAlphaByte(_instances, alphaByte);
            }
        }

        public void Restore(L2ActorFade fade)
        {
            if (!_applied)
            {
                return;
            }

            if (fade != null)
            {
                fade.Restore(_renderers, _sharedBackup, _instances);
            }

            _applied = false;
            _renderers = null;
            _instances = null;
            _sharedBackup = null;
        }
    }
}
