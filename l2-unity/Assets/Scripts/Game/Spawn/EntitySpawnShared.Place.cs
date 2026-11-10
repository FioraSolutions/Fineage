

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
        // Village NPCs: keep server Z (DE caves under 20_18 otherwise steal GroundSnap).
        // Monsters still snap to walkable mesh.
        Vector3 placePos = identity.EntityType == EntityType.NPC
            ? serverPos
            : GroundSnapHelper.SnapToGroundOrKeep(serverPos);
        identity.Position = placePos;
        if (poolType.HasValue && pool != null)
        {
            ObjectType tag = poolType.Value;
            pool.AddPrefabToPool(tag, prefab, 1);
            GameObject pooled = pool.SpawnFromPool(tag, prefab);
            if (pooled != null)
            {
                ApplyGroundedTransform(pooled, placePos, identity.Heading);
                return pooled;
            }
        }

        bool prefabWasActive = prefab.activeSelf;
        if (prefabWasActive)
            prefab.SetActive(false);

        GameObject npcGo = Object.Instantiate(prefab, placePos, identity.Heading);
        ApplyGroundedTransform(npcGo, placePos, identity.Heading);

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
        Vector3 grounded = GroundSnapHelper.SnapToGroundOrKeep(position);
        ApplyGroundedTransform(go, grounded, rotation);
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
        if (!GroundSnapHelper.TrySnapToGround(from, out Vector3 snapped))
            return false;

        identity.Position = snapped;
        ApplyGroundedTransform(go, snapped, identity.Heading);
        return true;
    }

    public static void ApplyGroundedTransform(GameObject go, Vector3 position, Quaternion rotation)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        bool[] wasEnabled = new bool[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            wasEnabled[i] = controllers[i].enabled;
            controllers[i].enabled = false;
        }

        go.transform.SetPositionAndRotation(position, rotation);

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            controllers[i].enabled = wasEnabled[i];
        }
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
