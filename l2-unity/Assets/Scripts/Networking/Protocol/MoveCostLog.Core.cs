using System;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MoveCostLog
{
    public static void Wire(World world)
    {
        if (world != null)
            NetworkingPacketServices.Bind(world: world);
    }

    public static void NoteCharInfo()
    {
        if (!Enabled)
            return;
        _fChar++;
    }

    public static void NoteStep(int moving, int ccMoves, double ms)
    {
        if (!Enabled)
            return;
        _moving = moving;
        _fCc += ccMoves;
        _fStepMs += ms;
    }
}
