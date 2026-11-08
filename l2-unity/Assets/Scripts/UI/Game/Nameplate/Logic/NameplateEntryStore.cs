using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World nameplate entry dictionary: discover, upsert, visibility cull, paint list.
/// </summary>
public sealed class NameplateEntryStore
{
    private readonly Dictionary<int, NameplateEntry> _entries;
    private readonly List<int> _removeIds;
    private readonly List<int> _entryKeys;
    private readonly NameplateBubbleResolver _resolver;

    private int _removeObjId;

    public NameplateEntryStore(NameplateBubbleResolver resolver, int capacity = 100)
    {
        _resolver = resolver;
        _entries = new Dictionary<int, NameplateEntry>(capacity);
        _removeIds = new List<int>(32);
        _entryKeys = new List<int>(capacity);
    }

    public int Count => _entries.Count;

    public void Clear()
    {
        _entries.Clear();
        L2OverheadAnchor.ClearAll();
    }

    public void Remove(int id, NameplatePixelSnap snap)
    {
        if (_entries.Remove(id))
        {
            _removeObjId = id;
            snap?.Clear(id);
            L2OverheadAnchor.Invalidate(id);
        }
    }

    public void Discover(
        Collider[] hits,
        Color defaultNameColor)
    {
        if (hits == null)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider c = hits[i];
            if (c == null)
            {
                continue;
            }

            Transform hitT = c.transform;
            if (hitT == null)
            {
                continue;
            }

            Entity entity = hitT.GetComponent<Entity>();
            if (entity == null || entity.Identity == null)
            {
                continue;
            }

            if (entity.Identity.Id == _removeObjId)
            {
                continue;
            }

            UpsertEntry(entity, defaultNameColor);
        }
    }

    public void EnsureHoverAndTarget(LayerMask entityMask, Color defaultNameColor)
    {
        if (ControllerPacketServices.Clicks != null && ControllerPacketServices.Clicks.HoverObjectData != null)
        {
            ObjectData hover = ControllerPacketServices.Clicks.HoverObjectData;
            if (hover.ObjectTransform != null &&
                entityMask == (entityMask | (1 << hover.ObjectLayer)))
            {
                Entity e = hover.ObjectTransform.GetComponent<Entity>();
                if (e != null)
                {
                    UpsertEntry(e, defaultNameColor);
                }
            }
        }

        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null &&
            targets.HasTarget() &&
            targets.Target?.Data?.ObjectTransform != null)
        {
            Entity e = targets.Target.Data.ObjectTransform.GetComponent<Entity>();
            if (e != null)
            {
                UpsertEntry(e, defaultNameColor);
            }
        }
    }

    public void UpsertEntry(Entity entity, Color defaultNameColor)
    {
        if (entity == null || entity.Identity == null || entity.transform == null)
        {
            return;
        }

        // Hidden entities (pooled, ride-hidden pets, despawning) must never get a plate.
        if (!entity.gameObject.activeInHierarchy)
        {
            return;
        }

        EntityIdentity idn = entity.Identity;
        if (string.IsNullOrEmpty(idn.Name))
        {
            return;
        }

        int id = idn.Id;
        Color titleColor = defaultNameColor;
        if (!string.IsNullOrEmpty(idn.TitleColor))
        {
            titleColor = StringUtils.HexToColor(idn.TitleColor);
        }

        _entries[id] = new NameplateEntry
        {
            Id = id,
            Target = entity.transform,
            Entity = entity,
            Name = idn.Name,
            Title = idn.Title ?? string.Empty,
            NameColor = defaultNameColor,
            TitleColor = titleColor,
            Visible = true
        };
    }

    public void RefreshVisibility(
        Transform playerTransform,
        float nameplateViewDistance,
        NameplatePixelSnap snap)
    {
        _removeIds.Clear();
        _entryKeys.Clear();
        foreach (int id in _entries.Keys)
        {
            _entryKeys.Add(id);
        }

        for (int i = 0; i < _entryKeys.Count; i++)
        {
            int id = _entryKeys[i];
            if (!_entries.TryGetValue(id, out NameplateEntry e))
            {
                continue;
            }

            if (e.Target == null || e.Entity == null ||
                !e.Entity.gameObject.activeInHierarchy)
            {
                // Destroyed or hidden (pooled / ride-hidden / despawning): drop at once
                // instead of waiting for the distance or camera cull.
                _removeIds.Add(id);
                continue;
            }

            bool visible = IsNameplateVisible(e.Target, playerTransform, nameplateViewDistance);
            e.Visible = visible;
            _entries[id] = e;

            bool isLocal = DomainPacketServices.LocalPlayer != null && e.Entity == DomainPacketServices.LocalPlayer;
            if (!visible && !isLocal && !_resolver.IsHoverOrTarget(e.Target))
            {
                _removeIds.Add(id);
            }
        }

        for (int i = 0; i < _removeIds.Count; i++)
        {
            int rid = _removeIds[i];
            _entries.Remove(rid);
            snap?.Clear(rid);
            L2OverheadAnchor.Invalidate(rid);
        }
    }

    public void BuildPaintList(List<NameplatePaintItem> paintList, bool drawTitles)
    {
        paintList.Clear();

        foreach (KeyValuePair<int, NameplateEntry> kv in _entries)
        {
            NameplateEntry e = kv.Value;
            if (!e.Visible || e.Target == null || string.IsNullOrEmpty(e.Name))
            {
                continue;
            }

            if (e.Entity != null && e.Entity.Identity != null)
            {
                EntityIdentity idn = e.Entity.Identity;
                e.Name = idn.Name;
                e.Title = idn.Title ?? string.Empty;
                if (!string.IsNullOrEmpty(idn.TitleColor))
                {
                    e.TitleColor = StringUtils.HexToColor(idn.TitleColor);
                }

                if (idn.ClanId > 0)
                {
                    string clanName = NetClanBook.Shared.NameFor(idn.ClanId);
                    if (!string.IsNullOrEmpty(clanName))
                    {
                        e.Title = clanName;
                        e.TitleColor = new Color(0.4f, 0.8f, 1f, 1f);
                    }
                }
            }

            bool isLocal = DomainPacketServices.LocalPlayer != null && e.Entity == DomainPacketServices.LocalPlayer;
            L2TargetRenderType bubbleType = _resolver.ResolveForPaint(e.Target, isLocal);
            float castRem = isLocal ? CastBarOverlay.Remaining01 : -1f;

            paintList.Add(new NameplatePaintItem
            {
                Id = e.Id,
                World = L2OverheadAnchor.GetWorldPos(e.Entity, e.Target),
                Name = e.Name,
                Title = drawTitles ? e.Title : null,
                NameColor = e.NameColor,
                TitleColor = e.TitleColor,
                IsLocalPlayer = isLocal,
                BubbleType = bubbleType,
                CastRemaining01 = castRem
            });
        }
    }

    private bool IsNameplateVisible(
        Transform target,
        Transform playerTransform,
        float nameplateViewDistance)
    {
        if (target == null || playerTransform == null)
        {
            return false;
        }

        if (DomainPacketServices.LocalPlayer != null && target == DomainPacketServices.LocalPlayer.transform)
        {
            return true;
        }

        if (_resolver.IsHoverOrTarget(target))
        {
            return true;
        }

        if (Vector3.Distance(playerTransform.position, target.position) > nameplateViewDistance)
        {
            return false;
        }

        CameraController cameraController = CameraPacketServices.Controller;
        if (cameraController != null)
        {
            return cameraController.IsObjectVisible(target);
        }

        return true;
    }
}
