using System.Runtime.InteropServices;
using UnityEngine;

public static class NativeFunctions
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeCoords
    {
        public int X;
        public int Y;
    }

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out NativeCoords lpPoint);
#else
    public static bool SetCursorPos(int x, int y)
    {
        return true;
    }

    public static bool GetCursorPos(out NativeCoords lpPoint)
    {
        Vector3 mp = Input.mousePosition;
        lpPoint = new NativeCoords
        {
            X = Mathf.RoundToInt(mp.x),
            Y = Mathf.RoundToInt(Screen.height - mp.y)
        };
        return true;
    }
#endif
}
