using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeRequestWindow : L2PopupWindow
{
    private void OnTimeout()
    {
        SendDeclineResponse();
        HideWindow();
    }

    private void SendAcceptResponse()
    {
        // Отправляем пакет подтверждения торговли
        ZoneCommands.Send(new AnswerTradeRequestCommand(1, _senderId));

        OnTradeResponse?.Invoke(true);
    }

    private void SendDeclineResponse()
    {
        // Отправляем пакет отклонения торговли
        ZoneCommands.Send(new AnswerTradeRequestCommand(0, _senderId));

        OnTradeResponse?.Invoke(false);
    }

    public override void HideWindow()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }

        base.HideWindow();
    }

    private void OnDestroy()
    {
        _instance = null;

        // Отписываемся от событий
        if (_confirmButton != null)
            _confirmButton.UnregisterCallback<ClickEvent>(OnConfirmClicked);
        if (_cancelButton != null)
            _cancelButton.UnregisterCallback<ClickEvent>(OnCancelClicked);
    }
}
