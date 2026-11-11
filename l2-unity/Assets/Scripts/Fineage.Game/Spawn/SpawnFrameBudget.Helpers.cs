using System.Collections.Generic;
using UnityEngine;

public static partial class SpawnFrameBudget
{
    public static void ClearCancel(int objectId)
    {
        Cancelled.Remove(objectId);
    }

    public static void Cancel(int objectId)
    {
        Cancelled.Add(objectId);
    }

    public static bool IsCancelled(int objectId)
    {
        return Cancelled.Contains(objectId);
    }

    static void Sync()
    {
        if (_frame != Time.frameCount)
            BeginFrame();
    }
}
