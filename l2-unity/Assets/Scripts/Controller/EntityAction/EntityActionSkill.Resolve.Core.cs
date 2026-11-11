using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill : IEntityActionProcess
{
    public static Entity ResolveEntity(int objectId)
    {
        if (objectId == 0)
            return null;

        if (ControllerPacketServices.GameWorld != null)
        {
            Entity fromWorld = ControllerPacketServices.GameWorld.GetEntityNoLockSync(objectId);
            if (fromWorld != null)
                return fromWorld;
        }

        if (DomainPacketServices.LocalPlayer != null &&
            DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == objectId)
            return DomainPacketServices.LocalPlayer;

        return null;
    }

    public static MagicCastData ResolveCastData(int objectId)
    {
        Entity entity = ResolveEntity(objectId);
        return entity != null ? entity.GetMagicCastData() : null;
    }
}
