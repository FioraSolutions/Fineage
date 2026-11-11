using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    int FindFreeSlot()
    {
        int count = _slots.Count;
        if (count <= 0 || _active == null)
        {
            return -1;
        }

        for (int i = 0; i < count; i++)
        {
            int slot = (_cursor + i) % count;
            if (!IsSlotActive(slot))
            {
                return slot;
            }
        }

        return -1;
    }

    bool HasAnyActiveSlot()
    {
        int count = _slots.Count;
        if (count <= 0 || _active == null)
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            if (IsSlotActive(i))
            {
                return true;
            }
        }

        return false;
    }

    int CountInactiveSlots()
    {
        int count = _slots.Count;
        if (count <= 0)
        {
            return 0;
        }

        int inactive = 0;
        for (int i = 0; i < count; i++)
        {
            if (!IsSlotActive(i))
            {
                inactive++;
            }
        }

        return inactive;
    }

    bool IsSlotActive(int slot)
    {
        bool simActive = _simulation.HasLifetimeBuffers && _simulation.IsActive(slot);
        return simActive || (_active != null && slot < _active.Length && _active[slot]);
    }

    void KillAll()
    {
        _simulation.ClearAllActive(_active);
        _slots.HideAll();
    }

    void EnsureSlots(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (_active != null && _active.Length == count)
        {
            for (int i = 0; i < count; i++)
            {
                _active[i] = false;
                _spawnTimes[i] = 0f;
            }

            return;
        }

        _active = new bool[count];
        _spawnTimes = new float[count];
    }
}
