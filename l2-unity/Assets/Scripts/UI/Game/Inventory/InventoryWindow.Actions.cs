using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class InventoryWindow : L2PopupWindow
{
    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        _expanded = false;

        ApplyInterludeInventoryTextures();

        var dragArea = GetElementByClass("drag-area");
        if (dragArea != null)
        {
            DragManipulator drag = new DragManipulator(dragArea, _windowEle);
            dragArea.AddManipulator(drag);
        }

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        // Interlude InventoryWnd has no expand control; Classic DF chrome still may.
        _expandButton = (Button)GetElementByClass("expand-btn");
        if (_expandButton != null)
        {
            _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
            _expandButton.RegisterCallback<MouseDownEvent>(OnExpandButtonPressed, TrickleDown.TrickleDown);
        }

        Button adenaDistribution = (Button)GetElementById("AdenaDistribBtn");
        if (adenaDistribution != null)
        {
            adenaDistribution.AddManipulator(new ButtonClickSoundManipulator(adenaDistribution));
            adenaDistribution.AddManipulator(new TooltipManipulator(adenaDistribution, "Adena distribution"));
        }

        Button compoundBtn = (Button)GetElementById("CompoundBtn");
        if (compoundBtn != null)
        {
            compoundBtn.AddManipulator(new ButtonClickSoundManipulator(compoundBtn));
            compoundBtn.AddManipulator(new TooltipManipulator(compoundBtn, "Compound"));
        }

        Button trashBtn = (Button)GetElementById("TrashBtn");
        if (trashBtn != null)
        {
            trashBtn.AddManipulator(new ButtonClickSoundManipulator(trashBtn));
            trashBtn.AddManipulator(new TooltipManipulator(trashBtn, "Trash"));
            _ = new L2Slot(trashBtn, 100, SlotType.Trash);
        }

        _inventoryCountLabel = GetLabelById("InventoryCount");

        _weiBar= GetElementById("WeightBg");
        _weiBarBg = GetElementById("WeightGauge");
        PaintWeightFillParts();

        _adenaCountLabel = GetLabelById("AdenaCount");
        _weightLabel = GetLabelById("CurrentWeight");
    }

    private void OnExpandButtonPressed(MouseDownEvent evt)
    {
        if (!_expanded)
        {
            if (!_windowEle.ClassListContains("expanded"))
            {
                _windowEle.AddToClassList("expanded");
            }
            if (!_expandButton.ClassListContains("expanded"))
            {
                _expandButton.AddToClassList("expanded");
            }
            _expanded = true;

            //UpdateItemList(_playerItems);
        }
        else
        {
            if (_windowEle.ClassListContains("expanded"))
            {
                _windowEle.RemoveFromClassList("expanded");
            }
            if (_expandButton.ClassListContains("expanded"))
            {
                _expandButton.RemoveFromClassList("expanded");
            }
            _expanded = false;

           //UpdateItemList(_playerItems);
        }

        evt.PreventDefault();
    }

    private void CreateMinimizedWindow()
    {

        Button minimizeWindowButton = (Button)GetElementByClass("minimize-btn");
        if (minimizeWindowButton != null)
        {
            minimizeWindowButton.AddManipulator(new ButtonClickSoundManipulator(minimizeWindowButton));
            minimizeWindowButton.RegisterCallback<MouseUpEvent>(OnMinimizeInventoryClick, TrickleDown.TrickleDown);
        }

        // Minized inventory button
        _minimizedInventoryBtn = _minimizedTemplate.Instantiate()[0];
        _minimizedInventoryMouseOverManipulator = new MouseOverDetectionManipulator(_minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryMouseOverManipulator);
        _minimizedInventoryMouseOverManipulator.Disable();

        _minimizedInventoryBtn.style.left = new StyleLength(Screen.width / 2);
        _minimizedInventoryBtn.style.top = new StyleLength(Screen.height / 2);

        _minimizedInventoryBtn.RegisterCallback<ClickEvent>(OnMinimizedInventoryClick, TrickleDown.TrickleDown);
        _minimizedInventoryDragManipulator = new DragManipulator(_minimizedInventoryBtn, _minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryDragManipulator);

        _root.Add(_minimizedInventoryBtn);
        _minimizedInventoryBtn.style.display = DisplayStyle.None;
    }

    private void OnMinimizeInventoryClick(MouseUpEvent evt)
    {
        _minimizedInventoryBtn.style.display = DisplayStyle.Flex;
        _minimizedInventoryMouseOverManipulator.Enable();
        HideWindow();
    }

    private void OnMinimizedInventoryClick(ClickEvent evt)
    {
        if (!_minimizedInventoryDragManipulator.dragged)
        {
            AudioPacketServices.Audio.PlayUISound("click_01");
            _minimizedInventoryBtn.style.display = DisplayStyle.None;
            _minimizedInventoryMouseOverManipulator.Disable();
            ShowWindow();
        }
    }
}
