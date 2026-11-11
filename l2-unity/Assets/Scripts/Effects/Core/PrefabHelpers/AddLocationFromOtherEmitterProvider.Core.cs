using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class AddLocationFromOtherEmitterProvider : MonoBehaviour, IParticleSpawnLocationAddProvider
{
    [SerializeField] private List<Binding> _bindings = new();

    public bool ContainsTail(ParticleGroupV2 group)
    {
        if (group == null || _bindings == null)
        {
            return false;
        }

        for (int i = 0; i < _bindings.Count; i++)
        {
            Binding binding = _bindings[i];
            if (binding != null && binding.tailEmitter == group)
            {
                return true;
            }
        }

        return false;
    }

    private void OnEnable()
    {
        _effectStartedAt = Time.time;
    }

    public bool TryGetSpawnLocationAddUe(EffectPart tailEmitter, float spawnTime, out Vector4 addUe)
    {
        addUe = Vector4.zero;
        if (tailEmitter == null || _bindings == null || _bindings.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < _bindings.Count; i++)
        {
            Binding binding = _bindings[i];
            if (binding == null || binding.tailEmitter != tailEmitter || binding.sourceEmitter == null)
            {
                continue;
            }

            if (!TryResolveSourceLocationUe(binding, spawnTime, out Vector3 locationUe))
            {
                return false;
            }

            Transform sourceTransform = ResolveSourceTransform(binding.sourceEmitter);
            Vector3 locationLocal = new Vector3(
                locationUe.x,
                locationUe.z,
                locationUe.y) * UuToUnityMeters;
            Vector3 locationWorld = sourceTransform.TransformPoint(locationLocal);

            // w=1 marks an absolute world-space position. The shader converts
            // it back through the current tail matrix, so later root rotation
            // cannot drag an already spawned PTCS_Spray particle with the ball.
            addUe = new Vector4(
                locationWorld.x,
                locationWorld.y,
                locationWorld.z,
                1f);
            return true;
        }

        return false;
    }

    bool TryResolveSourceLocationUe(Binding binding, float spawnTime, out Vector3 locationUe)
    {
        locationUe = Vector3.zero;
        Material sourceMat = ResolveSourceMaterial(binding.sourceEmitter);
        if (sourceMat == null)
        {
            return false;
        }

        locationUe = ReadRangeMid(sourceMat, StartLocationRangeXUcID)
            + ReadRangeMid(sourceMat, StartLocationRangeYUcID)
            + ReadRangeMid(sourceMat, StartLocationRangeZUcID);
        if (sourceMat.HasProperty(StartLocationOffsetUcID))
        {
            Vector4 offset = sourceMat.GetVector(StartLocationOffsetUcID);
            locationUe += new Vector3(offset.x, offset.y, offset.z);
        }

        if (Mathf.Abs(binding.revolutionsPerSecondZ) > 1e-6f)
        {
            float age = Mathf.Max(0f, spawnTime - _effectStartedAt);
            locationUe = RotateAroundUcZ(locationUe, age * binding.revolutionsPerSecondZ * Mathf.PI * 2f);
        }

        return locationUe.sqrMagnitude > 1e-6f;
    }
}
