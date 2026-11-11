using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicSkillUseTrace
{
    struct Row
    {
        public int Level;
        public string Name;
        public double Ms;
    }

    const double DumpMs = 8.0;
    static string LogPath => L2DiagPaths.DiagFile("magicSkillUse.txt");

    static readonly List<Row> Rows = new List<Row>(24);
    static bool _active;
    static int _suppress;
    static int _level;
    static int _skillId;
    static long _started;




    public struct Span : IDisposable
    {
        readonly string _name;
        readonly long _started;
        readonly int _level;
        readonly bool _on;

        public Span(string name)
        {
            _on = _active;
            _name = name;
            _level = MagicSkillUseTrace._level;
            _started = Stopwatch.GetTimestamp();
            if (_on)
                MagicSkillUseTrace._level++;
        }

        public void Dispose()
        {
            if (!_on)
                return;

            MagicSkillUseTrace._level--;
            Rows.Add(new Row
            {
                Level = _level,
                Name = _name,
                Ms = Elapsed(_started)
            });
        }
    }


}
