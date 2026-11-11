using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2StaticMeshSunVisibility
{
    public static float GetSlotCenterHours(int slot)
    {
        float slotHours = HoursPerDay / SlotCount;
        return slot * slotHours + slotHours * 0.5f;
    }

    public static Vector3 GetIncomingDirection(int slot)
    {
        L2CelestialLut.EnsureLoaded();
        Vector3 incoming = -L2CelestialLut.SampleAt(GetSlotCenterHours(slot)).sunDirUnity;
        if (incoming.y > 0f)
            incoming.y = 0.0001f;
        return incoming.sqrMagnitude > 1e-8f ? incoming.normalized : Vector3.down;
    }
}
