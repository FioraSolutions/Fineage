using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public abstract partial class AbstractPoolManager : MonoBehaviour
{
    protected void SetupPoolHierarchy(List<Pool> pools, Transform poolParent)
    {
        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                // Debug.LogWarning($"Prefab for tag {pool.tag} is not set!");
                continue;
            }

            if (!poolDictionary.ContainsKey(pool.tag))
            {
                poolDictionary[pool.tag] = new Dictionary<GameObject, Queue<GameObject>>();
                tagToPrefabMap[pool.tag] = pool.prefab;
            }

            if (!poolDictionary[pool.tag].ContainsKey(pool.prefab))
            {
                poolDictionary[pool.tag][pool.prefab] = new Queue<GameObject>();
            }

            Queue<GameObject> prefabPool = poolDictionary[pool.tag][pool.prefab];
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolParent);
                pool.usePrefab = obj;
                obj.SetActive(false);
                //prefabPool.Enqueue(obj);
            }
        }
    }

    public void SetPoolLimit(ObjectType type, int maxSize)
    {
        if (maxSize <= 0)
        {
            // Debug.LogWarning($"Лимит пула для {type} должен быть больше 0!");
            return;
        }

        objectTypePoolLimits[type] = maxSize;
        // Debug.Log($"Установлен лимит пула для {type} равный {maxSize}");
    }

    protected void ValidateAndCreateDictionary(ObjectType tag, GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            poolDictionary[tag] = new Dictionary<GameObject, Queue<GameObject>>();
            tagToPrefabMap[tag] = prefab;
        }
    }
}
