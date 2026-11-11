using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ServerSelectWindow : L2Window
{
    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/ServerList/ServerListWindow");
        _serverElementTemplate = LoadAsset("Data/UI/_Elements/Login/ServerList/ServerElement");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        if (_windowEle == null)
        {
            Debug.LogError("[ServerSelect] BuildWindow: _windowEle null after InitWindow");
            yield break;
        }

        // Absolute ButtonBg/Label children steal picks unless Ignore — clicks never reach Button.
        foreach (var child in _windowEle.Query(className: "l2-button-bg").ToList())
            child.pickingMode = PickingMode.Ignore;
        foreach (var child in _windowEle.Query(className: "button-label").ToList())
            child.pickingMode = PickingMode.Ignore;

        Button confirmButton = _windowEle.Q<Button>("ConfirmButton");
        if (confirmButton != null)
        {
            confirmButton.AddManipulator(new ButtonClickSoundManipulator(confirmButton));
            confirmButton.RegisterCallback<ClickEvent>(evt => ConfirmButtonPressed());
        }
        else
            Debug.LogError("[ServerSelect] ConfirmButton missing in UXML");

        Button cancelButton = _windowEle.Q<Button>("CancelButton");
        if (cancelButton != null)
        {
            cancelButton.AddManipulator(new ButtonClickSoundManipulator(cancelButton));
            cancelButton.RegisterCallback<ClickEvent>(evt => CancelButtonPressed());
        }

        Button nameButton = _windowEle.Q<Button>("Name");
        if (nameButton != null)
            nameButton.AddManipulator(new ButtonClickSoundManipulator(nameButton));
        Button trafficButton = _windowEle.Q<Button>("Traffic");
        if (trafficButton != null)
            trafficButton.AddManipulator(new ButtonClickSoundManipulator(trafficButton));
        Button characterButton = _windowEle.Q<Button>("Character");
        if (characterButton != null)
            characterButton.AddManipulator(new ButtonClickSoundManipulator(characterButton));

        _listContentContainer = _windowEle.Q<VisualElement>("ServerContainer");

        _serverElements = new List<VisualElement>();

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }
}
