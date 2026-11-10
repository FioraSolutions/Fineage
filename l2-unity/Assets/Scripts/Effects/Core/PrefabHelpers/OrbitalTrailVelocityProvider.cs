using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed partial class OrbitalTrailVelocityProvider : MonoBehaviour
{
    [Serializable]
    public sealed class TrailBinding
    {
        public string name;
        public Transform tailRoot;
        public Transform velocitySource;
        public string velocitySourceName = "waterball";
        public bool useOrbitRadiusOverride = true;
        public Vector3 orbitRadiusLocalOverride = new Vector3(0f, 0f, 0.35f);
        public bool preferSelfPositionForRadius;
        public Renderer[] targetRenderers;
        public bool autoCollectChildren = true;
        public bool setDirectionFlags;
        public bool convertWorldToLocal = true;
        public Transform localSpaceReference;
        public float velocityScale = 0.35f;
        public float rangeSpread = 0.2f;
        public bool invertTrailDirection = true;
        public float trailSign = 1f;

        [NonSerialized] public Vector3 lastPosition;
        [NonSerialized] public Vector3 smoothedVelocity;
        [NonSerialized] public bool hasLastPosition;
        [NonSerialized] public bool loggedFirstVelocity;
    }

    [Header("Shared Orbit Settings")]
    [SerializeField] private bool _useSyntheticOrbitVelocity = true;
    [SerializeField] private EffectPrefabRotator _rotatorSource;
    [SerializeField] private Transform _orbitCenter;
    [SerializeField] private bool _clockwise;
    [SerializeField] private float _minimumOrbitRadius = 0.01f;
    [SerializeField] private float _minimumAxisRange = 0.01f;
    [SerializeField] private float _smoothing = 16f;
    [SerializeField] private bool _debugLogs = true;

    [Header("Bindings (Tail -> Source)")]
    [SerializeField] private List<TrailBinding> _bindings = new();

    // UE2.5 macro-world: Unity meters * 52.5 = Unreal Units (L2Fx_UcPositionToUnityMeters).
    private const float UnityMetersToUu = 52.5f;

    private static readonly int UseDirectionAsID = Shader.PropertyToID("_UseDirectionAs");
    private static readonly int GetVelocityDirectionFromID = Shader.PropertyToID("_GetVelocityDirectionFrom");
    private static readonly int StartVelocityRangeXID = Shader.PropertyToID("_StartVelocityRangeX");
    private static readonly int StartVelocityRangeYID = Shader.PropertyToID("_StartVelocityRangeY");
    private static readonly int StartVelocityRangeZID = Shader.PropertyToID("_StartVelocityRangeZ");
    private static readonly int StartVelocityRangeXUcID = Shader.PropertyToID("_StartVelocityRangeXUc");
    private static readonly int StartVelocityRangeYUcID = Shader.PropertyToID("_StartVelocityRangeYUc");
    private static readonly int StartVelocityRangeZUcID = Shader.PropertyToID("_StartVelocityRangeZUc");

    private void OnEnable()
    {
        ResolveSharedSources();

        for (int i = 0; i < _bindings.Count; i++)
        {
            TrailBinding binding = _bindings[i];
            if (binding == null || binding.tailRoot == null)
            {
                continue;
            }

            ResolveBindingSources(binding);
            binding.hasLastPosition = false;
            binding.smoothedVelocity = Vector3.zero;
            binding.loggedFirstVelocity = false;

            if (_debugLogs)
            {
                int rendererCount = binding.targetRenderers != null ? binding.targetRenderers.Length : 0;
                string sourceName = binding.velocitySource != null ? binding.velocitySource.name : "null";
                // Debug.Log($"[OrbitalTrailVelocityProvider] Enabled binding='{binding.name}' tail='{binding.tailRoot.name}' source={sourceName} renderers={rendererCount}", this);
            }
        }
    }

}
