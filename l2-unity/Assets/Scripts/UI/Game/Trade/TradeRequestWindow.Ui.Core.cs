using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeRequestWindow : L2PopupWindow
{
    private void ResetProgressBar()
    {
        _progressBar.style.width = Length.Percent(100);
    }

    private void StartCountdown()
    {
        if (_countdownCoroutine != null)
            StopCoroutine(_countdownCoroutine);

        _countdownCoroutine = StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        float currentTime = _timeoutSeconds;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            float progress = currentTime / _timeoutSeconds;

            // Обновляем прогресс-бар
            _progressBar.style.width = Length.Percent(progress * 100);

            yield return null;
        }

        OnTimeout();
    }

    private void OnConfirmClicked(ClickEvent evt)
    {
        SendAcceptResponse();
        HideWindow();
    }

    private void OnCancelClicked(ClickEvent evt)
    {
        SendDeclineResponse();
        HideWindow();
    }
}
