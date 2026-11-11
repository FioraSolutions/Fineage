using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class CharSelectWindow : L2Window
{
    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                ReLoginPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                StartGamePressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharSelectWindow"); 
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }
}
