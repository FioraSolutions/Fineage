using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Single client visibility authority: known-list pawns, Interlude clip distances, LOD tiers, char cap.
/// </summary>
public sealed class VisibilityManager : MonoBehaviour
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
    readonly int[] _histogram = new int[5];

    CrowdLodProfile _profile;
    string _histogramLine;
    float _terrainAppliedMeters;

    void Awake()
    {
        _profile = CrowdLodProfileCatalog.LoadActive();
        VisibilityPacketServices.Bind(this);
        VisibilityTierRegistry.Bind(GetTier, ShouldShowNameplate, () => _histogramLine);
    }

    void OnDestroy()
    {
        if (VisibilityPacketServices.Manager == this)
        {
            VisibilityPacketServices.Bind(null);
            VisibilityTierRegistry.Clear();
        }
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
        NpcVoicePlaybackGate.End(entityId);

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

    public bool ShouldPlayWorldAudio(int entityId, Vector3 worldPosition)
    {
        if (entityId != 0 && GetTier(entityId) == CrowdDetailTier.TCulled)
            return false;

        if (!AudioWorldGate.TryGetListenerPosition(out Vector3 listener))
            return entityId == 0;

        Vector3 d = worldPosition - listener;
        d.y = 0f;
        float hear = InterludeVisibilityOptions.WorldHearDistanceMeters;
        return d.sqrMagnitude <= hear * hear;
    }

    public bool ShouldPlayNpcVoice(int entityId, NpcVoicePriority priority)
    {
        if (entityId == 0)
            return true;
        if (GetTier(entityId) == CrowdDetailTier.TCulled)
            return false;
        return NpcVoicePlaybackGate.TryBegin(entityId, priority);
    }

    public void NotifyNpcVoiceEnded(int entityId) => NpcVoicePlaybackGate.End(entityId);

    public float NameplateViewDistanceMeters => InterludeVisibilityOptions.NameplateViewDistanceMeters;

    public string HistogramLine => _histogramLine;

    void LateUpdate()
    {
        using (FineagePerfProfiling.CrowdTier.Auto())
        {
            StepTiers();
            ApplyTerrainDrawDistance();
        }
    }

    void StepTiers()
    {
        if (_profile == null)
            _profile = CrowdLodProfileCatalog.LoadActive();

        for (int i = 0; i < _histogram.Length; i++)
            _histogram[i] = 0;

        if (_entries.Count == 0)
        {
            _histogramLine = FormatHistogram(0);
            return;
        }

        if (!TryGetObserver(out float3 observer))
            return;

        float scale = InterludeVisibilityOptions.TierDistanceScaleForModelDetail();
        float tier1Sq = _profile.Tier1MaxSq * scale * scale;
        float tier2Sq = _profile.Tier2MaxSq * scale * scale;
        float tier3Sq = _profile.Tier3MaxSq * scale * scale;
        float pawnDrawSq = InterludeVisibilityOptions.PawnDrawDistanceSq;

        int count = _entries.Count;
        NativeArray<float3> positions = new NativeArray<float3>(count, Allocator.TempJob);
        NativeArray<byte> tiers = new NativeArray<byte>(count, Allocator.TempJob);
        var entityIds = new int[count];
        var distSq = new float[count];
        var tierScratch = new CrowdDetailTier[count];

        for (int i = 0; i < count; i++)
        {
            Entry entry = _entries[i];
            entityIds[i] = entry.Id;
            Transform root = entry.Root;
            float3 pos = root != null ? (float3)root.position : observer;
            positions[i] = pos;
            float3 d = pos - observer;
            d.y = 0f;
            distSq[i] = math.lengthsq(d);
        }

        var job = new AssignCrowdTierJob
        {
            Positions = positions,
            Observer = observer,
            Tier1MaxSq = tier1Sq,
            Tier2MaxSq = tier2Sq,
            Tier3MaxSq = tier3Sq,
            PawnDrawMaxSq = pawnDrawSq,
            Tiers = tiers,
        };

        JobHandle handle = job.Schedule(count, 64);
        handle.Complete();

        for (int i = 0; i < count; i++)
            tierScratch[i] = (CrowdDetailTier)tiers[i];

        VisibilityDecisionPolicy.ApplyRenderCharacterCap(
            entityIds,
            distSq,
            tierScratch,
            InterludeVisibilityOptions.RenderCharacterCount,
            localPlayerIndex: -1);

        for (int i = 0; i < count; i++)
        {
            int id = entityIds[i];
            var tier = tierScratch[i];
            _tierById[id] = tier;
            Transform root = _entries[i].Root;
            if (root != null)
            {
                CrowdTierVisualPolicy.Apply(root, tier, _profile);
                ApplyCulledDraw(root, tier);
            }
        }

        positions.Dispose();
        tiers.Dispose();

        foreach (KeyValuePair<int, CrowdDetailTier> kvp in _tierById)
        {
            int b = (int)kvp.Value;
            if (b >= 0 && b < _histogram.Length)
                _histogram[b]++;
        }

        _histogramLine = FormatHistogram(count);
    }

    static void ApplyCulledDraw(Transform root, CrowdDetailTier tier)
    {
        bool show = tier != CrowdDetailTier.TCulled;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(false);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r != null && r.enabled != show)
                r.enabled = show;
        }
    }

    string FormatHistogram(int tracked)
    {
        return "visibility T0=" + _histogram[0] + " T1=" + _histogram[1] + " T2=" + _histogram[2]
            + " T3=" + _histogram[3] + " culled=" + _histogram[4] + " n=" + tracked
            + " cap=" + InterludeVisibilityOptions.RenderCharacterCount;
    }

    void ApplyTerrainDrawDistance()
    {
        float meters = InterludeVisibilityOptions.TerrainDrawDistanceMeters;
        if (Mathf.Abs(meters - _terrainAppliedMeters) < 0.5f)
            return;
        _terrainAppliedMeters = meters;

        Camera cam = Camera.main;
        if (cam != null && cam.farClipPlane < meters)
            cam.farClipPlane = meters;

        var props = FindObjectsByType<L2InstancedPropRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < props.Length; i++)
        {
            if (props[i] != null)
                props[i].MaxDrawDistance = meters;
        }
    }

    static bool TryGetObserver(out float3 observer)
    {
        if (AudioWorldGate.TryGetListenerPosition(out Vector3 listener))
        {
            observer = listener;
            return true;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            observer = float3.zero;
            return false;
        }

        observer = cam.transform.position;
        return true;
    }
}