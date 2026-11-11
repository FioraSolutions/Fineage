using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject/Material side of ParticleGroup. Simulation stays Burst-only.
/// </summary>
public sealed partial class ParticleGroupRendererService
{
    const string OwnerWorldPosShaderProperty = "_OwnerWorldPos";
    static readonly int StartTimeShaderId = Shader.PropertyToID("_StartTime");
    static readonly int SeedShaderId = Shader.PropertyToID("_Seed");
    static readonly int DebugMeshPreviewShaderId = Shader.PropertyToID("_DebugMeshPreview");

    readonly EffectPart _host;
    readonly Transform _transform;
    Renderer[] _particles;
    MaterialPropertyBlock _propertyBlock;
    ParticleGroupSpawnSpin _spawnSpin;
    bool _spawnSpinResolved;
    Matrix4x4[] _objectToWorldMatrices;
    TransformOrigin[] _transformOrigins;

    struct TransformOrigin
    {
        public Transform parent;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }

}
