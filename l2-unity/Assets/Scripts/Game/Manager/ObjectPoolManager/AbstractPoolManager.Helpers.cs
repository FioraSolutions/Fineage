using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public abstract partial class AbstractPoolManager : MonoBehaviour
{
    protected void ValidAndCreateQueue(ObjectType tag, GameObject prefab)
    {
        if (!poolDictionary[tag].ContainsKey(prefab))
        {
            poolDictionary[tag][prefab] = new Queue<GameObject>();
        }
    }

    protected bool ValidatePool(ObjectType tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            // Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return false;
        }

        return true;
    }

    protected bool FindMatchingPrefab(ObjectType tag, GameObject objectToReturn, out GameObject prefab)
    {
        prefab = null;
        if (objectToReturn == null)
        {
            return false;
        }

        PooledInstance pooled = objectToReturn.GetComponent<PooledInstance>();
        if (pooled != null &&
            pooled.SourcePrefab != null &&
            pooled.Type == tag &&
            poolDictionary.ContainsKey(tag) &&
            poolDictionary[tag].ContainsKey(pooled.SourcePrefab))
        {
            prefab = pooled.SourcePrefab;
            return true;
        }

        Dictionary<GameObject, Queue<GameObject>> prefabPools = poolDictionary[tag];
        foreach (KeyValuePair<GameObject, Queue<GameObject>> kvp in prefabPools)
        {
            if (objectToReturn.name.StartsWith(kvp.Key.name))
            {
                prefab = kvp.Key;
                return true;
            }
        }

        // Debug.LogWarning($"Could not find matching prefab for object {objectToReturn.name} in pool {tag}");
        return false;
    }

    protected void ResetPosition(GameObject objectToReturn)
    {
        objectToReturn.transform.localPosition = Vector3.zero;
        objectToReturn.transform.localRotation = Quaternion.identity;

        PooledInstance pooled = objectToReturn.GetComponent<PooledInstance>();
        if (pooled != null &&
            (pooled.Type == ObjectType.Npc || pooled.Type == ObjectType.Monster || pooled.Type == ObjectType.User))
        {
            // L2 character meshes often ship with non-1 scale — don't flatten them.
            objectToReturn.transform.localScale = pooled.OriginalLocalScale;
        }
        else
        {
            objectToReturn.transform.localScale = Vector3.one;
        }
    }
}
