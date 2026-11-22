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
        _windowEle.AddToClassList("drag-area");
        InterludeWindowLayout.Apply(_windowEle, "AbnormalStatusWnd");

        VisualElement rail = _windowEle.Q<VisualElement>("InterludeHudVerticalRail");
        if (rail != null)
            rail.RemoveFromHierarchy();

        VisualElement slots = GetElementById("anchor");
        VisualElement icon = GetElementById("StatusIcon");
        if (slots != null && icon != null)
        {
            icon.Add(slots);
            icon.style.width = StyleKeyword.Auto;
            icon.style.height = StyleKeyword.Auto;
        }

        float layoutWidth;
        float layoutHeight;
        if (InterludeWindowLayout.TryWindowSize("AbnormalStatusWnd", out layoutWidth, out layoutHeight))
        {
            _windowEle.style.minWidth = layoutWidth;
            _windowEle.style.minHeight = layoutHeight;
        }

        _windowEle.style.width = StyleKeyword.Auto;
        _windowEle.style.height = StyleKeyword.Auto;

        _content = GetElementById("content");
        if (_content == null)
            _content = _windowEle;
        DragManipulator drag = new DragManipulator(_windowEle, _windowEle);
        _windowEle.AddManipulator(drag);

        CreateItemsPanel1();

        yield return new WaitForEndOfFrame();

        _content.style.opacity = 0;
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
