using System;
using UnityEngine;
using System.Collections.Generic;

public partial class FNManagerLight
{
    public void SpawnHitFlash(Vector3 worldPosition, Vector3 aimDirection)
    {
        SpawnHitFlash(worldPosition, aimDirection, null);
    }

    public void SpawnHitFlash(Vector3 worldPosition, Vector3 aimDirection, LightEffectSetting settings)
    {
        EnsurePool();
        FlashSlot slot = AcquireSlot();
        if (slot == null || slot.Light == null)
        {
            // Debug.LogError($"{LOG_TAG} SpawnHitFlash FAILED — no pool slot / Light null");
            return;
        }

        Vector3 dir = aimDirection;
        if (dir.sqrMagnitude > 0.0001f)
        {
            dir.Normalize();
            slot.Go.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        else
        {
            dir = Vector3.forward;
            slot.Go.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        Color color = settings != null ? settings.color : _color;
        float intensity = settings != null ? settings.intensity : _intensity;
        float duration = settings != null ? settings.durationSeconds : _durationSeconds;
        float range = settings != null ? settings.rangeMeters : _rangeMeters;
        float spotAngle = settings != null ? settings.spotAngle : _spotAngle;
        float innerSpot = settings != null ? settings.innerSpotAngle : _innerSpotAngle;

        _spawnCounter += 1;
        slot.SpawnId = _spawnCounter;
        slot.Go.transform.position = worldPosition;
        slot.Age = 0f;
        slot.Duration = Mathf.Max(0.05f, duration);
        slot.PeakIntensity = Mathf.Max(0.01f, intensity);
        slot.Light.type = LightType.Spot;
        slot.Light.spotAngle = Mathf.Clamp(spotAngle, 1f, 179f);
        slot.Light.innerSpotAngle = Mathf.Clamp(innerSpot, 0f, slot.Light.spotAngle);
        slot.Light.color = color;
        slot.Light.range = Mathf.Max(0.05f, range);
        slot.Light.intensity = slot.PeakIntensity;
        slot.Light.enabled = true;
        slot.Active = true;
        slot.Go.SetActive(true);

        if (_debugLog)
        {
            string settingsName = settings != null ? settings.name : "<fallback>";
            // Debug.Log(
                // $"{LOG_TAG} SPAWN #{slot.SpawnId} settings={settingsName} type=Spot pos={worldPosition} dir={dir} " +
                // $"intensity={slot.Light.intensity:F2} range={slot.Light.range:F3}m " +
                // $"spotAngle={slot.Light.spotAngle:F0} duration={slot.Duration:F3}s " +
                // $"activeSlots={CountActive()}/{_slots.Count}");
        }
    }

    private int CountActive()
    {
        int n = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Active)
            {
                n++;
            }
        }

        return n;
    }
}
