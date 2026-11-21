using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateEntryStore
{
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
            if (visible && !CrowdTierPacketServices.ShouldShowNameplate(id))
                visible = false;
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
            Color nameColor = NameplateColorsEntity.Resolve(e.Entity, Color.white);
            e.NameColor = nameColor;

            byte questMark = 0;
            if (e.Entity?.Identity != null)
                questMark = QuestJournalState.NpcMarkForTemplate(e.Entity.Identity.NpcId);

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
                CastRemaining01 = castRem,
                QuestMark = questMark
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

        Entity entity = target.GetComponentInParent<Entity>();
        if (entity != null && !EntityInteraction.IsAttackable(entity))
        {
            // Interlude: peaceful / teleporter / merchant names in range without LOS to feet.
            return true;
        }

        CameraController cameraController = CameraPacketServices.Controller;
        if (cameraController != null)
        {
            Vector3 anchor = entity != null
                ? L2OverheadAnchor.GetWorldPos(entity, target)
                : target.position;
            return cameraController.IsPointVisible(anchor);
        }

        return true;
    }
}
