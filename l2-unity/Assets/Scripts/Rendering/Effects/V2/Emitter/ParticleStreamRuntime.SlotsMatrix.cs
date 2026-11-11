using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    void EnsureCoordinateMatrices(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (_spawnObjectToWorld == null || _spawnObjectToWorld.Length != count)
        {
            _spawnObjectToWorld = new Matrix4x4[count];
            _hasSpawnObjectToWorld = new bool[count];
            return;
        }

        for (int i = 0; i < _hasSpawnObjectToWorld.Length; i++)
        {
            _hasSpawnObjectToWorld[i] = false;
        }
    }

    void CaptureSpawnObjectToWorld(int slot)
    {
        if (!L2ParticleCoordinateSystemUtil.FreezesSpawnMatrix(_authoring.coordinateSystem))
        {
            return;
        }

        Matrix4x4[] current = _slots.ResolveObjectToWorldMatrices();
        if (current == null || slot < 0 || slot >= current.Length)
        {
            return;
        }

        Matrix4x4 matrix = current[slot];
        if (_authoring.coordinateSystem == L2ParticleCoordinateSystem.Independent)
        {
            matrix = Matrix4x4.TRS(
                matrix.GetColumn(3),
                Quaternion.identity,
                new Vector3(
                    matrix.GetColumn(0).magnitude,
                    matrix.GetColumn(1).magnitude,
                    matrix.GetColumn(2).magnitude));
        }

        _spawnObjectToWorld[slot] = matrix;
        _hasSpawnObjectToWorld[slot] = true;
    }

    Matrix4x4[] ResolveDrawMatrices(Matrix4x4[] current)
    {
        if (!L2ParticleCoordinateSystemUtil.FreezesSpawnMatrix(_authoring.coordinateSystem) ||
            current == null ||
            _spawnObjectToWorld == null)
        {
            return current;
        }

        int count = Mathf.Min(current.Length, _spawnObjectToWorld.Length);
        for (int i = 0; i < count; i++)
        {
            if (_hasSpawnObjectToWorld[i])
            {
                current[i] = _spawnObjectToWorld[i];
            }
        }

        return current;
    }

    void ApplyLifeStretchToRenderer(Renderer renderer)
    {
        if (!ShouldStretchLife())
        {
            return;
        }

        L2FxAdjustParticleLife.ApplyToRenderer(
            renderer,
            _authoring.authoredParticleLife,
            _authoring.targetParticleLife);
    }

    void ApplyLifeStretchToGpuMaterials()
    {
        if (!ShouldStretchLife() || !_batch.Bound)
        {
            return;
        }

        Material[] materials = _simulation.GpuMaterials;
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            L2FxAdjustParticleLife.ApplyToMaterial(
                materials[i],
                _authoring.authoredParticleLife,
                _authoring.targetParticleLife);
        }
    }

    bool ShouldStretchLife()
    {
        return _authoring.stretchParticleLifeToWindow &&
               Mathf.Abs(_authoring.targetParticleLife - _authoring.authoredParticleLife) > 1e-4f;
    }

    Vector4 ResolveSpawnLocationAddUe(float spawnTime)
    {
        if (_host == null)
        {
            return Vector4.zero;
        }

        IParticleSpawnLocationAddProvider provider =
            _host.GetComponentInParent<IParticleSpawnLocationAddProvider>();
        if (provider != null &&
            provider.TryGetSpawnLocationAddUe(_host, spawnTime, out Vector4 addUe))
        {
            return addUe;
        }

        return Vector4.zero;
    }
}
