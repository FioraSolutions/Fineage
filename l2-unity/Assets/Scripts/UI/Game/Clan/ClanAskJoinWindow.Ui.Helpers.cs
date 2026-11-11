using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanAskJoinWindow : L2PopupWindow
{
    void OnNoClicked(ClickEvent evt)
    {
        Send(0);
        HideWindow();
    }

    void Send(int response)
    {
        if (_requestorObjectId <= 0)
            return;
        if (ZoneCommands.IsConnected)
            ZoneCommands.Send(new ClanAnswerJoinRequestCommand(_requestorObjectId, response));
        _requestorObjectId = 0;
    }

    public override void HideWindow()
    {
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }

        base.HideWindow();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        if (_yesButton != null)
            _yesButton.UnregisterCallback<ClickEvent>(OnYesClicked);
        if (_noButton != null)
            _noButton.UnregisterCallback<ClickEvent>(OnNoClicked);
    }
}
