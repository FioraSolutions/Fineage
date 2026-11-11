using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2GameUI
{
    public void EnableMouse()
    {
        if (!_mouseEnabled)
        {
            _mouseEnabled = true;
#if UNITY_STANDALONE_WIN
            NativeFunctions.SetCursorPos(_lastMousePosition.X, _lastMousePosition.Y);
#endif
        }
    }

    public void DisableMouse()
    {
        if (_mouseEnabled)
        {
#if UNITY_STANDALONE_WIN
            NativeFunctions.GetCursorPos(out _lastMousePosition);
#endif
            _mouseEnabled = false;
        }
    }

    public void CheckUIInputs()
    {
        InputManager input = ControllerPacketServices.Input;
        if (input == null)
            return;

        if (input.OpenCharacerStatus)
        {
            if (HudCorePacketServices.CharacterInfo != null)
            {
                HudCorePacketServices.CharacterInfo.ToggleHideWindow();
            }
        }

        if (input.OpenInventory)
        {
            if (InventoryPacketServices.Window != null)
            {
                InventoryPacketServices.Window.ToggleHideWindowManual();
            }
        }

        if (input.OpenSystemMenu)
        {
            if (SystemUIPacketServices.Menu != null)
            {
                SystemUIPacketServices.Menu.ToggleHideWindow();
            }
        }

        if (input.OpenActions)
        {
            if (HudCorePacketServices.Action != null)
            {
                HudCorePacketServices.Action.ToggleHideWindow();
            }
        }

        if (input.OpenMap)
        {
            if (HudCorePacketServices.Map != null)
                HudCorePacketServices.Map.ToggleHideWindow();
        }

        if (input.CloseWindow)
        {
            if (MessagePacketServices.Chat != null && MessagePacketServices.Chat.ChatOpened)
            {
                MessagePacketServices.Chat.CloseChat(false);
                return;
            }

            if (_openedWindows != null && _openedWindows.Count > 0)
            {
                _openedWindows[_openedWindows.Count - 1].HideWindow();
            }
            else
            {
                SystemUIPacketServices.Menu.ToggleHideWindow();
            }
        }
    }
}
