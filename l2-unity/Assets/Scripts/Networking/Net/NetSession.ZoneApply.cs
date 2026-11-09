using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{

    internal void ApplyEntitySpawn(EntitySpawnMsg spawn)
    {
        if (spawn.Kind == EntityKindNet.Npc)
        {
            ApplyNpcSpawn(spawn);
            return;
        }

        if (spawn.Kind != EntityKindNet.Player)
            return;

        if (!spawn.IsSelf)
        {
            ApplyRemotePlayerSpawn(spawn);
            return;
        }

        // Zone object id of the local player; stable only for this EnterWorld (see CharacterId).
        SessionObjectId = (int)spawn.EntityValue;

        var identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = (int)spawn.EntityValue,
            Name = string.IsNullOrEmpty(spawn.Name) ? "Player" : spawn.Name,
            PlayerClass = spawn.ClassId,
            Owned = true,
            IsRunning = spawn.Running
        };
        identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);

        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        var status = new PlayerStatus();
        status.SetHp(maxHp);

        var stats = new PlayerStats
        {
            Level = spawn.Level > 0 ? spawn.Level : 1,
            MaxHp = maxHp,
            MaxMp = 0,
            MaxCp = 0,
            BaseRunSpeed = Mathf.RoundToInt(spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f),
            RunRealSpeed = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f,
            WalkRealSpeed = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f
        };

        var appearance = new PlayerAppearance
        {
            Race = spawn.Race >= 0 && spawn.Race <= (int)CharacterRace.Dwarf
                ? spawn.Race
                : (int)MapClassId.GetCharacterRace(spawn.ClassId),
            Sex = spawn.Sex,
            Running = spawn.Running,
            // BaseClass is Fighter/Mystic (0/1), not L2 classId — ResolveRaceAnimation uses PlayerClass.
            BaseClass = CharacterClassParser.IsMage(spawn.ClassId)
                ? (int)BaseClass.MMagic
                : (int)BaseClass.Fighter
        };

        // Stash PlayerInfo then OnCharacterSelect → LoadSWMap.
        // Spawning happens in OnWorldSceneLoaded once the tile is ready.
        // Without PlayerInfo.Identity, LoadSWMap returns immediately and loading never ends.
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null)
        {
            transport.PlayerInfo = new PlayerInfoInterlude
            {
                Identity = identity,
                Status = status,
                Stats = stats,
                Appearance = appearance
            };
        }
        else
            Debug.LogWarning("[Net] Zone transport missing on EntitySpawn — world load will stall");

        GameManager gameManager = NetworkingPacketServices.Manager ?? GamePacketServices.Manager;
        if (gameManager != null)
            gameManager.OnCharacterSelect();
        else
            Debug.LogWarning("[Net] GameManager missing on EntitySpawn — world load will stall");

        EnteredWorld?.Invoke();
    }


    internal void ApplyRemotePlayerSpawn(EntitySpawnMsg spawn)
    {
        CharInfoDto dto = new CharInfoDto();
        dto.Identity.Id = (int)spawn.EntityValue;
        dto.Identity.Name = string.IsNullOrEmpty(spawn.Name) ? "Unknown" : spawn.Name;
        dto.Identity.EntityType = EntityType.User;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.PlayerClass = spawn.ClassId;
        dto.Identity.IsRunning = spawn.Running;
        dto.Appearance.Race = spawn.Race >= 0 && spawn.Race <= (int)CharacterRace.Dwarf
            ? spawn.Race
            : (int)MapClassId.GetCharacterRace(spawn.ClassId);
        dto.Appearance.Sex = spawn.Sex;
        dto.Appearance.BaseClass = CharacterClassParser.IsMage(spawn.ClassId)
            ? (int)BaseClass.MMagic
            : (int)BaseClass.Fighter;
        dto.Appearance.Running = spawn.Running;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.RunRealSpeed = run;
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxHp = maxHp;
        dto.Status.SetHp(spawn.Dead ? 0 : maxHp);
        dto.Status.SetMp(100);
        dto.Stats.MaxMp = 100;
        if (spawn.PAtkSpd > 0)
        {
            dto.Stats.PAtkSpd = spawn.PAtkSpd;
            dto.Stats.PAtkRealSpeed = spawn.PAtkSpd;
        }
        // Paperdoll is what makes peers visible in gear: without it every other player spawns
        // undressed while the wearer sees their own kit (UserInfo path).
        if (spawn.Paperdoll != null && spawn.Paperdoll.Length > 0)
            ApplySpawnPaperdollItemIds(dto.Appearance, spawn.Paperdoll);
        if (_presence != null) _presence.ApplyCharInfo(dto);
        else CharInfoApplyIncomingPacketServices.ApplyCharInfo(dto);
    }


    internal static void ApplySpawnPaperdollItemIds(PlayerAppearance appearance, int[] itemIds) =>
        NetPaperdollApply.ApplyItemIds(appearance, itemIds);


    internal void ApplyNpcSpawn(EntitySpawnMsg spawn)
    {
        int objectId = (int)spawn.EntityValue;
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = objectId;
        dto.Identity.NpcId = spawn.TemplateId;
        if (!string.IsNullOrEmpty(spawn.Name))
            dto.Identity.Name = spawn.Name;
        dto.Identity.SetHideHp(spawn.TemplateId);
        dto.Identity.EntityType = EntityType.NPC;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.IsRunning = spawn.Running;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        // Wire carries MaxHp only; alive NPCs spawn at full HP. Stubbing CurHp=100 while MaxHp is
        // the real template value made TargetWindow BarFillRatio stick at 1 until a StatusUpdate
        // arrived — and look broken when updates lagged (#522).
        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxHp = maxHp;
        dto.Status.SetHp(spawn.Dead ? 0 : maxHp);
        int pAtkSpd = spawn.PAtkSpd > 0 ? spawn.PAtkSpd : 253;
        dto.Stats.PAtkSpd = pAtkSpd;
        dto.Stats.PAtkRealSpeed = pAtkSpd;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.RunRealSpeed = run;
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        NetNpcSession.RememberNpc(objectId, spawn.Name, spawn.Attackable);
        if (_presence != null) _presence.ApplyNpcInfo(dto);
        else NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(dto);
    }


    internal void ApplyEntityDespawn(EntityDespawnMsg despawn)
    {
        int objectId = (int)despawn.EntityValue;
        NetNpcSession.Forget(objectId);
        var dto = new DeleteObjectDto { ObjectId = objectId };
        DeleteObjectGroundApplyService deleteObjectGround = _deleteObjectGround;
        if (deleteObjectGround != null) deleteObjectGround.Apply(dto);
        else DeleteObjectGroundApplyIncomingPacketServices.ApplyDeleteObject(dto);
    }


}
