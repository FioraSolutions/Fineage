using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MenuWindow : L2Window
{
    void DockToBottomRight()
    {
        if (_windowEle == null)
            return;

        _windowEle.style.left = StyleKeyword.Null;
        _windowEle.style.top = StyleKeyword.Null;
        _windowEle.style.right = 0;
        _windowEle.style.bottom = 0;
        _windowEle.style.position = Position.Absolute;
    }

    void WireMenuButton(string buttonName, Action onClick)
    {
        Button btn = _windowEle.Q<Button>(buttonName);
        if (btn == null)
            return;
        btn.AddManipulator(new ButtonClickSoundManipulator(btn));
        btn.RegisterCallback<ClickEvent>(_ => onClick());
    }

    void ApplyInterludeMenuTextures()
    {
        if (_windowEle == null)
            return;

        PaintNamed("MenuWndBackTexLeft", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back1");
        PaintNamed("MenuWndBackTexMiddle", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back2");
        PaintNamed("MenuWndBackTexRight", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back3");

        PaintButtonStates(
            "CharacterButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1_Down");
        PaintButtonStates(
            "InventoryButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2_Down");
        PaintButtonStates(
            "MapButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3_Down");
        PaintButtonStates(
            "SystemMenuButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4_Down");
    }
}
