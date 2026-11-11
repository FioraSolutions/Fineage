using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
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
