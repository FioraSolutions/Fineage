using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    static int ResolveSpawnRace(EntitySpawnMsg spawn)
    {
        if (spawn.Race >= 0 && spawn.Race <= (int)CharacterRace.Dwarf)
            return spawn.Race;

        SpawnAppearanceApplyLog.LogRaceDerivedFromClass((int)spawn.EntityValue, spawn.Race, spawn.ClassId);
        return (int)MapClassId.GetCharacterRace(spawn.ClassId);
    }

    static int ResolveSpawnCurHp(EntitySpawnMsg spawn, int maxHp)
    {
        if (spawn.Dead)
            return 0;
        if (spawn.CurHp < 0)
            return maxHp;
        if (spawn.CurHp > maxHp)
            return maxHp;
        return spawn.CurHp;
    }

    internal void ApplyNpcSpawn(EntitySpawnMsg spawn)
    {
        int objectId = (int)spawn.EntityValue;
        bool liveDoor = EntitySpawnApply.IsLiveDoorSpawn(spawn.TemplateId, spawn.Name);
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = objectId;
        dto.Identity.NpcId = spawn.TemplateId;
        if (!string.IsNullOrEmpty(spawn.Name))
            dto.Identity.Name = spawn.Name;
        if (liveDoor)
        {
            NetDoorSession.RememberDoorSpawn(objectId);
            dto.Identity.EntityType = EntitySpawnApply.ResolveDoorEntityType(spawn.Attackable);
        }
        else
            dto.Identity.EntityType = EntitySpawnApply.ResolveEntityType(spawn.Attackable);
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.OrigHeading = EntitySpawnApply.ResolveHeadingDegrees(spawn.Heading);
        dto.Identity.Heading = Quaternion.Euler(0f, dto.Identity.OrigHeading, 0f);
        dto.Identity.IsRunning = spawn.Running;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        // Wire carries MaxHp + CurHp (#522 bar size; CurHp so select/AOI StatusUpdate is not a false hit).
        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxHp = maxHp;
        dto.Status.SetHp(ResolveSpawnCurHp(spawn, maxHp));
        int pAtkSpd = spawn.PAtkSpd > 0 ? spawn.PAtkSpd : 253;
        dto.Stats.PAtkSpd = pAtkSpd;
        dto.Stats.PAtkRealSpeed = pAtkSpd;
        // Live doors (Mobius NpcKind door, template id 0) never move: the server sends speed 0 for them.
        if (!liveDoor && (spawn.RunSpeed <= 0.001f || spawn.WalkSpeed <= 0.001f))
            SpawnAppearanceApplyLog.LogDefaultMoveSpeed(objectId, spawn.RunSpeed, spawn.WalkSpeed);
        PawnMoveSpeed.Apply(dto.Stats, spawn.RunSpeed, spawn.WalkSpeed, templateChanged: true);
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        NetNpcSession.RememberNpc(objectId, spawn.Name, liveDoor || spawn.Attackable, spawn.Rhand, spawn.Lhand);
        if (_presence != null) _presence.ApplyNpcInfo(dto);
        else NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(dto);
    }

    internal void ApplyEntityDespawn(EntityDespawnMsg despawn)
    {
        int objectId = (int)despawn.EntityValue;
        NetNpcSession.Forget(objectId);
        NetDoorSession.Forget(objectId);
        var dto = new DeleteObjectDto { ObjectId = objectId };
        DeleteObjectGroundApplyService deleteObjectGround = _deleteObjectGround;
        if (deleteObjectGround != null) deleteObjectGround.Apply(dto);
        else DeleteObjectGroundApplyIncomingPacketServices.ApplyDeleteObject(dto);
    }
}
