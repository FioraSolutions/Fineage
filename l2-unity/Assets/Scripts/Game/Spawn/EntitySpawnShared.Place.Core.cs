using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public static partial class EntitySpawnShared
{
    public static GameObject AcquireNpcGameObject(
        ObjectPoolManager pool,
        GameObject prefab,
        EntityIdentity identity)
    {
        if (prefab == null)
            return null;

        ObjectType? poolType = null;
        if (identity.EntityType == EntityType.NPC)
            poolType = ObjectType.Npc;
        else if (identity.EntityType == EntityType.Monster)
            poolType = ObjectType.Monster;

        Vector3 serverPos = identity.Position;
        WorldPlacement.PlacementKind kind = identity.EntityType == EntityType.Monster
            ? WorldPlacement.PlacementKind.MonsterSpawn
            : WorldPlacement.PlacementKind.NpcSpawn;
        Vector3 placePos = WorldPlacement.Resolve(kind, serverPos);
        identity.Position = placePos;
        if (poolType.HasValue && pool != null)
        {
            ObjectType tag = poolType.Value;
            pool.AddPrefabToPool(tag, prefab, 1);
            GameObject pooled = pool.SpawnFromPool(tag, prefab);
            if (pooled != null)
            {
                WorldPlacement.ApplyPose(pooled, placePos, identity.Heading);
                WorldPlacement.SettleFeet(pooled, placePos);
                return pooled;
            }
        }

        bool prefabWasActive = prefab.activeSelf;
        if (prefabWasActive)
            prefab.SetActive(false);

        GameObject npcGo = Object.Instantiate(prefab, placePos, identity.Heading);
        WorldPlacement.ApplyPose(npcGo, placePos, identity.Heading);
        WorldPlacement.SettleFeet(npcGo, placePos);

        if (prefabWasActive)
            prefab.SetActive(true);

        return npcGo;
    }

    public static void Place(GameObject go, Transform parent, Vector3 position, Quaternion rotation, string name)
    {
        if (go == null)
            return;
        if (parent != null)
            go.transform.SetParent(parent);
        Vector3 grounded = WorldPlacement.Resolve(WorldPlacement.PlacementKind.WalkValidate, position);
        WorldPlacement.ApplyPose(go, grounded, rotation);
        WorldPlacement.SettleFeet(go, grounded);
        if (!string.IsNullOrEmpty(name))
            go.transform.name = name;
    }

    public static void ReapplyGroundAfterActivate(GameObject go, EntityIdentity identity)
    {
        if (go == null || identity == null)
            return;

        TryApplyGroundSnap(go, identity);

        MonoBehaviour host = go.GetComponent<Entity>();
        if (host == null)
            host = WorldApplyPacketServices.GameWorld;
        if (host != null)
            host.StartCoroutine(RetryGroundSnap(go, identity));
    }

    static IEnumerator RetryGroundSnap(GameObject go, EntityIdentity identity)
    {
        // Tile mesh colliders often appear after the NpcInfo that spawned the body.
        float[] at = { 0.25f, 1f, 2f, 5f };
        float elapsed = 0f;
        for (int i = 0; i < at.Length; i++)
        {
            yield return new WaitForSeconds(at[i] - elapsed);
            elapsed = at[i];
            if (go == null || identity == null)
                yield break;

            // Keep going: a foot settle before the tile collider exists is not the floor.
            TryApplyGroundSnap(go, identity);
        }
    }
}
