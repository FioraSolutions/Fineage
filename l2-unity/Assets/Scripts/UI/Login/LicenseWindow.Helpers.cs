using System;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class LicenseWindow : L2Window
{
    private IEnumerator AutoAgreeWhenShown() {
        yield return null;
        yield return null;
        if (!_isWindowHidden)
            AgreeButtonPressed();
    }

    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                DisagreeButtonPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                AgreeButtonPressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/LicenseWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button agreeButton = (Button)GetElementById("AgreeButton");
        agreeButton.AddManipulator(new ButtonClickSoundManipulator(agreeButton));
        agreeButton.RegisterCallback<ClickEvent>(evt => AgreeButtonPressed());

        Button disagreeButton = (Button)GetElementById("DisagreeButton");
        disagreeButton.AddManipulator(new ButtonClickSoundManipulator(disagreeButton));
        disagreeButton.RegisterCallback<ClickEvent>(evt => DisagreeButtonPressed());

        var highBtn = (RepeatButton)GetElementById("unity-high-button");
        var lowBtn = (RepeatButton)GetElementById("unity-low-button");
        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));
    }

    private void AgreeButtonPressed() {
        // The Interlude license handshake (Init → GGAuth → LoginOk → RequestServerList) is gone:
        // Fineage Net authenticates on the zone socket and has no license step. This window is
        // unreachable from the Net lobby, so just record the click.
        LobbyFlowLog.Warn("LicenseWindow: legacy Interlude license handshake has no Net equivalent");
    }

    private void DisagreeButtonPressed() {
        IZoneTransport transport = Transport;
        if (transport != null)
            transport.Disconnect();
    }
}
