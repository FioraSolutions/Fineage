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
}
