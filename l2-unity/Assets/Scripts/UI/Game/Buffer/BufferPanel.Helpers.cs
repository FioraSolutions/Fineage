using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

public partial class BufferPanel : L2Window
{
    protected override void LoadAssets()
    {
        _barSlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotBuffer");
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Buffer/SkillTray");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        _content = GetElementById("content");
        
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        CreateItemsPanel1();

        yield return new WaitForEndOfFrame();

        _content.style.opacity = 0;
        MovePanelToChatPosition(_defaultLeftOffset);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        _dictElement?.Clear();
        _dictElement = null;
        _bLink?.OnDisable();
    }
}
