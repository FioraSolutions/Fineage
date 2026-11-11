using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

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


    #endregion


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



}
