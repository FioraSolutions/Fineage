using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class PacketApplyLog
{
    public static void Time(INetworkModel model, Action handle)
    {
        Time(model != null ? model.GetType().Name : "packet", handle);
    }

    public static void Time(string name, Action handle)
    {
        long start = Stopwatch.GetTimestamp();
        _depth++;
        try
        {
            handle();
        }
        finally
        {
            _depth--;
            if (_depth == 0)
            {
                double elapsed = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
                Stat stat;
                Stats.TryGetValue(name, out stat);
                stat.Count++;
                stat.Ms += elapsed;
                Stats[name] = stat;
                _packets++;
                _ms += elapsed;
            }
        }
    }
}
