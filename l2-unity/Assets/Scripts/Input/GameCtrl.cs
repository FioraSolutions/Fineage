using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Interlude force-use / force-attack: Left or Right Control held.
/// </summary>
public static class GameCtrl
{
    public static bool IsHeld
    {
        get
        {
            Keyboard kb = Keyboard.current;
            if (kb == null)
                return false;
            return kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed;
        }
    }

    public static int AsInt => IsHeld ? 1 : 0;
}
