using System;

using System.Collections.Generic;
using UnityEngine;

public partial class ObjectPoolManager : AbstractPoolManager, IPoolManager
{
    [System.Serializable]
    public class Pool
    {
        public ObjectType tag;
        public GameObject prefab;
        public int size;
        public GameObject usePrefab;
    }

    public List<Pool> pools;
    // Минимальный размер пула >= 3
    private int _maxSizePool = 3;
    [SerializeField] private Transform poolParent;

    #region Singleton
    // Interface-typed static: after Restart the old pool is destroyed but not C# null, and the new
    // Game scene is preloaded inactive (no Awake). `Instance?.` then hit a dead pool, LoadMesh threw,
    // and every armor/weapon mesh came back missing (heads-only lobby pawns).
    static IPoolManager _instance;

    [System.Obsolete("Use [Inject] or ObjectPoolPacketServices.Pool instead")]
    public static IPoolManager Instance
    {
        get { return (_instance as UnityEngine.Object) != null ? _instance : null; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
        ObjectPoolPacketServices.Bind(pool: this);
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
    }
    #endregion

    void Start()
    {
        poolDictionary = new Dictionary<ObjectType, Dictionary<GameObject, Queue<GameObject>>>();
        tagToPrefabMap = new Dictionary<ObjectType, GameObject>();
        createdInstancesTracker = new Dictionary<GameObject, int>();
        objectTypePoolLimits = new Dictionary<ObjectType, int>();

        foreach (ObjectType type in System.Enum.GetValues(typeof(ObjectType)))
        {
            objectTypePoolLimits[type] = _maxSizePool;
        }

        SetPoolLimit(ObjectType.Arrow, 25);
        // Shared meshes (e.g. many a_guard_*) need headroom — limit=2 destroyed returns on leave-city.
        SetPoolLimit(ObjectType.Npc, 32);
        // Same headroom as Npc/guards; can tune later.
        SetPoolLimit(ObjectType.Monster, 32);
        // Idle UserEntity bodies per race prefab. Live players are not this cap.
        SetPoolLimit(ObjectType.User, 24);
        SetupPoolHierarchy(pools, poolParent);
        PrewarmUserContainers();
        // Debug.Log($"Создание пула объектов успешно. Размер: {poolDictionary.Count}");
    }

    static readonly CharacterRaceAnimation[] UserPrewarmRaces =
    {
        CharacterRaceAnimation.MFighter,
        CharacterRaceAnimation.FFighter,
        CharacterRaceAnimation.MMagic,
        CharacterRaceAnimation.FMagic,
        CharacterRaceAnimation.MElf,
        CharacterRaceAnimation.FElf,
        CharacterRaceAnimation.MDarkElf,
        CharacterRaceAnimation.FDarkElf
    };

    void PrewarmUserContainers()
    {
        ModelTable models = ObjectPoolPacketServices.Models;
        if (models == null)
            return;

        const int idlePerRace = 4;
        for (int i = 0; i < UserPrewarmRaces.Length; i++)
        {
            GameObject prefab = models.GetContainer(UserPrewarmRaces[i], EntityType.User);
            if (prefab != null)
                AddPrefabToPool(ObjectType.User, prefab, idlePerRace);
        }
    }

    public void AddPrefabToPool(ObjectType tag, GameObject prefab, int count = 2)
    {
        if (prefab == null)
        {
            // Debug.LogWarning("Префаб не может быть null!");
            return;
        }

        if (count <= 0)
        {
            // Debug.LogWarning("Количество должно быть больше 0!");
            return;
        }

        ValidateAndCreateDictionary(tag, prefab);
        ValidAndCreateQueue(tag, prefab);

        int currentLimit = objectTypePoolLimits[tag];

        if (GetCreateCount(prefab) >= currentLimit)
        {
            return;
        }

        Transform parentTag = EnsurePoolSlot(tag, pools, poolParent);
        Queue<GameObject> prefabPool = poolDictionary[tag][prefab];

        int availableSpace = currentLimit - prefabPool.Count;
        int objectsToAdd = Mathf.Min(count, availableSpace);

        if (objectsToAdd <= 0)
        {
            // Debug.Log(
                // $"Пул для {prefab.name} достиг максимального размера ({currentLimit}). " +
                // $"Невозможно добавить {count} объектов.");
            return;
        }

        for (int i = 0; i < objectsToAdd; i++)
        {
            if (GetCreateCount(prefab) >= currentLimit)
            {
                break;
            }

            GameObject obj = CopyObject(tag, prefab, parentTag, poolParent);
            obj.SetActive(false);
            prefabPool.Enqueue(obj);
            Plus1Create(prefab);
        }

        // Debug.Log(
            // $"[ObjectPool] Add {objectsToAdd} → {tag}/{prefab.name} " +
            // $"queue={prefabPool.Count}/{currentLimit} created={GetCreateCount(prefab)}");
    }

}
