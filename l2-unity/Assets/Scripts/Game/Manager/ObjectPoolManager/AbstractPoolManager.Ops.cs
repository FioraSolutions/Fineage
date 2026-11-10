using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public abstract partial class AbstractPoolManager : MonoBehaviour
{
    protected GameObject CopyObject(
        ObjectType tag,
        GameObject prefab,
        Transform parentTag,
        Transform poolParent)
    {
        bool prefabWasActive = prefab != null && prefab.activeSelf;
        if (prefabWasActive)
        {
            prefab.SetActive(false);
        }

        GameObject obj = parentTag != null
            ? Instantiate(prefab, parentTag)
            : Instantiate(prefab, poolParent);

        if (prefabWasActive)
        {
            prefab.SetActive(true);
        }

        EnsurePooledMeta(obj, tag, prefab);
        SanitizeCharacterControllerStepOffset(obj);
        return obj;
    }

    /// <summary>Legacy gear path — Weapon/Armor/Arrow; preserves old signature.</summary>
    protected GameObject CopyObject(GameObject prefab, Transform parentTag, Transform poolParent)
    {
        return CopyObject(ObjectType.Weapon, prefab, parentTag, poolParent);
    }

    protected void EnsurePooledMeta(GameObject instance, ObjectType tag, GameObject sourcePrefab)
    {
        if (instance == null)
        {
            return;
        }

        PooledInstance pooled = instance.GetComponent<PooledInstance>();
        if (pooled == null)
        {
            pooled = instance.AddComponent<PooledInstance>();
            pooled.OriginalLocalScale = instance.transform.localScale;
        }

        pooled.Type = tag;
        pooled.SourcePrefab = sourcePrefab;
        if (tag == ObjectType.Npc || tag == ObjectType.Monster || tag == ObjectType.User)
        {
            pooled.CaptureSharedMaterials();
        }
    }

    protected static void SanitizeCharacterControllerStepOffset(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        CharacterController[] controllers = root.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            CharacterController cc = controllers[i];
            if (cc != null)
            {
                cc.stepOffset = 0f;
            }
        }
    }

    /// <summary>
    /// Scene pools list may not define Npc yet — create a hidden slot under poolParent.
    /// </summary>
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
