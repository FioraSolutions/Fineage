using UnityEngine;
using UnityEngine.InputSystem;

public static partial class PollInput
{
    public static bool KeyHeld(Key key)
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[key].isPressed;
    }
}
