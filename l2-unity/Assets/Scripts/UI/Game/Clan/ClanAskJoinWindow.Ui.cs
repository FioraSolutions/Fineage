using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanAskJoinWindow : L2PopupWindow
{
    public override void ShowWindow()
    {
        base.ShowWindow();
        if (_progressBar != null)
            _progressBar.style.width = Length.Percent(100);
        if (_countdown != null)
            StopCoroutine(_countdown);
        _countdown = StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        float left = TimeoutSeconds;
        while (left > 0f)
        {
            left -= Time.deltaTime;
            if (_progressBar != null)
                _progressBar.style.width = Length.Percent(Mathf.Clamp01(left / TimeoutSeconds) * 100f);
            yield return null;
        }

        Send(0);
        HideWindow();
    }

    void OnYesClicked(ClickEvent evt)
    {
        Send(1);
        HideWindow();
    }

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
