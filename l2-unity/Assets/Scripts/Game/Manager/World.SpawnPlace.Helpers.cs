using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using System.Security.Principal;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;

public partial class World
{
    public void SpawnNpc(
        EntityIdentity identity,
        NpcStatusInterlude status,
        Stats stats,
        out bool unresolvable)
    {
        unresolvable = false;
        if (identity == null)
            return;
        if (ContainsNpc(identity.Id))
        {
            // Tile unload can destroy the body and leave the id registered. A later NpcInfo
            // must be allowed to build it again.
            Entity live = GetEntityNoLockSync(identity.Id);
            if (live != null)
                return;
            RemoveObject(identity.Id);
        }

        if (identity.NpcId == 20481)
            Debug.Log(" object NpcInfo 5 " + identity.Id);

        NpcSpawnRequest request;
        if (!EntitySpawnShared.TryResolveNpc(identity, _npcGrps, _npcNames, _models, out request))
        {
            unresolvable = true;
            return;
        }

        request.Status = status;
        request.Stats = stats;
        // Debug.Log("Name NPC " + request.NpcName.Name);

        if (identity.EntityType == EntityType.NPC)
        {
            if (_npcSpawner != null)
                _npcSpawner.Spawn(request, this);
        }
        else if (_monsterSpawner != null)
        {
            _monsterSpawner.Spawn(request, this);
        }
    }

    public void UpdateNpc(Entity entity , NpcInfoDto npcInfo)
    {
        if (_monsterSpawner != null)
            _monsterSpawner.UpdateInfo(entity, npcInfo);
    }

    public void UpdateUserInfo(Entity entity, UserInfoDto userInfo)
    {
        if (_playerSpawner != null)
            _playerSpawner.UpdateInfo(entity, userInfo);
    }

    public async Task DeleteObject(int objectId)
    {
        Entity entity = await GetEntityNoLock(objectId);
        if (entity == null)
        {
            // Debug.LogWarning(
                // $"[DeleteObject] HANDLER id={objectId} → entity NOT in World._objects " +
                // "(packet arrived but client has no entity — GO may be orphaned in Hierarchy)");
            return;
        }

        string typeName = entity.GetType().Name;
        // Debug.Log(
            // $"[DeleteObject] HANDLER id={objectId} found type={typeName} name={entity.name} " +
            // $"activeInHierarchy={entity.gameObject.activeInHierarchy}");

        if (entity.GetType() == typeof(MonsterEntity))
        {
            if (entity.IsDead())
            {
                if (_deadManager != null)
                    _deadManager.AddDeadAndRemove(objectId, new DeadData(entity));
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} Monster DEAD → DeadManager");
            }
            else
            {
                RemoveObject(objectId);
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} Monster → RemoveObject");
            }
        }
        else if (entity is ItemEntity)
        {
            if (_itemDropPresentation != null)
                _itemDropPresentation.StopItemPresentation(objectId);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DropMeshLifetimeLog.NotifyRemove(objectId, "DeleteObject_packet");
#endif
            RemoveObject(objectId);
            // Debug.Log($"[DeleteObject] HANDLER id={objectId} ItemEntity → RemoveObject");
        }
        else if (entity is NpcEntity)
        {
            if (entity.IsDead() && _deadManager != null)
            {
                _deadManager.AddDeadAndRemove(objectId, new DeadData(entity));
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} NpcEntity DEAD → DeadManager");
            }
            else
            {
                BeginActorDisappear(objectId, entity, FinishDisappearedNpc);
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} NpcEntity → disappear fade");
            }
        }
        else if (entity is UserEntity)
        {
            Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(entity.transform.position);
            string nick = entity.Identity != null ? entity.Identity.Name : entity.name;
            // Debug.Log("[MOVE_SNAP] DESPAWN nick=" + nick +
                // " l2=" + l2.x.ToString("F0") + "," + l2.y.ToString("F0") + "," + l2.z.ToString("F0"));
            BeginActorDisappear(objectId, entity, DestroyDisappearedUser);
            // Debug.Log($"[DeleteObject] HANDLER id={objectId} UserEntity → disappear fade");
        }
        else
        {
            // Same as historical catch-all: anything else still must leave the world.
            // Debug.LogWarning(
                // $"[DeleteObject] HANDLER id={objectId} unexpected type={typeName} → RemoveObject");
            RemoveObject(objectId);
        }
    }
}
