using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateEntryStore
{
    public void EnsureHoverAndTarget(LayerMask entityMask, Color defaultNameColor)
    {
        if (ControllerPacketServices.Clicks != null && ControllerPacketServices.Clicks.HoverObjectData != null)
        {
            ObjectData hover = ControllerPacketServices.Clicks.HoverObjectData;
            if (hover.ObjectTransform != null &&
                entityMask == (entityMask | (1 << hover.ObjectLayer)))
            {
                Entity e = hover.ObjectTransform.GetComponentInParent<Entity>();
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
            Entity e = targets.Target.Data.ObjectTransform.GetComponentInParent<Entity>();
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
            NameColor = NameplateColors.Resolve(entity, defaultNameColor),
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
            Color nameColor = NameplateColors.Resolve(e.Entity, Color.white);
            e.NameColor = nameColor;

            paintList.Add(new NameplatePaintItem
            {
                Id = e.Id,
                World = L2OverheadAnchor.GetWorldPos(e.Entity, e.Target),
                Name = e.Name,
                Title = drawTitles ? e.Title : null,
                NameColor = nameColor,
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
