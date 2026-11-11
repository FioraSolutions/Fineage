using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class PartyInvitationWindow : L2PopupWindow
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

    private void OnYesClicked(ClickEvent evt)
    {
        SendAcceptResponse();
        HideWindow();
    }

    private void OnNoClicked(ClickEvent evt)
    {
        SendDeclineResponse();
        HideWindow();
    }

    private void OnTimeout()
    {
        SendDeclineResponse();
        HideWindow();
    }

    private void SendAcceptResponse()
    {
        ZoneCommands.Send(new RequestAnswerJoinPartyCommand(1, _requestorObjectId));

        OnInvitationResponse?.Invoke(true);
    }

    private void SendDeclineResponse()
    {
        ZoneCommands.Send(new RequestAnswerJoinPartyCommand(0, _requestorObjectId));

        OnInvitationResponse?.Invoke(false);
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
        if (_yesButton != null)
            _yesButton.UnregisterCallback<ClickEvent>(OnYesClicked);
        if (_noButton != null)
            _noButton.UnregisterCallback<ClickEvent>(OnNoClicked);
    }
}
