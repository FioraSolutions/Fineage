using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class PacketApplyLog
{
    struct Stat
    {
        public int Count;
        public double Ms;
    }

    const double DumpMs = 50.0;
    static string LogPath => L2DiagPaths.DiagFile("packetApply.txt");

    static readonly Dictionary<string, Stat> Stats = new Dictionary<string, Stat>(64);
    static readonly List<KeyValuePair<string, Stat>> SortBuffer = new List<KeyValuePair<string, Stat>>(64);
    static int _depth;
    static int _packets;
    static double _ms;





}
