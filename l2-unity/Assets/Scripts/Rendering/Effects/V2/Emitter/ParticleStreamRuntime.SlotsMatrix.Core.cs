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
}
