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
}
