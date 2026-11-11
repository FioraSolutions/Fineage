using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public abstract partial class AbstractPoolManager : MonoBehaviour
{
    protected Transform EnsurePoolSlot(ObjectType tag, List<Pool> pools, Transform poolParent)
    {
        Transform existing = GetParent(tag, pools);
        if (existing != null)
        {
            return existing;
        }

        if (pools == null)
        {
            return poolParent;
        }

        GameObject slot = new GameObject($"_PoolSlot_{tag}");
        slot.SetActive(false);
        if (poolParent != null)
        {
            slot.transform.SetParent(poolParent);
        }

        pools.Add(new Pool
        {
            tag = tag,
            prefab = null,
            size = 0,
            usePrefab = slot
        });

        return slot.transform;
    }

    protected Transform GetParent(ObjectType tag, List<Pool> pools)
    {
        if (pools == null)
        {
            return null;
        }

        for (int b = 0; b < pools.Count; b++)
        {
            Pool pollParent = pools[b];
            if (pollParent.tag == tag && pollParent.usePrefab != null)
            {
                return pollParent.usePrefab.transform;
            }
        }

        return null;
    }

    protected void Plus1Create(GameObject prefab)
    {
        if (createdInstancesTracker.ContainsKey(prefab))
        {
            int countCreate = createdInstancesTracker[prefab];
            createdInstancesTracker[prefab] = countCreate + 1;
        }
        else
        {
            createdInstancesTracker.Add(prefab, 1);
        }
    }

    protected void Minus1Create(GameObject prefab)
    {
        if (prefab == null || !createdInstancesTracker.ContainsKey(prefab))
        {
            return;
        }

        int count = createdInstancesTracker[prefab] - 1;
        if (count <= 0)
        {
            createdInstancesTracker.Remove(prefab);
        }
        else
        {
            createdInstancesTracker[prefab] = count;
        }
    }

    protected int GetCreateCount(GameObject prefab)
    {
        if (createdInstancesTracker.ContainsKey(prefab))
        {
            return createdInstancesTracker[prefab];
        }

        return 0;
    }
}
