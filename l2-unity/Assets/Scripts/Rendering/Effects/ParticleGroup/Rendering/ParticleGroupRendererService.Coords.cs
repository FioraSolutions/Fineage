using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public void RestoreCoordinateSystemSlots()
    {
        if (_particles == null || _transformOrigins == null)
        {
            return;
        }

        int count = Mathf.Min(_particles.Length, _transformOrigins.Length);
        for (int i = 0; i < count; i++)
        {
            RestoreCoordinateSystemSlot(i);
        }
    }

    void EnsureTransformOrigins()
    {
        int count = Count;
        if (_transformOrigins != null && _transformOrigins.Length == count)
        {
            return;
        }

        _transformOrigins = new TransformOrigin[count];
        for (int i = 0; i < count; i++)
        {
            Transform particleTransform = _particles[i] != null ? _particles[i].transform : null;
            if (particleTransform == null)
            {
                continue;
            }

            _transformOrigins[i] = new TransformOrigin
            {
                parent = particleTransform.parent,
                localPosition = particleTransform.localPosition,
                localRotation = particleTransform.localRotation,
                localScale = particleTransform.localScale
            };
        }
    }

    void RestoreCoordinateSystemSlot(int slot)
    {
        if (_particles == null ||
            _transformOrigins == null ||
            slot < 0 ||
            slot >= _particles.Length ||
            slot >= _transformOrigins.Length ||
            _particles[slot] == null)
        {
            return;
        }

        TransformOrigin origin = _transformOrigins[slot];
        Transform particleTransform = _particles[slot].transform;
        particleTransform.SetParent(origin.parent, false);
        particleTransform.localPosition = origin.localPosition;
        particleTransform.localRotation = origin.localRotation;
        particleTransform.localScale = origin.localScale;
    }

    public Matrix4x4[] ResolveObjectToWorldMatrices()
    {
        int count = Count;
        if (_objectToWorldMatrices == null || _objectToWorldMatrices.Length != count)
            _objectToWorldMatrices = new Matrix4x4[count];

        Matrix4x4 fallback = _transform.localToWorldMatrix;
        for (int i = 0; i < count; i++)
        {
            Renderer renderer = _particles[i];
            _objectToWorldMatrices[i] = renderer != null
                ? renderer.transform.localToWorldMatrix
                : fallback;
        }

        return _objectToWorldMatrices;
    }

    public void UpdateWorldPositions(Renderer renderer)
    {
        if (renderer == null)
            return;

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
            SetWorldPositions(materials[i]);
    }

    void ApplySpawnTiming(Renderer renderer, float shaderStartTime, float seed)
    {
        if (renderer == null)
            return;

        _propertyBlock ??= new MaterialPropertyBlock();
        renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(StartTimeShaderId, shaderStartTime);
        _propertyBlock.SetFloat(SeedShaderId, seed);
        _propertyBlock.SetFloat(DebugMeshPreviewShaderId, 0f);
        renderer.SetPropertyBlock(_propertyBlock);
    }

    void ApplySpawnSpin(Renderer renderer, float seed)
    {
        if (renderer == null)
            return;

        if (!_spawnSpinResolved)
        {
            _spawnSpin = _transform.GetComponent<ParticleGroupSpawnSpin>();
            _spawnSpinResolved = true;
        }

        _spawnSpin?.Apply(renderer, seed);
    }

    void SetWorldPositions(Material material)
    {
        if (material == null)
            return;

        if (material.HasProperty(OwnerWorldPosShaderProperty))
            material.SetVector(OwnerWorldPosShaderProperty, _host.ResolveOwnerWorldPosForShader(material));

        if (!material.HasProperty(L2MaterialPropertyCopier.L2FxTargetWorldPosId))
            return;

        bool hasTarget = _host.TryResolveShaderTargetWorldPos(out Vector3 targetWorldPos);
        material.SetVector(L2MaterialPropertyCopier.L2FxTargetWorldPosId, targetWorldPos);
        if (material.HasProperty(L2MaterialPropertyCopier.UseExternalTargetPositionId))
            material.SetFloat(L2MaterialPropertyCopier.UseExternalTargetPositionId, hasTarget ? 1f : 0f);
    }

    public Vector4 ResolveGpuOwnerWorldPos(Material[] gpuMaterials)
    {
        Material material = gpuMaterials != null && gpuMaterials.Length > 0 ? gpuMaterials[0] : null;
        return material != null ? (Vector4)_host.ResolveOwnerWorldPosForShader(material) : Vector4.zero;
    }
}
