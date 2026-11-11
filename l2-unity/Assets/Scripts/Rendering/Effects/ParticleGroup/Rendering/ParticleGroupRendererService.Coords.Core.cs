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
}
