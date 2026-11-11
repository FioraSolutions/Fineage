using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    [Serializable]
    public sealed class TrailBinding
    {
        public string name;
        public Transform tailRoot;
        public Transform velocitySource;
        public string velocitySourceName = "SpriteEmitter5";
        public Renderer[] targetRenderers;
        public bool autoCollectChildren = true;
        public bool followSourcePosition = true;
        public bool placeRenderersOnHistory = true;
        public float historySeconds = 0.28f;
        [Range(0f, 0.5f)]
        public float headLagPercent = 0.05f;
        public bool useCylinderSpread = true;
        public float cylinderRadiusHead = 0.008f;
        public float cylinderRadiusTail = 0.04f;
        [Range(0.5f, 3f)]
        public float cylinderRadiusPower = 1.6f;
        public bool scaleOverTrail = true;
        public float particleScaleHead = 0.45f;
        public float particleScaleTail = 1.2f;
        [Range(0.5f, 3f)]
        public float particleScalePower = 1.4f;
        [Range(0.3f, 1.5f)]
        public float alongTrailPower = 0.75f;
        public bool placeByParticleAge = true;
        public float trailTravelSeconds = 0.333f;
        public bool fadeAlphaOverTrail = true;
        [Range(0f, 1f)]
        public float trailFadeHead = 1f;
        [Range(0f, 1f)]
        public float trailFadeTail = 0f;
        [Range(0.5f, 3f)]
        public float trailFadePower = 1.25f;
        public bool onlyActiveRenderers = false;
        public bool convertWorldToLocal = true;
        public Transform localSpaceReference;
        public float velocityScale = 0.08f;
        public float rangeSpread = 0.2f;
        public bool invertTrailDirection = true;
        public float trailSign = 1f;

        [NonSerialized] public Vector3 lastPosition;
        [NonSerialized] public Vector3 smoothedVelocity;
        [NonSerialized] public bool hasLastPosition;
        [NonSerialized] public List<TrailSample> history;
        [NonSerialized] public Dictionary<Transform, Vector3> baseLocalScales;
    }

    public struct TrailSample
    {
        public float time;
        public float distance;
        public Vector3 position;
    }
}
