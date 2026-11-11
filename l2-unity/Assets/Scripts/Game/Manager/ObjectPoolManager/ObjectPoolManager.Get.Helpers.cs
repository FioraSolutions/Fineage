using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ObjectPoolManager : AbstractPoolManager, IPoolManager
{
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
