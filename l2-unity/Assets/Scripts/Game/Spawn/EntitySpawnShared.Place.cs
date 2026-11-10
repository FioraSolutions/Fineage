using System.Collections;
using UnityEngine;

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
        float[] at = { 0.25f, 1f, 2f };
        float elapsed = 0f;
        for (int i = 0; i < at.Length; i++)
        {
            yield return new WaitForSeconds(at[i] - elapsed);
            elapsed = at[i];
            if (go == null || identity == null)
                yield break;

            if (TryApplyGroundSnap(go, identity))
                yield break;
        }
    }

    static bool TryApplyGroundSnap(GameObject go, EntityIdentity identity)
    {
        if (PetSummonRise.IsRising(go))
            return false;

        Vector3 from = go.transform.position;
        WorldPlacement.PlacementKind kind = identity.EntityType == EntityType.Monster
            ? WorldPlacement.PlacementKind.MonsterSpawn
            : WorldPlacement.PlacementKind.NpcSpawn;
        Vector3 snapped = WorldPlacement.Resolve(kind, from);
        if ((snapped - from).sqrMagnitude < 0.0001f)
            return false;

        identity.Position = snapped;
        WorldPlacement.ApplyPose(go, snapped, identity.Heading);
        WorldPlacement.SettleFeet(go, snapped);
        return true;
    }

    public static void ApplyGroundedTransform(GameObject go, Vector3 position, Quaternion rotation)
    {
        WorldPlacement.ApplyPose(go, position, rotation);
    }

    public static void SanitizeCharacterControllerStepOffset(GameObject root)
    {
        if (root == null)
            return;

        CharacterController[] controllers = root.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            CharacterController cc = controllers[i];
            if (cc == null)
                continue;
            if (cc.radius <= 0f)
                cc.radius = 0.1f;
            if (cc.height < cc.radius * 2f)
                cc.height = cc.radius * 2f;
            // Unity: stepOffset <= height + 2*radius; keep well inside to avoid the warning on scaled rigs.
            float maxStep = Mathf.Max(0f, Mathf.Min(cc.height * 0.5f, cc.height + cc.radius * 2f - 0.01f));
            if (cc.stepOffset > maxStep)
                cc.stepOffset = maxStep;
        }
    }

    /// <summary>
    /// Leftover client-prediction: SetNewPosition is never called, so it lerps back to spawn.
    /// </summary>
    public static void DisableLegacyPositionSync(GameObject root)
    {
        if (root == null)
            return;

        NetworkTransformReceive receive = root.GetComponent<NetworkTransformReceive>();
        if (receive != null)
            receive.enabled = false;
    }
}
