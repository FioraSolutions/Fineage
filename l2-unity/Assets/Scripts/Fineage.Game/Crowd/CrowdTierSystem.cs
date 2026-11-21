using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>Burst tier assignment for visible pawns (roadmap step 13.1).</summary>
public sealed class CrowdTierSystem : MonoBehaviour
{
    readonly struct Entry
    {
        public readonly int Id;
        public readonly Transform Root;

        public Entry(int id, Transform root)
        {
            Id = id;
            Root = root;
        }
    }

    readonly List<Entry> _entries = new List<Entry>(256);
    readonly Dictionary<int, int> _indexById = new Dictionary<int, int>(256);
    readonly Dictionary<int, CrowdDetailTier> _tierById = new Dictionary<int, CrowdDetailTier>(256);
    readonly int[] _histogram = new int[4];

    CrowdLodProfile _profile;
    string _histogramLine;

    void Awake()
    {
        _profile = CrowdLodProfileCatalog.LoadActive();
        CrowdTierPacketServices.Bind(this);
    }

    void OnDestroy()
    {
        if (CrowdTierPacketServices.System == this)
            CrowdTierPacketServices.Bind(null);
    }

    public void Register(int entityId, Transform root, bool isLocalPlayer = false)
    {
        if (entityId == 0 || root == null)
            return;

        if (isLocalPlayer)
        {
            _tierById[entityId] = CrowdDetailTier.T0Hero;
            return;
        }

        if (_indexById.ContainsKey(entityId))
            return;

        int index = _entries.Count;
        _entries.Add(new Entry(entityId, root));
        _indexById[entityId] = index;
        _tierById[entityId] = CrowdDetailTier.T1Near;
    }

    public void Unregister(int entityId)
    {
        if (entityId == 0)
            return;

        _tierById.Remove(entityId);
        if (!_indexById.TryGetValue(entityId, out int removeIndex))
            return;

        int lastIndex = _entries.Count - 1;
        if (removeIndex != lastIndex)
        {
            Entry moved = _entries[lastIndex];
            _entries[removeIndex] = moved;
            _indexById[moved.Id] = removeIndex;
        }

        _entries.RemoveAt(lastIndex);
        _indexById.Remove(entityId);
    }

    public CrowdDetailTier GetTier(int entityId)
    {
        if (_tierById.TryGetValue(entityId, out CrowdDetailTier tier))
            return tier;
        return CrowdDetailTier.T1Near;
    }

    public bool ShouldShowNameplate(int entityId) =>
        _profile != null && _profile.ShouldShowNameplate(GetTier(entityId));

    public string HistogramLine => _histogramLine;

    void LateUpdate()
    {
        using (FineagePerfProfiling.CrowdTier.Auto())
            StepTiers();
    }

    void StepTiers()
    {
        if (_profile == null)
            _profile = CrowdLodProfileCatalog.LoadActive();

        for (int i = 0; i < _histogram.Length; i++)
            _histogram[i] = 0;

        foreach (KeyValuePair<int, CrowdDetailTier> kvp in _tierById)
        {
            int b = (int)kvp.Value;
            if (b >= 0 && b < _histogram.Length)
                _histogram[b]++;
        }

        if (_entries.Count == 0)
        {
            _histogramLine = $"crowd tiers T0={_histogram[0]} T1={_histogram[1]} T2={_histogram[2]} T3={_histogram[3]}";
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
            return;

        float3 observer = cam.transform.position;
        int count = _entries.Count;
        NativeArray<float3> positions = new NativeArray<float3>(count, Allocator.TempJob);
        NativeArray<byte> tiers = new NativeArray<byte>(count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            Transform root = _entries[i].Root;
            positions[i] = root != null ? (float3)root.position : observer;
        }

        var job = new AssignCrowdTierJob
        {
            Positions = positions,
            Observer = observer,
            Tier1MaxSq = _profile.Tier1MaxSq,
            Tier2MaxSq = _profile.Tier2MaxSq,
            Tier3MaxSq = _profile.Tier3MaxSq,
            Tiers = tiers,
        };

        JobHandle handle = job.Schedule(count, 64);
        handle.Complete();

        for (int i = 0; i < count; i++)
        {
            int id = _entries[i].Id;
            var tier = (CrowdDetailTier)tiers[i];
            _tierById[id] = tier;
            Transform root = _entries[i].Root;
            if (root != null)
                CrowdTierVisualPolicy.Apply(root, tier, _profile);
        }

        positions.Dispose();
        tiers.Dispose();

        for (int i = 0; i < _histogram.Length; i++)
            _histogram[i] = 0;
        foreach (KeyValuePair<int, CrowdDetailTier> kvp in _tierById)
        {
            int b = (int)kvp.Value;
            if (b >= 0 && b < _histogram.Length)
                _histogram[b]++;
        }

        _histogramLine =
            $"crowd tiers T0={_histogram[0]} T1={_histogram[1]} T2={_histogram[2]} T3={_histogram[3]} n={count}";
    }
}