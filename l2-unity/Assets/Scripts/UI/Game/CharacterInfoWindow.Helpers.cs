using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow : L2PopupWindow
{
    void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(characterInfo: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/CharacterInfoWindow");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");
        dragArea.AddManipulator(new DragManipulator(dragArea, _windowEle));

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEventByName("Close-Window-Button");
        RegisterClickWindowEvent(_windowEle, dragArea);

        ApplyInterludeTextures();
    }
}
