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
}
