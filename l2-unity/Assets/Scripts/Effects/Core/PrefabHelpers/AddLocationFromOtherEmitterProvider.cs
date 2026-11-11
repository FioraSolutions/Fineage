using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class AddLocationFromOtherEmitterProvider : MonoBehaviour, IParticleSpawnLocationAddProvider
{
    private const float UuToUnityMeters = 1f / 52.5f;

    [Serializable]
    public sealed class Binding
    {
        public ParticleGroupV2 tailEmitter;
        public ParticleGroupV2 sourceEmitter;
        [Tooltip("UC RevolutionsPerSecondRange.Z on the source waterball.")]
        public float revolutionsPerSecondZ;
    }


    private static readonly int StartLocationRangeXUcID = Shader.PropertyToID("_StartLocationRangeXUc");
    private static readonly int StartLocationRangeYUcID = Shader.PropertyToID("_StartLocationRangeYUc");
    private static readonly int StartLocationRangeZUcID = Shader.PropertyToID("_StartLocationRangeZUc");
    private static readonly int StartLocationOffsetUcID = Shader.PropertyToID("_StartLocationOffsetUc");

    private float _effectStartedAt = -1f;







}
