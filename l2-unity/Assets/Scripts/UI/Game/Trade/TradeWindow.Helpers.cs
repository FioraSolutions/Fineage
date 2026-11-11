using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class TradeWindow : L2PopupWindow
{
    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TradeWindow");
        _inventorySlotTemplate = LoadAsset(L2InventoryUiAssets.Slot);
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        _playerNameLabel = (Label)GetElementById("PlayerNameLabel");
        _okButton = (Button)GetElementById("OKButton");
        _cancelButton = (Button)GetElementById("CancelButton");

        // Инициализация слотов обмена
        InitializeExchangeSlots(GetElementById("PlayerOfferContent"), _exchangeSlots);
        InitializeExchangeSlots(GetElementById("OtherPlayerOfferContent"), _partnerExchangeSlots);

        // Подписываемся на события кнопок
        _okButton.AddManipulator(new ButtonClickSoundManipulator(_okButton));
        _cancelButton.AddManipulator(new ButtonClickSoundManipulator(_cancelButton));

        _okButton.RegisterCallback<ClickEvent>(OnOkClicked);
        _cancelButton.RegisterCallback<ClickEvent>(OnCancelClicked);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        OnCenterScreen(root);
    }
}
