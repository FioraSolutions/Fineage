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
}
