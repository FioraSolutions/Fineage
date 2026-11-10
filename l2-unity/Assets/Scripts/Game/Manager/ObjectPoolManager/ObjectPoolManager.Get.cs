using System;

public partial class ObjectPoolManager : AbstractPoolManager, IPoolManager
{
    public GameObject SpawnFromPool(ObjectType tag, GameObject specificPrefab = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            // Debug.LogWarning($"Пул с тегом {tag} не существует.");
            return null;
        }

        Dictionary<GameObject, Queue<GameObject>> prefabPools = poolDictionary[tag];
        GameObject prefab = specificPrefab ?? tagToPrefabMap[tag];

        if (!prefabPools.ContainsKey(prefab))
        {
            // First spawn of an unpooled armor/weapon prefab (e.g. onepiece robe).
            AddPrefabToPool(tag, prefab, count: 1);
            if (!prefabPools.ContainsKey(prefab))
                return null;
        }

        Queue<GameObject> objectPool = prefabPools[prefab];
        int queueBefore = objectPool.Count;

        if (queueBefore == 0)
        {
            // Grow by one and hand out — do NOT enqueue (old bug: same GO in queue + in use).
            Transform parentTag = EnsurePoolSlot(tag, pools, poolParent);
            GameObject newObj = CopyObject(tag, prefab, parentTag, poolParent);
            newObj.SetActive(false);
            Plus1Create(prefab);
            // Debug.Log(
                // $"[ObjectPool] Spawn GREW {tag}/{prefab.name} " +
                // $"queue=0/{objectTypePoolLimits[tag]} created={GetCreateCount(prefab)}");
            return newObj;
        }

        GameObject objectToSpawn = objectPool.Dequeue();
        EnsurePooledMeta(objectToSpawn, tag, prefab);
        // Debug.Log(
            // $"[ObjectPool] Spawn REUSED {tag}/{prefab.name} " +
            // $"queue={objectPool.Count}/{objectTypePoolLimits[tag]}");
        return objectToSpawn;
    }

    public bool ReturnToPool(ObjectType tag, GameObject objectToReturn)
    {
        if (objectToReturn == null)
        {
            // Debug.LogWarning($"[ObjectPool] Return REJECT tag={tag} go=null");
            return false;
        }

        if (!ValidatePool(tag))
        {
            // Debug.LogWarning($"[ObjectPool] Return REJECT id-go={objectToReturn.name} no pool tag={tag}");
            return false;
        }

        if (!FindMatchingPrefab(tag, objectToReturn, out GameObject prefab))
        {
            // Debug.LogWarning(
                // $"[ObjectPool] Return REJECT FindMatchingPrefab FAIL go={objectToReturn.name} tag={tag}");
            return false;
        }

        if (_maxSizePool <= 0)
        {
            // Debug.LogWarning($"[ObjectPool] Return REJECT maxSizePool<=0 go={objectToReturn.name}");
            return false;
        }

        if (!PrepareObjectForReturn(objectToReturn, tag))
        {
            // Debug.LogWarning($"[ObjectPool] Return REJECT Prepare FAIL go={objectToReturn.name}");
            return false;
        }

        return HandlePoolReturn(tag, prefab, objectToReturn, _maxSizePool);
    }

    private bool PrepareObjectForReturn(GameObject objectToReturn, ObjectType tag)
    {
        Transform parent = EnsurePoolSlot(tag, pools, poolParent);
        if (parent == null)
        {
            return false;
        }

        objectToReturn.transform.SetParent(parent);
        ResetPosition(objectToReturn);

        PooledInstance pooled = objectToReturn.GetComponent<PooledInstance>();
        if (pooled != null &&
            (tag == ObjectType.Npc || tag == ObjectType.Monster || tag == ObjectType.User))
        {
            // DeadManager fades via material instances — restore shared mats for next spawn.
            pooled.RestoreSharedMaterials();
        }

        objectToReturn.SetActive(false);
        return true;
    }

    private bool HandlePoolReturn(ObjectType tag, GameObject prefab, GameObject objectToReturn, int maxSize)
    {
        Queue<GameObject> objectPool = poolDictionary[tag][prefab];
        int currentLimit = objectTypePoolLimits[tag];

        if (objectPool.Count >= currentLimit)
        {
            // Debug.LogWarning(
                // $"[ObjectPool] Return DROP(destroy) {tag}/{objectToReturn.name} " +
                // $"queueFull={objectPool.Count}/{currentLimit}");
            Minus1Create(prefab);
            Destroy(objectToReturn);
        }
        else
        {
            objectPool.Enqueue(objectToReturn);
            // Debug.Log(
                // $"[ObjectPool] Return OK {tag}/{prefab.name} " +
                // $"queue={objectPool.Count}/{currentLimit}");
        }

        return true;
    }
}
