using System;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MoveCostLog
{
    public static void EndFrame()
    {
        if (!Enabled)
            return;
        _wFrames++;
        _wChar += _fChar;
        _wCc += _fCc;
        _wStepMs += _fStepMs;
        if (_fChar > _maxChar)
            _maxChar = _fChar;
        if (_fCc > _maxCc)
            _maxCc = _fCc;
        if (_fChar > 0)
            _wTicksChar++;

        _fChar = 0;
        _fCc = 0;
        _fStepMs = 0;

        if (_nextFlush < 0f)
            _nextFlush = Time.unscaledTime + FlushSec;
        if (Time.unscaledTime < _nextFlush)
            return;

        Dump();
        _nextFlush = Time.unscaledTime + FlushSec;
        _wChar = 0;
        _wCc = 0;
        _wFrames = 0;
        _wTicksChar = 0;
        _maxChar = 0;
        _maxCc = 0;
        _wStepMs = 0;
    }

    static int WorldCount()
    {
        World world = NetworkingPacketServices.GameWorld;
        if (world == null)
            return 0;
        int n = 0;
        world.ForEachEntity(_ => n++);
        return n;
    }

    static void Dump()
    {
        Line.Length = 0;
        Line.Append("unity ");
        Line.Append("frames ").Append(_wFrames);
        Line.Append(" world ").Append(WorldCount());
        Line.Append(" charInfo ").Append(_wChar);
        Line.Append(" moving ").Append(_moving);
        Line.Append(" ccMoves ").Append(_wCc);
        Line.Append(" stepMs ").Append(_wStepMs.ToString("0.000"));
        Line.Append(" maxChar ").Append(_maxChar);
        Line.Append(" maxCc ").Append(_maxCc);
        Line.Append(" ticksChar ").Append(_wTicksChar);
        string text = Line.ToString();
        try
        {
            string dir = Path.GetDirectoryName(LogPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(LogPath, text + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("MoveCostLog file: " + ex.Message);
        }
        Debug.Log(text);
    }
}
