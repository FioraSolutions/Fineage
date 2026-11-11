using UnityEngine;
using UnityEngine.InputSystem;

public static partial class PollInput
{
    /// <summary>Screen-space mouse position. (0,0) when no mouse is present.</summary>
    public static Vector2 MousePosition
    {
        get
        {
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        }
    }

    /// <summary>Raw mouse delta in pixels for this frame.</summary>
    public static Vector2 MouseDelta
    {
        get
        {
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
        }
    }

    /// <summary>
    /// Approximate legacy <c>Input.GetAxis("Mouse X/Y")</c> scale so existing sense multipliers stay usable.
    /// </summary>
    public static Vector2 MouseAxis => MouseDelta * 0.05f;

    /// <summary>Legacy-style scroll notches (~±1 per wheel step).</summary>
    public static float ScrollAxis
    {
        get
        {
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.scroll.ReadValue().y / 120f : 0f;
        }
    }

    /// <summary>Pixel scroll delta for this frame (Input System native units).</summary>
    public static Vector2 ScrollDelta
    {
        get
        {
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.scroll.ReadValue() : Vector2.zero;
        }
    }

    public static bool LeftMouseDown => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    public static bool RightMouseDown => Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
    public static bool MiddleMouseDown => Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame;
    public static bool LeftMouseHeld => Mouse.current != null && Mouse.current.leftButton.isPressed;
    public static bool RightMouseHeld => Mouse.current != null && Mouse.current.rightButton.isPressed;
    public static bool MiddleMouseHeld => Mouse.current != null && Mouse.current.middleButton.isPressed;



    public static bool EscapeDown => KeyDown(Key.Escape);
    public static bool EnterDown => KeyDown(Key.Enter) || KeyDown(Key.NumpadEnter);
    public static bool AnyKeyOrMouseDown =>
        (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        || LeftMouseDown
        || RightMouseDown;

    /// <summary>WASD / arrow equivalent of the old Horizontal/Vertical axes.</summary>
    public static Vector2 MoveAxes
    {
        get
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return Vector2.zero;

            float x = 0f;
            float y = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                y += 1f;
            return new Vector2(x, y);
        }
    }
}
