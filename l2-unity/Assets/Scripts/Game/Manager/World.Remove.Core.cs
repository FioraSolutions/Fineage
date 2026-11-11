using System;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using System.Collections.Generic;
using System.Threading.Tasks;
using VContainer;

public partial class World
{
    public void RemoveObject(int id) {
        Entity entity;
        if (!_objects.TryGetValue(id, out entity))
        {
            // Debug.LogWarning($"[DeleteObject] RemoveObject MISS dict id={id} (already gone or never spawned)");
            return;
        }

        string goName = entity != null ? entity.name : "null";
        string typeName = entity != null ? entity.GetType().Name : "null";
        GameObject go = entity != null ? entity.gameObject : null;
        bool wasActive = go != null && go.activeSelf;
        string parentBefore = go != null && go.transform.parent != null
            ? go.transform.parent.name
            : "null";

        // Debug.Log(
            // $"[DeleteObject] RemoveObject START id={id} type={typeName} name={goName} " +
            // $"activeSelf={wasActive} parent={parentBefore}");

        if (_npcDeco != null)
            _npcDeco.Stop(id);

        UnregisterObject(id, entity);
        if (_appearFade != null)
            _appearFade.Cancel(id);

        if (go == null)
        {
            // Debug.LogWarning($"[DeleteObject] RemoveObject id={id} entity.gameObject is null");
            return;
        }

        // City NPC + field monsters + remote users → pool. If pool fails or leaves object visible → Destroy.
        if (_objectPool != null &&
            (entity is NpcEntity || entity is MonsterEntity || entity is UserEntity) &&
            !(entity is ItemEntity))
        {
            ObjectType poolType = entity is MonsterEntity
                ? ObjectType.Monster
                : entity is UserEntity
                    ? ObjectType.User
                    : ObjectType.Npc;
            bool returned = _objectPool.ReturnToPool(poolType, go);
            bool stillVisible = go != null && go.activeInHierarchy;
            // Debug.Log(
                // $"[DeleteObject] RemoveObject {poolType} id={id} returned={returned} " +
                // $"activeSelf={(go != null && go.activeSelf)} " +
                // $"activeInHierarchy={stillVisible} " +
                // $"parent={(go != null && go.transform.parent != null ? go.transform.parent.name : "null")}");

            if (returned && !stillVisible)
            {
                return;
            }

            // Debug.LogWarning(
                // $"[DeleteObject] RemoveObject {poolType} id={id} pool did not hide → Destroy " +
                // $"(returned={returned} stillVisible={stillVisible})");
        }

        Destroy(go);
        // Debug.Log($"[DeleteObject] RemoveObject DESTROY id={id} name={goName}");
    }

    void UnregisterObject(int id, Entity entity)
    {
        if (MoveAllCharactersPacketServices.Moves != null)
            MoveAllCharactersPacketServices.Moves.Forget(id);
        _players.Remove(id);
        _npcs.Remove(id);
        _objects.Remove(id);
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Remove(entity);

        if (_gravityNpc != null)
            _gravityNpc.DeleteGravity(id);

        if (Animations != null)
            Animations.UnregisterController(id);
    }

    void BeginActorDisappear(int id, Entity entity, Action<GameObject> onFinished)
    {
        EntityActionVisual.CancelMove(entity);
        EntityActionVisual.PlayDisappearWait(entity);
        GameObject go = entity != null ? entity.gameObject : null;
        if (_npcDeco != null)
            _npcDeco.Stop(id);
        UnregisterObject(id, entity);
        NameplatesManager nameplates = NameplatePacketServices.Nameplates;
        if (nameplates != null)
            nameplates.Remove(id);
        SetEntityInteraction(go, false);

        if (_appearFade == null || go == null)
        {
            if (onFinished != null)
                onFinished(go);
            return;
        }

        _appearFade.BeginDisappear(entity, onFinished);
    }
}
