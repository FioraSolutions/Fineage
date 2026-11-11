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
}
