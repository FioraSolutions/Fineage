using System;
using System.Collections.Generic;

public static partial class NetAutoShot
{
    public static bool IsEnabled(int itemId)
    {
        lock (Gate)
            return Enabled.Contains(itemId);
    }

    public static void Reset()
    {
        lock (Gate)
            Enabled.Clear();
    }
}
