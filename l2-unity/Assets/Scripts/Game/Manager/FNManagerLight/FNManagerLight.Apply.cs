using System;

public partial class FNManagerLight
{
    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < _slots.Count; i++)
        {
            FlashSlot slot = _slots[i];
            if (!slot.Active)
            {
                continue;
            }

            slot.Age += dt;
            float t = slot.Age / slot.Duration;
            if (t >= 1f)
            {
                if (_debugLog)
                {
                    // Debug.Log($"{LOG_TAG} RELEASE #{slot.SpawnId} after {slot.Age:F3}s");
                }

                ReleaseSlot(slot);
                continue;
            }

            float envelope = 1f - t;
            envelope *= envelope;
            slot.Light.intensity = slot.PeakIntensity * envelope;
        }
    }

    private void EnsurePool()
    {
        if (_poolRoot == null)
        {
            GameObject root = new GameObject("FNPawnLightPool");
            root.transform.SetParent(transform, false);
            _poolRoot = root.transform;
        }

        while (_slots.Count < _poolSize)
        {
            _slots.Add(CreateSlot(_slots.Count));
        }
    }

    private FlashSlot CreateSlot(int index)
    {
        GameObject go = new GameObject($"FNPawnLight_{index}");
        go.transform.SetParent(_poolRoot, false);
        Light light = go.AddComponent<Light>();
        light.type = LightType.Spot;
        light.spotAngle = _spotAngle;
        light.innerSpotAngle = _innerSpotAngle;
        light.shadows = LightShadows.None;
        light.renderMode = LightRenderMode.ForcePixel;
        light.enabled = true;
        go.SetActive(false);

        return new FlashSlot
        {
            Go = go,
            Light = light,
            Active = false
        };
    }

    private FlashSlot AcquireSlot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots[i].Active)
            {
                return _slots[i];
            }
        }

        FlashSlot oldest = _slots[0];
        float maxAge = oldest.Age;
        for (int i = 1; i < _slots.Count; i++)
        {
            if (_slots[i].Age > maxAge)
            {
                maxAge = _slots[i].Age;
                oldest = _slots[i];
            }
        }

        ReleaseSlot(oldest);
        return oldest;
    }

    private static void ReleaseSlot(FlashSlot slot)
    {
        slot.Active = false;
        slot.Age = 0f;
        if (slot.Light != null)
        {
            slot.Light.intensity = 0f;
        }

        if (slot.Go != null)
        {
            slot.Go.SetActive(false);
        }
    }
}
