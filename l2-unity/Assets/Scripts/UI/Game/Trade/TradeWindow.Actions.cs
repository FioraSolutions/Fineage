

using System;
using System.Linq;
using static L2Slot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeWindow
{
    private void OnOkClicked(ClickEvent evt)
    {
        SendOkResponse();
    }

    private void OnCancelClicked(ClickEvent evt)
    {
        SendCancelResponse();
        HideWindow();
    }

    private void SendOkResponse()
    {
        // Отправляем пакет подтверждения обмена
        ZoneCommands.Send(new TradeDoneCommand(1));

        OnExchangeResponse?.Invoke(true);
    }

    private void SendCancelResponse()
    {
        // Отправляем пакет отмены обмена
        ZoneCommands.Send(new TradeDoneCommand(0));

        OnExchangeResponse?.Invoke(false);
    }

    public override void HideWindow()
    {
        base.HideWindow();

        // Очищаем слоты при закрытии
        ClearAllExchangeSlots();
    }

}
