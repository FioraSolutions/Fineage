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
}
