using System;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MoveCostLog
{

    static string LogPath => L2DiagPaths.DiagFile("moveTick.txt");
    const float FlushSec = 1f;

    static int _fChar;
    static int _fCc;
    static double _fStepMs;

    static int _wChar;
    static int _wCc;
    static int _wFrames;
    static int _wTicksChar;
    static int _maxChar;
    static int _maxCc;
    static double _wStepMs;
    static int _moving;
    static float _nextFlush = -1f;
    static readonly StringBuilder Line = new StringBuilder(384);

    public static bool Enabled { get; set; } = false;





}
